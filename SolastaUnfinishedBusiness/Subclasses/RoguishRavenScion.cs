using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Behaviors.Specific;
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
        //
        // LEVEL 03
        //

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
            .SetDamageValueDetermination(ExtraAdditionalDamageValueDetermination.CharacterLevel)
            .SetRequiredProperty(RestrictedContextRequiredProperty.Weapon)
            .AddCustomSubFeatures(
                new RogueModifyAdditionalDamageClassLevelHolder(),
                new ValidateContextInsteadOfRestrictedProperty(
                    (_, _, _, _, _, mode, _) => (OperationType.Set, ValidatorsWeapon.IsTwoHandedRanged(mode))))
            .AddToDB();

        //
        // LEVEL 09
        //

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
                            .AddCustomSubFeatures(new ModifyAttackOutcomeHeartSeekingShot())
                            .AddToDB()))
                    .SetParticleEffectParameters(PowerPactChainImp)
                    .Build())
            .AddToDB();

        powerHeartSeekingShot.EffectDescription.EffectParticleParameters.conditionStartParticleReference =
            new AssetReference();
        powerHeartSeekingShot.EffectDescription.EffectParticleParameters.conditionEndParticleReference =
            new AssetReference();

        // Killing Spree

        var featureRavenKillingSpree = FeatureDefinitionBuilder
            .Create($"Feature{Name}KillingSpree")
            .SetGuiPresentation(Category.Feature)
            .AddCustomSubFeatures(
                // bonus range attack from main and can use sniper's aim again during this turn
                new OnReducedToZeroHpByMeKillingSpree(ConditionDefinitionBuilder
                    .Create($"Condition{Name}KillingSpree")
                    .SetGuiPresentationNoContent(true)
                    .SetSilent(Silent.WhenAddedOrRemoved)
                    .SetFeatures(FeatureDefinitionAdditionalActionBuilder
                        .Create($"AdditionalAction{Name}KillingSpree")
                        .SetGuiPresentationNoContent(true)
                        .SetActionType(ActionDefinitions.ActionType.Main)
                        .SetRestrictedActions(ActionDefinitions.Id.AttackMain)
                        .SetMaxAttacksNumber(1)
                        .AddCustomSubFeatures(ValidateAdditionalActionAttack.TwoHandedRanged)
                        .AddToDB())
                    .AddToDB()))
            .AddToDB();

        //
        // LEVEL 13
        //

        // Deadly Focus

        var powerDeadlyFocus = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}DeadlyFocus")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.Reaction, RechargeRate.ShortRest)
            .SetReactionContext(ExtraReactionContext.Custom)
            .AddToDB();

        powerDeadlyFocus.AddCustomSubFeatures(new TryAlterOutcomePhysicalAttackDeadlyAim(powerDeadlyFocus));

        //
        // LEVEL 17
        //

        // Perfect Shot

        // kept for backward compatibility
        _ = FeatureDefinitionDieRollModifierBuilder
            .Create($"DieRollModifier{Name}PerfectShot")
            .SetGuiPresentation(Category.Feature)
            .SetModifiers(RollContext.AttackDamageValueRoll, 1, 2, 1,
                "Feature/&DieRollModifierRavenPainMakerReroll")
            .AddCustomSubFeatures(new RoguishRaven.RavenRerollAnyDamageDieMarker())
            .AddToDB();

        //
        // MAIN
        //

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

    private sealed class RogueModifyAdditionalDamageClassLevelHolder : IModifyAdditionalDamageClassLevel
    {
        public CharacterClassDefinition Class => CharacterClassDefinitions.Rogue;
    }

    //
    // Killing Spree
    //

    private sealed class OnReducedToZeroHpByMeKillingSpree(
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        ConditionDefinition condition) : IOnReducedToZeroHpByMe
    {
        public IEnumerator HandleReducedToZeroHpByMe(
            GameLocationCharacter attacker,
            GameLocationCharacter downedCreature,
            RulesetAttackMode attackMode,
            RulesetEffect activeEffect)
        {
            if (activeEffect != null || !ValidatorsWeapon.IsTwoHandedRanged(attackMode))
            {
                yield break;
            }

            var rulesetAttacker = attacker.RulesetCharacter;

            if (rulesetAttacker.HasAnyConditionOfType(condition.Name))
            {
                yield break;
            }

            if (!attacker.IsMyTurn())
            {
                yield break;
            }

            attacker.UsedSpecialFeatures.Remove($"AdditionalDamage{Name}SniperAim");

            rulesetAttacker.InflictCondition(
                condition.Name,
                DurationType.Round,
                0,
                TurnOccurenceType.StartOfTurn,
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

    private class ModifyAttackOutcomeHeartSeekingShot : ITryAlterOutcomePhysicalAttack
    {
        public IEnumerator OnAttackTryAlterOutcome(
            GameLocationBattleManager instance,
            CharacterAction action,
            GameLocationCharacter attacker, 
            GameLocationCharacter target,
            ActionModifier attackModifier)
        {
            if (action.AttackRollOutcome is not (RollOutcome.Success or RollOutcome.CriticalSuccess))
            {
                yield break;
            }

            var sourceDefinition = action.ActionParams.attackMode.SourceDefinition;
            
            if (sourceDefinition is not ItemDefinition itemDefinition
                || !ValidatorsWeapon.IsTwoHandedRanged(itemDefinition))
            {
                yield break;
            }
            
            action.AttackRollOutcome = RollOutcome.CriticalSuccess;
            action.AttackSuccessDelta = 0;
        }
    }

    //
    // Deadly Focus
    //

    private class TryAlterOutcomePhysicalAttackDeadlyAim(FeatureDefinitionPower power) : ITryAlterOutcomePhysicalAttack
    {
        public IEnumerator OnAttackTryAlterOutcome(
            GameLocationBattleManager battle,
            CharacterAction action,
            GameLocationCharacter me,
            GameLocationCharacter target,
            ActionModifier attackModifier)
        {
            var attackMode = action.actionParams.attackMode;
            var rulesetAttacker = me.RulesetCharacter;

            if (rulesetAttacker.GetRemainingPowerCharges(power) <= 0 ||
                !ValidatorsWeapon.IsTwoHandedRanged(attackMode) || !me.CanPerceiveTarget(target))
            {
                yield break;
            }

            var manager = ServiceRepository.GetService<IGameLocationActionService>() as GameLocationActionManager;

            if (manager == null)
            {
                yield break;
            }

            var reactionParams = new CharacterActionParams(me, (ActionDefinitions.Id)ExtraActionId.DoNothingFree);
            var previousReactionCount = manager.PendingReactionRequestGroups.Count;
            var reactionRequest = new ReactionRequestCustom("RavenScionDeadlyFocus", reactionParams);

            manager.AddInterruptRequest(reactionRequest);

            yield return battle.WaitForReactions(me, manager, previousReactionCount);

            if (!reactionParams.ReactionValidated)
            {
                yield break;
            }

            rulesetAttacker.UpdateUsageForPower(power, power.CostPerUse);

            var totalRoll = (action.AttackRoll + attackMode.ToHitBonus).ToString();
            var rollCaption = action.AttackRoll == 1
                ? "Feedback/&RollCheckCriticalFailureTitle"
                : "Feedback/&CriticalAttackFailureOutcome";

            rulesetAttacker.LogCharacterUsedPower(
                power,
                $"Feedback/&Trigger{Name}RerollLine",
                false,
                (ConsoleStyleDuplet.ParameterType.Base, $"{action.AttackRoll}+{attackMode.ToHitBonus}"),
                (ConsoleStyleDuplet.ParameterType.FailedRoll, Gui.Format(rollCaption, totalRoll)));

            var advantageTrends =
                new List<TrendInfo> { new(1, FeatureSourceType.CharacterFeature, power.Name, power) };

            var roll = rulesetAttacker.RollAttack(
                attackMode.toHitBonus,
                target.RulesetCharacter,
                attackMode.sourceDefinition,
                attackModifier.attackToHitTrends,
                false,
                advantageTrends,
                attackMode.ranged,
                false,
                attackModifier.attackRollModifier,
                out var outcome,
                out var successDelta,
                -1,
                // testMode true avoids the roll to display on combat log as the original one will get there with altered results
                true);

            attackModifier.ignoreAdvantage = false;
            attackModifier.attackAdvantageTrends = advantageTrends;
            action.AttackRollOutcome = outcome;
            action.AttackSuccessDelta = successDelta;
            action.AttackRoll = roll;
        }
    }
}
