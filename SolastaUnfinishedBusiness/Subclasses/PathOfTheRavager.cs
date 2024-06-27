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
using SolastaUnfinishedBusiness.Properties;
using SolastaUnfinishedBusiness.Validators;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionActionAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;

namespace SolastaUnfinishedBusiness.Subclasses;

[UsedImplicitly]
public sealed class PathOfTheRavager : AbstractSubclass
{
    private const string Name = "PathOfTheRavager";

    public PathOfTheRavager()
    {
        // Frenzy

        var additionalDamageBrutalStrike =
            FeatureDefinitionAdditionalDamageBuilder
                .Create($"AdditionalDamage{Name}Frenzy")
                .SetGuiPresentation(Category.Feature)
                .SetNotificationTag("Frenzy")
                .SetDamageDice(DieType.D6, 2)
                .SetAdditionalDamageType(AdditionalDamageType.SameAsBaseDamage)
                .SetAdvancement(AdditionalDamageAdvancement.ClassLevel,
                    DiceByRankBuilder.InterpolateDiceByRankTable(1, 20, (3, 2), (9, 3), (16, 4)))
                .SetFrequencyLimit(FeatureLimitedUsage.OnceInMyTurn)
                .SetRequiredProperty(RestrictedContextRequiredProperty.Weapon)
                .AddCustomSubFeatures(
                    ModifyAdditionalDamageClassLevelBarbarian.Instance,
                    new ValidateContextInsteadOfRestrictedProperty((_, _, character, _, _, mode, _) => (
                        OperationType.Set,
                        mode is { AbilityScore: AttributeDefinitions.Strength } &&
                        character.HasConditionOfTypeOrSubType(ConditionRaging) &&
                        character.HasConditionOfCategoryAndType(
                            AttributeDefinitions.TagCombat, ConditionReckless))))
                .AddToDB();

        // Intimidating Presence

        var powerIntimidatingPresence = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}IntimidatingPresence")
            .SetGuiPresentation(Category.Feature,
                Sprites.GetSprite("IntimidatingPresence", Resources.PowerDreadfulPresence, 256, 128))
            .SetUsesFixed(ActivationTime.BonusAction, RechargeRate.LongRest)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Minute, 1)
                    .SetTargetingData(Side.Enemy, RangeType.Self, 0, TargetType.Sphere, 6)
                    .SetSavingThrowData(false, AttributeDefinitions.Wisdom, true,
                        EffectDifficultyClassComputation.AbilityScoreAndProficiency, AttributeDefinitions.Strength, 8)
                    .SetParticleEffectParameters(PowerBerserkerIntimidatingPresence)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.Negates, TurnOccurenceType.EndOfTurn, true)
                            .SetConditionForm(
                                ConditionDefinitions.ConditionFrightened,
                                ConditionForm.ConditionOperation.Add)
                            .Build())
                    .Build())
            .AddToDB();

        powerIntimidatingPresence.AddCustomSubFeatures(
            new ValidatorsValidatePowerUse(c =>
                c.HasConditionOfTypeOrSubType(ConditionRaging) &&
                c.GetRemainingPowerUses(powerIntimidatingPresence) > 0));

        var powerIntimidatingPresenceRageCost = FeatureDefinitionPowerBuilder
            .Create(powerIntimidatingPresence, $"Power{Name}IntimidatingPresenceRageCost")
            .SetUsesFixed(ActivationTime.BonusAction, RechargeRate.RagePoints)
            .AddToDB();

        powerIntimidatingPresenceRageCost.AddCustomSubFeatures(
            new ValidatorsValidatePowerUse(c =>
                c.HasConditionOfTypeOrSubType(ConditionRaging) &&
                c.GetRemainingPowerUses(powerIntimidatingPresence) == 0),
            new MagicEffectFinishedByMeIntimidatingPresence(
                powerIntimidatingPresence, powerIntimidatingPresenceRageCost));

        var featureSetIntimidatingPresence = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}IntimidatingPresence")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(
                powerIntimidatingPresence, powerIntimidatingPresenceRageCost)
            .AddToDB();

        // MAIN

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Subclass, CharacterSubclassDefinitions.PathBerserker)
            .AddFeaturesAtLevel(3, additionalDamageBrutalStrike)
            .AddFeaturesAtLevel(6, PowerBerserkerMindlessRage)
            .AddFeaturesAtLevel(10, ActionAffinityRetaliation)
            .AddFeaturesAtLevel(14, featureSetIntimidatingPresence)
            .AddToDB();
    }

    internal override CharacterClassDefinition Klass => CharacterClassDefinitions.Barbarian;

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceBarbarianPrimalPath;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }

    private sealed class MagicEffectFinishedByMeIntimidatingPresence(
        FeatureDefinitionPower powerLongRest,
        FeatureDefinitionPower powerRageCost) : IMagicEffectFinishedByMe
    {
        public IEnumerator OnMagicEffectFinishedByMe(
            CharacterActionMagicEffect action,
            GameLocationCharacter attacker,
            // ReSharper disable once InconsistentNaming
            List<GameLocationCharacter> _targets)
        {
            if (ServiceRepository.GetService<IGameLocationBattleService>() is not GameLocationBattleManager
                {
                    IsBattleInProgress: true
                } battleManager)
            {
                yield break;
            }

            if (action is not CharacterActionUsePower characterActionUsePower ||
                (characterActionUsePower.activePower.PowerDefinition != PowerBarbarianRageStart &&
                 characterActionUsePower.activePower.PowerDefinition.OverriddenPower != PowerBarbarianRageStart))
            {
                yield break;
            }

            var rulesetCharacter = attacker.RulesetCharacter;

            var power = rulesetCharacter.GetRemainingPowerUses(powerLongRest) > 0
                ? powerLongRest
                : rulesetCharacter.GetRemainingPowerUses(powerRageCost) > 0
                    ? powerRageCost
                    : null;

            if (!power)
            {
                yield break;
            }

            var actionService = ServiceRepository.GetService<IGameLocationActionService>();
            var implementationManager =
                ServiceRepository.GetService<IRulesetImplementationService>() as RulesetImplementationManager;

            var usablePower = PowerProvider.Get(power, rulesetCharacter);
            var targets = battleManager.Battle
                .GetContenders(attacker, null, true, withinRange: 6);
            var actionModifiers = new List<ActionModifier>();

            for (var i = 0; i < targets.Count; i++)
            {
                actionModifiers.Add(new ActionModifier());
            }

            var reactionParams = new CharacterActionParams(attacker, ActionDefinitions.Id.PowerNoCost)
            {
                ActionModifiers = actionModifiers,
                StringParameter = "IntimidatingPresence",
                RulesetEffect = implementationManager
                    .MyInstantiateEffectPower(rulesetCharacter, usablePower, false),
                UsablePower = usablePower,
                targetCharacters = targets
            };
            var count = actionService.PendingReactionRequestGroups.Count;

            actionService.ReactToUsePower(reactionParams, "UsePower", attacker);

            yield return battleManager.WaitForReactions(attacker, actionService, count);

            if (!reactionParams.ReactionValidated)
            {
                yield break;
            }

            if (power == powerRageCost)
            {
                rulesetCharacter.SpendRagePoint();
            }
        }
    }
}
