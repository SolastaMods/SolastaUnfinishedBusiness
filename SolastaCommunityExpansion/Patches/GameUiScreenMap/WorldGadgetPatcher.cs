using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using static SolastaModApi.DatabaseHelper.GadgetDefinitions;

namespace SolastaCommunityExpansion.Patches.GameUiScreenMap
{
    // hides item highlights in fog of war areas
    [HarmonyPatch(typeof(WorldGadget), "SetHighlightVisibility")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class WorldGadget_SetHighlightVisibility
    {
        //internal static void Prefix_Original_Solution_Faster_But_Not_Precise(WorldGadget __instance, ref bool visible)
        //{
        //    // IsCellPerceivedByCharacter returns false for any wall cell so ignoring wall placeable gadgets as well
        //    if (!Main.Settings.HideGadgetsInFogOfWar || !visible || !__instance.IsUserGadget || __instance.UserGadget.GadgetBlueprint.WallPlacement)
        //    {
        //        return;
        //    }

        //    var gameLocationCharacterService = ServiceRepository.GetService<IGameLocationCharacterService>();
        //    var gameLocationVisibilityService = ServiceRepository.GetService<IGameLocationVisibilityService>();
        //    var position = new TA.int3((int)__instance.GameGadget.FeedbackPosition.x, (int)__instance.GameGadget.FeedbackPosition.y, (int)__instance.GameGadget.FeedbackPosition.z);

        //    foreach (var gameLocationCharacter in gameLocationCharacterService.PartyCharacters)
        //    {
        //        visible = gameLocationVisibilityService.IsCellPerceivedByCharacter(position, gameLocationCharacter);

        //        if (visible)
        //        {
        //            return;
        //        }
        //    }
        //}

        internal static void Prefix(WorldGadget __instance, ref bool visible)
        {
            if (!Main.Settings.HideGadgetsInFogOfWar || !visible || !__instance.IsUserGadget)
            {
                return;
            }

            TA.int3 position;
            var gameLocationCharacterService = ServiceRepository.GetService<IGameLocationCharacterService>();
            var gameLocationVisibilityService = ServiceRepository.GetService<IGameLocationVisibilityService>();
            var feedbackPosition = __instance.GameGadget.FeedbackPosition;

            // scan the gadget cell
            position = new TA.int3((int)feedbackPosition.x, (int)feedbackPosition.y, (int)feedbackPosition.z);

            foreach (var gameLocationCharacter in gameLocationCharacterService.PartyCharacters)
            {
                visible = gameLocationVisibilityService.IsCellPerceivedByCharacter(position, gameLocationCharacter);

                if (visible)
                {
                    return;
                }
            }

            // doors and wall gadgets need some special treatment as they aren't perceived in their original positions
            if (!__instance.GadgetDefinition == Door || !__instance.UserGadget.GadgetBlueprint.WallPlacement)
            {
                return;
            }

            // scan doors or wall gadgets surrounding cells
            for (var x = -1; x <= 1; x++)
            {
                for (var z = -1; z <= 1; z++)
                {
                    // gadget cell was already scanned
                    if (x == 0 && z == 0)
                    {
                        continue;
                    }

                    position = new TA.int3((int)feedbackPosition.x + x, (int)feedbackPosition.y, (int)feedbackPosition.z + z);

                    foreach (var gameLocationCharacter in gameLocationCharacterService.PartyCharacters)
                    {
                        visible = gameLocationVisibilityService.IsCellPerceivedByCharacter(position, gameLocationCharacter);

                        if (visible)
                        {
                            return;
                        }
                    }
                }
            }
        }
    }
}
