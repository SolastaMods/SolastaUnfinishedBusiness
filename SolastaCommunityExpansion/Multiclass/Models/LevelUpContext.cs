using System.Collections.Generic;
using System.Linq;
using SolastaCommunityExpansion.Multiclass.CustomDefinitions;
using static SolastaCommunityExpansion.Multiclass.Models.IntegrationContext;
using static SolastaModApi.DatabaseHelper.CharacterClassDefinitions;

namespace SolastaCommunityExpansion.Multiclass.Models
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

        // these features will be replaced to comply to SRD multiclass rules
        private static readonly Dictionary<string, Dictionary<string, string>> FeaturesToReplace = new()
        {
            {
                RuleDefinitions.BarbarianClass,
                new Dictionary<string, string> {
                { "ProficiencyBarbarianArmor", "BarbarianArmorProficiencyMulticlass"} }
            },

            {
                RuleDefinitions.FighterClass,
                new Dictionary<string, string> {
                { "ProficiencyFighterArmor", "FighterArmorProficiencyMulticlass"} }
            },

            {
                RuleDefinitions.PaladinClass,
                new Dictionary<string, string> {
                 { "ProficiencyPaladinArmor", "PaladinArmorProficiencyMulticlass"} }
            },

            {
                RuleDefinitions.RangerClass,
                new Dictionary<string, string> {
                { "PointPoolRangerSkillPoints", "PointPoolRangerSkillPointsMulticlass"} }
            },

            {
                RuleDefinitions.RogueClass,
                new Dictionary<string, string> {
                { "PointPoolRogueSkillPoints", "PointPoolRogueSkillPointsMulticlass"} }
            },

            {
                CLASS_BARD,
                new Dictionary<string, string> {
                { "BardSkillProficiency", "PointPoolBardSkillPointsMulticlass"} }
            },

            {
                CLASS_WARDEN,
                new Dictionary<string, string> {
                { "ProficiencyWardenArmor", "WardenArmorProficiencyMulticlass"} }
            },
        };

        // these features will be removed to comply with SRD multiclass rules
        private static readonly Dictionary<string, List<string>> FeaturesToExclude = new()
        {
            {
                RuleDefinitions.BarbarianClass,
                new List<string> {
                "PointPoolBarbarianrSkillPoints",
                "ProficiencyBarbarianSavingThrow" }
            },

            {
                RuleDefinitions.ClericClass,
                new List<string> {
                "ProficiencyClericWeapon",
                "PointPoolClericSkillPoints",
                "ProficiencyClericSavingThrow" }
            },

            {
                RuleDefinitions.DruidClass,
                new List<string> {
                "PointPoolDruidSkillPoints",
                "ProficiencyDruidSavingThrow" }
            },

            {
                RuleDefinitions.FighterClass,
                new List<string> {
                "PointPoolFighterSkillPoints",
                "ProficiencyFighterSavingThrow" }
            },

            {
                RuleDefinitions.PaladinClass,
                new List<string> {
                "PointPoolPaladinSkillPoints",
                "ProficiencyPaladinSavingThrow" }
            },

            {
                RuleDefinitions.RangerClass,
                new List<string> {
                "ProficiencyRangerSavingThrow" }
            },

            {
                RuleDefinitions.RogueClass,
                new List<string> {
                "ProficiencyRogueWeapon",
                "ProficiencyRogueSavingThrow" }
            },

            {
                RuleDefinitions.SorcererClass,
                new List<string> {
                "ProficiencySorcererWeapon",
                "ProficiencySorcererArmor",
                "PointPoolSorcererSkillPoints",
                "ProficiencySorcererSavingThrow"}
            },

            {
                RuleDefinitions.WizardClass,
                new List<string> {
                "ProficiencyWizardWeapon",
                "ProficiencyWizardArmor",
                "PointPoolWizardSkillPoints",
                "ProficiencyWizardSavingThrow"}
            },

            {
                CLASS_TINKERER,
                new List<string> {
                "ProficiencyWeaponTinkerer",
                "PointPoolTinkererSkillPoints",
                "ProficiencyTinkererSavingThrow"}
            },

            {
                CLASS_WARDEN,
                new List<string> {
                "PointPoolWardenSkillPoints",
                "ProficiencyWardenSavingthrow" }
            },

            {
                CLASS_WITCH,
                new List<string> {
                "ProficiencyWitchWeapon",
                "PointPoolWitchSkillPoints",
                "ProficiencyWitchSavingthrow"}
            },

            {
                CLASS_ALCHEMIST,
                new List<string> {
                "AlchemistWeaponProficiency",
                "AlchemistSkillProficiency",
                "AlchemistSavingthrowProficiency" }
            },

            {
                CLASS_BARD,
                new List<string> {
                "BardWeaponProficiency",
                "BardSavingthrowProficiency" }
            },

            {
                CLASS_MONK,
                new List<string> {
                "MonkSkillProficiency",
                "MonkSavingthrowProficiency" }
            },

            {
                CLASS_WARLOCK,
                new List<string> {
                "WarlockWeaponProficiency",
                "WarlockSkillProficiency",
                "WarlockSavingthrowProficiency" }
            },
        };

        // always load the custom definitions even when multiclass is disabled to avoid errors parsing MC heroes during startup
        internal static void Load()
        {
            _ = ArmorProficiencyMulticlassBuilder.BarbarianArmorProficiencyMulticlass;
            _ = ArmorProficiencyMulticlassBuilder.FighterArmorProficiencyMulticlass;
            _ = ArmorProficiencyMulticlassBuilder.PaladinArmorProficiencyMulticlass;
            _ = ArmorProficiencyMulticlassBuilder.WardenArmorProficiencyMulticlass;
            _ = SkillProficiencyPointPoolSkills.PointPoolBardSkillPointsMulticlass;
            _ = SkillProficiencyPointPoolSkills.PointPoolRangerSkillPointsMulticlass;
            _ = SkillProficiencyPointPoolSkills.PointPoolRogueSkillPointsMulticlass;
        }

        internal static RulesetCharacterHero GetHero(string name)
        {
            var kvp = LevelUpTab.FirstOrDefault(x => x.Key.Name == name);

            return kvp.Key ?? null;
        }

        internal static void RegisterHero(RulesetCharacterHero rulesetCharacterHero)
        {
            LevelUpTab.Add(rulesetCharacterHero, new LevelUpData());
        }

        internal static void UnregisterHero(RulesetCharacterHero rulesetCharacterHero)
        {
            LevelUpTab.Remove(rulesetCharacterHero);
        }

        internal static CharacterClassDefinition GetSelectedClass(RulesetCharacterHero rulesetCharacterHero)
        {
            return LevelUpTab.TryGetValue(rulesetCharacterHero, out var levelUpData)
                ? levelUpData.SelectedClass
                : null;
        }

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
                    levelUpData.SelectedClass == AlchemistClass ||
                    levelUpData.SelectedClass == BardClass ||
                    levelUpData.SelectedClass == WarlockClass ||
                    levelUpData.SelectedClass == WitchClass
                ) &&
                !(
                    classesAndLevels.ContainsKey(Ranger) ||
                    classesAndLevels.ContainsKey(Sorcerer) ||
                    classesAndLevels.ContainsKey(Wizard) ||
                    classesAndLevels.ContainsKey(TinkererClass) ||
                    classesAndLevels.ContainsKey(AlchemistClass) ||
                    classesAndLevels.ContainsKey(BardClass) ||
                    classesAndLevels.ContainsKey(WarlockClass) ||
                    classesAndLevels.ContainsKey(WitchClass)
                );

            levelUpData.RequiresDruidicFocus =
                (levelUpData.SelectedClass == Druid) && !classesAndLevels.ContainsKey(Druid);

            levelUpData.RequiresSpellbook =
                !classesAndLevels.ContainsKey(Wizard) && levelUpData.SelectedClass == Wizard;
        }

        internal static CharacterSubclassDefinition GetSelectedSubclass(RulesetCharacterHero rulesetCharacterHero)
        {
            return LevelUpTab.TryGetValue(rulesetCharacterHero, out var levelUpData)
                ? levelUpData.SelectedSubclass
                : null;
        }

        internal static void SetSelectedSubclass(RulesetCharacterHero rulesetCharacterHero, CharacterSubclassDefinition characterSubclassDefinition)
        {
            if (!LevelUpTab.TryGetValue(rulesetCharacterHero, out var levelUpData))
            {
                return;
            }

            levelUpData.SelectedSubclass = characterSubclassDefinition;
        }

        internal static bool IsClassSelectionStage(RulesetCharacterHero rulesetCharacterHero)
        {
            return LevelUpTab.TryGetValue(rulesetCharacterHero, out var levelUpData) && levelUpData.IsClassSelectionStage;
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
        {
            return LevelUpTab.TryGetValue(rulesetCharacterHero, out var levelUpData) && levelUpData.RequiresDeity;
        }

        internal static int SelectedClassLevel(RulesetCharacterHero rulesetCharacterHero)
        {
            if (LevelUpTab.TryGetValue(rulesetCharacterHero, out var levelUpData)
                && levelUpData.SelectedClass != null
                && rulesetCharacterHero.ClassesAndLevels.TryGetValue(levelUpData.SelectedClass, out var classLevel))
            {
                return classLevel;
            }

            return 1;
        }

        internal static bool IsLevelingUp(RulesetCharacterHero rulesetCharacterHero)
        {
            return LevelUpTab.TryGetValue(rulesetCharacterHero, out var _);
        }

        internal static bool IsMulticlass(RulesetCharacterHero rulesetCharacterHero)
        {
            if (LevelUpTab.TryGetValue(rulesetCharacterHero, out var levelUpData))
            {
                return
                    rulesetCharacterHero?.ClassesAndLevels.Count > 1
                    || (rulesetCharacterHero?.ClassesAndLevels.Count > 0 && rulesetCharacterHero?.ClassesAndLevels.ContainsKey(levelUpData.SelectedClass) != true);
            }

            return false;
        }

        //
        // need to grant some additional items depending on the new class
        //

        internal static void GrantItemsIfRequired(RulesetCharacterHero rulesetCharacterHero)
        {
            if (!LevelUpTab.TryGetValue(rulesetCharacterHero, out var levelUpData))
            {
                return;
            }

            if (Main.Settings.EnableGrantHolySymbol)
            {
                GrantHolySymbol(rulesetCharacterHero, levelUpData);
            }

            if (Main.Settings.EnableGrantCLothesWizard)
            {
                GrantClothesWizard(rulesetCharacterHero, levelUpData);
            }

            if (Main.Settings.EnableGrantComponentPouch)
            {
                GrantComponentPouch(rulesetCharacterHero, levelUpData);
            }

            if (Main.Settings.EnableGrantDruidicFocus)
            {
                GrantDruidicFocus(rulesetCharacterHero, levelUpData);
            }

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
                var holySymbolAmulet = new RulesetItemSpellbook(SolastaModApi.DatabaseHelper.ItemDefinitions.HolySymbolAmulet);

                rulesetCharacterHero.GrantItem(holySymbolAmulet, true);
                levelUpData.HasHolySymbolGranted = true;
            }
        }

        private static void UngrantHolySymbol(RulesetCharacterHero rulesetCharacterHero, LevelUpData levelUpData)
        {
            if (levelUpData.HasHolySymbolGranted)
            {
                var holySymbolAmulet = new RulesetItemSpellbook(SolastaModApi.DatabaseHelper.ItemDefinitions.HolySymbolAmulet);

                rulesetCharacterHero.LoseItem(holySymbolAmulet);
                levelUpData.HasHolySymbolGranted = false;
            }
        }

        private static void GrantClothesWizard(RulesetCharacterHero rulesetCharacterHero, LevelUpData levelUpData)
        {
            if (levelUpData.RequiresClothesWizard && !levelUpData.HasClothesWizardGranted)
            {
                var clothesWizard = new RulesetItemSpellbook(SolastaModApi.DatabaseHelper.ItemDefinitions.ClothesWizard);

                rulesetCharacterHero.GrantItem(clothesWizard, false);
                levelUpData.HasClothesWizardGranted = true;
            }
        }

        private static void UngrantClothesWizard(RulesetCharacterHero rulesetCharacterHero, LevelUpData levelUpData)
        {
            if (levelUpData.HasClothesWizardGranted)
            {
                var clothesWizard = new RulesetItemSpellbook(SolastaModApi.DatabaseHelper.ItemDefinitions.ClothesWizard);

                rulesetCharacterHero.LoseItem(clothesWizard);
                levelUpData.HasClothesWizardGranted = false;
            }
        }

        private static void GrantComponentPouch(RulesetCharacterHero rulesetCharacterHero, LevelUpData levelUpData)
        {
            if (levelUpData.RequiresComponentPouch && !levelUpData.HasComponentPouchGranted)
            {
                var componentPouch = new RulesetItemSpellbook(SolastaModApi.DatabaseHelper.ItemDefinitions.ComponentPouch);

                rulesetCharacterHero.GrantItem(componentPouch, true);
                levelUpData.HasComponentPouchGranted = true;
            }
        }

        private static void UngrantComponentPouch(RulesetCharacterHero rulesetCharacterHero, LevelUpData levelUpData)
        {
            if (levelUpData.HasComponentPouchGranted)
            {
                var componentPouch = new RulesetItemSpellbook(SolastaModApi.DatabaseHelper.ItemDefinitions.ComponentPouch);

                rulesetCharacterHero.LoseItem(componentPouch);
                levelUpData.HasComponentPouchGranted = false;
            }
        }

        private static void GrantDruidicFocus(RulesetCharacterHero rulesetCharacterHero, LevelUpData levelUpData)
        {
            if (levelUpData.RequiresDruidicFocus && !levelUpData.HasDruidicFocusGranted)
            {
                var druidicFocus = new RulesetItemSpellbook(SolastaModApi.DatabaseHelper.ItemDefinitions.DruidicFocus);

                rulesetCharacterHero.GrantItem(druidicFocus, true);
                levelUpData.HasDruidicFocusGranted = true;
            }
        }

        private static void UngrantDruidicFocus(RulesetCharacterHero rulesetCharacterHero, LevelUpData levelUpData)
        {
            if (levelUpData.HasDruidicFocusGranted)
            {
                var druidicFocus = new RulesetItemSpellbook(SolastaModApi.DatabaseHelper.ItemDefinitions.DruidicFocus);

                rulesetCharacterHero.LoseItem(druidicFocus);
                levelUpData.HasDruidicFocusGranted = false;
            }
        }

        private static void GrantSpellbook(RulesetCharacterHero rulesetCharacterHero, LevelUpData levelUpData)
        {
            if (levelUpData.RequiresSpellbook && !levelUpData.HasSpellbookGranted)
            {
                var spellbook = new RulesetItemSpellbook(SolastaModApi.DatabaseHelper.ItemDefinitions.Spellbook);

                rulesetCharacterHero.GrantItem(spellbook, false);
                levelUpData.HasSpellbookGranted = true;
            }
        }

        private static void UngrantSpellbook(RulesetCharacterHero rulesetCharacterHero, LevelUpData levelUpData)
        {
            if (levelUpData.HasSpellbookGranted)
            {
                var spellbook = new RulesetItemSpellbook(SolastaModApi.DatabaseHelper.ItemDefinitions.Spellbook);

                rulesetCharacterHero.LoseItem(spellbook);
                levelUpData.HasSpellbookGranted = false;
            }
        }

        //
        // transpilers support
        //

        // used on many transpilers (20+) to support class filtered feature unlocks
        public static List<FeatureUnlockByLevel> ClassFilteredFeatureUnlocks(CharacterClassDefinition characterClassDefinition, RulesetCharacterHero rulesetCharacterHero)
        {
            var firstClass = rulesetCharacterHero.ClassesHistory[0];
            var selectedClass = IsLevelingUp(rulesetCharacterHero) ? GetSelectedClass(rulesetCharacterHero) : characterClassDefinition;

            if (!IsMulticlass(rulesetCharacterHero) || firstClass == selectedClass)
            {
                return characterClassDefinition.FeatureUnlocks;
            }

            var className = selectedClass.Name;
            var dbFeatureDefinition = DatabaseRepository.GetDatabase<FeatureDefinition>();
            var filteredFeatureUnlockByLevels = selectedClass.FeatureUnlocks.ToList();

            // remove any extra attacks except on fighter progression
            var attacksNumber = rulesetCharacterHero.GetAttribute(AttributeDefinitions.AttacksNumber);

            if (attacksNumber.ActiveModifiers.Count > 0)
            {
                filteredFeatureUnlockByLevels.RemoveAll(x =>
                    x.FeatureDefinition is FeatureDefinitionAttributeModifier featureDefinitionAttributeModifier
                    && featureDefinitionAttributeModifier.ModifiedAttribute == AttributeDefinitions.AttacksNumber
                    && !(selectedClass == Fighter && SelectedClassLevel(rulesetCharacterHero) >= 11));
            }

            // replace features per mc rules
            FeaturesToReplace.TryGetValue(className, out var featureNamesToReplace);

            if (featureNamesToReplace != null)
            {
                foreach (var featureNameToReplace in featureNamesToReplace)
                {
                    var count = filteredFeatureUnlockByLevels.RemoveAll(x => x.FeatureDefinition.Name == featureNameToReplace.Key);

                    if (count > 0)
                    {
                        var newFeatureDefinition = dbFeatureDefinition.GetElement(featureNameToReplace.Value);

                        filteredFeatureUnlockByLevels.Add(new FeatureUnlockByLevel(newFeatureDefinition, 1));
                    }
                }
            }

            // exclude features per mc rules
            FeaturesToExclude.TryGetValue(className, out var featureNamesToExclude);

            if (featureNamesToExclude != null)
            {
                filteredFeatureUnlockByLevels.RemoveAll(x => featureNamesToExclude.Contains(x.FeatureDefinition.Name));
            }

            // sort back results
            filteredFeatureUnlockByLevels.Sort((a, b) =>
            {
                var result = a.Level.CompareTo(b.Level);

                if (result == 0)
                {
                    result = a.FeatureDefinition.FormatTitle().CompareTo(b.FeatureDefinition.FormatTitle());
                }

                return result;
            });

            return filteredFeatureUnlockByLevels;
        }

        // used on many transpilers (20+) to support subclass filtered feature unlocks
        public static List<FeatureUnlockByLevel> SubclassFilteredFeatureUnlocks(CharacterSubclassDefinition characterSublassDefinition, RulesetCharacterHero rulesetCharacterHero)
        {
            if (!IsMulticlass(rulesetCharacterHero))
            {
                return characterSublassDefinition.FeatureUnlocks;
            }

            var attacksNumber = rulesetCharacterHero.GetAttribute(AttributeDefinitions.AttacksNumber);

            if (attacksNumber.ActiveModifiers.Count == 0)
            {
                return characterSublassDefinition.FeatureUnlocks;
            }

            // remove any extra attacks from sub classes if the hero already has at least one
            var filteredFeatureUnlockByLevels = characterSublassDefinition.FeatureUnlocks.ToList();

            filteredFeatureUnlockByLevels.RemoveAll(x =>
                x.FeatureDefinition is FeatureDefinitionAttributeModifier featureDefinitionAttributeModifier
                && featureDefinitionAttributeModifier.ModifiedAttribute == AttributeDefinitions.AttacksNumber);

            return filteredFeatureUnlockByLevels;
        }

        // used on transpilers CharacterStageClassSelectionPanel.FillClassFeatures and CharacterStageClassSelectionPanel.RefreshCharacter
        public static int GetClassLevel(RulesetCharacterHero rulesetCharacterHero)
        {
            var selectedClass = GetSelectedClass(rulesetCharacterHero);

            return
                selectedClass == null
                || !rulesetCharacterHero.ClassesAndLevels.ContainsKey(selectedClass) ? 1 : rulesetCharacterHero.ClassesAndLevels[selectedClass];
        }
    }
}
