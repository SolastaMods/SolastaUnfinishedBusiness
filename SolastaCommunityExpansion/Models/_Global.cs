using System.Collections.Generic;

namespace SolastaCommunityExpansion.Models
{
    // keep public for sidecars
    public static class Global
    {
        // holds the active player character when in battle
        public static GameLocationCharacter ActivePlayerCharacter { get; set; }

        // holds the current action from any character on the map
        public static CharacterAction CurrentAction { get; set; }

        // holds a collection of conditions that should display on char panel even if set to silent
        public static HashSet<ConditionDefinition> CharacterLabelEnabledConditions { get; } = new();

        // true if in a multiplayer game
        public static bool IsMultiplayer => ServiceRepository.GetService<INetworkingService>().IsMultiplayerGame;

        // true if not in game
        public static bool IsOffGame => Gui.Game == null;
    }
}
