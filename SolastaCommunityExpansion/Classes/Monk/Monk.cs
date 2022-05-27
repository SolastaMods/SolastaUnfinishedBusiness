using System;
using System.Collections.Generic;
using SolastaCommunityExpansion.Api.AdditionalExtensions;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaCommunityExpansion.Classes.Monk.Subclasses;
using SolastaCommunityExpansion.CustomDefinitions;
using SolastaCommunityExpansion.CustomInterfaces;
using SolastaCommunityExpansion.CustomUI;
using SolastaCommunityExpansion.Models;
using SolastaCommunityExpansion.Properties;
using SolastaCommunityExpansion.Utils;
using SolastaModApi;
using UnityEngine.AddressableAssets;
using static FeatureDefinitionAttributeModifier;
using static RuleDefinitions;
using static SolastaCommunityExpansion.Utils.CustomIcons;
using static SolastaModApi.DatabaseHelper;

namespace SolastaCommunityExpansion.Classes.Monk
{
    public static class Monk
    {
        public const string ClassName = "ClassMonk";
        public const string WeaponTag = "MonkWeapon";
        public const string FlurryTag = "MonkFlurryAttack";
        public static readonly Guid GUID = new("1478A002-D107-4E34-93A3-CEA260DA25C9");

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

        private static AssetReferenceSprite _monkClassSprite,
            _monkPictogram,
            _monkKiIcon,
            _monkFlurryOfBlowsIcon,
            _monkStunningStrikeIcon,
            _monkPatientDefenseIcon,
            _monkStepOfTheWindIcon,
            _monkStillnessOfMindIcon,
            _monkSlowFallIcon,
            _monkEmptyBodyIcon;


        private static FeatureDefinition _unarmoredMovement, _unarmoredMovementBonus;
        private static ConditionalMovementModifier _movementBonusApplier;

        private static ConditionDefinition attackedWithMonkWeaponCondition;
        public static CharacterValidator attackedWithMonkWeapon;
        private static FeatureDefinition ki, martialArts, flurryOfBlows, patientDefense, stepOfTheWind, stunningStrike;
        private static int kiPoolIncreases, martailArtsDiceProgression, unarmoredMovementProgression;

        private static ConditionDefinition MonkClimbingCondition;
        public static CharacterClassDefinition Class { get; private set; }

        public static FeatureDefinitionPower KiPool { get; private set; }

        private static AssetReferenceSprite MonkClassSprite => _monkClassSprite ??=
            CreateAssetReferenceSprite("Monk", Resources.Monk, 1024, 576);

        private static AssetReferenceSprite MonkPictogram => _monkPictogram ??=
            CreateAssetReferenceSprite("MonkPictogram", Resources.MonkPictogram, 128, 128);

        private static AssetReferenceSprite MonkKiIcon => _monkKiIcon ??=
            CreateAssetReferenceSprite("MonkKiIcon", Resources.MonkKiIcon, 64, 64);

        private static AssetReferenceSprite MonkFlurryOfBlowsIcon => _monkFlurryOfBlowsIcon ??=
            CreateAssetReferenceSprite("MonkFlurryOfBlows", Resources.MonkFlurryOfBlows, 128, 64);

        private static AssetReferenceSprite MonkStunningStrikeIcon => _monkStunningStrikeIcon ??=
            CreateAssetReferenceSprite("MonkStunningStrike", Resources.MonkStunningStrike, 128, 64);

        private static AssetReferenceSprite MonkPatientDefenseIcon => _monkPatientDefenseIcon ??=
            CreateAssetReferenceSprite("MonkPatientDefense", Resources.MonkPatientDefense, 128, 64);

        private static AssetReferenceSprite MonkStepOfTheWindIcon => _monkStepOfTheWindIcon ??=
            CreateAssetReferenceSprite("MonkStepOfTheWind", Resources.MonkStepOfTheWind, 128, 64);

        private static AssetReferenceSprite MonkStillnessOfMindIcon => _monkStillnessOfMindIcon ??=
            CreateAssetReferenceSprite("MonkStillnessOfMind", Resources.MonkStillnessOfMind, 128, 64);

        private static AssetReferenceSprite MonkSlowFallIcon => _monkSlowFallIcon ??=
            CreateAssetReferenceSprite("MonkSlowFall", Resources.MonkSlowFall, 128, 64);

        private static AssetReferenceSprite MonkEmptyBodyIcon => _monkEmptyBodyIcon ??=
            CreateAssetReferenceSprite("MonkEmptyBody", Resources.MonkEmptyBody, 128, 64);

        private static FeatureDefinition UnarmoredMovement => _unarmoredMovement ??= BuildUnarmoredMovement();

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

            BuildClimbingCondition();
            BuildMartialArts();
            BuildKiFeatureSet();

            Class = CharacterClassDefinitionBuilder
                .Create(ClassName, GUID)

                #region Presentation

                .SetGuiPresentation(Category.Class, MonkClassSprite)
                .SetPictogram(MonkPictogram)
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
                    .Create("ClassMonkWeaponProficiency", GUID)
                    .SetGuiPresentation(Category.Feature, "Feature/&WeaponTrainingShortDescription")
                    .SetProficiencies(ProficiencyType.Weapon,
                        WeaponCategoryDefinitions.SimpleWeaponCategory.Name,
                        WeaponTypeDefinitions.ShortswordType.Name)
                    .AddToDB())

                // Saves
                .AddFeatureAtLevel(1, FeatureDefinitionProficiencyBuilder
                    .Create("ClassMonkSavingThrowProficiency", GUID)
                    .SetGuiPresentation("SavingThrowProficiency", Category.Feature)
                    .SetProficiencies(ProficiencyType.SavingThrow,
                        AttributeDefinitions.Strength,
                        AttributeDefinitions.Dexterity)
                    .AddToDB())

                // Skill points
                .AddFeatureAtLevel(1, FeatureDefinitionPointPoolBuilder
                    .Create("ClassMonkSkillProficiency", GUID)
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
                    .Create("ClassMonkUnarmoredDefense", GUID)
                    .SetGuiPresentation(Category.Feature)
                    .SetCustomSubFeatures(ExclusiveArmorClassBonus.MARKER)
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

                #region Subclasses

                .AddFeatureAtLevel(3, FeatureDefinitionSubclassChoiceBuilder
                    .Create("ClassMonkSubclassChoice", DefinitionBuilder.CENamespaceGuid)
                    .SetGuiPresentation("ClassMonkTradition", Category.Subclass)
                    .SetSubclassSuffix("ClassMonkTradition")
                    .SetFilterByDeity(false)
                    .SetSubclasses(
                        WayOfTheOpenHand.Build(),
                        ZenArcher.Build()
                    )
                    .AddToDB())

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
                    BuildSlowFall(),
                    BuildKiPoolIncrease()
                )

                #endregion

                #region Level 05

                .AddFeaturesAtLevel(5,
                    BuildMartialDiceProgression(),
                    stunningStrike,
                    BuildExtraAttack(),
                    BuildKiPoolIncrease()
                )

                #endregion

                #region Level 06

                .AddFeaturesAtLevel(6,
                    BuildUnarmoredMovementImprovement(),
                    BuildKiEmpoweredStrikes(),
                    BuildKiPoolIncrease()
                )

                #endregion

                #region Level 07

                .AddFeaturesAtLevel(7,
                    FeatureDefinitionSavingThrowAffinitys.SavingThrowAffinityRogueEvasion,
                    BuildStillnessOfMind(),
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
                    BuildUnarmoredMovementVerticalSurface(),
                    BuildKiPoolIncrease()
                )

                #endregion

                #region Level 10

                .AddFeaturesAtLevel(10,
                    BuildUnarmoredMovementImprovement(),
                    BuildPurityOfBody(),
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
                    BuildTongueOfSunAndMoon(),
                    BuildKiPoolIncrease()
                )

                #endregion

                #region Level 14

                .AddFeaturesAtLevel(14,
                    BuildUnarmoredMovementImprovement(),
                    BuildDiamondSoul(),
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
                    BuildEmptyBody(),
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
                    BuildPerfectSelf(),
                    BuildKiPoolIncrease()
                )

                #endregion

                .AddToDB();

            return Class;
        }

        private static void BuildMartialArts()
        {
            attackedWithMonkWeaponCondition = ConditionDefinitionBuilder
                .Create("ClassMonkAttackedWithMonkWeapon", GUID)
                .SetGuiPresentationNoContent(true)
                .SetSilent(Silent.WhenAddedOrRemoved)
                .SetDuration(DurationType.Round, 1)
                .SetTurnOccurence(TurnOccurenceType.StartOfTurn)
                .SetSpecialInterruptions(ConditionInterruption.BattleEnd, ConditionInterruption.AnyBattleTurnEnd)
                .AddToDB();

            attackedWithMonkWeapon = CharacterValidators.HasAnyOfConditions(attackedWithMonkWeaponCondition);

            martialArts = FeatureDefinitionBuilder
                .Create("ClassMonkMartialArts", GUID)
                .SetGuiPresentation(Category.Feature)
                .SetCustomSubFeatures(
                    new CanUseAttributeForWeapon(AttributeDefinitions.Dexterity, IsMonkWeapon,
                        CharacterValidators.NoArmor, CharacterValidators.NoShield),
                    new UpgradeWeaponDice(GetMartialDice, IsMonkWeapon,
                        CharacterValidators.NoArmor, CharacterValidators.NoShield),
                    new ApplyMonkWeaponStatusOnAttack(),
                    //TODO: add an option in mod setting to include or exclude this unarmed attack, plus maybe add checks that you have weapon in main hand, so no double options
                    // new AddBonusUnarmedAttack(ActionDefinitions.ActionType.Main, 
                    //     CharacterValidators.NoArmor, CharacterValidators.NoShield),
                    new AddExtraUnarmedAttack(ActionDefinitions.ActionType.Bonus, UsingOnlyMonkWeapons,
                        attackedWithMonkWeapon, CharacterValidators.NoShield, CharacterValidators.NoArmor)
                )
                .AddToDB();
        }

        private static FeatureDefinition BuildUnarmoredMovement()
        {
            var feature = FeatureDefinitionMovementAffinityBuilder
                .Create("ClassMonkUnarmoredMovementModifier", GUID)
                .SetGuiPresentationNoContent(true)
                .SetBaseSpeedAdditiveModifier(2)
                .AddToDB();

            return FeatureDefinitionBuilder
                .Create("ClassMonkUnarmoredMovement", GUID)
                .SetGuiPresentation(Category.Feature)
                .SetCustomSubFeatures(new ConditionalMovementModifier(feature,
                    CharacterValidators.NoArmor, CharacterValidators.NoShield))
                .AddToDB();
        }

        private static FeatureDefinition BuildUnarmoredMovementBonus()
        {
            return FeatureDefinitionMovementAffinityBuilder
                .Create("ClassMonkUnarmoredMovementBonusModifier", GUID)
                .SetGuiPresentationNoContent(true)
                .SetBaseSpeedAdditiveModifier(1)
                .AddToDB();
        }

        private static FeatureDefinition BuildUnarmoredMovementImprovement()
        {
            return FeatureDefinitionBuilder
                .Create($"ClassMonkUnarmoredMovementBonus{unarmoredMovementProgression++:D2}", GUID)
                .SetGuiPresentation("ClassMonkUnarmoredMovementBonus", Category.Feature)
                .SetCustomSubFeatures(MovementBonusApplier)
                .AddToDB();
        }

        private static FeatureDefinition BuildUnarmoredMovementVerticalSurface()
        {
            return FeatureDefinitionBuilder
                .Create("ClassMonkUnarmoredMovementVerticalSurface", GUID)
                .SetGuiPresentation(Category.Feature)
                .SetCustomSubFeatures(new MonkClimbing())
                .AddToDB();
        }

        private static void BuildClimbingCondition()
        {
            MonkClimbingCondition = ConditionDefinitionBuilder
                .Create(ConditionDefinitions.ConditionSpiderClimb, "ClassMonkClimbingCondition", GUID)
                .SetSilent(Silent.WhenAddedOrRemoved)
                .AddToDB();
        }

        private static FeatureDefinition BuildMartialDiceProgression()
        {
            return FeatureDefinitionBuilder
                .Create($"ClassMonkMartialDiceProgression{martailArtsDiceProgression++:D2}", GUID)
                .SetGuiPresentation(Category.Feature)
                .AddToDB();
        }

        private static void BuildKiFeatureSet()
        {
            var inBattleNoShieldOrArmor = new PowerUseValidity(
                CharacterValidators.InBattle,
                CharacterValidators.NoShield,
                CharacterValidators.NoArmor
            );

            KiPool = FeatureDefinitionPowerBuilder
                .Create("ClassMonkKiPool", GUID)
                .SetGuiPresentation(Category.Power, hidden: true)
                .SetUsesFixed(2)
                .SetRechargeRate(RechargeRate.ShortRest)
                .AddToDB();

            KiPool.SetCustomSubFeatures(new CustomPortraitPoolPower(KiPool, icon: MonkKiIcon));

            var extraFlurryAttack1 = FeatureDefinitionAdditionalActionBuilder
                .Create("ClassMonkFlurryOfBlowsExtraAttacks1", GUID)
                .SetGuiPresentationNoContent(true)
                .SetCustomSubFeatures(new AddExtraUnarmedAttack(ActionDefinitions.ActionType.Bonus, 1, true,
                        CharacterValidators.NoArmor, CharacterValidators.NoShield)
                    .SetTags(FlurryTag))
                .SetActionType(ActionDefinitions.ActionType.Bonus)
                .SetRestrictedActions(ActionDefinitions.Id.AttackOff)
                .AddToDB();

            var extraFlurryAttack2 = FeatureDefinitionAdditionalActionBuilder
                .Create("ClassMonkFlurryOfBlowsExtraAttacks2", GUID)
                .SetGuiPresentationNoContent(true)
                .SetActionType(ActionDefinitions.ActionType.Bonus)
                .SetRestrictedActions(ActionDefinitions.Id.AttackOff)
                .AddToDB();

            flurryOfBlows = FeatureDefinitionPowerSharedPoolBuilder
                .Create("ClassMonkFlurryOfBlows", GUID)
                .SetGuiPresentation(Category.Power, MonkFlurryOfBlowsIcon)
                .SetSharedPool(KiPool)
                .SetActivationTime(ActivationTime.BonusAction)
                .SetCostPerUse(1)
                .SetRechargeRate(RechargeRate.ShortRest)
                .SetShowCasting(false)
                .SetCustomSubFeatures(new PowerUseValidity(attackedWithMonkWeapon,
                    CharacterValidators.NoShield, CharacterValidators.NoArmor))
                .SetEffectDescription(new EffectDescriptionBuilder()
                    .AddEffectForm(new EffectFormBuilder()
                        .SetConditionForm(ConditionDefinitionBuilder
                                .Create("ClassMonkFlurryOfBlowsCondition", GUID)
                                .SetGuiPresentationNoContent(true)
                                .SetSilent(Silent.WhenAddedOrRemoved)
                                .SetDuration(DurationType.Round, 0, false)
                                .SetSpecialDuration(true)
                                .SetTurnOccurence(TurnOccurenceType.EndOfTurn)
                                .SetSpecialInterruptions(ConditionInterruption.BattleEnd,
                                    ConditionInterruption.AnyBattleTurnEnd)
                                .SetFeatures(extraFlurryAttack1, extraFlurryAttack2)
                                .AddToDB(),
                            ConditionForm.ConditionOperation.Add, true, true)
                        .Build())
                    .Build())
                .AddToDB();

            var dodging = ConditionDefinitions.ConditionDodging;
            patientDefense = FeatureDefinitionPowerSharedPoolBuilder
                .Create("ClassMonkPatientDefense", GUID)
                .SetGuiPresentation(Category.Power, MonkPatientDefenseIcon)
                .SetSharedPool(KiPool)
                .SetActivationTime(ActivationTime.BonusAction)
                .SetCostPerUse(1)
                .SetRechargeRate(RechargeRate.ShortRest)
                .SetShowCasting(false)
                .SetCustomSubFeatures(inBattleNoShieldOrArmor)
                .SetEffectDescription(new EffectDescriptionBuilder()
                    .AddEffectForm(new EffectFormBuilder()
                        .CreatedByCharacter()
                        .SetConditionForm(ConditionDefinitionBuilder
                                .Create("ClassMonkPatientDefenseCondition", GUID)
                                .SetGuiPresentation("ConditionDodging", Category.Rules,
                                    dodging.GuiPresentation.SpriteReference)
                                .SetConditionParticleReferenceFrom(dodging)
                                .SetSilent(Silent.None)
                                .SetDuration(DurationType.Round, 0, false)
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
                .Create("ClassMonkStepOfTheWind", GUID)
                .SetGuiPresentation(Category.Power, MonkStepOfTheWindIcon)
                .SetSharedPool(KiPool)
                .SetActivationTime(ActivationTime.BonusAction)
                .SetCostPerUse(1)
                .SetRechargeRate(RechargeRate.ShortRest)
                .SetShowCasting(false)
                .SetCustomSubFeatures(inBattleNoShieldOrArmor)
                .SetEffectDescription(new EffectDescriptionBuilder()
                    .AddEffectForm(new EffectFormBuilder()
                        .SetConditionForm(ConditionDefinitionBuilder
                                .Create("ClassMonkStepOfTheWindCondition", GUID)
                                .SetGuiPresentation(Category.Condition,
                                    ConditionDefinitions.ConditionJump.GuiPresentation.SpriteReference)
                                .SetSilent(Silent.None)
                                .SetPossessive(true)
                                .SetDuration(DurationType.Round, 0, false)
                                .SetSpecialDuration(true)
                                .SetTurnOccurence(TurnOccurenceType.EndOfTurn)
                                .SetFeatures(FeatureDefinitionAdditionalActionBuilder
                                        .Create("ClassMonkStepOfTheWindFeature", GUID)
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
                .Create("ClassMonkKi", GUID)
                .SetGuiPresentation(Category.Feature)
                .SetCustomSubFeatures(CustomSetDescription.Marker)
                .SetMode(FeatureDefinitionFeatureSet.FeatureSetMode.Union)
                .SetFeatureSet(KiPool, flurryOfBlows, patientDefense, stepOfTheWind)
                .AddToDB();

            stunningStrike = FeatureDefinitionPowerSharedPoolBuilder
                .Create("ClassMonkStunningStrike", GUID)
                .SetGuiPresentation(Category.Power, MonkStunningStrikeIcon)
                .SetSharedPool(KiPool)
                .SetActivationTime(ActivationTime.OnAttackHit)
                .SetRechargeRate(RechargeRate.ShortRest)
                .SetCostPerUse(1)
                .SetCustomSubFeatures(new ReactionAttackModeRestriction(
                    ReactionAttackModeRestriction.MeleeOnly,
                    ReactionAttackModeRestriction.TargenHasNoCondition(ConditionDefinitions.ConditionStunned)
                ))
                .SetEffectDescription(new EffectDescriptionBuilder()
                    .SetDurationData(DurationType.Round, 1)
                    .SetTargetingData(Side.Enemy, RangeType.Self, 1, TargetType.Individuals)
                    .SetTargetFiltering(TargetFilteringMethod.CharacterOnly)
                    .SetSavingThrowData(
                        true,
                        true,
                        AttributeDefinitions.Constitution,
                        true,
                        EffectDifficultyClassComputation.AbilityScoreAndProficiency,
                        AttributeDefinitions.Wisdom
                    )
                    .SetEffectForms(new EffectFormBuilder()
                        .HasSavingThrow(EffectSavingThrowType.Negates)
                        .SetLevelAdvancement(EffectForm.LevelApplianceType.No, LevelSourceType.ClassLevel)
                        .SetConditionForm(ConditionDefinitions.ConditionStunned, ConditionForm.ConditionOperation.Add)
                        .Build())
                    .Build())
                .AddToDB();
        }

        private static FeatureDefinition BuildKiPoolIncrease()
        {
            return FeatureDefinitionPowerPoolModifierBuilder
                .Create($"ClassMonkKiPoolIncrease{kiPoolIncreases++:D2}", GUID)
                .SetGuiPresentationNoContent(true)
                .Configure(1, UsesDetermination.Fixed, "", KiPool)
                .AddToDB();
        }

        private static FeatureDefinition BuildDeflectMissile()
        {
            var deflectMissile = FeatureDefinitionActionAffinityBuilder
                .Create("ClassMonkDeflectMissile", GUID)
                .SetGuiPresentation(Category.Feature)
                .SetAuthorizedActions(ActionDefinitions.Id.DeflectMissile)
                .SetCustomSubFeatures(new CustomMissileDeflection
                {
                    characterClass = ClassName, classLevelMult = 1, descriptionTag = "Monk"
                })
                .AddToDB();

            deflectMissile.AllowedActionTypes = new[] {true, true, true, true, true, true};

            return deflectMissile;
        }

        private static FeatureDefinition BuildSlowFall()
        {
            //TODO: should we hide it from power menu?
            return FeatureDefinitionPowerBuilder
                .Create("ClassMonkSlowFall", GUID)
                .SetGuiPresentation(Category.Power, MonkSlowFallIcon)
                .SetActivationTime(ActivationTime.Reaction)
                .SetRechargeRate(RechargeRate.AtWill)
                .SetCostPerUse(0)
                .SetEffectDescription(new EffectDescriptionBuilder(SpellDefinitions.FeatherFall.EffectDescription)
                    .SetTargetFiltering(TargetFilteringMethod.CharacterOnly)
                    .SetTargetingData(Side.Ally, RangeType.Self, 1, TargetType.Self)
                    .SetDurationData(DurationType.Round, 1)
                    .SetEffectForms(new EffectFormBuilder()
                        .SetConditionForm(ConditionDefinitions.ConditionFeatherFalling,
                            ConditionForm.ConditionOperation.Add)
                        .Build())
                    .Build())
                .AddToDB();
        }

        private static FeatureDefinition BuildExtraAttack()
        {
            return FeatureDefinitionAttributeModifierBuilder
                .Create("ClassMonkExtraAttack", GUID)
                .SetGuiPresentation(Category.Feature)
                .SetModifiedAttribute(AttributeDefinitions.AttacksNumber)
                .SetModifierType2(AttributeModifierOperation.Additive)
                .SetModifierValue(1)
                .AddToDB();
        }

        private static FeatureDefinition BuildKiEmpoweredStrikes()
        {
            return FeatureDefinitionBuilder
                .Create("ClassMonkKiEmpoweredStrikes", GUID)
                .SetGuiPresentation(Category.Feature)
                .SetCustomSubFeatures(new AddTagToWeaponAttack(TagsDefinitions.Magical,
                    WeaponValidators.IsUnarmedWeapon))
                .AddToDB();
        }

        private static FeatureDefinition BuildStillnessOfMind()
        {
            return FeatureDefinitionPowerBuilder
                .Create("ClassMonkKiStillnessOfMind", GUID)
                .SetGuiPresentation(Category.Power, MonkStillnessOfMindIcon)
                .SetActivationTime(ActivationTime.Action)
                .SetCostPerUse(0)
                .SetEffectDescription(new EffectDescriptionBuilder()
                    .SetTargetingData(Side.Ally, RangeType.Self, 1, TargetType.Self)
                    .SetDurationData(DurationType.Instantaneous)
                    .SetEffectForms(new EffectFormBuilder()
                        .SetConditionForm(null, ConditionForm.ConditionOperation.RemoveDetrimentalRandom, true, false,
                            ConditionDefinitions.ConditionCharmed,
                            ConditionDefinitions.ConditionFrightened
                        )
                        .Build())
                    .Build())
                .AddToDB();
        }

        private static FeatureDefinition BuildPurityOfBody()
        {
            return FeatureDefinitionFeatureSetBuilder
                .Create("ClassMonkPurityOfBody", GUID)
                .SetGuiPresentation(Category.Feature)
                .SetFeatureSet(
                    FeatureDefinitionConditionAffinitys.ConditionAffinityPoisonImmunity,
                    FeatureDefinitionConditionAffinitys.ConditionAffinityDiseaseImmunity,
                    FeatureDefinitionDamageAffinitys.DamageAffinityPoisonImmunity
                )
                .AddToDB();
        }

        private static FeatureDefinition BuildDiamondSoul()
        {
            return FeatureDefinitionProficiencyBuilder
                .Create("ClassMonkDiamondSoul", GUID)
                .SetGuiPresentation(Category.Feature)
                .SetProficiencies(ProficiencyType.SavingThrow,
                    AttributeDefinitions.Strength,
                    AttributeDefinitions.Dexterity,
                    AttributeDefinitions.Constitution,
                    AttributeDefinitions.Intelligence,
                    AttributeDefinitions.Wisdom,
                    AttributeDefinitions.Charisma
                )
                .SetCustomSubFeatures(new CustomRerollFailedSave(FeatureDefinitionPowerSharedPoolBuilder
                    .Create("ClassMonkDiamondSoulPower", GUID)
                    .SetGuiPresentation(Category.Power)
                    .SetSharedPool(KiPool)
                    .SetCostPerUse(1)
                    .AddToDB(), "DiamondSoul"))
                .AddToDB();
        }

        private static FeatureDefinition BuildTongueOfSunAndMoon()
        {
            return FeatureDefinitionFeatureSetBuilder
                .Create("ClassMonkTongueOfSunAndMoon", GUID)
                .SetGuiPresentation(Category.Feature)
                .SetFeatureSet(FeatureDefinitionFeatureSets.FeatureSetAllLanguagesButCode.FeatureSet.ToArray())
                .AddToDB();
        }

        private static FeatureDefinition BuildEmptyBody()
        {
            return FeatureDefinitionPowerSharedPoolBuilder
                .Create("ClassMonkEmptyBody", GUID)
                .SetGuiPresentation(Category.Power, MonkEmptyBodyIcon)
                .SetSharedPool(KiPool)
                .SetCostPerUse(4)
                .SetActivationTime(ActivationTime.Action)
                .SetRechargeRate(RechargeRate.ShortRest)
                .SetEffectDescription(new EffectDescriptionBuilder()
                    .SetTargetingData(Side.Ally, RangeType.Self, 1, TargetType.Self)
                    .SetDurationData(DurationType.Minute, 1)
                    .SetEffectForms(
                        new EffectFormBuilder()
                            .SetConditionForm(ConditionDefinitions.ConditionInvisibleGreater,
                                ConditionForm.ConditionOperation.Add)
                            .Build(),
                        new EffectFormBuilder()
                            .SetConditionForm(ConditionDefinitionBuilder
                                .Create("ClassMonkEmptyBodyCondition", GUID)
                                .SetGuiPresentation(Category.Condition,
                                    ConditionDefinitions.ConditionShielded.GuiPresentation.SpriteReference)
                                .AddFeatures(
                                    FeatureDefinitionDamageAffinitys.DamageAffinityAcidResistance,
                                    FeatureDefinitionDamageAffinitys.DamageAffinityColdResistance,
                                    FeatureDefinitionDamageAffinitys.DamageAffinityFireResistance,
                                    FeatureDefinitionDamageAffinitys.DamageAffinityLightningResistance,
                                    FeatureDefinitionDamageAffinitys.DamageAffinityNecroticResistance,
                                    FeatureDefinitionDamageAffinitys.DamageAffinityPoisonResistance,
                                    FeatureDefinitionDamageAffinitys.DamageAffinityPsychicResistance,
                                    FeatureDefinitionDamageAffinitys.DamageAffinityRadiantResistance,
                                    FeatureDefinitionDamageAffinitys.DamageAffinityThunderResistance,
                                    FeatureDefinitionDamageAffinityBuilder
                                        .Create("ClassMonkEmptyBodyBludgeoningResistance", GUID)
                                        .SetDamageType(DamageTypeBludgeoning)
                                        .SetDamageAffinityType(DamageAffinityType.Resistance)
                                        .AddToDB(),
                                    FeatureDefinitionDamageAffinityBuilder
                                        .Create("ClassMonkEmptyBodyPiercingResistance", GUID)
                                        .SetDamageType(DamageTypePiercing)
                                        .SetDamageAffinityType(DamageAffinityType.Resistance)
                                        .AddToDB(),
                                    FeatureDefinitionDamageAffinityBuilder
                                        .Create("ClassMonkEmptyBodySlashingResistance", GUID)
                                        .SetDamageType(DamageTypeSlashing)
                                        .SetDamageAffinityType(DamageAffinityType.Resistance)
                                        .AddToDB()
                                )
                                .SetPossessive(true)
                                .AddToDB(), ConditionForm.ConditionOperation.Add)
                            .Build()
                    )
                    .Build())
                .AddToDB();
        }

        private static FeatureDefinition BuildPerfectSelf()
        {
            return FeatureDefinitionBuilder
                .Create("ClassMonkPerfectSelf", GUID)
                .SetGuiPresentation(Category.Feature)
                .SetCustomSubFeatures(new PerfectSelf())
                .AddToDB();
        }

        public static bool IsMonkWeapon(RulesetAttackMode attackMode, RulesetItem weapon, RulesetCharacter character)
        {
            return IsMonkWeapon(character, attackMode) || IsMonkWeapon(character, weapon);
        }

        public static bool IsMonkWeapon(RulesetCharacter character, RulesetAttackMode attackMode)
        {
            if (attackMode is not {SourceDefinition: ItemDefinition item})
            {
                return false;
            }

            return IsMonkWeapon(character, item);
        }
        
        public static bool IsMonkWeapon(RulesetCharacter character, RulesetItem weapon)
        {
            //fists
            if (weapon == null)
            {
                return true;
            }

            return IsMonkWeapon(character, weapon.ItemDefinition);
        }
        
        public static bool IsMonkWeapon(RulesetCharacter character, ItemDefinition weapon)
        {
            if (weapon == null)
            {
                return false;
            }
            var typeDefinition = weapon.WeaponDescription?.WeaponTypeDefinition;

            if (typeDefinition == null)
            {
                return false;
            }

            return MonkWeapons.Contains(typeDefinition)
                || ZenArcher.IsMonkWeapon(character, weapon);
        }

        public static bool UsingOnlyMonkWeapons(RulesetCharacter character)
        {
            var inventorySlotsByName = character.CharacterInventory.InventorySlotsByName;
            var mainHand = inventorySlotsByName[EquipmentDefinitions.SlotTypeMainHand].EquipedItem;
            var offHand = inventorySlotsByName[EquipmentDefinitions.SlotTypeOffHand].EquipedItem;

            return IsMonkWeapon(character, mainHand) && IsMonkWeapon(character, offHand);
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

            if (level >= 11)
            {
                return (DieType.D8, 1);
            }

            if (level >= 5)
            {
                return (DieType.D6, 1);
            }

            return (DieType.D4, 1);
        }

        private class MonkClimbing : ICustomOnActionFeature, ICharacterTurnStartListener, ICharacterTurnEndListener,
            IHeroRefreshedListener, ICharacterBattlStartedListener, ICharacterBattlEndedListener
        {
            private static readonly string CATEGORY = "MonkClimbing";
            private static readonly HashSet<string> Forbidden = new();

            private static readonly CharacterValidator[] validators =
            {
                CharacterValidators.NoArmor, CharacterValidators.NoShield
            };

            public void OnChracterBattleEnded(GameLocationCharacter locationCharacter)
            {
                AllowClimbing(locationCharacter.RulesetCharacter);
            }

            public void OnChracterBattleStarted(GameLocationCharacter locationCharacter, bool surprise)
            {
                ForbidClimbing(locationCharacter.RulesetCharacter);
            }

            public void OnChracterTurnEnded(GameLocationCharacter locationCharacter)
            {
                var character = locationCharacter.RulesetCharacter;
                ForbidClimbing(character);
                LoseClimbing(character);
            }

            public void OnChracterTurnStarted(GameLocationCharacter locationCharacter)
            {
                var character = locationCharacter.RulesetCharacter;
                AllowClimbing(character);
                TryBecomeClimbing(character);
            }

            public void OnBeforeAction(CharacterAction characterAction)
            {
                if (characterAction.ActionId != ActionDefinitions.Id.TacticalMove
                    && characterAction.ActionId != ActionDefinitions.Id.ExplorationMove)
                {
                    var character = characterAction.ActingCharacter.RulesetCharacter;
                    ForbidClimbing(character);
                    LoseClimbing(character);
                }
            }

            public void OnAfterAction(CharacterAction characterAction)
            {
                var character = characterAction.ActingCharacter.RulesetCharacter;
                AllowClimbing(character);
                TryBecomeClimbing(character);
            }

            public void OnHeroRefreshed(RulesetCharacter character)
            {
                TryBecomeClimbing(character);
            }

            private static bool Validate(RulesetCharacter character)
            {
                if (character == null)
                {
                    return false;
                }

                if (Forbidden.Contains(CharacterId(character))
                    || !character.IsValid(validators))
                {
                    LoseClimbing(character);
                    return false;
                }

                return true;
            }

            private static void TryBecomeClimbing(RulesetCharacter character)
            {
                if (!Validate(character))
                {
                    return;
                }

                if (character == null || character.HasConditionOfCategory(CATEGORY))
                {
                    return;
                }

                character.AddConditionOfCategory(CATEGORY, RulesetCondition.CreateActiveCondition(character.Guid,
                    MonkClimbingCondition, DurationType.Permanent,
                    1,
                    TurnOccurenceType.StartOfTurn,
                    character.Guid,
                    character.CurrentFaction.Name
                ));
            }

            private static void ForbidClimbing(RulesetCharacter character)
            {
                if (character != null)
                {
                    Forbidden.Add(CharacterId(character));
                }
            }

            private static void AllowClimbing(RulesetCharacter character)
            {
                if (character != null)
                {
                    Forbidden.Remove(CharacterId(character));
                }
            }

            private static string CharacterId(RulesetCharacter character)
            {
                return $"{character.Name}:{character.Guid}";
            }

            private static void LoseClimbing(RulesetCharacter character)
            {
                if (character != null && character.HasConditionOfCategory(CATEGORY))
                {
                    character.RemoveAllConditionsOfCategory(CATEGORY);
                }
            }
        }

        private class PerfectSelf : ICharacterBattlStartedListener
        {
            public void OnChracterBattleStarted(GameLocationCharacter locationCharacter, bool surprise)
            {
                var character = locationCharacter.RulesetCharacter;
                if (character == null)
                {
                    return;
                }

                if (character.GetRemainingPowerUses(KiPool) == 0)
                {
                    character.UpdateUsageForPower(KiPool, -4);
                    GameConsoleHelper.LogCharacterActivatesAbility(character, "Feature/&MonkPerfectSelfTitle");
                }
            }
        }

        private class ApplyMonkWeaponStatusOnAttack : IOnAttackEffect
        {
            public void BeforeOnAttack(GameLocationCharacter attacker, GameLocationCharacter defender,
                ActionModifier attackModifier,
                RulesetAttackMode attackerAttackMode)
            {
            }

            public void AfterOnAttack(GameLocationCharacter attacker, GameLocationCharacter defender,
                ActionModifier attackModifier,
                RulesetAttackMode attackerAttackMode)
            {
                var character = attacker.RulesetCharacter;
                if (character == null)
                {
                    return;
                }

                if (!IsMonkWeapon(character, attackerAttackMode))
                {
                    return;
                }

                character.AddConditionOfCategory(AttributeDefinitions.TagCombat,
                    RulesetCondition.CreateActiveCondition(character.Guid,
                        attackedWithMonkWeaponCondition, DurationType.Round,
                        1,
                        TurnOccurenceType.StartOfTurn,
                        character.Guid,
                        character.CurrentFaction.Name
                    ));
            }
        }
    }
}
