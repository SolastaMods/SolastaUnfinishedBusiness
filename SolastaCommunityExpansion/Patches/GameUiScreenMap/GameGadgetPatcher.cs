using System;
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
            // Node,
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

            if (Array.IndexOf(gadgetBlueprintsToRevealAfterDiscovery, userGadget.GadgetBlueprint) < 0)
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

            AccessTools.Field(__instance.GetType(), "referenceBoundingBox").SetValue(__instance, referenceBoundingBox);

            var gridAccessor = GridAccessor.Default;

            foreach (var position in referenceBoundingBox.EnumerateAllPositionsWithin())
            {
                if (gridAccessor.Visited(position))
                {
                    revealedField.SetValue(__instance, true);
                    __result = true;

                    break;
                }
            }
        }
    }
}
