using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.CustomDefinitions;

namespace SolastaCommunityExpansion.Patches.GameUi.LevelUp
{
    [HarmonyPatch(typeof(FeatureDefinitionFeatureSet), "FeatureSet", MethodType.Getter)]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class FeatureDefinitionFeatureSet_FeatureSet_Getter
    {
        internal static bool Prefix(FeatureDefinitionFeatureSet __instance, ref List<FeatureDefinition> __result)
        {
            if (__instance is not FeatureDefinitionFeatureSetDynamic featureDefinitionFeatureSetDynamic)
            {
                return true;
            }

            //__result = featureDefinitionFeatureSetDynamic.DynamicFeatureSet(featureDefinitionFeatureSetDynamic);

            return true;
        }
    }
}
