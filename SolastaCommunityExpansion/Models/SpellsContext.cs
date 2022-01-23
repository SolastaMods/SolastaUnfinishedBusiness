using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using ModKit;
using SolastaCommunityExpansion.Spells;

namespace SolastaCommunityExpansion.Models
{
    internal static class SpellsContext
    {
        internal const string NOT_IN_MIN_SET = null;

        internal class SpellRecord
        {
            public List<string> MinimumSpellLists { get; set; }

            public List<string> SuggestedSpellLists { get; set; }

            public bool IsFromOtherMod { get; set; }
        }

        internal static readonly Dictionary<SpellDefinition, SpellRecord> RegisteredSpells = new Dictionary<SpellDefinition, SpellRecord>();

        private static readonly List<SpellDefinition> RegisteredSpellsList = new List<SpellDefinition>();

        private static readonly SortedDictionary<string, SpellListDefinition> spellLists = new SortedDictionary<string, SpellListDefinition>();

        internal static SortedDictionary<string, SpellListDefinition> SpellLists
        {
            get
            {
                if (spellLists.Count != 0)
                {
                    return spellLists;
                }

                var dbCharacterClassDefinition = DatabaseRepository.GetDatabase<CharacterClassDefinition>();
                var dbCharacterSubclassDefinition = DatabaseRepository.GetDatabase<CharacterSubclassDefinition>();

                foreach (var characterClass in dbCharacterClassDefinition)
                {
                    var title = characterClass.FormatTitle();

                    var featureDefinitionCastSpell = characterClass.FeatureUnlocks
                        .Select(x => x.FeatureDefinition)
                        .OfType<FeatureDefinitionCastSpell>()
                        .FirstOrDefault();

                    if (featureDefinitionCastSpell?.SpellListDefinition != null
                        && !spellLists.ContainsValue(featureDefinitionCastSpell.SpellListDefinition))
                    {
                        spellLists.Add(title, featureDefinitionCastSpell.SpellListDefinition);
                    }
                }

                foreach (var characterSubclass in dbCharacterSubclassDefinition)
                {
                    var title = characterSubclass.FormatTitle();

                    var featureDefinition = characterSubclass.FeatureUnlocks
                        .Select(x => x.FeatureDefinition)
                        .FirstOrDefault(x => x is FeatureDefinitionCastSpell || x is FeatureDefinitionMagicAffinity);

                    if (featureDefinition is FeatureDefinitionMagicAffinity featureDefinitionMagicAffinity
                        && featureDefinitionMagicAffinity.ExtendedSpellList != null
                        && !spellLists.ContainsValue(featureDefinitionMagicAffinity.ExtendedSpellList))
                    {
                        spellLists.Add(title.grey().italic(), featureDefinitionMagicAffinity.ExtendedSpellList);
                    }
                    else if (featureDefinition is FeatureDefinitionCastSpell featureDefinitionCastSpell
                        && featureDefinitionCastSpell.SpellListDefinition != null
                        && !spellLists.ContainsValue(featureDefinitionCastSpell.SpellListDefinition))
                    {
                        spellLists.Add(title.grey().italic(), featureDefinitionCastSpell.SpellListDefinition);
                    }
                }

                return spellLists;
            }
        }

        private static List<SpellDefinition> GetAllUnofficialSpells()
        {
            var officialSpellNames = typeof(SolastaModApi.DatabaseHelper.SpellDefinitions)
                .GetProperties(BindingFlags.Public | BindingFlags.Static)
                .Where(f => f.PropertyType == typeof(SpellDefinition))
                .Select(f => f.Name).ToHashSet();

            return DatabaseRepository.GetDatabase<SpellDefinition>()
                .Where(f => !officialSpellNames.Contains(f.Name)).ToList();
        }

        private static void LoadAllUnofficialSpells()
        {
            var unofficialSpells = GetAllUnofficialSpells();

            foreach (var spellList in SpellLists.Values)
            {
                foreach (var unofficialSpell in unofficialSpells.Where(x => spellList.ContainsSpell(x)))
                {
                    RegisterSpell(unofficialSpell, isFromOtherMod: true, spellList.Name);
                }
            }
        }

        internal static void AddToDB()
        {
            BazouSpells.AddToDB();
            SrdSpells.AddToDB();
        }

        internal static void Load()
        {
            BazouSpells.Register();
            SrdSpells.Register();
            HouseSpellTweaks.Register();

            if (Main.Settings.AllowDisplayAllUnofficialContent)
            {
                LoadAllUnofficialSpells();
            }

            foreach (var registeredSpell in RegisteredSpells.Where(x => !Main.Settings.SpellSpellListEnabled.ContainsKey(x.Key.Name)))
            {
                Main.Settings.SpellSpellListEnabled.Add(registeredSpell.Key.Name, registeredSpell.Value.MinimumSpellLists.ToList());
            }

            SwitchSpellList();
        }

        private static void SwitchSpell(SpellListDefinition spellListDefinition, SpellDefinition spellDefinition, bool enabled)
        {
            var spellsByLevel = spellListDefinition.SpellsByLevel;

            if (enabled && !spellListDefinition.ContainsSpell(spellDefinition))
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
            else if (!enabled && spellListDefinition.ContainsSpell(spellDefinition))
            {
                spellListDefinition.SpellsByLevel.First(x => x.Level == spellDefinition.SpellLevel).Spells.Remove(spellDefinition);
            }
        }

        internal static void SwitchSpellList(SpellDefinition spellDefinition = null, SpellListDefinition spellListDefinition = null)
        {
            if (spellDefinition == null)
            {
                RegisteredSpellsList.ForEach(x => SwitchSpellList(x, null));

                return;
            }

            if (spellListDefinition == null)
            {
                SpellLists.Values.ToList().ForEach(x => SwitchSpellList(spellDefinition, x));

                return;
            }

            var enabled = Main.Settings.SpellSpellListEnabled[spellDefinition.Name].Contains(spellListDefinition.Name);

            SwitchSpell(spellListDefinition, spellDefinition, enabled);
        }

        internal static void RegisterSpell(SpellDefinition spellDefinition, bool isFromOtherMod, string minimumSpellList, params string[] suggestedSpellLists)
        {
            var dbSpellListDefinition = DatabaseRepository.GetDatabase<SpellListDefinition>();

            var cleansedMinimumSpellLists = new List<string> { minimumSpellList }.Where(x => dbSpellListDefinition.TryGetElement(x, out _)).ToList();
            var cleansedSuggestedSpellLists = suggestedSpellLists.Where(x => dbSpellListDefinition.TryGetElement(x, out _)).ToList();

            cleansedMinimumSpellLists.ForEach(x => cleansedSuggestedSpellLists.TryAdd(x));

            if (!RegisteredSpells.ContainsKey(spellDefinition))
            {
                RegisteredSpellsList.Add(spellDefinition);
                RegisteredSpells.Add(spellDefinition, new SpellRecord
                {
                    IsFromOtherMod = isFromOtherMod,
                    MinimumSpellLists = cleansedMinimumSpellLists,
                    SuggestedSpellLists = cleansedSuggestedSpellLists
                });
            }
            else
            {
                cleansedMinimumSpellLists.ForEach(x => RegisteredSpells[spellDefinition].MinimumSpellLists.TryAdd(x));
                cleansedSuggestedSpellLists.ForEach(x => RegisteredSpells[spellDefinition].SuggestedSpellLists.TryAdd(x));
            }
        }

        internal static void SwitchAllSpellLists(bool select = true, SpellDefinition spellDefinition = null)
        {
            if (spellDefinition == null)
            {
                RegisteredSpellsList.ForEach(x => SwitchAllSpellLists(select, x));

                return;
            }

            Main.Settings.SpellSpellListEnabled[spellDefinition.Name].Clear();

            if (select)
            {
                Main.Settings.SpellSpellListEnabled[spellDefinition.Name].AddRange(SpellLists.Values.Select(x => x.Name));
            }

            SwitchSpellList(spellDefinition);
        }

        internal static void SwitchMinimumSpellLists(bool select = true, SpellDefinition spellDefinition = null)
        {
            if (spellDefinition == null)
            {
                RegisteredSpellsList.ForEach(x => SwitchMinimumSpellLists(select, x));

                return;
            }

            Main.Settings.SpellSpellListEnabled[spellDefinition.Name].Clear();

            if (select)
            {
                Main.Settings.SpellSpellListEnabled[spellDefinition.Name].AddRange(RegisteredSpells[spellDefinition].MinimumSpellLists);
            }

            SwitchSpellList(spellDefinition);
        }

        internal static void SwitchSuggestedSpellLists(bool select = true, SpellDefinition spellDefinition = null)
        {
            if (spellDefinition == null)
            {
                RegisteredSpellsList.ForEach(x => SwitchSuggestedSpellLists(select, x));

                return;
            }

            Main.Settings.SpellSpellListEnabled[spellDefinition.Name].Clear();

            if (select)
            {
                Main.Settings.SpellSpellListEnabled[spellDefinition.Name].AddRange(RegisteredSpells[spellDefinition].SuggestedSpellLists);
            }

            SwitchSpellList(spellDefinition);
        }

        internal static bool AreAllSpellListsSelected()
        {
            return RegisteredSpellsList.All(x => AreAllSpellListsSelected(x));
        }

        internal static bool AreAllSpellListsSelected(SpellDefinition spellDefinition)
        {
            return Main.Settings.SpellSpellListEnabled[spellDefinition.Name].Count == SpellsContext.SpellLists.Count;
        }

        internal static bool AreMinimumSpellListsSelected()
        {
            return RegisteredSpellsList.All(x => AreMinimumSpellListsSelected(x));
        }

        internal static bool AreMinimumSpellListsSelected(SpellDefinition spellDefinition)
        {
            var minimumSpellLists = RegisteredSpells[spellDefinition].MinimumSpellLists;
            var selectedSpellLists = Main.Settings.SpellSpellListEnabled[spellDefinition.Name];

            if (minimumSpellLists.Count != selectedSpellLists.Count)
            {
                return false;
            }

            return minimumSpellLists.All(x => selectedSpellLists.Contains(x));
        }

        internal static bool AreSuggestedSpellListsSelected()
        {
            return RegisteredSpellsList.All(x => AreSuggestedSpellListsSelected(x));
        }

        internal static bool AreSuggestedSpellListsSelected(SpellDefinition spellDefinition)
        {
            var suggestedSpellLists = RegisteredSpells[spellDefinition].SuggestedSpellLists;
            var selectedSpellLists = Main.Settings.SpellSpellListEnabled[spellDefinition.Name];

            if (suggestedSpellLists.Count != selectedSpellLists.Count)
            {
                return false;
            }

            return suggestedSpellLists.All(x => selectedSpellLists.Contains(x));
        }

        public static string GenerateSpellsDescription()
        {
            var outString = new StringBuilder("[heading]Spells[/heading]");

            outString.Append("\n[list]");

            foreach (var spell in RegisteredSpellsList)
            {
                outString.Append("\n[*][b]");
                outString.Append(spell.FormatTitle());
                outString.Append("[/b]: ");
                outString.Append(spell.FormatDescription());
            }

            outString.Append("\n[/list]");

            return outString.ToString();
        }
    }
}
