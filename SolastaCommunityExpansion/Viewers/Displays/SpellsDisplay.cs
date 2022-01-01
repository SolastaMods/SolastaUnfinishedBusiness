using ModKit;
using SolastaCommunityExpansion.Models;
using System.Collections.Generic;
using System.Linq;

namespace SolastaCommunityExpansion.Viewers.Displays
{
    internal static class SpellsDisplay
    {
        private const int MAX_COLUMNS = 4;

        private const float PIXELS_PER_COLUMN = 225;

        private static bool Initialized { get; set; }

        private static readonly List<SpellDefinition> SortedRegisteredSpells = new List<SpellDefinition>();

        private static bool ExpandAllToggle { get; set; }

        private static readonly Dictionary<string, bool> SpellNamesToggle = new Dictionary<string, bool>();

        internal static void DisplaySpells()
        {
            bool toggle;

            if (!Initialized)
            {
                SpellsContext.RegisteredSpells.Keys
                    .Select(x => x.Name)
                    .ToList()
                    .ForEach(x => SpellNamesToggle.Add(x, false));

                SortedRegisteredSpells.AddRange(SpellsContext.RegisteredSpells
                    .Select(x => x.Key)
                    .OrderBy(x => $"{x.SpellLevel} - {x.FormatTitle()}"));

                Initialized = true;
            }

            UI.Label("");

            using (UI.HorizontalScope())
            {
                var expanded = SpellNamesToggle.Select(x => x.Value).Count();
                var total = SpellNamesToggle.Count;

                UI.Space(20);

                toggle = SpellsContext.AreAllClassesSelected();

                if (UI.Toggle("Select All", ref toggle, UI.Width(PIXELS_PER_COLUMN)))
                {
                    SpellsContext.SelectAllClasses(toggle);
                    SpellsContext.SwitchClass();
                }

                toggle = SpellsContext.AreSuggestedClassesSelected();
                if (UI.Toggle("Select Suggested", ref toggle, UI.Width(PIXELS_PER_COLUMN)))
                {
                    SpellsContext.SelectSuggestedClasses(toggle);
                    SpellsContext.SwitchClass();
                }

                ExpandAllToggle = SpellNamesToggle.Count == SpellNamesToggle.Count(x => x.Value);
                toggle = ExpandAllToggle;
                if (UI.Toggle("Expand All", ref toggle, UI.Width(PIXELS_PER_COLUMN)))
                {
                    ExpandAllToggle = toggle;
                    SpellNamesToggle.Keys.ToList().ForEach(x => SpellNamesToggle[x] = toggle);
                }
            }

            UI.Label("");

            foreach (var spellDefinition in SortedRegisteredSpells)
            {
                var spellName = spellDefinition.Name;
                var spellTitle = $"{spellDefinition.SpellLevel} - {spellDefinition.FormatTitle()}";

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
            UI.Label($"{spellDefinition.FormatDescription()}");

            using (UI.HorizontalScope())
            {
                UI.Space(20);

                using (UI.VerticalScope())
                {
                    DisplaySpellClassSelection(spellDefinition);
                }
            }
        }

        internal static void DisplaySpellClassSelection(SpellDefinition spellDefinition)
        {
            var spellName = spellDefinition.Name;
            bool toggle;
            int columns;
            var current = 0;
            var classes = SpellsContext.GetCasterClasses;
            var classesCount = classes.Count;

            UI.Label("");

            using (UI.HorizontalScope())
            {
                toggle = SpellsContext.AreAllClassesSelected(spellDefinition);
                if (UI.Toggle("Select All", ref toggle, UI.Width(PIXELS_PER_COLUMN)))
                {
                    SpellsContext.SelectAllClasses(spellDefinition, toggle);
                    SpellsContext.SwitchClass(spellDefinition);
                }

                toggle = SpellsContext.AreSuggestedClassesSelected(spellDefinition);
                if (UI.Toggle("Select Suggested", ref toggle, UI.Width(PIXELS_PER_COLUMN)))
                {
                    SpellsContext.SelectSuggestedClasses(spellDefinition, toggle);
                    SpellsContext.SwitchClass(spellDefinition);
                }
            }

            UI.Label("");

            while (current < classesCount)
            {
                columns = MAX_COLUMNS;

                using (UI.HorizontalScope())
                {
                    while (current < classesCount && columns-- > 0)
                    {
                        var classDefinition = classes.ElementAt(current);
                        var className = classDefinition.Name;
                        var classTitle = classDefinition.FormatTitle();

                        toggle = Main.Settings.ClassSpellEnabled[spellName].Contains(className);
                        if (UI.Toggle(classTitle, ref toggle, UI.Width(PIXELS_PER_COLUMN)))
                        {
                            if (toggle)
                            {
                                Main.Settings.ClassSpellEnabled[spellName].Add(className);
                            }
                            else
                            {
                                Main.Settings.ClassSpellEnabled[spellName].Remove(className);
                            }

                            SpellsContext.SwitchClass(spellDefinition, classDefinition);
                        }

                        current++;
                    }
                }
            }
        }
    }
}
