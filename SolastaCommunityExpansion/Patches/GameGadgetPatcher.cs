using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;
using SolastaCommunityExpansion.Api.Extensions;
using SolastaCommunityExpansion.Models;
using TA;

namespace SolastaCommunityExpansion.Patches;

internal static class GameGadgetPatcher
{
    //PATCH: HideExitsAndTeleportersGizmosIfNotDiscovered
    //hides certain element from the map on custom dungeons unless already discovered
    [HarmonyPatch(typeof(GameGadget), "ComputeIsRevealed")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class ComputeIsRevealed_Patch
    {
        internal static void Postfix(GameGadget __instance, ref bool __result)
        {
            if (!__instance.Revealed || Gui.GameLocation.UserLocation == null ||
                !Main.Settings.HideExitsAndTeleportersGizmosIfNotDiscovered)
            {
                return;
            }

            var userGadget = Gui.GameLocation.UserLocation.UserRooms
                .SelectMany(a => a.UserGadgets)
                .FirstOrDefault(b => b.UniqueName == __instance.UniqueNameId);

            if (userGadget == null || !GameUiContext.IsGadgetExit(userGadget.GadgetBlueprint))
            {
                return;
            }

            // reverts the revealed state and recalculates it
            __instance.revealed = false;
            __result = false;

            var x = (int)__instance.FeedbackPosition.x;
            var y = (int)__instance.FeedbackPosition.z;

            var feedbackPosition = new int3(x, 0, y);
            var referenceBoundingBox = new BoxInt(feedbackPosition, feedbackPosition);

            var gridAccessor = GridAccessor.Default;

            foreach (var position in referenceBoundingBox.EnumerateAllPositionsWithin())
            {
                if (!gridAccessor.Visited(position))
                {
                    continue;
                }

                var gameLocationService = ServiceRepository.GetService<IGameLocationService>();
                var worldGadgets = gameLocationService.WorldLocation.WorldSectors.SelectMany(ws => ws.WorldGadgets);
                var worldGadget = worldGadgets.FirstOrDefault(wg => wg.GameGadget == __instance);

                var isInvisible = __instance.IsInvisible();
                var isEnabled = __instance.IsEnabled();

                if (worldGadget != null)
                {
                    GameLocationManagerPatcher.ReadyLocation_Patch.SetTeleporterGadgetActiveAnimation(worldGadget,
                        isEnabled && !isInvisible);
                }

                __instance.revealed = true;
                __result = true;

                break;
            }
        }
    }

    //PATCH: HideExitsAndTeleportersGizmosIfNotDiscovered
    [HarmonyPatch(typeof(GameGadget), "SetCondition")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class SetCondition_Patch
    {
        internal static void Postfix(GameGadget __instance, int conditionIndex, bool state)
        {
            if (!Main.Settings.HideExitsAndTeleportersGizmosIfNotDiscovered)
            {
                return;
            }

            if (conditionIndex >= 0 && conditionIndex < __instance.conditionNames.Count)
            {
                var param = __instance.conditionNames[conditionIndex];

                Main.Log($"GameGadget_SetCondition {__instance.UniqueNameId}: {param} state = {state}");

#if DEBUG
                //Main.Log("GameGadget_SetCondition: " + string.Join(",", __instance.conditionNames.Select(n => $"{n}={__instance.CheckConditionName(n, true, false)}")));
#endif

                if ((param != GameGadgetExtensions.Enabled && param != GameGadgetExtensions.ParamEnabled) ||
                    !__instance.UniqueNameId.StartsWith(TagsDefinitions.Teleport))
                {
                    return;
                }

                var service = ServiceRepository.GetService<IGameLocationService>();

                if (service == null)
                {
                    return;
                }

                var worldGadget = service.WorldLocation.WorldSectors
                    .SelectMany(ws => ws.WorldGadgets)
                    .FirstOrDefault(wg => wg.GameGadget == __instance);

                if (worldGadget == null)
                {
                    return;
                }

                Main.Log($"GameGadget_SetCondition-setting-animation {__instance.UniqueNameId}: {state}");

                GameLocationManagerPatcher.ReadyLocation_Patch.SetTeleporterGadgetActiveAnimation(worldGadget, state);
            }
            else
            {
                Main.Log($"GameGadget_SetCondition {__instance.UniqueNameId}: condition index out of range.");
            }
        }
    }
    
    //TODO: Check if this patch is still required
#if false
    [HarmonyPatch(typeof(GameGadget), "SetCondition")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    // Not fixed as of 1.3.17
    internal static class GameGadget_SetCondition
    {
        /// <summary>
        /// Fix issue where a button activator fires Triggered event with state=true first time and correctly activates attached gadget,
        /// but fires Triggered event with state=false second time and doesn't activate attached gadget.
        /// </summary>
        /// <param name="__instance"></param>
        /// <param name="conditionIndex"></param>
        /// <param name="state"></param>
        /// <param name="__instance.conditionNames"></param>
        public static void Postfix(GameGadget __instance, int conditionIndex, bool state, List<string> __instance.conditionNames)
        {
            if (!Main.Settings.BugFixButtonActivatorTriggerIssue)
            {
                return;
            }

            if (conditionIndex >= 0 && conditionIndex < __instance.conditionNames.Count)
            {
                var param = __instance.conditionNames[conditionIndex];

                // NOTE: only handling 'button activator'
                // TODO: check other activators for same issue
                if (param == GameGadgetExtensions.Triggered && !state && __instance.UniqueNameId.StartsWith("ActivatorButton"))
                {
                    Main.Log($"GameGadget_SetCondition: Resetting '{param}' to true.");

                    // Reset 'Triggered' to true otherwise we have to press the activator twice
                    __instance.SetCondition(conditionIndex, true, new List<GameLocationCharacter>());
                }
            }
        }
    }
#endif
}
