using System.Collections;
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
using static RuleDefinitions;

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
                    .SetProficiencies(ProficiencyType.Weapon,
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
            .SetTriggerCondition(AdditionalActionTriggerCondition.HasDownedAnEnemy)
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
            .SetModifiers(RollContext.AttackDamageValueRoll, 1, 1, 1,
                "Feature/&DieRollModifierRavenPainMakerReroll")
            .SetCustomSubFeatures(new RavenRerollAnyDamageDieMarker())
            .AddToDB();

        Subclass = CharacterSubclassDefinitionBuilder
            .Create("RoguishRaven")
            .SetGuiPresentation(Category.Subclass, RangerShadowTamer.GuiPresentation.SpriteReference)
            .AddFeaturesAtLevel(3,
                featureSetRavenSharpShooter,
                BuildHeartSeekingShot())
            .AddFeaturesAtLevel(9,
                additionalActionRavenKillingSpree,
                dieRollModifierRavenPainMaker)
            .AddToDB();
    }

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceRogueRoguishArchetypes;

    private static FeatureDefinitionFeatureSet BuildHeartSeekingShot()
    {
        var concentrationProvider = new StopPowerConcentrationProvider("HeartSeekingShot",
            "Tooltip/&HeartSeekingShotConcentration",
            CustomIcons.GetSprite("DeadeyeConcentrationIcon",
                Resources.DeadeyeConcentrationIcon, 64, 64));

        var conditionRavenHeartSeekingShotTrigger = ConditionDefinitionBuilder
            .Create("ConditionRavenHeartSeekingShotTrigger")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetDuration(DurationType.Permanent)
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
                    .Configure(AttackModifierMethod.FlatValue, -4)
                    .SetCustomSubFeatures(new RestrictedContextValidator(OperationType.Set,
                        ValidatorsCharacter.HasTwoHandedRangeWeapon))
                    .SetRequiredProperty(RestrictedContextRequiredProperty.RangeWeapon)
                    .AddToDB(),
                FeatureDefinitionAdditionalDamageBuilder
                    .Create("AdditionalDamageRavenHeartSeekingShot")
                    .SetGuiPresentation(Category.Feature)
                    .SetFrequencyLimit(FeatureLimitedUsage.None)
                    .SetTriggerCondition(AdditionalDamageTriggerCondition.CriticalHit)
                    .SetAdditionalDamageType(AdditionalDamageType.SameAsBaseDamage)
                    .SetCustomSubFeatures(new RestrictedContextValidator(OperationType.Set,
                        ValidatorsCharacter.HasTwoHandedRangeWeapon))
                    .SetRequiredProperty(RestrictedContextRequiredProperty.RangeWeapon)
                    .SetDamageValueDetermination(AdditionalDamageValueDetermination.Die)
                    .SetDamageDice(DieType.D6, 1)
                    .SetAdvancement(
                        AdditionalDamageAdvancement.ClassLevel,
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
                CustomIcons.GetSprite("DeadeyeIcon", Resources.DeadeyeIcon, 128, 64))
            .SetUsesFixed(
                ActivationTime.NoCost,
                RechargeRate.AtWill,
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Ally, RangeType.Self, 1,
                        TargetType.Self)
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
            .SetCustomSubFeatures(new ValidatorPowerUse(ValidatorsCharacter.HasTwoHandedRangeWeapon))
            .AddToDB();

        Global.PowersThatIgnoreInterruptions.Add(powerRavenHeartSeekingShot);

        var powerRavenTurnOffHeartSeekingShot = FeatureDefinitionPowerBuilder
            .Create("PowerRavenTurnOffHeartSeekingShot")
            .SetGuiPresentationNoContent(true)
            .SetUsesFixed(
                ActivationTime.NoCost,
                RechargeRate.AtWill,
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Ally, RangeType.Self, 1, TargetType.Self)
                    .SetDurationData(DurationType.Round, 1)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(conditionRavenHeartSeekingShotTrigger,
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
            .SetMode(FeatureDefinitionFeatureSet.FeatureSetMode.Union)
            .AddToDB();
    }

    // marker to reroll any damage die including sneak attack
    internal sealed class RavenRerollAnyDamageDieMarker
    {
    }

    private sealed class RefreshSneakAttackOnKill : ITargetReducedToZeroHp
    {
        public IEnumerator HandleCharacterReducedToZeroHp(GameLocationCharacter attacker,
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
