using ModKit;
using SolastaCommunityExpansion.Models;
using SolastaModApi.Infrastructure;
using System.Collections.Generic;
using System.Linq;

namespace SolastaCommunityExpansion.Viewers.Displays
{
    internal static class SpellsDisplay
    {
        private const int SHOW_ALL = -1;

        private const int MAX_COLUMNS = 4;

        private const float PIXELS_PER_COLUMN = 225;

        private static int SpellLevelFilter { get; set; } = SHOW_ALL;

        private static bool Initialized { get; set; }

        private static readonly List<SpellDefinition> SortedRegisteredSpells = new List<SpellDefinition>();

        private static readonly List<bool> IsFromOtherModList = new List<bool>();

        private static bool ExpandAllToggle { get; set; }

        private static readonly Dictionary<string, bool> SpellNamesToggle = new Dictionary<string, bool>();

        private static string WarningMessage => Main.Settings.AllowDisplayAllUnofficialContent ? ". Spells in " + "orange".orange() + " were not created by this mod" : string.Empty;

        private static void RecacheSortedRegisteredSpells()
        {
            SortedRegisteredSpells.SetRange(SpellsContext.RegisteredSpells
                .Select(x => x.Key)
                .Where(x => SpellLevelFilter == SHOW_ALL || x.SpellLevel == SpellLevelFilter)
                .OrderBy(x => $"{x.SpellLevel} - {x.FormatTitle()}"));

            IsFromOtherModList.SetRange(SpellsContext.RegisteredSpells
                .Where(x => SpellLevelFilter == SHOW_ALL || x.Key.SpellLevel == SpellLevelFilter)
                .OrderBy(x => $"{x.Key.SpellLevel} - {x.Key.FormatTitle()}")
                .Select(x => x.Value.IsFromOtherMod));
        }

        internal static void DisplaySpells()
        {
            int intValue;
            bool toggle;

            if (!Initialized)
            {
                SpellsContext.RegisteredSpells.Keys
                    .Select(x => x.Name)
                    .ToList()
                    .ForEach(x => SpellNamesToggle.Add(x, false));

                RecacheSortedRegisteredSpells();

                Initialized = true;
            }

            UI.Label("");
            UI.Label($". You can individually assign each spell to any caster spell list or simply select the suggested set{WarningMessage}");
            UI.Label("");

            using (UI.HorizontalScope())
            {
                UI.Space(20);

                toggle = SpellsContext.AreAllSpellListsSelected();

                if (UI.Toggle("Select All", ref toggle, UI.Width(PIXELS_PER_COLUMN)))
                {
                    SpellsContext.SwitchAllSpellLists(toggle);
                }

                toggle = SpellsContext.AreSuggestedSpellListsSelected();
                if (UI.Toggle("Select Suggested", ref toggle, UI.Width(PIXELS_PER_COLUMN)))
                {
                    SpellsContext.SwitchSuggestedSpellLists(toggle);
                }
            }

            UI.Label("");

            using (UI.HorizontalScope())
            {
                UI.Space(20);

                intValue = SpellLevelFilter;
                if (UI.Slider("Spell level filter ".white() + "[-1 to display all spells]".italic().yellow(), ref intValue, SHOW_ALL, 9, SHOW_ALL, "L", UI.AutoWidth()))
                {
                    SpellLevelFilter = intValue;
                    RecacheSortedRegisteredSpells();
                }
            }

            UI.Label("");

            if (SortedRegisteredSpells.Count > 0)
            {
                using (UI.HorizontalScope())
                {
                    UI.Space(20);

                    ExpandAllToggle = SpellNamesToggle.Count == SpellNamesToggle.Count(x => x.Value);
                    toggle = ExpandAllToggle;
                    if (UI.Toggle("Expand All", ref toggle, UI.Width(PIXELS_PER_COLUMN)))
                    {
                        ExpandAllToggle = toggle;
                        SpellNamesToggle.Keys.ToList().ForEach(x => SpellNamesToggle[x] = toggle);
                    }
                }
            }
            else
            {
                UI.Label($". No unofficial level {SpellLevelFilter} spells available".red().bold());
            }

            UI.Label("");

            for (var i = 0; i < SortedRegisteredSpells.Count; i++) 
            {
                var spellDefinition = SortedRegisteredSpells[i];
                var spellName = spellDefinition.Name;
                var spellTitle = $"{spellDefinition.SpellLevel} - {spellDefinition.FormatTitle()}";

                if (IsFromOtherModList.ElementAt(i))
                {
                    spellTitle = spellTitle.orange();
                }

                toggle = SpellNamesToggle[spellName];
                if (UI.DisclosureToggle(spellTitle.yellow(), ref toggle, 200))
                {
                    SpellNamesToggle[spellName] = toggle;
                }

                if (SpellNamesToggle[spellName])
                {
                    DisplaySpell(spellDefinition);
                }

                UI.Label("");
            }
        }

        internal static void DisplaySpell(SpellDefinition spellDefinition)
        {
            UI.Label("");
            UI.Label($". {spellDefinition.FormatDescription()}");

            using (UI.HorizontalScope())
            {
                UI.Space(20);

                using (UI.VerticalScope())
                {
                    DisplaySpellListSelection(spellDefinition);
                }
            }
        }

        internal static void DisplaySpellListSelection(SpellDefinition spellDefinition)
        {
            var spellName = spellDefinition.Name;
            bool toggle;
            int columns;
            var current = 0;
            var spellListsTitles = SpellsContext.SpellLists.Keys;
            var spellLists = SpellsContext.SpellLists.Values;
            var spellListsCount = spellLists.Count;

            UI.Label("");

            using (UI.HorizontalScope())
            {
                toggle = SpellsContext.AreAllSpellListsSelected(spellDefinition);
                if (UI.Toggle("Select All", ref toggle, UI.Width(PIXELS_PER_COLUMN)))
                {
                    SpellsContext.SwitchAllSpellLists(toggle, spellDefinition);
                }

                toggle = SpellsContext.AreSuggestedSpellListsSelected(spellDefinition);
                if (UI.Toggle("Select Suggested", ref toggle, UI.Width(PIXELS_PER_COLUMN)))
                {
                    SpellsContext.SwitchSuggestedSpellLists(toggle, spellDefinition);
                }
            }

            UI.Label("");

            while (current < spellListsCount)
            {
                columns = MAX_COLUMNS;

                using (UI.HorizontalScope())
                {
                    while (current < spellListsCount && columns-- > 0)
                    {
                        var spellListDefinition = spellLists.ElementAt(current);
                        var spellListName = spellListDefinition.Name;
                        var spellListTitle = spellListsTitles.ElementAt(current);

                        toggle = Main.Settings.SpellSpellListEnabled[spellName].Contains(spellListName);
                        if (UI.Toggle(spellListTitle, ref toggle, UI.Width(PIXELS_PER_COLUMN)))
                        {
                            if (toggle)
                            {
                                Main.Settings.SpellSpellListEnabled[spellName].Add(spellListName);
                            }
                            else
                            {
                                Main.Settings.SpellSpellListEnabled[spellName].Remove(spellListName);
                            }

                            SpellsContext.SwitchSpellList(spellDefinition, spellListDefinition);
                        }

                        current++;
                    }
                }
            }
        }
    }
}
