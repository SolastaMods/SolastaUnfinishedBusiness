using HarmonyLib;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SolastaCommunityExpansion.Patches.CustomFeatures
{
    internal static class ApplyFeatureProcessor
    {
        internal static void RecursiveGrantCustomFeatures(RulesetCharacterHero hero, List<FeatureDefinition> features)
        {
            foreach (FeatureDefinition grantedFeature in features)
            {
                if (grantedFeature is FeatureDefinitionFeatureSet set && set.Mode == FeatureDefinitionFeatureSet.FeatureSetMode.Union)
                {
                    RecursiveGrantCustomFeatures(hero, set.FeatureSet);
                }
                if (grantedFeature is CustomFeatureDefinitions.FeatureDefinitionCustomCode customFeature)
                {
                    customFeature.ApplyFeature(hero);
                }
            }
        }
    }

    //
    // this patch shouldn't be protected
    //
    [HarmonyPatch(typeof(CharacterBuildingManager), "GrantFeatures")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class CharacterBuildingManager_GrantFeatures
    {
        /**
         * When a character is being granted features, this patch will apply the effect of custom features.
         */
        internal static void Prefix(CharacterBuildingManager __instance, List<FeatureDefinition> grantedFeatures)
        {
            ApplyFeatureProcessor.RecursiveGrantCustomFeatures(__instance.HeroCharacter, grantedFeatures);
        }
    }

    //
    // this patch shouldn't be protected
    //
    [HarmonyPatch(typeof(RulesetCharacterHero), "TrainFeats")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class RulesetCharacterHero_TrainFeats
    {
        internal static void Postfix(RulesetCharacterHero __instance, List<FeatDefinition> feats)
        {
            foreach (FeatDefinition feat in feats)
            {
                ApplyFeatureProcessor.RecursiveGrantCustomFeatures(__instance, feat.Features);
            }

        }
    }
}
