using ModKit;
using SolastaCommunityExpansion.Models;

namespace SolastaCommunityExpansion.Viewers.Displays
{
    internal static class CharacterDisplay
    {
        private static readonly string reqRestart = "[requires restart]".italic().red();

        internal static void DisplayCharacter()
        {
            int intValue;
            bool toggle;

            UI.Label("");
            //UI.Label("Progression:".yellow());
            //UI.Label("");

            toggle = Main.Settings.EnableLevel20;
            if (UI.Toggle("Enables Level 20 " + reqRestart, ref toggle, 0, UI.AutoWidth()))
            {
                Main.Settings.EnableLevel20 = toggle;
            }

            toggle = Main.Settings.EnableRespec;
            if (UI.Toggle("Enables RESPEC", ref toggle, 0, UI.AutoWidth()))
            {
                Main.Settings.EnableRespec = toggle;
            }

            UI.Label("");

            toggle = Main.Settings.EnablesAsiAndFeat;
            if (UI.Toggle("Enables both ASI and feat", ref toggle, 0, UI.AutoWidth()))
            {
                Main.Settings.EnablesAsiAndFeat = toggle;
                AsiAndFeatContext.Switch(toggle);
            }

            toggle = Main.Settings.EnableEpicPoints;
            if (UI.Toggle("Enables epic [17,15,13,12,10,8] array instead of standard [15,14,13,12,10,8]", ref toggle, 0, UI.AutoWidth()))
            {
                Main.Settings.EnableEpicPoints = toggle;
                EpicArrayContext.Load();
            }

            toggle = Main.Settings.EnableAlternateHuman;
            if (UI.Toggle("Enables the Alternate Human [+1 feat / +2 attribute choices / +1 skill]", ref toggle, 0, UI.AutoWidth()))
            {
                Main.Settings.EnableAlternateHuman = toggle;
                InitialChoicesContext.RefreshAllRacesInitialFeats();
            }

            toggle = Main.Settings.EnableFlexibleBackgrounds;
            if (UI.Toggle("Enables flexible backgrounds [Select skill and tool proficiencies from backgrounds]", ref toggle, 0, UI.AutoWidth()))
            {
                Main.Settings.EnableFlexibleBackgrounds = toggle;
                FlexibleBackgroundsContext.Switch(toggle);
            }

            toggle = Main.Settings.EnableFlexibleRaces;
            if (UI.Toggle("Enables flexible races [Assign ability score points instead of the racial defaults]\n" + "High Elf has 3 points to assign instead of +2 Dex / +1 Int".italic().red(), ref toggle, 0, UI.AutoWidth()))
            {
                Main.Settings.EnableFlexibleRaces = toggle;
                FlexibleRacesContext.Switch(toggle);
            }

            UI.Label("");
            intValue = Main.Settings.AllRacesInitialFeats;
            if (UI.Slider("Total feats granted at first level".white(), ref intValue, Settings.MIN_INITIAL_FEATS, Settings.MAX_INITIAL_FEATS, 0, "", UI.AutoWidth()))
            {
                Main.Settings.AllRacesInitialFeats = intValue;
                InitialChoicesContext.RefreshAllRacesInitialFeats();
            }
        }
    }
}
