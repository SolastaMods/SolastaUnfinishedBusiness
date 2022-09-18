using System.Diagnostics.CodeAnalysis;
using HarmonyLib;

namespace SolastaUnfinishedBusiness.Patches;

internal static class GameMenuModalPatcher
{
    //PATCH: enables the cheats window during gameplay
    [HarmonyPatch(typeof(GameMenuModal), "SetButtonAvailability")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class SetButtonAvailability_Patch
    {
        internal static void Postfix(GameMenuModal __instance, GameMenuModal.MenuButtonIndex index)
        {
            if (!Main.Settings.EnableCheatMenu || index != GameMenuModal.MenuButtonIndex.Cheats)
            {
                return;
            }

            __instance.buttonCanvases[(int)index].gameObject.SetActive(true);
            __instance.buttonCanvases[(int)index].interactable = true;
            __instance.buttonCanvases[(int)index].alpha = 1f;
        }
    }
}
