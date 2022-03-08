using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;

namespace SolastaCommunityExpansion.Patches.CustomFeatures
{
    [HarmonyPatch(typeof(CharacterBuildingManager), "GrantFeatures")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class CharacterBuildingManager_GrantFeatures
    {
        /**
         * When a character is being granted features, this patch will apply the effect of custom features.
         */

        internal static void Prefix(CharacterBuildingManager __instance, List<FeatureDefinition> grantedFeatures)
        {
            Models.CustomFeaturesContext.RecursiveGrantCustomFeatures(__instance.CurrentLocalHeroCharacter, grantedFeatures);
        }
    }
}
