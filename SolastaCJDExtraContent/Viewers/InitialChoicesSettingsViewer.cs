using UnityModManagerNet;
using ModKit;

namespace SolastaCJDExtraContent.Menus.Viewers
{
    public class InitialChoicesSettingsViewer : IMenuSelectablePage
    {
        public string Name => "Initial Choices Settings";

        public int Priority => 1;

        public static void DisplayInitialChoicesSettings()
        {
            int value;
            bool toggle;

            UI.Label("");
            UI.Label("Settings:".yellow());

            UI.Label("");
            toggle = Main.Settings.AlternateHuman;
            if (UI.Toggle("Enables the Alternate Human", ref toggle, 0, UI.AutoWidth()))
            {
                Main.Settings.AlternateHuman = toggle;
                Models.InitialChoicesContext.RefreshAllRacesInitialFeats();
            }

            value = Main.Settings.AllRacesInitialFeats;
            if (UI.Slider("Number of feats granted at first level", ref value, Settings.MIN_INITIAL_FEATS, Settings.MAX_INITIAL_FEATS, 0, "", UI.AutoWidth()))
            {
                Main.Settings.AllRacesInitialFeats = value;
                Models.InitialChoicesContext.RefreshAllRacesInitialFeats();
            }
        }

        public void OnGUI(UnityModManager.ModEntry modEntry)
        {
            UI.Label("Welcome to Solasta Content Expansion".yellow().bold());
            UI.Div();

            DisplayInitialChoicesSettings();
        }
    }
}