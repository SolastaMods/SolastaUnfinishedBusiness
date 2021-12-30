using System;
using System.Diagnostics.CodeAnalysis;
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
        //
        // TODO: @ImpPhil, tweak this array per your needs on the map feature
        //
        private static readonly GadgetBlueprint[] gadgetBlueprintsToRevealAfterDiscovery = new GadgetBlueprint[]
        {
            Exit,
            Node,
            TeleporterIndividual,
            TeleporterParty
        };

        internal static void Postfix(GameGadget __instance, ref bool __result)
        {
            if (!__instance.Revealed || Gui.GameLocation.UserLocation == null || !Main.Settings.EnableAdditionalIconsOnLevelMap )
            {
                return;
            }

            var gadgetBlueprint = Gui.GameLocation.UserLocation.GadgetsByName[__instance.UniqueNameId].GadgetBlueprint;

            if (Array.IndexOf(gadgetBlueprintsToRevealAfterDiscovery, gadgetBlueprint) == -1)
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
                    revealedField.SetValue(__instance, true);
                    __result = true;

                    break;
                }
            }
        }
    }
}
