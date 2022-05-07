using System.Collections.Generic;
using System.Linq;
using static SolastaModApi.DatabaseHelper.CharacterClassDefinitions;
using static SolastaModApi.DatabaseHelper.ItemDefinitions;
using static SolastaCommunityExpansion.Models.IntegrationContext;

namespace SolastaCommunityExpansion.Models
{
    public static class LevelUpContext
    {
        public const bool THIS_CLASS = true;
        public const bool OTHER_CLASSES = false;

        // keeps the multiclass level up context
        private class LevelUpData
        {
            public CharacterClassDefinition SelectedClass;
            public CharacterSubclassDefinition SelectedSubclass;
            public bool IsClassSelectionStage { get; set; }
            public bool IsLevelingUp { get; set; }
            public bool RequiresDeity { get; set; }
            public HashSet<ItemDefinition> GrantedItems { get; set; }
            public List<SpellDefinition> AllowedSpells { get; set; }
            public Dictionary<bool, List<SpellDefinition>> AutoPreparedSpells { get; set; }
            public Dictionary<bool, List<SpellDefinition>> KnownSpells { get; set; }
        }

        // keeps a tab on all heroes leveling up
        private static readonly Dictionary<RulesetCharacterHero, LevelUpData> LevelUpTab = new();

        public static RulesetCharacterHero GetHero(string name)
            => LevelUpTab.FirstOrDefault(x => x.Key.Name == name).Key;

        public static void RegisterHero(
            RulesetCharacterHero rulesetCharacterHero,
            CharacterClassDefinition lastClass,
            CharacterSubclassDefinition lastSubclass,
            bool levelingUp = false)
            => LevelUpTab.TryAdd(rulesetCharacterHero, new()
            {
                SelectedClass = lastClass,
                SelectedSubclass = lastSubclass,
                IsLevelingUp = levelingUp
            });
 
        public static void UnregisterHero(RulesetCharacterHero rulesetCharacterHero)
            => LevelUpTab.Remove(rulesetCharacterHero);

        public static CharacterClassDefinition GetSelectedClass(RulesetCharacterHero rulesetCharacterHero)
            => LevelUpTab.TryGetValue(rulesetCharacterHero, out var levelUpData)
                ? levelUpData.SelectedClass
                : null;

        public static void SetSelectedClass(RulesetCharacterHero rulesetCharacterHero, CharacterClassDefinition characterClassDefinition)
        {
            if (!LevelUpTab.TryGetValue(rulesetCharacterHero, out var levelUpData))
            {
                return;
            }

            levelUpData.SelectedClass = characterClassDefinition;

            bool required;
            var classesAndLevels = rulesetCharacterHero.ClassesAndLevels;

            rulesetCharacterHero.ClassesAndSubclasses.TryGetValue(levelUpData.SelectedClass, out var subclass);
            levelUpData.SelectedSubclass = subclass;

            levelUpData.RequiresDeity = 
                (levelUpData.SelectedClass == Cleric && !classesAndLevels.ContainsKey(Cleric))
                || (levelUpData.SelectedClass == Paladin && rulesetCharacterHero.DeityDefinition == null);

            levelUpData.GrantedItems = new();

            // Holy Symbol
            required =
                (
                    levelUpData.SelectedClass == Cleric ||
                    levelUpData.SelectedClass == Paladin
                ) &&
                !(
                    classesAndLevels.ContainsKey(Cleric) ||
                    classesAndLevels.ContainsKey(Paladin)
                 );

            if (required)
            {
                levelUpData.GrantedItems.Add(HolySymbolAmulet);
            }

            // Clothes Wizard
            required =
                !classesAndLevels.ContainsKey(Wizard) && levelUpData.SelectedClass == Wizard;

            if (required)
            {
                levelUpData.GrantedItems.Add(ClothesWizard);
            }

            // Component Pouch
            required =
                (
                    levelUpData.SelectedClass == Ranger ||
                    levelUpData.SelectedClass == Sorcerer ||
                    levelUpData.SelectedClass == Wizard ||
                    levelUpData.SelectedClass == TinkererClass ||
                    levelUpData.SelectedClass == WarlockClass ||
                    levelUpData.SelectedClass == WitchClass
                ) &&
                !(
                    classesAndLevels.ContainsKey(Ranger) ||
                    classesAndLevels.ContainsKey(Sorcerer) ||
                    classesAndLevels.ContainsKey(Wizard) ||
                    classesAndLevels.ContainsKey(TinkererClass) ||
                    classesAndLevels.ContainsKey(WarlockClass) ||
                    classesAndLevels.ContainsKey(WitchClass)
                );

            if (required)
            {
                levelUpData.GrantedItems.Add(ComponentPouch);
            }
            
            // Druidic Focus
            required =
                (levelUpData.SelectedClass == Druid) && !classesAndLevels.ContainsKey(Druid);

            if (required)
            {
                levelUpData.GrantedItems.Add(DruidicFocus);
            }

            // Spellbook
            required =
                !classesAndLevels.ContainsKey(Wizard) && levelUpData.SelectedClass == Wizard;

            if (required)
            {
                levelUpData.GrantedItems.Add(Spellbook);
            }
        }

        public static CharacterSubclassDefinition GetSelectedSubclass(RulesetCharacterHero rulesetCharacterHero)
            => LevelUpTab.TryGetValue(rulesetCharacterHero, out var levelUpData)
                ? levelUpData.SelectedSubclass
                : null;

        public static void SetSelectedSubclass(RulesetCharacterHero rulesetCharacterHero, CharacterSubclassDefinition characterSubclassDefinition)
        {
            if (!LevelUpTab.TryGetValue(rulesetCharacterHero, out var levelUpData))
            {
                return;
            }

            levelUpData.SelectedSubclass = characterSubclassDefinition;
        }

        public static RulesetSpellRepertoire GetSelectedClassOrSubclassRepertoire(RulesetCharacterHero rulesetCharacterHero)
            => rulesetCharacterHero.SpellRepertoires.FirstOrDefault(x =>
                (x.SpellCastingClass != null && x.SpellCastingClass == GetSelectedClass(rulesetCharacterHero))
                || (x.SpellCastingSubclass != null && x.SpellCastingSubclass == GetSelectedSubclass(rulesetCharacterHero)));

        public static void SetIsClassSelectionStage(RulesetCharacterHero rulesetCharacterHero, bool isClassSelectionStage)
        {
            if (!LevelUpTab.TryGetValue(rulesetCharacterHero, out var levelUpData))
            {
                return;
            }

            levelUpData.IsClassSelectionStage = isClassSelectionStage;
        }

        public static bool RequiresDeity(RulesetCharacterHero rulesetCharacterHero)
            => LevelUpTab.TryGetValue(rulesetCharacterHero, out var levelUpData)
                && levelUpData.RequiresDeity;

        // also referenced by 4 transpilers in PatchingContext
        public static int GetSelectedClassLevel(RulesetCharacterHero rulesetCharacterHero)
        {
            var selectedClass = GetSelectedClass(rulesetCharacterHero);

            if (selectedClass != null && rulesetCharacterHero.ClassesAndLevels.TryGetValue(selectedClass, out var classLevel))
            {
                return classLevel;
            }

            // first time hero is getting this class
            return 1;
        }

        public static bool IsClassSelectionStage(RulesetCharacterHero rulesetCharacterHero)
            => LevelUpTab.TryGetValue(rulesetCharacterHero, out var levelUpData) && levelUpData.IsClassSelectionStage;

        public static bool IsLevelingUp(RulesetCharacterHero rulesetCharacterHero)
            => LevelUpTab.TryGetValue(rulesetCharacterHero, out var levelUpData) && levelUpData.IsLevelingUp;

        public static bool IsMulticlass(RulesetCharacterHero rulesetCharacterHero)
            => LevelUpTab.TryGetValue(rulesetCharacterHero, out var levelUpData)
                && levelUpData.SelectedClass != null
                && (rulesetCharacterHero.ClassesAndLevels.Count > 1
                    || !rulesetCharacterHero.ClassesAndLevels.ContainsKey(levelUpData.SelectedClass));

        public static bool IsRepertoireFromSelectedClassSubclass(RulesetCharacterHero rulesetCharacterHero, RulesetSpellRepertoire rulesetSpellRepertoire)
        {
            var selectedClass = GetSelectedClass(rulesetCharacterHero);
            var selectedSubclass = GetSelectedSubclass(rulesetCharacterHero);

            return
                (rulesetSpellRepertoire.SpellCastingFeature.SpellCastingOrigin == FeatureDefinitionCastSpell.CastingOrigin.Class
                    && rulesetSpellRepertoire.SpellCastingClass == selectedClass) ||
                (rulesetSpellRepertoire.SpellCastingFeature.SpellCastingOrigin == FeatureDefinitionCastSpell.CastingOrigin.Subclass
                    && rulesetSpellRepertoire.SpellCastingSubclass == selectedSubclass);
        }

        private static List<FeatureDefinition> FilterCastingFeatures(IEnumerable<FeatureDefinition> featureDefinitions)
        {
            var result = new List<FeatureDefinition>();

            foreach (var featureDefinition in featureDefinitions)
            {
                if (featureDefinition is FeatureDefinitionCastSpell featureDefinitionCastSpell)
                {
                    result.Add(featureDefinitionCastSpell);
                }
                else if (featureDefinition is FeatureDefinitionMagicAffinity featureDefinitionMagicAffinity)
                {
                    result.Add(featureDefinitionMagicAffinity);
                }
                else if (featureDefinition is FeatureDefinitionAutoPreparedSpells featureDefinitionAutoPreparedSpells)
                {
                    result.Add(featureDefinitionAutoPreparedSpells);
                }
                else if (featureDefinition is FeatureDefinitionBonusCantrips featureDefinitionBonusCantrips)
                {
                    result.Add(featureDefinitionBonusCantrips);
                }
                else if (featureDefinition is FeatureDefinitionFeatureSet featureDefinitionFeatureSet)
                {
                    result.AddRange(FilterCastingFeatures(featureDefinitionFeatureSet.FeatureSet));
                }
            }

            return result;
        }

        private static Dictionary<bool, List<SpellDefinition>> GetAllKnownSpells(RulesetCharacterHero hero)
        {
            var selectedRepertoire = GetSelectedClassOrSubclassRepertoire(hero);
            var thisClassKnownSpells = new List<SpellDefinition>();
            var otherClassesKnownSpells = new List<SpellDefinition>();

            foreach (var spellRepertoire in hero.SpellRepertoires)
            {
                var thisClass = spellRepertoire == selectedRepertoire;

                if (thisClass)
                {
                    thisClassKnownSpells.AddRange(spellRepertoire.AutoPreparedSpells);
                    thisClassKnownSpells.AddRange(spellRepertoire.KnownCantrips);
                    thisClassKnownSpells.AddRange(spellRepertoire.KnownSpells);
                    thisClassKnownSpells.AddRange(spellRepertoire.EnumerateAvailableScribedSpells());
                }
                else
                {
                    otherClassesKnownSpells.AddRange(spellRepertoire.AutoPreparedSpells);
                    otherClassesKnownSpells.AddRange(spellRepertoire.KnownCantrips);
                    otherClassesKnownSpells.AddRange(spellRepertoire.KnownSpells);
                    otherClassesKnownSpells.AddRange(spellRepertoire.EnumerateAvailableScribedSpells());
                }
            }

            return new()
            {
                { false, otherClassesKnownSpells },
                { true, thisClassKnownSpells }
            };
        }

        private static Dictionary<bool, List<SpellDefinition>> GetAllAutoPreparedSpells(RulesetCharacterHero hero)
        {
            var selectedRepertoire = GetSelectedClassOrSubclassRepertoire(hero);
            var thisClassAutoPreparedSpells = new List<SpellDefinition>();
            var otherClassesAutoPreparedSpells = new List<SpellDefinition>();

            foreach (var spellRepertoire in hero.SpellRepertoires)
            {
                var thisClass = spellRepertoire == selectedRepertoire;

                if (thisClass)
                {
                    thisClassAutoPreparedSpells.AddRange(spellRepertoire.AutoPreparedSpells);
                }
                else
                {
                    otherClassesAutoPreparedSpells.AddRange(spellRepertoire.AutoPreparedSpells);
                }
            }

            return new()
            {
                { false, otherClassesAutoPreparedSpells },
                { true, thisClassAutoPreparedSpells }
            };
        }

        private static List<SpellDefinition> GetSpellsFromFeatures(List<FeatureDefinition> featureDefinitions)
        {
            var result = new List<SpellDefinition>();

            foreach (var featureDefinition in featureDefinitions)
            {
                if (featureDefinition is FeatureDefinitionCastSpell featureDefinitionCastSpell && featureDefinitionCastSpell.SpellListDefinition != null)
                {
                    result.AddRange(featureDefinitionCastSpell.SpellListDefinition.SpellsByLevel.SelectMany(x => x.Spells));
                }
                else if (featureDefinition is FeatureDefinitionMagicAffinity featureDefinitionMagicAffinity && featureDefinitionMagicAffinity.ExtendedSpellList != null)
                {
                    result.AddRange(featureDefinitionMagicAffinity.ExtendedSpellList.SpellsByLevel.SelectMany(x => x.Spells));
                }
                else if (featureDefinition is FeatureDefinitionBonusCantrips featureDefinitionBonusCantrips && featureDefinitionBonusCantrips.BonusCantrips != null)
                {
                    result.AddRange(featureDefinitionBonusCantrips.BonusCantrips);
                }
                else if (featureDefinition is FeatureDefinitionAutoPreparedSpells featureDefinitionAutoPreparedSpells && featureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroups != null)
                {
                    result.AddRange(featureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroups.SelectMany(x => x.SpellsList));
                }
            }

            return result;
        }

        public static void CacheAllowedSpells(RulesetCharacterHero rulesetCharacterHero)
        {
            if (!LevelUpTab.TryGetValue(rulesetCharacterHero, out var levelUpData))
            {
                return;
            }

            var selectedClassName = levelUpData.SelectedClass.Name;
            var thisClassCastingFeatures = rulesetCharacterHero.GetHeroBuildingData().AllActiveFeatures;
            var otherClassesCastingFeatures = rulesetCharacterHero.ActiveFeatures
                .Where(x => !x.Key.Contains(selectedClassName))
                .SelectMany(x => x.Value)
                .ToList();

            otherClassesCastingFeatures = FilterCastingFeatures(otherClassesCastingFeatures);
            thisClassCastingFeatures = FilterCastingFeatures(thisClassCastingFeatures);
            thisClassCastingFeatures.RemoveAll(x => otherClassesCastingFeatures.Contains(x));

            levelUpData.AllowedSpells = GetSpellsFromFeatures(thisClassCastingFeatures);
            levelUpData.AutoPreparedSpells = GetAllAutoPreparedSpells(rulesetCharacterHero);
            levelUpData.KnownSpells = GetAllKnownSpells(rulesetCharacterHero);
        }

        public static List<SpellDefinition> GetAllowedSpells(RulesetCharacterHero hero)
        {
            if (!LevelUpTab.TryGetValue(hero, out var levelUpData))
            {
                return new();
            }

            return levelUpData.AllowedSpells;
        }

        public static Dictionary<bool, List<SpellDefinition>> GetAutoPreparedSpells(RulesetCharacterHero hero)
        {
            if (!LevelUpTab.TryGetValue(hero, out var levelUpData))
            {
                return new()
                {
                    { false, new() },
                    { true, new() }
                };
            }

            return levelUpData.AutoPreparedSpells;
        }

        public static Dictionary<bool, List<SpellDefinition>> GetKnownSpells(RulesetCharacterHero hero)
        {
            if (!LevelUpTab.TryGetValue(hero, out var levelUpData))
            {
                return new ()
                {
                    { false, new() },
                    { true, new()}
                };
            }

            return levelUpData.KnownSpells;
        }

        public static void GrantItemsIfRequired(RulesetCharacterHero rulesetCharacterHero)
        {
            if (!LevelUpTab.TryGetValue(rulesetCharacterHero, out var levelUpData))
            {
                return;
            }

            foreach (var grantedItem in levelUpData.GrantedItems)
            {
                rulesetCharacterHero.GrantItem(grantedItem, tryToEquip: false);
            }
        }

        public static void UngrantItemsIfRequired(RulesetCharacterHero rulesetCharacterHero)
        {
            if (!LevelUpTab.TryGetValue(rulesetCharacterHero, out var levelUpData))
            {
                return;
            }

            foreach (var grantedItem in levelUpData.GrantedItems)
            {
                rulesetCharacterHero.LoseItem(grantedItem, allInstances: false);
            }
        }
    }
}
