using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Behaviors;
using SolastaUnfinishedBusiness.Behaviors.Specific;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Properties;
using SolastaUnfinishedBusiness.Validators;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static SolastaUnfinishedBusiness.Builders.Features.AutoPreparedSpellsGroupBuilder;

namespace SolastaUnfinishedBusiness.Subclasses;

[UsedImplicitly]
public sealed class RangerSkyWarrior : AbstractSubclass
{
    private const string Name = "RangerSkyWarrior";

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
            .SetSpecialDuration(DurationType.Round, 0, TurnOccurenceType.EndOfSourceTurn)
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

        var conditionGiftOfTheWind = ConditionDefinitionBuilder
            .Create($"Condition{Name}GiftOfTheWind")
            .SetGuiPresentation($"Condition{Name}GiftOfTheWindAttacked", Category.Condition,
                Gui.NoLocalization)
            .SetPossessive()
            .AddFeatures(movementAffinityGiftOfTheWind, combatAffinityGiftOfTheWind)
            .AddToDB();

        conditionGiftOfTheWind.AddCustomSubFeatures(new ItemEquippedCheckHeroStillHasShield(conditionGiftOfTheWind));

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
                    .SetEffectForms(EffectFormBuilder.ConditionForm(conditionGiftOfTheWind))
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

        // kept for backward compatibility
        _ = FeatureDefinitionAttributeModifierBuilder
            .Create($"AttributeModifier{Name}SwiftStrike")
            .SetGuiPresentation(Category.Feature)
            .SetModifierAbilityScore(AttributeDefinitions.Initiative, AttributeDefinitions.Wisdom)
            .AddToDB();

        var powerGhostlyHowl = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}GhostlyHowl")
            .SetGuiPresentation(Category.Feature)
            .SetUsesAbilityBonus(ActivationTime.NoCost, RechargeRate.LongRest, AttributeDefinitions.Wisdom)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round)
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 6, TargetType.IndividualsUnique)
                    .SetSavingThrowData(false, AttributeDefinitions.Wisdom, false,
                        EffectDifficultyClassComputation.SpellCastingFeature)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(ConditionDefinitions.ConditionFrightened,
                                ConditionForm.ConditionOperation.Add)
                            .HasSavingThrow(EffectSavingThrowType.Negates, TurnOccurenceType.StartOfTurn, true)
                            .Build())
                    .Build())
            .AddToDB();

        powerGhostlyHowl.AddCustomSubFeatures(
            ModifyPowerVisibility.Hidden,
            new CustomBehaviorGhostlyHowl(powerGhostlyHowl));

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

        // kept name for backward compatibility
        var powerDeathFromAbove = FeatureDefinitionPowerBuilder
            .Create($"Feature{Name}DeathFromAbove")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.NoCost)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 6, TargetType.IndividualsUnique)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetBonusMode(AddBonusMode.Proficiency)
                            .SetDamageForm()
                            .Build())
                    .Build())
            .AddCustomSubFeatures(ModifyPowerVisibility.Hidden)
            .AddToDB();

        powerDeathFromAbove.AddCustomSubFeatures(
            new CustomBehaviorDeathFromAbove(
                powerDeathFromAbove, conditionGiftOfTheWind, conditionGiftOfTheWindAttacked));

        //
        // LEVEL 15
        //

        // Cloud Dance

        var conditionCloudDance = ConditionDefinitionBuilder
            .Create(ConditionDefinitions.ConditionFlyingAdaptive, $"Condition{Name}CloudDance")
            .AddToDB();

        conditionCloudDance.AddCustomSubFeatures(new ItemEquippedCheckHeroStillHasShield(conditionCloudDance));

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
                autoPreparedSpells, additionalDamageGiftOfTheWind, powerGiftOfTheWind, proficiencyAerialAgility)
            .AddFeaturesAtLevel(7,
                powerGhostlyHowl, damageAffinityIntangibleForm)
            .AddFeaturesAtLevel(11,
                powerDeathFromAbove)
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

    private sealed class ItemEquippedCheckHeroStillHasShield(
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        ConditionDefinition condition) : IOnItemEquipped
    {
        public void OnItemEquipped(RulesetCharacterHero hero)
        {
            if (!ValidatorsCharacter.HasShield(hero) &&
                hero.TryGetConditionOfCategoryAndType(
                    AttributeDefinitions.TagEffect, condition.Name, out var activeCondition))
            {
                hero.RemoveCondition(activeCondition);
            }
        }
    }

    private class CustomBehaviorGhostlyHowl(FeatureDefinitionPower powerGhostlyHowl)
        : ITryAlterOutcomeAttack, IModifyEffectDescription
    {
        public bool IsValid(BaseDefinition definition, RulesetCharacter character, EffectDescription effectDescription)
        {
            return definition == powerGhostlyHowl;
        }

        public EffectDescription GetEffectDescription(
            BaseDefinition definition,
            EffectDescription effectDescription,
            RulesetCharacter character,
            RulesetEffect rulesetEffect)
        {
            var wisdom = character.TryGetAttributeValue(AttributeDefinitions.Wisdom);
            var wisMod = Math.Max(1, AttributeDefinitions.ComputeAbilityScoreModifier(wisdom));

            effectDescription.durationParameter = wisMod;

            return effectDescription;
        }

        public int HandlerPriority => 30;

        public IEnumerator OnTryAlterOutcomeAttack(
            GameLocationBattleManager instance,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            GameLocationCharacter helper,
            ActionModifier actionModifier,
            RulesetAttackMode attackMode,
            RulesetEffect rulesetEffect)
        {
            var battleManager = ServiceRepository.GetService<IGameLocationBattleService>() as GameLocationBattleManager;

            if (!battleManager)
            {
                yield break;
            }

            var rulesetHelper = helper.RulesetCharacter;

            if (helper != defender ||
                !defender.CanReact() ||
                !defender.CanPerceiveTarget(attacker) ||
                !defender.IsWithinRange(attacker, 12) ||
                rulesetHelper.GetRemainingPowerUses(powerGhostlyHowl) == 0)
            {
                yield break;
            }

            var actionService = ServiceRepository.GetService<IGameLocationActionService>();
            var implementationManager =
                ServiceRepository.GetService<IRulesetImplementationService>() as RulesetImplementationManager;

            var usablePower = PowerProvider.Get(powerGhostlyHowl, rulesetHelper);
            var actionParams =
                new CharacterActionParams(helper, ActionDefinitions.Id.PowerReaction)
                {
                    StringParameter = "GhostlyHowl",
                    ActionModifiers = { new ActionModifier() },
                    RulesetEffect = implementationManager
                        .MyInstantiateEffectPower(rulesetHelper, usablePower, false),
                    UsablePower = usablePower,
                    TargetCharacters = { attacker }
                };

            var count = actionService.PendingReactionRequestGroups.Count;

            actionService.ReactToUsePower(actionParams, "UsePower", defender);

            yield return battleManager.WaitForReactions(attacker, actionService, count);
        }
    }

    private sealed class CustomBehaviorDeathFromAbove(
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        FeatureDefinitionPower powerDeathFromAbove,
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        ConditionDefinition conditionGiftOfTheWind,
        ConditionDefinition conditionGiftOfTheWindAttacked) : IPhysicalAttackFinishedByMe, IMagicEffectFinishedByMe
    {
        public IEnumerator OnMagicEffectFinishedByMe(
            CharacterActionMagicEffect action,
            GameLocationCharacter attacker,
            List<GameLocationCharacter> targets)
        {
            var rulesetEffect = action.ActionParams.RulesetEffect;

            if (action.AttackRoll == 0 ||
                action.AttackRollOutcome is not (RollOutcome.Success or RollOutcome.CriticalSuccess) ||
                (rulesetEffect != null &&
                 rulesetEffect.EffectDescription.RangeType is not (RangeType.MeleeHit or RangeType.RangeHit)))
            {
                yield break;
            }

            HandleConditionAndDamage(attacker, action.AttackRollOutcome == RollOutcome.CriticalSuccess);
        }

        public IEnumerator OnPhysicalAttackFinishedByMe(
            GameLocationBattleManager battleManager,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RulesetAttackMode attackMode,
            RollOutcome rollOutcome,
            int damageAmount)
        {
            if (action.AttackRoll == 0 ||
                action.AttackRollOutcome is not (RollOutcome.Success or RollOutcome.CriticalSuccess))
            {
                yield break;
            }

            HandleConditionAndDamage(attacker, action.AttackRollOutcome == RollOutcome.CriticalSuccess);
        }

        private void HandleConditionAndDamage(GameLocationCharacter attacker, bool criticalHit)
        {
            var rulesetAttacker = attacker.RulesetCharacter;

            if (Gui.Battle == null ||
                !rulesetAttacker.HasConditionOfCategoryAndType(
                    AttributeDefinitions.TagEffect, conditionGiftOfTheWind.Name))
            {
                return;
            }

            var targets = Gui.Battle
                .GetContenders(attacker, hasToPerceiveTarget: true, withinRange: 1);

            if (criticalHit)
            {
                foreach (var rulesetDefender in targets
                             .Select(gameLocationDefender => gameLocationDefender.RulesetCharacter))
                {
                    rulesetDefender.InflictCondition(
                        conditionGiftOfTheWindAttacked.Name,
                        conditionGiftOfTheWindAttacked.DurationType,
                        conditionGiftOfTheWindAttacked.DurationParameter,
                        conditionGiftOfTheWindAttacked.TurnOccurence,
                        AttributeDefinitions.TagEffect,
                        rulesetAttacker.guid,
                        rulesetAttacker.CurrentFaction.Name,
                        1,
                        conditionGiftOfTheWindAttacked.Name,
                        0,
                        0,
                        0);
                }
            }

            var implementationManager =
                ServiceRepository.GetService<IRulesetImplementationService>() as RulesetImplementationManager;

            var usablePower = PowerProvider.Get(powerDeathFromAbove, rulesetAttacker);
            var actionModifiers = new List<ActionModifier>();

            for (var i = 0; i < targets.Count; i++)
            {
                actionModifiers.Add(new ActionModifier());
            }

            var actionParams = new CharacterActionParams(attacker, ActionDefinitions.Id.PowerNoCost)
            {
                ActionModifiers = actionModifiers,
                RulesetEffect = implementationManager
                    .MyInstantiateEffectPower(rulesetAttacker, usablePower, false),
                UsablePower = usablePower,
                targetCharacters = targets
            };

            ServiceRepository.GetService<IGameLocationActionService>()?
                .ExecuteAction(actionParams, null, true);
        }
    }
}
