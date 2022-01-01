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
                SpellsContext.RegisteredSpells.Values
                    .Select(x => x.SpellDefinition.Name)
                    .ToList()
                    .ForEach(x => SpellNamesToggle.Add(x, false));

                SortedRegisteredSpells.AddRange(SpellsContext.RegisteredSpells
                    .Select(x => x.Value.SpellDefinition)
                    .OrderBy(x => $"{x.SpellLevel} - {x.FormatTitle()}"));

                Initialized = true;
            }

            UI.Label("");

            using (UI.HorizontalScope())
            {
                var expanded = SpellNamesToggle.Select(x => x.Value).Count();
                var total = SpellNamesToggle.Count;

                UI.Space(20);

                toggle = Main.Settings.ClassSpellEnabled.Sum(x => x.Value.Count) == SpellsContext.GetCasterClasses.Count * SpellsContext.RegisteredSpells.Count
                    && Main.Settings.SubclassSpellEnabled.Sum(x => x.Value.Count) == SpellsContext.GetCasterSubclasses.Count * SpellsContext.RegisteredSpells.Count;

                if (UI.Toggle("Select All", ref toggle, UI.Width(PIXELS_PER_COLUMN)))
                {
                    SpellsContext.SelectAllClasses(toggle);
                    SpellsContext.SelectAllSubclasses(toggle);
                }

                toggle = SpellsContext.AreSuggestedClassesSelected() && SpellsContext.AreSuggestedSubclassesSelected();
                if (UI.Toggle("Select Suggested", ref toggle, UI.Width(PIXELS_PER_COLUMN)))
                {
                    SpellsContext.SelectSuggestedClasses(toggle);
                    SpellsContext.SelectSuggestedSubclasses(toggle);
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

            using (UI.HorizontalScope())
            {
                UI.Space(20);

                using (UI.VerticalScope())
                {
                    DisplaySpellSubclassSelection(spellDefinition);
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
            UI.Label("Classes:".yellow());
            UI.Label("");

            using (UI.HorizontalScope())
            {
                toggle = Main.Settings.ClassSpellEnabled[spellName].Count == classesCount;
                if (UI.Toggle("Select All", ref toggle, UI.Width(PIXELS_PER_COLUMN)))
                {
                    SpellsContext.SelectAllClasses(spellDefinition, toggle);
                }

                toggle = SpellsContext.AreSuggestedClassesSelected(spellDefinition);
                if (UI.Toggle("Select Suggested", ref toggle, UI.Width(PIXELS_PER_COLUMN)))
                {
                    SpellsContext.SelectSuggestedClasses(spellDefinition, toggle);
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
                        }

                        current++;
                    }
                }
            }
        }

        internal static void DisplaySpellSubclassSelection(SpellDefinition spellDefinition)
        {
            var spellName = spellDefinition.Name;
            bool toggle;
            int columns;
            var current = 0;
            var subclasses = SpellsContext.GetCasterSubclasses;
            var subclassesCount = subclasses.Count;

            UI.Label("");
            UI.Label("Subclasses:".yellow());
            UI.Label("");

            using (UI.HorizontalScope())
            {
                toggle = Main.Settings.SubclassSpellEnabled[spellName].Count == subclassesCount;
                if (UI.Toggle("Select All", ref toggle, UI.Width(PIXELS_PER_COLUMN)))
                {
                    SpellsContext.SelectAllSubclasses(spellDefinition, toggle);
                }

                toggle = SpellsContext.AreSuggestedSubclassesSelected(spellDefinition);
                if (UI.Toggle("Select Suggested", ref toggle, UI.Width(PIXELS_PER_COLUMN)))
                {
                    SpellsContext.SelectSuggestedSubclasses(spellDefinition, toggle);
                }
            }

            UI.Label("");

            while (current < subclassesCount)
            {
                columns = MAX_COLUMNS;

                using (UI.HorizontalScope())
                {
                    while (current < subclassesCount && columns-- > 0)
                    {
                        var subclassDefinition = subclasses.ElementAt(current);
                        var subclassName = subclassDefinition.Name;
                        var subclassTitle = subclassDefinition.FormatTitle();

                        toggle = Main.Settings.SubclassSpellEnabled[spellName].Contains(subclassName);
                        if (UI.Toggle(subclassTitle, ref toggle, UI.Width(PIXELS_PER_COLUMN)))
                        {
                            if (toggle)
                            {
                                Main.Settings.SubclassSpellEnabled[spellName].Add(subclassName);
                            }
                            else
                            {
                                Main.Settings.SubclassSpellEnabled[spellName].Remove(subclassName);
                            }
                        }

                        current++;
                    }
                }
            }
        }
    }
}
