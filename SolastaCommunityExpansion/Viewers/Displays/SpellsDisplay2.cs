using System.Collections.Generic;
using System.Linq;
using ModKit;
using SolastaCommunityExpansion.Models;
using static SolastaCommunityExpansion.Viewers.Displays.Shared;

namespace SolastaCommunityExpansion.Viewers.Displays
{
    internal static class SpellsDisplay
    {
        private const int SHOW_ALL = -1;

        private static HashSet<SpellDefinition> FilteredSpells { get; set; } = SpellsContext.Spells.ToHashSet();

        private static HashSet<SpellDefinition> FilteredSpellsNoCantrips { get; set; } = FilteredSpells.Where(x => x.SpellLevel > 0).ToHashSet();

        private static int SpellLevelFilter { get; set; } = SHOW_ALL;

        internal static void DisplaySpells()
        {
            int intValue;
            bool toggle;

            UI.Label("");
            UI.Label($". You can individually assign each spell to any spell list or simply select the minimum or suggested sets");

            if (!Main.Settings.EnableLevel20)
            {
                UI.Label(". Level 20 feature isn't enabled under Character > General. Spells above level 6 won't be offered in game");
            }

            UI.Label("");

            using (UI.HorizontalScope())
            {
                toggle = SpellsContext.IsAllSetSelected;

                if (UI.Toggle("Select All", ref toggle, UI.Width(PIXELS_PER_COLUMN)))
                {
                    SpellsContext.SelectAllSet(toggle);
                }

                toggle = SpellsContext.IsMinimumSetSelected;
                if (UI.Toggle("Select Minimum", ref toggle, UI.Width(PIXELS_PER_COLUMN)))
                {
                    if (toggle)
                    {
                        SpellsContext.SelectMinimumSet();
                    }
                    else
                    {
                        SpellsContext.SelectAllSet(false);
                    }
                }

                toggle = SpellsContext.IsSuggestedSetSelected;
                if (UI.Toggle("Select Suggested", ref toggle, UI.Width(PIXELS_PER_COLUMN)))
                {
                    if (toggle)
                    {
                        SpellsContext.SelectSuggestedSet();
                    }
                    else
                    {
                        SpellsContext.SelectAllSet(false);
                    }
                }
            }

            intValue = SpellLevelFilter;
            if (UI.Slider("spell level filter ".bold().italic().white() + "[-1 to display all spells]".bold().italic().yellow(), ref intValue, SHOW_ALL, 9, SHOW_ALL))
            {
                SpellLevelFilter = intValue;

                if (intValue == SHOW_ALL)
                {
                    FilteredSpells = SpellsContext.Spells.ToHashSet();
                    FilteredSpellsNoCantrips = FilteredSpells.Where(x => x.SpellLevel > 0).ToHashSet();
                }
                else
                {
                    FilteredSpells = SpellsContext.Spells.Where(x => x.SpellLevel == SpellLevelFilter).ToHashSet();
                    FilteredSpellsNoCantrips = FilteredSpells.Where(x => x.SpellLevel > 0).ToHashSet();
                }
            }

            UI.Label("");

            UI.Div();

            foreach (var kvp in SpellsContext.SpellLists)
            {
                var spellListDefinition = kvp.Value;
                var name = spellListDefinition.name;
                var displayToggle = Main.Settings.DisplaySpellListsToggle[name];
                var sliderPos = Main.Settings.SpellListSliderPosition[name];
                var spellEnabled = Main.Settings.SpellListSpellEnabled[name];

                void AdditionalRendering()
                {
                    toggle = SpellsContext.SpellListContextTab[spellListDefinition].IsMinimumSetSelected;
                    if (UI.Toggle("Select Minimum", ref toggle, UI.Width(PIXELS_PER_COLUMN)))
                    {
                        SpellsContext.SpellListContextTab[spellListDefinition].SelectMinimumSet();
                    }

                    toggle = SpellsContext.SpellListContextTab[spellListDefinition].IsSuggestedSetSelected;
                    if (UI.Toggle("Select Suggested", ref toggle, UI.Width(PIXELS_PER_COLUMN)))
                    {
                        SpellsContext.SpellListContextTab[spellListDefinition].SelectSuggestedSet();
                    }
                }

                DisplayDefinitions(
                    $"{kvp.Key}:".yellow(),
                    SpellsContext.SpellListContextTab[spellListDefinition].Switch,
                    spellListDefinition.HasCantrips ? FilteredSpells : FilteredSpellsNoCantrips,
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
