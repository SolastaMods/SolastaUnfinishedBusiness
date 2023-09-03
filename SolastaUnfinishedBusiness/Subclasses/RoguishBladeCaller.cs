using System.Collections;
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
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionActionAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.WeaponTypeDefinitions;
using static SolastaUnfinishedBusiness.Api.Helpers.EffectHelpers;

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
            .SetCustomSubFeatures(new RogueClassHolder())
            .AddToDB();

        var combatAffinityBladeMark = FeatureDefinitionCombatAffinityBuilder
            .Create($"CombatAffinity{Name}BladeMark")
            .SetGuiPresentation($"Condition{Name}BladeMark", Category.Condition)
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
                    .SetDurationData(DurationType.Instantaneous)
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 3, TargetType.Individuals)
                    .SetSavingThrowData(false, AttributeDefinitions.Dexterity, false,
                        EffectDifficultyClassComputation.AbilityScoreAndProficiency, AttributeDefinitions.Dexterity, 8)
                    .SetParticleEffectParameters(SpellDefinitions.ShadowDagger)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.HalfDamage)
                            .SetDamageForm(DamageTypeForce, 0, DieType.D6)
                            .SetDiceAdvancement(LevelSourceType.ClassLevel, 1, 1, 2)
                            .Build(),
                        EffectFormBuilder.ConditionForm(conditionBladeMark))
                    .Build())
            .AddToDB();

        var actionAffinityHailOfBladesToggle = FeatureDefinitionActionAffinityBuilder
            .Create(ActionAffinitySorcererMetamagicToggle, "ActionAffinityHailOfBladesToggle")
            .SetGuiPresentationNoContent(true)
            .SetAuthorizedActions((ActionDefinitions.Id)ExtraActionId.HailOfBladesToggle)
            .SetCustomSubFeatures(
                new ValidatorsDefinitionApplication(ValidatorsCharacter.HasAvailablePowerUsage(powerHailOfBlades)))
            .AddToDB();

        var featureSetHailOfBlades = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}HailOfBlades")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(actionAffinityHailOfBladesToggle, powerHailOfBlades)
            .AddToDB();

        // LEVEL 13 - Blade Surge

        var movementAffinityBladeSurge = FeatureDefinitionMovementAffinityBuilder
            .Create($"MovementAffinity{Name}BladeSurge")
            .SetGuiPresentationNoContent(true)
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

        featureBladeStorm.SetCustomSubFeatures(new OnTargetReducedToZeroHpBladeStorm(powerHailOfBlades));

        // MAIN

        featureBladeBond.SetCustomSubFeatures(
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

            attackMode.AttackTags.TryAdd(TagsDefinitions.MagicalWeapon);
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

    private sealed class CustomBehaviorBladeMark : IPhysicalAttackInitiatedByMe, IPhysicalAttackFinishedByMe
    {
        private readonly ConditionDefinition _conditionBladeMark;
        private readonly ConditionDefinition _conditionBladeSurge;
        private readonly FeatureDefinitionPower _powerHailOfBlades;
        private bool _targetWithBladeMarkHit;

        public CustomBehaviorBladeMark(
            ConditionDefinition conditionBladeMark,
            ConditionDefinition conditionBladeSurge,
            FeatureDefinitionPower powerHailOfBlades)
        {
            _conditionBladeMark = conditionBladeMark;
            _conditionBladeSurge = conditionBladeSurge;
            _powerHailOfBlades = powerHailOfBlades;
        }

        public IEnumerator OnAttackFinishedByMe(
            GameLocationBattleManager battleManager,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RulesetAttackMode attackerAttackMode,
            RollOutcome attackRollOutcome,
            int damageAmount)
        {
            var rulesetDefender = defender.RulesetCharacter;
            var rulesetAttacker = attacker.RulesetCharacter;
            var classLevel = rulesetAttacker.GetClassLevel(CharacterClassDefinitions.Rogue);

            if (_targetWithBladeMarkHit)
            {
                // remove Blade Mark condition
                if (rulesetDefender is { isDeadOrDyingOrUnconscious: false })
                {
                    rulesetDefender.RemoveAllConditionsOfType(_conditionBladeMark.Name);
                }

                if (classLevel >= 13 &&
                    attackRollOutcome is RollOutcome.Success or RollOutcome.CriticalSuccess &&
                    !rulesetAttacker.HasAnyConditionOfType(_conditionBladeSurge.Name))
                {
                    rulesetAttacker.InflictCondition(
                        _conditionBladeSurge.Name,
                        _conditionBladeSurge.DurationType,
                        _conditionBladeSurge.DurationParameter,
                        _conditionBladeSurge.TurnOccurence,
                        AttributeDefinitions.TagCombat,
                        rulesetAttacker.guid,
                        rulesetAttacker.CurrentFaction.Name,
                        1,
                        null,
                        0,
                        0,
                        0);
                }

                if (attackRollOutcome is RollOutcome.Success or RollOutcome.CriticalSuccess &&
                    rulesetAttacker.IsToggleEnabled((ActionDefinitions.Id)ExtraActionId.HailOfBladesToggle))
                {
                    yield return HandleHailOfBlades(battleManager, attacker, defender);
                }
            }

            // inflict Blade Mark condition
            if (_targetWithBladeMarkHit ||
                attackRollOutcome is not (RollOutcome.Success or RollOutcome.CriticalSuccess) ||
                rulesetDefender is not { isDeadOrDyingOrUnconscious: false } ||
                !attacker.OnceInMyTurnIsValid(BladeMark))
            {
                yield break;
            }

            attacker.UsedSpecialFeatures.TryAdd(BladeMark, 1);

            rulesetDefender.InflictCondition(
                _conditionBladeMark.Name,
                _conditionBladeMark.DurationType,
                _conditionBladeMark.DurationParameter,
                _conditionBladeMark.TurnOccurence,
                AttributeDefinitions.TagCombat,
                rulesetAttacker.guid,
                rulesetAttacker.CurrentFaction.Name,
                1,
                null,
                0,
                0,
                0);
        }

        public IEnumerator OnAttackInitiatedByMe(
            GameLocationBattleManager __instance,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier attackModifier,
            RulesetAttackMode attackerAttackMode)
        {
            _targetWithBladeMarkHit = false;

            if (!IsBladeCallerWeapon(attackerAttackMode, null, null))
            {
                yield break;
            }

            var rulesetDefender = defender.RulesetCharacter;

            if (rulesetDefender is not { IsDeadOrDyingOrUnconscious: false })
            {
                yield break;
            }

            if (!rulesetDefender.HasAnyConditionOfType(_conditionBladeMark.Name))
            {
                yield break;
            }

            _targetWithBladeMarkHit = true;
        }

        private IEnumerator HandleHailOfBlades(
            GameLocationBattleManager battleManager,
            GameLocationCharacter attacker,
            GameLocationCharacter defender)
        {
            var rulesetAttacker = attacker.RulesetCharacter;

            if (!attacker.CanReact() || !rulesetAttacker.CanUsePower(_powerHailOfBlades))
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

            var usablePower = UsablePowersProvider.Get(_powerHailOfBlades, rulesetAttacker);
            var effectPower = ServiceRepository.GetService<IRulesetImplementationService>()
                .InstantiateEffectPower(rulesetAttacker, usablePower, false)
                .AddAsActivePowerToSource();

            rulesetAttacker.UsePower(usablePower);

            foreach (var target in Gui.Battle.GetOpposingContenders(rulesetAttacker.Side)
                         .Where(x => x.RulesetCharacter is { IsDeadOrDyingOrUnconscious: false } &&
                                     battleManager.IsWithinXCells(x, defender, 3))
                         .ToList())
            {
                StartVisualEffect(attacker, defender, SpellDefinitions.ShadowDagger);
                StartVisualEffect(attacker, defender, SpellDefinitions.ShadowDagger, EffectType.Effect);
                effectPower.ApplyEffectOnCharacter(target.RulesetCharacter, true, target.LocationPosition);
            }
        }
    }

    //
    // Blade Storm
    //

    private sealed class OnTargetReducedToZeroHpBladeStorm : IOnTargetReducedToZeroHp
    {
        private readonly FeatureDefinitionPower _powerHailOfBlades;

        public OnTargetReducedToZeroHpBladeStorm(FeatureDefinitionPower powerHailOfBlades)
        {
            _powerHailOfBlades = powerHailOfBlades;
        }

        public IEnumerator HandleCharacterReducedToZeroHp(
            GameLocationCharacter attacker,
            GameLocationCharacter downedCreature,
            RulesetAttackMode attackMode,
            RulesetEffect activeEffect)
        {
            var rulesetAttacker = attacker.RulesetCharacter;

            if (rulesetAttacker.CanUsePower(_powerHailOfBlades))
            {
                yield break;
            }

            var usablePower = UsablePowersProvider.Get(_powerHailOfBlades, rulesetAttacker);

            usablePower.RepayUse();
        }
    }
}
