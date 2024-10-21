using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Api.ModKit;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.Displays;

internal static class BackgroundsAndRacesDisplay
{
    private static bool _displayTabletop;

    internal static void DisplayBackgroundsAndRaces()
    {
        UI.Label();

        using (UI.HorizontalScope())
        {
            UI.ActionButton(Gui.Localize("ModUi/&DocsBackgrounds").Bold().Khaki(),
                () => UpdateContext.OpenDocumentation("Backgrounds.md"), UI.Width(150f));
            UI.ActionButton(Gui.Localize("ModUi/&DocsRaces").Bold().Khaki(),
                () => UpdateContext.OpenDocumentation("Races.md"), UI.Width(150f));
            UI.ActionButton(Gui.Localize("ModUi/&DocsSubraces").Bold().Khaki(),
                () => UpdateContext.OpenDocumentation("Subraces.md"), UI.Width(150f));
        }

        UI.Label();

        var toggle = Main.Settings.EnableFlexibleBackgrounds;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableFlexibleBackgrounds"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableFlexibleBackgrounds = toggle;
            FlexibleBackgroundsContext.SwitchFlexibleBackgrounds();
        }

        toggle = Main.Settings.EnableFlexibleRaces;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableFlexibleRaces"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableFlexibleRaces = toggle;
            FlexibleRacesContext.SwitchFlexibleRaces();
        }

        UI.Label();

        toggle = Main.Settings.ChangeDragonbornElementalBreathUsages;
        if (UI.Toggle(Gui.Localize("ModUi/&ChangeDragonbornElementalBreathUsages"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.ChangeDragonbornElementalBreathUsages = toggle;
            CharacterContext.SwitchDragonbornElementalBreathUsages();
        }


        toggle = Main.Settings.EnableAlternateHuman;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableAlternateHuman"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableAlternateHuman = toggle;
            CharacterContext.SwitchFirstLevelTotalFeats();
        }

        UI.Label();

        toggle = Main.Settings.DisableSenseDarkVisionFromAllRaces;
        if (UI.Toggle(Gui.Localize("ModUi/&DisableSenseDarkVisionFromAllRaces"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.DisableSenseDarkVisionFromAllRaces = toggle;
        }

        toggle = Main.Settings.DisableSenseSuperiorDarkVisionFromAllRaces;
        if (UI.Toggle(Gui.Localize("ModUi/&DisableSenseSuperiorDarkVisionFromAllRaces"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.DisableSenseSuperiorDarkVisionFromAllRaces = toggle;
        }

        UI.Label();

        toggle = Main.Settings.AddDarknessPerceptiveToDarkRaces;
        if (UI.Toggle(Gui.Localize("ModUi/&AddDarknessPerceptiveToDarkRaces"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.AddDarknessPerceptiveToDarkRaces = toggle;
            CharacterContext.SwitchDarknessPerceptive();
        }

        UI.Label();

        toggle = Main.Settings.RaceLightSensitivityApplyOutdoorsOnly;
        if (UI.Toggle(Gui.Localize("ModUi/&RaceLightSensitivityApplyOutdoorsOnly"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.RaceLightSensitivityApplyOutdoorsOnly = toggle;
        }

        UI.Label();
        UI.Label();

        using (UI.HorizontalScope())
        {
            toggle =
                Main.Settings.DisplayBackgroundsToggle &&
                Main.Settings.DisplayRacesToggle &&
                Main.Settings.DisplaySubracesToggle;

            if (UI.Toggle(Gui.Localize("ModUi/&ExpandAll"), ref toggle, UI.Width(ModUi.PixelsPerColumn)))
            {
                Main.Settings.DisplayBackgroundsToggle = toggle;
                Main.Settings.DisplayRacesToggle = toggle;
                Main.Settings.DisplaySubracesToggle = toggle;
            }

            toggle =
                BackgroundsContext.Backgrounds.Count == Main.Settings.BackgroundEnabled.Count &&
                RacesContext.Races.Count == Main.Settings.RaceEnabled.Count &&
                RacesContext.Subraces.Count == Main.Settings.SubraceEnabled.Count;

            if (UI.Toggle(Gui.Localize("ModUi/&SelectAll"), ref toggle, UI.Width(ModUi.PixelsPerColumn)))
            {
                foreach (var background in BackgroundsContext.Backgrounds)
                {
                    BackgroundsContext.Switch(background, toggle);
                }

                foreach (var race in RacesContext.Races)
                {
                    RacesContext.Switch(race, toggle);
                }

                foreach (var subrace in RacesContext.Subraces)
                {
                    RacesContext.SwitchSubrace(subrace, toggle);
                }
            }

            toggle = _displayTabletop;
            if (UI.Toggle(Gui.Localize("ModUi/&SelectTabletop"), ref toggle, UI.Width(ModUi.PixelsPerColumn)))
            {
                foreach (var background in BackgroundsContext.Backgrounds)
                {
                    BackgroundsContext.Switch(background, toggle && ModUi.TabletopDefinitions.Contains(background));
                }

                foreach (var race in RacesContext.Races)
                {
                    RacesContext.Switch(race, toggle && ModUi.TabletopDefinitions.Contains(race));
                }

                foreach (var subrace in RacesContext.Subraces)
                {
                    RacesContext.SwitchSubrace(subrace, toggle && ModUi.TabletopDefinitions.Contains(subrace));
                }
            }
        }

        UI.Div();

        var displayToggle = Main.Settings.DisplayBackgroundsToggle;
        var sliderPos = Main.Settings.BackgroundSliderPosition;
        var isBackgroundTabletop = ModUi.DisplayDefinitions(
            Gui.Localize("ModUi/&Backgrounds"),
            BackgroundsContext.Switch,
            BackgroundsContext.Backgrounds,
            Main.Settings.BackgroundEnabled,
            ref displayToggle,
            ref sliderPos);
        Main.Settings.DisplayBackgroundsToggle = displayToggle;
        Main.Settings.BackgroundSliderPosition = sliderPos;

        displayToggle = Main.Settings.DisplayRacesToggle;
        sliderPos = Main.Settings.RaceSliderPosition;
        var isRaceTabletop = ModUi.DisplayDefinitions(
            Gui.Localize("ModUi/&Races"),
            RacesContext.Switch,
            RacesContext.Races,
            Main.Settings.RaceEnabled,
            ref displayToggle,
            ref sliderPos);
        Main.Settings.DisplayRacesToggle = displayToggle;
        Main.Settings.RaceSliderPosition = sliderPos;

        displayToggle = Main.Settings.DisplaySubracesToggle;
        sliderPos = Main.Settings.SubraceSliderPosition;
        var isSubraceTabletop = ModUi.DisplayDefinitions(
            Gui.Localize("ModUi/&Subraces"),
            RacesContext.SwitchSubrace,
            RacesContext.Subraces,
            Main.Settings.SubraceEnabled,
            ref displayToggle,
            ref sliderPos);
        Main.Settings.DisplaySubracesToggle = displayToggle;
        Main.Settings.SubraceSliderPosition = sliderPos;

        _displayTabletop = isBackgroundTabletop && isRaceTabletop && isSubraceTabletop;

#if false
        displayToggle = Main.Settings.DisplayDeitiesToggle;
        sliderPos = Main.Settings.DeitySliderPosition;
        ModUi.DisplayDefinitions(
            Gui.Localize("ModUi/&Deities"),
            DeitiesContext.Switch,
            DeitiesContext.Deities,
            Main.Settings.DeityEnabled,
            ref displayToggle,
            ref sliderPos);
        Main.Settings.DisplayDeitiesToggle = displayToggle;
        Main.Settings.DeitySliderPosition = sliderPos;
#endif

        UI.Label();
    }
}
