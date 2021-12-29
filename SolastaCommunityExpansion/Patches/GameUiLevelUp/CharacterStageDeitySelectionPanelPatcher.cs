using HarmonyLib;
using System.Diagnostics.CodeAnalysis;

namespace SolastaCommunityExpansion.Patches.GameUiLevelUp
{
    [HarmonyPatch(typeof(CharacterStageDeitySelectionPanel), "FillSubclassFeatures")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class CharacterStageDeitySelectionPanel_FillSubclassFeatures
    {
        internal static void Prefix(CharacterSubclassDefinition currentSubclassDefinition)
        {
            if (!Main.Settings.EnableSortingFutureFeatures)
            {
                return;
            }

            currentSubclassDefinition.FeatureUnlocks.Sort((a, b) => a.Level - b.Level);
        }
    }

    [HarmonyPatch(typeof(CharacterStageDeitySelectionPanel), "Compare", new System.Type[] { typeof(DeityDefinition), typeof(DeityDefinition) })]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class CharacterStageDeitySelectionPanel_Compare_DeityDefinition
    {
        internal static void Postfix(DeityDefinition left, DeityDefinition right, ref int __result)
        {
            if (!Main.Settings.EnableSortingDeities)
            {
                return;
            }

            __result = left.FormatTitle().CompareTo(right.FormatTitle());
        }
    }

    [HarmonyPatch(typeof(CharacterStageDeitySelectionPanel), "Compare", new System.Type[] { typeof(CharacterSubclassDefinition), typeof(CharacterSubclassDefinition) })]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class CharacterStageDeitySelectionPanel_Compare_CharacterSubclassDefinition
    {
        internal static void Postfix(CharacterSubclassDefinition left, CharacterSubclassDefinition right, ref int __result)
        {
            if (!Main.Settings.EnableSortingDeities)
            {
                return;
            }

            __result = left.FormatTitle().CompareTo(right.FormatTitle());
        }
    }
}
