using UnityModManagerNet;
using ModKit;

namespace SolastaContentExpansion.Viewers
{
    public class SettingsViewer : IMenuSelectablePage
    {
        public string Name => "Settings";

        public int Priority => 1;

        public static void DisplaySettings()
        {
            int intValue;
            bool toggle;

            UI.Label("");
            UI.Label("Progression settings:".yellow());
            UI.Label("");

            toggle = Main.Settings.AlternateHuman;
            if (UI.Toggle("Enables the Alternate Human", ref toggle, 0, UI.AutoWidth()))
            {
                Main.Settings.AlternateHuman = toggle;
                Models.InitialChoicesContext.RefreshAllRacesInitialFeats();
            }

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

            intValue = Main.Settings.AllRacesInitialFeats;
            if (UI.Slider("Total feats granted at first level", ref intValue, Settings.MIN_INITIAL_FEATS, Settings.MAX_INITIAL_FEATS, 0, "", UI.AutoWidth()))
            {
                Main.Settings.AllRacesInitialFeats = intValue;
                Models.InitialChoicesContext.RefreshAllRacesInitialFeats();
            }
        }

        private static void DisplaySpellPanelSettings()
        {
            bool toggle;

            UI.Label("");
            UI.Label("Game UI Settings:".yellow());
            UI.Label("");

            toggle = Main.Settings.InvertAltBehaviorOnTooltips;
            if (UI.Toggle("Offers additional lore friendly names on Hero Creation", ref toggle, 0, UI.AutoWidth()))
            {
                Main.Settings.InvertAltBehaviorOnTooltips = toggle;
            }

            toggle = Main.Settings.InvertAltBehaviorOnTooltips;
            if (UI.Toggle("Inverts ALT key behavior on Tooltips", ref toggle, 0, UI.AutoWidth()))
            {
                Main.Settings.InvertAltBehaviorOnTooltips = toggle;
            }

            int intValue = Main.Settings.MaxSpellLevelsPerLine;
            if (UI.Slider("Max levels per line on Spell Panel", ref intValue, 3, 7, 5, "", UI.AutoWidth()))
            {
                Main.Settings.MaxSpellLevelsPerLine = intValue;
            }

            float floatValue = Main.Settings.SpellPanelGapBetweenLines;
            if (UI.Slider("Gap between spell lines on Spell Panel", ref floatValue, 0f, 200f, 50f, 0, "", UI.AutoWidth()))
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