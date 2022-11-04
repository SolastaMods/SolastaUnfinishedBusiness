using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Emit;
using HarmonyLib;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Subclasses;

namespace SolastaUnfinishedBusiness.Patches;

public static class RulesetCharacterMonsterPatcher
{
    [HarmonyPatch(typeof(RulesetCharacterMonster), "FinalizeMonster")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class FinalizeMonster_Patch
    {
        public static void Postfix(RulesetCharacterMonster __instance, bool keepMentalAbilityScores)
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
    public static class RefreshAll_Patch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            //PATCH: support for rage/ki/other stuff while shape-shifted

            // refresh values of attribute modifiers before refreshing attributes
            var refreshAttributes = typeof(RulesetEntity).GetMethod("RefreshAttributes");
            var refreshAttributeModifiers =
                typeof(RulesetCharacter).GetMethod("RefreshAttributeModifierFromAbilityScore");

            return instructions.ReplaceCalls(refreshAttributes,
                new CodeInstruction(OpCodes.Call, refreshAttributeModifiers),
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Call, refreshAttributes));
        }
    }

    [HarmonyPatch(typeof(RulesetCharacterMonster), "RefreshArmorClass")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class RefreshArmorClass_Patch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            //PATCH: implements exclusivity for some AC modifiers
            // Makes sure various unarmored defense features don't stack with themselves and Dragon Resilience
            // Replaces calls to `RulesetAttributeModifier.SortAttributeModifiersList` with custom method
            // that removes inactive exclusive modifiers, and then calls `RulesetAttributeModifier.SortAttributeModifiersList`
            return ArmorClassStacking.AddAcTrendsToMonsterAcRefreshTranspiler(instructions);
        }
    }

    [HarmonyPatch(typeof(RulesetCharacterMonster), "ComputeBaseSavingThrowBonus")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class ComputeBaseSavingThrowBonus_Patch
    {
        public static void Postfix(RulesetCharacterMonster __instance, ref int __result,
            string abilityScoreName,
            List<RuleDefinitions.TrendInfo> savingThrowModifierTrends)
        {
            //PATCH: allows `AddPBToSummonCheck` to add summoner's PB to the saving throws
            AddPBToSummonCheck.ModifyCheckBonus<ISavingThrowPerformanceProvider>(__instance, ref __result,
                abilityScoreName, savingThrowModifierTrends);
        }
    }


    [HarmonyPatch(typeof(RulesetCharacterMonster), "ComputeBaseAbilityCheckBonus")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class ComputeBaseAbilityCheckBonus_Patch
    {
        public static void Postfix(RulesetCharacterMonster __instance, ref int __result,
            List<RuleDefinitions.TrendInfo> abilityCheckModifierTrends,
            string proficiencyName)
        {
            //PATCH: allows `AddPBToSummonCheck` to add summoner's PB to the skill checks
            AddPBToSummonCheck.ModifyCheckBonus<IAbilityCheckPerformanceProvider>(__instance, ref __result,
                proficiencyName, abilityCheckModifierTrends);
        }
    }
}
