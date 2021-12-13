using System.Collections.Generic;
using System.Linq;

namespace SolastaCommunityExpansion.Models
{
    internal static class HeroControllerContext
    {
        internal static readonly string[] Controllers = new string[] { "Human", "Computer" };

        internal static readonly int[] ControllersChoices = new int[Settings.MAX_PARTY_SIZE];

        internal static bool IsOffGame => Gui.Game == null;

        internal static bool IsMultiplayer => ServiceRepository.GetService<INetworkingService>().IsMultiplayerGame;

        internal static List<GameLocationCharacter> PartyCharacters => ServiceRepository.GetService<IGameLocationCharacterService>().PartyCharacters;

        private static void UpdateControlledCharacters(int[] controllers)
        {
            for (var i = 0; i < PartyCharacters.Count; i++)
            {
                var controllerId = controllers[i] == 0 ? Settings.PLAYER_CONTROLLER_ID : Settings.DM_CONTROLLER_ID;

                PartyCharacters[i].ControllerId = controllerId;
            }

            Gui.ActivePlayerController.DirtyControlledCharacters();
        }

        internal static void Start()
        {
            if (Main.Settings.EnableControllersOverride)
            {
                UpdateControlledCharacters(ControllersChoices);
            }
        }

        internal static void Stop()
        {
            UpdateControlledCharacters(Enumerable.Repeat(0, Settings.MAX_PARTY_SIZE).ToArray());
        }
    }
}
