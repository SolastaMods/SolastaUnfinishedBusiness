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

        internal static bool Prefix(GameGadget __instance, ref bool __result)
        {
            if (!Main.Settings.EnableAdditionalIconsOnLevelMap || Gui.GameLocation.UserLocation == null)
            {
                return true;
            }

            if (!__instance.Revealed)
            {
                BoxInt referenceBoundingBox;
                var gadgetBlueprint = Gui.GameLocation.UserLocation.GadgetsByName[__instance.UniqueNameId].GadgetBlueprint;

                if (Array.IndexOf(gadgetBlueprintsToRevealAfterDiscovery, gadgetBlueprint) != -1)
                {
                    var x = (int)__instance.FeedbackPosition.x;
                    var y = (int)__instance.FeedbackPosition.z;
                    var position = new int3(x, 0, y);

                    referenceBoundingBox = new BoxInt(position, position);
                }
                else
                {
                    referenceBoundingBox = __instance.ReferenceBoundingBox;
                }

                if (referenceBoundingBox.IsValid)
                {
                    var gridAccessor = GridAccessor.Default;

                    foreach (var position in referenceBoundingBox.EnumerateAllPositionsWithin())
                    {
                        if (gridAccessor.Visited(position))
                        {
                            AccessTools.Field(__instance.GetType(), "revealed").SetValue(__instance, true);

                            break;
                        }
                    }
                }
                else
                {
                    AccessTools.Field(__instance.GetType(), "revealed").SetValue(__instance, true);
                }
            }

            __result = __instance.Revealed;

            return false;
        }
    }
}
