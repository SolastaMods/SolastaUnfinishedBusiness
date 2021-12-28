using HarmonyLib;
using SolastaCommunityExpansion.CustomFeatureDefinitions;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace SolastaCommunityExpansion.Patches.NegativeFeature
{
    //
    // this patch shouldn't be protected
    //
    [HarmonyPatch(typeof(CharacterBuildingManager), "FinalizeCharacter")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class CharacterBuildingManager_FinalizeCharacter
    {
        internal static void Prefix(CharacterBuildingManager __instance)
        {
            var activeFeatures = __instance.HeroCharacter.ActiveFeatures;
            var removeGrantedFeatures = activeFeatures.SelectMany(x => x.Value.FindAll(y => y is FeatureDefinitionRemoveGrantedFeature));

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
