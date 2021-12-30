using HarmonyLib;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace SolastaCommunityExpansion.Patches.FeatureDefinitionCustomCode
{

    //
    // this patch shouldn't be protected
    //
    [HarmonyPatch(typeof(CharacterBuildingManager), "GrantFeatures")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class CharacterBuildingManager_GrantFeatures
    {
        internal static void Prefix(CharacterBuildingManager __instance, List<FeatureDefinition> grantedFeatures)
        {
            foreach (CustomFeatureDefinitions.FeatureDefinitionCustomCode grantedFeature in grantedFeatures.OfType<CustomFeatureDefinitions.FeatureDefinitionCustomCode>())
            {
                grantedFeature.ApplyFeature(__instance.HeroCharacter);
            }
        }
    }
}
