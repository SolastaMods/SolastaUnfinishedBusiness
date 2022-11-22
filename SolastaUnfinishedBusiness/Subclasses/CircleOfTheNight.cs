using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterSubclassDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.MonsterDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.MonsterAttackDefinitions;
using static EffectForm;

namespace SolastaUnfinishedBusiness.Subclasses;

internal sealed class CircleOfTheNight : AbstractSubclass
{
    private const string CircleOfTheNightName = "CircleOfTheNight";


    // custom wildshapes

    /**
     * based on MM Cave Bear
     * */
    internal static MonsterDefinition HBWildShapeDireBear()
    {
        // attacks
        // Bite
        // TODO Bump damage mod from +4 to +5
        var biteAttack = new MonsterAttackIteration
        {
            monsterAttackDefinition = MonsterAttackDefinitionBuilder
                .Create(Attack_Wildshape_BrownBear_Bite, "Attack_Wildshape_DireBear_Bite")
                .SetToHitBonus(7)
                .AddToDB()
        };

        // Claw
        var clawAttack = new MonsterAttackIteration
        {
            monsterAttackDefinition = MonsterAttackDefinitionBuilder
            .Create(Attack_Wildshape_BrownBear_Claw, "Attack_Wildshape_DireBear_Claw")
            .SetToHitBonus(7)
            .AddToDB()
        };

        MonsterDefinition shape = MonsterDefinitionBuilder.Create(WildshapeBlackBear, "WildShapeDireBear")
            // STR, DEX, CON, INT, WIS, CHA
            .SetAbilityScores(new int[] { 20, 10, 16, 2, 13, 7 })
            .SetArmorClass(14)
            .SetStandardHitPoints(42)
            .SetHitDice(DieType.D10, 5)
            .SetChallengeRating(2)
            .SetOrUpdateGuiPresentation(Category.Monster, WildshapeBlackBear)
            .SetAttackIterations(new MonsterAttackIteration[] { biteAttack, clawAttack })
            .AddToDB();

        return shape;
    }

    internal static MonsterDefinition HBWildShapeAirElemental()
    {
        MonsterDefinition shape = MonsterDefinitionBuilder.Create(Air_Elemental, "WildShapeAirElemental")
            // STR, DEX, CON, INT, WIS, CHA
            .SetAbilityScores(new int[] { 14, 20, 14, 6, 10, 6 })
            .SetArmorClass(15)
            .SetStandardHitPoints(90)
            .SetHitDice(DieType.D10, 12)
            .AddToDB();

        return shape;
    }

    internal static MonsterDefinition HBWildShapeFireElemental()
    {
        MonsterDefinition shape = MonsterDefinitionBuilder.Create(Fire_Elemental, "WildShapeFireElemental")
            .AddToDB();

        return shape;
    }

    internal static MonsterDefinition HBWildShapeEarthElemental()
    {
        MonsterDefinition shape = MonsterDefinitionBuilder.Create(Earth_Elemental, "WildShapeEarthElemental")
            .AddToDB();

        return shape;
    }

    internal static MonsterDefinition HBWildShapeWaterElemental()
    {
        MonsterDefinition shape = MonsterDefinitionBuilder.Create(Ice_Elemental, "WildShapeWaterElemental")
            .AddToDB();

        return shape;
    }

    internal static ShapeOptionDescription ShapeBuilder(int level, MonsterDefinition monster)
    {
        var shape = new ShapeOptionDescription
        {
            requiredLevel = level,
            substituteMonster = monster
        };
        return shape;
    }

    internal static EffectDescription BuildCombatWildShapeEffectDescription()
    {
        var WildShapeEffect = EffectDescriptionBuilder
            .Create()
            .SetEffectAdvancement(EffectIncrementMethod.None)
            .Build();

        //WildShapeEffect.targetParameter = 1;
        WildShapeEffect.rangeType = RangeType.Self;
        WildShapeEffect.rangeParameter = 0;
        WildShapeEffect.halfDamageOnAMiss = false;
        //WildShapeEffect.hitAffinitiesByTargetTag = [];
        WildShapeEffect.targetType = TargetType.Self;
        //WildShapeEffect.itemSelectionType = ActionDefinitions.ItemSelectionType.Equiped;
        //WildShapeEffect.targetParameter = 1;
        //WildShapeEffect.targetParameter2 = 2;
        WildShapeEffect.emissiveBorder = RuleDefinitions.EmissiveBorder.None;
        //WildShapeEffect.emissiveParameter = 1;
        WildShapeEffect.requiresTargetProximity = false;
        //WildShapeEffect.targetProximityDistance = 6;
        WildShapeEffect.targetExcludeCaster = false;
        WildShapeEffect.canBePlacedOnCharacter = true;
        WildShapeEffect.affectOnlyGround = false;
        WildShapeEffect.targetFilteringMethod = RuleDefinitions.TargetFilteringMethod.AllCharacterAndGadgets;
        WildShapeEffect.targetFilteringTag = RuleDefinitions.TargetFilteringTag.No;
        //WildShapeEffect.requiresVisibilityForPosition = true;
        WildShapeEffect.inviteOptionalAlly = false;
        //WildShapeEffect.slotTypes = [];
        WildShapeEffect.recurrentEffect = RuleDefinitions.RecurrentEffect.No;
        WildShapeEffect.retargetAfterDeath = false;
        //WildShapeEffect.retargetActionType = ActionDefinitions.ActionType.Bonus;
        //WildShapeEffect.poolFilterDiceNumber = 5;
        //WildShapeEffect.poolFilterDieType = RuleDefinitions.DieType.D8;
        //WildShapeEffect.trapRangeType = Triggerer";
        //WildShapeEffect.targetConditionName = ";
        WildShapeEffect.targetConditionAsset = null;
        //WildShapeEffect.targetSide = Enemy;
        WildShapeEffect.durationType = RuleDefinitions.DurationType.HalfClassLevelHours;
        //WildShapeEffect.durationParameter = 1;
        //WildShapeEffect.endOfEffect = EndOfTurn;
        WildShapeEffect.hasSavingThrow = false;
        WildShapeEffect.disableSavingThrowOnAllies = false;
        //WildShapeEffect.savingThrowAbility = Dexterity";
        WildShapeEffect.ignoreCover = false;
        WildShapeEffect.grantedConditionOnSave = null;
        WildShapeEffect.rollSaveOnlyIfRelevantForms = false;
        WildShapeEffect.hasShoveRoll = false;
        WildShapeEffect.createdByCharacter = true;
        WildShapeEffect.difficultyClassComputation = RuleDefinitions.EffectDifficultyClassComputation.SpellCastingFeature;
        //WildShapeEffect.savingThrowDifficultyAbility = Wisdom;
        //WildShapeEffect.fixedSavingThrowDifficultyClass = 15;
        //WildShapeEffect.savingThrowAffinitiesBySense = [];
        //WildShapeEffect.savingThrowAffinitiesByFamily = [];
        WildShapeEffect.advantageForEnemies = false;
        WildShapeEffect.canBeDispersed = false;
        WildShapeEffect.hasVelocity = false;
        WildShapeEffect.velocityCellsPerRound = 2;
        WildShapeEffect.velocityType = RuleDefinitions.VelocityType.AwayFromSourceOriginalPosition;
        //WildShapeEffect.restrictedCreatureFamilies = [];
        //WildShapeEffect.immuneCreatureFamilies = [];
        //WildShapeEffect.restrictedCharacterSizes = [];
        WildShapeEffect.hasLimitedEffectPool = false;
        WildShapeEffect.effectPoolAmount = 60;
        WildShapeEffect.effectApplication = RuleDefinitions.EffectApplication.All;
        //WildShapeEffect.effectFormFilters = [];
        //WildShapeEffect.specialFormsDescription = "";
        WildShapeEffect.speedType = RuleDefinitions.SpeedType.Instant;
        WildShapeEffect.speedParameter = 10f;
        WildShapeEffect.offsetImpactTimeBasedOnDistance = false;
        //WildShapeEffect.offsetImpactTimeBasedOnDistanceFactor = 0.1f;
        WildShapeEffect.offsetImpactTimePerTarget = 0.0f;
        WildShapeEffect.animationMagicEffect = AnimationDefinitions.AnimationMagicEffect.Animation0;
        WildShapeEffect.lightCounterDispellsEffect = false;
        WildShapeEffect.effectAIParameters = PowerDruidWildShape.effectDescription.effectAIParameters;
        WildShapeEffect.effectParticleParameters = PowerDruidWildShape.effectDescription.effectParticleParameters;

        var effectForm1 = new EffectForm
        {
            formType = EffectFormType.ShapeChange,
            addBonusMode = RuleDefinitions.AddBonusMode.None,
            applyLevel = LevelApplianceType.No,
            levelType = RuleDefinitions.LevelSourceType.ClassLevel,
            levelMultiplier = 0,
            createdByCharacter = true,
            createdByCondition = false,
            hasSavingThrow = false,
            savingThrowAffinity = RuleDefinitions.EffectSavingThrowType.None,
            dcModifier = 0,
            canSaveToCancel = false,
            saveOccurence = RuleDefinitions.TurnOccurenceType.StartOfTurn,
            hasFilterId = false,
            filterId = 0,
            shapeChangeForm = new ShapeChangeForm
            {
                shapeChangeType = ShapeChangeForm.Type.ClassLevelListSelection,
                keepMentalAbilityScores = true,
                //specialSubstituteCondition = PowerDruidWildShape.effectDescription.effectForms[0].shapeChangeForm.specialSubstituteCondition,
                specialSubstituteCondition = ConditionDefinitions.ConditionWildShapeSubstituteForm
            }
        };

        // doesn't make much sense to have weaker forms of the shapes such as wolf and blackbear
        //effectForm1.shapeChangeForm.shapeOptions.Add(ShapeBuilder(2, WildShapeWolf));
        //effectForm1.shapeChangeForm.shapeOptions.Add(ShapeBuilder(2, WildShapeBrownBear));
        effectForm1.shapeChangeForm.shapeOptions.Add(ShapeBuilder(2, WildShapeBadlandsSpider));
        effectForm1.shapeChangeForm.shapeOptions.Add(ShapeBuilder(2, WildshapeDirewolf));
        effectForm1.shapeChangeForm.shapeOptions.Add(ShapeBuilder(2, WildShapeBrownBear));
        effectForm1.shapeChangeForm.shapeOptions.Add(ShapeBuilder(4, WildshapeDeepSpider));
        effectForm1.shapeChangeForm.shapeOptions.Add(ShapeBuilder(4, HBWildShapeDireBear())); // Homebrewed Cave Bear similar
        effectForm1.shapeChangeForm.shapeOptions.Add(ShapeBuilder(6, WildShapeApe)); // CR3 but nerfed, so we make it level 6
        effectForm1.shapeChangeForm.shapeOptions.Add(ShapeBuilder(8, WildshapeTiger_Drake)); // flying
        effectForm1.shapeChangeForm.shapeOptions.Add(ShapeBuilder(8, WildShapeGiant_Eagle)); // flying
        //effectForm1.shapeChangeForm.shapeOptions.Add(ShapeBuilder(10, WildShapeTundraTiger)); // CR 4, but nerfed so level 10

        // Elementals
        // According to the rules, transforming into an elemental should cost 2 Wild Shape Charges
        // However elementals in this game are nerfed, since they don't have special attacks, such as Whirlwind
        // TODO Create a new feature for elemental transformation.
        // TODO Add special attacks to elemental forms (whirlwind, Whelm, Earth Glide maybe)
        effectForm1.shapeChangeForm.shapeOptions.Add(ShapeBuilder(10, HBWildShapeAirElemental()));
        effectForm1.shapeChangeForm.shapeOptions.Add(ShapeBuilder(10, HBWildShapeFireElemental()));
        effectForm1.shapeChangeForm.shapeOptions.Add(ShapeBuilder(10, HBWildShapeWaterElemental()));
        effectForm1.shapeChangeForm.shapeOptions.Add(ShapeBuilder(10, HBWildShapeEarthElemental()));

        WildShapeEffect.effectForms.Add(effectForm1);

        return WildShapeEffect;
    }


    internal static EffectDescription CombatHealing(int diceNumber = 1, DieType dieType = DieType.D8, int bonusHealing = 0)
    {
        EffectForm healingForm = EffectFormBuilder.Create()
            .SetHealingForm(
                HealingComputation.Dice,
                bonusHealing,
                dieType,
                diceNumber,
                false,
                HealingCap.MaximumHitPoints)
            .Build();


        EffectDescription effectDescription = EffectDescriptionBuilder.Create()
            .SetRequiredCondition(ConditionDefinitions.ConditionWildShapeSubstituteForm)
            .SetDurationData(DurationType.Instantaneous)
            .SetEffectForms(healingForm)
            .SetTargetingData(Side.Ally, RangeType.Self, 1, TargetType.Self)
            .Build();

        return effectDescription;
    }

    internal CircleOfTheNight()
    {
        // 3rd level
        // Combat Wildshape 
        // Official dnd 5e rules are CR = 1/3 of druid level. However in solasta the selection of beasts is greatly reduced
        var combatWildshape = FeatureDefinitionPowerSharedPoolBuilder
            .Create("PowerDruidCombatWildShape")
            .SetOverriddenPower(PowerDruidWildShape)
            .SetGuiPresentation(Category.Feature, hidden: true)
            .SetSharedPool(ActivationTime.BonusAction, PowerDruidWildShape)
            .SetEffectDescription(BuildCombatWildShapeEffectDescription())
            .AddToDB();

        // Combat Wild Shape Healing
        // While wildshaped, you can use a bonus action to heal yourself for 1d8 hit points.
        // You can use this feature a number of times equal to your Wisdom modifier per long rest
        var combatHealing = FeatureDefinitionPowerBuilder
            .Create("PowerDruidCombatWildShapeHealing")
            .SetGuiPresentation(Category.Feature, PowerPaladinCureDisease)
            //.SetUsesFixed(ActivationTime.BonusAction, RechargeRate.LongRest, 1, 2) // manual proficiency
            .SetUsesProficiencyBonus(ActivationTime.BonusAction)
            .SetEffectDescription(CombatHealing())
            .AddToDB();


        // 6th Level
        // Primal Strike
        // Starting at 6th level, your attacks in beast form count as magical for the purpose of overcoming resistance
        // and immunity to nonmagical attacks and damage.
        // NOTE: (BUG)This also affects attacks with regular weapons
        var primalStrike = FeatureDefinitionAttackModifierBuilder
            .Create("PowerDruidPrimalStrike")
            .SetGuiPresentation(Category.Feature)
            .SetMagicalWeapon()
            //.SetRequiredProperty(RestrictedContextRequiredProperty.Unarmed)
            .AddToDB();


        // Improved Combat Healing
        // At 6th level, your combat healing improves to 2d8 + 2
        var improvedCombatHealing = FeatureDefinitionPowerBuilder
            .Create("PowerDruidImprovedCombatWildShapeHealing")
            .SetGuiPresentation(Category.Feature, PowerPaladinCureDisease)
            .SetUsesProficiencyBonus(ActivationTime.BonusAction)
            .SetEffectDescription(CombatHealing(2))
            .SetOverriddenPower(combatHealing)
            .AddToDB();


        // 10th Level
        // Superior Combat Healing
        // At 10th level, your combat healing improves to 3d8 + 6
        var superiorCombatHealing = FeatureDefinitionPowerBuilder
            .Create("PowerDruidSuperiorCombatWildShapeHealing")
            .SetGuiPresentation(Category.Feature, PowerPaladinCureDisease)
            .SetEffectDescription(CombatHealing(3, DieType.D8, 6))
            .SetUsesProficiencyBonus(ActivationTime.BonusAction)
            .SetOverriddenPower(improvedCombatHealing)
            .AddToDB();


        Subclass = CharacterSubclassDefinitionBuilder
            .Create(CircleOfTheNightName)
            .SetGuiPresentation(Category.Subclass, PathClaw)
            .AddFeaturesAtLevel(2,
                combatWildshape,
                combatHealing)
            .AddFeaturesAtLevel(6,
                primalStrike,
                improvedCombatHealing)
            .AddFeaturesAtLevel(10,
                superiorCombatHealing)
            .AddToDB();
    }

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceDruidCircle;
}
