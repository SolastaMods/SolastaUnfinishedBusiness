using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class GameGadgetPatcher
{
    [HarmonyPatch(typeof(GameGadget), nameof(GameGadget.ComputeIsRevealed))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class ComputeIsRevealed_Patch
    {
        [UsedImplicitly]
        public static void Postfix(GameGadget __instance, ref bool __result)
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

    [HarmonyPatch(typeof(GameGadget), nameof(GameGadget.CheckHasActiveDetectedTrap))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class CheckHasActiveDetectedTrap_Patch
    {
        [UsedImplicitly]
        public static bool Prefix(GameGadget __instance, out bool __result)
        {
            //PATCH: fixes plant traps being shown on map even after destroyed
            //mostly copy of a base method with added check
            __result = __instance.CheckConditionName("Param_IsTrap", true, false) &&
                       __instance.CheckConditionName("Param_Enabled", true, true) &&
                       __instance.CheckConditionName("TrapDetected", true, true) &&
                       __instance.CheckConditionName("TrapDisarmed", false, false) &&
                       __instance.CheckConditionName("TrapTriggered", false, false) &&
                       //this is new check - traps triggered by shooting have `Triggered`, not `TrapTriggered` set to true
                       __instance.CheckConditionName("Triggered", false, false);
            return false;
        }
    }

    [HarmonyPatch(typeof(GameGadget), nameof(GameGadget.SetCondition))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class SetCondition_Patch
    {
        [UsedImplicitly]
        public static void Postfix(GameGadget __instance, int conditionIndex, bool state)
        {
            //BUGFIX: fix issue where a button activator fires Triggered event with state=true first time and
            // correctly activates attached gadget, but fires Triggered event with state=false second time and
            // doesn't activate attached gadget.
            if (conditionIndex >= 0 && conditionIndex < __instance.conditionNames.Count)
            {
                var param = __instance.conditionNames[conditionIndex];

                // NOTE: only handling 'button activator'
                // TODO: check other activators for same issue
                if (param == GameGadgetExtensions.Triggered && !state &&
                    __instance.UniqueNameId.StartsWith("ActivatorButton"))
                {
                    // Reset 'Triggered' to true otherwise we have to press the activator twice
                    __instance.SetCondition(conditionIndex, true, new List<GameLocationCharacter>());
                }
            }

            //PATCH: HideExitsAndTeleportersGizmosIfNotDiscovered
            if (!Main.Settings.HideExitsAndTeleportersGizmosIfNotDiscovered)
            {
                return;
            }

            GameUiContext.HideExitsAndTeleportersGizmosIfNotDiscovered(__instance, conditionIndex, state);
        }
    }
}
