using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class GameMenuModalPatcher
{
    [HarmonyPatch(typeof(GameMenuModal), nameof(GameMenuModal.SetButtonAvailability))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class SetButtonAvailability_Patch
    {
        [UsedImplicitly]
        public static void Postfix(GameMenuModal __instance, GameMenuModal.MenuButtonIndex index)
        {
            //PATCH: enables the cheats window during gameplay
            if (!SettingsContext.GuiModManagerInstance.EnableCheatMenu || index != GameMenuModal.MenuButtonIndex.Cheats)
            {
                return;
            }

            __instance.buttonCanvases[(int)index].gameObject.SetActive(true);
            __instance.buttonCanvases[(int)index].interactable = true;
            __instance.buttonCanvases[(int)index].alpha = 1f;
        }
    }
}
