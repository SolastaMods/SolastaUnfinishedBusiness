using System.Diagnostics.CodeAnalysis;
using HarmonyLib;

namespace SolastaCommunityExpansion.Patches.GameUi.ScreenMap
{
    // disables item highlights not in party field of view
    [HarmonyPatch(typeof(WorldGadget), "SetHighlightVisibility")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class WorldGadget_SetHighlightVisibility
    {
        internal static void Prefix(WorldGadget __instance, ref bool visible)
        {
            if (!Main.Settings.AltOnlyHighlightItemsInPartyFieldOfView || !visible || !__instance.IsUserGadget)
            {
                return;
            }

            if (!Models.GameUiContext.IsGadgetExit(__instance.UserGadget.GadgetBlueprint, onlyWithGizmos: true))
            {
                return;
            }

            var gameLocationCharacterService = ServiceRepository.GetService<IGameLocationCharacterService>();
            var gameLocationVisibilityService = ServiceRepository.GetService<IGameLocationVisibilityService>();
            var feedbackPosition = __instance.GameGadget.FeedbackPosition;

            // activators aren't detected in their original position so we handle them in a different way
            if (!__instance.GadgetDefinition == SolastaModApi.DatabaseHelper.GadgetDefinitions.Activator)
            {
                var position = new TA.int3((int)feedbackPosition.x, (int)feedbackPosition.y, (int)feedbackPosition.z);

                foreach (var gameLocationCharacter in gameLocationCharacterService.PartyCharacters)
                {
                    visible = gameLocationVisibilityService.IsCellPerceivedByCharacter(position, gameLocationCharacter);

                    if (visible)
                    {
                        return;
                    }
                }

                return;
            }

            // scan activators surrounding cells
            for (var x = -1; x <= 1; x++)
            {
                for (var z = -1; z <= 1; z++)
                {
                    // jump original position
                    if (x == 0 && z == 0)
                    {
                        continue;
                    }

                    var position = new TA.int3((int)feedbackPosition.x + x, (int)feedbackPosition.y, (int)feedbackPosition.z + z);

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
