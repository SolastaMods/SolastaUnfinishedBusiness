using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.CustomValidators;
using SolastaUnfinishedBusiness.Properties;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionAdditionalDamages;
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

        // Storm Surge

        var combatAffinityStormSurge = FeatureDefinitionCombatAffinityBuilder
            .Create($"CombatAffinity{Name}StormSurge")
            .SetGuiPresentation($"Condition{Name}AppliedStormSurge", Category.Condition)
            .SetAttackOnMeAdvantage(AdvantageType.Advantage)
            .AddToDB();

        var conditionAppliedStormSurge = ConditionDefinitionBuilder
            .Create($"Condition{Name}AppliedStormSurge")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionDistracted)
            .SetPossessive()
            .SetConditionType(ConditionType.Detrimental)
            .SetSilent(Silent.WhenAdded)
            .CopyParticleReferences(ConditionDefinitions.ConditionBranded)
            .SetSpecialDuration(DurationType.Round, 0, TurnOccurenceType.EndOfSourceTurn)
            .SetSpecialInterruptions(ConditionInterruption.Attacked, ConditionInterruption.AnyBattleTurnEnd)
            .SetFeatures(combatAffinityStormSurge)
            .AddToDB();

        var conditionStormSurge = ConditionDefinitionBuilder
            .Create($"Condition{Name}StormSurge")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionBaned)
            .SetPossessive()
            .SetConditionType(ConditionType.Detrimental)
            .SetSilent(Silent.WhenRemoved)
            .CopyParticleReferences(ConditionDefinitions.ConditionBranded)
            .SetSpecialDuration(DurationType.Round, 0, TurnOccurenceType.EndOfSourceTurn)
            .SetCancellingConditions(conditionAppliedStormSurge)
            .SetSpecialInterruptions(ConditionInterruption.AnyBattleTurnEnd)
            .AddToDB();

        var additionalDamageStormSurge = FeatureDefinitionAdditionalDamageBuilder
            .Create($"AdditionalDamage{Name}StormSurge")
            .SetGuiPresentationNoContent(true)
            .SetRequiredProperty(RestrictedContextRequiredProperty.UnarmedOrMonkWeapon)
            .SetConditionOperations(new ConditionOperationDescription
            {
                ConditionDefinition = conditionStormSurge,
                Operation = ConditionOperationDescription.ConditionOperation.Add
            })
            .AddToDB();

        var additionalDamageAppliedStormSurge = FeatureDefinitionAdditionalDamageBuilder
            .Create($"AdditionalDamage{Name}AppliedStormSurge")
            .SetGuiPresentationNoContent(true)
            .SetRequiredProperty(RestrictedContextRequiredProperty.UnarmedOrMonkWeapon)
            .SetTargetCondition(conditionStormSurge, AdditionalDamageTriggerCondition.TargetHasCondition)
            .SetConditionOperations(new ConditionOperationDescription
            {
                ConditionDefinition = conditionAppliedStormSurge,
                Operation = ConditionOperationDescription.ConditionOperation.Add
            })
            .AddToDB();

        var conditionExtraUnarmoredAttackStormSurge = ConditionDefinitionBuilder
            .Create($"Condition{Name}ExtraUnarmoredAttackStormSurge")
            .SetGuiPresentation(Category.Condition)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetSpecialDuration(DurationType.Round, 1, TurnOccurenceType.StartOfTurn)
            .SetFeatures(
                FeatureDefinitionBuilder
                    .Create($"Feature{Name}ExtraUnarmoredAttackStormSurge")
                    .SetGuiPresentationNoContent(true)
                    .SetCustomSubFeatures(new AddExtraUnarmedAttack(ActionDefinitions.ActionType.Bonus))
                    .AddToDB())
            .AddToDB();

        var featureStormSurge = FeatureDefinitionBuilder
            .Create($"Feature{Name}StormSurge")
            .SetGuiPresentationNoContent(true)
            .SetCustomSubFeatures(new OnAfterActionStormSurge(conditionExtraUnarmoredAttackStormSurge))
            .AddToDB();

        var featureSetStormSurge = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}StormSurge")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(additionalDamageStormSurge, additionalDamageAppliedStormSurge, featureStormSurge)
            .AddToDB();

        // LEVEL 11

        // Tempest’s Fury

        var powerTempestFury = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}TempestFury")
            .SetGuiPresentation(Category.Feature,
                Sprites.GetSprite("TempestFury", Resources.PowerTempestFury, 256, 128))
            .SetUsesFixed(ActivationTime.NoCost, RechargeRate.KiPoints, 3)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetDurationData(DurationType.Round, 1)
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

        // Unfettered Deluge

        // Mark

        var conditionUnfetteredDeluge = ConditionDefinitionBuilder
            .Create($"Condition{Name}UnfetteredDeluge")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionMarkedByBrandingSmite)
            .SetPossessive()
            .SetConditionType(ConditionType.Detrimental)
            .CopyParticleReferences(ConditionDefinitions.ConditionMarkedByBrandingSmite)
            .SetSpecialDuration(DurationType.Minute, 1, TurnOccurenceType.EndOfSourceTurn)
            .AddToDB();

        var additionalDamageUnfetteredDeluge = FeatureDefinitionAdditionalDamageBuilder
            .Create($"AdditionalDamage{Name}UnfetteredDeluge")
            .SetGuiPresentationNoContent(true)
            .SetRequiredProperty(RestrictedContextRequiredProperty.UnarmedOrMonkWeapon)
            .SetImpactParticleReference(AdditionalDamageHuntersMark.impactParticleReference)
            .SetConditionOperations(new ConditionOperationDescription
            {
                ConditionDefinition = conditionUnfetteredDeluge,
                Operation = ConditionOperationDescription.ConditionOperation.Add
            })
            .AddToDB();

        // Staggered

        var abilityCheckAffinityUnfetteredDeluge = FeatureDefinitionAbilityCheckAffinityBuilder
            .Create($"AbilityCheckAffinity{Name}AppliedUnfetteredDeluge")
            .SetGuiPresentation($"Condition{Name}AppliedUnfetteredDeluge", Category.Condition)
            .BuildAndSetAffinityGroups(CharacterAbilityCheckAffinity.Disadvantage,
                AttributeDefinitions.Strength,
                AttributeDefinitions.Dexterity,
                AttributeDefinitions.Constitution,
                AttributeDefinitions.Intelligence,
                AttributeDefinitions.Wisdom,
                AttributeDefinitions.Charisma)
            .AddToDB();

        var combatAffinityUnfetteredDeluge = FeatureDefinitionCombatAffinityBuilder
            .Create($"CombatAffinity{Name}AppliedUnfetteredDeluge")
            .SetGuiPresentation($"Condition{Name}AppliedUnfetteredDeluge", Category.Condition)
            .SetMyAttackAdvantage(AdvantageType.Disadvantage)
            .AddToDB();

        var conditionAppliedUnfetteredDeluge = ConditionDefinitionBuilder
            .Create($"Condition{Name}AppliedUnfetteredDeluge")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionMarkedByHunter)
            .SetPossessive()
            .SetConditionType(ConditionType.Detrimental)
            .CopyParticleReferences(ConditionDefinitions.ConditionMarkedByHunter)
            .AddFeatures(abilityCheckAffinityUnfetteredDeluge, combatAffinityUnfetteredDeluge)
            .AddToDB();

        // Powers

        var powerUnfetteredDelugeLeap = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}UnfetteredDelugeLeap")
            .SetGuiPresentation($"FeatureSet{Name}UnfetteredDeluge", Category.Feature, hidden: true)
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
                            .SetConditionForm(conditionAppliedUnfetteredDeluge, ConditionForm.ConditionOperation.Add)
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .Build(),
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(conditionUnfetteredDeluge, ConditionForm.ConditionOperation.Remove)
                            .Build())
                    .Build())
            .SetCustomSubFeatures(ValidatorsPowerUse.InCombat)
            .AddToDB();

        var powerUnfetteredDeluge = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}UnfetteredDeluge")
            .SetGuiPresentation($"FeatureSet{Name}UnfetteredDeluge", Category.Feature, PowerOathOfDevotionTurnUnholy)
            .SetUsesFixed(ActivationTime.Action, RechargeRate.KiPoints, 3)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetDurationData(DurationType.Instantaneous)
                    .SetParticleEffectParameters(PowerDomainElementalLightningBlade)
                    .Build())
            .AddToDB();

        powerUnfetteredDeluge.SetCustomSubFeatures(
            ValidatorsPowerUse.InCombat,
            new OnAfterActionUnfetteredDeluge(
                powerUnfetteredDeluge, powerUnfetteredDelugeLeap, conditionUnfetteredDeluge));

        var featureSetUnfetteredDeluge = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}UnfetteredDeluge")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(additionalDamageUnfetteredDeluge, powerUnfetteredDeluge, powerUnfetteredDelugeLeap)
            .AddToDB();

        //
        // MAIN
        //

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Subclass, Sprites.GetSprite(Name, Resources.WayOfTheTempest, 256))
            .AddFeaturesAtLevel(3, movementAffinityTempestSwiftness)
            .AddFeaturesAtLevel(6, featureSetStormSurge)
            .AddFeaturesAtLevel(11, powerTempestFury)
            .AddFeaturesAtLevel(17, featureSetUnfetteredDeluge)
            .AddToDB();
    }

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceMonkMonasticTraditions;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }

    //
    // Storm Surge
    //

    private sealed class OnAfterActionStormSurge : IOnAfterActionFeature
    {
        private readonly ConditionDefinition _conditionStormSurge;

        public OnAfterActionStormSurge(ConditionDefinition conditionStormSurge)
        {
            _conditionStormSurge = conditionStormSurge;
        }

        public void OnAfterAction(CharacterAction action)
        {
            if (action.ActionType != ActionDefinitions.ActionType.Main || action is CharacterActionAttack)
            {
                return;
            }

            var rulesetCharacter = action.ActingCharacter.RulesetCharacter;
            var rulesetCondition = RulesetCondition.CreateActiveCondition(
                rulesetCharacter.guid,
                _conditionStormSurge,
                _conditionStormSurge.DurationType,
                _conditionStormSurge.DurationParameter,
                _conditionStormSurge.TurnOccurence,
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
                .Where(x => x.Side != caster.Side && battleService.IsWithin1Cell(caster, x))
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
    // Unfettered Deluge
    //

    private sealed class OnAfterActionUnfetteredDeluge : IOnAfterActionFeature
    {
        private readonly ConditionDefinition _conditionUnfetteredDeluge;
        private readonly FeatureDefinitionPower _powerUnfetteredDeluge;
        private readonly FeatureDefinitionPower _powerUnfetteredDelugeLeap;

        public OnAfterActionUnfetteredDeluge(
            FeatureDefinitionPower powerUnfetteredDeluge,
            FeatureDefinitionPower powerUnfetteredDelugeLeap,
            ConditionDefinition conditionUnfetteredDeluge)
        {
            _powerUnfetteredDeluge = powerUnfetteredDeluge;
            _powerUnfetteredDelugeLeap = powerUnfetteredDelugeLeap;
            _conditionUnfetteredDeluge = conditionUnfetteredDeluge;
        }

        public void OnAfterAction(CharacterAction action)
        {
            if (action is not CharacterActionUsePower characterActionUsePower ||
                characterActionUsePower.activePower.PowerDefinition != _powerUnfetteredDeluge)
            {
                return;
            }

            var battleService = ServiceRepository.GetService<IGameLocationBattleService>();

            if (battleService == null)
            {
                return;
            }

            var rulesetCharacter = action.ActingCharacter.RulesetCharacter;
            var usablePower = UsablePowersProvider.Get(_powerUnfetteredDelugeLeap, rulesetCharacter);
            var effectPower = new RulesetEffectPower(rulesetCharacter, usablePower);

            foreach (var targetLocationCharacter in battleService.Battle.AllContenders
                         .Where(x => x.Side != rulesetCharacter.Side && x.RulesetCharacter != null)
                         .Where(x => x.RulesetCharacter.AllConditions
                             .Any(y => y.ConditionDefinition == _conditionUnfetteredDeluge &&
                                       y.SourceGuid == rulesetCharacter.Guid)))
            {
                effectPower.ApplyEffectOnCharacter(
                    targetLocationCharacter.RulesetCharacter, true, targetLocationCharacter.LocationPosition);
            }
        }
    }
}
