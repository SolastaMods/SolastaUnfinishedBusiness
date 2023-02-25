using SolastaUnfinishedBusiness.Api.ModKit;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.Displays;

internal static class RacesClassesSubclassesDisplay
{
    internal static void DisplayClassesAndSubclasses()
    {
        var displayToggle = Main.Settings.DisplayRacesToggle;
        var sliderPos = Main.Settings.RaceSliderPosition;
        ModUi.DisplayDefinitions(
            Gui.Localize("ModUi/&Races"),
            RacesContext.Switch,
            RacesContext.Races,
            Main.Settings.RaceEnabled,
            ref displayToggle,
            ref sliderPos);
        Main.Settings.DisplayRacesToggle = displayToggle;
        Main.Settings.RaceSliderPosition = sliderPos;

        displayToggle = Main.Settings.DisplayBackgroundsToggle;
        sliderPos = Main.Settings.BackgroundSliderPosition;
        ModUi.DisplayDefinitions(
            Gui.Localize("ModUi/&Backgrounds"),
            BackgroundsContext.Switch,
            BackgroundsContext.Backgrounds,
            Main.Settings.BackgroundEnabled,
            ref displayToggle,
            ref sliderPos);
        Main.Settings.DisplayBackgroundsToggle = displayToggle;
        Main.Settings.BackgroundSliderPosition = sliderPos;

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

        displayToggle = Main.Settings.DisplayClassesToggle;
        sliderPos = Main.Settings.ClassSliderPosition;
        ModUi.DisplayDefinitions(
            Gui.Localize("ModUi/&Classes"),
            ClassesContext.Switch,
            ClassesContext.Classes,
            Main.Settings.ClassEnabled,
            ref displayToggle,
            ref sliderPos);
        Main.Settings.DisplayClassesToggle = displayToggle;
        Main.Settings.ClassSliderPosition = sliderPos;

        displayToggle = Main.Settings.DisplaySubclassesToggle;
        sliderPos = Main.Settings.SubclassSliderPosition;
        ModUi.DisplayDefinitions(
            Gui.Localize("ModUi/&Subclasses"),
            SubclassesContext.Switch,
            SubclassesContext.Subclasses,
            Main.Settings.SubclassEnabled,
            ref displayToggle,
            ref sliderPos);
        Main.Settings.DisplaySubclassesToggle = displayToggle;
        Main.Settings.SubclassSliderPosition = sliderPos;

        UI.Label();
    }
}
