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

            public bool IsMinimumSetSelected()
            {
                if (MinimumSpells.Count != SelectedSpells.Count)
                {
                    return false;
                }

                return MinimumSpells.All(x => SelectedSpells.Contains(x.Name));
            }

            public void SelectMinimumSet() => SelectedSpells.SetRange(MinimumSpells.Select(x => x.Name));

            public bool IsSuggestedSetSelected()
            {
                if (SuggestedSpells.Count != SelectedSpells.Count)
                {
                    return false;
                }

                return SuggestedSpells.All(x => SelectedSpells.Contains(x.Name));
            }

            public void SelectSuggestedSet() => SelectedSpells.SetRange(SuggestedSpells.Select(x => x.Name));

            public void Switch(SpellDefinition spellDefinition, bool active)
            {
                if (!Spells.Contains(spellDefinition))
                {
                    return;
                }

                var spellListName = SpellList.Name;
                var spellName = spellDefinition.Name;

                if (active)
                {
                    if (!Main.Settings.SpellListSpellEnabled[spellListName].Contains(spellName))
                    {
                        Main.Settings.SpellListSpellEnabled[spellListName].Add(spellName);
                    }
                }
                else
                {
                    Main.Settings.SpellListSpellEnabled[spellListName].Remove(spellName);
                }
            }
        }

        internal static readonly Dictionary<SpellListDefinition, SpellListContext> SpellListContextTab = new();

        internal static bool IsAllSetSelected => !Main.Settings.SpellListSpellEnabled.Values.Where(x => x.Count != Spells.Count).Any();

        internal static bool IsMinimumSetSelected => !SpellListContextTab.Values.Where(x => !x.IsMinimumSetSelected()).Any();

        internal static bool IsSuggestedSetSelected => !SpellListContextTab.Values.Where(x => !x.IsSuggestedSetSelected()).Any();

        internal static void SelectAllSet(bool enable)
        {
            var spellNames = Spells.Select(x => x.Name);

            foreach (var spellEnabled in Main.Settings.SpellListSpellEnabled.Values)
            {
                spellEnabled.SetRange(enable ? spellNames : new List<string>());
            }
        }

        internal static void SelectMinimumSet()
        {
            foreach (var spellListContext in SpellListContextTab.Values)
            {
                Main.Settings.SpellListSpellEnabled[spellListContext.SpellList.Name]
                    .SetRange(spellListContext.MinimumSpells.Select(x => x.Name));
            }
        }

        internal static void SelectSuggestedSet()
        {
            foreach (var spellListContext in SpellListContextTab.Values)
            {
                Main.Settings.SpellListSpellEnabled[spellListContext.SpellList.Name]
                    .SetRange(spellListContext.SuggestedSpells.Select(x => x.Name));
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

                if (!Main.Settings.DisplaySpellListsToggle.ContainsKey(name))
                {
                    Main.Settings.DisplaySpellListsToggle.Add(name, new());
                }

                if (!Main.Settings.SpellListSliderPosition.ContainsKey(name))
                {
                    Main.Settings.SpellListSliderPosition.Add(name, new());
                }

                if (!Main.Settings.SpellListSpellEnabled.ContainsKey(name))
                {
                    Main.Settings.SpellListSpellEnabled.Add(name, new());
                }
            }
        }

        internal static void Load()
        {
            InitCollections();

            BazouSpells.Register();
            SrdSpells.Register();
            HouseSpellTweaks.Register();

            Spells = Spells.OrderBy(x => x.SpellLevel).ThenBy(x => x.FormatTitle()).ToHashSet();
        }

        internal static void AddToDB()
        {
            BazouSpells.AddToDB();
            SrdSpells.AddToDB();
        }

        //
        // TODO: step2, remove this and refactor RegisterSpell calls
        //
        internal static void RegisterSpell(
            SpellDefinition spellDefinition,
            bool isFromOtherMod, 
            string minimumSpellList, 
            params string[] suggestedSpellLists)
        {
            var dbSpellListDefinition = DatabaseRepository.GetDatabase<SpellListDefinition>();
            var minimumSpells = new HashSet<SpellListDefinition>();
            var suggestedSpells = suggestedSpellLists.Select(x => dbSpellListDefinition.GetElement(x)).ToHashSet();

            if (minimumSpellList != NOT_IN_MIN_SET && dbSpellListDefinition.TryGetElement(minimumSpellList, out var spellListDefinition))
            {
                minimumSpells.Add(spellListDefinition);
                suggestedSpells.Add(spellListDefinition);
            }

            RegisterSpell(spellDefinition, minimumSpells, suggestedSpells);
        }

        internal static void RegisterSpell(
            SpellDefinition spellDefinition, 
            HashSet<SpellListDefinition> minimumSpellLists, 
            HashSet<SpellListDefinition> suggestedSpellLists)
        {
            if (Spells.Contains(spellDefinition))
            {
                return;
            }

            var spellName = spellDefinition.Name;

            Spells.Add(spellDefinition);
                
            foreach (var spellList in minimumSpellLists)
            {
                var spellEnabled = Main.Settings.SpellListSpellEnabled[spellList.Name];

                SpellListContextTab[spellList].MinimumSpells.Add(spellDefinition);
                spellEnabled.TryAdd(spellName);
            }

            foreach (var spellList in suggestedSpellLists)
            {
                SpellListContextTab[spellList].SuggestedSpells.Add(spellDefinition);
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
