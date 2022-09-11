using ModKit;
using SolastaUnfinishedBusiness.Models;
using static SolastaUnfinishedBusiness.Displays.Shared;

namespace SolastaUnfinishedBusiness.Displays;

internal static class FeatsAndFightingStylesDisplay
{
    internal static void DisplayFeatsAndFightingStyles()
    {
        var displayToggle = Main.Settings.DisplayFeatsToggle;
        var sliderPos = Main.Settings.FeatSliderPosition;
        DisplayDefinitions(
            Gui.Localize("ModUi/&Feats"),
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
            Gui.Localize("ModUi/&FightingStyles"),
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
