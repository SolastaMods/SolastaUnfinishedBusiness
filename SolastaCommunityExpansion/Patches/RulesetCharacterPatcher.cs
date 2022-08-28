using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaCommunityExpansion.Api.Extensions;
using SolastaCommunityExpansion.CustomDefinitions;
using SolastaCommunityExpansion.CustomInterfaces;
using SolastaCommunityExpansion.Feats;
using SolastaCommunityExpansion.Models;
using SolastaCommunityExpansion.Subclasses.Rogue;
using TA;
using UnityEngine;

namespace SolastaCommunityExpansion.Patches;

internal static class RulesetCharacterPatcher
{
    [HarmonyPatch(typeof(RulesetCharacter), "TerminateMatchingUniquePower")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class TerminateMatchingUniquePower_Patch
    {
        internal static void Postfix(RulesetCharacter __instance, FeatureDefinitionPower powerDefinition)
        {
            //PATCH: terminates all matching spells and powers of same group
            GlobalUniqueEffects.TerminateMatchingUniquePower(__instance, powerDefinition);
        }
    }

    [HarmonyPatch(typeof(RulesetCharacter), "TerminateMatchingUniqueSpell")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class TerminateMatchingUniqueSpell_Patch
    {
        internal static void Postfix(RulesetCharacter __instance, SpellDefinition spellDefinition)
        {
            //PATCH: terminates all matching spells and powers of same group
            GlobalUniqueEffects.TerminateMatchingUniqueSpell(__instance, spellDefinition);
        }
    }

    [HarmonyPatch(typeof(RulesetCharacter), "OnConditionAdded")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class OnConditionAdded_Patch
    {
        internal static void Postfix(RulesetCharacter __instance, RulesetCondition activeCondition)
        {
            //PATCH: notifies custom condition features that condition is applied 
            activeCondition.ConditionDefinition.Features
                .SelectMany(f => f.GetAllSubFeaturesOfType<ICustomConditionFeature>())
                .ToList()
                .ForEach(c => c.ApplyFeature(__instance));
        }
    }

    [HarmonyPatch(typeof(RulesetCharacter), "OnConditionRemoved")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class OnConditionRemoved_Patch
    {
        internal static void Postfix(RulesetCharacter __instance, RulesetCondition activeCondition)
        {
            //PATCH: notifies custom condition features that condition is removed 
            activeCondition.ConditionDefinition.Features
                .SelectMany(f => f.GetAllSubFeaturesOfType<ICustomConditionFeature>())
                .ToList()
                .ForEach(c => c.RemoveFeature(__instance));
        }
    }

    [HarmonyPatch(typeof(RulesetCharacter), "IsComponentSomaticValid")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class IsComponentSomaticValid_Patch
    {
        internal static void Postfix(RulesetCharacter __instance, ref bool __result,
            SpellDefinition spellDefinition, ref string failure)
        {
            //PATCH: Allows valid Somatic component if specific material component is held in main hand or off hand slots
            // allows casting somatic spells with full hands if one of the hands holds metarial component for the spell
            if (__result || spellDefinition.MaterialComponentType != RuleDefinitions.MaterialComponentType.Specific)
            {
                return;
            }

            var materialTag = spellDefinition.SpecificMaterialComponentTag;
            var inventorySlotsByName = __instance.CharacterInventory.InventorySlotsByName;
            var mainHand = inventorySlotsByName[EquipmentDefinitions.SlotTypeMainHand].EquipedItem;
            var offHand = inventorySlotsByName[EquipmentDefinitions.SlotTypeOffHand].EquipedItem;
            var tagsMap = new Dictionary<string, TagsDefinitions.Criticity>();

            mainHand?.FillTags(tagsMap, __instance, true);
            offHand?.FillTags(tagsMap, __instance, true);

            if (!tagsMap.ContainsKey(materialTag))
            {
                return;
            }

            __result = true;
            failure = string.Empty;
        }
    }

    [HarmonyPatch(typeof(RulesetCharacter), "IsComponentMaterialValid")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class IsComponentMaterialValid_Patch
    {
        internal static void Postfix(RulesetCharacter __instance, ref bool __result,
            SpellDefinition spellDefinition, ref string failure)
        {
            //PATCH: Allows spells to satisfy specific material components by actual active tags on an item that are not directly defined in ItemDefinition (like "Melee")
            //Used mostly for melee cantrips requiring melee weapon to cast
            if (__result || spellDefinition.MaterialComponentType != RuleDefinitions.MaterialComponentType.Specific)
            {
                return;
            }

            var materialTag = spellDefinition.SpecificMaterialComponentTag;
            var requiredCost = spellDefinition.SpecificMaterialComponentCostGp;

            List<RulesetItem> items = new();
            __instance.CharacterInventory.EnumerateAllItems(items);
            var tagsMap = new Dictionary<string, TagsDefinitions.Criticity>();
            foreach (var rulesetItem in items)
            {
                tagsMap.Clear();
                rulesetItem.FillTags(tagsMap, __instance, true);
                var itemItemDefinition = rulesetItem.ItemDefinition;
                var costInGold = EquipmentDefinitions.GetApproximateCostInGold(itemItemDefinition.Costs);

                if (!tagsMap.ContainsKey(materialTag) || costInGold < requiredCost)
                {
                    continue;
                }

                __result = true;
                failure = string.Empty;
            }
        }
    }

    [HarmonyPatch(typeof(RulesetCharacter), "IsValidReadyCantrip")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class RulesetCharacter_IsValidReadyCantrip
    {
        internal static void Postfix(RulesetCharacter __instance, ref bool __result,
            SpellDefinition cantrip)
        {
            //PATCH: Modifies validity of ready cantrip action to include attack cantrips even if they don't have damage forms
            //makes melee cantrips valid for ready action
            if (__result)
            {
                return;
            }

            var effect = CustomFeaturesContext.ModifySpellEffect(cantrip, __instance);
            var hasDamage = effect.HasFormOfType(EffectForm.EffectFormType.Damage);
            var hasAttack = cantrip.HasSubFeatureOfType<IPerformAttackAfterMagicEffectUse>();
            var notGadgets = effect.TargetFilteringMethod != RuleDefinitions.TargetFilteringMethod.GadgetOnly;
            var componentsValid = __instance.AreSpellComponentsValid(cantrip);

            __result = (hasDamage || hasAttack) && notGadgets && componentsValid;
        }
    }


    [HarmonyPatch(typeof(RulesetCharacter), "RollAttackMode")]
    internal static class RollAttackMode_Patch
    {
        internal static void Prefix(
            RulesetCharacter __instance,
            RulesetAttackMode attackMode,
            bool ignoreAdvantage,
            List<RuleDefinitions.TrendInfo> advantageTrends,
            bool testMode)
        {
            //PATCH: support for `Elven Accuracy` feat
            //sets up this character as elven accuracy hero for `RolDie` patch to modify rolls

            Global.ElvenAccuracyHero = null;

            if (ignoreAdvantage
                || !testMode
                || attackMode.abilityScore is AttributeDefinitions.Strength or AttributeDefinitions.Constitution)
            {
                return;
            }

            var advantageType = RuleDefinitions.ComputeAdvantage(advantageTrends);

            if (advantageType != RuleDefinitions.AdvantageType.Advantage)
            {
                return;
            }

            var hero = __instance as RulesetCharacterHero ?? __instance.OriginalFormCharacter as RulesetCharacterHero;

            if (hero != null && hero.TrainedFeats.Any(x => x.Name.Contains(ZappaFeats.ElvenAccuracyTag)))
            {
                Global.ElvenAccuracyHero = hero;
            }
        }

        internal static void Postfix(ref int __result)
        {
            //PATCH: support for `Elven Accuracy` feat
            //clears this character from being marked for die roll modification
            Global.ElvenAccuracyHero = null;

            //PATCH: support for Magus's Spell Strike
            //TODO: can we get rid of this global variable?
            if (Global.IsSpellStrike)
            {
                Global.SpellStrikeDieRoll = __result;
            }
        }
    }

    [HarmonyPatch(typeof(RulesetCharacter), "RollMagicAttack")]
    internal static class RollMagicAttack_Patch
    {
        internal static bool Prefix(
            RulesetCharacter __instance,
            ref int __result,
            List<RuleDefinitions.TrendInfo> advantageTrends,
            ref RuleDefinitions.RollOutcome outcome,
            bool testMode)
        {
            //PATCH: support for `Elven Accuracy` feat
            //sets up this character as elven accuracy hero for `RolDie` patch to modify rolls

            Global.ElvenAccuracyHero = null;

            if (testMode)
            {
                var advantageType = RuleDefinitions.ComputeAdvantage(advantageTrends);

                if (advantageType == RuleDefinitions.AdvantageType.Advantage)
                {
                    var hero = __instance as RulesetCharacterHero ??
                               __instance.OriginalFormCharacter as RulesetCharacterHero;

                    if (hero != null && hero.TrainedFeats.Any(x => x.Name.Contains(ZappaFeats.ElvenAccuracyTag)))
                    {
                        Global.ElvenAccuracyHero = hero;
                    }
                }
            }

            return true;
        }

        internal static void Postfix()
        {
            //PATCH: support for `Elven Accuracy` feat
            //clears this character from being marked for die roll modification
            Global.ElvenAccuracyHero = null;
        }
    }


    [HarmonyPatch(typeof(RulesetActor), "RollDie")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class RollDie_Patch
    {
        internal static IEnumerable<CodeInstruction> Transpiler([NotNull] IEnumerable<CodeInstruction> instructions)
        {
            var rollDieMethod = typeof(RuleDefinitions).GetMethod("RollDie", BindingFlags.Public | BindingFlags.Static);
            var myRollDieMethod = typeof(RollDie_Patch).GetMethod("RollDie");

            foreach (var instruction in instructions)
            {
                if (instruction.Calls(rollDieMethod))
                {
                    yield return new CodeInstruction(OpCodes.Call, myRollDieMethod);
                }
                else
                {
                    yield return instruction;
                }
            }
        }

        // ReSharper disable once UnusedMember.Global
        public static int RollDie(
            RuleDefinitions.DieType diceType,
            RuleDefinitions.AdvantageType advantageType,
            out int firstRoll,
            out int secondRoll,
            float rollAlterationScore)
        {
            //PATCH: support for `Elven Accuracy` feat
            //rerolls dice when appropriate
            
            var hero = Global.ElvenAccuracyHero;

            if (hero == null)
            {
                return RuleDefinitions.RollDie(diceType, advantageType, out firstRoll, out secondRoll,
                    rollAlterationScore);
            }

            var flag = rollAlterationScore != 0.0;
            var rolls = new int[3];

            rolls[0] = flag
                ? RuleDefinitions.RollKarmicDie(diceType, rollAlterationScore)
                : 1 + DeterministicRandom.Range(0, RuleDefinitions.DiceMaxValue[(int) diceType]);
            rolls[1] = flag
                ? RuleDefinitions.RollKarmicDie(diceType, rollAlterationScore)
                : 1 + DeterministicRandom.Range(0, RuleDefinitions.DiceMaxValue[(int) diceType]);
            rolls[2] = flag
                ? RuleDefinitions.RollKarmicDie(diceType, rollAlterationScore)
                : 1 + DeterministicRandom.Range(0, RuleDefinitions.DiceMaxValue[(int) diceType]);

            Array.Sort(rolls);

            var line = Gui.Format("Feedback/&ElvenAccuracyTriggered",
                hero.name, rolls[2].ToString(), rolls[1].ToString(),
                rolls[0].ToString());

            Gui.Game.GameConsole.LogSimpleLine(line);

            firstRoll = rolls[1];
            secondRoll = rolls[2];

            return Mathf.Max(firstRoll, secondRoll);
        }

        // TODO: make this more generic
        internal static void Prefix(RulesetActor __instance, RuleDefinitions.RollContext rollContext,
            ref bool enumerateFeatures, ref bool canRerollDice)
        {
            //PATCH: support for `Raven` Rogue subclass
            
            if (!__instance.HasSubFeatureOfType<Raven.RavenRerollAnyDamageDieMarker>() ||
                rollContext != RuleDefinitions.RollContext.AttackDamageValueRoll)
            {
                return;
            }

            enumerateFeatures = true;
            canRerollDice = true;
        }
    }
}