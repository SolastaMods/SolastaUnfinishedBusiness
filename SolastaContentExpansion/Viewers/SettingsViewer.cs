using UnityModManagerNet;
using ModKit;

namespace SolastaContentExpansion.Viewers
{
    public class SettingsViewer : IMenuSelectablePage
    {
        public string Name => "Settings";

        public int Priority => 1;

        private static string reqRestart = "[requires restart]".italic().red();

        public static void DisplaySettings()
        {
            int intValue;
            bool toggle;

            UI.Label("");
            UI.Label("Progression settings:".yellow());
            UI.Label("");

            toggle = Main.Settings.EnableAlternateHuman;
            if (UI.Toggle("Enables the Alternate Human [+2 attribute choices / +1 skill]", ref toggle, 0, UI.AutoWidth()))
            {
                Main.Settings.EnableAlternateHuman = toggle;
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

            // TODO- check if the vision changes only take effect when creating a character. If so we may want to make
            // this clear so players don't expect to be able to toggle mid-game.
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

            intValue = Main.Settings.AllRacesInitialFeats;
            if (UI.Slider("Total feats granted at first level".white(), ref intValue, Settings.MIN_INITIAL_FEATS, Settings.MAX_INITIAL_FEATS, 0, "", UI.AutoWidth()))
            {
                Main.Settings.AllRacesInitialFeats = intValue;
                Models.InitialChoicesContext.RefreshAllRacesInitialFeats();
            }
        }

        private static void DisplaySpellPanelSettings()
        {
            bool toggle;
            int intValue;
            float floatValue;

            UI.Label("");
            UI.Label("Game UI Settings:".yellow());
            UI.Label("");

            toggle = Main.Settings.OfferAdditionalNames;
            if (UI.Toggle("Offers additional lore friendly names " + reqRestart, ref toggle, 0, UI.AutoWidth()))
            {
                Main.Settings.OfferAdditionalNames = toggle;
            }

            toggle = Main.Settings.InvertAltBehaviorOnTooltips;
            if (UI.Toggle("Inverts ALT key behavior on Tooltips", ref toggle, 0, UI.AutoWidth()))
            {
                Main.Settings.InvertAltBehaviorOnTooltips = toggle;
            }

            toggle = Main.Settings.HideMonsterHitPoints;
            if (UI.Toggle("Displays Monsters's health in steps of 25/50/75/100% instead of exact hit points.", ref toggle, 0, UI.AutoWidth()))
            {
                Main.Settings.HideMonsterHitPoints = toggle;
            }

            toggle = Main.Settings.AutoPauseOnVictory;
            if (UI.Toggle("Pauses the UI when victorious in battle", ref toggle, 0, UI.AutoWidth()))
            {
                Main.Settings.AutoPauseOnVictory = toggle;
            }

            toggle = Main.Settings.PermanentSpeedUp;
            if (UI.Toggle("Permanently speeds battle up", ref toggle, 0, UI.AutoWidth()))
            {
                Main.Settings.PermanentSpeedUp = toggle;
            }

            floatValue = Main.Settings.CustomTimeScale;
            if (UI.Slider("Battle timescale modifier".white(), ref floatValue, 1f, 50f, 1f, 2, "", UI.AutoWidth()))
            {
                Main.Settings.CustomTimeScale = floatValue;
            }

            intValue = Main.Settings.MaxSpellLevelsPerLine;
            if (UI.Slider("Max levels per line on Spell Panel".white(), ref intValue, 3, 7, 5, "", UI.AutoWidth()))
            {
                Main.Settings.MaxSpellLevelsPerLine = intValue;
            }

            floatValue = Main.Settings.SpellPanelGapBetweenLines;
            if (UI.Slider("Gap between spell lines on Spell Panel".white(), ref floatValue, 0f, 200f, 50f, 0, "", UI.AutoWidth()))
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
