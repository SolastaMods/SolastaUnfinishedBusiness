using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.Models;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterClassDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionMagicAffinitys;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class RulesetCharacterPatcher
{
    // helper to get infusions modifiers from items
    private static void EnumerateSpellCastingAffinityProviderFromItems(
        RulesetCharacter __instance,
        List<FeatureDefinition> featuresToBrowse,
        Dictionary<FeatureDefinition,
            RuleDefinitions.FeatureOrigin> featuresOrigin)
    {
        __instance.EnumerateFeaturesToBrowse<ISpellCastingAffinityProvider>(featuresToBrowse, featuresOrigin);

        if (__instance is not RulesetCharacterHero hero)
        {
            return;
        }

        foreach (var definition in hero.CharacterInventory.InventorySlotsByName
                     .Select(keyValuePair => keyValuePair.Value)
                     .Where(slot => slot.EquipedItem != null && !slot.Disabled && !slot.ConfigSlot)
                     .Select(slot => slot.EquipedItem)
                     .SelectMany(equipedItem => equipedItem.DynamicItemProperties
                         .Select(dynamicItemProperty => dynamicItemProperty.FeatureDefinition)
                         .Where(definition => definition != null && definition is ISpellCastingAffinityProvider)))
        {
            featuresToBrowse.Add(definition);

            if (featuresOrigin.ContainsKey(definition))
            {
                continue;
            }

            featuresOrigin.Add(definition, new RuleDefinitions.FeatureOrigin(
                RuleDefinitions.FeatureSourceType.CharacterFeature, definition.Name,
                null, definition.ParseSpecialFeatureTags()));
        }
    }

    [HarmonyPatch(typeof(RulesetCharacter), nameof(RulesetCharacter.IsWieldingMonkWeapon))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class IsWieldingMonkWeapon_Patch
    {
        [UsedImplicitly]
        public static void Postfix(RulesetCharacter __instance, ref bool __result)
        {
            //PATCH: count wild-shaped heroes with monk classes as wielding monk weapons
            if (__instance is not RulesetCharacterMonster ||
                __instance.OriginalFormCharacter is not RulesetCharacterHero hero)
            {
                return;
            }

            __result = hero.GetClassLevel(Monk) > 0;
        }
    }

    [HarmonyPatch(typeof(RulesetCharacter), nameof(RulesetCharacter.OnConditionAdded))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class OnConditionAdded_Patch
    {
        [UsedImplicitly]
        public static void Postfix(RulesetCharacter __instance, RulesetCondition activeCondition)
        {
            var definition = activeCondition.ConditionDefinition;

            //PATCH: notifies custom condition features that condition is applied
            definition.GetAllSubFeaturesOfType<ICustomConditionFeature>()
                .ForEach(c => c.ApplyFeature(__instance, activeCondition));

            definition.Features
                .SelectMany(f => f.GetAllSubFeaturesOfType<ICustomConditionFeature>())
                .Do(c => c.ApplyFeature(__instance, activeCondition));
        }
    }

    [HarmonyPatch(typeof(RulesetCharacter), nameof(RulesetCharacter.OnConditionRemoved))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class OnConditionRemoved_Patch
    {
        [UsedImplicitly]
        public static void Postfix(RulesetCharacter __instance, RulesetCondition activeCondition)
        {
            //PATCH: notifies custom condition features that condition is removed 
            var definition = activeCondition.ConditionDefinition;

            definition.GetAllSubFeaturesOfType<ICustomConditionFeature>()
                .ForEach(c => c.RemoveFeature(__instance, activeCondition));

            definition.Features
                .SelectMany(f => f.GetAllSubFeaturesOfType<ICustomConditionFeature>())
                .Do(c => c.RemoveFeature(__instance, activeCondition));
        }
    }

    [HarmonyPatch(typeof(RulesetCharacter), nameof(RulesetCharacter.AcknowledgeAttackedCharacter))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class AcknowledgeAttackedCharacter_Patch
    {
        [UsedImplicitly]
        public static void Postfix([CanBeNull] RulesetCharacter target)
        {
            //PATCH: Allows condition interruption after target was attacked
            target?.ProcessConditionsMatchingInterruption(
                (RuleDefinitions.ConditionInterruption)ExtraConditionInterruption.AfterWasAttacked);
        }
    }

    [HarmonyPatch(typeof(RulesetCharacter), nameof(RulesetCharacter.GetAbilityScoreOfPower))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class GetAbilityScoreOfPower_Patch
    {
        [UsedImplicitly]
        public static void Postfix(RulesetCharacter __instance,
            ref string __result,
            FeatureDefinitionPower featureDefinitionPower)
        {
            //PATCH: allow powers have magic attack bonus based on spell attack
            if (featureDefinitionPower.AttackHitComputation !=
                (RuleDefinitions.PowerAttackHitComputation)ExtraPowerAttackHitComputation.SpellAttack)
            {
                return;
            }

            var repertoire =
                __instance.GetClassSpellRepertoire(__instance.FindClassHoldingFeature(featureDefinitionPower));

            if (repertoire == null)
            {
                return;
            }

            __result = repertoire.SpellCastingAbility;
        }
    }

    [HarmonyPatch(typeof(RulesetCharacter), nameof(RulesetCharacter.GetLowestSlotLevelAndRepertoireToCastSpell))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class GetLowestSlotLevelAndRepertoireToCastSpell_Patch
    {
        [UsedImplicitly]
        public static void Postfix(RulesetCharacter __instance,
            SpellDefinition spellDefinitionToCast,
            ref int __result,
            ref RulesetSpellRepertoire matchingRepertoire)
        {
            //BUGFIX: game doesn't consider cantrips gained from BonusCantrips feature
            //because of this issue Inventor can't use Light cantrip from quick-cast button on UI
            //this patch tries to find requested cantrip in repertoire's ExtraSpellsByTag
            if (spellDefinitionToCast.spellLevel != 0 || matchingRepertoire != null)
            {
                return;
            }

            foreach (var repertoire in __instance.SpellRepertoires
                         .Where(repertoire => repertoire.ExtraSpellsByTag
                             .Any(x => x.Value.Contains(spellDefinitionToCast))))
            {
                matchingRepertoire = repertoire;
                __result = 0;

                break;
            }
        }
    }

    [HarmonyPatch(typeof(RulesetCharacter), nameof(RulesetCharacter.IsComponentSomaticValid))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class IsComponentSomaticValid_Patch
    {
        [UsedImplicitly]
        public static void Postfix(
            RulesetCharacter __instance, ref bool __result, SpellDefinition spellDefinition, ref string failure)
        {
            if (__result)
            {
                return;
            }

            //PATCH: Allows valid Somatic component if specific material component is held in main hand or off hand slots
            // allows casting somatic spells with full hands if one of the hands holds material component for the spell
            ValidateIfMaterialInHand(__instance, spellDefinition, ref __result, ref failure);

            if (__result)
            {
                return;
            }

            //PATCH: Allows valid Somatic component if Inventor has infused item in main hand or off hand slots
            // allows casting somatic spells with full hands if one of the hands holds item infused by the caster
            ValidateIfInfusedInHand(__instance, ref __result, ref failure);
        }

        //TODO: move to separate file
        private static void ValidateIfMaterialInHand(RulesetCharacter caster, SpellDefinition spellDefinition,
            ref bool result, ref string failure)
        {
            if (spellDefinition.MaterialComponentType != RuleDefinitions.MaterialComponentType.Specific)
            {
                return;
            }

            var materialTag = spellDefinition.SpecificMaterialComponentTag;
            var mainHand = caster.GetItemInSlot(EquipmentDefinitions.SlotTypeMainHand);
            var offHand = caster.GetItemInSlot(EquipmentDefinitions.SlotTypeOffHand);
            var tagsMap = new Dictionary<string, TagsDefinitions.Criticity>();

            mainHand?.FillTags(tagsMap, caster, true);
            offHand?.FillTags(tagsMap, caster, true);

            if (!tagsMap.ContainsKey(materialTag))
            {
                return;
            }

            result = true;
            failure = string.Empty;
        }

        //TODO: move to separate file
        private static void ValidateIfInfusedInHand(
            RulesetCharacter caster,
            ref bool result,
            ref string failure)
        {
            var mainHand = caster.GetItemInSlot(EquipmentDefinitions.SlotTypeMainHand);
            var offHand = caster.GetItemInSlot(EquipmentDefinitions.SlotTypeOffHand);

            if (!caster.HoldsMyInfusion(mainHand) && !caster.HoldsMyInfusion(offHand))
            {
                return;
            }

            result = true;
            failure = string.Empty;
        }
    }

    [HarmonyPatch(typeof(RulesetCharacter), nameof(RulesetCharacter.IsComponentMaterialValid))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class IsComponentMaterialValid_Patch
    {
        [UsedImplicitly]
        public static void Postfix(
            RulesetCharacter __instance,
            ref bool __result,
            SpellDefinition spellDefinition,
            ref string failure)
        {
            //PATCH: Allow spells to satisfy material components by using stack of equal or greater value
            StackedMaterialComponent.IsComponentMaterialValid(__instance, spellDefinition, ref failure, ref __result);

            if (__result)
            {
                return;
            }

            //PATCH: Allows spells to satisfy specific material components by actual active tags on an item that are not directly defined in ItemDefinition (like "Melee")
            //Used mostly for melee cantrips requiring melee weapon to cast
            ValidateSpecificComponentsByTags(__instance, spellDefinition, ref __result, ref failure);

            if (__result)
            {
                return;
            }

            //PATCH: Allows spells to satisfy mundane material components if Inventor has infused item equipped
            //Used mostly for melee cantrips requiring melee weapon to cast
            ValidateInfusedFocus(__instance, spellDefinition, ref __result, ref failure);
        }

        //TODO: move to separate file
        private static void ValidateSpecificComponentsByTags(
            RulesetCharacter caster,
            SpellDefinition spell,
            ref bool result,
            ref string failure)
        {
            if (spell.MaterialComponentType != RuleDefinitions.MaterialComponentType.Specific)
            {
                return;
            }

            var materialTag = spell.SpecificMaterialComponentTag;
            var requiredCost = spell.SpecificMaterialComponentCostGp;
            var items = new List<RulesetItem>();

            caster.CharacterInventory.EnumerateAllItems(items);

            var tagsMap = new Dictionary<string, TagsDefinitions.Criticity>();

            foreach (var rulesetItem in items)
            {
                tagsMap.Clear();
                rulesetItem.FillTags(tagsMap, caster, true);

                var itemItemDefinition = rulesetItem.ItemDefinition;
                var costInGold = EquipmentDefinitions.GetApproximateCostInGold(itemItemDefinition.Costs);

                if (tagsMap.ContainsKey(materialTag) && costInGold >= requiredCost)
                {
                    continue;
                }

                result = true;
                failure = string.Empty;
            }
        }

        //TODO: move to separate file
        private static void ValidateInfusedFocus(
            RulesetCharacter caster,
            SpellDefinition spell,
            ref bool result,
            ref string failure)
        {
            if (spell.MaterialComponentType != RuleDefinitions.MaterialComponentType.Mundane)
            {
                return;
            }

            var items = new List<RulesetItem>();

            caster.CharacterInventory.EnumerateAllItems(items);

            if (!items.Any(caster.HoldsMyInfusion))
            {
                return;
            }

            result = true;
            failure = string.Empty;
        }
    }

    [HarmonyPatch(typeof(RulesetCharacter), nameof(RulesetCharacter.SpendSpellMaterialComponentAsNeeded))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class SpendSpellMaterialComponentAsNeeded_Patch
    {
        [UsedImplicitly]
        public static bool Prefix(RulesetCharacter __instance, RulesetEffectSpell activeSpell)
        {
            //PATCH: Modify original code to spend enough of a stack to meet component cost
            return StackedMaterialComponent.SpendSpellMaterialComponentAsNeeded(__instance, activeSpell);
        }
    }

    [HarmonyPatch(typeof(RulesetCharacter), nameof(RulesetCharacter.IsValidReadyCantrip))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class IsValidReadyCantrip_Patch
    {
        [UsedImplicitly]
        public static void Postfix(RulesetCharacter __instance, ref bool __result,
            SpellDefinition cantrip)
        {
            //PATCH: Modifies validity of ready cantrip action to include attack cantrips even if they don't have damage forms
            //makes melee cantrips valid for ready action
            if (__result)
            {
                return;
            }

            var effect = PowerBundle.ModifySpellEffect(cantrip, __instance);
            var hasDamage = effect.HasFormOfType(EffectForm.EffectFormType.Damage);
            var hasAttack = cantrip.HasSubFeatureOfType<IPerformAttackAfterMagicEffectUse>();
            var notGadgets = effect.TargetFilteringMethod != RuleDefinitions.TargetFilteringMethod.GadgetOnly;
            var componentsValid = __instance.AreSpellComponentsValid(cantrip);

            __result = (hasDamage || hasAttack) && notGadgets && componentsValid;
        }
    }

    [HarmonyPatch(typeof(RulesetCharacter), nameof(RulesetCharacter.IsSubjectToAttackOfOpportunity))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class IsSubjectToAttackOfOpportunity_Patch
    {
        // ReSharper disable once RedundantAssignment
        [UsedImplicitly]
        public static void Postfix(
            RulesetCharacter __instance, ref bool __result, RulesetCharacter attacker, float distance)
        {
            //PATCH: allows custom exceptions for attack of opportunity triggering
            //Mostly for Sentinel feat
            __result = AttacksOfOpportunity.IsSubjectToAttackOfOpportunity(__instance, attacker, __result, distance);
        }
    }

    [HarmonyPatch(typeof(RulesetCharacter), nameof(RulesetCharacter.ComputeSaveDC))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    // ReSharper disable once InconsistentNaming
    public static class ComputeSaveDC_Patch
    {
        [UsedImplicitly]
        public static void Postfix(RulesetCharacter __instance, ref int __result)
        {
            //PATCH: support for `IIncreaseSpellDC`
            //Adds extra modifiers to spell DC

            var features = __instance.GetSubFeaturesByType<IIncreaseSpellDc>();

            __result += features.Sum(feature => feature.GetSpellModifier(__instance));
        }
    }

    //PATCH: ensures that the wildshape heroes or heroes under rage cannot cast any spells (Multiclass)
    [HarmonyPatch(typeof(RulesetCharacter), nameof(RulesetCharacter.CanCastSpells))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class CanCastSpells_Patch
    {
        [UsedImplicitly]
        public static void Postfix(RulesetCharacter __instance, ref bool __result)
        {
            // wildshape
            if (__instance.OriginalFormCharacter is RulesetCharacterHero hero && hero != __instance &&
                hero.classesAndLevels.TryGetValue(Druid, out var level) &&
                level < 18)
            {
                __result = false;
            }

            // raging
            if (__instance.AllConditions
                .Any(x => x.ConditionDefinition == DatabaseHelper.ConditionDefinitions.ConditionRaging))
            {
                __result = false;
            }
        }
    }

    //PATCH: ensures that the wildshape hero has access to spell repertoires for calculating slot related features (Multiclass)
    [HarmonyPatch(typeof(RulesetCharacter), nameof(RulesetCharacter.SpellRepertoires), MethodType.Getter)]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class SpellRepertoires_Getter_Patch
    {
        [UsedImplicitly]
        public static void Postfix(RulesetCharacter __instance, ref List<RulesetSpellRepertoire> __result)
        {
            if (__instance.OriginalFormCharacter is RulesetCharacterHero hero && hero != __instance)
            {
                __result = hero.SpellRepertoires;
            }
        }
    }

    //PATCH: ensures that original character sorcery point pool is in sync with substitute (Multiclass)
    [HarmonyPatch(typeof(RulesetCharacter), nameof(RulesetCharacter.CreateSorceryPoints))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class CreateSorceryPoints_Patch
    {
        [UsedImplicitly]
        public static void Postfix(RulesetCharacter __instance, int slotLevel, RulesetSpellRepertoire repertoire)
        {
            if (__instance.OriginalFormCharacter is RulesetCharacterHero hero && hero != __instance)
            {
                hero.CreateSorceryPoints(slotLevel, repertoire);
            }
        }
    }

    //PATCH: ensures that original character sorcery point pool is in sync with substitute (Multiclass)
    [HarmonyPatch(typeof(RulesetCharacter), nameof(RulesetCharacter.GainSorceryPoints))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class GainSorceryPoints_Patch
    {
        [UsedImplicitly]
        public static void Postfix(RulesetCharacter __instance, int sorceryPointsGain)
        {
            if (__instance.OriginalFormCharacter is RulesetCharacterHero hero && hero != __instance)
            {
                hero.GainSorceryPoints(sorceryPointsGain);
            }
        }
    }

    [HarmonyPatch(typeof(RulesetCharacter), nameof(RulesetCharacter.UsePower))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class UsePower_Patch
    {
        [UsedImplicitly]
        public static void Postfix(RulesetCharacter __instance, RulesetUsablePower usablePower)
        {
            if (__instance.OriginalFormCharacter is RulesetCharacterHero hero && hero != __instance)
            {
                // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
                switch (usablePower.PowerDefinition.RechargeRate)
                {
                    //PATCH: ensures that original character rage pool is in sync with substitute (Multiclass)
                    case RuleDefinitions.RechargeRate.RagePoints:
                        hero.SpendRagePoint();

                        break;
                    //PATCH: ensures that original character ki pool is in sync with substitute (Multiclass)
                    case RuleDefinitions.RechargeRate.KiPoints:
                        hero.ForceKiPointConsumption(usablePower.PowerDefinition.CostPerUse);

                        break;
                }
            }

            //PATCH: update usage for power pools 
            __instance.UpdateUsageForPower(usablePower, usablePower.PowerDefinition.CostPerUse);

            //PATCH: support for counting uses of power in the UsedSpecialFeatures dictionary of the GameLocationCharacter
            CountPowerUseInSpecialFeatures.Count(__instance, usablePower);
        }
    }

    [HarmonyPatch(typeof(RulesetCharacter), nameof(RulesetCharacter.RefreshAttributeModifiersFromConditions))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class RefreshAttributeModifiersFromConditions_Patch
    {
        [NotNull]
        [UsedImplicitly]
        public static IEnumerable<CodeInstruction> Transpiler([NotNull] IEnumerable<CodeInstruction> instructions)
        {
            //PATCH: support for validation of attribute modifications applied through conditions
            //Replaces first `IsInst` operator with custom validator

            var validate = new Func<
                FeatureDefinition,
                RulesetCharacter,
                FeatureDefinition
            >(FeatureApplicationValidation.ValidateAttributeModifier).Method;

            return instructions.ReplaceCode(instruction => instruction.opcode == OpCodes.Isinst,
                -1, "RulesetCharacter.RefreshAttributeModifiersFromConditions",
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Call, validate));
        }
    }

    [HarmonyPatch(typeof(RulesetCharacter), nameof(RulesetCharacter.RollAttack))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class RollAttack_Patch
    {
        [NotNull]
        [UsedImplicitly]
        public static IEnumerable<CodeInstruction> Transpiler([NotNull] IEnumerable<CodeInstruction> instructions)
        {
            //PATCH: support for Mirror Image - replaces target's AC with 10 + DEX bonus if we targeting mirror image
            var currentValueMethod = typeof(RulesetAttribute).GetMethod("get_CurrentValue");
            var method =
                new Func<RulesetAttribute, RulesetActor, List<RuleDefinitions.TrendInfo>, int>(MirrorImageLogic.GetAC)
                    .Method;

            return instructions.ReplaceCall(currentValueMethod,
                1, "RulesetCharacter.RollAttack",
                new CodeInstruction(OpCodes.Ldarg_2),
                new CodeInstruction(OpCodes.Ldarg, 4),
                new CodeInstruction(OpCodes.Call, method));
        }
    }

    [HarmonyPatch(typeof(RulesetCharacter), nameof(RulesetCharacter.RollAttackMode))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class RollAttackMode_Patch
    {
        [UsedImplicitly]
        public static void Prefix(
            [NotNull] RulesetCharacter __instance,
            RulesetAttackMode attackMode,
            RulesetActor target,
            List<RuleDefinitions.TrendInfo> toHitTrends,
            bool testMode)
        {
            //PATCH: support for Mirror Image - checks if we have Mirror Images, rolls for it and adds proper to hit trend to mark this roll
            MirrorImageLogic.AttackRollPrefix(__instance, target, toHitTrends, testMode);

            //PATCH: support Elven Precision - sets up flag if this physical attack is valid 
            ElvenPrecisionLogic.PhysicalAttackRollPrefix(__instance, attackMode);
        }

        [UsedImplicitly]
        public static void Postfix(
            RulesetAttackMode attackMode,
            RulesetActor target,
            List<RuleDefinitions.TrendInfo> toHitTrends,
            ref RuleDefinitions.RollOutcome outcome,
            ref int successDelta,
            bool testMode)
        {
            //PATCH: support for Mirror Image - checks if we have Mirror Images, and makes attack miss target and removes 1 image if it was hit
            MirrorImageLogic.AttackRollPostfix(attackMode, target, toHitTrends,
                ref outcome,
                ref successDelta,
                testMode);

            //PATCH: support for Elven Precision - reset flag after physical attack is finished
            ElvenPrecisionLogic.Active = false;
        }
    }

    [HarmonyPatch(typeof(RulesetCharacter), nameof(RulesetCharacter.RollMagicAttack))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class RollMagicAttack_Patch
    {
        internal static RulesetEffect CurrentMagicEffect;

        [UsedImplicitly]
        public static void Prefix(
            [NotNull] RulesetCharacter __instance,
            RulesetEffect activeEffect,
            RulesetActor target,
            List<RuleDefinitions.TrendInfo> toHitTrends,
            bool testMode)
        {
            CurrentMagicEffect = activeEffect;

            //PATCH: support for Mirror Image - checks if we have Mirror Images, rolls for it and adds proper to hit trend to mark this roll
            MirrorImageLogic.AttackRollPrefix(__instance, target, toHitTrends, testMode);

            //PATCH: support Elven Precision - sets up flag if this physical attack is valid 
            ElvenPrecisionLogic.MagicAttackRollPrefix(__instance, activeEffect);
        }

        [UsedImplicitly]
        public static void Postfix(
            RulesetActor target,
            List<RuleDefinitions.TrendInfo> toHitTrends,
            ref RuleDefinitions.RollOutcome outcome,
            ref int successDelta,
            bool testMode)
        {
            //PATCH: support for Mirror Image - checks if we have Mirror Images, and makes attack miss target and removes 1 image if it was hit
            MirrorImageLogic.AttackRollPostfix(null, target, toHitTrends, ref outcome, ref successDelta, testMode);

            //PATCH: support for Elven Precision - reset flag after magic attack is finished
            ElvenPrecisionLogic.Active = false;
            CurrentMagicEffect = null;
        }
    }

    //PATCH: IChangeAbilityCheck
    [HarmonyPatch(typeof(RulesetCharacter), nameof(RulesetCharacter.RollAbilityCheck))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class RollAbilityCheck_Patch
    {
        [UsedImplicitly]
        public static void Prefix(
            [NotNull] RulesetCharacter __instance,
            int baseBonus,
            string abilityScoreName,
            string proficiencyName,
            List<RuleDefinitions.TrendInfo> modifierTrends,
            List<RuleDefinitions.TrendInfo> advantageTrends,
            int rollModifier,
            ref int minRoll)
        {
            var features = __instance.GetSubFeaturesByType<IChangeAbilityCheck>();

            if (features.Count <= 0)
            {
                return;
            }

            var newMinRoll = features
                .Max(x => x.MinRoll(__instance, baseBonus, rollModifier, abilityScoreName, proficiencyName,
                    advantageTrends, modifierTrends));

            if (minRoll < newMinRoll)
            {
                minRoll = newMinRoll;
            }
        }
    }

    //PATCH: IChangeAbilityCheck
    [HarmonyPatch(typeof(RulesetCharacter), nameof(RulesetCharacter.ResolveContestCheck))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class ResolveContestCheck_Patch
    {
        [UsedImplicitly]
        public static int ExtendedRollDie(
            [NotNull] RulesetCharacter rulesetCharacter,
            RuleDefinitions.DieType dieType,
            RuleDefinitions.RollContext rollContext,
            bool isProficient,
            RuleDefinitions.AdvantageType advantageType,
            out int firstRoll,
            out int secondRoll,
            bool enumerateFeatures,
            bool canRerollDice,
            string skill,
            int baseBonus,
            int rollModifier,
            string abilityScoreName,
            string proficiencyName,
            List<RuleDefinitions.TrendInfo> advantageTrends,
            List<RuleDefinitions.TrendInfo> modifierTrends)
        {
            var features = rulesetCharacter.GetSubFeaturesByType<IChangeAbilityCheck>();
            var result = rulesetCharacter.RollDie(dieType, rollContext, isProficient, advantageType,
                out firstRoll, out secondRoll, enumerateFeatures, canRerollDice, skill);

            if (features.Count <= 0)
            {
                return result;
            }

            var newMinRoll = features
                .Max(x => x.MinRoll(rulesetCharacter, baseBonus, rollModifier, abilityScoreName, proficiencyName,
                    advantageTrends, modifierTrends));

            if (result < newMinRoll)
            {
                result = newMinRoll;
            }

            return result;
        }

        //
        // there are 2 calls to RollDie on this method
        // we replace them to allow us to compare the die result vs. the minRoll value from any IChangeAbilityCheck feature
        //
        [UsedImplicitly]
        public static IEnumerable<CodeInstruction> Transpiler([NotNull] IEnumerable<CodeInstruction> instructions)
        {
            var rollDieMethod = typeof(RulesetActor).GetMethod("RollDie");
            var extendedRollDieMethod = typeof(ResolveContestCheck_Patch).GetMethod("ExtendedRollDie");

            return instructions
                // first call to roll die checks the initiator
                .ReplaceCall(rollDieMethod,
                    1, "RulesetCharacter.ResolveContestCheck.RollDie1",
                    new CodeInstruction(OpCodes.Ldarg, 1), // baseBonus
                    new CodeInstruction(OpCodes.Ldarg, 2), // rollModifier
                    new CodeInstruction(OpCodes.Ldarg, 3), // abilityScoreName
                    new CodeInstruction(OpCodes.Ldarg, 4), // proficiencyName
                    new CodeInstruction(OpCodes.Ldarg, 5), // advantageTrends
                    new CodeInstruction(OpCodes.Ldarg, 6), // modifierTrends
                    new CodeInstruction(OpCodes.Call, extendedRollDieMethod))
                // second call to roll die checks the opponent
                .ReplaceCall(
                    rollDieMethod, // in fact this is 2nd occurence on game code but as we replaced on previous step we set to 1
                    1, "RulesetCharacter.ResolveContestCheck.RollDie2",
                    new CodeInstruction(OpCodes.Ldarg, 7), // opponentBaseBonus
                    new CodeInstruction(OpCodes.Ldarg, 8), // opponentRollModifier
                    new CodeInstruction(OpCodes.Ldarg, 9), // opponentAbilityScoreName
                    new CodeInstruction(OpCodes.Ldarg, 10), // opponentProficiencyName
                    new CodeInstruction(OpCodes.Ldarg, 11), // opponentAdvantageTrends
                    new CodeInstruction(OpCodes.Ldarg, 12), // opponentModifierTrends
                    new CodeInstruction(OpCodes.Call, extendedRollDieMethod));
        }
    }

    //PATCH: logic to correctly calculate spell slots under MC (Multiclass)
    [HarmonyPatch(typeof(RulesetCharacter), nameof(RulesetCharacter.RefreshSpellRepertoires))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class RefreshSpellRepertoires_Patch
    {
        [NotNull]
        [UsedImplicitly]
        public static IEnumerable<CodeInstruction> Transpiler([NotNull] IEnumerable<CodeInstruction> instructions)
        {
            var enumerate = new Action<
                RulesetCharacter,
                List<FeatureDefinition>,
                Dictionary<FeatureDefinition, RuleDefinitions.FeatureOrigin>
            >(EnumerateSpellCastingAffinityProviderFromItems).Method;

            //PATCH: make ISpellCastingAffinityProvider from dynamic item properties apply to repertoires
            return instructions.ReplaceEnumerateFeaturesToBrowse("ISpellCastingAffinityProvider",
                -1, "RulesetCharacter.RefreshSpellRepertoires",
                new CodeInstruction(OpCodes.Call, enumerate));
        }

        [UsedImplicitly]
        public static void Postfix(RulesetCharacter __instance)
        {
            if (__instance is not RulesetCharacterHero hero || !SharedSpellsContext.IsMulticaster(hero))
            {
                return;
            }

            var slots = new Dictionary<int, int>();

            // adds features slots
            foreach (var additionalSlot in hero.FeaturesToBrowse
                         .OfType<FeatureDefinitionMagicAffinity>()
                         // special Warlock case so we should discard it here
                         .Where(x => x != MagicAffinityChitinousBoonAdditionalSpellSlot)
                         .OfType<ISpellCastingAffinityProvider>()
                         .SelectMany(x => x.AdditionalSlots))
            {
                slots.TryAdd(additionalSlot.SlotLevel, 0);
                slots[additionalSlot.SlotLevel] += additionalSlot.SlotsNumber;
            }

            // adds spell slots
            var sharedCasterLevel = SharedSpellsContext.GetSharedCasterLevel(hero);
            var sharedSpellLevel = SharedSpellsContext.GetSharedSpellLevel(hero);

            for (var i = 1; i <= sharedSpellLevel; i++)
            {
                slots.TryAdd(i, 0);
                slots[i] += SharedSpellsContext.FullCastingSlots[sharedCasterLevel - 1].Slots[i - 1];
            }

            var warlockSpellLevel = SharedSpellsContext.GetWarlockSpellLevel(hero);

            for (var i = 1; i <= warlockSpellLevel; i++)
            {
                slots.TryAdd(i, 0);
                slots[i] += SharedSpellsContext.GetWarlockMaxSlots(hero);
            }

            // reassign slots back to repertoires except for race ones
            foreach (var spellRepertoire in hero.SpellRepertoires
                         .Where(x => x.SpellCastingFeature.SpellCastingOrigin
                             is FeatureDefinitionCastSpell.CastingOrigin.Class
                             or FeatureDefinitionCastSpell.CastingOrigin.Subclass))
            {
                spellRepertoire.spellsSlotCapacities = slots.DeepCopy();
                spellRepertoire.RepertoireRefreshed?.Invoke(spellRepertoire);
            }
        }
    }

    [HarmonyPatch(typeof(RulesetCharacter), nameof(RulesetCharacter.RechargePowersForTurnStart))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class RechargePowersForTurnStart_Patch
    {
        [UsedImplicitly]
        public static void Postfix(RulesetCharacter __instance)
        {
            //PATCH: support for powers that recharge on turn start
            foreach (var usablePower in __instance.UsablePowers)
            {
                if (usablePower.RemainingUses >= usablePower.MaxUses)
                {
                    continue;
                }

                var startOfTurnRecharge = usablePower.PowerDefinition.GetFirstSubFeatureOfType<IStartOfTurnRecharge>();

                if (startOfTurnRecharge == null)
                {
                    continue;
                }

                usablePower.Recharge();

                if (!startOfTurnRecharge.IsRechargeSilent && __instance.PowerRecharged != null)
                {
                    __instance.PowerRecharged(__instance, usablePower);
                }
            }
        }
    }

    [HarmonyPatch(typeof(RulesetCharacter), nameof(RulesetCharacter.RepayPowerUse))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class RepayPowerUse_Patch
    {
        [UsedImplicitly]
        public static void Postfix(RulesetCharacter __instance, RulesetUsablePower usablePower)
        {
            //PATCH: update usage for power pools
            __instance.UpdateUsageForPower(usablePower, -usablePower.PowerDefinition.CostPerUse);
        }
    }

    [HarmonyPatch(typeof(RulesetCharacter), nameof(RulesetCharacter.GrantPowers))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class GrantPowers_Patch
    {
        [UsedImplicitly]
        public static void Postfix(RulesetCharacter __instance)
        {
            //PATCH: update usage for power pools
            PowerBundle.RechargeLinkedPowers(__instance, RuleDefinitions.RestType.LongRest);
        }
    }

    [HarmonyPatch(typeof(RulesetCharacter), nameof(RulesetCharacter.ApplyRest))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class ApplyRest_Patch
    {
        [UsedImplicitly]
        public static void Postfix(
            RulesetCharacter __instance, RuleDefinitions.RestType restType, bool simulate)
        {
            //PATCH: update usage for power pools
            if (!simulate)
            {
                PowerBundle.RechargeLinkedPowers(__instance, restType);
            }

            // The player isn't recharging the shared pool features, just the pool.
            // Hide the features that use the pool from the UI.
            foreach (var feature in __instance.RecoveredFeatures.Where(f => f is IPowerSharedPool).ToArray())
            {
                __instance.RecoveredFeatures.Remove(feature);
            }

            //PATCH: support for invocations that recharge on short rest (like Fey Teleportation feat)
            foreach (var invocation in __instance.Invocations
                         .Where(invocation =>
                             invocation.InvocationDefinition.HasSubFeatureOfType<InvocationShortRestRecharge>()))
            {
                invocation.Recharge();
            }
        }

        [UsedImplicitly]
        public static IEnumerable<CodeInstruction> Transpiler([NotNull] IEnumerable<CodeInstruction> instructions)
        {
            //PATCH: Makes powers that have their max usage extended by pool modifiers show up correctly during rest
            //replace calls to MaxUses getter to custom method that accounts for extended power usage
            var bind = typeof(RulesetUsablePower).GetMethod("get_MaxUses", BindingFlags.Public | BindingFlags.Instance);
            var maxUses =
                new Func<RulesetUsablePower, RulesetCharacter, int>(PowerBundle.GetMaxUsesForPool).Method;
            var restoreAllSpellSlotsMethod = typeof(RulesetSpellRepertoire).GetMethod("RestoreAllSpellSlots");
            var myRestoreAllSpellSlotsMethod =
                new Action<RulesetSpellRepertoire, RulesetCharacter, RuleDefinitions.RestType>(RestoreAllSpellSlots)
                    .Method;

            return instructions
                .ReplaceCalls(bind,
                    "RulesetCharacter.ApplyRest.MaxUses",
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Call, maxUses))
                .ReplaceCalls(restoreAllSpellSlotsMethod,
                    "RulesetCharacter.ApplyRest.RestoreAllSpellSlots",
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldarg_1),
                    new CodeInstruction(OpCodes.Call, myRestoreAllSpellSlotsMethod));
        }

        private static void RestoreAllSpellSlots(
            RulesetSpellRepertoire __instance,
            RulesetCharacter rulesetCharacter,
            RuleDefinitions.RestType restType)
        {
            if (restType == RuleDefinitions.RestType.LongRest
                || rulesetCharacter is not RulesetCharacterHero hero
                || !SharedSpellsContext.IsMulticaster(hero))
            {
                rulesetCharacter.RestoreAllSpellSlots();

                return;
            }

            var warlockSpellLevel = SharedSpellsContext.GetWarlockSpellLevel(hero);
            var warlockUsedSlots = SharedSpellsContext.GetWarlockUsedSlots(hero);

            foreach (var spellRepertoire in hero.SpellRepertoires
                         .Where(x => x.SpellCastingFeature.SpellCastingOrigin
                             is FeatureDefinitionCastSpell.CastingOrigin.Class
                             or FeatureDefinitionCastSpell.CastingOrigin.Subclass))
            {
                for (var i = SharedSpellsContext.PactMagicSlotsTab; i <= warlockSpellLevel; i++)
                {
                    if (spellRepertoire.usedSpellsSlots.ContainsKey(i))
                    {
                        spellRepertoire.usedSpellsSlots[i] -= warlockUsedSlots;
                    }
                }

                spellRepertoire.RepertoireRefreshed?.Invoke(spellRepertoire);
            }
        }
    }

    //PATCH: ensures auto prepared spells from subclass are considered on level up
    [HarmonyPatch(typeof(RulesetCharacter), nameof(RulesetCharacter.ComputeAutopreparedSpells))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class ComputeAutopreparedSpells_Patch
    {
        [UsedImplicitly]
        public static bool Prefix(
            [NotNull] RulesetCharacter __instance, [NotNull] RulesetSpellRepertoire spellRepertoire)
        {
            //BEGIN PATCH
            var spellcastingClass = spellRepertoire.SpellCastingClass;

            if (spellcastingClass == null && spellRepertoire.SpellCastingSubclass != null)
            {
                spellcastingClass = LevelUpContext.GetClassForSubclass(spellRepertoire.SpellCastingSubclass);
            }
            //END PATCH

            // this includes all the logic for the base function
            spellRepertoire.AutoPreparedSpells.Clear();
            __instance.EnumerateFeaturesToBrowse<FeatureDefinitionAutoPreparedSpells>(__instance.FeaturesToBrowse);

            var features = __instance.FeaturesToBrowse.OfType<FeatureDefinitionAutoPreparedSpells>();

            foreach (var autoPreparedSpells in features)
            {
                var matcher = autoPreparedSpells.GetFirstSubFeatureOfType<RepertoireValidForAutoPreparedFeature>();
                bool matches;

                if (matcher == null)
                {
                    matches = autoPreparedSpells.SpellcastingClass == spellcastingClass;
                }
                else
                {
                    matches = matcher(spellRepertoire, __instance);
                }

                if (!matches)
                {
                    continue;
                }

                var classLevel = __instance.GetSpellcastingLevel(spellRepertoire);

                foreach (var preparedSpellsGroup in autoPreparedSpells.AutoPreparedSpellsGroups
                             .Where(preparedSpellsGroup => preparedSpellsGroup.ClassLevel <= classLevel))
                {
                    spellRepertoire.AutoPreparedSpells.AddRange(preparedSpellsGroup.SpellsList);
                    spellRepertoire.AutoPreparedTag = autoPreparedSpells.AutoPreparedTag;
                }
            }

            return false;
        }
    }

    [HarmonyPatch(typeof(RulesetCharacter), nameof(RulesetCharacter.RollInitiative))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class RollInitiative_Patch
    {
        [UsedImplicitly]
        public static void Prefix(RulesetCharacter __instance, ref int forcedInitiative)
        {
            //PATCH: allows summons to have forced initiative of a summoner
            if (!__instance.HasSubFeatureOfType<ForceInitiativeToSummoner>())
            {
                return;
            }

            var summoner = __instance.GetMySummoner();

            if (summoner == null)
            {
                return;
            }

            forcedInitiative = summoner.lastInitiative;
        }
    }

    [HarmonyPatch(typeof(RulesetCharacter), nameof(RulesetCharacter.RefreshUsableDeviceFunctions))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class RefreshUsableDeviceFunctions_Patch
    {
        [UsedImplicitly]
        public static IEnumerable<CodeInstruction> Transpiler([NotNull] IEnumerable<CodeInstruction> instructions)
        {
            var isFunctionAvailable = typeof(RulesetItemDevice).GetMethod("IsFunctionAvailable");
            var customMethod = typeof(RefreshUsableDeviceFunctions_Patch).GetMethod("IsFunctionAvailable");

            return instructions.ReplaceCalls(isFunctionAvailable,
                "RulesetCharacter.RefreshUsableDeviceFunctions",
                new CodeInstruction(OpCodes.Call, customMethod));
        }

        [UsedImplicitly]
        public static bool IsFunctionAvailable(RulesetItemDevice device,
            RulesetDeviceFunction function,
            RulesetCharacter character,
            bool inCombat,
            bool usedMainSpell,
            bool usedBonusSpell,
            out string failureFlag)
        {
            //PATCH: allow PowerVisibilityModifier to make power device functions visible even if not valid
            //used to make Grenadier's grenade functions not be be hidden when you have not enough charges
            var result = device.IsFunctionAvailable(function, character, inCombat, usedMainSpell, usedBonusSpell,
                out failureFlag);

            if (result || function.DeviceFunctionDescription.type != DeviceFunctionDescription.FunctionType.Power)
            {
                return result;
            }

            var power = function.DeviceFunctionDescription.FeatureDefinitionPower;

            if (PowerVisibilityModifier.IsPowerHidden(character, power, ActionDefinitions.ActionType.Main)
                || !character.CanUsePower(power, false))
            {
                return false;
            }

            failureFlag = string.Empty;

            return true;
        }
    }

    [HarmonyPatch(typeof(RulesetCharacter), nameof(RulesetCharacter.ComputeSpeedAddition))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class ComputeSpeedAddition_Patch
    {
        [UsedImplicitly]
        public static void Postfix(RulesetCharacter __instance, IMovementAffinityProvider provider, ref int __result)
        {
            if (provider is not FeatureDefinition feature)
            {
                return;
            }

            var modifier = feature.GetFirstSubFeatureOfType<IModifyMovementSpeedAddition>();

            if (modifier != null)
            {
                __result += modifier.ModifySpeedAddition(__instance, provider);
            }
        }
    }

    //PATCH: implement IPreventRemoveConcentrationWithPowerUse
    [HarmonyPatch(typeof(RulesetCharacter), nameof(RulesetCharacter.TerminateSpell))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class TerminateSpell_Patch
    {
        [UsedImplicitly]
        public static bool Prefix()
        {
            var currentAction = Global.CurrentAction;

            return currentAction is not CharacterActionUsePower characterActionUsePower || characterActionUsePower
                    .activePower.PowerDefinition.GetFirstSubFeatureOfType<IPreventRemoveConcentrationWithPowerUse>() ==
                null;
        }
    }

    //PATCH: implement IPreventRemoveConcentrationWithPowerUse
    [HarmonyPatch(typeof(RulesetCharacter), nameof(RulesetCharacter.TerminatePower))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class TerminatePower_Patch
    {
        [UsedImplicitly]
        public static bool Prefix()
        {
            var currentAction = Global.CurrentAction;

            return currentAction is not CharacterActionUsePower characterActionUsePower || characterActionUsePower
                    .activePower.PowerDefinition.GetFirstSubFeatureOfType<IPreventRemoveConcentrationWithPowerUse>() ==
                null;
        }
    }

    //PATCH: support Monk Ki Points Toggle
    [HarmonyPatch(typeof(RulesetCharacter), nameof(RulesetCharacter.RemainingKiPoints), MethodType.Getter)]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class RemainingKiPoints_Getter_Patch
    {
        [UsedImplicitly]
        public static void Postfix(RulesetCharacter __instance, ref int __result)
        {
            if (!__instance.IsToggleEnabled((ActionDefinitions.Id)ExtraActionId.MonkKiPointsToggle))
            {
                __result = 0;
            }
        }
    }

    //PATCH: support adding required action affinities to classes that can use toggles
    [HarmonyPatch(typeof(RulesetCharacter), nameof(RulesetCharacter.PostLoad))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class PostLoad_Patch
    {
        [UsedImplicitly]
        public static void Postfix(RulesetCharacter __instance)
        {
            if (__instance is not RulesetCharacterHero hero)
            {
                return;
            }

            if (hero.ClassesHistory.Contains(Monk))
            {
                var tag = AttributeDefinitions.GetClassTag(Monk, 1);

                switch (Main.Settings.AddMonkKiPointsToggle)
                {
                    case true:
                        if (!hero.HasAnyFeature(GameUiContext.ActionAffinityMonkKiPointsToggle))
                        {
                            hero.ActiveFeatures[tag].Add(GameUiContext.ActionAffinityMonkKiPointsToggle);
                            hero.EnableToggle((ActionDefinitions.Id)ExtraActionId.MonkKiPointsToggle);
                        }

                        break;
                    case false:
                        if (hero.HasAnyFeature(GameUiContext.ActionAffinityMonkKiPointsToggle))
                        {
                            hero.ActiveFeatures[tag].Remove(GameUiContext.ActionAffinityMonkKiPointsToggle);
                        }

                        hero.EnableToggle((ActionDefinitions.Id)ExtraActionId.MonkKiPointsToggle);
                        break;
                }
            }

            if (hero.ClassesHistory.Contains(Paladin))
            {
                var tag = AttributeDefinitions.GetClassTag(Paladin, 1);

                switch (Main.Settings.AddPaladinSmiteToggle)
                {
                    case true:
                        if (!hero.HasAnyFeature(GameUiContext.ActionAffinityPaladinSmiteToggle))
                        {
                            hero.ActiveFeatures[tag].Add(GameUiContext.ActionAffinityPaladinSmiteToggle);
                            hero.EnableToggle((ActionDefinitions.Id)ExtraActionId.PaladinSmiteToggle);
                        }

                        break;
                    case false:
                        if (hero.HasAnyFeature(GameUiContext.ActionAffinityPaladinSmiteToggle))
                        {
                            hero.ActiveFeatures[tag].Remove(GameUiContext.ActionAffinityPaladinSmiteToggle);
                        }

                        hero.EnableToggle((ActionDefinitions.Id)ExtraActionId.PaladinSmiteToggle);
                        break;
                }
            }

            if (hero.ActiveFeatures
                .SelectMany(k => k.Value)
                .OfType<FeatureDefinitionPower>()
                .Any(power => hero.GetPowerFromDefinition(power) == null))
            {
                Main.Info($"Hero [{hero.Name}] had missing powers, granting them");
                hero.GrantPowers();
            }

            hero.RefreshAll();
        }
    }

    //PATCH: allow modifiers from items to be considered on concentration checks
    [HarmonyPatch(typeof(RulesetCharacter), nameof(RulesetCharacter.RollConcentrationCheck))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class RollConcentrationCheck_Patch
    {
        [UsedImplicitly]
        public static IEnumerable<CodeInstruction> Transpiler([NotNull] IEnumerable<CodeInstruction> instructions)
        {
            var enumerate = new Action<
                RulesetCharacter,
                List<FeatureDefinition>,
                Dictionary<FeatureDefinition, RuleDefinitions.FeatureOrigin>
            >(EnumerateSpellCastingAffinityProviderFromItems).Method;

            //PATCH: make ISpellCastingAffinityProvider from dynamic item properties apply to repertoires
            return instructions.ReplaceEnumerateFeaturesToBrowse("ISpellCastingAffinityProvider",
                -1, "RulesetCharacter.RollConcentrationCheck",
                new CodeInstruction(OpCodes.Call, enumerate));
        }
    }

    //PATCH: allow modifiers from items to be considered on concentration checks
    [HarmonyPatch(typeof(RulesetCharacter), nameof(RulesetCharacter.RollConcentrationCheckFromDamage))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class RollConcentrationCheckFromDamage_Patch
    {
        [UsedImplicitly]
        public static IEnumerable<CodeInstruction> Transpiler([NotNull] IEnumerable<CodeInstruction> instructions)
        {
            var enumerate = new Action<
                RulesetCharacter,
                List<FeatureDefinition>,
                Dictionary<FeatureDefinition, RuleDefinitions.FeatureOrigin>
            >(EnumerateSpellCastingAffinityProviderFromItems).Method;

            //PATCH: make ISpellCastingAffinityProvider from dynamic item properties apply to repertoires
            return instructions.ReplaceEnumerateFeaturesToBrowse("FeatureDefinitionMagicAffinity",
                -1, "RulesetCharacter.RollConcentrationCheckFromDamage",
                new CodeInstruction(OpCodes.Call, enumerate));
        }
    }

    //PATCH: allow AddProficiencyBonus to be considered on attribute modifiers used on reaction attacks
    [HarmonyPatch(typeof(RulesetCharacter), nameof(RulesetCharacter.CanAttackOutcomeFromAlterationMagicalEffectFail))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class CanAttackOutcomeFromAlterationMagicalEffectFail_Patch
    {
        [UsedImplicitly]
        public static bool Prefix(
            RulesetCharacter __instance,
            out bool __result,
            List<EffectForm> effectForms,
            int totalAttack)
        {
            foreach (var feature in effectForms
                         .Where(effectForm => effectForm.FormType == EffectForm.EffectFormType.Condition &&
                                              effectForm.ConditionForm.Operation ==
                                              ConditionForm.ConditionOperation.Add).SelectMany(effectForm =>
                             effectForm.ConditionForm.ConditionDefinition.Features))
            {
                if (feature is not FeatureDefinitionAttributeModifier
                    {
                        ModifiedAttribute: AttributeDefinitions.ArmorClass
                    } attributeModifier ||
                    (attributeModifier.ModifierOperation !=
                     FeatureDefinitionAttributeModifier.AttributeModifierOperation.Additive &&
                     attributeModifier.ModifierOperation != FeatureDefinitionAttributeModifier
                         .AttributeModifierOperation.AddProficiencyBonus))
                {
                    continue;
                }

                var currentValue = __instance.RefreshArmorClass(dryRun: true, dryRunFeature: feature).CurrentValue;

                __instance.GetAttribute(AttributeDefinitions.ArmorClass).ReleaseCopy();

                if (currentValue <= totalAttack)
                {
                    continue;
                }

                __result = true;

                return false;
            }

            __result = false;

            return false;
        }
    }
}
