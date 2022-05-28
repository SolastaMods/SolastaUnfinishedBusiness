using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.Models;

namespace SolastaCommunityExpansion.Patches.LevelUp
{
    // ensures we only add the dice max value on level 1
    [HarmonyPatch(typeof(RulesetCharacterHero), "AddClassLevel")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class RulesetCharacterHero_AddClassLevel
    {
        internal static bool Prefix(RulesetCharacterHero __instance, CharacterClassDefinition classDefinition,
            List<int> ___hitPointsGainHistory)
        {
            if (!LevelUpContext.IsLevelingUp(__instance))
            {
                return true;
            }

            __instance.ClassesHistory.Add(classDefinition);

            __instance.ClassesAndLevels.TryAdd(classDefinition, 0);
            __instance.ClassesAndLevels[classDefinition]++;

            ___hitPointsGainHistory.Add(HeroDefinitions.RollHitPoints(classDefinition.HitDice));

            __instance.ComputeCharacterLevel();
            __instance.ComputeProficiencyBonus();

            return false;
        }
    }

    [HarmonyPatch(typeof(RulesetCharacterHero), "TrainFeats")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class RulesetCharacterHero_TrainFeats
    {
        internal static void Postfix(RulesetCharacterHero __instance, List<FeatDefinition> feats)
        {
            foreach (var feat in feats)
            {
                CustomFeaturesContext.RecursiveGrantCustomFeatures(__instance, null, feat.Features);
            }
        }
    }
}
