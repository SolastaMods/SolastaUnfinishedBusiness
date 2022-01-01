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

        private static readonly SortedDictionary<string, bool> SpellNamesToggle = new SortedDictionary<string, bool>();

        internal static void DisplaySpells()
        {
            bool toggle;

            if (!Initialized)
            {
                SpellsContext.RegisteredSpells.Values.ToList().ForEach(x => SpellNamesToggle.Add(x.SpellDefinition.FormatTitle(), false));

                Initialized = true;
            }

            UI.Label("");

            foreach (var spellRecord in SpellsContext.RegisteredSpells)
            {
                var spellDefinition = spellRecord.Value.SpellDefinition;
                var spellTitle = spellDefinition.FormatTitle();

                toggle = SpellNamesToggle[spellTitle];
                if (UI.DisclosureToggle(spellTitle.yellow(), ref toggle, 200))
                {
                    SpellNamesToggle[spellTitle] = toggle;
                }

                if (SpellNamesToggle[spellTitle])
                {
                    DisplaySpell(spellDefinition);
                }

                UI.Label("");
            }
        }

        internal static void DisplaySpell(SpellDefinition spellDefinition)
        {
            UI.Label("");
            UI.Label(spellDefinition.FormatDescription());

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
            var classes = SpellsContext.GetCasterClasses.ToList();
            var classesCount = classes.Count;

            UI.Label("");
            UI.Label("Classes:".yellow());
            UI.Label("");

            var xx = Main.Settings.ClassSpellEnabled;

            toggle = Main.Settings.ClassSpellEnabled[spellName].Count == classesCount;
            if (UI.Toggle("Select All", ref toggle, UI.AutoWidth()))
            {
                if (toggle)
                {
                    SpellsContext.SelectAllClasses(spellDefinition);
                }
            }

            UI.Label("");

            while (current < classesCount)
            {
                columns = 4;

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
            var subclasses = SpellsContext.GetCasterSubclasses.ToList();
            var subclassesCount = subclasses.Count;

            UI.Label("");
            UI.Label("Subclasses:".yellow());
            UI.Label("");

            toggle = Main.Settings.SubclassSpellEnabled[spellName].Count == subclassesCount;
            if (UI.Toggle("Select All", ref toggle, UI.AutoWidth()))
            {
                if (toggle)
                {
                    SpellsContext.SelectAllSubclasses(spellDefinition);
                }
            }

            UI.Label("");

            while (current < subclassesCount)
            {
                columns = 4;

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
