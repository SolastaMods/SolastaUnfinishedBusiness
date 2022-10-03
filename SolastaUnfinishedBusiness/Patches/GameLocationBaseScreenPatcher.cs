using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.Patches;

public static class GameLocationBaseScreenPatcher
{
    [HarmonyPatch(typeof(GameLocationBaseScreen), "HandleInput")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class HandleInput_Patch
    {
        public static bool Prefix(GameLocationBaseScreen __instance, InputCommands.Id command, ref bool __result)
        {
            //PATCH: prevents game from receive input if Mod UI is open
            if (UnityModManagerUIPatcher.ModManagerUI.IsOpen)
            {
                __result = true;

                return false;
            }

            //PATCH: handles all hotkeys defined in the mod
            GameUiContext.HandleInput(__instance, command);

            return true;
        }
    }
}
