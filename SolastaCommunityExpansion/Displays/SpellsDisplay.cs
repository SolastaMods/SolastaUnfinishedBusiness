using System.Collections.Generic;
using System.Linq;
using ModKit;
using SolastaCommunityExpansion.Models;
using static SolastaCommunityExpansion.Displays.Shared;

namespace SolastaCommunityExpansion.Displays
{
    internal static class SpellsDisplay
    {
        private const int SHOW_ALL = -1;

        private static int SpellLevelFilter { get; set; } = SHOW_ALL;

        internal static void DisplaySpells()
        {
            int intValue;
            bool toggle;

            UI.Label("");
            UI.Label($". You can individually assign each spell to any spell list or simply select the suggested set");
            UI.Label($". You won't be able to unselect some spells from spell lists as these are required for the classes to work properly");

            if (!Main.Settings.EnableLevel20)
            {
                UI.Label(". Level 20 feature isn't enabled under Character > General. Spells above level 6 won't be offered in game");
            }

            UI.Label("");

            intValue = SpellLevelFilter;
            if (UI.Slider("spell level filter ".bold().italic().white() + "[-1 to display all spells]".bold().italic().yellow(), ref intValue, SHOW_ALL, 9, SHOW_ALL))
            {
                SpellLevelFilter = intValue;
            }

            UI.Label("");

            using (UI.HorizontalScope())
            {
                toggle = SpellsContext.IsAllSetSelected();
                if (UI.Toggle("Select All", ref toggle, UI.Width(PIXELS_PER_COLUMN)))
                {
                    SpellsContext.SelectAllSet(toggle);
                }

                toggle = SpellsContext.IsSuggestedSetSelected();
                if (UI.Toggle("Select Suggested", ref toggle, UI.Width(PIXELS_PER_COLUMN)))
                {
                    SpellsContext.SelectSuggestedSet(toggle);
                }

                toggle = Main.Settings.DisplaySpellListsToggle.All(x => x.Value);
                if (UI.Toggle("Expand All", ref toggle, UI.Width(PIXELS_PER_COLUMN)))
                {
                    var keys = Main.Settings.DisplaySpellListsToggle.Keys.ToHashSet();

                    foreach (var key in keys)
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
                var allowedSpells = spellListContext.AllSpells
                    .Where(x => SpellLevelFilter == SHOW_ALL || x.SpellLevel == SpellLevelFilter).ToHashSet();

                void AdditionalRendering()
                {
                    toggle = spellListContext.IsAllSetSelected;
                    if (UI.Toggle("Select All", ref toggle, UI.Width(PIXELS_PER_COLUMN)))
                    {
                        spellListContext.SelectAllSetInternal(toggle);
                    }

                    toggle = spellListContext.IsSuggestedSetSelected;
                    if (UI.Toggle("Select Suggested", ref toggle, UI.Width(PIXELS_PER_COLUMN)))
                    {
                        spellListContext.SelectSuggestedSetInternal(toggle);
                    }
                }

                DisplayDefinitions(
                    $"{kvp.Key}:".yellow(),
                    spellListContext.Switch,
                    allowedSpells,
                    spellEnabled,
                    ref displayToggle,
                    ref sliderPos,
                    AdditionalRendering);

                Main.Settings.DisplaySpellListsToggle[name] = displayToggle;
                Main.Settings.SpellListSliderPosition[name] = sliderPos;
            }

            UI.Label("");
        }
    }
}
