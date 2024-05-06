using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Api.ModKit;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.Displays;

internal static class BackgroundsAndRacesDisplay
{
    private static bool _displayTabletop;

    internal static void DisplayBackgroundsAndDeities()
    {
        UI.Label();
        UI.Label();

        using (UI.HorizontalScope())
        {
            var toggle =
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
            ref sliderPos,
            headerRendering: RacesHeader);
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
            ref sliderPos,
            headerRendering: RacesHeader);
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

    private static void RacesHeader()
    {
        UI.ActionButton("Races docs".Bold().Khaki(),
            () => UpdateContext.OpenDocumentation("Races.md"), UI.Width(200f));

        UI.Label();
    }
}
