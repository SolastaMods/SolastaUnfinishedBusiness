using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using TA;

namespace SolastaCommunityExpansion.Patches.GameUiScreenMap
{
    // hides certain element from the map on custom dungeons unless already discovered
    [HarmonyPatch(typeof(WorldGadget), "SetHighlightVisibility")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class WorldGadget_SetHighlightVisibility
    {
        internal static void Prefix(WorldGadget __instance, ref bool visible)
        {
            if (!Main.Settings.HideGadgetsInFogOfWar || !visible || !__instance.IsUserGadget)
            {
                return;
            }

            var gameLocationCharacterService = ServiceRepository.GetService<IGameLocationCharacterService>();
            var gameLocationVisibilityService = ServiceRepository.GetService<IGameLocationVisibilityService>();
            var position = new int3((int)__instance.GameGadget.FeedbackPosition.x, 0, (int)__instance.GameGadget.FeedbackPosition.z);

            visible = false;

            foreach (var gameLocationCharacter in gameLocationCharacterService.PartyCharacters)
            {
                visible |= gameLocationVisibilityService.IsCellPerceivedByCharacter(position, gameLocationCharacter);
            }
        }
    }
}
