using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.Patches;

internal static class GameGadgetPatcher
{
    [HarmonyPatch(typeof(GameGadget), "ComputeIsRevealed")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class ComputeIsRevealed_Patch
    {
        internal static void Postfix(GameGadget __instance, ref bool __result)
        {
            //PATCH: HideExitsAndTeleportersGizmosIfNotDiscovered
            //hides certain element from the map on custom dungeons unless already discovered
            if (!Main.Settings.HideExitsAndTeleportersGizmosIfNotDiscovered
                || Gui.GameLocation.UserLocation == null
                || !__instance.Revealed)
            {
                return;
            }

            GameUiContext.ComputeIsRevealedExtended(__instance, ref __result);
        }
    }

    [HarmonyPatch(typeof(GameGadget), "SetCondition")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class SetCondition_Patch
    {
        internal static void Postfix(GameGadget __instance, int conditionIndex, bool state)
        {
            //PATCH: HideExitsAndTeleportersGizmosIfNotDiscovered
            if (!Main.Settings.HideExitsAndTeleportersGizmosIfNotDiscovered)
            {
                return;
            }

            GameUiContext.HideExitsAndTeleportersGizmosIfNotDiscovered(__instance, conditionIndex, state);
        }
    }
}
