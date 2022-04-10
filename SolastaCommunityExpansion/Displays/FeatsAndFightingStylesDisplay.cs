using ModKit;
using SolastaCommunityExpansion.Feats;
using SolastaCommunityExpansion.Models;
using static SolastaCommunityExpansion.Displays.Shared;

namespace SolastaCommunityExpansion.Displays
{
    internal static class FeatsAndFightingStylesDisplay
    {
        internal static void DisplayGeneral()
        {
            bool toggle;
            int intValue;

            UI.Label("");

            toggle = Main.Settings.DisplayFeatFightingStyleToggle;
            if (UI.DisclosureToggle("General:".yellow(), ref toggle, 200))
            {
                Main.Settings.DisplayFeatFightingStyleToggle = toggle;
            }

            if (!Main.Settings.DisplayFeatFightingStyleToggle)
            {
                return;
            }

            UI.Label("");

            intValue = Main.Settings.FeatPowerAttackModifier;
            if (UI.Slider("Power Attack".orange() + " modifier ".white() + RequiresRestart, ref intValue, 1, 6, 3, ""))
            {
                Main.Settings.FeatPowerAttackModifier = intValue;
                AcehighFeats.UpdatePowerAttackModifier();
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
                "Feats:".yellow(),
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
                "Fighting Styles:".yellow(),
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
