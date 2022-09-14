using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.Patches.LevelUp;

//PATCH: ensures we only add the dice max value on level 1 (MULTICLASS)
[HarmonyPatch(typeof(RulesetCharacterHero), "AddClassLevel")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class RulesetCharacterHero_AddClassLevel
{
    internal static bool Prefix([NotNull] RulesetCharacterHero __instance, CharacterClassDefinition classDefinition)
    {
        if (!LevelUpContext.IsLevelingUp(__instance))
        {
            return true;
        }

        __instance.ClassesHistory.Add(classDefinition);
        __instance.ClassesAndLevels.TryAdd(classDefinition, 0);
        __instance.ClassesAndLevels[classDefinition]++;
        __instance.hitPointsGainHistory.Add(HeroDefinitions.RollHitPoints(classDefinition.HitDice));
        __instance.ComputeCharacterLevel();
        __instance.ComputeProficiencyBonus();

        return false;
    }
}

//PATCH: Ensure that we correctly grant custom features from Feats
[HarmonyPatch(typeof(RulesetCharacterHero), "TrainFeats")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class RulesetCharacterHero_TrainFeats
{
    internal static void Postfix([NotNull] RulesetCharacterHero __instance, [NotNull] List<FeatDefinition> feats)
    {
        foreach (var feat in feats)
        {
            CustomFeaturesContext.RecursiveGrantCustomFeatures(__instance, null, feat.Features);
        }
    }
}

//PATCH: Ensures we don't offer invocations unlearn on non Warlock MC (Multiclass)
[HarmonyPatch(typeof(RulesetCharacterHero), "InvocationProficiencies", MethodType.Getter)]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class RulesetCharacterHero_InvocationProficiencies
{
    internal static bool Prefix(RulesetCharacterHero __instance, ref List<string> __result)
    {
        var isLevelingUp = LevelUpContext.IsLevelingUp(__instance);

        if (!isLevelingUp)
        {
            return true;
        }

        var selectedClass = LevelUpContext.GetSelectedClass(__instance);

        if (selectedClass == DatabaseHelper.CharacterClassDefinitions.Warlock)
        {
            return true;
        }

        __result = new List<string>();

        return false;
    }
}
