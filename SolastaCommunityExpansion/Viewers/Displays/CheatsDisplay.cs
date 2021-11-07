using ModKit;
using SolastaCommunityExpansion.Models;

namespace SolastaCommunityExpansion.Viewers.Displays
{
    internal static class CheatsDisplay
    {
        private static readonly string reqRestart = "[requires restart]".italic().red();

        internal static void DisplayCheats()
        {
            bool toggle;

            UI.Label("");
            UI.Label("Cheats:".yellow());

            UI.Label("");

            toggle = Main.Settings.EnableRespec;
            if (UI.Toggle("Enables RESPEC", ref toggle, 0, UI.AutoWidth()))
            {
                Main.Settings.EnableRespec = toggle;
            }

            toggle = Main.Settings.NoExperienceOnLevelUp;
            if (UI.Toggle("No experience is required on level up", ref toggle, 0, UI.AutoWidth()))
            {
                Main.Settings.NoExperienceOnLevelUp = toggle;
            }

            toggle = Main.Settings.NoIdentification;
            if (UI.Toggle("Removes identification requirements " + reqRestart, ref toggle, 0, UI.AutoWidth()))
            {
                Main.Settings.NoIdentification = toggle;
                RemoveIdentificationContext.Load();
            }

            toggle = Main.Settings.NoAttunement;
            if (UI.Toggle("Removes attunement requirements " + reqRestart, ref toggle, 0, UI.AutoWidth()))
            {
                Main.Settings.NoAttunement = toggle;
                RemoveIdentificationContext.Load();
            }
        }
    }
}