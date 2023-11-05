using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.CustomValidators;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Properties;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static SolastaUnfinishedBusiness.Builders.Features.AutoPreparedSpellsGroupBuilder;

namespace SolastaUnfinishedBusiness.Subclasses;

[UsedImplicitly]
public sealed class RangerSkyWarrior : AbstractSubclass
{
    private const string Name = "RangerSkyWarrior";

    private static ConditionDefinition _conditionGiftOfTheWind;

    public RangerSkyWarrior()
    {
        //
        // LEVEL 03
        //

        var autoPreparedSpells = FeatureDefinitionAutoPreparedSpellsBuilder
            .Create($"AutoPreparedSpells{Name}")
            .SetGuiPresentation(Category.Feature)
            .SetAutoTag("Ranger")
            .SetSpellcastingClass(CharacterClassDefinitions.Ranger)
            .SetPreparedSpellGroups(
                BuildSpellGroup(2, SpellsContext.AirBlast),
                BuildSpellGroup(5, SpellsContext.MirrorImage),
                BuildSpellGroup(9, Fly),
                BuildSpellGroup(13, PhantasmalKiller),
                BuildSpellGroup(17, MindTwist))
            .AddToDB();

        // Gift of The Wind

        var conditionGiftOfTheWindAttacked = ConditionDefinitionBuilder
            .Create($"Condition{Name}GiftOfTheWindAttacked")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionBaned)
            .SetPossessive()
            .SetSpecialDuration(DurationType.Round, 1, TurnOccurenceType.StartOfTurn)
            .SetSpecialInterruptions(ConditionInterruption.AnyBattleTurnEnd)
            .AddToDB();

        var additionalDamageGiftOfTheWind = FeatureDefinitionAdditionalDamageBuilder
            .Create($"AdditionalDamage{Name}GiftOfTheWind")
            .SetGuiPresentationNoContent(true)
            .SetRequiredProperty(RestrictedContextRequiredProperty.Weapon)
            .SetAttackModeOnly()
            .AddConditionOperation(ConditionOperationDescription.ConditionOperation.Add, conditionGiftOfTheWindAttacked)
            .AddToDB();

        var movementAffinityGiftOfTheWind = FeatureDefinitionMovementAffinityBuilder
            .Create($"MovementAffinity{Name}GiftOfTheWind")
            .SetGuiPresentation($"Condition{Name}GiftOfTheWindAttacked", Category.Condition, Gui.NoLocalization)
            .SetBaseSpeedAdditiveModifier(2)
            .AddToDB();

        var combatAffinityGiftOfTheWind = FeatureDefinitionCombatAffinityBuilder
            .Create($"CombatAffinity{Name}GiftOfTheWind")
            .SetGuiPresentation($"Condition{Name}GiftOfTheWindAttacked", Category.Condition, Gui.NoLocalization)
            .SetAttackOfOpportunityImmunity(true)
            .SetSituationalContext(SituationalContext.SourceHasCondition, conditionGiftOfTheWindAttacked)
            .AddToDB();

        _conditionGiftOfTheWind = ConditionDefinitionBuilder
            .Create($"Condition{Name}GiftOfTheWind")
            .SetGuiPresentation($"Condition{Name}GiftOfTheWindAttacked", Category.Condition,
                Gui.NoLocalization)
            .SetPossessive()
            .AddFeatures(movementAffinityGiftOfTheWind, combatAffinityGiftOfTheWind)
            .AddToDB();

        _conditionGiftOfTheWind.AddCustomSubFeatures(new CheckConditionValidity(_conditionGiftOfTheWind));

        var powerGiftOfTheWind = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}GiftOfTheWind")
            .SetGuiPresentation(Category.Feature, FeatureDefinitionPowers.PowerFighterSecondWind)
            .SetUsesFixed(ActivationTime.BonusAction)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 0, TurnOccurenceType.StartOfTurn)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetParticleEffectParameters(FeatureDefinitionPowers.PowerFighterSecondWind)
                    .SetEffectForms(EffectFormBuilder.ConditionForm(_conditionGiftOfTheWind))
                    .Build())
            .AddCustomSubFeatures(new ValidatorsValidatePowerUse(ValidatorsCharacter.HasShield))
            .AddToDB();

        // Aerial Agility

        var proficiencyAerialAgility = FeatureDefinitionProficiencyBuilder
            .Create($"Proficiency{Name}AerialAgility")
            .SetGuiPresentation(Category.Feature)
            .SetProficiencies(ProficiencyType.Skill, SkillDefinitions.Acrobatics)
            .AddToDB();

        //
        // LEVEL 07
        //

        // Swift Strike

        var attributeModifierSwiftStrike = FeatureDefinitionAttributeModifierBuilder
            .Create($"AttributeModifier{Name}SwiftStrike")
            .SetGuiPresentation(Category.Feature)
            .SetModifierAbilityScore(AttributeDefinitions.Initiative, AttributeDefinitions.Wisdom)
            .AddToDB();

        // Intangible Form

        var damageAffinityIntangibleForm = FeatureDefinitionDamageAffinityBuilder
            .Create($"DamageAffinity{Name}IntangibleForm")
            .SetGuiPresentation(Category.Feature)
            .SetDamageType(DamageTypeBludgeoning)
            .SetDamageAffinityType(DamageAffinityType.Resistance)
            .AddToDB();
        //
        // LEVEL 11
        //

        // Death from Above

        var featureDeathFromAbove = FeatureDefinitionBuilder
            .Create($"Feature{Name}DeathFromAbove")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        featureDeathFromAbove.AddCustomSubFeatures(
            new AttackBeforeHitConfirmedOnEnemyDeathFromAbove(featureDeathFromAbove, conditionGiftOfTheWindAttacked));

        //
        // LEVEL 15
        //

        // Cloud Dance

        var conditionCloudDance = ConditionDefinitionBuilder
            .Create(ConditionDefinitions.ConditionFlyingAdaptive, $"Condition{Name}CloudDance")
            .AddToDB();

        conditionCloudDance.AddCustomSubFeatures(new CheckConditionValidity(conditionCloudDance));

        var powerAngelicFormSprout = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}CloudDanceSprout")
            .SetGuiPresentation(Category.Feature,
                Sprites.GetSprite("FlightSprout", Resources.PowerAngelicFormSprout, 256, 128))
            .SetUsesFixed(ActivationTime.Action)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Permanent)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(conditionCloudDance, ConditionForm.ConditionOperation.Add)
                            .Build())
                    .Build())
            .AddCustomSubFeatures(new ValidatorsValidatePowerUse(
                ValidatorsCharacter.HasShield,
                ValidatorsCharacter.HasNoneOfConditions(conditionCloudDance.Name)))
            .AddToDB();

        var powerAngelicFormDismiss = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}CloudDanceDismiss")
            .SetGuiPresentation(Category.Feature,
                Sprites.GetSprite("FlightDismiss", Resources.PowerAngelicFormDismiss, 256, 128))
            .SetUsesFixed(ActivationTime.BonusAction)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetEffectForms(
                        EffectFormBuilder.ConditionForm(conditionCloudDance, ConditionForm.ConditionOperation.Remove))
                    .Build())
            .AddCustomSubFeatures(
                new ValidatorsValidatePowerUse(
                    ValidatorsCharacter.HasAnyOfConditions(conditionCloudDance.Name)))
            .AddToDB();

        var featureSetFairyFlight = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}CloudDance")
            .SetGuiPresentation($"Power{Name}CloudDanceSprout", Category.Feature)
            .AddFeatureSet(powerAngelicFormSprout, powerAngelicFormDismiss)
            .AddToDB();

        //
        // MAIN
        //

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Subclass, CharacterSubclassDefinitions.DomainBattle)
            .AddFeaturesAtLevel(3,
                autoPreparedSpells,
                additionalDamageGiftOfTheWind,
                powerGiftOfTheWind,
                proficiencyAerialAgility)
            .AddFeaturesAtLevel(7,
                attributeModifierSwiftStrike,
                damageAffinityIntangibleForm)
            .AddFeaturesAtLevel(11,
                featureDeathFromAbove)
            .AddFeaturesAtLevel(15,
                featureSetFairyFlight)
            .AddToDB();
    }

    internal override CharacterClassDefinition Klass => CharacterClassDefinitions.Ranger;

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceRangerArchetypes;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }

    private sealed class CheckConditionValidity : IOnItemEquipped
    {
        private readonly ConditionDefinition _condition;

        public CheckConditionValidity(ConditionDefinition condition)
        {
            _condition = condition;
        }

        public void OnItemEquipped(RulesetCharacterHero hero)
        {
            if (ValidatorsCharacter.HasShield(hero))
            {
                return;
            }

            var rulesetCondition = hero.AllConditions
                .FirstOrDefault(x => x.ConditionDefinition == _condition);

            if (rulesetCondition != null)
            {
                hero.RemoveCondition(rulesetCondition);
            }
        }
    }

    private sealed class AttackBeforeHitConfirmedOnEnemyDeathFromAbove : IAttackBeforeHitConfirmedOnEnemy
    {
        private readonly ConditionDefinition _conditionGiftOfTheWindAttacked;
        private readonly FeatureDefinition _featureDefinition;

        public AttackBeforeHitConfirmedOnEnemyDeathFromAbove(
            FeatureDefinition featureDefinition,
            ConditionDefinition conditionGiftOfTheWindAttacked)
        {
            _featureDefinition = featureDefinition;
            _conditionGiftOfTheWindAttacked = conditionGiftOfTheWindAttacked;
        }

        public IEnumerator OnAttackBeforeHitConfirmedOnEnemy(
            GameLocationBattleManager battle,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier attackModifier,
            RulesetAttackMode attackMode,
            bool rangedAttack,
            AdvantageType advantageType,
            List<EffectForm> actualEffectForms,
            RulesetEffect rulesetEffect,
            bool firstTarget,
            bool criticalHit)
        {
            if (battle is not { IsBattleInProgress: true })
            {
                yield break;
            }

            if (rulesetEffect != null)
            {
                yield break;
            }

            var rulesetAttacker = attacker.RulesetCharacter;

            if (rulesetAttacker is not { IsDeadOrDyingOrUnconscious: false })
            {
                yield break;
            }

            if (!rulesetAttacker.HasAnyConditionOfType(_conditionGiftOfTheWind.Name))
            {
                yield break;
            }

            var rolls = new List<int>();
            var bonusDamage = AttributeDefinitions.ComputeProficiencyBonus(
                rulesetAttacker.TryGetAttributeValue(AttributeDefinitions.CharacterLevel));

            rulesetAttacker.LogCharacterUsedFeature(_featureDefinition);

            foreach (var gameLocationDefender in battle.Battle.AllContenders
                         .Where(x => x.IsOppositeSide(attacker.Side) &&
                                     x.RulesetCharacter is { IsDeadOrDyingOrUnconscious: false } &&
                                     battle.IsWithin1Cell(attacker, x)))
            {
                var rulesetDefender = gameLocationDefender.RulesetCharacter;

                var damage = new DamageForm
                {
                    DamageType = DamageTypeBludgeoning,
                    DieType = DieType.D1,
                    DiceNumber = 0,
                    BonusDamage = bonusDamage
                };

                RulesetActor.InflictDamage(
                    bonusDamage,
                    damage,
                    damage.DamageType,
                    new RulesetImplementationDefinitions.ApplyFormsParams { targetCharacter = rulesetDefender },
                    rulesetDefender,
                    criticalHit,
                    attacker.Guid,
                    false,
                    attackMode.AttackTags,
                    new RollInfo(damage.DieType, rolls, bonusDamage),
                    false,
                    out _);

                if (!criticalHit)
                {
                    continue;
                }

                var rulesetCondition = rulesetDefender.AllConditions.FirstOrDefault(
                    x => x.ConditionDefinition == _conditionGiftOfTheWindAttacked);

                if (rulesetCondition == null)
                {
                    rulesetDefender.InflictCondition(
                        _conditionGiftOfTheWindAttacked.Name,
                        _conditionGiftOfTheWindAttacked.DurationType,
                        _conditionGiftOfTheWindAttacked.DurationParameter,
                        _conditionGiftOfTheWindAttacked.TurnOccurence,
                        AttributeDefinitions.TagCombat,
                        rulesetAttacker.guid,
                        rulesetAttacker.CurrentFaction.Name,
                        1,
                        _conditionGiftOfTheWindAttacked.Name,
                        0,
                        0,
                        0);
                }
            }
        }
    }
}
