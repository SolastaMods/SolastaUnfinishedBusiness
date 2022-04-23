using System.Collections.Generic;
using System.Linq;
using static SolastaModApi.DatabaseHelper.CharacterClassDefinitions;
using static SolastaModApi.DatabaseHelper.ItemDefinitions;
using static SolastaMulticlass.Models.IntegrationContext;

namespace SolastaMulticlass.Models
{
    internal static class LevelUpContext
    {
        // keeps the multiclass level up context
        private class LevelUpData
        {
            public CharacterClassDefinition SelectedClass;
            public CharacterSubclassDefinition SelectedSubclass;
            public bool IsClassSelectionStage { get; set; }
            public bool RequiresDeity { get; set; }
            public HashSet<ItemDefinition> GrantedItems { get; set; }
        }

        // keeps a tab on all heroes leveling up
        private static readonly Dictionary<RulesetCharacterHero, LevelUpData> LevelUpTab = new();

        internal static RulesetCharacterHero GetHero(string name)
            => LevelUpTab.FirstOrDefault(x => x.Key.Name == name).Key;

        internal static void RegisterHero(RulesetCharacterHero rulesetCharacterHero)
            => LevelUpTab.TryAdd(rulesetCharacterHero, new LevelUpData());
 
        internal static void UnregisterHero(RulesetCharacterHero rulesetCharacterHero)
            => LevelUpTab.Remove(rulesetCharacterHero);

        internal static CharacterClassDefinition GetSelectedClass(RulesetCharacterHero rulesetCharacterHero)
            => LevelUpTab.TryGetValue(rulesetCharacterHero, out var levelUpData)
                ? levelUpData.SelectedClass
                : null;

        internal static void SetSelectedClass(RulesetCharacterHero rulesetCharacterHero, CharacterClassDefinition characterClassDefinition)
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

        internal static CharacterSubclassDefinition GetSelectedSubclass(RulesetCharacterHero rulesetCharacterHero)
            => LevelUpTab.TryGetValue(rulesetCharacterHero, out var levelUpData)
                ? levelUpData.SelectedSubclass
                : null;

        internal static void SetSelectedSubclass(RulesetCharacterHero rulesetCharacterHero, CharacterSubclassDefinition characterSubclassDefinition)
        {
            if (!LevelUpTab.TryGetValue(rulesetCharacterHero, out var levelUpData))
            {
                return;
            }

            levelUpData.SelectedSubclass = characterSubclassDefinition;
        }

        internal static void SetIsClassSelectionStage(RulesetCharacterHero rulesetCharacterHero, bool isClassSelectionStage)
        {
            if (!LevelUpTab.TryGetValue(rulesetCharacterHero, out var levelUpData))
            {
                return;
            }

            levelUpData.IsClassSelectionStage = isClassSelectionStage;
        }

        internal static bool RequiresDeity(RulesetCharacterHero rulesetCharacterHero)
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

        internal static bool IsClassSelectionStage(RulesetCharacterHero rulesetCharacterHero)
            => LevelUpTab.TryGetValue(rulesetCharacterHero, out var levelUpData) && levelUpData.IsClassSelectionStage;

        internal static bool IsLevelingUp(RulesetCharacterHero rulesetCharacterHero)
            => LevelUpTab.TryGetValue(rulesetCharacterHero, out var _);

        internal static bool IsMulticlass(RulesetCharacterHero rulesetCharacterHero)
            => LevelUpTab.TryGetValue(rulesetCharacterHero, out var levelUpData)
                && (rulesetCharacterHero.ClassesAndLevels.Count > 1
                    || !rulesetCharacterHero.ClassesAndLevels.ContainsKey(levelUpData.SelectedClass));

        internal static void GrantItemsIfRequired(RulesetCharacterHero rulesetCharacterHero)
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

        internal static void UngrantItemsIfRequired(RulesetCharacterHero rulesetCharacterHero)
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
