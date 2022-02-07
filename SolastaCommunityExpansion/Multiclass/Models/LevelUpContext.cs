using System.Collections.Generic;
using System.Linq;
using SolastaCommunityExpansion.Multiclass.CustomDefinitions;
using static SolastaCommunityExpansion.Multiclass.Models.IntegrationContext;
using static SolastaModApi.DatabaseHelper.CharacterClassDefinitions;

namespace SolastaCommunityExpansion.Multiclass.Models
{
    internal static class LevelUpContext
    {
        private static readonly Dictionary<string, Dictionary<string, string>> featuresToReplace = new()
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

        private static readonly Dictionary<string, List<string>> featuresToExclude = new()
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

        private static bool levelingUp { get; set; }
        private static bool requiresDeity { get; set; }
        private static bool requiresHolySymbol { get; set; }
        private static bool requiresClothesWizard { get; set; }
        private static bool requiresComponentPouch { get; set; }
        private static bool requiresDruidicFocus { get; set; }
        private static bool requiresSpellbook { get; set; }
        private static bool hasHolySymbolGranted { get; set; }
        private static bool hasComponentPouchGranted { get; set; }
        private static bool hasDruidicFocusGranted { get; set; }
        private static bool hasClothesWizardGranted { get; set; }
        private static bool hasSpellbookGranted { get; set; }
        private static RulesetCharacterHero selectedHero { get; set; }
        private static CharacterClassDefinition selectedClass { get; set; }
        private static CharacterSubclassDefinition selectedSubclass { get; set; }

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

        internal static RulesetCharacterHero SelectedHero
        {
            get => selectedHero;
            set
            {
                selectedHero = value;
                selectedClass = null;
                selectedSubclass = null;
                levelingUp = value != null;
                requiresDeity = false;
                requiresHolySymbol = false;
                requiresClothesWizard = false;
                requiresComponentPouch = false;
                requiresDruidicFocus = false;
                requiresSpellbook = false;
                hasHolySymbolGranted = false;
                hasComponentPouchGranted = false;
                hasDruidicFocusGranted = false;
                hasClothesWizardGranted = false;
                hasSpellbookGranted = false;
            }
        }

        internal static CharacterClassDefinition SelectedClass
        {
            get => selectedClass;
            set
            {
                selectedClass = value;
                selectedSubclass = null;

                if (selectedClass == null)
                {
                    return;
                }

                var classesAndLevels = selectedHero.ClassesAndLevels;

                selectedHero.ClassesAndSubclasses.TryGetValue(selectedClass, out var subclass);

                selectedSubclass = subclass;

                requiresDeity =
                    (selectedClass == Cleric && !classesAndLevels.ContainsKey(Cleric)) || (selectedClass == Paladin && selectedHero.DeityDefinition == null);

                requiresHolySymbol =
                    !(classesAndLevels.ContainsKey(Cleric) || classesAndLevels.ContainsKey(Paladin)) && (selectedClass == Cleric || selectedClass == Paladin);

                requiresClothesWizard =
                    !classesAndLevels.ContainsKey(Wizard) && selectedClass == Wizard;

                requiresComponentPouch =
                    (
                        selectedClass == Ranger ||
                        selectedClass == Sorcerer ||
                        selectedClass == Wizard ||
                        selectedClass == TinkererClass ||
                        selectedClass == AlchemistClass ||
                        selectedClass == BardClass ||
                        selectedClass == WarlockClass
                    ) &&
                    !(
                        classesAndLevels.ContainsKey(Ranger) ||
                        classesAndLevels.ContainsKey(Sorcerer) ||
                        classesAndLevels.ContainsKey(Wizard) ||
                        classesAndLevels.ContainsKey(TinkererClass) ||
                        classesAndLevels.ContainsKey(AlchemistClass) ||
                        classesAndLevels.ContainsKey(BardClass) ||
                        classesAndLevels.ContainsKey(WarlockClass)
                    );

                requiresDruidicFocus = (selectedClass == Druid) && !classesAndLevels.ContainsKey(Druid);

                requiresSpellbook =
                    !classesAndLevels.ContainsKey(Wizard) && selectedClass == Wizard;
            }
        }

        internal static CharacterSubclassDefinition SelectedSubclass { get => selectedSubclass; set => selectedSubclass = value; }

        internal static int SelectedHeroLevel => (selectedHero?.ClassesHistory.Count) ?? 0;

        internal static int SelectedClassLevel
        {
            get
            {
                var classLevel = 1;

                if (selectedHero != null && selectedClass != null)
                {
                    selectedHero.ClassesAndLevels.TryGetValue(selectedClass, out classLevel);
                }

                return classLevel;
            }
        }

        internal static bool DisplayingClassPanel { get; set; }

        internal static bool LevelingUp => levelingUp;

        internal static bool RequiresDeity => requiresDeity;

        internal static bool IsMulticlass => selectedHero?.ClassesAndLevels?.Count > 1 || (selectedHero?.ClassesAndLevels.Count > 0 && selectedHero?.ClassesAndLevels.ContainsKey(selectedClass) != true);

        internal static List<FeatureUnlockByLevel> FilteredFeaturesUnlocks(List<FeatureUnlockByLevel> featureUnlockByLevels)
        {
            var dbFeatureDefinition = DatabaseRepository.GetDatabase<FeatureDefinition>();
            var filteredFeatureUnlockByLevels = new List<FeatureUnlockByLevel>();
            var firstClassName = selectedHero.ClassesHistory[0].Name;
            var selectedClassName = selectedClass.Name;
            var attacksNumber = selectedHero.GetAttribute(AttributeDefinitions.AttacksNumber);

            featuresToExclude.TryGetValue(selectedClassName, out var featureNamesToExclude);
            featuresToReplace.TryGetValue(selectedClassName, out var featureNamesToReplace);

            foreach (var featureUnlock in featureUnlockByLevels)
            {
                var featureDefinition = featureUnlock.FeatureDefinition;
                var foundFeatureToExclude = false;
                var foundFeatureToReplace = false;
                var foundExtraAttackToExclude = false;

                if (firstClassName != selectedClassName)
                {
                    // check if proficiencies should be replaced
                    if (featureNamesToReplace != null)
                    {
                        foreach (var featureNameToReplace in featureNamesToReplace.Where(x => x.Key == featureDefinition.Name))
                        {
                            var newFeatureDefinition = dbFeatureDefinition.GetElement(featureNameToReplace.Value);

                            filteredFeatureUnlockByLevels.Add(new FeatureUnlockByLevel(newFeatureDefinition, featureUnlock.Level));
                            foundFeatureToReplace = true;
                        }
                    }

                    // check if proficiencies should be excluded
                    if (featureNamesToExclude != null)
                    {
                        foundFeatureToExclude = featureNamesToExclude.Exists(x => x == featureDefinition.Name);
                    }
                }

                // check if extra attacks should be excluded
                foundExtraAttackToExclude = attacksNumber.ActiveModifiers.Count > 0
                    && featureDefinition is FeatureDefinitionAttributeModifier featureDefinitionAttributeModifier
                    && featureDefinitionAttributeModifier.ModifiedAttribute == AttributeDefinitions.AttacksNumber
                    && !(selectedClass == Fighter && SelectedClassLevel >= 11);

                // only add if not supposed to be excluded, replaced or an invalid extra attack
                if (!foundFeatureToExclude && !foundFeatureToReplace && !foundExtraAttackToExclude)
                {
                    filteredFeatureUnlockByLevels.Add(featureUnlock);
                }
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

        //
        // need to grant some additional items depending on the new class
        //

        internal static void GrantItemsIfRequired()
        {
            if (Main.Settings.EnableGrantHolySymbol)
            {
                GrantHolySymbol();
            }

            if (Main.Settings.EnableGrantCLothesWizard)
            {
                GrantClothesWizard();
            }

            if (Main.Settings.EnableGrantComponentPouch)
            {
                GrantComponentPouch();
            }

            if (Main.Settings.EnableGrantDruidicFocus)
            {
                GrantDruidicFocus();
            }

            GrantSpellbook();
        }

        internal static void UngrantItemsIfRequired()
        {
            UngrantHolySymbol();
            UngrantClothesWizard();
            UngrantComponentPouch();
            UngrantDruidicFocus();
            UngrantSpellbook();
        }

        internal static void GrantHolySymbol()
        {
            if (requiresHolySymbol && !hasHolySymbolGranted)
            {
                var holySymbolAmulet = new RulesetItemSpellbook(SolastaModApi.DatabaseHelper.ItemDefinitions.HolySymbolAmulet);

                selectedHero.GrantItem(holySymbolAmulet, true);
                hasHolySymbolGranted = true;
            }
        }

        internal static void UngrantHolySymbol()
        {
            if (selectedHero != null && hasHolySymbolGranted)
            {
                var holySymbolAmulet = new RulesetItemSpellbook(SolastaModApi.DatabaseHelper.ItemDefinitions.HolySymbolAmulet);

                selectedHero.LoseItem(holySymbolAmulet);
                hasHolySymbolGranted = false;
            }
        }

        internal static void GrantClothesWizard()
        {
            if (requiresClothesWizard && !hasClothesWizardGranted)
            {
                var clothesWizard = new RulesetItemSpellbook(SolastaModApi.DatabaseHelper.ItemDefinitions.ClothesWizard);

                selectedHero.GrantItem(clothesWizard, false);
                hasClothesWizardGranted = true;
            }
        }

        internal static void UngrantClothesWizard()
        {
            if (selectedHero != null && hasClothesWizardGranted)
            {
                var clothesWizard = new RulesetItemSpellbook(SolastaModApi.DatabaseHelper.ItemDefinitions.ClothesWizard);

                selectedHero.LoseItem(clothesWizard);
                hasClothesWizardGranted = false;
            }
        }

        internal static void GrantComponentPouch()
        {
            if (requiresComponentPouch && !hasComponentPouchGranted)
            {
                var componentPouch = new RulesetItemSpellbook(SolastaModApi.DatabaseHelper.ItemDefinitions.ComponentPouch);

                selectedHero.GrantItem(componentPouch, true);
                hasComponentPouchGranted = true;
            }
        }

        internal static void UngrantComponentPouch()
        {
            if (selectedHero != null && hasComponentPouchGranted)
            {
                var componentPouch = new RulesetItemSpellbook(SolastaModApi.DatabaseHelper.ItemDefinitions.ComponentPouch);

                selectedHero.LoseItem(componentPouch);
                hasComponentPouchGranted = false;
            }
        }

        internal static void GrantDruidicFocus()
        {
            if (requiresDruidicFocus && !hasDruidicFocusGranted)
            {
                var druidicFocus = new RulesetItemSpellbook(SolastaModApi.DatabaseHelper.ItemDefinitions.DruidicFocus);

                selectedHero.GrantItem(druidicFocus, true);
                hasDruidicFocusGranted = true;
            }
        }

        internal static void UngrantDruidicFocus()
        {
            if (selectedHero != null && hasDruidicFocusGranted)
            {
                var druidicFocus = new RulesetItemSpellbook(SolastaModApi.DatabaseHelper.ItemDefinitions.DruidicFocus);

                selectedHero.LoseItem(druidicFocus);
                hasDruidicFocusGranted = false;
            }
        }

        internal static void GrantSpellbook()
        {
            if (requiresSpellbook && !hasSpellbookGranted)
            {
                var spellbook = new RulesetItemSpellbook(SolastaModApi.DatabaseHelper.ItemDefinitions.Spellbook);

                selectedHero.GrantItem(spellbook, false);
                hasSpellbookGranted = true;
            }
        }

        internal static void UngrantSpellbook()
        {
            if (selectedHero != null && hasSpellbookGranted)
            {
                var spellbook = new RulesetItemSpellbook(SolastaModApi.DatabaseHelper.ItemDefinitions.Spellbook);

                selectedHero.LoseItem(spellbook);
                hasSpellbookGranted = false;
            }
        }

        // used on transpiler CharacterStageClassSelectionPanel.FillClassFeatures and CharacterStageClassSelectionPanel.RefreshCharacter
        public static int GetClassLevel(RulesetCharacterHero _ = null)
        {
            return selectedHero == null || selectedClass == null || !selectedHero.ClassesAndLevels.ContainsKey(selectedClass) ? 1 : selectedHero.ClassesAndLevels[selectedClass];
        }

        // used on transpiler CharacterStageLevelGainsPanel.RefreshSpellcastingFeatures
        public static List<RulesetSpellRepertoire> SpellRepertoires(RulesetCharacter rulesetCharacter)
        {
            if (levelingUp && IsMulticlass)
            {
                var result = new List<RulesetSpellRepertoire>();

                result.AddRange(rulesetCharacter.SpellRepertoires.Where(x => CacheSpellsContext.IsRepertoireFromSelectedClassSubclass(x)));

                return result;
            }

            return rulesetCharacter.SpellRepertoires;
        }

        // used on transpiler CharacterStageLevelGainsPanel.EnterStage
        public static void GetLastAssignedClassAndLevel(ICharacterBuildingService characterBuildingService, out CharacterClassDefinition lastClassDefinition, out int level)
        {
            if (levelingUp)
            {
                GrantItemsIfRequired();
                DisplayingClassPanel = false;
                lastClassDefinition = selectedClass;
                level = selectedHero.ClassesHistory.Count;
            }
            else
            {
                lastClassDefinition = null;
                level = 0;

                if (characterBuildingService.HeroCharacter.ClassesHistory.Count > 0)
                {
                    lastClassDefinition = characterBuildingService.HeroCharacter.ClassesHistory[characterBuildingService.HeroCharacter.ClassesHistory.Count - 1];
                    level = characterBuildingService.HeroCharacter.ClassesAndLevels[lastClassDefinition];
                }
            }
        }
    }
}
