using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Behaviors;
using SolastaUnfinishedBusiness.Behaviors.Specific;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Validators;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionActionAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.WeaponTypeDefinitions;

namespace SolastaUnfinishedBusiness.Subclasses;

[UsedImplicitly]
public sealed class RoguishBladeCaller : AbstractSubclass
{
    private const string Name = "RoguishBladeCaller";
    private const string BladeMark = "BladeMark";

    private static readonly IsWeaponValidHandler IsBladeCallerWeapon = (mode, item, character) =>
        ValidatorsWeapon.IsOfWeaponType(DaggerType)(mode, item, character);

    public RoguishBladeCaller()
    {
        // LEVEL 03

        // Blade Bond

        var featureBladeBond = FeatureDefinitionBuilder
            .Create($"Feature{Name}BladeBond")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        // Blade Mark

        var conditionBladeMark = ConditionDefinitionBuilder
            .Create($"Condition{Name}BladeMark")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionTargetedByGuidingBolt)
            .SetPossessive()
            .SetConditionType(ConditionType.Detrimental)
            .SetAmountOrigin(ExtraOriginOfAmount.SourceClassLevel)
            .SetSpecialDuration(DurationType.Round, 1, TurnOccurenceType.EndOfSourceTurn)
            .AddToDB();

        var additionalDamageBladeMark = FeatureDefinitionAdditionalDamageBuilder
            .Create($"AdditionalDamage{Name}BladeMark")
            .SetGuiPresentationNoContent(true)
            .SetNotificationTag(BladeMark)
            .SetAttackOnly()
            .SetTargetCondition(conditionBladeMark, AdditionalDamageTriggerCondition.TargetHasCondition)
            .SetDamageValueDetermination(ExtraAdditionalDamageValueDetermination.FlatWithProgression)
            .SetAdvancement(AdditionalDamageAdvancement.ClassLevel, 1, 1, 2)
            .SetRequiredProperty(RestrictedContextRequiredProperty.Weapon)
            .AddCustomSubFeatures(
                new RogueClassHolder(),
                new ValidateContextInsteadOfRestrictedProperty(
                    (_, _, character, _, _, mode, _) =>
                        (OperationType.Set, IsBladeCallerWeapon(mode, null, character))))
            .AddToDB();

        var combatAffinityBladeMark = FeatureDefinitionCombatAffinityBuilder
            .Create($"CombatAffinity{Name}BladeMark")
            .SetGuiPresentation($"Condition{Name}BladeMark", Category.Condition, Gui.NoLocalization)
            .SetSituationalContext(SituationalContext.TargetHasCondition, conditionBladeMark)
            .SetMyAttackAdvantage(AdvantageType.Advantage)
            .AddToDB();

        var featureSetBladeMark = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}BladeMark").SetGuiPresentation(Category.Feature)
            .AddFeatureSet(additionalDamageBladeMark, combatAffinityBladeMark)
            .AddToDB();

        // LEVEL 09 - Hail of Blades

        var powerHailOfBlades = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}HailOfBlades")
            .SetGuiPresentation(Category.Feature, hidden: true)
            .SetUsesFixed(ActivationTime.Reaction, RechargeRate.ShortRest)
            .SetReactionContext(ExtraReactionContext.Custom)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Touch, 0, TargetType.IndividualsUnique)
                    .SetSavingThrowData(false, AttributeDefinitions.Dexterity, false,
                        EffectDifficultyClassComputation.AbilityScoreAndProficiency, AttributeDefinitions.Dexterity, 8)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.HalfDamage)
                            .SetDamageForm(DamageTypeForce, 0, DieType.D6)
                            .SetDiceAdvancement(LevelSourceType.ClassLevel, 1, 1, 2)
                            .Build(),
                        EffectFormBuilder.ConditionForm(conditionBladeMark))
                    .SetParticleEffectParameters(SpellDefinitions.ShadowDagger)
                    .Build())
            .AddToDB();

        var actionAffinityHailOfBladesToggle = FeatureDefinitionActionAffinityBuilder
            .Create(ActionAffinitySorcererMetamagicToggle, "ActionAffinityHailOfBladesToggle")
            .SetGuiPresentationNoContent(true)
            .SetAuthorizedActions((ActionDefinitions.Id)ExtraActionId.HailOfBladesToggle)
            .AddCustomSubFeatures(
                new ValidateDefinitionApplication(ValidatorsCharacter.HasAvailablePowerUsage(powerHailOfBlades)))
            .AddToDB();

        var featureSetHailOfBlades = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}HailOfBlades")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(actionAffinityHailOfBladesToggle, powerHailOfBlades)
            .AddToDB();

        // LEVEL 13 - Blade Surge

        var movementAffinityBladeSurge = FeatureDefinitionMovementAffinityBuilder
            .Create($"MovementAffinity{Name}BladeSurge")
            .SetGuiPresentation($"Condition{Name}BladeSurge", Category.Condition, Gui.NoLocalization)
            .SetBaseSpeedAdditiveModifier(2)
            .AddToDB();

        var additionalActionBladeSurge = FeatureDefinitionAdditionalActionBuilder
            .Create($"AdditionalAction{Name}BladeSurge")
            .SetGuiPresentationNoContent(true)
            .SetActionType(ActionDefinitions.ActionType.Main)
            .SetRestrictedActions(ActionDefinitions.Id.AttackMain)
            .SetMaxAttacksNumber(1)
            .AddToDB();

        var conditionBladeSurge = ConditionDefinitionBuilder
            .Create($"Condition{Name}BladeSurge")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionHasted)
            .SetSilent(Silent.WhenRemoved)
            .SetPossessive()
            .SetSpecialDuration(DurationType.Round, 0, TurnOccurenceType.StartOfTurn)
            .AddFeatures(additionalActionBladeSurge, movementAffinityBladeSurge)
            .AddSpecialInterruptions(ConditionInterruption.AnyBattleTurnEnd)
            .AddToDB();

        var featureBladeSurge = FeatureDefinitionBuilder
            .Create($"Feature{Name}BladeSurge")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        // LEVEL 17 - Blade Storm

        var featureBladeStorm = FeatureDefinitionBuilder
            .Create($"Feature{Name}BladeStorm")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        featureBladeStorm.AddCustomSubFeatures(new OnReducedToZeroHpByMeBladeStorm(powerHailOfBlades));

        // MAIN

        featureBladeBond.AddCustomSubFeatures(
            new ReturningWeapon(IsBladeCallerWeapon),
            new ModifyWeaponAttackModeBladeBond(),
            new CustomBehaviorBladeMark(conditionBladeMark, conditionBladeSurge, powerHailOfBlades));

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(Name).SetGuiPresentation(Category.Subclass, CharacterSubclassDefinitions.RangerSwiftBlade)
            .AddFeaturesAtLevel(3, featureBladeBond, featureSetBladeMark)
            .AddFeaturesAtLevel(9, featureSetHailOfBlades)
            .AddFeaturesAtLevel(13, featureBladeSurge)
            .AddFeaturesAtLevel(17, featureBladeStorm)
            .AddToDB();
    }

    internal override CharacterClassDefinition Klass => CharacterClassDefinitions.Rogue;

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceRogueRoguishArchetypes;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }

    //
    // Blade Bond
    //

    private sealed class ModifyWeaponAttackModeBladeBond : IModifyWeaponAttackMode
    {
        public void ModifyAttackMode(RulesetCharacter character, RulesetAttackMode attackMode)
        {
            if (!IsBladeCallerWeapon(attackMode, null, character))
            {
                return;
            }

            attackMode.AddAttackTagAsNeeded(TagsDefinitions.MagicalWeapon);
            attackMode.closeRange += 4;
            attackMode.maxRange += 4;
        }
    }

    //
    // Blade Mark
    //

    private sealed class RogueClassHolder : IClassHoldingFeature
    {
        public CharacterClassDefinition Class => CharacterClassDefinitions.Rogue;
    }

    private sealed class CustomBehaviorBladeMark(
        ConditionDefinition conditionBladeMark,
        ConditionDefinition conditionBladeSurge,
        FeatureDefinitionPower powerHailOfBlades)
        : IPhysicalAttackInitiatedByMe, IPhysicalAttackFinishedByMe
    {
        private BladeMarkStatus _bladeMarkStatus;

        public IEnumerator OnPhysicalAttackFinishedByMe(
            GameLocationBattleManager battleManager,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RulesetAttackMode attackerAttackMode,
            RollOutcome attackRollOutcome,
            int damageAmount)
        {
            var rulesetDefender = defender.RulesetCharacter;

            // ALWAYS remove Blade Mark condition
            if (rulesetDefender is { isDeadOrDyingOrUnconscious: false })
            {
                rulesetDefender.RemoveAllConditionsOfCategoryAndType(
                    AttributeDefinitions.TagEffect, conditionBladeMark.Name);
            }

            // exit earlier if not a hit
            if (attackRollOutcome is not (RollOutcome.Success or RollOutcome.CriticalSuccess))
            {
                yield break;
            }

            var rulesetAttacker = attacker.RulesetCharacter;

            if (_bladeMarkStatus == BladeMarkStatus.With)
            {
                var classLevel = rulesetAttacker.GetClassLevel(CharacterClassDefinitions.Rogue);

                // inflict Blade Surge
                if (classLevel >= 13 && !rulesetAttacker.HasAnyConditionOfType(conditionBladeSurge.Name))
                {
                    rulesetAttacker.InflictCondition(
                        conditionBladeSurge.Name,
                        conditionBladeSurge.DurationType,
                        conditionBladeSurge.DurationParameter,
                        conditionBladeSurge.TurnOccurence,
                        AttributeDefinitions.TagEffect,
                        rulesetAttacker.guid,
                        rulesetAttacker.CurrentFaction.Name,
                        1,
                        conditionBladeSurge.Name,
                        0,
                        0,
                        0);
                }

                // offer Hail of Blades
                if (rulesetAttacker.IsToggleEnabled((ActionDefinitions.Id)ExtraActionId.HailOfBladesToggle))
                {
                    yield return HandleHailOfBlades(action, battleManager, attacker);
                }
            }

            // inflict Blade Mark condition
            if (_bladeMarkStatus != BladeMarkStatus.Without
                || !attacker.OnceInMyTurnIsValid(BladeMark))
            {
                yield break;
            }

            attacker.UsedSpecialFeatures.TryAdd(BladeMark, 1);

            rulesetDefender.InflictCondition(
                conditionBladeMark.Name,
                conditionBladeMark.DurationType,
                conditionBladeMark.DurationParameter,
                conditionBladeMark.TurnOccurence,
                AttributeDefinitions.TagEffect,
                rulesetAttacker.guid,
                rulesetAttacker.CurrentFaction.Name,
                1,
                conditionBladeMark.Name,
                0,
                0,
                0);
        }

        public IEnumerator OnPhysicalAttackInitiatedByMe(
            GameLocationBattleManager __instance,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier attackModifier,
            RulesetAttackMode attackerAttackMode)
        {
            _bladeMarkStatus = BladeMarkStatus.Invalid;

            if (!IsBladeCallerWeapon(attackerAttackMode, null, null))
            {
                yield break;
            }

            var rulesetDefender = defender.RulesetCharacter;

            if (rulesetDefender is not { IsDeadOrDyingOrUnconscious: false })
            {
                yield break;
            }

            if (rulesetDefender.HasAnyConditionOfType(conditionBladeMark.Name))
            {
                _bladeMarkStatus = BladeMarkStatus.With;

                yield break;
            }

            _bladeMarkStatus = BladeMarkStatus.Without;
        }

        private IEnumerator HandleHailOfBlades(
            CharacterAction action,
            GameLocationBattleManager battleManager,
            GameLocationCharacter attacker)
        {
            var rulesetAttacker = attacker.RulesetCharacter;

            if (!attacker.CanReact() || !rulesetAttacker.CanUsePower(powerHailOfBlades))
            {
                yield break;
            }

            var manager = ServiceRepository.GetService<IGameLocationActionService>() as GameLocationActionManager;

            if (manager == null)
            {
                yield break;
            }

            var reactionParams =
                new CharacterActionParams(attacker, (ActionDefinitions.Id)ExtraActionId.DoNothingReaction)
                {
                    StringParameter = $"Reaction/&CustomReaction{Name}HailOfBladesReactDescription"
                };
            var previousReactionCount = manager.PendingReactionRequestGroups.Count;
            var reactionRequest = new ReactionRequestCustom($"{Name}HailOfBlades", reactionParams);

            manager.AddInterruptRequest(reactionRequest);

            yield return battleManager.WaitForReactions(attacker, manager, previousReactionCount);

            if (!reactionParams.ReactionValidated)
            {
                yield break;
            }

            rulesetAttacker.UpdateUsageForPower(powerHailOfBlades, powerHailOfBlades.CostPerUse);

            var implementationManagerService =
                ServiceRepository.GetService<IRulesetImplementationService>() as RulesetImplementationManager;

            var usablePower = PowerProvider.Get(powerHailOfBlades, rulesetAttacker);
            var actionParams = new CharacterActionParams(attacker, ActionDefinitions.Id.SpendPower)
            {
                RulesetEffect = implementationManagerService
                    //CHECK: no need for AddAsActivePowerToSource
                    .MyInstantiateEffectPower(rulesetAttacker, usablePower, false),
                UsablePower = usablePower,
                targetCharacters =
                    battleManager.Battle.GetContenders(attacker, hasToPerceiveTarget: true, isWithinXCells: 3)
            };

            // different follow up pattern [not adding to ResultingActions] as it doesn't work after a reaction
            ServiceRepository.GetService<ICommandService>()?.ExecuteAction(actionParams, null, false);
        }

        private enum BladeMarkStatus
        {
            Invalid,
            With,
            Without
        }
    }

    //
    // Blade Storm
    //

    private sealed class OnReducedToZeroHpByMeBladeStorm(FeatureDefinitionPower powerHailOfBlades)
        : IOnReducedToZeroHpByMe
    {
        public IEnumerator HandleReducedToZeroHpByMe(
            GameLocationCharacter attacker,
            GameLocationCharacter downedCreature,
            RulesetAttackMode attackMode,
            RulesetEffect activeEffect)
        {
            var rulesetAttacker = attacker.RulesetCharacter;

            if (rulesetAttacker.CanUsePower(powerHailOfBlades))
            {
                yield break;
            }

            var usablePower = PowerProvider.Get(powerHailOfBlades, rulesetAttacker);

            rulesetAttacker.RepayPowerUse(usablePower);
        }
    }
}
