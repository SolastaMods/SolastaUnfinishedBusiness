using SolastaCommunityExpansion.Spells;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SolastaCommunityExpansion.Models
{
    internal static class SpellsContext
    {
        internal static readonly Dictionary<SpellDefinition, List<string>> RegisteredSpells = new Dictionary<SpellDefinition, List<string>>();

        private static readonly SortedDictionary<string, SpellListDefinition> spellLists = new SortedDictionary<string, SpellListDefinition>();

        internal static SortedDictionary<string, SpellListDefinition> GetSpellLists
        {
            get
            {
                if (spellLists.Count == 0)
                {
                    var dbCharacterClassDefinition = DatabaseRepository.GetDatabase<CharacterClassDefinition>();
                    var dbCharacterSubclassDefinition = DatabaseRepository.GetDatabase<CharacterSubclassDefinition>();

                    foreach (var characterClass in dbCharacterClassDefinition)
                    {
                        var title = characterClass.FormatTitle();
                        var characterClassCastSpell = characterClass.FeatureUnlocks
                            .Select(x => x.FeatureDefinition)
                            .Where(x => x is FeatureDefinitionCastSpell)
                            .FirstOrDefault();

                        if (characterClassCastSpell is FeatureDefinitionCastSpell featureDefinitionCastSpell)
                        {
                            spellLists.Add(title, featureDefinitionCastSpell.SpellListDefinition);
                        }
                    }

                    foreach (var characterSubclass in dbCharacterSubclassDefinition)
                    {
                        var title = characterSubclass.FormatTitle();
                        var characterSubclassCastSpell = characterSubclass.FeatureUnlocks
                            .Select(x => x.FeatureDefinition)
                            .Where(x => x is FeatureDefinitionMagicAffinity)
                            .FirstOrDefault();

                        if (characterSubclassCastSpell is FeatureDefinitionMagicAffinity featureDefinitionMagicAffinity && featureDefinitionMagicAffinity.ExtendedSpellList != null)
                        {
                            spellLists.Add(title, featureDefinitionMagicAffinity.ExtendedSpellList);
                        }
                    }
                }

                return spellLists;
            }
        }

        internal static void Load()
        {
            BazouSpells.Load();

            foreach (var registeredSpell in RegisteredSpells)
            {
                var spellName = registeredSpell.Key.Name;

                if (!Main.Settings.SpellSpellListEnabled.ContainsKey(spellName))
                {
                    Main.Settings.SpellSpellListEnabled.Add(spellName, registeredSpell.Value);
                }
            }

            SwitchSpellList();

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

        internal static void SwitchSpellList(SpellDefinition spellDefinition = null, SpellListDefinition spellListDefinition = null)
        {
            if (spellDefinition == null)
            {
                RegisteredSpells.Keys.ToList().ForEach(x => SwitchSpellList(x, null));

                return;
            }

            if (spellListDefinition == null)
            {
                GetSpellLists.Values.ToList().ForEach(x => SwitchSpellList(spellDefinition, x));

                return;
            }

            var enabled = Main.Settings.SpellSpellListEnabled[spellDefinition.Name].Contains(spellListDefinition.Name);

            SwitchSpell(spellListDefinition, spellDefinition, enabled);
        }

        internal static void RegisterSpell(SpellDefinition spellDefinition, params string[] suggestedSpellLists)
        {
            if (!RegisteredSpells.ContainsKey(spellDefinition))
            {
                RegisteredSpells.Add(spellDefinition, suggestedSpellLists.ToList());
            }
        }

        internal static void SelectAllSpellLists(bool select = true) => RegisteredSpells.Keys.ToList().ForEach(x => SelectAllSpellLists(x, select));

        internal static void SelectAllSpellLists(SpellDefinition spellDefinition, bool select = true)
        {
            Main.Settings.SpellSpellListEnabled[spellDefinition.Name].Clear();

            if (select)
            {
                Main.Settings.SpellSpellListEnabled[spellDefinition.Name].AddRange(GetSpellLists.Values.Select(x => x.Name));
            }
        }

        internal static void SelectSuggestedSpellLists(bool select = true) => RegisteredSpells.Keys.ToList().ForEach(x => SelectSuggestedSpellLists(x, select));

        internal static void SelectSuggestedSpellLists(SpellDefinition spellDefinition, bool select = true)
        {
            Main.Settings.SpellSpellListEnabled[spellDefinition.Name].Clear();

            if (select)
            {
                Main.Settings.SpellSpellListEnabled[spellDefinition.Name].AddRange(RegisteredSpells[spellDefinition]);
            }
        }

        internal static bool AreAllSpellListsSelected() => !RegisteredSpells.Keys.Any(x => !AreAllSpellListsSelected(x));

        internal static bool AreAllSpellListsSelected(SpellDefinition spellDefinition) => Main.Settings.SpellSpellListEnabled[spellDefinition.Name].Count == SpellsContext.GetSpellLists.Count;

        internal static bool AreSuggestedSpellListsSelected() => !RegisteredSpells.Keys.Any(x => !AreSuggestedSpellListsSelected(x));

        internal static bool AreSuggestedSpellListsSelected(SpellDefinition spellDefinition)
        {
            var suggestedClasses = RegisteredSpells[spellDefinition];
            var selectedClasses = Main.Settings.SpellSpellListEnabled[spellDefinition.Name];

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
