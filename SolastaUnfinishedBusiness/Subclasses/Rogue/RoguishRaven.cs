using System.Collections;
using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomDefinitions;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.Feats;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Properties;
using SolastaUnfinishedBusiness.Utils;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterSubclassDefinitions;

namespace SolastaUnfinishedBusiness.Subclasses.Rogue;

internal sealed class RoguishRaven : AbstractSubclass
{
    internal override FeatureDefinitionSubclassChoice GetSubclassChoiceList()
    {
        return FeatureDefinitionSubclassChoices.SubclassChoiceRogueRoguishArchetypes;
    }

    internal override CharacterSubclassDefinition GetSubclass()
    {
        return CreateRaven();
    }

    private static FeatureDefinitionFeatureSet BuildHeartSeekingShot()
    {
        var concentrationProvider = new EwFeats.StopPowerConcentrationProvider("HeartSeekingShot",
            "Tooltip/&HeartSeekingShotConcentration",
            CustomIcons.CreateAssetReferenceSprite("DeadeyeConcentrationIcon",
                Resources.DeadeyeConcentrationIcon, 64, 64));

        var triggerCondition = ConditionDefinitionBuilder
            .Create("ConditionHeartSeekingShotTrigger", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetDuration(RuleDefinitions.DurationType.Permanent)
            .SetFeatures(
                FeatureDefinitionBuilder
                    .Create("HeartSeekingShotTriggerFeature", DefinitionBuilder.CENamespaceGuid)
                    .SetGuiPresentationNoContent(true)
                    .SetCustomSubFeatures(concentrationProvider)
                    .AddToDB())
            .AddToDB();

        // -4 attack roll but critical threshold is 18 and deal 3d6 additional damage
        var heartSeekingShotCondition = ConditionDefinitionBuilder
            .Create("ConditionHeartSeekingShot", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation(Category.Condition)
            .AddFeatures(
                FeatureDefinitionAttributeModifierBuilder
                    .Create("AttributeModifierHeartSeekingShotCriticalThreshold", DefinitionBuilder.CENamespaceGuid)
                    .SetGuiPresentation(Category.Feature)
                    .SetModifier(FeatureDefinitionAttributeModifier.AttributeModifierOperation.Additive,
                        AttributeDefinitions.CriticalThreshold, -2)
                    .AddToDB(),
                FeatureDefinitionAttackModifierBuilder
                    .Create("AttackModifierHeartSeekingShot", DefinitionBuilder.CENamespaceGuid)
                    .SetGuiPresentation(Category.Feature)
                    .Configure(RuleDefinitions.AttackModifierMethod.FlatValue, -4)
                    .SetRequiredProperty(RuleDefinitions.RestrictedContextRequiredProperty.RangeWeapon)
                    .AddToDB(),
                FeatureDefinitionAdditionalDamageBuilder
                    .Create("AdditionalDamageHeartSeekingShot", DefinitionBuilder.CENamespaceGuid)
                    .SetGuiPresentation(Category.Feature)
                    .SetFrequencyLimit(RuleDefinitions.FeatureLimitedUsage.None)
                    .SetTriggerCondition(RuleDefinitions.AdditionalDamageTriggerCondition.CriticalHit)
                    .SetAdditionalDamageType(RuleDefinitions.AdditionalDamageType.SameAsBaseDamage)
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

        var turnOnPower = FeatureDefinitionPowerBuilder
            .Create("HeartSeekingShot", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation("HeartSeekingShot", Category.Feature,
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
                        .SetConditionForm(triggerCondition, ConditionForm.ConditionOperation.Add)
                        .Build(),
                    new EffectFormBuilder()
                        .SetConditionForm(heartSeekingShotCondition, ConditionForm.ConditionOperation.Add)
                        .Build())
                .Build())
            .SetCustomSubFeatures(new PowerUseValidity(CharacterValidators.HasTwoHandedRangeWeapon))
            .AddToDB();

        PowersContext.PowersThatIgnoreInterruptions.Add(turnOnPower);

        var turnOffPower = FeatureDefinitionPowerBuilder
            .Create("TurnOffHeartSeekingShotPower", DefinitionBuilder.CENamespaceGuid)
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
                        .SetConditionForm(triggerCondition, ConditionForm.ConditionOperation.Remove)
                        .Build(),
                    new EffectFormBuilder()
                        .SetConditionForm(heartSeekingShotCondition, ConditionForm.ConditionOperation.Remove)
                        .Build())
                .Build())
            .AddToDB();

        PowersContext.PowersThatIgnoreInterruptions.Add(turnOffPower);
        concentrationProvider.StopPower = turnOffPower;

        return FeatureDefinitionFeatureSetBuilder
            .Create("FeatureSetRoguishRavenHeartSeekingShot", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(turnOnPower, turnOffPower)
            .SetMode(FeatureDefinitionFeatureSet.FeatureSetMode.Union)
            .AddToDB();
    }

    private static CharacterSubclassDefinition CreateRaven()
    {
        // proficient with all two handed range weapons
        // ignore cover and long range disadvantage
        var sharpShooter = FeatureDefinitionFeatureSetBuilder
            .Create("FeatureSetRoguishRavenSharpShooter", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation(Category.Feature)
            .SetFeatureSet(
                FeatureDefinitionProficiencyBuilder
                    .Create("ProficiencyRoguishRavenRangeWeapon", DefinitionBuilder.CENamespaceGuid)
                    .SetGuiPresentationNoContent(true)
                    .SetProficiencies(RuleDefinitions.ProficiencyType.Weapon,
                        WeaponTypeDefinitions.HeavyCrossbowType.Name,
                        WeaponTypeDefinitions.LongbowType.Name)
                    .AddToDB(),
                FeatureDefinitionCombatAffinityBuilder
                    .Create("CombatAffinityRoguishRavenRangeAttack", DefinitionBuilder.CENamespaceGuid)
                    .SetGuiPresentationNoContent(true)
                    .SetIgnoreCover()
                    .SetCustomSubFeatures(new BumpWeaponAttackRangeToMax(WeaponValidators.AlwaysValid))
                    .AddToDB()
            )
            .SetMode(FeatureDefinitionFeatureSet.FeatureSetMode.Union)
            .AddToDB();

        // killing spree 
        // bonus range attack from main and can sneak attack after killing an enemies
        var killingSpree = FeatureDefinitionAdditionalActionBuilder
            .Create("AdditionalActionRoguishRavenKillingSpree", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation(Category.Feature)
            .SetTriggerCondition(RuleDefinitions.AdditionalActionTriggerCondition.HasDownedAnEnemy)
            .SetActionType(ActionDefinitions.ActionType.Main)
            .SetRestrictedActions(ActionDefinitions.Id.AttackMain)
            .SetMaxAttacksNumber(1)
            .SetCustomSubFeatures(new RefreshSneakAttckOnKill())
            .AddToDB();

        // pain maker
        // reroll any 1 when roll damage but need to use the new roll
        var painMaker = FeatureDefinitionDieRollModifierBuilder
            .Create("DieRollModifierRoguishRavenPainMaker", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation(Category.Feature)
            .SetModifiers(RuleDefinitions.RollContext.AttackDamageValueRoll, 1, 1,
                "Feature/&DieRollModifierRoguishRavenPainMakerReroll")
            .SetCustomSubFeatures(new RavenRerollAnyDamageDieMarker())
            .AddToDB();

        return CharacterSubclassDefinitionBuilder
            .Create("RoguishRaven", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation(Category.Subclass, RangerShadowTamer.GuiPresentation.SpriteReference)
            .AddFeatureAtLevel(sharpShooter, 3)
            .AddFeatureAtLevel(BuildHeartSeekingShot(), 3)
            .AddFeatureAtLevel(killingSpree, 9)
            .AddFeatureAtLevel(painMaker, 9)
            .AddToDB();
    }

    // marker to reroll any damage die including sneak attack
    public sealed class RavenRerollAnyDamageDieMarker
    {
    }

    private sealed class RefreshSneakAttckOnKill : ITargetReducedToZeroHP
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
