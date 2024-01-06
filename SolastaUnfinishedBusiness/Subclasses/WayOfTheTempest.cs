using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
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

[UsedImplicitly]
public sealed class WayOfTheTempest : AbstractSubclass
{
    private const string Name = "WayOfTheTempest";

    internal static readonly FeatureDefinitionFeatureSet FeatureSetTempestFury = FeatureDefinitionFeatureSetBuilder
        .Create($"FeatureSet{Name}TempestFury")
        .SetGuiPresentation(Category.Feature)
        .AddToDB();

    public WayOfTheTempest()
    {
        // LEVEL 03

        // Tempest's Swiftness

        var movementAffinityTempestSwiftness = FeatureDefinitionMovementAffinityBuilder
            .Create($"MovementAffinity{Name}TempestSwiftness")
            .SetGuiPresentation(Category.Feature)
            .SetBaseSpeedAdditiveModifier(2)
            .AddCustomSubFeatures(new ActionFinishedByMeTempestSwiftness())
            .AddToDB();

        // LEVEL 06

        // Gathering Storm

        var additionalDamageGatheringStorm = FeatureDefinitionAdditionalDamageBuilder
            .Create($"AdditionalDamage{Name}GatheringStorm")
            .SetGuiPresentationNoContent(true)
            .SetNotificationTag("GatheringStorm")
            .SetRequiredProperty(RestrictedContextRequiredProperty.Unarmed)
            .SetTriggerCondition(ExtraAdditionalDamageTriggerCondition.FlurryOfBlows)
            .SetDamageValueDetermination(AdditionalDamageValueDetermination.SameAsBaseWeaponDie)
            .SetSpecificDamageType(DamageTypeLightning)
            .SetImpactParticleReference(
                ShockingGrasp.EffectDescription.EffectParticleParameters.effectParticleReference)
            .AddToDB();

        var featureSetGatheringStorm = FeatureDefinitionFeatureSetBuilder
            .Create($"Feature{Name}GatheringStorm")
            .SetGuiPresentation($"FeatureSet{Name}GatheringStorm", Category.Feature)
            .AddFeatureSet(
                FeatureDefinitionDamageAffinitys.DamageAffinityLightningResistance,
                additionalDamageGatheringStorm)
            .AddToDB();

        // LEVEL 11

        // Tempest's Fury

        var powerTempestFury = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}TempestFury")
            .SetGuiPresentationNoContent(true)
            .SetUsesFixed(ActivationTime.NoCost, RechargeRate.KiPoints, 3, 3)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetDurationData(DurationType.Round)
                    .SetParticleEffectParameters(ShockingGrasp)
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

        powerTempestFury.AddCustomSubFeatures(new AttackAfterMagicEffectTempestFury());

        _ = ActionDefinitionBuilder
            .Create(DatabaseHelper.ActionDefinitions.FlurryOfBlows, "ActionTempestFury")
            .SetOrUpdateGuiPresentation(Category.Action)
            .SetActionId(ExtraActionId.TempestFury)
            .SetActivatedPower(powerTempestFury, ActionDefinitions.ActionParameter.ActivatePower, false)
            .SetStealthBreakerBehavior(ActionDefinitions.StealthBreakerBehavior.RollIfTargets)
            .OverrideClassName("UsePower")
            .AddToDB();

        var actionAffinityTempestFury =
            FeatureDefinitionActionAffinityBuilder
                .Create($"ActionAffinity{Name}TempestFury")
                .SetGuiPresentationNoContent(true)
                .SetAuthorizedActions((ActionDefinitions.Id)ExtraActionId.TempestFury)
                .AddCustomSubFeatures(
                    new ValidateDefinitionApplication(
                        character => Main.Settings.EnableMonkDoNotRequireAttackActionForFlurry ||
                                     character.ExecutedAttacks > 0,
                        ValidatorsCharacter.HasAvailableBonusAction,
                        ValidatorsCharacter.HasNoneOfConditions(ConditionFlurryOfBlows)))
                .AddToDB();

        FeatureSetTempestFury.FeatureSet.AddRange(actionAffinityTempestFury, powerTempestFury);

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
            .SetGuiPresentation($"Condition{Name}AppliedEyeOfTheStorm", Category.Condition, Gui.NoLocalization)
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
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 0, TargetType.IndividualsUnique)
                    .SetDurationData(DurationType.Round, 1, TurnOccurenceType.EndOfSourceTurn)
                    .SetSavingThrowData(false, AttributeDefinitions.Dexterity, true,
                        EffectDifficultyClassComputation.AbilityScoreAndProficiency)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetDamageForm(DamageTypeLightning, 5, DieType.D10)
                            .HasSavingThrow(EffectSavingThrowType.HalfDamage)
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
                    .SetParticleEffectParameters(PowerDomainElementalLightningBlade)
                    .Build())
            .AddCustomSubFeatures(ValidatorsValidatePowerUse.InCombat)
            .AddToDB();

        powerEyeOfTheStormLeap.EffectDescription.EffectParticleParameters.impactParticleReference =
            powerEyeOfTheStormLeap.EffectDescription.EffectParticleParameters.effectParticleReference;

        var powerEyeOfTheStorm = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}EyeOfTheStorm")
            .SetGuiPresentation($"FeatureSet{Name}EyeOfTheStorm", Category.Feature, PowerOathOfDevotionTurnUnholy)
            .SetUsesFixed(ActivationTime.Action, RechargeRate.KiPoints, 3)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetParticleEffectParameters(ShockingGrasp)
                    .Build())
            .AddCustomSubFeatures(
                ValidatorsValidatePowerUse.InCombat,
                new MagicEffectFinishedByMeEyeOfTheStorm(powerEyeOfTheStormLeap, conditionEyeOfTheStorm))
            .AddToDB();

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
            .AddFeaturesAtLevel(6, featureSetGatheringStorm)
            .AddFeaturesAtLevel(11, FeatureSetTempestFury)
            .AddFeaturesAtLevel(17, featureSetEyeOfTheStorm)
            .AddToDB();
    }

    internal override CharacterClassDefinition Klass => CharacterClassDefinitions.Monk;

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceMonkMonasticTraditions;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }

    //
    // Tempest Swiftness
    //

    private sealed class ActionFinishedByMeTempestSwiftness : IActionFinishedByMe
    {
        public IEnumerator OnActionFinishedByMe(CharacterAction action)
        {
            if (action is not CharacterActionUsePower characterActionUsePower
                || characterActionUsePower.activePower.PowerDefinition != PowerMonkFlurryOfBlows)
            {
                yield break;
            }

            var rulesetCharacter = action.ActingCharacter.RulesetCharacter;

            rulesetCharacter.InflictCondition(
                ConditionDisengaging,
                DurationType.Round,
                1,
                TurnOccurenceType.StartOfTurn,
                // all disengaging in game is set under TagCombat (why?)
                AttributeDefinitions.TagCombat,
                rulesetCharacter.guid,
                rulesetCharacter.CurrentFaction.Name,
                1,
                ConditionDisengaging,
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

            if (battleService is not { IsBattleInProgress: true })
            {
                return false;
            }

            var attackModifier = new ActionModifier();
            var evalParams = new BattleDefinitions.AttackEvaluationParams();

            evalParams.FillForPhysicalReachAttack(
                caster, caster.LocationPosition, attackMode, target, target.LocationPosition, attackModifier);

            return battleService.CanAttack(evalParams);
        }

        [CanBeNull]
        private static IEnumerable<CharacterActionParams> DefaultAttackHandler(
            [CanBeNull] CharacterActionMagicEffect effect)
        {
            var actionParams = effect?.ActionParams;

            if (actionParams == null)
            {
                return null;
            }

            var battleService = ServiceRepository.GetService<IGameLocationBattleService>();

            if (battleService is not { IsBattleInProgress: true })
            {
                return null;
            }

            var caster = actionParams.ActingCharacter;
            var targets = battleService.Battle.AllContenders
                .Where(x => x.IsOppositeSide(caster.Side) && battleService.IsWithin1Cell(caster, x))
                .ToList();

            if (targets.Empty())
            {
                return null;
            }

            var attackMode = caster.FindActionAttackMode(ActionDefinitions.Id.AttackOff);

            if (attackMode == null)
            {
                return null;
            }

            //get copy to be sure we don't break existing mode
            var rulesetAttackModeCopy = RulesetAttackMode.AttackModesPool.Get();

            rulesetAttackModeCopy.Copy(attackMode);

            attackMode = rulesetAttackModeCopy;

            //set action type to be same as the one used for the magic effect
            attackMode.ActionType = effect.ActionType;

            var attackModifier = new ActionModifier();

            return targets
                .Where(t => CanMeleeAttack(caster, t))
                .Select(target =>
                    new CharacterActionParams(caster, ActionDefinitions.Id.AttackFree)
                    {
                        AttackMode = attackMode, TargetCharacters = { target }, ActionModifiers = { attackModifier }
                    });
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

    private sealed class MagicEffectFinishedByMeEyeOfTheStorm : IMagicEffectFinishedByMe
    {
        private readonly ConditionDefinition _conditionEyeOfTheStorm;
        private readonly FeatureDefinitionPower _powerEyeOfTheStormLeap;

        public MagicEffectFinishedByMeEyeOfTheStorm(
            FeatureDefinitionPower powerEyeOfTheStormLeap,
            ConditionDefinition conditionEyeOfTheStorm)
        {
            _powerEyeOfTheStormLeap = powerEyeOfTheStormLeap;
            _conditionEyeOfTheStorm = conditionEyeOfTheStorm;
        }

        public IEnumerator OnMagicEffectFinishedByMe(CharacterActionMagicEffect action, BaseDefinition baseDefinition)
        {
            var gameLocationBattleService = ServiceRepository.GetService<IGameLocationBattleService>();

            if (gameLocationBattleService is not { IsBattleInProgress: true })
            {
                yield break;
            }

            var actionParams = action.ActionParams.Clone();
            var attacker = action.ActingCharacter;
            var rulesetAttacker = attacker.RulesetCharacter;
            var usablePower = UsablePowersProvider.Get(_powerEyeOfTheStormLeap, rulesetAttacker);

            actionParams.ActionDefinition = DatabaseHelper.ActionDefinitions.SpendPower;
            actionParams.RulesetEffect = ServiceRepository.GetService<IRulesetImplementationService>()
                //CHECK: no need for AddAsActivePowerToSource
                .InstantiateEffectPower(rulesetAttacker, usablePower, false);
            actionParams.TargetCharacters.SetRange(gameLocationBattleService.Battle.AllContenders
                .Where(x =>
                    x.IsOppositeSide(attacker.Side) &&
                    x.RulesetCharacter is { IsDeadOrDyingOrUnconscious: false } &&
                    x.RulesetCharacter.AllConditions
                        .Any(y => y.ConditionDefinition == _conditionEyeOfTheStorm &&
                                  y.SourceGuid == rulesetAttacker.Guid))
                .ToList());

            var actionService = ServiceRepository.GetService<IGameLocationActionService>();

            actionService.ExecuteAction(actionParams, null, false);
        }
    }
}
