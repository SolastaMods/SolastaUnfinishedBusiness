using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Api.ModKit;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.Displays;

internal static class BackgroundsAndRacesDisplay
{
    internal static void DisplayBackgroundsAndDeities()
    {
        var displayToggle = Main.Settings.DisplayBackgroundsToggle;
        var sliderPos = Main.Settings.BackgroundSliderPosition;
        ModUi.DisplayDefinitions(
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
        ModUi.DisplayDefinitions(
            Gui.Localize("ModUi/&Races"),
            RacesContext.Switch,
            RacesContext.Races,
            Main.Settings.RaceEnabled,
            ref displayToggle,
            ref sliderPos,
            headerRendering: RacesHeader);
        Main.Settings.DisplayRacesToggle = displayToggle;
        Main.Settings.RaceSliderPosition = sliderPos;

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
