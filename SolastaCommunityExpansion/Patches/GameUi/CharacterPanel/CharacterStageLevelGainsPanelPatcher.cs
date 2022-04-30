using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.CustomDefinitions;
using SolastaModApi.Infrastructure;

namespace SolastaCommunityExpansion.Patches.GameUi.CharacterPanel
{
    [HarmonyPatch(typeof(CharacterStageLevelGainsPanel), "EnumerateActiveClassFeatures")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class CharacterStageLevelGainsPanel_EnumerateActiveClassFeatures
    {
        internal static void Postfix(CharacterStageLevelGainsPanel __instance)
        {
            var features = __instance.GetField<List<FeatureDefinition>>("activeFeatures");
            features.RemoveAll(f => f is CustomFeatureDefinitionSet);
        }
    }
}
