using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;
using TA;
using static SolastaModApi.DatabaseHelper.GadgetBlueprints;

namespace SolastaCommunityExpansion.Patches.GameUiScreenMap
{
    // hides certain element from the map on custom dungeons unless already discovered
    [HarmonyPatch(typeof(GameGadget), "ComputeIsRevealed")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class GameGadget_ComputeIsRevealed
    {
        private static readonly GadgetBlueprint[] gadgetBlueprintsToRevealAfterDiscovery = new GadgetBlueprint[]
        {
            Exit,
            ExitMultiple,
            TeleporterIndividual,
            TeleporterParty,
            VirtualExit,
            VirtualExitMultiple,
        };

        internal static void Postfix(GameGadget __instance, ref bool __result)
        {
            if (!__instance.Revealed || Gui.GameLocation.UserLocation == null || !Main.Settings.EnableAdditionalIconsOnLevelMap )
            {
                return;
            }

            var userGadget = Gui.GameLocation.UserLocation.UserRooms
                .SelectMany(a => a.UserGadgets)
                .FirstOrDefault(b => b.UniqueName == __instance.UniqueNameId);        

            if (userGadget == null || Array.IndexOf(gadgetBlueprintsToRevealAfterDiscovery, userGadget.GadgetBlueprint) < 0)
            {
                return;
            }

            // reverts the revealed state and recalculates it
            var revealedField = AccessTools.Field(__instance.GetType(), "revealed");

            revealedField.SetValue(__instance, false);
            __result = false;

            var x = (int)__instance.FeedbackPosition.x;
            var y = (int)__instance.FeedbackPosition.z;

            var feedbackPosition = new int3(x, 0, y);
            var referenceBoundingBox = new BoxInt(feedbackPosition, feedbackPosition);
              
            var gridAccessor = GridAccessor.Default;

            foreach (var position in referenceBoundingBox.EnumerateAllPositionsWithin())
            {
                if (gridAccessor.Visited(position))
                {
                    var gameLocationService = ServiceRepository.GetService<IGameLocationService>();
                    var worldGadgets = gameLocationService.WorldLocation.WorldSectors.SelectMany(ws => ws.WorldGadgets);
                    var worldGadget = worldGadgets.FirstOrDefault(wg => wg.GameGadget == __instance);

                    var conditionNames = AccessTools.Field(__instance.GetType(), "conditionNames").GetValue(__instance) as List<string>;

                    var invisibleIndex = conditionNames.IndexOf("Invisible");
                    var isInvisible = invisibleIndex >= 0 && __instance.CurrentConditionStates[invisibleIndex];

                    var enabledIndex = conditionNames.IndexOf("Enabled");
                    var isEnabled = invisibleIndex >= 0 && __instance.CurrentConditionStates[enabledIndex];

                    GameLocationManager_ReadyLocation.SetGadgetVisibility(worldGadget, isEnabled && !isInvisible);

                    revealedField.SetValue(__instance, true);
                    __result = true;

                    break;
                }
            }
        }
    }
}
