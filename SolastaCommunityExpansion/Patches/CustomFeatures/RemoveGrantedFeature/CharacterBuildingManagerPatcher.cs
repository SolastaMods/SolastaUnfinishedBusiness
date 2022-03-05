using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;
using SolastaCommunityExpansion.CustomFeatureDefinitions;

namespace SolastaCommunityExpansion.Patches.CustomFeatures.RemoveGrantedFeature
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
}
