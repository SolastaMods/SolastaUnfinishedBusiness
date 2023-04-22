using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.CustomValidators;
using SolastaUnfinishedBusiness.Properties;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;

namespace SolastaUnfinishedBusiness.Subclasses;

internal sealed class WayOfTheTempest : AbstractSubclass
{
    private const string Name = "WayOfTheTempest";

    internal WayOfTheTempest()
    {
        // LEVEL 03

        // Tempest's Swiftness

        var movementAffinityTempestSwiftness = FeatureDefinitionMovementAffinityBuilder
            .Create($"MovementAffinity{Name}TempestSwiftness")
            .SetGuiPresentation(Category.Feature)
            .SetBaseSpeedAdditiveModifier(2)
            .SetCustomSubFeatures(new OnAfterActionTempestSwiftness())
            .AddToDB();

        // LEVEL 06

        // Gathering Storm

        var combatAffinityGatheringStorm = FeatureDefinitionCombatAffinityBuilder
            .Create($"CombatAffinity{Name}GatheringStorm")
            .SetGuiPresentation($"Condition{Name}AppliedGatheringStorm", Category.Condition)
            .SetMyAttackAdvantage(AdvantageType.Advantage)
            .AddToDB();

        var conditionAppliedGatheringStorm = ConditionDefinitionBuilder
            .Create($"Condition{Name}AppliedGatheringStorm")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionLightSensitiveSorakSaboteur)
            .SetPossessive()
            .SetSilent(Silent.WhenRemoved)
            .SetSpecialDuration(DurationType.Round, 1)
            .CopyParticleReferences(ConditionDefinitions.ConditionBranded)
            .SetFeatures(combatAffinityGatheringStorm)
            .AddToDB();

        var conditionGatheringStorm = ConditionDefinitionBuilder
            .Create($"Condition{Name}GatheringStorm")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionLightSensitiveSorakSaboteur)
            .SetPossessive()
            .SetSilent(Silent.WhenRemoved)
            .SetSpecialDuration(DurationType.Round, 0, TurnOccurenceType.StartOfTurn)
            .CopyParticleReferences(ConditionDefinitions.ConditionGuided)
            .SetCancellingConditions(conditionAppliedGatheringStorm)
            .SetSpecialInterruptions(ConditionInterruption.AnyBattleTurnEnd)
            .AddToDB();

        var featureGatheringStorm = FeatureDefinitionBuilder
            .Create($"Feature{Name}GatheringStorm")
            .SetGuiPresentation(Category.Feature)
            .SetCustomSubFeatures(
                new CustomBehaviorGatheringStorm(
                    conditionGatheringStorm,
                    conditionAppliedGatheringStorm))
            .AddToDB();

        // LEVEL 11

        // Tempest’s Fury

        var powerTempestFury = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}TempestFury")
            .SetGuiPresentation(Category.Feature,
                Sprites.GetSprite("TempestFury", Resources.PowerTempestFury, 256, 128))
            .SetUsesFixed(ActivationTime.NoCost, RechargeRate.KiPoints, 2)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetDurationData(DurationType.Round)
                    .SetParticleEffectParameters(PowerMonkFlurryOfBlows)
                    .AddEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(
                                ConditionDefinitions.ConditionMonkFlurryOfBlowsUnarmedStrikeBonus,
                                ConditionForm.ConditionOperation.Add, true)
                            .Build(),
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(
                                ConditionDefinitions.ConditionDisengaging,
                                ConditionForm.ConditionOperation.Add, true)
                            .Build())
                    .Build())
            .AddToDB();

        powerTempestFury.SetCustomSubFeatures(
            ValidatorsPowerUse.InCombat,
            new PerformAttackAfterMagicEffectUseTempestFury(),
            new ValidatorsPowerUse(ValidatorsCharacter.HasAttacked),
            new ValidatorsPowerUse(ValidatorsCharacter.HasNoneOfConditions(ConditionFlurryOfBlows)));

        // LEVEL 17

        // Eye of The Storm

        // Mark

        var conditionEyeOfTheStorm = ConditionDefinitionBuilder
            .Create($"Condition{Name}EyeOfTheStorm")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionShocked)
            .SetPossessive()
            .SetConditionType(ConditionType.Detrimental)
            .CopyParticleReferences(ConditionDefinitions.ConditionShocked)
            .SetSpecialDuration(DurationType.Minute, 1, TurnOccurenceType.EndOfSourceTurn)
            .AddToDB();

        var additionalDamageEyeOfTheStorm = FeatureDefinitionAdditionalDamageBuilder
            .Create($"AdditionalDamage{Name}EyeOfTheStorm")
            .SetGuiPresentationNoContent(true)
            .SetRequiredProperty(RestrictedContextRequiredProperty.UnarmedOrMonkWeapon)
            .SetImpactParticleReference(ConditionDefinitions.ConditionShocked.conditionParticleReference)
            .SetConditionOperations(new ConditionOperationDescription
            {
                ConditionDefinition = conditionEyeOfTheStorm,
                Operation = ConditionOperationDescription.ConditionOperation.Add
            })
            .AddToDB();

        // Staggered

        var abilityCheckAffinityEyeOfTheStorm = FeatureDefinitionAbilityCheckAffinityBuilder
            .Create($"AbilityCheckAffinity{Name}AppliedEyeOfTheStorm")
            .SetGuiPresentation($"Condition{Name}AppliedEyeOfTheStorm", Category.Condition)
            .BuildAndSetAffinityGroups(CharacterAbilityCheckAffinity.Disadvantage,
                AttributeDefinitions.Strength,
                AttributeDefinitions.Dexterity,
                AttributeDefinitions.Constitution,
                AttributeDefinitions.Intelligence,
                AttributeDefinitions.Wisdom,
                AttributeDefinitions.Charisma)
            .AddToDB();

        var combatAffinityEyeOfTheStorm = FeatureDefinitionCombatAffinityBuilder
            .Create($"CombatAffinity{Name}AppliedEyeOfTheStorm")
            .SetGuiPresentation($"Condition{Name}AppliedEyeOfTheStorm", Category.Condition)
            .SetMyAttackAdvantage(AdvantageType.Disadvantage)
            .AddToDB();

        var conditionAppliedEyeOfTheStorm = ConditionDefinitionBuilder
            .Create($"Condition{Name}AppliedEyeOfTheStorm")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionMarkedByHunter)
            .SetPossessive()
            .SetConditionType(ConditionType.Detrimental)
            .CopyParticleReferences(ConditionDefinitions.ConditionMarkedByHunter)
            .AddFeatures(abilityCheckAffinityEyeOfTheStorm, combatAffinityEyeOfTheStorm)
            .AddToDB();

        // Powers

        var powerEyeOfTheStormLeap = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}EyeOfTheStormLeap")
            .SetGuiPresentation($"FeatureSet{Name}EyeOfTheStorm", Category.Feature, hidden: true)
            .SetUsesFixed(ActivationTime.NoCost)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 12, TargetType.IndividualsUnique)
                    .SetDurationData(DurationType.Round, 1, TurnOccurenceType.EndOfSourceTurn)
                    .SetSavingThrowData(false, AttributeDefinitions.Dexterity, true,
                        EffectDifficultyClassComputation.AbilityScoreAndProficiency)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetDamageForm(DamageTypeThunder, 3, DieType.D10)
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .Build(),
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(conditionAppliedEyeOfTheStorm, ConditionForm.ConditionOperation.Add)
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .Build(),
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(conditionEyeOfTheStorm, ConditionForm.ConditionOperation.Remove)
                            .Build())
                    .Build())
            .SetCustomSubFeatures(ValidatorsPowerUse.InCombat)
            .AddToDB();

        var powerEyeOfTheStorm = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}EyeOfTheStorm")
            .SetGuiPresentation($"FeatureSet{Name}EyeOfTheStorm", Category.Feature, PowerOathOfDevotionTurnUnholy)
            .SetUsesFixed(ActivationTime.Action, RechargeRate.KiPoints, 3)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetParticleEffectParameters(SpellDefinitions.ShockingGrasp)
                    .SetDurationData(DurationType.Instantaneous)
                    .Build())
            .AddToDB();

        powerEyeOfTheStorm.SetCustomSubFeatures(
            ValidatorsPowerUse.InCombat,
            new OnAfterActionEyeOfTheStorm(
                powerEyeOfTheStorm, powerEyeOfTheStormLeap, conditionEyeOfTheStorm));

        var featureSetEyeOfTheStorm = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}EyeOfTheStorm")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(additionalDamageEyeOfTheStorm, powerEyeOfTheStorm, powerEyeOfTheStormLeap)
            .AddToDB();

        //
        // MAIN
        //

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Subclass, Sprites.GetSprite(Name, Resources.WayOfTheTempest, 256))
            .AddFeaturesAtLevel(3, movementAffinityTempestSwiftness)
            .AddFeaturesAtLevel(6, featureGatheringStorm)
            .AddFeaturesAtLevel(11, powerTempestFury)
            .AddFeaturesAtLevel(17, featureSetEyeOfTheStorm)
            .AddToDB();
    }

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceMonkMonasticTraditions;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }

    //
    // Gathering Storm
    //

    private sealed class CustomBehaviorGatheringStorm : ICharacterTurnStartListener, IAfterAttackEffect
    {
        private readonly ConditionDefinition _conditionAppliedGatheringStorm;
        private readonly ConditionDefinition _conditionGatheringStorm;

        public CustomBehaviorGatheringStorm(
            ConditionDefinition conditionGatheringStorm,
            ConditionDefinition conditionAppliedGatheringStorm)
        {
            _conditionGatheringStorm = conditionGatheringStorm;
            _conditionAppliedGatheringStorm = conditionAppliedGatheringStorm;
        }

        public void AfterOnAttackHit(
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RollOutcome outcome,
            CharacterActionParams actionParams,
            RulesetAttackMode attackMode,
            ActionModifier attackModifier)
        {
            var rulesetCharacter = attacker.RulesetCharacter;

            if (rulesetCharacter == null || rulesetCharacter.IsDeadOrDyingOrUnconscious)
            {
                return;
            }

            if (outcome is RollOutcome.Failure or RollOutcome.CriticalFailure)
            {
                attacker.RulesetCharacter.RemoveAllConditionsOfCategoryAndType(AttributeDefinitions.TagCombat,
                    _conditionGatheringStorm.Name);

                return;
            }

            if (rulesetCharacter.HasAnyConditionOfType(_conditionAppliedGatheringStorm.Name))
            {
                attacker.RulesetCharacter.RemoveAllConditionsOfCategoryAndType(AttributeDefinitions.TagCombat,
                    _conditionAppliedGatheringStorm.Name);

                return;
            }

            if (!rulesetCharacter.HasAnyConditionOfType(_conditionGatheringStorm.Name))
            {
                var rulesetCondition = RulesetCondition.CreateActiveCondition(
                    rulesetCharacter.guid,
                    _conditionGatheringStorm,
                    _conditionGatheringStorm.DurationType,
                    _conditionGatheringStorm.DurationParameter,
                    _conditionGatheringStorm.TurnOccurence,
                    rulesetCharacter.guid,
                    rulesetCharacter.CurrentFaction.Name);

                rulesetCharacter.AddConditionOfCategory(AttributeDefinitions.TagCombat, rulesetCondition);

                return;
            }

            var rulesetConditionApplied = RulesetCondition.CreateActiveCondition(
                rulesetCharacter.guid,
                _conditionAppliedGatheringStorm,
                _conditionAppliedGatheringStorm.DurationType,
                _conditionAppliedGatheringStorm.DurationParameter,
                _conditionAppliedGatheringStorm.TurnOccurence,
                rulesetCharacter.guid,
                rulesetCharacter.CurrentFaction.Name);

            rulesetCharacter.AddConditionOfCategory(AttributeDefinitions.TagCombat, rulesetConditionApplied);
        }

        public void OnCharacterTurnStarted(GameLocationCharacter locationCharacter)
        {
            var condition = ConditionDefinitions.ConditionMonkMartialArtsUnarmedStrikeBonus;
            var rulesetCharacter = locationCharacter.RulesetCharacter;
            var rulesetCondition = RulesetCondition.CreateActiveCondition(
                rulesetCharacter.guid,
                condition,
                DurationType.Round,
                0,
                TurnOccurenceType.StartOfTurn,
                rulesetCharacter.guid,
                rulesetCharacter.CurrentFaction.Name);

            rulesetCharacter.AddConditionOfCategory(AttributeDefinitions.TagCombat, rulesetCondition);
        }
    }

    //
    // Tempest Swiftness
    //

    private sealed class OnAfterActionTempestSwiftness : IOnAfterActionFeature
    {
        public void OnAfterAction(CharacterAction action)
        {
            if (action is not CharacterActionUsePower characterActionUsePower ||
                characterActionUsePower.activePower.PowerDefinition != PowerMonkFlurryOfBlows)
            {
                return;
            }

            var rulesetCharacter = action.ActingCharacter.RulesetCharacter;
            var conditionDisengaging = ConditionDefinitions.ConditionDisengaging;
            var rulesetCondition = RulesetCondition.CreateActiveCondition(
                rulesetCharacter.guid,
                conditionDisengaging,
                DurationType.Round,
                1,
                TurnOccurenceType.StartOfTurn,
                rulesetCharacter.guid,
                rulesetCharacter.CurrentFaction.Name);

            rulesetCharacter.AddConditionOfCategory(AttributeDefinitions.TagCombat, rulesetCondition);
        }
    }

    //
    // Tempest Fury
    //

    private sealed class PerformAttackAfterMagicEffectUseTempestFury : IPerformAttackAfterMagicEffectUse
    {
        public IPerformAttackAfterMagicEffectUse.CanAttackHandler CanAttack { get; } =
            CanMeleeAttack;

        public IPerformAttackAfterMagicEffectUse.GetAttackAfterUseHandler PerformAttackAfterUse { get; } =
            DefaultAttackHandler;

        public IPerformAttackAfterMagicEffectUse.CanUseHandler CanBeUsedToAttack { get; } =
            DefaultCanUseHandler;

        private static bool CanMeleeAttack([NotNull] GameLocationCharacter caster, GameLocationCharacter target)
        {
            var attackMode = caster.FindActionAttackMode(ActionDefinitions.Id.AttackOff);

            if (attackMode == null)
            {
                return false;
            }

            var battleService = ServiceRepository.GetService<IGameLocationBattleService>();

            if (battleService == null)
            {
                return false;
            }

            var attackModifier = new ActionModifier();
            var evalParams = new BattleDefinitions.AttackEvaluationParams();

            evalParams.FillForPhysicalReachAttack(caster, caster.LocationPosition, attackMode, target,
                target.LocationPosition, attackModifier);

            return battleService.CanAttack(evalParams);
        }

        [NotNull]
        private static List<CharacterActionParams> DefaultAttackHandler([CanBeNull] CharacterActionMagicEffect effect)
        {
            var attacks = new List<CharacterActionParams>();
            var actionParams = effect?.ActionParams;

            if (actionParams == null)
            {
                return attacks;
            }

            var battleService = ServiceRepository.GetService<IGameLocationBattleService>();

            if (battleService == null)
            {
                return attacks;
            }

            var caster = actionParams.ActingCharacter;
            var targets = battleService.Battle.AllContenders
                .Where(x => x.IsOppositeSide(caster.Side) && battleService.IsWithin1Cell(caster, x))
                .ToList();

            if (caster == null || targets.Empty())
            {
                return attacks;
            }

            var attackMode = caster.FindActionAttackMode(ActionDefinitions.Id.AttackOff);

            if (attackMode == null)
            {
                return attacks;
            }

            //get copy to be sure we don't break existing mode
            var rulesetAttackModeCopy = RulesetAttackMode.AttackModesPool.Get();

            rulesetAttackModeCopy.Copy(attackMode);

            attackMode = rulesetAttackModeCopy;

            //set action type to be same as the one used for the magic effect
            attackMode.ActionType = effect.ActionType;

            var attackModifier = new ActionModifier();

            foreach (var target in targets.Where(t => CanMeleeAttack(caster, t)))
            {
                var attackActionParams =
                    new CharacterActionParams(caster, ActionDefinitions.Id.AttackFree) { AttackMode = attackMode };

                attackActionParams.TargetCharacters.Add(target);
                attackActionParams.ActionModifiers.Add(attackModifier);
                attacks.Add(attackActionParams);
            }

            return attacks;
        }

        private static bool DefaultCanUseHandler(
            [NotNull] CursorLocationSelectTarget targeting,
            GameLocationCharacter caster,
            GameLocationCharacter target, [NotNull] out string failure)
        {
            failure = string.Empty;

            return true;
        }
    }

    //
    // Eye of The Storm
    //

    private sealed class OnAfterActionEyeOfTheStorm : IOnAfterActionFeature
    {
        private readonly ConditionDefinition _conditionEyeOfTheStorm;
        private readonly FeatureDefinitionPower _powerEyeOfTheStorm;
        private readonly FeatureDefinitionPower _powerEyeOfTheStormLeap;

        public OnAfterActionEyeOfTheStorm(
            FeatureDefinitionPower powerEyeOfTheStorm,
            FeatureDefinitionPower powerEyeOfTheStormLeap,
            ConditionDefinition conditionEyeOfTheStorm)
        {
            _powerEyeOfTheStorm = powerEyeOfTheStorm;
            _powerEyeOfTheStormLeap = powerEyeOfTheStormLeap;
            _conditionEyeOfTheStorm = conditionEyeOfTheStorm;
        }

        public void OnAfterAction(CharacterAction action)
        {
            if (action is not CharacterActionUsePower characterActionUsePower ||
                characterActionUsePower.activePower.PowerDefinition != _powerEyeOfTheStorm)
            {
                return;
            }

            var battleService = ServiceRepository.GetService<IGameLocationBattleService>();

            if (battleService == null)
            {
                return;
            }

            var attacker = action.ActingCharacter;
            var rulesetCharacter = attacker.RulesetCharacter;
            var usablePower = UsablePowersProvider.Get(_powerEyeOfTheStormLeap, rulesetCharacter);
            var effectPower = new RulesetEffectPower(rulesetCharacter, usablePower);

            foreach (var defender in battleService.Battle.AllContenders
                         .Where(x => x.IsOppositeSide(attacker.Side))
                         .Where(x => x.RulesetCharacter.AllConditions
                             .Any(y => y.ConditionDefinition == _conditionEyeOfTheStorm &&
                                       y.SourceGuid == rulesetCharacter.Guid)))
            {
                EffectHelpers.StartVisualEffect(
                    attacker, defender, PowerDomainElementalLightningBlade, EffectHelpers.EffectType.Effect);
                effectPower.ApplyEffectOnCharacter(defender.RulesetCharacter, true, defender.LocationPosition);
            }
        }
    }
}
