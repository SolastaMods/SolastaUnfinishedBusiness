using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Behaviors;
using SolastaUnfinishedBusiness.Behaviors.Specific;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Subclasses;
using SolastaUnfinishedBusiness.Validators;
using UnityEngine;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterClassDefinitions;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class RulesetImplementationManagerPatcher
{
    private static void EnumerateFeatureDefinitionSavingThrowAffinity(
        RulesetCharacter __instance,
        List<FeatureDefinition> featuresToBrowse,
        Dictionary<FeatureDefinition, FeatureOrigin> featuresOrigin)
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
            bool maximumDamage,
            bool useVersatileDamage,
            bool attackModeDamage,
            List<int> rolledValues,
            bool canRerollDice)
        {
            var diceType = useVersatileDamage ? damageForm.VersatileDieType : damageForm.DieType;
            var diceMaxValue = DiceMaxValue[(int)damageForm.dieType];

            if (damageForm.OverrideWithBardicInspirationDie && rulesetActor is RulesetCharacterHero hero &&
                hero.GetBardicInspirationDieValue() != DieType.D1)
            {
                diceType = hero.GetBardicInspirationDieValue();
            }

            var totalDamage = rulesetActor.RollDiceAndSum(
                diceType,
                attackModeDamage
                    ? RollContext.AttackDamageValueRoll
                    : RollContext.MagicDamageValueRoll,
                damageForm.DiceNumber + addDice,
                rolledValues, canRerollDice, maximumDamage);

            // add additional dices equal with dice max value
            totalDamage += rolledValues.Count * diceMaxValue;
            rolledValues.AddRange(Enumerable.Repeat(diceMaxValue, rolledValues.Count));

            return Mathf.FloorToInt(damageMultiplier *
                                    (Mathf.Clamp(totalDamage + damageForm.BonusDamage - damageRollReduction, 0,
                                        int.MaxValue) + additionalDamage));
        }

        private static int RollDiceKeepRollingMaxAndSum(
            RulesetActor rulesetActor,
            DieType diceType,
            RollContext context,
            int diceNumber,
            ICollection<int> rolledValues = null,
            bool canRerollDice = true,
            string skill = "")
        {
            rulesetActor.EnumerateFeaturesToBrowse<IDieRollModificationProvider>(rulesetActor.featuresToBrowse);

            var maxDie = DiceMaxValue[(int)diceType];
            var total = 0;

            for (var index = 0; index < diceNumber; ++index)
            {
                var roll = maxDie;

                if (maxDie > 1)
                {
                    while (roll == maxDie)
                    {
                        roll = rulesetActor.RollDie(diceType, context, false, AdvantageType.None,
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
                hero.GetBardicInspirationDieValue() != DieType.D1)
            {
                diceType = hero.GetBardicInspirationDieValue();
            }

            var totalDamage = RollDiceKeepRollingMaxAndSum(rulesetActor,
                diceType,
                attackModeDamage
                    ? RollContext.AttackDamageValueRoll
                    : RollContext.MagicDamageValueRoll,
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
            bool maximumDamage,
            bool useVersatileDamage,
            bool attackModeDamage,
            List<int> rolledValues,
            bool canRerollDice)
        {
            var diceType = useVersatileDamage ? damageForm.VersatileDieType : damageForm.DieType;

            if (damageForm.OverrideWithBardicInspirationDie && rulesetActor is RulesetCharacterHero hero &&
                hero.GetBardicInspirationDieValue() != DieType.D1)
            {
                diceType = hero.GetBardicInspirationDieValue();
            }

            // different than original game code we roll usual dices and multiply result by 2
            var totalDamage = rulesetActor.RollDiceAndSum(
                diceType,
                attackModeDamage
                    ? RollContext.AttackDamageValueRoll
                    : RollContext.MagicDamageValueRoll,
                damageForm.DiceNumber + addDice,
                rolledValues, canRerollDice, maximumDamage);

            // duplicates the rolled dices
            rolledValues.AddRange(rolledValues.ToList());

            // doubles the rolled damage
            damageForm.bonusDamage *= 2;

            return Mathf.FloorToInt(
                damageMultiplier *
                ((2 * totalDamage) + damageForm.BonusDamage - damageRollReduction + additionalDamage));
        }

        private static int RollDamage(
            RulesetActor rulesetActor,
            DamageForm damageForm,
            int addDice,
            bool criticalSuccess,
            int additionalDamage,
            int damageRollReduction,
            float damageMultiplier,
            bool maximumDamage,
            bool useVersatileDamage,
            bool attackModeDamage,
            List<int> rolledValues,
            bool canRerollDice,
            RulesetImplementationDefinitions.ApplyFormsParams formsParams)
        {
            // commented out as causing issues with other features
#if false
            //BUGFIX: allow all damage forms from weapons to allow reroll dice (i.e.: Great Weapon Fighting)
            if (!canRerollDice)
            {
                canRerollDice = formsParams.attackMode != null &&
                                formsParams.attackMode.EffectDescription.EffectForms.Any(
                                    x => x.DamageForm == damageForm);
            }
#endif

            if (rulesetActor is not RulesetCharacter rulesetCharacter)
            {
                return rulesetActor.RollDamage(
                    damageForm, addDice, criticalSuccess, additionalDamage, damageRollReduction, damageMultiplier,
                    maximumDamage, useVersatileDamage, attackModeDamage, rolledValues, canRerollDice);
            }

            if (rulesetCharacter.HasSubFeatureOfType<IAllowRerollDiceOnAllDamageForms>())
            {
                canRerollDice = true;
            }

            var hero = rulesetCharacter.GetOriginalHero();

            //TODO: make this a proper interface in case we need to support other use cases
            if (hero != null &&
                hero.TrainedFeats.Any(x => x.Name is "FeatPiercerDex" or "FeatPiercerStr") &&
                damageForm.damageType == DamageTypePiercing)
            {
                canRerollDice = true;
            }

            //PATCH: supports different critical damage algorithms
            if (!criticalSuccess)
            {
                return rulesetActor.RollDamage(
                    damageForm, addDice, false, additionalDamage, damageRollReduction, damageMultiplier,
                    maximumDamage, useVersatileDamage, attackModeDamage, rolledValues, canRerollDice);
            }

            var rollDamageOption = rulesetActor.Side switch
            {
                Side.Ally => Main.Settings.CriticalHitModeAllies,
                Side.Enemy => Main.Settings.CriticalHitModeEnemies,
                Side.Neutral => Main.Settings.CriticalHitModeNeutral,
                _ => 0
            };

            //TODO: make this an interface in case other features need it
            //PATCH: supports Umbral Stalker level 17th
            if (rulesetActor.HasConditionOfCategoryAndType(
                    AttributeDefinitions.TagEffect, RoguishUmbralStalker.ConditionShadowDanceAdditionalDice.Name))
            {
                rollDamageOption = 2;
            }

            var damage = rollDamageOption switch
            {
                1 => RollDamageOption1(
                    rulesetActor, damageForm, addDice, additionalDamage, damageRollReduction, damageMultiplier,
                    maximumDamage, useVersatileDamage, attackModeDamage, rolledValues, canRerollDice),
                2 => RollDamageOption2(
                    rulesetActor, damageForm, addDice, additionalDamage, damageRollReduction, damageMultiplier,
                    useVersatileDamage, attackModeDamage, rolledValues, canRerollDice),
                3 => RollDamageOption3(
                    rulesetActor, damageForm, addDice, additionalDamage, damageRollReduction, damageMultiplier,
                    maximumDamage, useVersatileDamage, attackModeDamage, rolledValues, canRerollDice),
                _ => rulesetActor.RollDamage(
                    damageForm, addDice, true, additionalDamage, damageRollReduction, damageMultiplier, maximumDamage,
                    useVersatileDamage, attackModeDamage, rolledValues, canRerollDice)
            };

            return damage;
        }

        [NotNull]
        [UsedImplicitly]
        public static IEnumerable<CodeInstruction> Transpiler([NotNull] IEnumerable<CodeInstruction> instructions)
        {
            var rollDamageMethod = typeof(RulesetActor).GetMethod("RollDamage");
            var myRollDamageMethod =
                new Func<RulesetActor,
                    DamageForm, int, bool, int, int, float, bool, bool, bool, List<int>, bool,
                    RulesetImplementationDefinitions.ApplyFormsParams, int>(
                    RollDamage).Method;

            return instructions.ReplaceCalls(rollDamageMethod,
                "RulesetImplementationManager.ApplyDamageForm",
                new CodeInstruction(OpCodes.Ldarg_2),
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
                || formsParams.targetType != TargetType.Self
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


    [HarmonyPatch(typeof(RulesetImplementationManager), nameof(RulesetImplementationManager.TerminateEffect))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class TerminateEffect_Patch
    {
        [UsedImplicitly]
        public static bool Prefix(RulesetImplementationManager __instance, RulesetEffect activeEffect,
            bool showGraphics)
        {
            //PATCH:
            // allows for extra careful tracking of summoned items
            // removes tracked items from any character, container or loot pile
            // used for Inventor's item summoning
            TrackItemsCarefully.Process(activeEffect);

            // vanilla code with enumeration protection
            TerminateEffect(__instance, activeEffect, showGraphics);

            //PATCH:
            // Call parts of the stuff `RulesetImplementationManagerLocation` does for `RulesetImplementationManagerCampaign`
            // This makes light and item effects correctly terminate when resting during world travel
            // The code is prettified decompiled code from `RulesetImplementationManagerLocation`
            TerminateLightAndItemEffectsOnWorldTravel(__instance, activeEffect);

            return false;
        }

        // original vanilla code
        private static void TerminateEffect(
            RulesetImplementationManager __instance, RulesetEffect activeEffect, bool showGraphics = true)
        {
            activeEffect.Terminated = true;
            __instance.ClearDamageFormsByIndex();

            if (activeEffect.TrackedConditionGuids.Count > 0)
            {
                __instance.conditionGuidsToProcess.AddRange(activeEffect.TrackedConditionGuids);

                foreach (var guid in __instance.conditionGuidsToProcess.ToList())
                {
                    RulesetCondition rulesetCondition = null;
                    ref var local = ref rulesetCondition;

                    if (!RulesetEntity.TryGetEntity(guid, out local))
                    {
                        continue;
                    }

                    if (RulesetEntity.TryGetEntity(rulesetCondition.TargetGuid, out RulesetCharacter entity))
                    {
                        entity.ConditionRemoved -= activeEffect.ConditionRemoved;
                    }
                }

                foreach (var guid in __instance.conditionGuidsToProcess.ToList())
                {
                    RulesetCondition rulesetCondition = null;
                    ref var local = ref rulesetCondition;

                    if (!RulesetEntity.TryGetEntity(guid, out local))
                    {
                        continue;
                    }

                    rulesetCondition.EndOfDurationReached = activeEffect.EndOfDurationReached;

                    if (!RulesetEntity.TryGetEntity(rulesetCondition.TargetGuid, out RulesetCharacter entity1))
                    {
                        continue;
                    }

                    entity1.RemoveCondition(rulesetCondition, showGraphics: showGraphics);

                    var entity2 = RulesetEntity.GetEntity<RulesetCharacter>(rulesetCondition.SourceGuid);
                    var conditionTrackingStopped = activeEffect.ConditionTrackingStopped;

                    conditionTrackingStopped?.Invoke(
                        entity2, entity1, activeEffect.EffectDescription, rulesetCondition, showGraphics);
                }

                activeEffect.TrackedConditionGuids.Clear();
                __instance.conditionGuidsToProcess.Clear();
            }

            if (activeEffect.TrackedSummonedItemGuids.Count <= 0)
            {
                return;
            }

            __instance.summonedItemGuidsToProcess.AddRange(activeEffect.TrackedSummonedItemGuids);

            foreach (var guid in __instance.summonedItemGuidsToProcess.ToList())
            {
                RulesetItem rulesetItem = null;
                ref var local = ref rulesetItem;

                if (RulesetEntity.TryGetEntity(guid, out local))
                {
                    rulesetItem.ItemDestroyed -= activeEffect.ItemDestroyed;
                }
            }

            foreach (var guid in __instance.summonedItemGuidsToProcess.ToList())
            {
                RulesetItem itemToLose = null;
                ref var local = ref itemToLose;

                if (!RulesetEntity.TryGetEntity(guid, out local) ||
                    !RulesetEntity.TryGetEntity(activeEffect.SourceGuid, out RulesetCharacter entity))
                {
                    continue;
                }

                entity.LoseItem(itemToLose);
                itemToLose.Unregister();
            }

            activeEffect.TrackedSummonedItemGuids.Clear();
            __instance.summonedItemGuidsToProcess.Clear();
        }

        [UsedImplicitly]
        private static void TerminateLightAndItemEffectsOnWorldTravel(
            RulesetImplementationManager __instance, RulesetEffect activeEffect)
        {
            if (__instance is not RulesetImplementationManagerCampaign)
            {
                return;
            }

            if (activeEffect is { TrackedLightSourceGuids.Count: > 0 })
            {
                var service = ServiceRepository.GetService<IGameLocationVisibilityService>();

                foreach (var trackedLightSourceGuid in activeEffect.TrackedLightSourceGuids.ToList())
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

                        if (fromActor != null)
                        {
                            service?.RemoveCharacterLightSource(fromActor, rulesetItem.RulesetLightSource);
                        }

                        rulesetItem.RulesetLightSource?.Unregister();
                        rulesetItem.RulesetLightSource = null;
                    }
                    else if (rulesetLightSource.TargetGuid != 0UL &&
                             RulesetEntity.TryGetEntity(rulesetLightSource.TargetGuid, out bearer))
                    {
                        var fromActor = GameLocationCharacter.GetFromActor(bearer);

                        if (fromActor != null)
                        {
                            service?.RemoveCharacterLightSource(fromActor, rulesetLightSource);
                        }

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

            foreach (var itemPropertyGuid in activeEffect.TrackedItemPropertyGuids.ToList())
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
        public static bool Prefix(
            EffectForm effectForm,
            RulesetImplementationDefinitions.ApplyFormsParams formsParams)
        {
            var originalHero = formsParams.sourceCharacter?.GetOriginalHero();

            // this shouldn't happen so passing the problem back to original game code
            if (originalHero == null)
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
                    foreach (var spellRepertoire in originalHero.SpellRepertoires)
                    {
                        var currentValue = 0;

                        if (spellRepertoire.SpellCastingClass == characterClassDefinition)
                        {
                            currentValue = originalHero.ClassesAndLevels[characterClassDefinition];
                        }
                        else if (spellRepertoire.SpellCastingSubclass != null)
                        {
                            var characterClass = originalHero.ClassesAndSubclasses
                                .FirstOrDefault(x => x.Value == spellRepertoire.SpellCastingSubclass).Key;

                            if (characterClass == characterClassDefinition)
                            {
                                currentValue = originalHero.ClassesAndLevels[characterClassDefinition];
                            }
                        }

                        if (currentValue <= 0)
                        {
                            continue;
                        }

                        if (ServiceRepository.GetService<IPlayerControllerService>()?
                                .ActivePlayerController?
                                .IsCharacterControlled(originalHero) == false)
                        {
                            break;
                        }

                        var slotsCapital = (currentValue % 2) + (currentValue / 2);

                        Gui.GuiService.GetScreen<SlotRecoveryModal>().ShowSlotRecovery(
                            originalHero,
                            formsParams.activeEffect.SourceDefinition.Name,
                            spellRepertoire,
                            slotsCapital,
                            spellSlotsForm.MaxSlotLevel);

                        break;
                    }

                    break;
                }
                //
                // handles Sorcerer and Wildshape scenarios slots / points scenarios
                //
                case SpellSlotsForm.EffectType.CreateSpellSlot or SpellSlotsForm.EffectType.CreateSorceryPoints:
                {
                    if (ServiceRepository.GetService<IPlayerControllerService>()?
                            .ActivePlayerController?
                            .IsCharacterControlled(originalHero) == false)
                    {
                        break;
                    }

                    var spellRepertoire = originalHero.SpellRepertoires.Find(sr => sr.SpellCastingClass == Sorcerer);

                    Gui.GuiService.GetScreen<FlexibleCastingModal>().ShowFlexibleCasting(
                        originalHero,
                        spellRepertoire,
                        spellSlotsForm.Type == SpellSlotsForm.EffectType.CreateSpellSlot);
                    break;
                }
                case SpellSlotsForm.EffectType.GainSorceryPoints:
                    formsParams.sourceCharacter.GainSorceryPoints(spellSlotsForm.SorceryPointsGain);
                    break;
                case SpellSlotsForm.EffectType.RecovererSorceryHalfLevelUp:
                {
                    var currentValue = originalHero.ClassesAndLevels[Sorcerer];
                    var sorceryPointsGain = (currentValue % 2) + (currentValue / 2);

                    formsParams.sourceCharacter.GainSorceryPoints(sorceryPointsGain);
                    break;
                }
                case SpellSlotsForm.EffectType.RechargePower when formsParams.targetCharacter is RulesetCharacter:
                {
                    foreach (var usablePower in originalHero.UsablePowers.Where(usablePower =>
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
            //PATCH: support for `IValidateContextInsteadOfRestrictedProperty` feature
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
            var rollSavingThrowMethod = typeof(RulesetActor).GetMethod("RollSavingThrow");
            var myRollSavingThrowMethod = typeof(TryRollSavingThrow_Patch).GetMethod("RollSavingThrow");
            //PATCH: make ISpellCastingAffinityProvider from dynamic item properties apply to repertoires
            return instructions
                .ReplaceCalls(rollSavingThrowMethod,
                    "RulesetImplementationManager.TryRollSavingThrow.RollSavingThrow",
                    new CodeInstruction(OpCodes.Ldarg, 1),
                    new CodeInstruction(OpCodes.Ldarg, 13),
                    new CodeInstruction(OpCodes.Call, myRollSavingThrowMethod))
                .ReplaceEnumerateFeaturesToBrowse<ISavingThrowAffinityProvider>(
                    "RulesetImplementationManager.TryRollSavingThrow.EnumerateFeatureDefinitionSavingThrowAffinity",
                    EnumerateFeatureDefinitionSavingThrowAffinity);
        }

        [UsedImplicitly]
        public static void Prefix(
            RulesetCharacter caster,
            RulesetActor target,
            ActionModifier actionModifier,
            bool hasHitVisual,
            bool hasSavingThrow,
            ref string savingThrowAbility,
            int saveDC,
            List<EffectForm> effectForms,
            BaseDefinition sourceDefinition)
        {
            var hasForcedSaving = effectForms.Any(effectForm => effectForm.OverrideSavingThrowInfo != null);

            if (!hasForcedSaving && !hasSavingThrow)
            {
                return;
            }

            //PATCH: supports `OnSavingThrowInitiated` interface
            foreach (var rollSavingThrowInitiated in target.GetSubFeaturesByType<IRollSavingThrowInitiated>())
            {
                rollSavingThrowInitiated.OnSavingThrowInitiated(
                    caster,
                    target as RulesetCharacter,
                    ref savingThrowAbility,
                    sourceDefinition,
                    actionModifier.SavingThrowAdvantageTrends,
                    saveDC,
                    hasHitVisual,
                    effectForms);
            }
        }

        [UsedImplicitly]
        public static void RollSavingThrow(
            RulesetCharacter __instance,
            int saveBonus,
            string abilityScoreName,
            BaseDefinition sourceDefinition,
            List<TrendInfo> modifierTrends,
            List<TrendInfo> advantageTrends,
            int rollModifier,
            int saveDC,
            bool hasHitVisual,
            ref RollOutcome outcome,
            ref int outcomeDelta,
            RulesetCharacter caster,
            List<EffectForm> effectForms)
        {
            __instance.RollSavingThrow(
                saveBonus, abilityScoreName, sourceDefinition, modifierTrends, advantageTrends,
                rollModifier, saveDC, hasHitVisual, out outcome, out outcomeDelta);

            //PATCH: supports `IRollSavingThrowFinished` interface
            foreach (var rollSavingThrowFinished in __instance.GetSubFeaturesByType<IRollSavingThrowFinished>())
            {
                rollSavingThrowFinished.OnSavingThrowFinished(
                    caster,
                    __instance,
                    saveBonus,
                    abilityScoreName,
                    sourceDefinition,
                    modifierTrends,
                    advantageTrends,
                    rollModifier,
                    saveDC,
                    hasHitVisual,
                    ref outcome,
                    ref outcomeDelta,
                    effectForms);
            }
        }

        [UsedImplicitly]
        public static bool Prefix(
            RulesetCharacter caster,
            RulesetActor target,
            //ActionModifier actionModifier,
            ref string savingThrowAbility,
            //List<EffectForm> effectForms,
            BaseDefinition sourceDefinition)
        {
            //PATCH: supports Oath of Ancients / Oath of Dread Path of The Savagery
            OnRollSavingThrowOath(caster, target, sourceDefinition, OathOfAncients.ConditionElderChampionName,
                OathOfAncients.ConditionElderChampionEnemy);
            OnRollSavingThrowOath(caster, target, sourceDefinition, OathOfDread.ConditionAspectOfDreadName,
                OathOfDread.ConditionAspectOfDreadEnemy);
            PathOfTheSavagery.OnRollSavingThrowFuriousDefense(target, ref savingThrowAbility);

            //PATCH: illusionary spells against creatures with True Sight should automatically save
            if (!Main.Settings.IllusionSpellsAutomaticallyFailAgainstTrueSightInRange ||
                sourceDefinition is not
                    SpellDefinition { SchoolOfMagic: SchoolIllusion, EffectDescription.TargetSide: Side.Enemy } ||
                sourceDefinition == DatabaseHelper.SpellDefinitions.Silence)
            {
                return true;
            }

            var glCaster = GameLocationCharacter.GetFromActor(caster);
            var glTarget = GameLocationCharacter.GetFromActor(target);

            if (glCaster == null || glTarget == null)
            {
                return true;
            }

            var senseMode = glTarget.RulesetCharacter.SenseModes
                .FirstOrDefault(x => x.SenseType == SenseMode.Type.Truesight);

            return senseMode == null || !glTarget.IsWithinRange(glCaster, senseMode.SenseRange);
        }

        internal static void OnRollSavingThrowOath(
            RulesetCharacter caster,
            RulesetActor target,
            BaseDefinition sourceDefinition,
            string selfConditionName,
            ConditionDefinition conditionDefinitionEnemy)
        {
            if (caster == null ||
                caster.Side == target.Side ||
                !caster.HasConditionOfCategoryAndType(AttributeDefinitions.TagEffect, selfConditionName))
            {
                return;
            }

            if (sourceDefinition is not SpellDefinition { castingTime: ActivationTime.Action } &&
                sourceDefinition is not FeatureDefinitionPower { RechargeRate: RechargeRate.ChannelDivinity } &&
                !caster.AllConditions.Any(x => x.Name.Contains("Smite")))
            {
                return;
            }

            var gameLocationCaster = GameLocationCharacter.GetFromActor(caster);
            var gameLocationTarget = GameLocationCharacter.GetFromActor(target);

            if (gameLocationCaster == null ||
                gameLocationTarget == null ||
                !gameLocationCaster.IsWithinRange(gameLocationTarget, 2))
            {
                return;
            }

            target.InflictCondition(
                conditionDefinitionEnemy.Name,
                DurationType.Round,
                0,
                TurnOccurenceType.StartOfTurn,
                AttributeDefinitions.TagEffect,
                caster.guid,
                caster.CurrentFaction.Name,
                1,
                conditionDefinitionEnemy.Name,
                0,
                0,
                0);
        }
    }

    [HarmonyPatch(typeof(RulesetImplementationManager), nameof(RulesetImplementationManager.ApplyConditionForm))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class ApplyConditionForm_Patch
    {
        private static readonly List<CodeInstruction> MatchPattern =
        [
            new CodeInstruction(OpCodes.Ldarg_2),
            new CodeInstruction(OpCodes.Ldfld,
                typeof(RulesetImplementationDefinitions.ApplyFormsParams).GetField("activeEffect")),

            new CodeInstruction(OpCodes.Brtrue),
            new CodeInstruction(OpCodes.Ldloc_1),
            new CodeInstruction(OpCodes.Callvirt, typeof(ConditionDefinition).GetMethod("get_SpecialDuration")),
            new CodeInstruction(OpCodes.Brfalse),
            new CodeInstruction(OpCodes.Ldarg_1),
            new CodeInstruction(OpCodes.Callvirt,
                typeof(OverrideSavingThrowInfo).GetMethod("get_OverrideSavingThrowInfo")),

            new CodeInstruction(OpCodes.Brfalse_S)
        ];

        private static bool CompareInstructions(IReadOnlyList<CodeInstruction> codes)
        {
            for (var i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode != MatchPattern[i].opcode)
                {
                    return false;
                }

                if (MatchPattern[i].operand is null || codes[i].OperandIs(MatchPattern[i].operand))
                {
                    continue;
                }

                return false;
            }

            return true;
        }

        // delete the first check of 
        // if (formsParams.activeEffect == null && conditionDefinition.SpecialDuration && effectForm.OverrideSavingThrowInfo != null)
        [UsedImplicitly]
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var flag = false;

            var codes = new List<CodeInstruction>(instructions);

            for (var i = 0; i < codes.Count - MatchPattern.Count + 1; i++)
            {
                if (!CompareInstructions(codes.GetRange(i, MatchPattern.Count)))
                {
                    continue;
                }

                codes[i].opcode = OpCodes.Nop;
                codes.RemoveRange(i + 1, 2);
                flag = true;

                break;
            }

            if (!flag)
            {
                Main.Error("Failed to apply transpiler patch [RulesetImplementationManager.ApplyConditionForm]!");
            }

            return codes.AsEnumerable();
        }
    }

    [HarmonyPatch(typeof(RulesetImplementationManager),
        nameof(RulesetImplementationManager.IsValidContextForAttackModificationProvider))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class IsValidContextForAttackModificationProvider_Patch
    {
        [UsedImplicitly]
        public static bool Prefix(
            RulesetImplementationManager __instance,
            out bool __result,
            IAttackModificationProvider provider,
            RulesetCharacterHero hero,
            ItemDefinition itemDefinition,
            WeaponTypeDefinition weaponTypeDefinition,
            RulesetAttackMode attackMode)
        {
            __result =
                (provider.TriggerCondition != AttackModificationTriggerCondition.NotWearingArmorOrMageArmorOrShield ||
                 (!hero.IsWearingArmor() &&
                  !hero.HasConditionOfTypeOrSubType(ConditionMagicallyArmored) &&
                  (!hero.IsWearingShield() /* BEGIN PATCH */ || hero.HasMonkShieldExpert() /* END PATCH */))) &&
                __instance.IsValidContextForRestrictedContextProvider(
                    provider, hero, itemDefinition, attackMode.Ranged, attackMode, null);

            return false;
        }
    }
}
