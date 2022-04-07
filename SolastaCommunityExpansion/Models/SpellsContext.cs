using System.Collections.Generic;
using System.Linq;
using System.Text;
using SolastaCommunityExpansion.Spells;
using SolastaModApi.Infrastructure;

namespace SolastaCommunityExpansion.Models
{
    internal static class SpellsContext
    {
        internal const string NOT_IN_MIN_SET = null;
        internal const string CLERIC_SPELLLIST = "SpellListCleric";
        internal const string DRUID_SPELLLIST = "SpellListDruid";
        internal const string PALADIN_SPELLLIST = "SpellListPaladin";
        internal const string RANGER_SPELLLIST = "SpellListRanger";
        internal const string SORCERER_SPELLLIST = "SpellListSorcerer";
        internal const string WARLOCK_SPELLLIST = "ClassWarlockSpellList";
        internal const string WITCH_SPELLLIST = "WitchSpellList";
        internal const string WIZARD_SPELLLIST = "SpellListWizard";
        internal const string GREENMAGE_SPELLLIST = "SpellListWizardGreenmage";
        internal const string SHOCK_ARCANIST_SPELLLIST = "SpellListShockArcanist";

        internal static HashSet<SpellDefinition> Spells { get; private set; } = new();

        internal sealed class SpellListContext
        {
            private List<string> SelectedSpells => Main.Settings.SpellListSpellEnabled[SpellList.Name];
            public SpellListDefinition SpellList { get; private set; }
            public HashSet<SpellDefinition> MinimumSpells { get; private set; }
            public HashSet<SpellDefinition> SuggestedSpells { get; private set; }

            public SpellListContext(SpellListDefinition spellListDefinition)
            {
                SpellList = spellListDefinition;
                MinimumSpells = new();
                SuggestedSpells = new();
            }

            public bool IsAllSetSelected => Spells.Count == SelectedSpells.Count
                || Spells.All(x => SelectedSpells.Contains(x.Name));

            public bool IsSuggestedSetSelected => SuggestedSpells.Count == SelectedSpells.Count
                || SuggestedSpells.All(x => SelectedSpells.Contains(x.Name));

            public void SelectAllSet()
            {
                foreach (var spell in Spells)
                {
                    Switch(spell, true);
                }
            }

            public void SelectMinimumSet()
            {
                foreach (var spell in MinimumSpells)
                {
                    Switch(spell, true);
                }
            }

            public void SelectSuggestedSet()
            {
                foreach (var spell in SuggestedSpells)
                {
                    Switch(spell, true);
                }
            }

            public void Switch(SpellDefinition spellDefinition, bool active)
            {
                var spellListName = SpellList.Name;
                var spellName = spellDefinition.Name;
                var spellList = SpellList.SpellsByLevel.Find(x => x.Level == spellDefinition.SpellLevel).Spells;

                if (spellList == null)
                {
                    return;
                }

                if (active)
                {
                    spellList.TryAdd(spellDefinition);
                    Main.Settings.SpellListSpellEnabled[spellListName].TryAdd(spellName);
                }
                else
                {
                    spellList.Remove(spellDefinition);
                    Main.Settings.SpellListSpellEnabled[spellListName].Remove(spellName);
                }
            }
        }

        internal static readonly Dictionary<SpellListDefinition, SpellListContext> SpellListContextTab = new();

        internal static bool IsAllSetSelected() => !Main.Settings.SpellListSpellEnabled.Values.Where(x => x.Count != Spells.Count).Any();

        internal static bool IsSuggestedSetSelected() => !SpellListContextTab.Values.Where(x => !x.IsSuggestedSetSelected).Any();

        internal static void SelectAllSet()
        {
            foreach (var spellListContext in SpellListContextTab.Values)
            {
                spellListContext.SelectAllSet();
            }
        }

        internal static void SelectMinimumSet()
        {
            foreach (var spellListContext in SpellListContextTab.Values)
            {
                spellListContext.SelectMinimumSet();
            }
        }

        internal static void SelectSuggestedSet()
        {
            foreach (var spellListContext in SpellListContextTab.Values)
            {
                spellListContext.SelectSuggestedSet();
            }
        }

        private static readonly SortedDictionary<string, SpellListDefinition> spellLists = new();

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

                    // NOTE: don't use featureDefinitionCastSpell?. which bypasses Unity object lifetime check
                    if (featureDefinitionCastSpell
                        && featureDefinitionCastSpell.SpellListDefinition
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
                        spellLists.Add(title, featureDefinitionMagicAffinity.ExtendedSpellList);
                    }
                    else if (featureDefinition is FeatureDefinitionCastSpell featureDefinitionCastSpell
                        && featureDefinitionCastSpell.SpellListDefinition != null
                        && !spellLists.ContainsValue(featureDefinitionCastSpell.SpellListDefinition))
                    {
                        spellLists.Add(title, featureDefinitionCastSpell.SpellListDefinition);
                    }
                }

                return spellLists;
            }
        }

        private static void InitCollections()
        {
            foreach (var spellList in SpellLists.Values)
            {
                var name = spellList.Name;

                SpellListContextTab.Add(spellList, new SpellListContext(spellList));

                Main.Settings.SpellListSpellEnabled.TryAdd(name, new());
                Main.Settings.DisplaySpellListsToggle.TryAdd(name, false);
                Main.Settings.SpellListSliderPosition.TryAdd(name, 4);
            }
        }

        internal static void Load()
        {
            InitCollections();

            BazouSpells.Register();
            HolicSpells.Register();
            SrdSpells.Register();
            HouseSpellTweaks.Register();

            Spells = Spells.OrderBy(x => x.SpellLevel).ThenBy(x => x.FormatTitle()).ToHashSet();
        }

        internal static void AddToDB()
        {
            BazouSpells.AddToDB();
            HolicSpells.AddToDB();
            SrdSpells.AddToDB();
        }

        internal static void RegisterSpell(
            SpellDefinition spellDefinition,
            int suggestedStartsAt = 0,
            params string[] spellLists)
        {
            if (Spells.Contains(spellDefinition))
            {
                return;
            }

            var db = DatabaseRepository.GetDatabase<SpellListDefinition>();
            var spellName = spellDefinition.Name;

            Spells.Add(spellDefinition);
                
            for (var i = 0; i < spellLists.Length; i++)
            {
                var spellList = db.GetElement(spellLists[i]);
                var spellEnabled = Main.Settings.SpellListSpellEnabled[spellList.Name];

                SpellListContextTab[spellList].SuggestedSpells.Add(spellDefinition);

                if (i >= suggestedStartsAt)
                {
                    continue;
                }

                spellEnabled.TryAdd(spellName);
                SpellListContextTab[spellList].MinimumSpells.Add(spellDefinition);
                SpellListContextTab[spellList].Switch(spellDefinition, true);
            }
        }

#if DEBUG
        public static string GenerateSpellsDescription()
        {
            var outString = new StringBuilder("[size=3][b]Spells[/b][/size]\n");

            outString.Append("\n[list]");

            foreach (var spell in Spells)
            {
                outString.Append("\n[*][b]");
                outString.Append(spell.FormatTitle());
                outString.Append("[/b]: ");
                outString.Append(spell.FormatDescription());
            }

            outString.Append("\n[/list]");

            return outString.ToString();
        }
#endif
    }
}
