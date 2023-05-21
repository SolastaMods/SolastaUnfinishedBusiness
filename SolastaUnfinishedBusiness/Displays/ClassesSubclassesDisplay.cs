using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Api.ModKit;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.Displays;

internal static class ClassesSubclassesDisplay
{
    internal static void DisplayClassesAndSubclasses()
    {
        var displayToggle = Main.Settings.DisplayClassesToggle;
        var sliderPos = Main.Settings.ClassSliderPosition;
        ModUi.DisplayDefinitions(
            Gui.Localize("ModUi/&Classes"),
            ClassesContext.Switch,
            ClassesContext.Classes,
            Main.Settings.ClassEnabled,
            ref displayToggle,
            ref sliderPos,
            headerRendering: ClassesHeader);
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
            ref sliderPos,
            headerRendering: SubclassesHeader);
        Main.Settings.DisplaySubclassesToggle = displayToggle;
        Main.Settings.SubclassSliderPosition = sliderPos;

        UI.Label();
    }

    private static void ClassesHeader()
    {
        using (UI.HorizontalScope())
        {
            UI.ActionButton("UB Classes Docs".Bold().Khaki(),
                () => UpdateContext.OpenDocumentation("UnfinishedBusinessClasses.md"), UI.Width((float)200));
            20.Space();
            UI.ActionButton("Solasta Classes Docs".Bold().Khaki(),
                () => UpdateContext.OpenDocumentation("SolastaClasses.md"), UI.Width((float)200));
        }

        UI.Label();
    }

    private static void SubclassesHeader()
    {
        using (UI.HorizontalScope())
        {
            UI.ActionButton("UB Subclasses Docs".Bold().Khaki(),
                () => UpdateContext.OpenDocumentation("UnfinishedBusinessSubclasses.md"), UI.Width((float)200));
            20.Space();
            UI.ActionButton("Solasta Subclasses Docs".Bold().Khaki(),
                () => UpdateContext.OpenDocumentation("SolastaSubclasses.md"), UI.Width((float)200));
        }

        UI.Label();
    }
}
