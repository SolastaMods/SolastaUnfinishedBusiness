using UnityModManagerNet;
using ModKit;

namespace SolastaContentExpansion.Viewers
{
    public class OtherSettingsViewer : IMenuSelectablePage
    {
        public string Name => "Other Settings";

        public int Priority => 4;

        public static void DisplaySettings()
        {
            bool toggle;

            UI.Label("");
            UI.Label("Settings:".yellow());

            toggle = Main.Settings.EnablesAsiAndFeat;
            if (UI.Toggle("Enables both ASI and Feat", ref toggle, 0, UI.AutoWidth()))
            {
                Main.Settings.EnablesAsiAndFeat = toggle;
                Models.AsiAndFeatContext.Switch(toggle);
            }

            toggle = Main.Settings.EnableFlexibleBackgrounds;
            if (UI.Toggle("Enables flexible backgrounds", ref toggle, 0, UI.AutoWidth()))
            {
                Main.Settings.EnableFlexibleBackgrounds = toggle;
                Models.FlexibleBackgroundsContext.Switch(toggle);
            }

            toggle = Main.Settings.EnableFlexibleRaces;
            if (UI.Toggle("Enables flexible races", ref toggle, 0, UI.AutoWidth()))
            {
                Main.Settings.EnableFlexibleRaces = toggle;
                Models.FlexibleRacesContext.Switch(toggle);
            }
        }

        private static void DisplaySpellPanelSettings()
        {
            UI.Label("");
            UI.Label("Spells Panel:".yellow());

            int intValue = Main.Settings.MaxSpellLevelsPerLine;
            if (UI.Slider("Max Levels per line", ref intValue, 3, 7, 5, "", UI.AutoWidth()))
            {
                Main.Settings.MaxSpellLevelsPerLine = intValue;
            }

            float floatValue = Main.Settings.SpellPanelGapBetweenLines;
            if (UI.Slider("Gap between spell lines", ref floatValue, 0f, 200f, 50f, 0, "", UI.AutoWidth()))
            {
                Main.Settings.SpellPanelGapBetweenLines = floatValue;
            }
        }

        public void OnGUI(UnityModManager.ModEntry modEntry)
        {
            UI.Label("Welcome to Solasta Content Expansion".yellow().bold());
            UI.Div();

            DisplaySettings();
            DisplaySpellPanelSettings();
        }
    }
}