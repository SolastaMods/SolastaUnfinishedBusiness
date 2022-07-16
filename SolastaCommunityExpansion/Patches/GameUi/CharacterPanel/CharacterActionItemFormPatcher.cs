using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.CustomUI;

namespace SolastaCommunityExpansion.Patches.GameUi.CharacterPanel;

// Applies skipping of attack modes when item form refreshes, so UI would display proper attack mode data
[HarmonyPatch(typeof(CharacterActionItemForm), "Refresh")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class CharacterActionItemForm_Refresh
{
    internal static void Prefix(CharacterActionItemForm __instance)
    {
        if (__instance.GuiCharacterAction is CustomGuiCharacterAction action)
        {
            action.ApplySkip();
        }
    }

    internal static void Postfix(CharacterActionItemForm __instance)
    {
        if (__instance.GuiCharacterAction is CustomGuiCharacterAction action)
        {
            action.RemoveSkip();
        }
    }
}
