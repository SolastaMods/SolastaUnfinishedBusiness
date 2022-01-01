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

        private static readonly Dictionary<string, SpellRecord> RegisteredSpells = new Dictionary<string, SpellRecord>();

        private static IEnumerable<CharacterClassDefinition> casterClasses;

        internal static IEnumerable<CharacterClassDefinition> GetCasterClasses
        {
            get
            {
                if (casterClasses == null)
                {
                    casterClasses = DatabaseRepository.GetDatabase<CharacterClassDefinition>()
                        .Where(x => x.FeatureUnlocks.Exists(y => y.FeatureDefinition is FeatureDefinitionCastSpell));
                }

                return casterClasses;
            }
        }

        private static IEnumerable<CharacterSubclassDefinition> casterSubclasses;

        internal static IEnumerable<CharacterSubclassDefinition> GetCasterSubclasses
        {
            get
            {
                if (casterSubclasses == null)
                {
                    casterSubclasses = DatabaseRepository.GetDatabase<CharacterSubclassDefinition>()
                        .Where(x => x.FeatureUnlocks.Exists(y => y.FeatureDefinition is FeatureDefinitionCastSpell));
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

        internal static void SelectAll()
        {
            Main.Settings.ClassEnabled.Clear();
            Main.Settings.SubclassEnabled.Clear();

            foreach (var registeredSpell in RegisteredSpells)
            {
                Main.Settings.ClassSpellEnabled.Add(registeredSpell.Key, GetCasterClasses.Select(x => x.Name).ToList());
                Main.Settings.SubclassSpellEnabled.Add(registeredSpell.Key, GetCasterSubclasses.Select(x => x.Name).ToList());
            }
        }

        internal static void SelectToSuggested()
        {
            Main.Settings.ClassEnabled.Clear();
            Main.Settings.SubclassEnabled.Clear();

            foreach (var registeredSpell in RegisteredSpells)
            {
                Main.Settings.ClassSpellEnabled.Add(registeredSpell.Key, registeredSpell.Value.SuggestedClasses);
                Main.Settings.SubclassSpellEnabled.Add(registeredSpell.Key, registeredSpell.Value.SuggestedSubclasses);
            }
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
