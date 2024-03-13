using System.Linq;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Api.ModKit;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.Displays;

internal static class SpellsDisplay
{
    private const int ShowAll = -1;

    internal static int SpellLevelFilter { get; private set; } = ShowAll;

    internal static void DisplaySpells()
    {
        UI.Label();
        UI.Label();

        var toggle = Main.Settings.AllowDisplayingOfficialSpells;
        if (UI.Toggle(Gui.Localize("ModUi/&AllowDisplayingOfficialSpells"), ref toggle,
                UI.Width(ModUi.PixelsPerColumn)))
        {
            Main.Settings.AllowDisplayingOfficialSpells = toggle;
            SpellsContext.RecalculateDisplayedSpells();
        }

        toggle = Main.Settings.AllowDisplayingNonSuggestedSpells;
        if (UI.Toggle(Gui.Localize("ModUi/&AllowDisplayingNonSuggestedSpells"), ref toggle,
                UI.Width(ModUi.PixelsPerColumn)))
        {
            Main.Settings.AllowDisplayingNonSuggestedSpells = toggle;
            SpellsContext.RecalculateDisplayedSpells();
        }

        UI.Label();

        using (UI.HorizontalScope())
        {
            UI.ActionButton("UB Spells docs".Bold().Khaki(),
                () => UpdateContext.OpenDocumentation("UnfinishedBusinessSpells.md"), UI.Width(200f));
            20.Space();
            UI.ActionButton("Solasta Spells docs".Bold().Khaki(),
                () => UpdateContext.OpenDocumentation("SolastaSpells.md"), UI.Width(200f));
        }

        UI.Label();

        var intValue = SpellLevelFilter;
        if (UI.Slider(Gui.Localize("ModUi/&SpellLevelFilter"), ref intValue, ShowAll, 9, ShowAll))
        {
            SpellLevelFilter = intValue;
            SpellsContext.RecalculateDisplayedSpells();
        }

        UI.Label();

        using (UI.HorizontalScope())
        {
            toggle = SpellsContext.IsAllSetSelected();
            if (UI.Toggle(Gui.Localize("ModUi/&SelectAll"), ref toggle, UI.Width(ModUi.PixelsPerColumn)))
            {
                SpellsContext.SelectAllSet(toggle);
            }

            toggle = SpellsContext.IsSuggestedSetSelected();
            if (UI.Toggle(Gui.Localize("ModUi/&SelectSuggested"), ref toggle, UI.Width(ModUi.PixelsPerColumn)))
            {
                SpellsContext.SelectSuggestedSet(toggle);
            }

            toggle = SpellsContext.IsTabletopSetSelected();
            if (UI.Toggle(Gui.Localize("ModUi/&SelectTabletop"), ref toggle, UI.Width(ModUi.PixelsPerColumn)))
            {
                SpellsContext.SelectTabletopSet(toggle);
            }

            toggle = Main.Settings.DisplaySpellListsToggle.All(x => x.Value);
            if (UI.Toggle(Gui.Localize("ModUi/&ExpandAll"), ref toggle, UI.Width(ModUi.PixelsPerColumn)))
            {
                foreach (var key in Main.Settings.DisplaySpellListsToggle.Keys.ToHashSet())
                {
                    Main.Settings.DisplaySpellListsToggle[key] = toggle;
                }
            }
        }

        UI.Div();

        foreach (var kvp in SpellsContext.SpellLists)
        {
            var spellListDefinition = kvp.Value;
            var spellListContext = SpellsContext.SpellListContextTab[spellListDefinition];
            var name = spellListDefinition.name;
            var displayToggle = Main.Settings.DisplaySpellListsToggle[name];
            var sliderPos = Main.Settings.SpellListSliderPosition[name];
            var spellEnabled = Main.Settings.SpellListSpellEnabled[name];
            var allowedSpells = spellListContext.DisplayedSpells;

            ModUi.DisplayDefinitions(
                kvp.Key.Khaki(),
                spellListContext.Switch,
                allowedSpells,
                spellEnabled,
                ref displayToggle,
                ref sliderPos,
                additionalRendering: AdditionalRendering);

            Main.Settings.DisplaySpellListsToggle[name] = displayToggle;
            Main.Settings.SpellListSliderPosition[name] = sliderPos;

            continue;

            void AdditionalRendering()
            {
                toggle = spellListContext.IsAllSetSelected;
                if (UI.Toggle(Gui.Localize("ModUi/&SelectAll"), ref toggle, UI.Width(ModUi.PixelsPerColumn)))
                {
                    spellListContext.SelectAllSetInternal(toggle);
                }

                toggle = spellListContext.IsSuggestedSetSelected;
                if (UI.Toggle(Gui.Localize("ModUi/&SelectSuggested"), ref toggle, UI.Width(ModUi.PixelsPerColumn)))
                {
                    spellListContext.SelectSuggestedSetInternal(toggle);
                }

                toggle = spellListContext.IsTabletopSetSelected;
                if (UI.Toggle(Gui.Localize("ModUi/&SelectTabletop"), ref toggle, UI.Width(ModUi.PixelsPerColumn)))
                {
                    spellListContext.SelectTabletopSetInternal(toggle);
                }
            }
        }

        UI.Label();
    }
}
