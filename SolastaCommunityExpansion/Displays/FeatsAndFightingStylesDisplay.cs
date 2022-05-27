using ModKit;
using SolastaCommunityExpansion.Models;
using static SolastaCommunityExpansion.Displays.Shared;

namespace SolastaCommunityExpansion.Displays
{
    internal static class FeatsAndFightingStylesDisplay
    {
        internal static void DisplayGeneral()
        {
            bool toggle;

            UI.Label("");

            toggle = Main.Settings.DisplayFeatFightingStyleToggle;
            if (UI.DisclosureToggle(Gui.Format("ModUi/&General"), ref toggle, 200))
            {
                Main.Settings.DisplayFeatFightingStyleToggle = toggle;
            }

            if (!Main.Settings.DisplayFeatFightingStyleToggle)
            {
            }
        }

        internal static void DisplayFeatsAndFightingStyles()
        {
            bool displayToggle;
            int sliderPos;

            DisplayGeneral();

            displayToggle = Main.Settings.DisplayFeatsToggle;
            sliderPos = Main.Settings.FeatSliderPosition;
            DisplayDefinitions(
                Gui.Format("ModUi/&Feats"),
                FeatsContext.Switch,
                FeatsContext.Feats,
                Main.Settings.FeatEnabled,
                ref displayToggle,
                ref sliderPos);
            Main.Settings.DisplayFeatsToggle = displayToggle;
            Main.Settings.FeatSliderPosition = sliderPos;

            displayToggle = Main.Settings.DisplayFightingStylesToggle;
            sliderPos = Main.Settings.FightingStyleSliderPosition;
            DisplayDefinitions(
                Gui.Format("ModUi/&FightingStyles"),
                FightingStyleContext.Switch,
                FightingStyleContext.FightingStyles,
                Main.Settings.FightingStyleEnabled,
                ref displayToggle,
                ref sliderPos);
            Main.Settings.DisplayFightingStylesToggle = displayToggle;
            Main.Settings.FightingStyleSliderPosition = sliderPos;

            UI.Label("");
        }
    }
}
