using System.Diagnostics.CodeAnalysis;
using HarmonyLib;

namespace SolastaCommunityExpansion.Patches.GameUi.LevelUp;

[HarmonyPatch(typeof(CharacterStageDeitySelectionPanel), "Compare", typeof(DeityDefinition),
    typeof(DeityDefinition))]
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

[HarmonyPatch(typeof(CharacterStageDeitySelectionPanel), "Compare", typeof(CharacterSubclassDefinition),
    typeof(CharacterSubclassDefinition))]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class CharacterStageDeitySelectionPanel_Compare_CharacterSubclassDefinition
{
    internal static void Postfix(CharacterSubclassDefinition left, CharacterSubclassDefinition right,
        ref int __result)
    {
        if (!Main.Settings.EnableSortingDeities)
        {
            return;
        }

        __result = left.FormatTitle().CompareTo(right.FormatTitle());
    }
}
