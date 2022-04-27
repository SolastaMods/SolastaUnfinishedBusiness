using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;
using SolastaCommunityExpansion.CustomDefinitions;

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
            Models.CustomFeaturesContext.RecursiveGrantCustomFeatures(hero, grantedFeatures);
        }
    }

    // undo any custom logic when removing features
    [HarmonyPatch(typeof(CharacterBuildingManager), "UnassignLastClassLevel")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class CharacterBuildingManager_UnassignLastClassLevel
    {
        internal static void Prefix(RulesetCharacterHero hero)
        {
            var heroBuildingData = hero.GetHeroBuildingData();
            
            foreach (var feature in heroBuildingData.AllActiveFeatures
                .OfType<IFeatureDefinitionCustomCode>())
            {
                feature.RemoveFeature(hero);
            }
        }
    }

    // undo any custom logic when removing features
    [HarmonyPatch(typeof(CharacterBuildingManager), "UnassignLastSubclass")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class CharacterBuildingManager_UnassignLastSubclass
    {
        internal static void Prefix(RulesetCharacterHero hero)
        {
            var heroBuildingData = hero.GetHeroBuildingData();

            foreach (var feature in heroBuildingData.AllActiveFeatures
                .OfType<IFeatureDefinitionCustomCode>())
            {
                feature.RemoveFeature(hero);
            }
        }
    }
}
