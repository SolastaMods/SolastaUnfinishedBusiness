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
            internal SpellRecord(SpellDefinition spellDefinition, List<string> suggestedClasses)
            {
                var dbCharacterClassDefinition = DatabaseRepository.GetDatabase<CharacterClassDefinition>();

                SpellDefinition = spellDefinition;
                
                if (suggestedClasses != null)
                {
                    SuggestedClasses.AddRange(suggestedClasses
                        .Where(x => dbCharacterClassDefinition.TryGetElement(x, out var c) && GetCasterClasses.Contains(c)));
                }
            }

            internal SpellDefinition SpellDefinition { get; private set; }
            internal readonly List<string> SuggestedClasses = new List<string>();
        }

        internal static readonly Dictionary<SpellDefinition, List<string>> RegisteredSpells = new Dictionary<SpellDefinition, List<string>>();

        private static readonly List<CharacterClassDefinition> casterClasses = new List<CharacterClassDefinition>();

        internal static List<CharacterClassDefinition> GetCasterClasses
        {
            get
            {
                if (casterClasses.Count == 0)
                {
                    casterClasses.AddRange(DatabaseRepository.GetDatabase<CharacterClassDefinition>()
                        .Where(x => x.FeatureUnlocks.Exists(y => y.FeatureDefinition is FeatureDefinitionCastSpell))
                        .OrderBy(x => x.FormatTitle()));
                }

                return casterClasses;
            }
        }

        internal static void Load()
        {
            BazouSpells.Load();

            foreach (var registeredSpell in RegisteredSpells)
            {
                var spellName = registeredSpell.Key.Name;

                if (!Main.Settings.ClassSpellEnabled.ContainsKey(spellName))
                {
                    Main.Settings.ClassSpellEnabled.Add(spellName, registeredSpell.Value);
                }
            }

            SwitchClass();

            GuiWrapperContext.RecacheSpells();
        }

        private static void SwitchSpell(SpellListDefinition spellListDefinition, SpellDefinition spellDefinition, bool enabled)
        {
            var spellsByLevel = spellListDefinition.SpellsByLevel;

            if (enabled)
            {
                if (!spellListDefinition.ContainsSpell(spellDefinition))
                {
                    if (!spellsByLevel.Any(x => x.Level == spellDefinition.SpellLevel))
                    {
                        spellsByLevel.Add(new SpellListDefinition.SpellsByLevelDuplet
                        {
                            Level = spellDefinition.SpellLevel,
                            Spells = new List<SpellDefinition>()
                        });
                    }

                    spellListDefinition.SpellsByLevel.First(x => x.Level == spellDefinition.SpellLevel).Spells.Add(spellDefinition);
                }
            }
            else
            {
                if (spellListDefinition.ContainsSpell(spellDefinition))
                {
                    spellListDefinition.SpellsByLevel.First(x => x.Level == spellDefinition.SpellLevel).Spells.Remove(spellDefinition);
                }
            }
        }

        internal static void SwitchClass(SpellDefinition spellDefinition = null, CharacterClassDefinition characterClassDefinition = null)
        {
            if (spellDefinition == null)
            {
                RegisteredSpells.Keys.ToList().ForEach(x => SwitchClass(x, null));

                return;
            }

            if (characterClassDefinition == null)
            {
                GetCasterClasses.ForEach(x => SwitchClass(spellDefinition, x));

                return;
            }

            var enabled = Main.Settings.ClassSpellEnabled[spellDefinition.Name].Contains(characterClassDefinition.Name);
            var dbCharacterClassDefinition = DatabaseRepository.GetDatabase<CharacterClassDefinition>();
            var featureDefinitionCastSpell = characterClassDefinition.FeatureUnlocks.FirstOrDefault(x => x.FeatureDefinition is FeatureDefinitionCastSpell).FeatureDefinition as FeatureDefinitionCastSpell;
            var spellListDefinition = featureDefinitionCastSpell.SpellListDefinition;

            SwitchSpell(spellListDefinition, spellDefinition, enabled);
        }

        internal static void RegisterSpell(SpellDefinition spellDefinition, List<string> suggestedClasses = null)
        {
            if (!RegisteredSpells.ContainsKey(spellDefinition))
            {
                RegisteredSpells.Add(spellDefinition, suggestedClasses);
            }
        }

        internal static void SelectAllClasses(bool select = true) => RegisteredSpells.Keys.ToList().ForEach(x => SelectAllClasses(x, select));

        internal static void SelectAllClasses(SpellDefinition spellDefinition, bool select = true)
        {
            Main.Settings.ClassSpellEnabled[spellDefinition.Name].Clear();

            if (select)
            {
                Main.Settings.ClassSpellEnabled[spellDefinition.Name].AddRange(GetCasterClasses.Select(x => x.Name));
            }
        }

        internal static void SelectSuggestedClasses(bool select = true) => RegisteredSpells.Keys.ToList().ForEach(x => SelectSuggestedClasses(x, select));

        internal static void SelectSuggestedClasses(SpellDefinition spellDefinition, bool select = true)
        {
            Main.Settings.ClassSpellEnabled[spellDefinition.Name].Clear();

            if (select)
            {
                Main.Settings.ClassSpellEnabled[spellDefinition.Name].AddRange(RegisteredSpells[spellDefinition]);
            }
        }

        internal static bool AreAllClassesSelected() => !RegisteredSpells.Keys.Any(x => !AreAllClassesSelected(x));

        internal static bool AreAllClassesSelected(SpellDefinition spellDefinition) => Main.Settings.ClassSpellEnabled[spellDefinition.Name].Count == SpellsContext.GetCasterClasses.Count;

        internal static bool AreSuggestedClassesSelected() => !RegisteredSpells.Keys.Any(x => !AreSuggestedClassesSelected(x));

        internal static bool AreSuggestedClassesSelected(SpellDefinition spellDefinition)
        {
            var suggestedClasses = RegisteredSpells[spellDefinition];
            var selectedClasses = Main.Settings.ClassSpellEnabled[spellDefinition.Name];

            if (suggestedClasses.Count != selectedClasses.Count || suggestedClasses.Count == 0 || selectedClasses.Count == 0)
            {
                return false;
            }

            return !suggestedClasses.Where(x => !selectedClasses.Contains(x)).Any();
        }

        public static string GenerateSpellsDescription()
        {
            var outString = new StringBuilder("[heading]Spells[/heading]");

            outString.Append("\n[list]");

            foreach (var spell in RegisteredSpells.Keys)
            {
                outString.Append("\n[*][b]");
                outString.Append(spell.FormatTitle());
                outString.Append("[/b]: ");
                outString.Append(spell.FormatTitle());
            }

            outString.Append("\n[/list]");

            return outString.ToString();
        }
    }
}
