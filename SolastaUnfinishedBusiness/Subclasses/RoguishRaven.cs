using System.Collections;
using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomDefinitions;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Properties;
using SolastaUnfinishedBusiness.Utils;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterSubclassDefinitions;

namespace SolastaUnfinishedBusiness.Subclasses;

internal sealed class RoguishRaven : AbstractSubclass
{
    internal RoguishRaven()
    {
        // proficient with all two handed range weapons
        // ignore cover and long range disadvantage
        var featureSetRavenSharpShooter = FeatureDefinitionFeatureSetBuilder
            .Create("FeatureSetRavenSharpShooter")
            .SetGuiPresentation(Category.Feature)
            .SetFeatureSet(
                FeatureDefinitionProficiencyBuilder
                    .Create("ProficiencyRavenRangeWeapon")
                    .SetGuiPresentationNoContent(true)
                    .SetProficiencies(RuleDefinitions.ProficiencyType.Weapon,
                        WeaponTypeDefinitions.HeavyCrossbowType.Name,
                        WeaponTypeDefinitions.LongbowType.Name)
                    .AddToDB(),
                FeatureDefinitionCombatAffinityBuilder
                    .Create("CombatAffinityRavenRangeAttack")
                    .SetGuiPresentationNoContent(true)
                    .SetIgnoreCover()
                    .SetCustomSubFeatures(new BumpWeaponAttackRangeToMax(ValidatorsWeapon.AlwaysValid))
                    .AddToDB()
            )
            .SetMode(FeatureDefinitionFeatureSet.FeatureSetMode.Union)
            .AddToDB();

        // killing spree 
        // bonus range attack from main and can sneak attack after killing an enemies
        var additionalActionRavenKillingSpree = FeatureDefinitionAdditionalActionBuilder
            .Create("AdditionalActionRavenKillingSpree")
            .SetGuiPresentation(Category.Feature)
            .SetTriggerCondition(RuleDefinitions.AdditionalActionTriggerCondition.HasDownedAnEnemy)
            .SetActionType(ActionDefinitions.ActionType.Main)
            .SetRestrictedActions(ActionDefinitions.Id.AttackMain)
            .SetMaxAttacksNumber(1)
            .SetCustomSubFeatures(new RefreshSneakAttackOnKill())
            .AddToDB();

        // pain maker
        // reroll any 1 when roll damage but need to use the new roll
        var dieRollModifierRavenPainMaker = FeatureDefinitionDieRollModifierBuilder
            .Create("DieRollModifierRavenPainMaker")
            .SetGuiPresentation(Category.Feature)
            .SetModifiers(RuleDefinitions.RollContext.AttackDamageValueRoll, 1, 1,
                "Feature/&DieRollModifierRavenPainMakerReroll")
            .SetCustomSubFeatures(new RavenRerollAnyDamageDieMarker())
            .AddToDB();

        Subclass = CharacterSubclassDefinitionBuilder
            .Create("RoguishRaven")
            .SetGuiPresentation(Category.Subclass, RangerShadowTamer.GuiPresentation.SpriteReference)
            .AddFeaturesAtLevel(3, featureSetRavenSharpShooter)
            .AddFeaturesAtLevel(3, BuildHeartSeekingShot())
            .AddFeaturesAtLevel(9,
                additionalActionRavenKillingSpree
                , dieRollModifierRavenPainMaker)
            .AddToDB();
    }

    // ReSharper disable once InconsistentNaming
    private static CharacterSubclassDefinition Subclass { get; set; }

    private static FeatureDefinitionFeatureSet BuildHeartSeekingShot()
    {
        var concentrationProvider = new StopPowerConcentrationProvider("HeartSeekingShot",
            "Tooltip/&HeartSeekingShotConcentration",
            CustomIcons.CreateAssetReferenceSprite("DeadeyeConcentrationIcon",
                Resources.DeadeyeConcentrationIcon, 64, 64));

        var conditionRavenHeartSeekingShotTrigger = ConditionDefinitionBuilder
            .Create("ConditionRavenHeartSeekingShotTrigger")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetDuration(RuleDefinitions.DurationType.Permanent)
            .SetFeatures(
                FeatureDefinitionBuilder
                    .Create("RavenHeartSeekingShotTriggerFeature")
                    .SetGuiPresentationNoContent(true)
                    .SetCustomSubFeatures(concentrationProvider)
                    .AddToDB())
            .AddToDB();

        // -4 attack roll but critical threshold is 18 and deal 3d6 additional damage
        var conditionRavenHeartSeekingShot = ConditionDefinitionBuilder
            .Create("ConditionRavenHeartSeekingShot")
            .SetGuiPresentation(Category.Condition)
            .AddFeatures(
                FeatureDefinitionAttributeModifierBuilder
                    .Create("AttributeModifierRavenHeartSeekingShotCriticalThreshold")
                    .SetGuiPresentation(Category.Feature)
                    .SetModifier(FeatureDefinitionAttributeModifier.AttributeModifierOperation.Additive,
                        AttributeDefinitions.CriticalThreshold, -2)
                    .SetCustomSubFeatures(
                        new ValidatorDefinitionApplication(ValidatorsCharacter.HasTwoHandedRangeWeapon))
                    .AddToDB(),
                FeatureDefinitionAttackModifierBuilder
                    .Create("AttackModifierRavenHeartSeekingShot")
                    .SetGuiPresentation(Category.Feature)
                    .Configure(RuleDefinitions.AttackModifierMethod.FlatValue, -4)
                    .SetCustomSubFeatures(new RestrictedContextValidator(OperationType.Set,
                        ValidatorsCharacter.HasTwoHandedRangeWeapon))
                    .SetRequiredProperty(RuleDefinitions.RestrictedContextRequiredProperty.RangeWeapon)
                    .AddToDB(),
                FeatureDefinitionAdditionalDamageBuilder
                    .Create("AdditionalDamageRavenHeartSeekingShot")
                    .SetGuiPresentation(Category.Feature)
                    .SetFrequencyLimit(RuleDefinitions.FeatureLimitedUsage.None)
                    .SetTriggerCondition(RuleDefinitions.AdditionalDamageTriggerCondition.CriticalHit)
                    .SetAdditionalDamageType(RuleDefinitions.AdditionalDamageType.SameAsBaseDamage)
                    .SetCustomSubFeatures(new RestrictedContextValidator(OperationType.Set,
                        ValidatorsCharacter.HasTwoHandedRangeWeapon))
                    .SetRequiredProperty(RuleDefinitions.RestrictedContextRequiredProperty.RangeWeapon)
                    .SetDamageValueDetermination(RuleDefinitions.AdditionalDamageValueDetermination.Die)
                    .SetDamageDice(RuleDefinitions.DieType.D6, 1)
                    .SetAdvancement(
                        (RuleDefinitions.AdditionalDamageAdvancement)ExtraAdditionalDamageAdvancement.ClassLevel,
                        (3, 2),
                        (4, 2),
                        (5, 2),
                        (6, 2),
                        (7, 3),
                        (8, 3),
                        (9, 3),
                        (10, 3),
                        (11, 4),
                        (12, 4),
                        (13, 4),
                        (14, 4),
                        (15, 5),
                        (16, 5),
                        (17, 5),
                        (18, 5),
                        (19, 6),
                        (20, 6))
                    .SetNotificationTag("HeartSeekingShot")
                    .AddToDB()
            )
            .AddToDB();

        var powerRavenHeartSeekingShot = FeatureDefinitionPowerBuilder
            .Create("PowerRavenHeartSeekingShot")
            .SetGuiPresentation(Category.Feature,
                CustomIcons.CreateAssetReferenceSprite("DeadeyeIcon",
                    Resources.DeadeyeIcon, 128, 64))
            .SetActivationTime(RuleDefinitions.ActivationTime.NoCost)
            .SetUsesFixed(1)
            .SetCostPerUse(0)
            .SetRechargeRate(RuleDefinitions.RechargeRate.AtWill)
            .SetEffectDescription(new EffectDescriptionBuilder()
                .SetTargetingData(RuleDefinitions.Side.Ally, RuleDefinitions.RangeType.Self, 1,
                    RuleDefinitions.TargetType.Self)
                .SetDurationData(RuleDefinitions.DurationType.Permanent)
                .SetEffectForms(
                    new EffectFormBuilder()
                        .SetConditionForm(conditionRavenHeartSeekingShotTrigger, ConditionForm.ConditionOperation.Add)
                        .Build(),
                    new EffectFormBuilder()
                        .SetConditionForm(conditionRavenHeartSeekingShot, ConditionForm.ConditionOperation.Add)
                        .Build())
                .Build())
            .SetCustomSubFeatures(new ValidatorPowerUse(ValidatorsCharacter.HasTwoHandedRangeWeapon))
            .AddToDB();

        Global.PowersThatIgnoreInterruptions.Add(powerRavenHeartSeekingShot);

        var powerRavenTurnOffHeartSeekingShot = FeatureDefinitionPowerBuilder
            .Create("PowerRavenTurnOffHeartSeekingShot")
            .SetGuiPresentationNoContent(true)
            .SetActivationTime(RuleDefinitions.ActivationTime.NoCost)
            .SetUsesFixed(1)
            .SetCostPerUse(0)
            .SetRechargeRate(RuleDefinitions.RechargeRate.AtWill)
            .SetEffectDescription(new EffectDescriptionBuilder()
                .SetTargetingData(RuleDefinitions.Side.Ally, RuleDefinitions.RangeType.Self, 1,
                    RuleDefinitions.TargetType.Self)
                .SetDurationData(RuleDefinitions.DurationType.Round, 0, false)
                .SetEffectForms(
                    new EffectFormBuilder()
                        .SetConditionForm(conditionRavenHeartSeekingShotTrigger,
                            ConditionForm.ConditionOperation.Remove)
                        .Build(),
                    new EffectFormBuilder()
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
            .SetMode(FeatureDefinitionFeatureSet.FeatureSetMode.Union)
            .AddToDB();
    }

    internal override FeatureDefinitionSubclassChoice GetSubclassChoiceList()
    {
        return FeatureDefinitionSubclassChoices.SubclassChoiceRogueRoguishArchetypes;
    }

    internal override CharacterSubclassDefinition GetSubclass()
    {
        return Subclass;
    }

    // marker to reroll any damage die including sneak attack
    public sealed class RavenRerollAnyDamageDieMarker
    {
    }

    private sealed class RefreshSneakAttackOnKill : ITargetReducedToZeroHP
    {
        public IEnumerator HandleCharacterReducedToZeroHP(GameLocationCharacter attacker,
            GameLocationCharacter downedCreature,
            RulesetAttackMode attackMode, RulesetEffect activeEffect)
        {
            if (attacker.IsOppositeSide(downedCreature.Side))
            {
                attacker.UsedSpecialFeatures.Remove(FeatureDefinitionAdditionalDamages
                    .AdditionalDamageRogueSneakAttack.Name);
            }

            yield break;
        }
    }
}
