using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;
using SolastaCommunityExpansion.CustomFeatureDefinitions;

namespace SolastaCommunityExpansion.Patches.CustomFeatures
{
    [HarmonyPatch(typeof(CharacterBuildingManager), "FinalizeCharacter")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class CharacterBuildingManager_FinalizeCharacter
    {
        internal static void Prefix(CharacterBuildingManager __instance)
        {
            var activeFeatures = __instance.HeroCharacter.ActiveFeatures;
            var removeGrantedFeatures = activeFeatures.SelectMany(x => x.Value.OfType<FeatureDefinitionRemoveGrantedFeature>());

            foreach (FeatureDefinitionRemoveGrantedFeature removeGrantedFeature in removeGrantedFeatures)
            {
                if (activeFeatures.TryGetValue(removeGrantedFeature.Tag, out var featureDefinitions))
                {
                    featureDefinitions.RemoveAll(x => x == removeGrantedFeature.FeatureToRemove);
                }
            }
        }
    }

    [HarmonyPatch(typeof(CharacterBuildingManager), "GrantFeatures")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class CharacterBuildingManager_GrantFeatures
    {
        /**
         * When a character is being granted features, this patch will apply the effect of custom features.
         */
        internal static void Prefix(CharacterBuildingManager __instance, List<FeatureDefinition> grantedFeatures)
        {
            Models.CustomFeaturesContext.RecursiveGrantCustomFeatures(__instance.HeroCharacter, grantedFeatures);
        }
    }
}
