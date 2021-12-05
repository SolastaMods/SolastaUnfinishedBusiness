using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using UnityEngine;

namespace SolastaCommunityExpansion.Patches.Cheats
{
    // use this patch to enable the cheats window during gameplay
    [HarmonyPatch(typeof(GameMenuModal), "SetButtonAvailability")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class GameMenuModal_SetButtonAvailability
    {
        internal static void Postfix(GameMenuModal.MenuButtonIndex index, CanvasGroup[] ___buttonCanvases)
        {
            if (Main.Settings.EnableCheatMenuDuringGameplay && index == GameMenuModal.MenuButtonIndex.Cheats)
            {
                ___buttonCanvases[(int)index].gameObject.SetActive(true);
                ___buttonCanvases[(int)index].interactable = true;
                ___buttonCanvases[(int)index].alpha = 1f;
            }
        }
    }
}
