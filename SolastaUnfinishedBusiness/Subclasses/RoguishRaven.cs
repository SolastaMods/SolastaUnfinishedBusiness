using System.Collections;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Properties;
using static RuleDefinitions;
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
                    .SetGuiPresentationNoContent(true)
                    .SetIgnoreCover()
                    .SetCustomSubFeatures(new BumpWeaponAttackRangeToMax(ValidatorsWeapon.AlwaysValid))
                    .AddToDB())
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
            .SetGuiPresentation(Category.Subclass, RangerShadowTamer)
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
            new RestrictedContextValidator(OperationType.Set, ValidatorsCharacter.HasTwoHandedRangedWeapon);
        
        // -4 attack roll but critical threshold is 18 and deal 3d6 additional damage
        var conditionRavenHeartSeekingShot = ConditionDefinitionBuilder
            .Create("ConditionRavenHeartSeekingShot")
            .SetGuiPresentation("FeatureSetRavenHeartSeekingShot", Category.Feature)
            .SetPossessive()
            .AddFeatures(
                FeatureDefinitionAttributeModifierBuilder
                    .Create("AttributeModifierRavenHeartSeekingShotCriticalThreshold")
                    .SetGuiPresentation(Category.Feature)
                    .SetModifier(FeatureDefinitionAttributeModifier.AttributeModifierOperation.Additive,
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
                    .SetTriggerCondition(AdditionalDamageTriggerCondition.AlwaysActive)
                    .SetAdditionalDamageType(AdditionalDamageType.SameAsBaseDamage)
                    .SetCustomSubFeatures(
                        ValidatorsCharacter.HasTwoHandedRangedWeapon,
                        new HeartSeekingShotAdditionalDamageOnCritMarker(CharacterClassDefinitions.Rogue))
                    .SetRequiredProperty(RestrictedContextRequiredProperty.RangeWeapon)
                    .SetDamageDice(DieType.D6, 1)
                    .SetAdvancement(AdditionalDamageAdvancement.ClassLevel, 1, 1, 4, 3)
                    .AddToDB()
            )
            .AddToDB();

        var deadEyeSprite = Sprites.GetSprite("DeadeyeIcon", Resources.DeadeyeIcon, 128, 64);

        var powerRavenHeartSeekingShot = FeatureDefinitionPowerBuilder
            .Create("PowerRavenHeartSeekingShot")
            .SetGuiPresentation("FeatureSetRavenHeartSeekingShot", Category.Feature, deadEyeSprite)
            .SetUsesFixed(ActivationTime.NoCost)
            .SetEffectDescription(EffectDescriptionBuilder
                .Create()
                .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                .SetDurationData(DurationType.Permanent)
                .SetEffectForms(
                    EffectFormBuilder
                        .Create()
                        .SetConditionForm(conditionRavenHeartSeekingShotTrigger, ConditionForm.ConditionOperation.Add)
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
            .SetEffectDescription(EffectDescriptionBuilder
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

    private sealed class RefreshSneakAttackOnKill : ITargetReducedToZeroHp
    {
        public IEnumerator HandleCharacterReducedToZeroHp(
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
}
