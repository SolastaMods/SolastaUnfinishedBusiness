using System.Diagnostics.CodeAnalysis;
using HarmonyLib;

namespace SolastaCommunityExpansion.Patches.GameUiScreenMap
{
    // hides item highlights in fog of war areas
    [HarmonyPatch(typeof(WorldGadget), "SetHighlightVisibility")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class WorldGadget_SetHighlightVisibility
    {
        internal static void Prefix(WorldGadget __instance, ref bool visible)
        {
            // IsCellPerceivedByCharacter returns false for any wall cell so ignoring wall placeable gadgets as well
            if (!Main.Settings.HideGadgetsInFogOfWar || !visible || !__instance.IsUserGadget || __instance.UserGadget.GadgetBlueprint.WallPlacement)
            {
                return;
            }

            var gameLocationCharacterService = ServiceRepository.GetService<IGameLocationCharacterService>();
            var gameLocationVisibilityService = ServiceRepository.GetService<IGameLocationVisibilityService>();
            var position = new TA.int3((int)__instance.GameGadget.FeedbackPosition.x, (int)__instance.GameGadget.FeedbackPosition.y, (int)__instance.GameGadget.FeedbackPosition.z);

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
