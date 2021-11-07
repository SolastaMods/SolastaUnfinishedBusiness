using UnityModManagerNet;
using ModKit;

namespace SolastaCommunityExpansion.Viewers
{
    public class HeroesViewer : IMenuSelectablePage
    {
        public string Name => "Heroes";

        public int Priority => 1;

        private static string reqRestart = "[requires restart]".italic().red();

        public static void DisplayNewCharacterSettings()
        {
            int intValue;
            bool toggle;

            UI.Label("");
            UI.Label("New Character:".yellow());
            UI.Label("");

            toggle = Main.Settings.EnableEpicPoints;
            if (UI.Toggle("Enables Epic [17,15,13,12,10,8] array instead of Standard [15,14,13,12,10,8]", ref toggle, 0, UI.AutoWidth()))
            {
                Main.Settings.EnableEpicPoints = toggle;
                Models.EpicArrayContext.Load();
            }

            toggle = Main.Settings.EnableAlternateHuman;
            if (UI.Toggle("Enables the Alternate Human [+1 feat / +2 attribute choices / +1 skill]", ref toggle, 0, UI.AutoWidth()))
            {
                Main.Settings.EnableAlternateHuman = toggle;
                Models.InitialChoicesContext.RefreshAllRacesInitialFeats();
            }

            toggle = Main.Settings.EnableFlexibleBackgrounds;
            if (UI.Toggle("Enables flexible backgrounds [Select skill and tool proficiencies from backgrounds]", ref toggle, 0, UI.AutoWidth()))
            {
                Main.Settings.EnableFlexibleBackgrounds = toggle;
                Models.FlexibleBackgroundsContext.Switch(toggle);
            }

            toggle = Main.Settings.EnableFlexibleRaces;
            if (UI.Toggle("Enables flexible races [Assign ability score points instead of the racial defaults (High Elf has 3 points to assign instead of +2 Dex/+1 Int)]", ref toggle, 0, UI.AutoWidth()))
            {
                Main.Settings.EnableFlexibleRaces = toggle;
                Models.FlexibleRacesContext.Switch(toggle);
            }

            // TODO: vision changes only take effect when creating a character. not sure if new block label is clear enough on intentions or we need more explanation here.
            toggle = Main.Settings.DisableSenseDarkVisionFromAllRaces;
            if (UI.Toggle("Disables " + "Sense Dark Vision".orange() + " from all races " + reqRestart, ref toggle, 0, UI.AutoWidth()))
            {
                Main.Settings.DisableSenseDarkVisionFromAllRaces = toggle;
            }

            toggle = Main.Settings.DisableSenseSuperiorDarkVisionFromAllRaces;
            if (UI.Toggle("Disables " + "Superior Sense Dark Vision".orange() + " from all races " + reqRestart, ref toggle, 0, UI.AutoWidth()))
            {
                Main.Settings.DisableSenseSuperiorDarkVisionFromAllRaces = toggle;
            }

            toggle = Main.Settings.OfferAdditionalNames;
            if (UI.Toggle("Offers additional lore friendly names on character creation " + reqRestart, ref toggle, 0, UI.AutoWidth()))
            {
                Main.Settings.OfferAdditionalNames = toggle;
            }

            UI.Label("");
            intValue = Main.Settings.AllRacesInitialFeats;
            if (UI.Slider("Total feats granted at first level".white(), ref intValue, Settings.MIN_INITIAL_FEATS, Settings.MAX_INITIAL_FEATS, 0, "", UI.AutoWidth()))
            {
                Main.Settings.AllRacesInitialFeats = intValue;
                Models.InitialChoicesContext.RefreshAllRacesInitialFeats();
            }
        }

        private static void DisplayLevelUpSettings()
        {
            bool toggle;

            UI.Label("");
            UI.Label("Level Up:".yellow());
            UI.Label("");

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

            toggle = Main.Settings.EnablesAsiAndFeat;
            if (UI.Toggle("Enables both ASI and feat", ref toggle, 0, UI.AutoWidth()))
            {
                Main.Settings.EnablesAsiAndFeat = toggle;
                Models.AsiAndFeatContext.Switch(toggle);
            }
        }

        public void OnGUI(UnityModManager.ModEntry modEntry)
        {
            UI.Label("Welcome to Solasta Community Expansion".yellow().bold());
            UI.Div();

            if (!Main.Enabled) return;

            DisplayNewCharacterSettings();
            DisplayLevelUpSettings();
        }
    }
}
