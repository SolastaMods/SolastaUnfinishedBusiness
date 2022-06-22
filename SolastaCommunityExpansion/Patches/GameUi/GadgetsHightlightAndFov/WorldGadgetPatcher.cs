using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.Api;
using SolastaCommunityExpansion.Models;
using TA;

namespace SolastaCommunityExpansion.Patches.GameUi.GadgetsHightlightAndFov;

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

        if (GameUiContext.IsGadgetExit(__instance.UserGadget.GadgetBlueprint, true))
        {
            return;
        }

        var activator = DatabaseHelper.GetDefinition<GadgetDefinition>("Activator", "f05b58c5ba9444743a00057fd713faf2");
        var gameLocationCharacterService = ServiceRepository.GetService<IGameLocationCharacterService>();
        var gameLocationVisibilityService = ServiceRepository.GetService<IGameLocationVisibilityService>();
        var feedbackPosition = __instance.GameGadget.FeedbackPosition;

        // activators aren't detected in their original position so we handle them in a different way
        if (!__instance.GadgetDefinition == activator)
        {
            var position = new int3((int)feedbackPosition.x, (int)feedbackPosition.y, (int)feedbackPosition.z);

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

                var position = new int3((int)feedbackPosition.x + x, (int)feedbackPosition.y,
                    (int)feedbackPosition.z + z);

                foreach (var gameLocationCharacter in gameLocationCharacterService.PartyCharacters)
                {
                    visible = gameLocationVisibilityService.IsCellPerceivedByCharacter(position,
                        gameLocationCharacter);

                    if (visible)
                    {
                        return;
                    }
                }
            }
        }
    }
}
