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
