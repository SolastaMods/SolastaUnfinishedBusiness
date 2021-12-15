using HarmonyLib;
using System.Diagnostics.CodeAnalysis;

namespace SolastaCommunityExpansion.Patches.FutureFeatureSorting
{
    [HarmonyPatch(typeof(CharacterStageDeitySelectionPanel), "FillSubclassFeatures")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class CharacterStageDeitySelectionPanel_FillSubclassFeatures
    {
        internal static void Prefix(CharacterSubclassDefinition currentSubclassDefinition)
        {
            if (!Main.Settings.FutureFeatureSorting)
            {
                return;
            }
            currentSubclassDefinition.FeatureUnlocks.Sort((a, b) => a.Level - b.Level);
        }
    }
}
