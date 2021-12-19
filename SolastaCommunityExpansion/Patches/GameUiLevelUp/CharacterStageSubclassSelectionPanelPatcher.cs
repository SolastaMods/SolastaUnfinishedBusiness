using HarmonyLib;
using System.Diagnostics.CodeAnalysis;

namespace SolastaCommunityExpansion.Patches.GameUiLevelUp
{
    [HarmonyPatch(typeof(CharacterStageSubclassSelectionPanel), "FillSubclassFeatures")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class CharacterStageSubclassSelectionPanel_FillSubclassFeatures
    {
        internal static void Prefix(CharacterSubclassDefinition subclassDefinition)
        {
            if (!Main.Settings.FutureFeatureSorting)
            {
                return;
            }
            subclassDefinition.FeatureUnlocks.Sort((a, b) => a.Level - b.Level);
        }
    }
}
