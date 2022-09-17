using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Emit;
using HarmonyLib;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Subclasses;

namespace SolastaUnfinishedBusiness.Patches;

internal static class RulesetCharacterMonsterPatcher
{
    [HarmonyPatch(typeof(RulesetCharacterMonster), "FinalizeMonster")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class FinalizeMonster_Patch
    {
        internal static void Postfix(RulesetCharacterMonster __instance, bool keepMentalAbilityScores)
        {
            //PATCH: Fixes AC calculation for MC shape-shifters
            //Instead of setting monster's AC as base it adds it as a Natural Armor value
            //And adds base 10 and dex AC modifiers too, so they can mix with unarmored defense if needed
            //PATCH: support for rage/ki/other stuff while shape-shifted
            //Transfers some of the ability modifiers to shape shifted form 
            MulticlassWildshapeContext.FinalizeMonster(__instance, keepMentalAbilityScores);

            //PATCH: allows us to change monsters created by Dead Master
            //TODO: Consider using `FeatureDefinitionSummoningAffinity` for this
            WizardDeadMaster.OnMonsterCreated(__instance);
        }
    }

    [HarmonyPatch(typeof(RulesetCharacterMonster), "RefreshAll")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class RefreshAll_Patch
    {
        internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            //PATCH: support for rage/ki/other stuff while shape-shifted

            // refresh values of attribute modifiers before refreshing attributes
            var refreshAttributes = typeof(RulesetEntity).GetMethod("RefreshAttributes");
            var refreshAttributeModifiers =
                typeof(RulesetCharacter).GetMethod("RefreshAttributeModifierFromAbilityScore");

            foreach (var code in instructions)
            {
                if (code.Calls(refreshAttributes))
                {
                    yield return new CodeInstruction(OpCodes.Call, refreshAttributeModifiers);
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                }

                yield return code;
            }
        }
    }

    [HarmonyPatch(typeof(RulesetCharacterMonster), "RefreshArmorClass")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class RefreshArmorClass_Patch
    {
        internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            //PATCH: implements exclusivity for some AC modifiers
            // Makes sure various unarmored defense features don't stack with themselves and Dragon Resilience
            // Replaces calls to `RulesetAttributeModifier.SortAttributeModifiersList` with custom method
            // that removes inactive exclusive modifiers, and then calls `RulesetAttributeModifier.SortAttributeModifiersList`
            return ArmorClassStacking.AddACTrendsToMonsterACRefreshTranspiler(instructions);
        }
    }
}
