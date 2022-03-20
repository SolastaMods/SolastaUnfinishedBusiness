using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;

namespace SolastaCommunityExpansion.Patches.CustomFeatures
{
    //
    // ASI or FEAT, CustomFightingStyles and FeatureDefinitionCustomCode features
    //
    [HarmonyPatch(typeof(CharacterBuildingManager), "GrantFeatures")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class CharacterBuildingManager_GrantFeatures
    {
        /**
         * When a character is being granted features, this patch will apply the effect of custom features.
         */

        internal static void Prefix(RulesetCharacterHero hero, List<FeatureDefinition> grantedFeatures)
        {
            Models.CustomFeaturesContext.RecursiveGrantCustomFeatures(hero, grantedFeatures);
        }
    }
}
