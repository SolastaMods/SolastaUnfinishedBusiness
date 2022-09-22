using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;
using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.Models;
using TA;

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

            if (conditionIndex < 0 || conditionIndex >= __instance.conditionNames.Count)
            {
                return;
            }

            var param = __instance.conditionNames[conditionIndex];

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

            GameLocationManagerPatcher.ReadyLocation_Patch.SetTeleporterGadgetActiveAnimation(worldGadget, state);
        }
    }
}
