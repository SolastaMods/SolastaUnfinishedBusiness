using System.Linq;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Api.ModKit;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.Displays;

internal static class SubclassesDisplay
{
    internal static void DisplaySubclasses()
    {
        UI.Label();

        using (UI.HorizontalScope())
        {
            UI.ActionButton("UB Classes Docs".Bold().Khaki(),
                () => UpdateContext.OpenDocumentation("UnfinishedBusinessClasses.md"), UI.Width(200f));
            20.Space();
            UI.ActionButton("Solasta Classes Docs".Bold().Khaki(),
                () => UpdateContext.OpenDocumentation("SolastaClasses.md"), UI.Width(200f));
        }

        using (UI.HorizontalScope())
        {
            UI.ActionButton("UB Subclasses Docs".Bold().Khaki(),
                () => UpdateContext.OpenDocumentation("UnfinishedBusinessSubclasses.md"), UI.Width(200f));
            20.Space();
            UI.ActionButton("Solasta Subclasses Docs".Bold().Khaki(),
                () => UpdateContext.OpenDocumentation("SolastaSubclasses.md"), UI.Width(200f));
        }

        UI.Label();

        using (UI.HorizontalScope())
        {
            var toggle = Main.Settings.DisplayKlassToggle.All(x => x.Value);
            if (UI.Toggle(Gui.Localize("ModUi/&ExpandAll"), ref toggle, UI.Width(ModUi.PixelsPerColumn)))
            {
                foreach (var key in Main.Settings.DisplayKlassToggle.Keys.ToHashSet())
                {
                    Main.Settings.DisplayKlassToggle[key] = toggle;
                }
            }

            toggle = SubclassesContext.IsAllSetSelected();
            if (UI.Toggle(Gui.Localize("ModUi/&SelectAll"), ref toggle, UI.Width(ModUi.PixelsPerColumn)))
            {
                SubclassesContext.SelectAllSet(toggle);
            }

            toggle = SubclassesContext.IsTabletopSetSelected();
            if (UI.Toggle(Gui.Localize("ModUi/&SelectTabletop"), ref toggle, UI.Width(ModUi.PixelsPerColumn)))
            {
                SubclassesContext.SelectTabletopSet(toggle);
            }
        }

        UI.Div();

        foreach (var kvp in SubclassesContext.Klasses)
        {
            var displayName = kvp.Key;
            var klassName = kvp.Value.Item1;
            var klassDefinition = kvp.Value.Item2;
            var subclassListContext = SubclassesContext.KlassListContextTab[klassDefinition];
            var displayToggle = Main.Settings.DisplayKlassToggle[klassName];
            var sliderPos = Main.Settings.KlassListSliderPosition[klassName];
            var subclassEnabled = Main.Settings.KlassListSubclassEnabled[klassName];

            ModUi.DisplayDefinitions(
                displayName.Khaki(),
                subclassListContext.Switch,
                subclassListContext.AllSubClasses,
                subclassEnabled,
                ref displayToggle,
                ref sliderPos);

            Main.Settings.DisplayKlassToggle[klassName] = displayToggle;
            Main.Settings.KlassListSliderPosition[klassName] = sliderPos;
        }
    }
}
