using System.Collections.Generic;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Properties;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.MonsterDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.MonsterAttackDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionDamageAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionConditionAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionSenses;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionMoveModes;

namespace SolastaUnfinishedBusiness.Subclasses;

internal sealed class CircleOfTheNight : AbstractSubclass
{
    private const string CircleOfTheNightName = "CircleOfTheNight";

    internal CircleOfTheNight()
    {
        // 2nd level

        #region Unused definitions kept to not break existing characters

        ConditionDefinitionBuilder
            .Create("ConditionCircleOfTheNightWildshape")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .AddToDB();

        ConditionDefinitionBuilder
            .Create("ConditionCircleOfTheNightWildshapeAny")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .AddToDB();

        #endregion

        var combatWildshape = BuildWildShapePower();

        //remove regular WS action
        var blockRegularWildshape = FeatureDefinitionActionAffinityBuilder
            .Create("OnAfterActionWildShape")
            .SetGuiPresentationNoContent(true)
            .SetForbiddenActions(ActionDefinitions.Id.WildShape)
            .AddToDB();

        // Combat Wild Shape Healing
        // While wild shaped, you can use a bonus action to heal yourself for 1d8 hit points.
        // You can use this feature a number of times equal to your Proficiency Modifier per form per long rest
        var powerCircleOfTheNightWildShapeHealing = FeatureDefinitionPowerBuilder
            .Create("PowerCircleOfTheNightWildShapeHealing")
            .SetGuiPresentation(Category.Feature, PowerPaladinCureDisease)
            .SetUsesProficiencyBonus(ActivationTime.BonusAction)
            .SetEffectDescription(CombatHealing())
            .SetCustomSubFeatures(CanUseCombatHealing())
            .AddToDB();

        // 6th Level

        // Primal Strike
        // Starting at 6th level, your attacks in beast form count as magical for the purpose of overcoming resistance
        // and immunity to non magical attacks and damage.
        // NOTE: (BUG)This also affects attacks with regular weapons
        var powerCircleOfTheNightPrimalStrike = FeatureDefinitionAttackModifierBuilder
            .Create("PowerCircleOfTheNightPrimalStrike")
            .SetGuiPresentation(Category.Feature)
            .SetMagicalWeapon()
            //.SetRequiredProperty(RestrictedContextRequiredProperty.Unarmed)
            .AddToDB();

        // Improved Combat Healing
        // At 6th level, your combat healing improves to 2d8 + 2
        var powerCircleOfTheNightWildShapeImprovedHealing = FeatureDefinitionPowerBuilder
            .Create("PowerCircleOfTheNightWildShapeImprovedHealing")
            .SetGuiPresentation(Category.Feature, PowerPaladinCureDisease)
            .SetUsesProficiencyBonus(ActivationTime.BonusAction)
            .SetEffectDescription(CombatHealing(2))
            .SetCustomSubFeatures(CanUseCombatHealing())
            .SetOverriddenPower(powerCircleOfTheNightWildShapeHealing)
            .AddToDB();

        // 10th Level

        // Superior Combat Healing
        // At 10th level, your combat healing improves to 3d8 + 6
        var powerCircleOfTheNightWildShapeSuperiorHealing = FeatureDefinitionPowerBuilder
            .Create("PowerCircleOfTheNightWildShapeSuperiorHealing")
            .SetGuiPresentation(Category.Feature, PowerPaladinCureDisease)
            .SetUsesProficiencyBonus(ActivationTime.BonusAction)
            .SetEffectDescription(CombatHealing(3, DieType.D8, 6))
            .SetCustomSubFeatures(CanUseCombatHealing())
            .SetOverriddenPower(powerCircleOfTheNightWildShapeImprovedHealing)
            .AddToDB();

        // Elemental Forms
        var featureSetCircleOfTheNightElementalForms = FeatureDefinitionFeatureSetBuilder
            .Create("FeatureSetCircleOfTheNightElementalForms")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(CircleOfTheNightName)
            .SetGuiPresentation(Category.Subclass,
                Sprites.GetSprite("CircleOfTheNight", Resources.CircleOfTheNight, 256))
            .AddFeaturesAtLevel(2,
                combatWildshape,
                blockRegularWildshape,
                powerCircleOfTheNightWildShapeHealing)
            .AddFeaturesAtLevel(6,
                powerCircleOfTheNightPrimalStrike,
                powerCircleOfTheNightWildShapeImprovedHealing)
            .AddFeaturesAtLevel(10,
                featureSetCircleOfTheNightElementalForms,
                powerCircleOfTheNightWildShapeSuperiorHealing)
            .AddToDB();
    }

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceDruidCircle;

    internal override DeityDefinition DeityDefinition => null;

    private static FeatureDefinitionPower BuildWildShapePower()
    {
        if (!TryGetDefinition<ActionDefinition>("WildShape", out var baseAction))
        {
            Main.Error("Couldn't fine WildShape action!");
            return null;
        }

        // Official rules are CR = 1/3 of druid level. However in solasta the selection of beasts is greatly reduced
        var shapeOptions = new List<ShapeOptionDescription>
        {
            ShapeBuilder(2, WildShapeBadlandsSpider),
            ShapeBuilder(2, WildshapeDirewolf),
            ShapeBuilder(2, WildShapeBrownBear),
            ShapeBuilder(4, WildshapeDeepSpider),
            ShapeBuilder(4, HbWildShapeDireBear()),
            ShapeBuilder(6, WildShapeApe),
            // flying
            ShapeBuilder(8, WildshapeTiger_Drake),
            ShapeBuilder(8, WildShapeGiant_Eagle),
            // don't use future features
            // ShapeBuilder(10, WildShapeTundraTiger),
            // elementals
            // According to the rules, transforming into an elemental should cost 2 Wild Shape Charges
            // However elementals in this game are nerfed, since they don't have special attacks, such as Whirlwind
            //TODO: Create a new feature for elemental transformation.
            //TODO: Add special attacks to elemental forms (whirlwind, Whelm, Earth Glide maybe)
            ShapeBuilder(10, HbWildShapeAirElemental()),
            ShapeBuilder(10, HbWildShapeFireElemental()),
            ShapeBuilder(10, HbWildShapeEarthElemental()),
            ShapeBuilder(10, HbWildShapeWaterElemental())
        };

        const string NAME = "PowerCircleOfTheNightWildShapeCombat";
        var power = FeatureDefinitionPowerSharedPoolBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Feature)
            .SetOverriddenPower(PowerDruidWildShape)
            .SetSharedPool(ActivationTime.BonusAction, PowerDruidWildShape)
            .DelegatedToAction()
            .SetEffectDescription(EffectDescriptionBuilder.Create()
                .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                .SetDurationData(DurationType.HalfClassLevelHours)
                .SetParticleEffectParameters(PowerDruidWildShape)
                .SetEffectForms(EffectFormBuilder.Create()
                    .SetShapeChangeForm(ShapeChangeForm.Type.ClassLevelListSelection, true,
                        ConditionDefinitions.ConditionWildShapeSubstituteForm, shapeOptions)
                    .Build())
                .Build())
            .AddToDB();

        ActionDefinitionBuilder
            .Create(baseAction, "CombatWildShape")
            .SetGuiPresentation(NAME, Category.Feature, baseAction, baseAction.GuiPresentation.SortOrder)
            .OverrideClassName("WildShape")
            .SetActionId(ExtraActionId.CombatWildShape)
            .SetActionType(ActionDefinitions.ActionType.Bonus)
            .SetActivatedPower(power)
            .AddToDB();

        return power;
    }

    // custom wild shapes

    /**
     * based on MM Cave Bear
     * */
    private static MonsterDefinition HbWildShapeDireBear()
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

        var shape = MonsterDefinitionBuilder.Create(WildshapeBlackBear, "WildShapeDireBear")
            // STR, DEX, CON, INT, WIS, CHA
            .SetAbilityScores(20, 10, 16, 2, 13, 7)
            .SetArmorClass(14)
            .SetCreatureTags(TagsDefinitions.CreatureTagWildShape)
            .SetStandardHitPoints(42)
            .SetHitDice(DieType.D10, 5)
            .SetChallengeRating(2)
            .SetOrUpdateGuiPresentation(Category.Monster, WildshapeBlackBear)
            .SetAttackIterations(biteAttack, clawAttack)
            .AddToDB();

        return shape;
    }

    private static MonsterDefinition HbWildShapeAirElemental()
    {
        var shape = MonsterDefinitionBuilder.Create(Air_Elemental, "WildShapeAirElemental")
            // STR, DEX, CON, INT, WIS, CHA
            .SetAbilityScores(14, 20, 14, 6, 10, 6)
            .SetArmorClass(15)
            .SetCreatureTags(TagsDefinitions.CreatureTagWildShape)
            .SetStandardHitPoints(90)
            .SetHitDice(DieType.D10, 12)
            .AddToDB();

        return shape;
    }

    private static MonsterDefinition HbWildShapeFireElemental()
    {
        var shape = MonsterDefinitionBuilder
            .Create(Fire_Elemental, "WildShapeFireElemental")
            .SetCreatureTags(TagsDefinitions.CreatureTagWildShape)
            .AddToDB();

        return shape;
    }

    private static MonsterDefinition HbWildShapeEarthElemental()
    {
        var shape = MonsterDefinitionBuilder
            .Create(Earth_Elemental, "WildShapeEarthElemental")
            .SetCreatureTags(TagsDefinitions.CreatureTagWildShape)
            .AddToDB();

        return shape;
    }

    private static MonsterDefinition HbWildShapeWaterElemental()
    {
        // TODO Create Whelm attack (recharge 5/6)
        // Whelm(Recharge 4–6).Each creature in the elemental space must make a DC 15 Strength saving throw.
        // On a failure, a target takes 13 (2d8 + 4) bludgeoning damage. If it is Large or smaller,
        // it is also grappled (escape DC 14). Until this grapple ends, the target is restrained and
        // unable to breathe unless it can breathe water. If the saving throw is successful, the target
        // is pushed out of the elemental space.

        // TODO FUTURE: when IceElemental is implemented in Base Game, replace Air_Elemental with Ice_Elemental
        var shape = MonsterDefinitionBuilder
            .Create(Air_Elemental, "WildShapeWaterElemental")
            .SetAbilityScores(18, 14, 18, 5, 10, 8)
            .SetArmorClass(14)
            .SetCreatureTags(TagsDefinitions.CreatureTagWildShape)
            .SetHitDice(DieType.D10, 12)
            .SetHitPointsBonus(48)
            .SetStandardHitPoints(114)
            .SetFeatures(
                DamageAffinityAcidResistance,
                DamageAffinityBludgeoningResistance,
                DamageAffinityPiercingResistance,
                DamageAffinitySlashingResistance,
                DamageAffinityFireImmunity,
                DamageAffinityPoisonImmunity,
                ConditionAffinityExhaustionImmunity,
                ConditionAffinityGrappledImmunity,
                ConditionAffinityParalyzedmmunity,
                ConditionAffinityPetrifiedImmunity,
                ConditionAffinityPoisonImmunity,
                ConditionAffinityProneImmunity,
                ConditionAffinityRestrainedmmunity,
                ConditionAffinityUnconsciousImmunity,
                SenseNormalVision,
                SenseDarkvision,
                MoveModeMove10,
                MoveModeFly6
            )
            .SetOrUpdateGuiPresentation(Category.Monster, Air_Elemental)
            .AddToDB();

        return shape;
    }

    private static ShapeOptionDescription ShapeBuilder(int level, MonsterDefinition monster)
    {
        var shape = new ShapeOptionDescription { requiredLevel = level, substituteMonster = monster };

        return shape;
    }

    private static EffectDescription CombatHealing(
        int diceNumber = 1,
        DieType dieType = DieType.D8,
        int bonusHealing = 0)
    {
        var healingForm = EffectFormBuilder.Create()
            .SetHealingForm(
                HealingComputation.Dice,
                bonusHealing,
                dieType,
                diceNumber,
                false,
                HealingCap.MaximumHitPoints)
            .Build();

        var effectDescription = EffectDescriptionBuilder.Create()
            .SetRequiredCondition(ConditionDefinitions.ConditionWildShapeSubstituteForm)
            .SetDurationData(DurationType.Instantaneous)
            .SetEffectForms(healingForm)
            .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
            .Build();

        return effectDescription;
    }

    private static ValidatorsPowerUse CanUseCombatHealing()
    {
        return new ValidatorsPowerUse(
            ValidatorsCharacter.HasAnyOfConditions(ConditionDefinitions.ConditionWildShapeSubstituteForm.name));
    }
}
