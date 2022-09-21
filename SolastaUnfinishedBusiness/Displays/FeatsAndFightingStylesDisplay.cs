﻿using SolastaUnfinishedBusiness.Api.ModKit;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.Displays;

internal static class FeatsAndFightingStylesDisplay
{
    internal static void DisplayFeatsAndFightingStyles()
    {
        var displayToggle = Main.Settings.DisplayFeatsToggle;
        var sliderPos = Main.Settings.FeatSliderPosition;
        ModUi.DisplayDefinitions(
            Gui.Localize("ModUi/&Feats"),
            FeatsContext.SwitchFeat,
            FeatsContext.Feats,
            Main.Settings.FeatEnabled,
            ref displayToggle,
            ref sliderPos);
        Main.Settings.DisplayFeatsToggle = displayToggle;
        Main.Settings.FeatSliderPosition = sliderPos;

        static void FeatGroupsHeader()
        {
            UI.Label(Gui.Localize("ModUi/&FeatGroupsHelp"));
            UI.Label("");

            var toggle = Main.Settings.HideChildrenFeatsOnModUi;

            if (UI.Toggle(Gui.Localize("ModUi/&HideChildrenFeatsOnModUi"), ref toggle, UI.AutoWidth()))
            {
                Main.Settings.HideChildrenFeatsOnModUi = toggle;

                FeatsContext.UpdateFeatsAndGroupsChildrenDisplay(FeatsContext.FeatGroups);
            }

            UI.Label("");
        }

        displayToggle = Main.Settings.DisplayFeatGroupsToggle;
        sliderPos = Main.Settings.FeatGroupSliderPosition;
        ModUi.DisplayDefinitions(
            Gui.Localize("ModUi/&FeatGroups"),
            FeatsContext.SwitchFeatGroup,
            FeatsContext.FeatGroups,
            Main.Settings.FeatGroupEnabled,
            ref displayToggle,
            ref sliderPos,
            headerRendering: FeatGroupsHeader);
        Main.Settings.DisplayFeatGroupsToggle = displayToggle;
        Main.Settings.FeatGroupSliderPosition = sliderPos;

        displayToggle = Main.Settings.DisplayFightingStylesToggle;
        sliderPos = Main.Settings.FightingStyleSliderPosition;
        ModUi.DisplayDefinitions(
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
