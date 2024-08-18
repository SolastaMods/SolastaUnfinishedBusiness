using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Behaviors;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Properties;
using SolastaUnfinishedBusiness.Validators;
using UnityEngine.AddressableAssets;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;

namespace SolastaUnfinishedBusiness.Subclasses;

[UsedImplicitly]
public sealed class RoguishRavenScion : AbstractSubclass
{
    private const string Name = "RavenScion";

    public RoguishRavenScion()
    {
        // LEVEL 03

        // Ranged Specialist

        var featureSetRavenSharpShooter = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}RangedSpecialist")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(
                // proficient with all two handed range weapons
                FeatureDefinitionProficiencyBuilder
                    .Create($"Proficiency{Name}RangeWeapon")
                    .SetGuiPresentationNoContent(true)
                    .SetProficiencies(
                        ProficiencyType.Weapon,
                        WeaponTypeDefinitions.HeavyCrossbowType.Name,
                        WeaponTypeDefinitions.LongbowType.Name)
                    .AddToDB(),
                FeatureDefinitionProficiencyBuilder
                    .Create($"Proficiency{Name}FightingStyle")
                    .SetGuiPresentationNoContent(true)
                    .SetProficiencies(ProficiencyType.FightingStyle, "Archery")
                    .AddToDB())
            .AddToDB();

        // Sniper's Aim

        var additionalDamageSniperAim = FeatureDefinitionAdditionalDamageBuilder
            .Create($"AdditionalDamage{Name}SniperAim")
            .SetGuiPresentation(Category.Feature)
            .SetNotificationTag("SniperAim")
            .SetFrequencyLimit(FeatureLimitedUsage.OncePerTurn)
            .SetDamageValueDetermination(ExtraAdditionalDamageValueDetermination.FlatWithProgression)
            .SetAdvancement(AdditionalDamageAdvancement.ClassLevel)
            .SetRequiredProperty(RestrictedContextRequiredProperty.Weapon)
            .AddCustomSubFeatures(
                ClassHolder.Rogue,
                new ValidateContextInsteadOfRestrictedProperty(
                    (_, _, _, _, _, mode, _) => (OperationType.Set, ValidatorsWeapon.IsTwoHandedRanged(mode))))
            .AddToDB();

        // LEVEL 09

        // Heart-Seeking Shot

        var powerHeartSeekingShot = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}HeartSeekingShot")
            .SetGuiPresentation(Category.Feature, PowerMartialCommanderRousingShout)
            .SetUsesFixed(ActivationTime.NoCost, RechargeRate.ShortRest)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Minute, 1, TurnOccurenceType.StartOfTurn)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetEffectForms(EffectFormBuilder.ConditionForm(
                        ConditionDefinitionBuilder
                            .Create($"Condition{Name}HeartSeekingShot")
                            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionGuided)
                            .SetPossessive()
                            .SetSpecialInterruptions(ConditionInterruption.Attacks)
                            .AddCustomSubFeatures(new TryAlterOutcomeAttackDeadlyAimHeartSeekingShot())
                            .AddToDB()))
                    .SetParticleEffectParameters(PowerPactChainImp)
                    .Build())
            .AddToDB();

        powerHeartSeekingShot.EffectDescription.EffectParticleParameters.conditionStartParticleReference =
            new AssetReference();
        powerHeartSeekingShot.EffectDescription.EffectParticleParameters.conditionEndParticleReference =
            new AssetReference();

        // LEVEL 13

        // Deadly Focus

        var powerDeadlyFocus = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}DeadlyFocus")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.NoCost, RechargeRate.ShortRest)
            .AddToDB();

        powerDeadlyFocus.AddCustomSubFeatures(
            ModifyPowerVisibility.Hidden,
            new TryAlterOutcomeAttackDeadlyFocus(powerDeadlyFocus));

        // LEVEL 17

        // Killing Spree

        var featureRavenKillingSpree = FeatureDefinitionBuilder
            .Create($"Feature{Name}KillingSpree")
            .SetGuiPresentation(Category.Feature)
            .AddCustomSubFeatures(
                // bonus range attack from main and can use sniper's aim again during this turn
                new OnReducedToZeroHpByMeKillingSpree(
                    ConditionDefinitionBuilder
                        .Create($"Condition{Name}KillingSpree")
                        .SetGuiPresentationNoContent(true)
                        .SetSilent(Silent.WhenAddedOrRemoved)
                        .SetFeatures(FeatureDefinitionAdditionalActionBuilder
                            .Create($"AdditionalAction{Name}KillingSpree")
                            .SetGuiPresentationNoContent(true)
                            .SetActionType(ActionDefinitions.ActionType.Main)
                            .SetRestrictedActions(ActionDefinitions.Id.AttackMain)
                            .SetMaxAttacksNumber(1)
                            .AddToDB())
                        .AddToDB()))
            .AddToDB();

        // MAIN

        Subclass = CharacterSubclassDefinitionBuilder
            .Create($"Roguish{Name}")
            .SetGuiPresentation(Category.Subclass, Sprites.GetSprite(Name, Resources.RoguishRaven, 256))
            .AddFeaturesAtLevel(3, featureSetRavenSharpShooter, additionalDamageSniperAim)
            .AddFeaturesAtLevel(9, powerHeartSeekingShot)
            .AddFeaturesAtLevel(13, powerDeadlyFocus)
            .AddFeaturesAtLevel(17, featureRavenKillingSpree)
            .AddToDB();
    }

    internal override CharacterClassDefinition Klass => CharacterClassDefinitions.Rogue;

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceRogueRoguishArchetypes;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }

    //
    // Killing Spree
    //

    private sealed class OnReducedToZeroHpByMeKillingSpree(ConditionDefinition condition) : IOnReducedToZeroHpByMe
    {
        public IEnumerator HandleReducedToZeroHpByMe(
            GameLocationCharacter attacker,
            GameLocationCharacter downedCreature,
            RulesetAttackMode attackMode,
            RulesetEffect activeEffect)
        {
            var rulesetAttacker = attacker.RulesetCharacter;

            if (!attacker.IsMyTurn() ||
                !ValidatorsWeapon.IsTwoHandedRanged(attackMode) ||
                rulesetAttacker.HasConditionOfCategoryAndType(AttributeDefinitions.TagEffect, condition.Name))
            {
                yield break;
            }

            attacker.UsedSpecialFeatures.Remove($"AdditionalDamage{Name}SniperAim");

            rulesetAttacker.InflictCondition(
                condition.Name,
                DurationType.Round,
                0,
                TurnOccurenceType.EndOfTurn,
                AttributeDefinitions.TagEffect,
                rulesetAttacker.guid,
                rulesetAttacker.CurrentFaction.Name,
                1,
                condition.Name,
                0,
                0,
                0);
        }
    }

    //
    // Heart-Seeking Shot
    //

    private class TryAlterOutcomeAttackDeadlyAimHeartSeekingShot : ITryAlterOutcomeAttack
    {
        public int HandlerPriority => -10;

        public IEnumerator OnTryAlterOutcomeAttack(
            GameLocationBattleManager battle,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            GameLocationCharacter helper,
            ActionModifier attackModifier,
            RulesetAttackMode attackMode,
            RulesetEffect rulesetEffect)
        {
            if (action.AttackRollOutcome != RollOutcome.Success ||
                helper != attacker ||
                !ValidatorsWeapon.IsTwoHandedRanged(attackMode))
            {
                yield break;
            }

            action.AttackRoll = 20;
            action.AttackRollOutcome = RollOutcome.CriticalSuccess;
            action.AttackSuccessDelta = 0;
        }
    }

    //
    // Deadly Focus
    //

    private class TryAlterOutcomeAttackDeadlyFocus(FeatureDefinitionPower powerDeadlyFocus) : ITryAlterOutcomeAttack
    {
        public int HandlerPriority => -10;

        public IEnumerator OnTryAlterOutcomeAttack(
            GameLocationBattleManager battleManager,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            GameLocationCharacter helper,
            ActionModifier attackModifier,
            RulesetAttackMode attackMode,
            RulesetEffect rulesetEffect)
        {
            var rulesetHelper = helper.RulesetCharacter;
            var usablePower = PowerProvider.Get(powerDeadlyFocus, rulesetHelper);

            if (action.AttackRollOutcome is not (RollOutcome.Failure or RollOutcome.CriticalFailure) ||
                helper != attacker ||
                !ValidatorsWeapon.IsTwoHandedRanged(attackMode) ||
                rulesetHelper.GetRemainingUsesOfPower(usablePower) == 0)
            {
                yield break;
            }

            yield return attacker.MyReactToSpendPower(
                usablePower,
                attacker,
                "RavenScionDeadlyFocus",
                reactionValidated: ReactionValidated,
                battleManager: battleManager);

            yield break;

            void ReactionValidated()
            {
                usablePower.Consume();

                var totalRoll = (action.AttackRoll + attackMode.ToHitBonus).ToString();
                var rollCaption = action.AttackRollOutcome == RollOutcome.CriticalFailure
                    ? "Feedback/&RollAttackCriticalFailureTitle"
                    : "Feedback/&RollAttackFailureTitle";

                rulesetHelper.LogCharacterUsedPower(
                    powerDeadlyFocus,
                    $"Feedback/&Trigger{Name}RerollLine",
                    false,
                    (ConsoleStyleDuplet.ParameterType.Base, $"{action.AttackRoll}+{attackMode.ToHitBonus}"),
                    (ConsoleStyleDuplet.ParameterType.FailedRoll, Gui.Format(rollCaption, totalRoll)));

                var advantageTrends =
                    new List<TrendInfo>
                    {
                        new(1, FeatureSourceType.CharacterFeature, powerDeadlyFocus.Name, powerDeadlyFocus)
                    };

                attackModifier.AttackAdvantageTrends.SetRange(advantageTrends);

                var roll = rulesetHelper.RollAttack(
                    attackMode.toHitBonus,
                    defender.RulesetActor,
                    attackMode.sourceDefinition,
                    attackModifier.attackToHitTrends,
                    false,
                    attackModifier.AttackAdvantageTrends,
                    attackMode.ranged,
                    false,
                    attackModifier.attackRollModifier,
                    out var outcome,
                    out var successDelta,
                    -1,
                    true);

                action.AttackRollOutcome = outcome;
                action.AttackSuccessDelta = successDelta;
                action.AttackRoll = roll;
            }
        }
    }
}
