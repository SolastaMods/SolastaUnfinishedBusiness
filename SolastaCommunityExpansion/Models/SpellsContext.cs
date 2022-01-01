using SolastaCommunityExpansion.Spells;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SolastaCommunityExpansion.Models
{
    internal static class SpellsContext
    {
        internal class SpellRecord
        {
            internal SpellRecord(SpellDefinition spellDefinition, List<string> suggestedClasses, List<string> suggestedSubclasses)
            {
                SpellDefinition = spellDefinition;
                
                if (suggestedClasses != null)
                {
                    SuggestedClasses.AddRange(suggestedClasses);
                }

                if (suggestedSubclasses != null)
                {
                    suggestedSubclasses.AddRange(suggestedSubclasses);
                }
            }

            internal SpellDefinition SpellDefinition { get; private set; }
            internal readonly List<string> SuggestedClasses = new List<string>();
            internal readonly List<string> SuggestedSubclasses = new List<string>();
        }

        internal static readonly Dictionary<string, SpellRecord> RegisteredSpells = new Dictionary<string, SpellRecord>();

        private static List<CharacterClassDefinition> casterClasses;

        internal static List<CharacterClassDefinition> GetCasterClasses
        {
            get
            {
                if (casterClasses == null)
                {
                    casterClasses = DatabaseRepository.GetDatabase<CharacterClassDefinition>()
                        .Where(x => x.FeatureUnlocks.Exists(y => y.FeatureDefinition is FeatureDefinitionCastSpell))
                        .OrderBy(x => x.FormatTitle())
                        .ToList();
                }

                return casterClasses;
            }
        }

        private static List<CharacterSubclassDefinition> casterSubclasses;

        internal static List<CharacterSubclassDefinition> GetCasterSubclasses
        {
            get
            {
                if (casterSubclasses == null)
                {
                    casterSubclasses = DatabaseRepository.GetDatabase<CharacterSubclassDefinition>()
                        .Where(x => x.FeatureUnlocks.Exists(y => y.FeatureDefinition is FeatureDefinitionCastSpell))
                        .OrderBy(x => x.FormatTitle())
                        .ToList();
                }

                return casterSubclasses;
            }
        }

        internal static void Load()
        {
            BazouSpells.Load();

            foreach (var registeredSpell in RegisteredSpells)
            {
                if (!Main.Settings.ClassSpellEnabled.ContainsKey(registeredSpell.Key))
                {
                    Main.Settings.ClassSpellEnabled.Add(registeredSpell.Key, registeredSpell.Value.SuggestedClasses);
                }
                
                if (!Main.Settings.SubclassSpellEnabled.ContainsKey(registeredSpell.Key))
                {
                    Main.Settings.SubclassSpellEnabled.Add(registeredSpell.Key, registeredSpell.Value.SuggestedSubclasses);
                }
            }
        }

        internal static void RegisterSpell(SpellDefinition spellDefinition, List<string> suggestedClasses = null, List<string> suggestedSubclasses = null)
        {
            var spellName = spellDefinition.Name;

            if (!RegisteredSpells.ContainsKey(spellName))
            {
                RegisteredSpells.Add(spellName, new SpellRecord(spellDefinition, suggestedClasses, suggestedSubclasses));
            }
        }

        internal static void SelectAllClasses(SpellDefinition spellDefinition, bool select = true)
        {
            Main.Settings.ClassSpellEnabled[spellDefinition.Name].Clear();

            if (select)
            {
                Main.Settings.ClassSpellEnabled[spellDefinition.Name].AddRange(GetCasterClasses.Select(x => x.Name));
            }
        }

        internal static void SelectAllSubclasses(SpellDefinition spellDefinition, bool select = true)
        {
            Main.Settings.SubclassSpellEnabled[spellDefinition.Name].Clear();

            if (select)
            {
                Main.Settings.SubclassSpellEnabled[spellDefinition.Name].AddRange(GetCasterSubclasses.Select(x => x.Name));
            }
        }

        internal static void SelectSuggestedClasses(SpellDefinition spellDefinition, bool select = true)
        {
            Main.Settings.ClassSpellEnabled[spellDefinition.Name].Clear();

            if (select)
            {
                Main.Settings.ClassSpellEnabled[spellDefinition.Name].AddRange(RegisteredSpells[spellDefinition.Name].SuggestedClasses);
            }
        }

        internal static bool AreSuggestedClassesSelected(SpellDefinition spellDefinition)
        {
            var suggestedClasses = RegisteredSpells[spellDefinition.Name].SuggestedClasses;
            var selectedClasses = Main.Settings.ClassSpellEnabled[spellDefinition.Name];

            if (suggestedClasses.Count != selectedClasses.Count)
            {
                return false;
            }

            return !suggestedClasses.Where(x => !selectedClasses.Contains(x)).Any();
        }

        internal static void SelectSuggestedSubclasses(SpellDefinition spellDefinition, bool select = true)
        {
            Main.Settings.SubclassSpellEnabled[spellDefinition.Name].Clear();

            if (select)
            {
                Main.Settings.SubclassSpellEnabled[spellDefinition.Name].AddRange(RegisteredSpells[spellDefinition.Name].SuggestedSubclasses);
            }
        }

        internal static bool AreSuggestedSubclassesSelected(SpellDefinition spellDefinition)
        {
            var suggestedSubclasses = RegisteredSpells[spellDefinition.Name].SuggestedSubclasses;
            var selectedSubClasses = Main.Settings.SubclassSpellEnabled[spellDefinition.Name];

            if (suggestedSubclasses.Count != selectedSubClasses.Count)
            {
                return false;
            }

            return !suggestedSubclasses.Where(x => !selectedSubClasses.Contains(x)).Any();
        }

        public static string GenerateSpellsDescription()
        {
            var outString = new StringBuilder("[heading]Spells[/heading]");

            outString.Append("\n[list]");

            foreach (var spell in RegisteredSpells.Values)
            {
                outString.Append("\n[*][b]");
                outString.Append(spell.SpellDefinition.FormatTitle());
                outString.Append("[/b]: ");
                outString.Append(spell.SpellDefinition.FormatTitle());
            }

            outString.Append("\n[/list]");

            return outString.ToString();
        }
    }
}
