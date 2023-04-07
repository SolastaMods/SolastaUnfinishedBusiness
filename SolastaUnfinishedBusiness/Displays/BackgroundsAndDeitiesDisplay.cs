using SolastaUnfinishedBusiness.Api.ModKit;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.Displays;

internal static class BackgroundsAndDeitiesDisplay
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
