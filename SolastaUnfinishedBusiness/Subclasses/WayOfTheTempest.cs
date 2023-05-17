using System.Collections;
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
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;

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
            .SetCustomSubFeatures(new ActionFinishedTempestSwiftness())
            .AddToDB();

        // LEVEL 06

        // Gathering Storm

        var powerAppliedGatheringStorm = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}AppliedGatheringStorm")
            .SetGuiPresentation($"Condition{Name}AppliedGatheringStorm", Category.Condition, hidden: true)
            .SetUsesFixed(ActivationTime.NoCost)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Instantaneous)
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 2, TargetType.IndividualsUnique, 3)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetDamageForm(DamageTypeLightning, 1, DieType.D4)
                            .Build())
                    .Build())
            .SetCustomSubFeatures(new ModifyMagicEffectGatheringStorm())
            .AddToDB();

        var combatAffinityGatheringStorm = FeatureDefinitionCombatAffinityBuilder
            .Create($"CombatAffinity{Name}AppliedGatheringStorm")
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
            .SetSpecialDuration(DurationType.Round, 1)
            .CopyParticleReferences(ConditionDefinitions.ConditionGuided)
            .SetCancellingConditions(conditionAppliedGatheringStorm)
            .AddToDB();

        var featureGatheringStorm = FeatureDefinitionBuilder
            .Create($"Feature{Name}GatheringStorm")
            .SetGuiPresentation(Category.Feature)
            .SetCustomSubFeatures(
                new AttackEffectAfterDamageGatheringStorm(
                    conditionGatheringStorm,
                    conditionAppliedGatheringStorm,
                    powerAppliedGatheringStorm))
            .AddToDB();

        // LEVEL 11

        // Tempest's Fury

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
            new AttackAfterMagicEffectTempestFury(),
            new ValidatorsPowerUse(
                ValidatorsCharacter.HasAttacked,
                ValidatorsCharacter.HasAvailableBonusAction,
                ValidatorsCharacter.HasNoneOfConditions(ConditionFlurryOfBlows)));

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
            .SetConditionOperations(
                new ConditionOperationDescription
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
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionDazzled)
            .SetPossessive()
            .SetConditionType(ConditionType.Detrimental)
            .CopyParticleReferences(ConditionDefinitions.ConditionDazzled)
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
                            .SetDamageForm(DamageTypeLightning, 3, DieType.D10)
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
                    .SetParticleEffectParameters(ShockingGrasp)
                    .SetDurationData(DurationType.Instantaneous)
                    .Build())
            .AddToDB();

        powerEyeOfTheStorm.SetCustomSubFeatures(
            ValidatorsPowerUse.InCombat,
            new ActionFinishedEyeOfTheStorm(powerEyeOfTheStorm, powerEyeOfTheStormLeap, conditionEyeOfTheStorm));

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

    private sealed class ModifyMagicEffectGatheringStorm : IModifyMagicEffect
    {
        public EffectDescription ModifyEffect(
            BaseDefinition definition,
            EffectDescription effectDescription,
            RulesetCharacter character,
            RulesetEffect rulesetEffect)
        {
            var damage = effectDescription.FindFirstDamageForm();

            if (damage == null)
            {
                return effectDescription;
            }

            var monkLevel = character.GetClassLevel(CharacterClassDefinitions.Monk);
            var dieType = monkLevel switch
            {
                < 5 => DieType.D4,
                < 11 => DieType.D6,
                < 17 => DieType.D8,
                _ => DieType.D10
            };

            damage.dieType = dieType;

            return effectDescription;
        }
    }

    private sealed class AttackEffectAfterDamageGatheringStorm : IAttackEffectAfterDamage
    {
        private readonly ConditionDefinition _conditionAppliedGatheringStorm;
        private readonly ConditionDefinition _conditionGatheringStorm;
        private readonly FeatureDefinitionPower _powerAppliedGatheringStorm;

        public AttackEffectAfterDamageGatheringStorm(
            ConditionDefinition conditionGatheringStorm,
            ConditionDefinition conditionAppliedGatheringStorm,
            FeatureDefinitionPower powerAppliedGatheringStorm)
        {
            _conditionGatheringStorm = conditionGatheringStorm;
            _conditionAppliedGatheringStorm = conditionAppliedGatheringStorm;
            _powerAppliedGatheringStorm = powerAppliedGatheringStorm;
        }

        public void OnAttackEffectAfterDamage(
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
                attacker.RulesetCharacter.RemoveAllConditionsOfCategoryAndType(AttributeDefinitions.TagCombat,
                    _conditionAppliedGatheringStorm.Name);

                return;
            }

            if (rulesetCharacter.HasAnyConditionOfType(_conditionAppliedGatheringStorm.Name))
            {
                attacker.RulesetCharacter.RemoveAllConditionsOfCategoryAndType(AttributeDefinitions.TagCombat,
                    _conditionAppliedGatheringStorm.Name);

                ApplyDamage(attacker);

                return;
            }

            if (!rulesetCharacter.HasAnyConditionOfType(_conditionGatheringStorm.Name))
            {
                rulesetCharacter.InflictCondition(
                    _conditionGatheringStorm.Name,
                    _conditionGatheringStorm.DurationType,
                    _conditionGatheringStorm.DurationParameter,
                    _conditionGatheringStorm.TurnOccurence,
                    AttributeDefinitions.TagCombat,
                    rulesetCharacter.guid,
                    rulesetCharacter.CurrentFaction.Name,
                    1,
                    null,
                    0,
                    0,
                    0);

                return;
            }

            rulesetCharacter.InflictCondition(
                _conditionAppliedGatheringStorm.Name,
                _conditionAppliedGatheringStorm.DurationType,
                _conditionAppliedGatheringStorm.DurationParameter,
                _conditionAppliedGatheringStorm.TurnOccurence,
                AttributeDefinitions.TagCombat,
                rulesetCharacter.guid,
                rulesetCharacter.CurrentFaction.Name,
                1,
                null,
                0,
                0,
                0);
        }

        /*
        0. Create possible targets empty ordered list
        1. Add all non immune enemies within 5 feet ordered by current hit points
        2. Add all non immune enemies within 10 feet ordered by current hit points
        3. While list has at least one element and selected targets less than 3
            3.1 Find first non resistant target
            3.2 If not found find first target
            3.3 Remove target from ordered list and apply damage
         */
        private void ApplyDamage(GameLocationCharacter attacker)
        {
            var gameLocationBattleService =
                ServiceRepository.GetService<IGameLocationBattleService>() as GameLocationBattleManager;

            if (gameLocationBattleService == null)
            {
                return;
            }

            var battle = gameLocationBattleService.Battle;

            // add non immune targets within 5 feet
            var targets = battle.AllContenders
                .Where(x =>
                    x.Side != attacker.Side &&
                    x.RulesetCharacter is { IsDeadOrDying: false } &&
                    gameLocationBattleService.IsWithin1Cell(attacker, x) &&
                    x.RulesetCharacter
                        .GetFeaturesByType<IDamageAffinityProvider>()
                        .All(y =>
                            y.DamageAffinityType != DamageAffinityType.Immunity ||
                            y.DamageType != DamageTypeLightning))
                .OrderBy(x => x.RulesetCharacter.CurrentHitPoints)
                .ToList();

            // add non immune targets within 10 feet
            targets.AddRange(battle.AllContenders
                .Where(x =>
                    x.Side != attacker.Side &&
                    x.RulesetCharacter is { IsDeadOrDying: false } &&
                    gameLocationBattleService.IsWithinXCells(attacker, x, 2) &&
                    x.RulesetCharacter
                        .GetFeaturesByType<IDamageAffinityProvider>()
                        .All(y =>
                            y.DamageAffinityType != DamageAffinityType.Immunity ||
                            y.DamageType != DamageTypeLightning))
                .Except(targets)
                .OrderBy(x => x.RulesetCharacter.CurrentHitPoints));

            var rulesetAttacker = attacker.RulesetCharacter;
            var usablePower = UsablePowersProvider.Get(_powerAppliedGatheringStorm, attacker.RulesetCharacter);
            var effectPower = new RulesetEffectPower(rulesetAttacker, usablePower);
            var selected = 3;

            while (selected-- > 0 && targets.Count > 0)
            {
                var target =
                    targets.FirstOrDefault(x => x.RulesetCharacter
                        .GetFeaturesByType<IDamageAffinityProvider>()
                        .All(y =>
                            y.DamageAffinityType != DamageAffinityType.Resistance &&
                            y.DamageType != DamageTypeLightning)) ??
                    targets.First();

                EffectHelpers.StartVisualEffect(attacker, target, ShockingGrasp,
                    EffectHelpers.EffectType.Effect);
                effectPower.ApplyEffectOnCharacter(target.RulesetCharacter, true, target.LocationPosition);
                targets.Remove(target);
            }
        }
    }

    //
    // Tempest Swiftness
    //

    private sealed class ActionFinishedTempestSwiftness : IActionFinished
    {
        public IEnumerator OnActionFinished(CharacterAction action)
        {
            if (action is not CharacterActionUsePower characterActionUsePower ||
                characterActionUsePower.activePower.PowerDefinition != PowerMonkFlurryOfBlows)
            {
                yield break;
            }

            var rulesetCharacter = action.ActingCharacter.RulesetCharacter;

            rulesetCharacter.InflictCondition(
                ConditionDisengaging,
                DurationType.Round,
                1,
                TurnOccurenceType.StartOfTurn,
                AttributeDefinitions.TagCombat,
                rulesetCharacter.guid,
                rulesetCharacter.CurrentFaction.Name,
                1,
                null,
                0,
                0,
                0);
        }
    }

    //
    // Tempest Fury
    //

    private sealed class AttackAfterMagicEffectTempestFury : IAttackAfterMagicEffect
    {
        public IAttackAfterMagicEffect.CanAttackHandler CanAttack { get; } =
            CanMeleeAttack;

        public IAttackAfterMagicEffect.GetAttackAfterUseHandler PerformAttackAfterUse { get; } =
            DefaultAttackHandler;

        public IAttackAfterMagicEffect.CanUseHandler CanBeUsedToAttack { get; } =
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

    private sealed class ActionFinishedEyeOfTheStorm : IActionFinished
    {
        private readonly ConditionDefinition _conditionEyeOfTheStorm;
        private readonly FeatureDefinitionPower _powerEyeOfTheStorm;
        private readonly FeatureDefinitionPower _powerEyeOfTheStormLeap;

        public ActionFinishedEyeOfTheStorm(
            FeatureDefinitionPower powerEyeOfTheStorm,
            FeatureDefinitionPower powerEyeOfTheStormLeap,
            ConditionDefinition conditionEyeOfTheStorm)
        {
            _powerEyeOfTheStorm = powerEyeOfTheStorm;
            _powerEyeOfTheStormLeap = powerEyeOfTheStormLeap;
            _conditionEyeOfTheStorm = conditionEyeOfTheStorm;
        }

        public IEnumerator OnActionFinished(CharacterAction action)
        {
            if (action is not CharacterActionUsePower characterActionUsePower ||
                characterActionUsePower.activePower.PowerDefinition != _powerEyeOfTheStorm)
            {
                yield break;
            }

            var battleService = ServiceRepository.GetService<IGameLocationBattleService>();

            if (battleService == null)
            {
                yield break;
            }

            var attacker = action.ActingCharacter;
            var rulesetCharacter = attacker.RulesetCharacter;
            var usablePower = UsablePowersProvider.Get(_powerEyeOfTheStormLeap, rulesetCharacter);
            var effectPower = new RulesetEffectPower(rulesetCharacter, usablePower);

            foreach (var defender in battleService.Battle.AllContenders
                         .Where(x =>
                             x.RulesetCharacter is not { IsDeadOrDying: true } &&
                             x.IsOppositeSide(attacker.Side) &&
                             x.RulesetCharacter.AllConditions
                                 .Any(y => y.ConditionDefinition == _conditionEyeOfTheStorm &&
                                           y.SourceGuid == rulesetCharacter.Guid))
                         .ToList())
            {
                EffectHelpers.StartVisualEffect(
                    attacker, defender, PowerDomainElementalLightningBlade, EffectHelpers.EffectType.Effect);
                effectPower.ApplyEffectOnCharacter(defender.RulesetCharacter, true, defender.LocationPosition);
            }
        }
    }
}
