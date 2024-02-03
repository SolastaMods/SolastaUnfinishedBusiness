using System.Collections;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Behaviors;
using SolastaUnfinishedBusiness.Behaviors.Specific;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Properties;
using SolastaUnfinishedBusiness.Validators;
using static RuleDefinitions;
using static FeatureDefinitionAttributeModifier;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;

namespace SolastaUnfinishedBusiness.Subclasses;

[UsedImplicitly]
public sealed class RoguishRaven : AbstractSubclass
{
    private const string Name = "Raven";

    public RoguishRaven()
    {
        // Sharpshooter

        var featureSetRavenSharpShooter = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}SharpShooter")
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
                // ignore cover and long range disadvantage
                FeatureDefinitionCombatAffinityBuilder
                    .Create($"CombatAffinity{Name}RangeAttack")
                    .SetGuiPresentation($"FeatureSet{Name}SharpShooter", Category.Feature)
                    .SetIgnoreCover()
                    .AddCustomSubFeatures(
                        new BumpWeaponWeaponAttackRangeToMax(ValidatorsWeapon.IsTwoHandedRanged))
                    .AddToDB())
            .AddToDB();

        // Killing Spree

        var featureRavenKillingSpree = FeatureDefinitionBuilder
            .Create($"Feature{Name}KillingSpree")
            .SetGuiPresentation("AdditionalActionRavenKillingSpree", Category.Feature)
            .AddCustomSubFeatures(
                // bonus range attack from main and can sneak attack after killing an enemies
                new OnReducedToZeroHpByMeKillingSpree(
                    ConditionDefinitionBuilder
                        .Create($"Condition{Name}KillingSpree")
                        .SetGuiPresentationNoContent(true)
                        .SetSilent(Silent.WhenAddedOrRemoved)
                        .SetFeatures(
                            FeatureDefinitionAdditionalActionBuilder
                                .Create("AdditionalActionRavenKillingSpree")
                                .SetGuiPresentation(Category.Feature)
                                .SetActionType(ActionDefinitions.ActionType.Main)
                                .SetRestrictedActions(ActionDefinitions.Id.AttackMain)
                                .SetMaxAttacksNumber(1)
                                .AddCustomSubFeatures(ValidateAdditionalActionAttack.TwoHandedRanged)
                                .AddToDB())
                        .AddToDB()))
            .AddToDB();

        // Pain Maker

        // reroll any 1 when roll damage but need to use the new roll
        var dieRollModifierRavenPainMaker = FeatureDefinitionDieRollModifierBuilder
            .Create($"DieRollModifier{Name}PainMaker")
            .SetGuiPresentation(Category.Feature)
            .SetModifiers(RollContext.AttackDamageValueRoll, 1, 1, 1,
                "Feature/&DieRollModifierRavenPainMakerReroll")
            .AddCustomSubFeatures(new RavenRerollAnyDamageDieMarker())
            .AddToDB();

        // Deadly Aim

        var powerDeadlyAim = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}DeadlyAim")
            .SetGuiPresentation($"FeatureSet{Name}DeadlyAim", Category.Feature, hidden: true)
            .SetUsesFixed(ActivationTime.NoCost, RechargeRate.ShortRest)
            .AddToDB();

        var featureRavenDeadlyAim = FeatureDefinitionBuilder
            .Create($"Feature{Name}DeadlyAim")
            .SetGuiPresentationNoContent(true)
            .AddCustomSubFeatures(new TryAlterOutcomePhysicalAttackByMeDeadlyAim(powerDeadlyAim))
            .AddToDB();

        var featureSetRavenDeadlyAim = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}DeadlyAim")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(featureRavenDeadlyAim, powerDeadlyAim)
            .AddToDB();

        // Perfect Shot

        var dieRollModifierRavenPerfectShot = FeatureDefinitionDieRollModifierBuilder
            .Create($"DieRollModifier{Name}PerfectShot")
            .SetGuiPresentation(Category.Feature)
            .SetModifiers(RollContext.AttackDamageValueRoll, 1, 2, 1,
                "Feature/&DieRollModifierRavenPainMakerReroll")
            .AddCustomSubFeatures(new RavenRerollAnyDamageDieMarker())
            .AddToDB();

        Subclass = CharacterSubclassDefinitionBuilder
            .Create($"Roguish{Name}")
            .SetGuiPresentation(Category.Subclass,
                Sprites.GetSprite(Name, Resources.RoguishRaven, 256))
            .AddFeaturesAtLevel(3,
                featureSetRavenSharpShooter,
                BuildHeartSeekingShot())
            .AddFeaturesAtLevel(9,
                featureRavenKillingSpree,
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
        var concentrationProvider = new StopPowerConcentrationProvider(
            "HeartSeekingShot",
            "Tooltip/&HeartSeekingShotConcentration",
            Sprites.GetSprite("DeadeyeConcentrationIcon",
                Resources.DeadeyeConcentrationIcon, 64, 64));

        var conditionRavenHeartSeekingShotTrigger = ConditionDefinitionBuilder
            .Create("ConditionRavenHeartSeekingShotTrigger")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .AddCustomSubFeatures(concentrationProvider)
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
                    .SetGuiPresentation(Category.Feature, Gui.NoLocalization)
                    .SetModifier(AttributeModifierOperation.Additive, AttributeDefinitions.CriticalThreshold, -2)
                    .AddCustomSubFeatures(validateHasTwoHandedRangedWeapon)
                    .SetSituationalContext(SituationalContext.AttackingWithRangedWeapon)
                    .AddToDB(),
                FeatureDefinitionAttackModifierBuilder
                    .Create("AttackModifierRavenHeartSeekingShot")
                    .SetGuiPresentation(Category.Feature, Gui.NoLocalization)
                    .SetAttackRollModifier(-4)
                    .AddCustomSubFeatures(validateHasTwoHandedRangedWeapon)
                    .SetRequiredProperty(RestrictedContextRequiredProperty.RangeWeapon)
                    .AddToDB(),
                FeatureDefinitionAdditionalDamageBuilder
                    .Create("AdditionalDamageRavenHeartSeekingShot")
                    .SetGuiPresentationNoContent(true)
                    .SetNotificationTag("HeartSeekingShot")
                    .SetDamageDice(DieType.D6, 1)
                    .SetAdvancement(AdditionalDamageAdvancement.ClassLevel, 1, 1, 4, 3)
                    .SetRequiredProperty(RestrictedContextRequiredProperty.RangeWeapon)
                    .AddCustomSubFeatures(
                        ValidatorsCharacter.HasTwoHandedRangedWeapon,
                        new RogueModifyAdditionalDamageClassLevelHolder())
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
            .AddCustomSubFeatures(
                IgnoreInvisibilityInterruptionCheck.Marker,
                new ValidatorsValidatePowerUse(ValidatorsCharacter.HasTwoHandedRangedWeapon))
            .AddToDB();

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
            .AddCustomSubFeatures(IgnoreInvisibilityInterruptionCheck.Marker)
            .AddToDB();

        concentrationProvider.StopPower = powerRavenTurnOffHeartSeekingShot;

        return FeatureDefinitionFeatureSetBuilder
            .Create("FeatureSetRavenHeartSeekingShot")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(powerRavenHeartSeekingShot, powerRavenTurnOffHeartSeekingShot)
            .AddToDB();
    }

    // marker to reroll any damage die including sneak attack
    internal sealed class RavenRerollAnyDamageDieMarker;

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

            if (attacker.RulesetCharacter.HasAnyConditionOfType(condition.Name))
            {
                yield break;
            }

            if (!attacker.IsMyTurn())
            {
                yield break;
            }

            attacker.UsedSpecialFeatures.Remove(
                FeatureDefinitionAdditionalDamages.AdditionalDamageRogueSneakAttack.Name);

            attacker.RulesetCharacter.InflictCondition(
                condition.Name,
                DurationType.Round,
                0,
                TurnOccurenceType.EndOfTurn,
                AttributeDefinitions.TagEffect,
                attacker.RulesetCharacter.guid,
                attacker.RulesetCharacter.CurrentFaction.Name,
                1,
                condition.Name,
                0,
                0,
                0);
        }
    }

    //
    // Deadly Aim
    //

    private class TryAlterOutcomePhysicalAttackByMeDeadlyAim(FeatureDefinitionPower power) : ITryAlterOutcomeAttack
    {
        public IEnumerator OnTryAlterOutcomeAttack(
            GameLocationBattleManager battle,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            GameLocationCharacter helper,
            ActionModifier attackModifier)
        {
            var gameLocationActionManager =
                ServiceRepository.GetService<IGameLocationActionService>() as GameLocationActionManager;

            if (gameLocationActionManager == null)
            {
                yield break;
            }

            if (action.AttackRollOutcome is not (RollOutcome.Failure or RollOutcome.CriticalFailure))
            {
                yield break;
            }

            var attackMode = action.actionParams.attackMode;
            var rulesetCharacter = attacker.RulesetCharacter;

            if (rulesetCharacter is not { IsDeadOrDyingOrUnconscious: false } ||
                !rulesetCharacter.CanUsePower(power) ||
                !attacker.CanPerceiveTarget(defender) ||
                !attackMode.ranged)
            {
                yield break;
            }

            var reactionParams = new CharacterActionParams(attacker, (ActionDefinitions.Id)ExtraActionId.DoNothingFree)
            {
                StringParameter = "Reaction/&CustomReactionRavenDeadlyAimReactDescription"
            };
            var previousReactionCount = gameLocationActionManager.PendingReactionRequestGroups.Count;
            var reactionRequest = new ReactionRequestCustom("RavenDeadlyAim", reactionParams);

            gameLocationActionManager.AddInterruptRequest(reactionRequest);

            yield return battle.WaitForReactions(attacker, gameLocationActionManager, previousReactionCount);

            if (!reactionParams.ReactionValidated)
            {
                yield break;
            }

            rulesetCharacter.UpdateUsageForPower(power, power.CostPerUse);

            var totalRoll = (action.AttackRoll + attackMode.ToHitBonus).ToString();
            var rollCaption = action.AttackRoll == 1
                ? "Feedback/&RollCheckCriticalFailureTitle"
                : "Feedback/&CriticalAttackFailureOutcome";

            rulesetCharacter.LogCharacterUsedPower(
                power,
                "Feedback/&TriggerRerollLine",
                false,
                (ConsoleStyleDuplet.ParameterType.Base, $"{action.AttackRoll}+{attackMode.ToHitBonus}"),
                (ConsoleStyleDuplet.ParameterType.FailedRoll, Gui.Format(rollCaption, totalRoll)));

            var roll = rulesetCharacter.RollAttack(
                attackMode.toHitBonus,
                defender.RulesetCharacter,
                attackMode.sourceDefinition,
                attackModifier.attackToHitTrends,
                false,
                [new TrendInfo(1, FeatureSourceType.CharacterFeature, power.Name, power)],
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
                [new TrendInfo(1, FeatureSourceType.CharacterFeature, power.Name, power)];
            action.AttackRollOutcome = outcome;
            action.AttackSuccessDelta = successDelta;
            action.AttackRoll = roll;
        }
    }
}
