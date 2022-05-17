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
        public const string WeaponTag = "MonkWeapon";
        public static readonly Guid GUID = new("1478A002-D107-4E34-93A3-CEA260DA25C9");
        public static CharacterClassDefinition Class { get; private set; }

        //TODO: maybe instead of a list make dynamic weapon checker that will tell if weapon is a monk one?
        // Monk weapons are unarmed, shortswords and any simple melee weapons that don't have the two-handed or heavy property.
        private static readonly List<WeaponTypeDefinition> MonkWeapons = new()
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

        private static FeatureDefinition _unarmoredMovement, _unarmoredMovementBonus;
        private static ConditionalMovementModifier _movementBonusApplier;
        private static FeatureDefinition UnarmoredMovement => _unarmoredMovement ??= BuildUnarmoredMovement();

        private static ConditionDefinition attackedWithMonkWeaponCondition;
        private static CharacterValidator attackedWithMonkWeapon;
        private static FeatureDefinitionPower kiPool;
        private static FeatureDefinition ki, martialArts, flurryOfBlows, patientDefense, stepOfTheWind;
        private static int kiPoolIncreases, martailArtsDiceProgression, unarmoredMovementProgression;

        private static FeatureDefinition UnarmoredMovementBonus =>
            _unarmoredMovementBonus ??= BuildUnarmoredMovementBonus();

        private static ConditionalMovementModifier MovementBonusApplier => _movementBonusApplier ??=
            new ConditionalMovementModifier(UnarmoredMovementBonus,
                CharacterValidators.NoArmor, CharacterValidators.NoShield);


        public static CharacterClassDefinition BuildClass()
        {
            if (Class != null)
            {
                throw new ArgumentException("Trying to build Monk class additional time.");
            }

            BuildMartialArts();
            BuildKiFeatureSet();

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
                .AddFeatureAtLevel(1, martialArts)
                .AddFeatureAtLevel(1, UnarmoredMovement)

                #endregion

                #region Level 02

                .AddFeaturesAtLevel(2, ki)

                #endregion

                #region Level 03

                .AddFeaturesAtLevel(3,
                    BuildKiPoolIncrease(),
                    BuildDeflectMissile()
                )

                #endregion

                #region Level 04

                .AddFeaturesAtLevel(4,
                    FeatureDefinitionFeatureSets.FeatureSetAbilityScoreChoice,
                    BuildKiPoolIncrease()
                )

                #endregion

                #region Level 05

                .AddFeaturesAtLevel(5,
                    BuildMartialDiceProgression(),
                    BuildKiPoolIncrease()
                )

                #endregion

                #region Level 06

                .AddFeaturesAtLevel(6,
                    BuildUnarmoredMovementImprovement(),
                    BuildKiPoolIncrease()
                )

                #endregion

                #region Level 07

                .AddFeaturesAtLevel(7,
                    BuildKiPoolIncrease()
                )

                #endregion

                #region Level 08

                .AddFeaturesAtLevel(8,
                    FeatureDefinitionFeatureSets.FeatureSetAbilityScoreChoice,
                    BuildKiPoolIncrease()
                )

                #endregion

                #region Level 09

                .AddFeaturesAtLevel(9,
                    BuildKiPoolIncrease()
                )

                #endregion

                #region Level 10

                .AddFeaturesAtLevel(10,
                    BuildUnarmoredMovementImprovement(),
                    BuildKiPoolIncrease()
                )

                #endregion

                #region Level 11

                .AddFeaturesAtLevel(11,
                    BuildMartialDiceProgression(),
                    BuildKiPoolIncrease()
                )

                #endregion

                #region Level 12

                .AddFeaturesAtLevel(12,
                    FeatureDefinitionFeatureSets.FeatureSetAbilityScoreChoice,
                    BuildKiPoolIncrease()
                )

                #endregion

                #region Level 13

                .AddFeaturesAtLevel(13,
                    BuildKiPoolIncrease()
                )

                #endregion

                #region Level 14

                .AddFeaturesAtLevel(14,
                    BuildUnarmoredMovementImprovement(),
                    BuildKiPoolIncrease()
                )

                #endregion

                #region Level 15

                .AddFeaturesAtLevel(15,
                    BuildKiPoolIncrease()
                )

                #endregion

                #region Level 16

                .AddFeaturesAtLevel(16,
                    FeatureDefinitionFeatureSets.FeatureSetAbilityScoreChoice,
                    BuildKiPoolIncrease()
                )

                #endregion

                #region Level 17

                .AddFeaturesAtLevel(17,
                    BuildMartialDiceProgression(),
                    BuildKiPoolIncrease()
                )

                #endregion

                #region Level 18

                .AddFeaturesAtLevel(18,
                    BuildUnarmoredMovementImprovement(),
                    BuildKiPoolIncrease()
                )

                #endregion

                #region Level 19

                .AddFeaturesAtLevel(19,
                    FeatureDefinitionFeatureSets.FeatureSetAbilityScoreChoice,
                    BuildKiPoolIncrease()
                )

                #endregion

                #region Level 20

                .AddFeaturesAtLevel(20,
                    BuildKiPoolIncrease()
                )

                #endregion

                .AddToDB();

            return Class;
        }

        private static void BuildMartialArts()
        {
            attackedWithMonkWeaponCondition = ConditionDefinitionBuilder
                .Create("MonkAttackedWithMonkWeapon", GUID)
                .SetGuiPresentationNoContent(true)
                .SetSilent(Silent.WhenAddedOrRemoved)
                .SetDuration(DurationType.Round, 1)
                .SetTurnOccurence(TurnOccurenceType.StartOfTurn)
                .AddToDB();

            attackedWithMonkWeapon = CharacterValidators.HasAnyOfConditions(attackedWithMonkWeaponCondition);

            var attackedWithMonkWeaponEffect = new EffectFormBuilder()
                .SetConditionForm(attackedWithMonkWeaponCondition, ConditionForm.ConditionOperation.Add, true, false)
                .Build();

            martialArts = FeatureDefinitionBuilder
                .Create("MonkMartialArts", GUID)
                .SetGuiPresentation(Category.Feature)
                .SetCustomSubFeatures(
                    //TODO: add one big sub-feature that implements all these parts to improve performance
                    new CanUseAttributeForWeapon(AttributeDefinitions.Dexterity, IsMonkWeapon,
                        CharacterValidators.NoArmor, CharacterValidators.NoShield, UsingOnlyMonkWeapons),
                    new UpgradeWeaponDice(GetMartialDice, IsMonkWeapon,
                        CharacterValidators.NoArmor, CharacterValidators.NoShield, UsingOnlyMonkWeapons),
                    new AddEffectFormToWeaponAttack(attackedWithMonkWeaponEffect, IsMonkWeapon),
                    new AddBonusUnarmedAttack(ActionDefinitions.ActionType.Bonus,
                        attackedWithMonkWeapon, UsingOnlyMonkWeapons,
                        CharacterValidators.NoShield, CharacterValidators.NoArmor,
                        CharacterValidators
                            .EmptyOffhand) //Forcing empty offhand only because it isn't really shown if character already has bonus attack
                )
                .AddToDB();
        }

        private static FeatureDefinition BuildUnarmoredMovement()
        {
            var feature = FeatureDefinitionMovementAffinityBuilder
                .Create("MonkUnarmoredMovementModifier", GUID)
                .SetGuiPresentationNoContent(true)
                .SetBaseSpeedAdditiveModifier(2)
                .AddToDB();

            return FeatureDefinitionBuilder
                .Create("MonkUnarmoredMovement", GUID)
                .SetGuiPresentation(Category.Feature)
                .SetCustomSubFeatures(new ConditionalMovementModifier(feature,
                    CharacterValidators.NoArmor, CharacterValidators.NoShield))
                .AddToDB();
        }

        private static FeatureDefinition BuildUnarmoredMovementBonus()
        {
            return FeatureDefinitionMovementAffinityBuilder
                .Create("MonkUnarmoredMovementBonusModifier", GUID)
                .SetGuiPresentationNoContent(true)
                .SetBaseSpeedAdditiveModifier(1)
                .AddToDB();
        }

        private static FeatureDefinition BuildUnarmoredMovementImprovement()
        {
            return FeatureDefinitionBuilder
                .Create($"MonkUnarmoredMovementBonus{unarmoredMovementProgression++:D2}", GUID)
                .SetGuiPresentation("MonkUnarmoredMovementBonus", Category.Feature)
                .SetCustomSubFeatures(MovementBonusApplier)
                .AddToDB();
        }

        private static FeatureDefinition BuildMartialDiceProgression()
        {
            return FeatureDefinitionBuilder
                .Create($"MonkMartialDiceProgression{martailArtsDiceProgression++:D2}", GUID)
                .SetGuiPresentation(Category.Feature)
                .AddToDB();
        }

        //TODO: rework into feature set with generated description?
        private static void BuildKiFeatureSet()
        {
            var inBattleNoShieldOrArmor = new PowerUseValidity(
                CharacterValidators.InBattle,
                CharacterValidators.NoShield,
                CharacterValidators.NoArmor
            );

            kiPool = FeatureDefinitionPowerBuilder
                .Create("MonkKiPool", GUID)
                .SetGuiPresentationNoContent(true)
                .SetUsesFixed(2)
                .SetRechargeRate(RechargeRate.ShortRest)
                .AddToDB();

            var extraFlurryAttacks = FeatureDefinitionAdditionalActionBuilder
                .Create("MonkFlurryOfBlowsExtraAttacks", GUID)
                .SetCustomSubFeatures(new AddBonusUnarmedAttack(ActionDefinitions.ActionType.Main, 2, true,
                    CharacterValidators.NoArmor, CharacterValidators.NoShield, CharacterValidators.EmptyOffhand))
                .SetMaxAttacksNumber(2)
                .SetActionType(ActionDefinitions.ActionType.Main)
                .SetRestrictedActions(ActionDefinitions.Id.AttackMain)
                .AddToDB();

            flurryOfBlows = FeatureDefinitionPowerSharedPoolBuilder
                .Create("MonkFlurryOfBlows", GUID)
                .SetGuiPresentation(Category.Power) //TODO: add icon
                .SetSharedPool(kiPool)
                .SetActivationTime(ActivationTime.BonusAction)
                .SetCostPerUse(1)
                .SetRechargeRate(RechargeRate.ShortRest)
                .SetShowCasting(false)
                .SetCustomSubFeatures(new PowerUseValidity(attackedWithMonkWeapon,
                    CharacterValidators.NoShield, CharacterValidators.NoArmor, CharacterValidators.EmptyOffhand,
                    CharacterValidators.UsedAllMainAttacks))
                .SetEffectDescription(new EffectDescriptionBuilder()
                    .AddEffectForm(new EffectFormBuilder()
                        .SetConditionForm(ConditionDefinitionBuilder
                                .Create("MonkFlurryOfBlowsCondition", GUID)
                                .SetGuiPresentationNoContent(true)
                                .SetSilent(Silent.WhenAddedOrRemoved)
                                .SetDuration(DurationType.Round, 0)
                                .SetSpecialDuration(true)
                                .SetTurnOccurence(TurnOccurenceType.EndOfTurn)
                                .SetFeatures(extraFlurryAttacks)
                                .AddToDB(),
                            ConditionForm.ConditionOperation.Add, true, true)
                        .Build())
                    .Build())
                .AddToDB();

            var dodging = ConditionDefinitions.ConditionDodging;
            patientDefense = FeatureDefinitionPowerSharedPoolBuilder
                .Create("MonkPatientDefense", GUID)
                .SetGuiPresentation(Category.Power)
                .SetSharedPool(kiPool)
                .SetActivationTime(ActivationTime.BonusAction)
                .SetCostPerUse(1)
                .SetRechargeRate(RechargeRate.ShortRest)
                .SetShowCasting(false)
                .SetCustomSubFeatures(inBattleNoShieldOrArmor)
                .SetEffectDescription(new EffectDescriptionBuilder()
                    .AddEffectForm(new EffectFormBuilder()
                        .CreatedByCharacter()
                        .SetConditionForm(ConditionDefinitionBuilder
                                .Create("MonkPatientDefenseCondition", GUID)
                                .SetGuiPresentation("ConditionDodging", Category.Rules,
                                    dodging.GuiPresentation.SpriteReference)
                                .SetConditionParticleReferenceFrom(dodging)
                                .SetSilent(Silent.None)
                                .SetDuration(DurationType.Round, 0)
                                .SetSpecialDuration(true)
                                .SetTurnOccurence(TurnOccurenceType.StartOfTurn)
                                .SetFeatures(
                                    FeatureDefinitionCombatAffinitys.CombatAffinityDodging,
                                    FeatureDefinitionSavingThrowAffinitys.SavingThrowAffinityConditionDodging
                                ).AddToDB(),
                            ConditionForm.ConditionOperation.Add, true, false)
                        .Build())
                    .Build())
                .AddToDB();

            stepOfTheWind = FeatureDefinitionPowerSharedPoolBuilder
                .Create("MonkStepOfTheWind", GUID)
                .SetGuiPresentation(Category.Power)
                .SetSharedPool(kiPool)
                .SetActivationTime(ActivationTime.BonusAction)
                .SetCostPerUse(1)
                .SetRechargeRate(RechargeRate.ShortRest)
                .SetShowCasting(false)
                .SetCustomSubFeatures(inBattleNoShieldOrArmor)
                .SetEffectDescription(new EffectDescriptionBuilder()
                    .AddEffectForm(new EffectFormBuilder()
                        .SetConditionForm(ConditionDefinitionBuilder
                                .Create("MonkStepOfTheWindCondition", GUID)
                                .SetGuiPresentation(Category.Condition,
                                    ConditionDefinitions.ConditionJump.GuiPresentation.SpriteReference)
                                .SetSilent(Silent.None)
                                .SetPossessive(true)
                                .SetDuration(DurationType.Round, 0)
                                .SetSpecialDuration(true)
                                .SetTurnOccurence(TurnOccurenceType.EndOfTurn)
                                .SetFeatures(FeatureDefinitionAdditionalActionBuilder
                                        .Create("MonkStepOfTheWindFeature", GUID)
                                        .SetGuiPresentationNoContent(true)
                                        .SetActionType(ActionDefinitions.ActionType.Bonus)
                                        .SetRestrictedActions(ActionDefinitions.Id.DashBonus,
                                            ActionDefinitions.Id.DisengageBonus)
                                        .SetAuthorizedActions(ActionDefinitions.Id.DashBonus,
                                            ActionDefinitions.Id.DisengageBonus)
                                        .AddToDB(),
                                    FeatureDefinitionMovementAffinitys.MovementAffinityJump)
                                .AddToDB(),
                            ConditionForm.ConditionOperation.Add, true, true)
                        .Build())
                    .Build())
                .AddToDB();

            ki = FeatureDefinitionFeatureSetBuilder
                .Create("MonkKi", GUID)
                .SetGuiPresentation(Category.Feature)
                .SetCustomSubFeatures(CustomSetDescription.Marker)
                .SetMode(FeatureDefinitionFeatureSet.FeatureSetMode.Union)
                .SetFeatureSet(kiPool, flurryOfBlows, patientDefense, stepOfTheWind)
                .AddToDB();
        }

        private static FeatureDefinition BuildKiPoolIncrease()
        {
            return FeatureDefinitionPowerPoolModifierBuilder
                .Create($"MonkKiPoolIncrease{kiPoolIncreases++:D2}", GUID)
                .SetGuiPresentationNoContent(true)
                .Configure(1, UsesDetermination.Fixed, "", kiPool)
                .AddToDB();
        }

        private static FeatureDefinition BuildDeflectMissile()
        {
            var deflectMissile = FeatureDefinitionActionAffinityBuilder
                .Create("MonkDeflectMissile", GUID)
                .SetGuiPresentation(Category.Feature)
                .SetAuthorizedActions(ActionDefinitions.Id.DeflectMissile)
                .SetCustomSubFeatures(new CustomMissileDeflection()
                    {characterClass = ClassName, classLevelMult = 1, descriptionTag = "Monk"})
                .AddToDB();

            deflectMissile.AllowedActionTypes = new[] {true, true, true, true, true, true};

            return deflectMissile;
        }

        private static bool IsMonkWeapon(RulesetAttackMode attackMode, RulesetItem weapon)
        {
            return IsMonkWeapon(weapon);
        }

        public static bool IsMonkWeapon(RulesetItem weapon)
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