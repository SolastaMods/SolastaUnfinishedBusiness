using System.Collections;
using System.Collections.Generic;
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
using static FeatureDefinitionAttributeModifier;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;

namespace SolastaUnfinishedBusiness.Subclasses;

[UsedImplicitly]
public sealed class RoguishRaven : AbstractSubclass
{
    public RoguishRaven()
    {
        // proficient with all two handed range weapons
        // ignore cover and long range disadvantage
        var featureSetRavenSharpShooter = FeatureDefinitionFeatureSetBuilder
            .Create("FeatureSetRavenSharpShooter")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(
                FeatureDefinitionProficiencyBuilder
                    .Create("ProficiencyRavenRangeWeapon")
                    .SetGuiPresentationNoContent(true)
                    .SetProficiencies(
                        ProficiencyType.Weapon,
                        WeaponTypeDefinitions.HeavyCrossbowType.Name,
                        WeaponTypeDefinitions.LongbowType.Name)
                    .AddToDB(),
                FeatureDefinitionCombatAffinityBuilder
                    .Create("CombatAffinityRavenRangeAttack")
                    .SetGuiPresentation("FeatureSetRavenSharpShooter", Category.Feature)
                    .SetIgnoreCover()
                    .SetCustomSubFeatures(new BumpWeaponWeaponAttackRangeToMax(ValidatorsWeapon.AlwaysValid))
                    .AddToDB())
            .AddToDB();

        // killing spree 
        // bonus range attack from main and can sneak attack after killing an enemies
        var additionalActionRavenKillingSpree = FeatureDefinitionBuilder
            .Create("AdditionalActionRavenKillingSpree") //keeping old name for compatibility
            .SetGuiPresentation(Category.Feature)
            .SetCustomSubFeatures(new RefreshSneakAttackOnReducedToZeroHpKill(),
                new KillingSpree(
                    ConditionDefinitionBuilder
                        .Create("ConditionRavenKillingSpree")
                        .SetGuiPresentationNoContent(true)
                        .SetSilent(Silent.WhenAddedOrRemoved)
                        .SetFeatures(
                            FeatureDefinitionAdditionalActionBuilder
                                .Create("AdditionalActionRavenKillingSpree2")
                                .SetGuiPresentation("AdditionalActionRavenKillingSpree", Category.Feature)
                                .SetActionType(ActionDefinitions.ActionType.Main)
                                .SetRestrictedActions(ActionDefinitions.Id.AttackMain)
                                .SetMaxAttacksNumber(1)
                                .SetCustomSubFeatures(AdditionalActionAttackValidator.TwoHandedRanged)
                                .AddToDB())
                        .AddToDB()))
            .AddToDB();

        // pain maker
        // reroll any 1 when roll damage but need to use the new roll
        var dieRollModifierRavenPainMaker = FeatureDefinitionDieRollModifierBuilder
            .Create("DieRollModifierRavenPainMaker")
            .SetGuiPresentation(Category.Feature)
            .SetModifiers(RollContext.AttackDamageValueRoll, 1, 1, 1,
                "Feature/&DieRollModifierRavenPainMakerReroll")
            .SetCustomSubFeatures(new RavenRerollAnyDamageDieMarker())
            .AddToDB();

        // deadly aim
        var powerSteadyAim = FeatureDefinitionPowerBuilder
            .Create("PowerRavenDeadlyAim")
            .SetGuiPresentation("FeatureSetRavenDeadlyAim", Category.Feature, hidden: true)
            .SetUsesFixed(ActivationTime.NoCost, RechargeRate.ShortRest)
            .AddToDB();

        var featureRavenDeadlyAim = FeatureDefinitionBuilder
            .Create("FeatureRavenDeadlyAim")
            .SetGuiPresentationNoContent(true)
            .SetCustomSubFeatures(new TryAlterOutcomePhysicalAttackDeadlyAim(powerSteadyAim))
            .AddToDB();

        var featureSetRavenDeadlyAim = FeatureDefinitionFeatureSetBuilder
            .Create("FeatureSetRavenDeadlyAim")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(featureRavenDeadlyAim, powerSteadyAim)
            .AddToDB();

        // perfect Shot
        var dieRollModifierRavenPerfectShot = FeatureDefinitionDieRollModifierBuilder
            .Create("DieRollModifierRavenPerfectShot")
            .SetGuiPresentation(Category.Feature)
            .SetModifiers(RollContext.AttackDamageValueRoll, 1, 2, 1,
                "Feature/&DieRollModifierRavenPainMakerReroll")
            .SetCustomSubFeatures(new RavenRerollAnyDamageDieMarker())
            .AddToDB();

        Subclass = CharacterSubclassDefinitionBuilder
            .Create("RoguishRaven")
            .SetGuiPresentation(Category.Subclass,
                Sprites.GetSprite("RoguishRaven", Resources.RoguishRaven, 256))
            .AddFeaturesAtLevel(3,
                featureSetRavenSharpShooter,
                BuildHeartSeekingShot())
            .AddFeaturesAtLevel(9,
                additionalActionRavenKillingSpree,
                dieRollModifierRavenPainMaker)
            .AddFeaturesAtLevel(13,
                featureSetRavenDeadlyAim)
            .AddFeaturesAtLevel(17,
                dieRollModifierRavenPerfectShot)
            .AddToDB();
    }

    internal override CharacterClassDefinition Klass => CharacterClassDefinitions.Rogue;

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceRogueRoguishArchetypes;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }

    private static FeatureDefinitionFeatureSet BuildHeartSeekingShot()
    {
        var concentrationProvider = new StopPowerConcentrationProvider("HeartSeekingShot",
            "Tooltip/&HeartSeekingShotConcentration",
            Sprites.GetSprite("DeadeyeConcentrationIcon",
                Resources.DeadeyeConcentrationIcon, 64, 64));

        var conditionRavenHeartSeekingShotTrigger = ConditionDefinitionBuilder
            .Create("ConditionRavenHeartSeekingShotTrigger")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetFeatures(
                FeatureDefinitionBuilder
                    .Create("TriggerFeatureRavenHeartSeekingShot")
                    .SetGuiPresentationNoContent(true)
                    .SetCustomSubFeatures(concentrationProvider)
                    .AddToDB())
            .AddToDB();

        var validateHasTwoHandedRangedWeapon =
            new ValidateContextInsteadOfRestrictedProperty(OperationType.Set,
                ValidatorsCharacter.HasTwoHandedRangedWeapon);

        // -4 attack roll but critical threshold is 18 and deal 3d6 additional damage
        var conditionRavenHeartSeekingShot = ConditionDefinitionBuilder
            .Create("ConditionRavenHeartSeekingShot")
            .SetGuiPresentation("FeatureSetRavenHeartSeekingShot", Category.Feature)
            .SetPossessive()
            .AddFeatures(
                FeatureDefinitionAttributeModifierBuilder
                    .Create("AttributeModifierRavenHeartSeekingShotCriticalThreshold")
                    .SetGuiPresentation(Category.Feature)
                    .SetModifier(AttributeModifierOperation.Additive,
                        AttributeDefinitions.CriticalThreshold, -2)
                    .SetCustomSubFeatures(validateHasTwoHandedRangedWeapon)
                    .SetSituationalContext(SituationalContext.AttackingWithRangedWeapon)
                    .AddToDB(),
                FeatureDefinitionAttackModifierBuilder
                    .Create("AttackModifierRavenHeartSeekingShot")
                    .SetGuiPresentation(Category.Feature)
                    .SetAttackRollModifier(-4)
                    .SetCustomSubFeatures(validateHasTwoHandedRangedWeapon)
                    .SetRequiredProperty(RestrictedContextRequiredProperty.RangeWeapon)
                    .AddToDB(),
                FeatureDefinitionAdditionalDamageBuilder
                    .Create("AdditionalDamageRavenHeartSeekingShot")
                    .SetGuiPresentationNoContent(true)
                    .SetNotificationTag("HeartSeekingShot")
                    .SetDamageDice(DieType.D6, 1)
                    .SetAdvancement(AdditionalDamageAdvancement.ClassLevel, 1, 1, 4, 3)
                    .SetRequiredProperty(RestrictedContextRequiredProperty.RangeWeapon)
                    .SetCustomSubFeatures(
                        ValidatorsCharacter.HasTwoHandedRangedWeapon,
                        new HeartSeekingShotAdditionalDamageOnCritMarker(CharacterClassDefinitions.Rogue))
                    .AddToDB())
            .AddToDB();

        var deadEyeSprite = Sprites.GetSprite("DeadeyeIcon", Resources.DeadeyeIcon, 128, 64);

        var powerRavenHeartSeekingShot = FeatureDefinitionPowerBuilder
            .Create("PowerRavenHeartSeekingShot")
            .SetGuiPresentation("FeatureSetRavenHeartSeekingShot", Category.Feature, deadEyeSprite)
            .SetUsesFixed(ActivationTime.NoCost)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetDurationData(DurationType.Permanent)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(conditionRavenHeartSeekingShotTrigger,
                                ConditionForm.ConditionOperation.Add)
                            .Build(),
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(conditionRavenHeartSeekingShot, ConditionForm.ConditionOperation.Add)
                            .Build())
                    .Build())
            .SetCustomSubFeatures(new ValidatorsPowerUse(ValidatorsCharacter.HasTwoHandedRangedWeapon))
            .AddToDB();

        Global.PowersThatIgnoreInterruptions.Add(powerRavenHeartSeekingShot);

        var powerRavenTurnOffHeartSeekingShot = FeatureDefinitionPowerBuilder
            .Create("PowerRavenTurnOffHeartSeekingShot")
            .SetGuiPresentationNoContent(true)
            .SetUsesFixed(ActivationTime.NoCost)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetDurationData(DurationType.Round, 1)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(
                                conditionRavenHeartSeekingShotTrigger,
                                ConditionForm.ConditionOperation.Remove)
                            .Build(),
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(conditionRavenHeartSeekingShot, ConditionForm.ConditionOperation.Remove)
                            .Build())
                    .Build())
            .AddToDB();

        Global.PowersThatIgnoreInterruptions.Add(powerRavenTurnOffHeartSeekingShot);
        concentrationProvider.StopPower = powerRavenTurnOffHeartSeekingShot;

        return FeatureDefinitionFeatureSetBuilder
            .Create("FeatureSetRavenHeartSeekingShot")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(powerRavenHeartSeekingShot, powerRavenTurnOffHeartSeekingShot)
            .AddToDB();
    }

    // marker to reroll any damage die including sneak attack
    internal sealed class RavenRerollAnyDamageDieMarker
    {
    }

    private sealed class RefreshSneakAttackOnReducedToZeroHpKill : IOnReducedToZeroHpEnemy
    {
        public IEnumerator HandleReducedToZeroHpEnemy(
            GameLocationCharacter attacker,
            GameLocationCharacter downedCreature,
            RulesetAttackMode attackMode,
            RulesetEffect activeEffect)
        {
            if (attacker.IsOppositeSide(downedCreature.Side))
            {
                attacker.UsedSpecialFeatures.Remove(
                    FeatureDefinitionAdditionalDamages.AdditionalDamageRogueSneakAttack.Name);
            }

            yield break;
        }
    }

    private sealed class HeartSeekingShotAdditionalDamageOnCritMarker : IClassHoldingFeature
    {
        public HeartSeekingShotAdditionalDamageOnCritMarker(CharacterClassDefinition @class)
        {
            Class = @class;
        }

        public CharacterClassDefinition Class { get; }
    }

    private sealed class KillingSpree : IOnReducedToZeroHpEnemy
    {
        private readonly ConditionDefinition _condition;

        public KillingSpree(ConditionDefinition condition)
        {
            _condition = condition;
        }

        public IEnumerator HandleReducedToZeroHpEnemy(
            GameLocationCharacter attacker,
            GameLocationCharacter downedCreature,
            RulesetAttackMode attackMode,
            RulesetEffect activeEffect)
        {
            if (activeEffect != null || !ValidatorsWeapon.IsTwoHandedRanged(attackMode))
            {
                yield break;
            }

            if (attacker.RulesetCharacter.HasAnyConditionOfType(_condition.Name))
            {
                yield break;
            }

            if (Gui.Battle?.ActiveContender != attacker)
            {
                yield break;
            }

            attacker.RulesetCharacter.InflictCondition(
                _condition.Name,
                DurationType.Round,
                0,
                TurnOccurenceType.EndOfTurn,
                AttributeDefinitions.TagCombat,
                attacker.RulesetCharacter.guid,
                attacker.RulesetCharacter.CurrentFaction.Name,
                1,
                null,
                0,
                0,
                0);
        }
    }

    private class TryAlterOutcomePhysicalAttackDeadlyAim : ITryAlterOutcomePhysicalAttack
    {
        private readonly FeatureDefinitionPower _power;

        public TryAlterOutcomePhysicalAttackDeadlyAim(FeatureDefinitionPower power)
        {
            _power = power;
        }

        public IEnumerator OnAttackTryAlterOutcome(
            GameLocationBattleManager battle,
            CharacterAction action,
            GameLocationCharacter me,
            GameLocationCharacter target,
            ActionModifier attackModifier)
        {
            var attackMode = action.actionParams.attackMode;
            var rulesetAttacker = me.RulesetCharacter;

            if (rulesetAttacker is not { IsDeadOrDyingOrUnconscious: false } ||
                rulesetAttacker.GetRemainingPowerCharges(_power) <= 0 ||
                !attackMode.ranged)
            {
                yield break;
            }

            var manager = ServiceRepository.GetService<IGameLocationActionService>() as GameLocationActionManager;

            if (manager == null)
            {
                yield break;
            }

            var reactionParams = new CharacterActionParams(me, (ActionDefinitions.Id)ExtraActionId.DoNothingFree)
            {
                StringParameter = "Reaction/&CustomReactionRavenDeadlyAimReactDescription"
            };
            var previousReactionCount = manager.PendingReactionRequestGroups.Count;
            var reactionRequest = new ReactionRequestCustom("RavenDeadlyAim", reactionParams);

            manager.AddInterruptRequest(reactionRequest);

            yield return battle.WaitForReactions(me, manager, previousReactionCount);

            if (!reactionParams.ReactionValidated)
            {
                yield break;
            }

            rulesetAttacker.UpdateUsageForPower(_power, _power.CostPerUse);

            var totalRoll = (action.AttackRoll + attackMode.ToHitBonus).ToString();
            var rollCaption = action.AttackRoll == 1
                ? "Feedback/&RollCheckCriticalFailureTitle"
                : "Feedback/&CriticalAttackFailureOutcome";

            rulesetAttacker.LogCharacterUsedPower(_power,
                "Feedback/&TriggerRerollLine",
                false,
                (ConsoleStyleDuplet.ParameterType.Base, $"{action.AttackRoll}+{attackMode.ToHitBonus}"),
                (ConsoleStyleDuplet.ParameterType.FailedRoll, Gui.Format(rollCaption, totalRoll)));

            var roll = rulesetAttacker.RollAttack(
                attackMode.toHitBonus,
                target.RulesetCharacter,
                attackMode.sourceDefinition,
                attackModifier.attackToHitTrends,
                false,
                new List<TrendInfo> { new(1, FeatureSourceType.CharacterFeature, _power.Name, _power) },
                attackMode.ranged,
                false,
                attackModifier.attackRollModifier,
                out var outcome,
                out var successDelta,
                -1,
                // testMode true avoids the roll to display on combat log as the original one will get there with altered results
                true);

            attackModifier.ignoreAdvantage = false;
            attackModifier.attackAdvantageTrends =
                new List<TrendInfo> { new(1, FeatureSourceType.CharacterFeature, _power.Name, _power) };
            action.AttackRollOutcome = outcome;
            action.AttackSuccessDelta = successDelta;
            action.AttackRoll = roll;
        }
    }
}
