using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.CustomValidators;
using SolastaUnfinishedBusiness.Models;
using UnityEngine;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterClassDefinitions;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class RulesetImplementationManagerPatcher
{
    private static void EnumerateFeatureDefinitionSavingThrowAffinity(
        RulesetCharacter __instance,
        List<FeatureDefinition> featuresToBrowse,
        Dictionary<FeatureDefinition, RuleDefinitions.FeatureOrigin> featuresOrigin)
    {
        __instance.EnumerateFeaturesToBrowse<FeatureDefinitionSavingThrowAffinity>(featuresToBrowse, featuresOrigin);
        featuresToBrowse.RemoveAll(x =>
            !__instance.IsValid(x.GetAllSubFeaturesOfType<IsCharacterValidHandler>()));
    }

    [HarmonyPatch(typeof(RulesetImplementationManager),
        nameof(RulesetImplementationManager.InstantiateEffectInvocation))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class InstantiateEffectInvocation_Patch
    {
        [UsedImplicitly]
        public static void Postfix(
            RulesetEffectSpell __result,
            RulesetInvocation invocation)
        {
            //PATCH: setup repertoire for spells cast through invocation
            __result.spellRepertoire ??= invocation.invocationRepertoire;
        }
    }

    //PATCH: allow different algorithms to calculate critical damage
    [HarmonyPatch(typeof(RulesetImplementationManager), nameof(RulesetImplementationManager.ApplyDamageForm))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class ApplyDamageForm_Patch
    {
        // add the sum of the max damage dice of your attack to your total bonus
        private static int RollDamageOption1(
            RulesetActor rulesetActor,
            DamageForm damageForm,
            int addDice,
            int additionalDamage,
            int damageRollReduction,
            float damageMultiplier,
            bool useVersatileDamage,
            bool attackModeDamage,
            List<int> rolledValues,
            bool canRerollDice)
        {
            var diceType = useVersatileDamage ? damageForm.VersatileDieType : damageForm.DieType;
            var diceMaxValue = RuleDefinitions.DiceMaxValue[(int)damageForm.dieType];

            if (damageForm.OverrideWithBardicInspirationDie && rulesetActor is RulesetCharacterHero hero &&
                hero.GetBardicInspirationDieValue() != RuleDefinitions.DieType.D1)
            {
                diceType = hero.GetBardicInspirationDieValue();
            }

            var totalDamage = rulesetActor.RollDiceAndSum(
                diceType,
                attackModeDamage
                    ? RuleDefinitions.RollContext.AttackDamageValueRoll
                    : RuleDefinitions.RollContext.MagicDamageValueRoll,
                damageForm.DiceNumber + addDice,
                rolledValues, canRerollDice);

            // add additional dices equal with dice max value
            totalDamage += rolledValues.Count * diceMaxValue;
            rolledValues.AddRange(Enumerable.Repeat(diceMaxValue, rolledValues.Count));

            return Mathf.FloorToInt(damageMultiplier *
                                    (Mathf.Clamp(totalDamage + damageForm.BonusDamage - damageRollReduction, 0,
                                        int.MaxValue) + additionalDamage));
        }

        private static int RollDiceKeepRollingMaxAndSum(
            RulesetActor rulesetActor,
            RuleDefinitions.DieType diceType,
            RuleDefinitions.RollContext context,
            int diceNumber,
            ICollection<int> rolledValues = null,
            bool canRerollDice = true,
            string skill = "")
        {
            rulesetActor.EnumerateFeaturesToBrowse<IDieRollModificationProvider>(rulesetActor.featuresToBrowse);

            var maxDie = RuleDefinitions.DiceMaxValue[(int)diceType];
            var total = 0;

            for (var index = 0; index < diceNumber; ++index)
            {
                var roll = maxDie;

                if (maxDie > 1)
                {
                    while (roll == maxDie)
                    {
                        roll = rulesetActor.RollDie(diceType, context, false, RuleDefinitions.AdvantageType.None,
                            out _, out _, false, canRerollDice, skill);
                        rolledValues?.Add(roll);
                        total += roll;
                    }
                }
                else
                {
                    rolledValues?.Add(1);
                    total += 1;
                }
            }

            return total;
        }

        // keep re-rolling any dice that rolls it max value and add it to the total damage
        private static int RollDamageOption2(
            RulesetActor rulesetActor,
            DamageForm damageForm,
            int addDice,
            int additionalDamage,
            int damageRollReduction,
            float damageMultiplier,
            bool useVersatileDamage,
            bool attackModeDamage,
            ICollection<int> rolledValues,
            bool canRerollDice)
        {
            var diceType = useVersatileDamage ? damageForm.VersatileDieType : damageForm.DieType;

            if (damageForm.OverrideWithBardicInspirationDie && rulesetActor is RulesetCharacterHero hero &&
                hero.GetBardicInspirationDieValue() != RuleDefinitions.DieType.D1)
            {
                diceType = hero.GetBardicInspirationDieValue();
            }

            var totalDamage = RollDiceKeepRollingMaxAndSum(rulesetActor,
                diceType,
                attackModeDamage
                    ? RuleDefinitions.RollContext.AttackDamageValueRoll
                    : RuleDefinitions.RollContext.MagicDamageValueRoll,
                (damageForm.DiceNumber + addDice) * 2, // 2 as it's a critical hit
                rolledValues, canRerollDice);

            return Mathf.FloorToInt(damageMultiplier *
                                    (Mathf.Clamp(totalDamage + damageForm.BonusDamage - damageRollReduction, 0,
                                         int.MaxValue) +
                                     additionalDamage));
        }

        // double your initial rolled results instead of rolling additional critical dice
        private static int RollDamageOption3(
            RulesetActor rulesetActor,
            DamageForm damageForm,
            int addDice,
            int additionalDamage,
            int damageRollReduction,
            float damageMultiplier,
            bool useVersatileDamage,
            bool attackModeDamage,
            List<int> rolledValues,
            bool canRerollDice)
        {
            var diceType = useVersatileDamage ? damageForm.VersatileDieType : damageForm.DieType;

            if (damageForm.OverrideWithBardicInspirationDie && rulesetActor is RulesetCharacterHero hero &&
                hero.GetBardicInspirationDieValue() != RuleDefinitions.DieType.D1)
            {
                diceType = hero.GetBardicInspirationDieValue();
            }

            // different than original game code we roll usual dices and multiply result by 2
            var totalDamage = 2 * rulesetActor.RollDiceAndSum(
                diceType,
                attackModeDamage
                    ? RuleDefinitions.RollContext.AttackDamageValueRoll
                    : RuleDefinitions.RollContext.MagicDamageValueRoll,
                damageForm.DiceNumber + addDice,
                rolledValues, canRerollDice);

            // duplicates the rolled dices as well
            rolledValues.AddRange(rolledValues.ToList());

            return Mathf.FloorToInt(damageMultiplier *
                                    (Mathf.Clamp(totalDamage + damageForm.BonusDamage - damageRollReduction, 0,
                                        int.MaxValue) + additionalDamage));
        }

        private static int RollDamage(
            RulesetActor rulesetActor,
            DamageForm damageForm,
            int addDice,
            bool criticalSuccess,
            int additionalDamage,
            int damageRollReduction,
            float damageMultiplier,
            bool useVersatileDamage,
            bool attackModeDamage,
            List<int> rolledValues,
            bool canRerollDice)
        {
            var hero = rulesetActor as RulesetCharacterHero ??
                       (rulesetActor as RulesetCharacter)?.OriginalFormCharacter as RulesetCharacterHero;

            //TODO: make this a proper interface in case we need to support other use cases
            if (hero != null &&
                hero.TrainedFeats.Any(x => x.Name is "FeatPiercerDex" or "FeatPiercerStr") &&
                damageForm.damageType == RuleDefinitions.DamageTypePiercing)
            {
                canRerollDice = true;
            }

            if (!criticalSuccess || hero == null)
            {
                return rulesetActor.RollDamage(
                    damageForm, addDice, criticalSuccess, additionalDamage, damageRollReduction,
                    damageMultiplier, useVersatileDamage, attackModeDamage, rolledValues, canRerollDice);
            }

            return hero.Side switch
            {
                RuleDefinitions.Side.Enemy => Main.Settings.CriticalHitModeEnemies switch
                {
                    1 => RollDamageOption1(rulesetActor, damageForm, addDice, additionalDamage,
                        damageRollReduction, damageMultiplier, useVersatileDamage, attackModeDamage, rolledValues,
                        canRerollDice),
                    2 => RollDamageOption2(rulesetActor, damageForm, addDice, additionalDamage,
                        damageRollReduction, damageMultiplier, useVersatileDamage, attackModeDamage, rolledValues,
                        canRerollDice),
                    3 => RollDamageOption3(rulesetActor, damageForm, addDice, additionalDamage,
                        damageRollReduction, damageMultiplier, useVersatileDamage, attackModeDamage, rolledValues,
                        canRerollDice),
                    _ => rulesetActor.RollDamage(damageForm, addDice, true, additionalDamage,
                        damageRollReduction, damageMultiplier, useVersatileDamage, attackModeDamage, rolledValues,
                        canRerollDice)
                },
                RuleDefinitions.Side.Ally => Main.Settings.CriticalHitModeAllies switch
                {
                    1 => RollDamageOption1(rulesetActor, damageForm, addDice, additionalDamage,
                        damageRollReduction, damageMultiplier, useVersatileDamage, attackModeDamage, rolledValues,
                        canRerollDice),
                    2 => RollDamageOption2(rulesetActor, damageForm, addDice, additionalDamage,
                        damageRollReduction, damageMultiplier, useVersatileDamage, attackModeDamage, rolledValues,
                        canRerollDice),
                    3 => RollDamageOption3(rulesetActor, damageForm, addDice, additionalDamage,
                        damageRollReduction, damageMultiplier, useVersatileDamage, attackModeDamage, rolledValues,
                        canRerollDice),
                    _ => rulesetActor.RollDamage(damageForm, addDice, true, additionalDamage,
                        damageRollReduction, damageMultiplier, useVersatileDamage, attackModeDamage, rolledValues,
                        canRerollDice)
                },
                _ => rulesetActor.RollDamage(
                    damageForm, addDice, true, additionalDamage, damageRollReduction,
                    damageMultiplier, useVersatileDamage, attackModeDamage, rolledValues, canRerollDice)
            };
        }

        [NotNull]
        [UsedImplicitly]
        public static IEnumerable<CodeInstruction> Transpiler([NotNull] IEnumerable<CodeInstruction> instructions)
        {
            var rollDamageMethod = typeof(RulesetActor).GetMethod("RollDamage");
            var myRollDamageMethod =
                new Func<RulesetActor,
                    DamageForm, int, bool, int, int, float, bool, bool, List<int>, bool, int>(
                    RollDamage).Method;

            return instructions.ReplaceCalls(rollDamageMethod,
                "RulesetImplementationManager.ApplyDamageForm",
                new CodeInstruction(OpCodes.Call, myRollDamageMethod));
        }
    }

    [HarmonyPatch(typeof(RulesetImplementationManager), nameof(RulesetImplementationManager.ApplySummonForm))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class ApplySummonForm_Patch
    {
        [UsedImplicitly]
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
    [HarmonyPatch(typeof(RulesetImplementationManager), nameof(RulesetImplementationManager.TerminateEffect))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class TerminateEffect_Patch
    {
        [UsedImplicitly]
        public static void Prefix(RulesetImplementationManager __instance, RulesetEffect activeEffect)
        {
            //PATCH: allows for extra careful tracking of summoned items
            //removes tracked items from any character, container or loot pile
            //used for Inventor's item summoning
            ExtraCarefulTrackedItem.Process(activeEffect);
        }

        [UsedImplicitly]
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
                            bearer is { CharacterInventory: not null })
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
    [HarmonyPatch(typeof(RulesetImplementationManager), nameof(RulesetImplementationManager.ApplySpellSlotsForm))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class ApplySpellSlotsForm_Patch
    {
        [UsedImplicitly]
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

            // patch is only required for Wildshape Heroes or Multiclass ones
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

    [HarmonyPatch(typeof(RulesetImplementationManager),
        nameof(RulesetImplementationManager.IsValidContextForRestrictedContextProvider))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class IsValidContextForRestrictedContextProvider_Patch
    {
        [NotNull]
        [UsedImplicitly]
        public static IEnumerable<CodeInstruction> Transpiler([NotNull] IEnumerable<CodeInstruction> instructions)
        {
            //PATCH: support for shield counting as melee
            //replaces calls to ItemDefinition's isWeapon and WeaponDescription getter with custom ones that account for shield
            var weaponDescription = typeof(ItemDefinition).GetMethod("get_WeaponDescription");
            var isWeapon = typeof(ItemDefinition).GetMethod("get_IsWeapon");
            var customWeaponDescription =
                new Func<ItemDefinition, WeaponDescription>(ShieldAttack.EnhancedWeaponDescription).Method;

            //PATCH: support for AddTagToWeapon
            var weaponTags = typeof(WeaponDescription)
                .GetProperty(nameof(WeaponDescription.WeaponTags))
                ?.GetGetMethod();
            var customWeaponTags = new Func<
                WeaponDescription,
                RulesetCharacter,
                RulesetAttackMode,
                List<string>
            >(AddTagToWeapon.GetCustomWeaponTags).Method;
            var customIsWeapon = new Func<ItemDefinition, bool>(ShieldAttack.IsWeaponOrShield).Method;

            return instructions
                .ReplaceCalls(weaponDescription,
                    "RulesetImplementationManager.IsValidContextForRestrictedContextProvider.WeaponDescription",
                    new CodeInstruction(OpCodes.Call, customWeaponDescription))
                .ReplaceCalls(weaponTags,
                    "RulesetImplementationManager.IsValidContextForRestrictedContextProvider.WeaponTags",
                    new CodeInstruction(OpCodes.Ldarg_2),
                    new CodeInstruction(OpCodes.Ldarg, 5),
                    new CodeInstruction(OpCodes.Call, customWeaponTags))
                .ReplaceCalls(isWeapon,
                    "RulesetImplementationManager.IsValidContextForRestrictedContextProvider.IsWeapon",
                    new CodeInstruction(OpCodes.Call, customIsWeapon));
        }

        [UsedImplicitly]
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

    //PATCH: allow ISavingThrowAffinityProvider to be validated with IsCharacterValidHandler
    [HarmonyPatch(typeof(RulesetImplementationManager), nameof(RulesetImplementationManager.TryRollSavingThrow))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class TryRollSavingThrow_Patch
    {
        [UsedImplicitly]
        public static IEnumerable<CodeInstruction> Transpiler([NotNull] IEnumerable<CodeInstruction> instructions)
        {
            var enumerate = new Action<
                RulesetCharacter,
                List<FeatureDefinition>,
                Dictionary<FeatureDefinition, RuleDefinitions.FeatureOrigin>
            >(EnumerateFeatureDefinitionSavingThrowAffinity).Method;

            //PATCH: make ISpellCastingAffinityProvider from dynamic item properties apply to repertoires
            return instructions.ReplaceEnumerateFeaturesToBrowse("ISavingThrowAffinityProvider",
                -1, "RulesetImplementationManager.TryRollSavingThrow",
                new CodeInstruction(OpCodes.Call, enumerate));
        }

        private static void GetBestSavingThrowAbilityScore(RulesetActor rulesetActor, ref string attributeScore)
        {
            var savingThrowBonus =
                AttributeDefinitions.ComputeAbilityScoreModifier(rulesetActor.TryGetAttributeValue(attributeScore)) +
                rulesetActor.ComputeBaseSavingThrowBonus(attributeScore, new List<RuleDefinitions.TrendInfo>());

            var attr = attributeScore;

            foreach (var attribute in rulesetActor
                         .GetSubFeaturesByType<IChangeSavingThrowAttribute>()
                         .Where(x => x.IsValid(rulesetActor, attr))
                         .Select(x => x.SavingThrowAttribute(rulesetActor)))
            {
                var newSavingThrowBonus =
                    AttributeDefinitions.ComputeAbilityScoreModifier(rulesetActor.TryGetAttributeValue(attribute)) +
                    rulesetActor.ComputeBaseSavingThrowBonus(attribute, new List<RuleDefinitions.TrendInfo>());

                // get the last one instead unless we start using this with other subs and then need to decide which one is better
                if (newSavingThrowBonus <= savingThrowBonus)
                {
                    continue;
                }

                attributeScore = attribute;
                savingThrowBonus = newSavingThrowBonus;
            }
        }

        //PATCH: supports IChangeSavingThrowAttribute interface
        [UsedImplicitly]
        public static void Prefix(RulesetActor target, ref string savingThrowAbility)
        {
            GetBestSavingThrowAbilityScore(target, ref savingThrowAbility);
        }

        //PATCH: supports ISavingThrowAfterRoll interface
        [UsedImplicitly]
        public static void Postfix(
            RulesetCharacter caster,
            RuleDefinitions.Side sourceSide,
            RulesetActor target,
            ActionModifier actionModifier,
            bool hasHitVisual,
            bool hasSavingThrow,
            string savingThrowAbility,
            int saveDC,
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
            ref RuleDefinitions.RollOutcome saveOutcome,
            ref int saveOutcomeDelta)
        {
            //PATCH: react to failed saving throw
            foreach (var savingThrowAfterRoll in target.GetSubFeaturesByType<ISavingThrowAfterRoll>())
            {
                savingThrowAfterRoll.OnSavingThrowAfterRoll(
                    caster,
                    sourceSide,
                    target,
                    actionModifier,
                    hasHitVisual,
                    hasSavingThrow,
                    savingThrowAbility,
                    saveDC,
                    disableSavingThrowOnAllies,
                    advantageForEnemies,
                    ignoreCover,
                    featureSourceType,
                    effectForms,
                    savingThrowAffinitiesBySense,
                    savingThrowAffinitiesByFamily,
                    sourceName,
                    sourceDefinition,
                    schoolOfMagic,
                    metamagicOption,
                    ref saveOutcome,
                    ref saveOutcomeDelta);
            }
        }
    }
}
