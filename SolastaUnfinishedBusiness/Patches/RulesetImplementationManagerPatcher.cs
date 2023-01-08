using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.Models;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterClassDefinitions;

namespace SolastaUnfinishedBusiness.Patches;

public static class RulesetImplementationManagerPatcher
{
    [HarmonyPatch(typeof(RulesetImplementationManager), "InstantiateEffectInvocation")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class InstantiateEffectInvocation_Patch
    {
        public static void Postfix(
            RulesetImplementationManager __instance,
            RulesetEffectSpell __result,
            RulesetCharacter caster,
            RulesetInvocation invocation,
            bool delayRegistration,
            int subspellIndex)
        {
            //PATCH: setup repertoire for spells cast through invocation
            __result.spellRepertoire ??= invocation.invocationRepertoire;
        }
    }

    [HarmonyPatch(typeof(RulesetImplementationManager), "ApplySummonForm")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class ApplySummonForm_Patch
    {
        public static bool Prefix(
            EffectForm effectForm,
            RulesetImplementationDefinitions.ApplyFormsParams formsParams)
        {
            //PATCH: track item that is summoned to inventory
            //usually only items summoned to equipment slots are tracked
            //this code tracks item if it is single item summon and item is marked to be tracked
            //used to properly track items summoned by Inventor

            var summonForm = effectForm.SummonForm;

            if (summonForm.SummonType != SummonForm.Type.InventoryItem
                || summonForm.ItemDefinition == null
                || summonForm.Number != 1
                || !summonForm.TrackItem
                || formsParams.targetType != RuleDefinitions.TargetType.Self
                || formsParams.sourceCharacter is not RulesetCharacterHero)
            {
                return true;
            }

            var rulesetItem = ServiceRepository.GetService<IRulesetItemFactoryService>()
                .CreateStandardItem(summonForm.ItemDefinition);

            rulesetItem.SourceSummoningEffectGuid = formsParams.activeEffect.Guid;

            formsParams.sourceCharacter.GrantItem(rulesetItem, false);
            formsParams.activeEffect.TrackSummonedItem(rulesetItem);
            formsParams.sourceCharacter.RefreshAll();

            return false;
        }
    }

    //PATCH:
    // Call parts of the stuff `RulesetImplementationManagerLocation` does for `RulesetImplementationManagerCampaign`
    // This makes light and item effects correctly terminate when resting during world travel
    // The code is prettified decompiled code from `RulesetImplementationManagerLocation`
    [HarmonyPatch(typeof(RulesetImplementationManager), "TerminateEffect")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class TerminateEffect_Patch
    {
        public static void Prefix(RulesetImplementationManager __instance, RulesetEffect activeEffect)
        {
            //PATCH: allows for extra careful tracking of summoned items
            //removes tracked items from any character, container or loot pile
            //used for Inventor's item summoning
            ExtraCarefulTrackedItem.Process(__instance, activeEffect);
        }

        public static void Postfix(RulesetImplementationManager __instance, RulesetEffect activeEffect)
        {
            if (__instance is not RulesetImplementationManagerCampaign)
            {
                return;
            }

            if (activeEffect is { TrackedLightSourceGuids.Count: > 0 })
            {
                var service = ServiceRepository.GetService<IGameLocationVisibilityService>();

                foreach (var trackedLightSourceGuid in activeEffect.TrackedLightSourceGuids)
                {
                    var rulesetLightSource = (RulesetLightSource)null;
                    ref var local = ref rulesetLightSource;

                    if (!RulesetEntity.TryGetEntity(trackedLightSourceGuid, out local) || rulesetLightSource == null)
                    {
                        continue;
                    }

                    rulesetLightSource.LightSourceExtinguished -= activeEffect.LightSourceExtinguished;

                    RulesetCharacter bearer;

                    if (rulesetLightSource.TargetItemGuid != 0UL &&
                        RulesetEntity.TryGetEntity(rulesetLightSource.TargetItemGuid, out RulesetItem rulesetItem))
                    {
                        if (RulesetEntity.TryGetEntity(rulesetItem.BearerGuid, out bearer) &&
                            bearer is { CharacterInventory: { } })
                        {
                            bearer.CharacterInventory.ItemAltered?.Invoke(bearer.CharacterInventory,
                                bearer.CharacterInventory.FindSlotHoldingItem(rulesetItem), rulesetItem);
                        }

                        var fromActor = GameLocationCharacter.GetFromActor(bearer);

                        service?.RemoveCharacterLightSource(fromActor, rulesetItem.RulesetLightSource);
                        rulesetItem.RulesetLightSource?.Unregister();
                        rulesetItem.RulesetLightSource = null;
                    }
                    else if (rulesetLightSource.TargetGuid != 0UL &&
                             RulesetEntity.TryGetEntity(rulesetLightSource.TargetGuid, out bearer))
                    {
                        var fromActor = GameLocationCharacter.GetFromActor(bearer);

                        service?.RemoveCharacterLightSource(fromActor, rulesetLightSource);

                        if (rulesetLightSource.UseSpecificLocationPosition)
                        {
                            if (bearer is RulesetCharacterEffectProxy proxy)
                            {
                                proxy.RemoveAdditionalPersonalLightSource(rulesetLightSource);
                            }
                        }
                        else if (bearer != null)
                        {
                            bearer.PersonalLightSource = null;
                        }
                    }
                }

                activeEffect.TrackedLightSourceGuids.Clear();
            }

            if (activeEffect is not { TrackedItemPropertyGuids.Count: > 0 })
            {
                return;
            }

            foreach (var itemPropertyGuid in activeEffect.TrackedItemPropertyGuids)
            {
                var rulesetItemProperty = (RulesetItemProperty)null;
                ref var local = ref rulesetItemProperty;

                if (!RulesetEntity.TryGetEntity(itemPropertyGuid, out local) || rulesetItemProperty == null)
                {
                    continue;
                }

                if (!RulesetEntity.TryGetEntity(rulesetItemProperty.TargetItemGuid,
                        out RulesetItem rulesetItem) || rulesetItem == null)
                {
                    continue;
                }

                rulesetItem.ItemPropertyRemoved -= activeEffect.ItemPropertyRemoved;
                rulesetItem.RemoveDynamicProperty(rulesetItemProperty);

                if (!RulesetEntity.TryGetEntity(rulesetItem.BearerGuid,
                        out RulesetCharacter rulesetItemBearer) || rulesetItemBearer == null)
                {
                    continue;
                }

                var characterInventory = rulesetItemBearer.CharacterInventory;

                characterInventory?.ItemAltered?.Invoke(characterInventory,
                    characterInventory.FindSlotHoldingItem(rulesetItem),
                    rulesetItem);

                rulesetItemBearer.RefreshAll();
            }
        }
    }

    //PATCH: handles Sorcerer wildshape scenarios / enforces sorcerer class level / correctly handle slots recovery scenarios
    [HarmonyPatch(typeof(RulesetImplementationManager), "ApplySpellSlotsForm")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class ApplySpellSlotsForm_Patch
    {
        public static bool Prefix(EffectForm effectForm,
            RulesetImplementationDefinitions.ApplyFormsParams formsParams)
        {
            var originalHero = formsParams.sourceCharacter as RulesetCharacterHero;
            var substituteHero =
                originalHero ?? formsParams.sourceCharacter.OriginalFormCharacter as RulesetCharacterHero;

            // this shouldn't happen so passing the problem back to original game code
            if (substituteHero == null)
            {
                return true;
            }

            // patch is only required for Wildshape Heroes or Multiclassed ones
            if (!SharedSpellsContext.IsMulticaster(originalHero))
            {
                return true;
            }

            var spellSlotsForm = effectForm.SpellSlotsForm;

            switch (spellSlotsForm.Type)
            {
                case SpellSlotsForm.EffectType.RecoverHalfLevelUp
                    when SharedSpellsContext.RecoverySlots.TryGetValue(formsParams.activeEffect.Name,
                        out var invokerClass) && invokerClass is CharacterClassDefinition characterClassDefinition:
                {
                    foreach (var spellRepertoire in substituteHero.SpellRepertoires)
                    {
                        var currentValue = 0;

                        if (spellRepertoire.SpellCastingClass == characterClassDefinition)
                        {
                            currentValue = substituteHero.ClassesAndLevels[characterClassDefinition];
                        }
                        else if (spellRepertoire.SpellCastingSubclass != null)
                        {
                            var characterClass = substituteHero.ClassesAndSubclasses
                                .FirstOrDefault(x => x.Value == spellRepertoire.SpellCastingSubclass).Key;

                            if (characterClass == characterClassDefinition)
                            {
                                currentValue = substituteHero.ClassesAndLevels[characterClassDefinition];
                            }
                        }

                        if (currentValue <= 0)
                        {
                            continue;
                        }

                        var slotsCapital = (currentValue % 2) + (currentValue / 2);

                        Gui.GuiService.GetScreen<SlotRecoveryModal>()
                            .ShowSlotRecovery(substituteHero, formsParams.activeEffect.SourceDefinition.Name,
                                spellRepertoire, slotsCapital, spellSlotsForm.MaxSlotLevel);

                        break;
                    }

                    break;
                }
                //
                // handles Sorcerer and Wildshape scenarios slots / points scenarios
                //
                case SpellSlotsForm.EffectType.CreateSpellSlot or SpellSlotsForm.EffectType.CreateSorceryPoints:
                {
                    var spellRepertoire = substituteHero.SpellRepertoires.Find(sr => sr.SpellCastingClass == Sorcerer);

                    Gui.GuiService.GetScreen<FlexibleCastingModal>().ShowFlexibleCasting(substituteHero,
                        spellRepertoire,
                        spellSlotsForm.Type == SpellSlotsForm.EffectType.CreateSpellSlot);
                    break;
                }
                case SpellSlotsForm.EffectType.GainSorceryPoints:
                    formsParams.sourceCharacter.GainSorceryPoints(spellSlotsForm.SorceryPointsGain);
                    break;
                case SpellSlotsForm.EffectType.RecovererSorceryHalfLevelUp:
                {
                    var currentValue = substituteHero.ClassesAndLevels[Sorcerer];
                    var sorceryPointsGain = (currentValue % 2) + (currentValue / 2);

                    formsParams.sourceCharacter.GainSorceryPoints(sorceryPointsGain);
                    break;
                }
                case SpellSlotsForm.EffectType.RechargePower when formsParams.targetCharacter is RulesetCharacter:
                {
                    foreach (var usablePower in substituteHero.UsablePowers.Where(usablePower =>
                                 usablePower.PowerDefinition == spellSlotsForm.PowerDefinition))
                    {
                        usablePower.Recharge();
                    }

                    break;
                }
            }

            return false;
        }
    }

    [HarmonyPatch(typeof(RulesetImplementationManager), "IsValidContextForRestrictedContextProvider")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class IsValidContextForRestrictedContextProvider_Patch
    {
        [NotNull]
        public static IEnumerable<CodeInstruction> Transpiler([NotNull] IEnumerable<CodeInstruction> instructions)
        {
            //PATCH: support for shield counting as melee
            //replaces calls to ItemDefinition's isWeapon and WeaponDescription getter with custom ones that account for shield
            var weaponDescription = typeof(ItemDefinition).GetMethod("get_WeaponDescription");
            var isWeapon = typeof(ItemDefinition).GetMethod("get_IsWeapon");
            var customWeaponDescription =
                new Func<ItemDefinition, WeaponDescription>(ShieldAttack.CustomWeaponDescription).Method;
            var customIsWeapon = new Func<ItemDefinition, bool>(ShieldAttack.CustomIsWeapon).Method;

            return instructions
                .ReplaceCalls(weaponDescription,
                    "RulesetImplementationManager.IsValidContextForRestrictedContextProvider.WeaponDescription",
                    new CodeInstruction(OpCodes.Call, customWeaponDescription))
                .ReplaceCalls(isWeapon,
                    "RulesetImplementationManager.IsValidContextForRestrictedContextProvider.IsWeapon",
                    new CodeInstruction(OpCodes.Call, customIsWeapon));
        }

        public static void Postfix(ref bool __result,
            IRestrictedContextProvider provider,
            RulesetCharacter character,
            ItemDefinition itemDefinition,
            bool rangedAttack,
            RulesetAttackMode attackMode,
            RulesetEffect rulesetEffect)
        {
            //PATCH: support for `IRestrictedContextValidator` feature
            __result = RestrictedContextValidatorPatch.ModifyResult(__result, provider, character, itemDefinition,
                rangedAttack, attackMode, rulesetEffect);
        }
    }

    [HarmonyPatch(typeof(RulesetImplementationManager), "TryRollSavingThrow")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class TryRollSavingThrow_Patch
    {
        public static void Prefix(RulesetCharacter caster, ref int saveDC, BaseDefinition sourceDefinition)
        {
            //BUGFIX: for still an unknown reason we get DC 0 on some Grenadier powers so hack it here (MULTIPLAYER)
            if (saveDC != 0 || sourceDefinition is not FeatureDefinition featureDefinition)
            {
                return;
            }

            var spellRepertoire = caster.GetClassSpellRepertoire();

            if (spellRepertoire == null)
            {
                return;
            }

            // if I try to use this then I get an exception on our patch on ComputeSaveDC ???
            // saveDC = caster.ComputeSaveDC(repertoire);

            saveDC = 8 + caster.TryGetAttributeValue("ProficiencyBonus") +
                     AttributeDefinitions.ComputeAbilityScoreModifier(
                         caster.TryGetAttributeValue(spellRepertoire.SpellCastingAbility));
        }
    }

// use to debug MP issue with grenadier
#if false
    public static bool Prefix(
        RulesetImplementationManager __instance,
        RulesetCharacter caster,
        RuleDefinitions.Side sourceSide,
        RulesetActor target,
        ActionModifier actionModifier,
        bool hasHitVisual,
        bool hasSavingThrow,
        string savingThrowAbility,
        ref int saveDC,
        bool disableSavingThrowOnAllies,
        bool advantageForEnemies,
        bool ignoreCover,
        RuleDefinitions.FeatureSourceType featureSourceType,
        List<EffectForm> effectForms,
        List<SaveAffinityBySenseDescription> savingThrowAffinitiesBySense,
        List<SaveAffinityByFamilyDescription> savingThrowAffinitiesByFamily,
        string sourceName,
        BaseDefinition sourceDefinition,
        string schoolOfMagic,
        MetamagicOptionDefinition metamagicOption,
        out RuleDefinitions.RollOutcome saveOutcome,
        out int saveOutcomeDelta,
        ref bool __result)
    {
        var flag1 = false;
        saveOutcome = RuleDefinitions.RollOutcome.Failure;
        saveOutcomeDelta = 0;
        var sourceFamily = caster != null ? caster.CharacterFamily : string.Empty;
        if (caster != null && sourceDefinition != null)
        {
            caster.EnumerateFeaturesToBrowse<IEffectAffinityProvider>(caster.FeaturesToBrowse);
            foreach (IEffectAffinityProvider affinityProvider in caster.FeaturesToBrowse)
            {
                switch (affinityProvider.AddBonusToEffectSaveDC)
                {
                    case SpellAndPowersDefinitions.RulesetEffectSaveDCBonusType.Spell:
                        if (sourceDefinition == affinityProvider.SpellWithModifiedSaveDC)
                        {
                            break;
                        }

                        continue;
                    case SpellAndPowersDefinitions.RulesetEffectSaveDCBonusType.Power:
                        if (!(sourceDefinition == affinityProvider.PowerWithModifiedSaveDC))
                        {
                            continue;
                        }

                        break;
                    default:
                        continue;
                }

                saveDC += affinityProvider.BonusToEffectSaveDC;
            }
        }

        var flag2 = false;
        foreach (var effectForm in effectForms)
        {
            if (effectForm.OverrideSavingThrowInfo != null)
            {
                flag2 = true;
                savingThrowAbility = effectForm.OverrideSavingThrowInfo.SavingThrowAbility;
                saveDC = effectForm.OverrideSavingThrowInfo.SavingThrowDC;
                break;
            }
        }

        if (hasSavingThrow | flag2 && target != null)
        {
            flag1 = true;
            if (disableSavingThrowOnAllies && sourceSide == target.Side)
            {
                flag1 = false;
            }
            else if (metamagicOption != null &&
                     metamagicOption.Type == RuleDefinitions.MetamagicType.CarefulSpell &&
                     sourceSide == target.Side)
            {
                flag1 = true;
                saveOutcome = RuleDefinitions.RollOutcome.Success;
            }
            else if (!target.IsAutomaticallyFailingSavingThrow(savingThrowAbility))
            {
                var actionModifier1 =
                    actionModifier == null ? new ActionModifier() : actionModifier.Clone();
                target.EnumerateFeaturesToBrowse<ISavingThrowAffinityProvider>(target.FeaturesToBrowse);
                foreach (ISavingThrowAffinityProvider feature in target.FeaturesToBrowse)
                {
                    if (!string.IsNullOrEmpty(feature.PriorityAbilityScore) &&
                        feature.PriorityAbilityScore != savingThrowAbility)
                    {
                        var abilityScoreModifier =
                            AttributeDefinitions.ComputeAbilityScoreModifier(target.GetAttribute(savingThrowAbility)
                                .CurrentValue);
                        if (AttributeDefinitions.ComputeAbilityScoreModifier(target
                                .GetAttribute(feature.PriorityAbilityScore).CurrentValue) > abilityScoreModifier)
                        {
                            var abilityScoreForSave =
                                target.ReplacedAbilityScoreForSave;
                            if (abilityScoreForSave != null)
                            {
                                abilityScoreForSave(target, feature as FeatureDefinition, savingThrowAbility,
                                    feature.PriorityAbilityScore);
                            }

                            savingThrowAbility = feature.PriorityAbilityScore;
                        }
                    }
                }

                actionModifier1.SavingThrowModifierTrends.Clear();
                var savingThrowBonus = target.ComputeBaseSavingThrowBonus(savingThrowAbility,
                    actionModifier1.SavingThrowModifierTrends);
                foreach (var effectForm in effectForms)
                {
                    var damageType = effectForm.FormType == EffectForm.EffectFormType.Damage
                        ? effectForm.DamageForm.DamageType
                        : string.Empty;
                    var conditionType = string.Empty;
                    if (effectForm.FormType == EffectForm.EffectFormType.Condition &&
                        effectForm.ConditionForm.ConditionDefinition != null)
                    {
                        conditionType = effectForm.ConditionForm.ConditionDefinition.Name;
                    }

                    var savingThrowContext = __instance.ComputeCharacterSavingThrowContext(target);
                    target.ComputeSavingThrowModifier(savingThrowAbility, effectForm.FormType,
                        sourceDefinition != null ? sourceDefinition.Name : string.Empty,
                        schoolOfMagic, damageType, conditionType, sourceFamily, actionModifier1,
                        __instance.accountedProviders, savingThrowContext);
                    caster?.ComputeSavingThrowModifierImposedOnTarget(sourceName, actionModifier1, metamagicOption);
                }

                __instance.accountedProviders.Clear();
                if (advantageForEnemies &&
                    ((target.Side == RuleDefinitions.Side.Enemy && sourceSide == RuleDefinitions.Side.Ally) ||
                     (target.Side == RuleDefinitions.Side.Ally && sourceSide == RuleDefinitions.Side.Enemy)))
                {
                    actionModifier1.SavingThrowAdvantageTrends.Add(
                        new RuleDefinitions.TrendInfo(1, featureSourceType, sourceName, caster));
                }

                if (target is RulesetCharacter rulesetCharacter)
                {
                    if (savingThrowAffinitiesBySense != null)
                    {
                        foreach (var senseDescription in savingThrowAffinitiesBySense)
                        {
                            if (senseDescription.AdvantageType != RuleDefinitions.AdvantageType.None &&
                                rulesetCharacter.HasSenseType(senseDescription.SenseType))
                            {
                                actionModifier1.SavingThrowAdvantageTrends.Add(
                                    new RuleDefinitions.TrendInfo(
                                        senseDescription.AdvantageType == RuleDefinitions.AdvantageType.Advantage
                                            ? 1
                                            : -1, featureSourceType, sourceName, caster));
                                break;
                            }
                        }
                    }

                    if (rulesetCharacter is RulesetCharacterMonster monster &&
                        savingThrowAffinitiesByFamily != null)
                    {
                        var monsterDefinition = monster.MonsterDefinition;
                        foreach (var familyDescription in savingThrowAffinitiesByFamily)
                        {
                            if (familyDescription.AdvantageType != RuleDefinitions.AdvantageType.None &&
                                monsterDefinition.CharacterFamily == familyDescription.Family)
                            {
                                actionModifier1.SavingThrowAdvantageTrends.Add(
                                    new RuleDefinitions.TrendInfo(
                                        familyDescription.AdvantageType == RuleDefinitions.AdvantageType.Advantage
                                            ? 1
                                            : -1, featureSourceType, sourceName, caster));
                                break;
                            }
                        }
                    }
                }


                target.RollSavingThrow(savingThrowBonus, savingThrowAbility, sourceDefinition,
                    actionModifier1.SavingThrowModifierTrends, actionModifier1.SavingThrowAdvantageTrends,
                    actionModifier1.GetSavingThrowModifier(savingThrowAbility, ignoreCover), saveDC, hasHitVisual,
                    out saveOutcome, out saveOutcomeDelta);
            }
        }

        __result = flag1;

        return false;
    }
#endif
}
