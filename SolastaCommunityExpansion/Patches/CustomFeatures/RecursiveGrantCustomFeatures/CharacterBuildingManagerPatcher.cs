using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.Models;

namespace SolastaCommunityExpansion.Patches.CustomFeatures.RecursiveGrantCustomFeatures
{
    [HarmonyPatch(typeof(CharacterBuildingManager), "GrantFeatures")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class CharacterBuildingManager_GrantFeatures
    {
        /**
         * When a character is being granted features, this patch will apply the effect of custom features.
         */
        internal static void Postfix(RulesetCharacterHero hero, List<FeatureDefinition> grantedFeatures)
        {
            CustomFeaturesContext.RecursiveGrantCustomFeatures(hero, grantedFeatures);
        }
    }

    [HarmonyPatch(typeof(CharacterBuildingManager), "ClearPrevious")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class CharacterBuildingManager_ClearPrevious
    {
        internal static void Prefix(RulesetCharacterHero hero, string tag)
        {
            if (string.IsNullOrEmpty(tag) || !hero.ActiveFeatures.ContainsKey(tag))
                return;

            CustomFeaturesContext.RecursiveRemoveCustomFeatures(hero, hero.ActiveFeatures[tag]);
        }
    }
}
