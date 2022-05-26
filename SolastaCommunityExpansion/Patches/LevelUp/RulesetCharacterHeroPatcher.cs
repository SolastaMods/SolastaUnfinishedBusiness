using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;

namespace SolastaCommunityExpansion.Patches.LevelUp
{
    [HarmonyPatch(typeof(RulesetCharacterHero), "TrainFeats")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class RulesetCharacterHero_TrainFeats
    {
        internal static void Postfix(RulesetCharacterHero __instance, List<FeatDefinition> feats)
        {
            foreach (var feat in feats)
            {
                Models.CustomFeaturesContext.RecursiveGrantCustomFeatures(__instance, null, feat.Features);
            }
        }
    }
}
