using HarmonyLib;
using System.Diagnostics.CodeAnalysis;

namespace SolastaCommunityExpansion.Patches.GameUiLevelUp
{
    [HarmonyPatch(typeof(CharacterStageRaceSelectionPanel), "Compare")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class CharacterStageRaceSelectionPanel_Compare
    {
        internal static void Postfix(CharacterRaceDefinition left, CharacterRaceDefinition right, ref int __result)
        {
            if (!Main.Settings.EnableSortingRaces)
            {
                return;
            }

            __result = left.FormatTitle().CompareTo(right.FormatTitle());
        }
    }
}
