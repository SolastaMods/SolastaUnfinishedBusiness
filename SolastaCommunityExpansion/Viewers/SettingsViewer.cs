using UnityModManagerNet;
using ModKit;

namespace SolastaCommunityExpansion.Viewers
{
    public class SettingsViewer : IMenuSelectablePage
    {
        public string Name => "Settings";

        public int Priority => 0;

        private static string reqRestart = "[requires restart]".italic().red();

        private static void DisplayGameUiSettings()
        {
            bool toggle;
            int intValue;
            float floatValue;

            UI.Label("");
            UI.Label("Game UI:".yellow());
            UI.Label("");

            toggle = Main.Settings.AllowExtraKeyboardCharactersInNames;
            if (UI.Toggle("Allows extra keyboard characters in names", ref toggle, 0, UI.AutoWidth()))
            {
                Main.Settings.AllowExtraKeyboardCharactersInNames = toggle;
            }

            toggle = Main.Settings.HideMonsterHitPoints;
            if (UI.Toggle("Displays Monsters's health in steps of 25% / 50% / 75% / 100% instead of exact hit points", ref toggle, 0, UI.AutoWidth()))
            {
                Main.Settings.HideMonsterHitPoints = toggle;
            }

            toggle = Main.Settings.InvertAltBehaviorOnTooltips;
            if (UI.Toggle("Inverts ALT key behavior on tooltips", ref toggle, 0, UI.AutoWidth()))
            {
                Main.Settings.InvertAltBehaviorOnTooltips = toggle;
            }

            toggle = Main.Settings.AutoPauseOnVictory;
            if (UI.Toggle("Pauses the UI when victorious in battle", ref toggle, 0, UI.AutoWidth()))
            {
                Main.Settings.AutoPauseOnVictory = toggle;
            }

            UI.Label("");

            toggle = Main.Settings.PermanentSpeedUp;
            if (UI.Toggle("Permanently speeds battle up", ref toggle, 0, UI.AutoWidth()))
            {
                Main.Settings.PermanentSpeedUp = toggle;
            }

            UI.Label("");
            floatValue = Main.Settings.CustomTimeScale;
            if (UI.Slider("Battle timescale modifier".white(), ref floatValue, 1f, 50f, 1f, 1, "", UI.AutoWidth()))
            {
                Main.Settings.CustomTimeScale = floatValue;
            }

            UI.Label("");

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
            UI.Label("Welcome to Solasta Community Expansion".yellow().bold());
            UI.Div();

            if (!Main.Enabled) return;

            DisplayGameUiSettings();
        }
    }
}
