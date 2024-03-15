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
                Main.Settings.DisplayRacesToggle;

            if (UI.Toggle(Gui.Localize("ModUi/&ExpandAll"), ref toggle, UI.Width(ModUi.PixelsPerColumn)))
            {
                Main.Settings.DisplayBackgroundsToggle = toggle;
                Main.Settings.DisplayRacesToggle = toggle;
            }

            toggle =
                BackgroundsContext.Backgrounds.Count == Main.Settings.BackgroundEnabled.Count &&
                RacesContext.Races.Count == Main.Settings.RaceEnabled.Count;

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

        _displayTabletop = isBackgroundTabletop && isRaceTabletop;

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
        using (UI.HorizontalScope())
        {
            UI.ActionButton("UB Races docs".Bold().Khaki(),
                () => UpdateContext.OpenDocumentation("UnfinishedBusinessRaces.md"), UI.Width(200f));
            20.Space();
            UI.ActionButton("Solasta Races docs".Bold().Khaki(),
                () => UpdateContext.OpenDocumentation("SolastaRaces.md"), UI.Width(200f));
        }

        UI.Label();
    }
}
