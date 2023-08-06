using System.Linq;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Api.ModKit;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.Displays;

internal static class ClassesSubclassesDisplay
{
    internal static void DisplayClassesAndSubclasses()
    {
        UI.Label();

        using (UI.HorizontalScope())
        {
            UI.ActionButton("UB Classes Docs".Bold().Khaki(),
                () => UpdateContext.OpenDocumentation("UnfinishedBusinessClasses.md"), UI.Width((float)200));
            20.Space();
            UI.ActionButton("Solasta Classes Docs".Bold().Khaki(),
                () => UpdateContext.OpenDocumentation("SolastaClasses.md"), UI.Width((float)200));
        }

        UI.Label();

        using (UI.HorizontalScope())
        {
            UI.ActionButton("UB Subclasses Docs".Bold().Khaki(),
                () => UpdateContext.OpenDocumentation("UnfinishedBusinessSubclasses.md"), UI.Width((float)200));
            20.Space();
            UI.ActionButton("Solasta Subclasses Docs".Bold().Khaki(),
                () => UpdateContext.OpenDocumentation("SolastaSubclasses.md"), UI.Width((float)200));
        }

        UI.Label();

        using (UI.HorizontalScope())
        {
            var toggle = SubclassesContext.IsAllSetSelected();
            if (UI.Toggle(Gui.Localize("ModUi/&SelectAll"), ref toggle, UI.Width(ModUi.PixelsPerColumn)))
            {
                SubclassesContext.SelectAllSet(toggle);
            }

            toggle = Main.Settings.DisplayKlassToggle.All(x => x.Value);
            if (UI.Toggle(Gui.Localize("ModUi/&ExpandAll"), ref toggle, UI.Width(ModUi.PixelsPerColumn)))
            {
                var keys = Main.Settings.DisplayKlassToggle.Keys.ToHashSet();

                foreach (var key in keys)
                {
                    Main.Settings.DisplayKlassToggle[key] = toggle;
                }
            }
        }

        foreach (var kvp in SubclassesContext.Klasses)
        {
            var klassDefinition = kvp.Value;
            var subclassListContext = SubclassesContext.SubclassListContextTab[klassDefinition];
            var name = klassDefinition.name;
            var displayToggle = Main.Settings.DisplayKlassToggle[name];
            var sliderPos = Main.Settings.KlassListSliderPosition[name];
            var subclassEnabled = Main.Settings.KlassListSubclassEnabled[name];

            ModUi.DisplayDefinitions(
                kvp.Key.Khaki(),
                subclassListContext.Switch,
                subclassListContext.AllSubClasses,
                subclassEnabled,
                ref displayToggle,
                ref sliderPos);

            Main.Settings.DisplayKlassToggle[name] = displayToggle;
            Main.Settings.KlassListSliderPosition[name] = sliderPos;
        }
    }
}
