using HarmonyLib;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SolastaCommunityExpansion.Patches
{
    [HarmonyPatch(typeof(CharacterBuildingManager), "GrantFeatures")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class CharacterBuildingManager_GrantFeatures
    {
        internal static void Prefix(CharacterBuildingManager __instance, List<FeatureDefinition> grantedFeatures)
        {
            foreach (var grantedFeature in grantedFeatures)
            {
                if (grantedFeature is Subclasses.Rogue.Thug.NegativeFeatureDefinition negativeFeatureDefinition)
                {
                    foreach (var activeFeature in __instance.HeroCharacter.ActiveFeatures)
                    {
                        activeFeature.Value.RemoveAll(feature => feature == negativeFeatureDefinition.FeatureToRemove);
                    }
                }
            }
        }
    }

}
