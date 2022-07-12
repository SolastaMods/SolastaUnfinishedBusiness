using System;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;

namespace SolastaCommunityExpansion.Patches.GameUi.LevelUp;

[HarmonyPatch(typeof(CharacterStageClassSelectionPanel), "Compare")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class CharacterStageClassSelectionPanel_Compare
{
    internal static void Postfix(CharacterClassDefinition left, CharacterClassDefinition right, ref int __result)
    {
        if (!Main.Settings.EnableSortingClasses)
        {
            return;
        }

        __result = String.Compare(left.FormatTitle(), right.FormatTitle(), StringComparison.CurrentCultureIgnoreCase);
    }
}
