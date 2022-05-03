using System.Collections.Generic;
using System.Linq;
using static SolastaModApi.DatabaseHelper.CharacterClassDefinitions;
using static SolastaModApi.DatabaseHelper.ItemDefinitions;
using static SolastaMulticlass.Models.IntegrationContext;

namespace SolastaMulticlass.Models
{
    public static class LevelUpContext
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

        public static RulesetCharacterHero GetHero(string name)
            => LevelUpTab.FirstOrDefault(x => x.Key.Name == name).Key;

        public static void RegisterHero(RulesetCharacterHero rulesetCharacterHero)
            => LevelUpTab.TryAdd(rulesetCharacterHero, new LevelUpData());
 
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
            => LevelUpTab.TryGetValue(rulesetCharacterHero, out var _);

        public static bool IsMulticlass(RulesetCharacterHero rulesetCharacterHero)
            => LevelUpTab.TryGetValue(rulesetCharacterHero, out var levelUpData)
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

        public static bool IsSpellKnownBySelectedClassSubclass(RulesetCharacterHero rulesetCharacterHero, SpellDefinition spellDefinition)
        {
            var selectedClass = GetSelectedClass(rulesetCharacterHero);
            var selectedSubclass = GetSelectedSubclass(rulesetCharacterHero);

            var spellRepertoire = rulesetCharacterHero.SpellRepertoires.Find(sr =>
                (sr.SpellCastingFeature.SpellCastingOrigin == FeatureDefinitionCastSpell.CastingOrigin.Class
                    && sr.SpellCastingClass == selectedClass) ||
                (sr.SpellCastingFeature.SpellCastingOrigin == FeatureDefinitionCastSpell.CastingOrigin.Subclass
                    && sr.SpellCastingSubclass == selectedSubclass));

            if (spellRepertoire == null)
            {
                return false;
            }

            return spellRepertoire.HasKnowledgeOfSpell(spellDefinition);
        }

        public static bool IsSpellOfferedBySelectedClassSubclass(RulesetCharacterHero rulesetCharacterHero, SpellDefinition spellDefinition, bool onlyCurrentLevel = false)
        {
            var classLevel = GetSelectedClassLevel(rulesetCharacterHero);
            var selectedClass = GetSelectedClass(rulesetCharacterHero);
            var selectedSubclass = GetSelectedSubclass(rulesetCharacterHero);

            if (selectedClass != null && CacheSpellsContext.ClassSpellList.ContainsKey(selectedClass))
            {
                foreach (var levelSpell in CacheSpellsContext.ClassSpellList[selectedClass]
                    .Where(x => x.Key <= classLevel))
                {
                    if (levelSpell.Value.Contains(spellDefinition))
                    {
                        return true;
                    }
                    else if (onlyCurrentLevel)
                    {
                        break;
                    }
                }
            }

            if (selectedSubclass != null && CacheSpellsContext.SubclassSpellList.ContainsKey(selectedSubclass))
            {
                foreach (var levelSpell in CacheSpellsContext.SubclassSpellList[selectedSubclass]
                    .Where(x => x.Key <= classLevel))
                {
                    if (levelSpell.Value.Contains(spellDefinition))
                    {
                        return true;
                    }
                    else if (onlyCurrentLevel)
                    {
                        break;
                    }
                }
            }

            return false;
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
