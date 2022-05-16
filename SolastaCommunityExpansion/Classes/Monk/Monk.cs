using System;
using System.Collections.Generic;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaCommunityExpansion.Features;
using SolastaModApi;
using static FeatureDefinitionAttributeModifier;
using static RuleDefinitions;
using static SolastaModApi.DatabaseHelper;

namespace SolastaCommunityExpansion.Classes.Monk
{
    public static class Monk
    {
        public const string ClassName = MonkClass;
        public static readonly Guid GUID = new("1478A002-D107-4E34-93A3-CEA260DA25C9");
        public static CharacterClassDefinition Class { get; private set; }

        //TODO: maybe instead of a list make dynamic weapon checker that will tell if weapon is a monk one?
        // Monk weapons are unarmed, shortswords and any simple melee weapons that don't have the two-handed or heavy property.
        public static readonly List<WeaponTypeDefinition> MonkWeapons = new()
        {
            WeaponTypeDefinitions.ShortswordType,
            WeaponTypeDefinitions.ClubType,
            WeaponTypeDefinitions.DaggerType,
            WeaponTypeDefinitions.HandaxeType,
            WeaponTypeDefinitions.JavelinType,
            WeaponTypeDefinitions.MaceType,
            WeaponTypeDefinitions.QuarterstaffType,
            WeaponTypeDefinitions.SpearType,
            WeaponTypeDefinitions.UnarmedStrikeType
        };

        public static CharacterClassDefinition BuildClass()
        {
            if (Class != null)
            {
                throw new ArgumentException("Trying to build Monk class additional time.");
            }

            Class = CharacterClassDefinitionBuilder
                .Create(ClassName, GUID)

                #region Presentation

                .SetGuiPresentation(Category.Class,
                    CharacterClassDefinitions.Barbarian.GuiPresentation.SpriteReference) //TODO: add images
                .SetPictogram(CharacterClassDefinitions.Barbarian.ClassPictogramReference) //TODO: add class pictogram
                //.AddPersonality() //TODO: Add personality flags
                .SetAnimationId(AnimationDefinitions.ClassAnimationId.Fighter)

                #endregion

                #region Priorities

                .SetAbilityScorePriorities(
                    AttributeDefinitions.Wisdom,
                    AttributeDefinitions.Dexterity,
                    AttributeDefinitions.Constitution,
                    AttributeDefinitions.Strength,
                    AttributeDefinitions.Charisma,
                    AttributeDefinitions.Intelligence
                )
                .AddSkillPreferences(
                    DatabaseHelper.SkillDefinitions.Athletics,
                    DatabaseHelper.SkillDefinitions.History,
                    DatabaseHelper.SkillDefinitions.Insight,
                    DatabaseHelper.SkillDefinitions.Stealth,
                    DatabaseHelper.SkillDefinitions.Religion,
                    DatabaseHelper.SkillDefinitions.Perception,
                    DatabaseHelper.SkillDefinitions.Survival
                )
                .AddToolPreferences(
                    ToolTypeDefinitions.HerbalismKitType,
                    ToolTypeDefinitions.ArtisanToolSmithToolsType
                )
                //TODO: add dynamic preferred feats that can include ones that are disabled, but offered if enabled
                .AddFeatPreferences() //TODO: Add preferred feats

                #endregion

                .SetBattleAI(DecisionPackageDefinitions.DefaultMeleeWithBackupRangeDecisions)
                .SetIngredientGatheringOdds(CharacterClassDefinitions.Fighter.IngredientGatheringOdds)
                .SetHitDice(DieType.D8)

                #region Equipment

                .AddEquipmentRow(
                    new List<CharacterClassDefinition.HeroEquipmentOption>
                    {
                        EquipmentOptionsBuilder.Option(ItemDefinitions.Shortsword,
                            EquipmentDefinitions.OptionWeapon, 1)
                    },
                    new List<CharacterClassDefinition.HeroEquipmentOption>
                    {
                        EquipmentOptionsBuilder.Option(ItemDefinitions.Shortsword,
                            EquipmentDefinitions.OptionWeaponSimpleChoice, 1)
                    }
                )
                .AddEquipmentRow(new List<CharacterClassDefinition.HeroEquipmentOption>
                    {
                        EquipmentOptionsBuilder.Option(ItemDefinitions.ClothesCommon_Valley,
                            EquipmentDefinitions.OptionArmor, 1)
                    },
                    new List<CharacterClassDefinition.HeroEquipmentOption>
                    {
                        EquipmentOptionsBuilder.Option(ItemDefinitions.ClothesScavenger_A,
                            EquipmentDefinitions.OptionArmor, 1)
                    })
                .AddEquipmentRow(
                    new List<CharacterClassDefinition.HeroEquipmentOption>
                    {
                        EquipmentOptionsBuilder.Option(ItemDefinitions.DungeoneerPack,
                            EquipmentDefinitions.OptionStarterPack, 1)
                    },
                    new List<CharacterClassDefinition.HeroEquipmentOption>
                    {
                        EquipmentOptionsBuilder.Option(ItemDefinitions.ExplorerPack,
                            EquipmentDefinitions.OptionStarterPack, 1)
                    }
                )
                .AddEquipmentRow(new List<CharacterClassDefinition.HeroEquipmentOption>
                    {
                        EquipmentOptionsBuilder.Option(ItemDefinitions.Dart,
                            EquipmentDefinitions.OptionWeapon, 10)
                    },
                    new List<CharacterClassDefinition.HeroEquipmentOption>
                    {
                        EquipmentOptionsBuilder.Option(ItemDefinitions.Javelin,
                            EquipmentDefinitions.OptionWeapon, 5)
                    })

                #endregion

                #region Proficiencies

                // Weapons
                .AddFeatureAtLevel(1, FeatureDefinitionProficiencyBuilder
                    .Create("MonkWeaponProficiency", GUID)
                    .SetGuiPresentation(Category.Feature, "Feature/&WeaponTrainingShortDescription")
                    .SetProficiencies(ProficiencyType.Weapon,
                        WeaponCategoryDefinitions.SimpleWeaponCategory.Name,
                        WeaponTypeDefinitions.ShortswordType.Name)
                    .AddToDB())

                // Saves
                .AddFeatureAtLevel(1, FeatureDefinitionProficiencyBuilder
                    .Create("MonkSavingThrowProficiency", GUID)
                    .SetGuiPresentation("SavingThrowProficiency", Category.Feature)
                    .SetProficiencies(ProficiencyType.SavingThrow,
                        AttributeDefinitions.Strength,
                        AttributeDefinitions.Dexterity)
                    .AddToDB())

                // Skill points
                .AddFeatureAtLevel(1, FeatureDefinitionPointPoolBuilder
                    .Create("MonkSkillPoints", GUID)
                    .SetGuiPresentation(Category.Feature, "Feature/&SkillGainChoicesPluralDescription")
                    .SetPool(HeroDefinitions.PointsPoolType.Skill, 2)
                    .OnlyUniqueChoices()
                    .RestrictChoices(
                        SkillDefinitions.Acrobatics,
                        SkillDefinitions.Athletics,
                        SkillDefinitions.History,
                        SkillDefinitions.Insight,
                        SkillDefinitions.Religion,
                        SkillDefinitions.Stealth
                    )
                    .AddToDB())

                #endregion

                #region Level 01

                //TODO: make sure it doesn't work with shields
                //TODO: make sure it doesn't stack with other `ability bonus to AC` features
                .AddFeatureAtLevel(1, FeatureDefinitionAttributeModifierBuilder
                    .Create("MonkUnarmoredDefense", GUID)
                    .SetGuiPresentation(Category.Feature)
                    .SetModifiedAttribute(AttributeDefinitions.ArmorClass)
                    .SetModifierType2(AttributeModifierOperation.AddAbilityScoreBonus)
                    .SetModifierAbilityScore(AttributeDefinitions.Wisdom)
                    .SetSituationalContext(SituationalContext.NotWearingArmorOrMageArmor)
                    .AddToDB())
                .AddFeatureAtLevel(1, FeatureDefinitionBuilder
                    .Create("MonkMartialArts", GUID)
                    .SetGuiPresentation(Category.Feature)
                    .SetCustomSubFeatures(
                        new CanUseAttributeForWeapon(AttributeDefinitions.Dexterity, IsMonkWeapon, 
                            CharacterValidators.NoArmor, CharacterValidators.NoShield, UsingOnlyMonkWeapons),
                        new UpgradeWeaponDice(GetMartialDice, IsMonkWeapon, 
                            CharacterValidators.NoArmor, CharacterValidators.NoShield, UsingOnlyMonkWeapons))
                    .AddToDB())

                #endregion

                .AddToDB();

            return Class;
        }

        private static bool IsMonkWeapon(RulesetAttackMode attackMode, RulesetItem weapon)
        {
            return IsMonkWeapon(weapon);
        }

        private static bool IsMonkWeapon(RulesetItem weapon)
        {
            //fists
            if (weapon == null)
            {
                return true;
            }

            return MonkWeapons.Contains(weapon.ItemDefinition.WeaponDescription.WeaponTypeDefinition);
        }

        private static bool UsingOnlyMonkWeapons(RulesetCharacter character)
        {
            var inventorySlotsByName = character.CharacterInventory.InventorySlotsByName;
            var mainHand = inventorySlotsByName[EquipmentDefinitions.SlotTypeMainHand].EquipedItem;
            var offHand = inventorySlotsByName[EquipmentDefinitions.SlotTypeOffHand].EquipedItem;

            return IsMonkWeapon(mainHand) && IsMonkWeapon(offHand);
        }
        
        private static (DieType, int) GetMartialDice(RulesetCharacter character, RulesetItem weapon)
        {
            //TODO: maybe instead of level requirements count number of Martial Arts Dice upgrade features hero has
            if (character is not RulesetCharacterHero hero || !hero.ClassesAndLevels.ContainsKey(Class))
            {
                return (DieType.D1, 0);
            }
            
            var level = hero.ClassesAndLevels[Class];

            if (level >= 17)
            {
                return (DieType.D10, 1);
            }
            else if (level >= 11)
            {
                return (DieType.D8, 1);
            }
            else if (level >= 5)
            {
                return (DieType.D6, 1);
            }

            return (DieType.D4, 1);
        }
    }
}