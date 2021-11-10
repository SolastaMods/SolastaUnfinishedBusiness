using ModKit;
using SolastaCommunityExpansion.Models;

namespace SolastaCommunityExpansion.Viewers.Displays
{
    internal static class ToolsDisplay
    {
        // private static readonly string reqRestart = "[requires restart]".italic().red();

        internal static void DisplayTools()
        {
            bool toggle;

            UI.Label("");

            toggle = Main.Settings.EnableCheatMenuDuringGameplay;
            if (UI.Toggle("Enables the cheats menu during gameplay", ref toggle, 0, UI.AutoWidth()))
            {
                Main.Settings.EnableCheatMenuDuringGameplay = toggle;
            }

            toggle = Main.Settings.NoExperienceOnLevelUp;
            if (UI.Toggle("No experience is required to level up", ref toggle, 0, UI.AutoWidth()))
            {
                Main.Settings.NoExperienceOnLevelUp = toggle;
            }
        }
    }
}
