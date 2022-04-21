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
            public bool RequiresHolySymbol { get; set; }
            public bool RequiresClothesWizard { get; set; }
            public bool RequiresComponentPouch { get; set; }
            public bool RequiresDruidicFocus { get; set; }
            public bool RequiresSpellbook { get; set; }
            public bool HasHolySymbolGranted { get; set; }
            public bool HasComponentPouchGranted { get; set; }
            public bool HasDruidicFocusGranted { get; set; }
            public bool HasClothesWizardGranted { get; set; }
            public bool HasSpellbookGranted { get; set; }
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
            levelUpData.SelectedSubclass = null;

            if (characterClassDefinition == null)
            {
                return;
            }

            var classesAndLevels = rulesetCharacterHero.ClassesAndLevels;

            rulesetCharacterHero.ClassesAndSubclasses.TryGetValue(levelUpData.SelectedClass, out var subclass);

            levelUpData.SelectedSubclass = subclass;

            levelUpData.RequiresDeity =
                (levelUpData.SelectedClass == Cleric && !classesAndLevels.ContainsKey(Cleric))
                || (levelUpData.SelectedClass == Paladin && rulesetCharacterHero.DeityDefinition == null);

            levelUpData.RequiresHolySymbol =
                !(classesAndLevels.ContainsKey(Cleric)
                || classesAndLevels.ContainsKey(Paladin)) && (levelUpData.SelectedClass == Cleric || levelUpData.SelectedClass == Paladin);

            levelUpData.RequiresClothesWizard =
                !classesAndLevels.ContainsKey(Wizard) && levelUpData.SelectedClass == Wizard;

            levelUpData.RequiresComponentPouch =
                (
                    levelUpData.SelectedClass == Ranger ||
                    levelUpData.SelectedClass == Sorcerer ||
                    levelUpData.SelectedClass == Wizard ||
                    levelUpData.SelectedClass == TinkererClass ||
                    //levelUpData.SelectedClass == AlchemistClass ||
                    //levelUpData.SelectedClass == BardClass ||
                    levelUpData.SelectedClass == WarlockClass ||
                    levelUpData.SelectedClass == WitchClass
                ) &&
                !(
                    classesAndLevels.ContainsKey(Ranger) ||
                    classesAndLevels.ContainsKey(Sorcerer) ||
                    classesAndLevels.ContainsKey(Wizard) ||
                    classesAndLevels.ContainsKey(TinkererClass) ||
                    //classesAndLevels.ContainsKey(AlchemistClass) ||
                    //classesAndLevels.ContainsKey(BardClass) ||
                    classesAndLevels.ContainsKey(WarlockClass) ||
                    classesAndLevels.ContainsKey(WitchClass)
                );

            levelUpData.RequiresDruidicFocus =
                (levelUpData.SelectedClass == Druid) && !classesAndLevels.ContainsKey(Druid);

            levelUpData.RequiresSpellbook =
                !classesAndLevels.ContainsKey(Wizard) && levelUpData.SelectedClass == Wizard;
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
            => LevelUpTab.TryGetValue(rulesetCharacterHero, out var levelUpData) && levelUpData.RequiresDeity;

        // also referenced by 4 transpilers in PatchingContext
        public static int GetSelectedClassLevel(RulesetCharacterHero rulesetCharacterHero)
        {
            var selectedClass = GetSelectedClass(rulesetCharacterHero);

            if (selectedClass != null && rulesetCharacterHero.ClassesAndLevels.TryGetValue(selectedClass, out var classLevel))
            {
                return classLevel;
            }

            return 1; // first time hero is getting this class
        }

        internal static bool IsClassSelectionStage(RulesetCharacterHero rulesetCharacterHero)
            => LevelUpTab.TryGetValue(rulesetCharacterHero, out var levelUpData) && levelUpData.IsClassSelectionStage;

        internal static bool IsLevelingUp(RulesetCharacterHero rulesetCharacterHero)
            => LevelUpTab.TryGetValue(rulesetCharacterHero, out var _);

        internal static bool IsMulticlass(RulesetCharacterHero rulesetCharacterHero)
            => LevelUpTab.TryGetValue(rulesetCharacterHero, out var levelUpData)
                && (rulesetCharacterHero.ClassesAndLevels.Count > 1
                    || !rulesetCharacterHero.ClassesAndLevels.ContainsKey(levelUpData.SelectedClass));

        //
        // need to grant some additional items depending on the new class
        //

        internal static void GrantItemsIfRequired(RulesetCharacterHero rulesetCharacterHero)
        {
            if (!LevelUpTab.TryGetValue(rulesetCharacterHero, out var levelUpData))
            {
                return;
            }

            GrantHolySymbol(rulesetCharacterHero, levelUpData);
            GrantClothesWizard(rulesetCharacterHero, levelUpData);
            GrantComponentPouch(rulesetCharacterHero, levelUpData);
            GrantDruidicFocus(rulesetCharacterHero, levelUpData);
            GrantSpellbook(rulesetCharacterHero, levelUpData);
        }

        internal static void UngrantItemsIfRequired(RulesetCharacterHero rulesetCharacterHero)
        {
            if (!LevelUpTab.TryGetValue(rulesetCharacterHero, out var levelUpData))
            {
                return;
            }

            UngrantHolySymbol(rulesetCharacterHero, levelUpData);
            UngrantClothesWizard(rulesetCharacterHero, levelUpData);
            UngrantComponentPouch(rulesetCharacterHero, levelUpData);
            UngrantDruidicFocus(rulesetCharacterHero, levelUpData);
            UngrantSpellbook(rulesetCharacterHero, levelUpData);
        }

        private static void GrantHolySymbol(RulesetCharacterHero rulesetCharacterHero, LevelUpData levelUpData)
        {
            if (levelUpData.RequiresHolySymbol && !levelUpData.HasHolySymbolGranted)
            {
                var holySymbolAmulet = new RulesetItemSpellbook(HolySymbolAmulet);

                rulesetCharacterHero.GrantItem(holySymbolAmulet, true);
                levelUpData.HasHolySymbolGranted = true;
            }
        }

        private static void UngrantHolySymbol(RulesetCharacterHero rulesetCharacterHero, LevelUpData levelUpData)
        {
            if (levelUpData.HasHolySymbolGranted)
            {
                var holySymbolAmulet = new RulesetItemSpellbook(HolySymbolAmulet);

                rulesetCharacterHero.LoseItem(holySymbolAmulet);
                levelUpData.HasHolySymbolGranted = false;
            }
        }

        private static void GrantClothesWizard(RulesetCharacterHero rulesetCharacterHero, LevelUpData levelUpData)
        {
            if (levelUpData.RequiresClothesWizard && !levelUpData.HasClothesWizardGranted)
            {
                var clothesWizard = new RulesetItem(ClothesWizard);

                rulesetCharacterHero.GrantItem(clothesWizard, false);
                levelUpData.HasClothesWizardGranted = true;
            }
        }

        private static void UngrantClothesWizard(RulesetCharacterHero rulesetCharacterHero, LevelUpData levelUpData)
        {
            if (levelUpData.HasClothesWizardGranted)
            {
                var clothesWizard = new RulesetItem(ClothesWizard);

                rulesetCharacterHero.LoseItem(clothesWizard);
                levelUpData.HasClothesWizardGranted = false;
            }
        }

        private static void GrantComponentPouch(RulesetCharacterHero rulesetCharacterHero, LevelUpData levelUpData)
        {
            if (levelUpData.RequiresComponentPouch && !levelUpData.HasComponentPouchGranted)
            {
                var componentPouch = new RulesetItem(ComponentPouch);

                rulesetCharacterHero.GrantItem(componentPouch, true);
                levelUpData.HasComponentPouchGranted = true;
            }
        }

        private static void UngrantComponentPouch(RulesetCharacterHero rulesetCharacterHero, LevelUpData levelUpData)
        {
            if (levelUpData.HasComponentPouchGranted)
            {
                var componentPouch = new RulesetItem(ComponentPouch);

                rulesetCharacterHero.LoseItem(componentPouch);
                levelUpData.HasComponentPouchGranted = false;
            }
        }

        private static void GrantDruidicFocus(RulesetCharacterHero rulesetCharacterHero, LevelUpData levelUpData)
        {
            if (levelUpData.RequiresDruidicFocus && !levelUpData.HasDruidicFocusGranted)
            {
                var druidicFocus = new RulesetItem(DruidicFocus);

                rulesetCharacterHero.GrantItem(druidicFocus, true);
                levelUpData.HasDruidicFocusGranted = true;
            }
        }

        private static void UngrantDruidicFocus(RulesetCharacterHero rulesetCharacterHero, LevelUpData levelUpData)
        {
            if (levelUpData.HasDruidicFocusGranted)
            {
                var druidicFocus = new RulesetItem(DruidicFocus);

                rulesetCharacterHero.LoseItem(druidicFocus);
                levelUpData.HasDruidicFocusGranted = false;
            }
        }

        private static void GrantSpellbook(RulesetCharacterHero rulesetCharacterHero, LevelUpData levelUpData)
        {
            if (levelUpData.RequiresSpellbook && !levelUpData.HasSpellbookGranted)
            {
                var spellbook = new RulesetItemSpellbook(Spellbook);

                rulesetCharacterHero.GrantItem(spellbook, false);
                levelUpData.HasSpellbookGranted = true;
            }
        }

        private static void UngrantSpellbook(RulesetCharacterHero rulesetCharacterHero, LevelUpData levelUpData)
        {
            if (levelUpData.HasSpellbookGranted)
            {
                var spellbook = new RulesetItemSpellbook(Spellbook);

                rulesetCharacterHero.LoseItem(spellbook);
                levelUpData.HasSpellbookGranted = false;
            }
        }
    }
}
