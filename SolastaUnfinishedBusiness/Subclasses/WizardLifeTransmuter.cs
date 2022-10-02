using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterSubclassDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionDamageAffinitys;
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.Subclasses;

internal sealed class WizardLifeTransmuter : AbstractSubclass
{
    internal WizardLifeTransmuter()
    {
        var magicAffinityLifeTransmuterHeightened = FeatureDefinitionMagicAffinityBuilder
            .Create("MagicAffinityLifeTransmuterHeightened")
            .SetGuiPresentation(Category.Feature)
            .SetWarList(2,
                FalseLife, // necromancy
                MagicWeapon, // transmutation
                Blindness, // necromancy
                Fly, // transmutation
                BestowCurse, // necromancy
                VampiricTouch, // necromancy
                Blight, // necromancy
                CloudKill) // conjuration)
            .AddToDB();

        var powerSharedPoolLifeTransmuterHealingPool = FeatureDefinitionPowerPoolBuilder
            .Create("PowerSharedPoolLifeTransmuterHealingPool")
            .Configure(
                2,
                UsesDetermination.Fixed,
                AttributeDefinitions.Intelligence,
                RechargeRate.LongRest)
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        var conditionLifeTransmuterDarkvision = ConditionDefinitionBuilder
            .Create("ConditionLifeTransmuterDarkvision")
            .SetGuiPresentation(
                "PowerSharedPoolLifeTransmuterDarkvision",
                Category.Feature,
                ConditionDefinitions.ConditionDarkvision.GuiPresentation.SpriteReference)
            .SetFeatures(FeatureDefinitionSenses.SenseSuperiorDarkvision)
            .SetConditionType(ConditionType.Beneficial)
            .SetAllowMultipleInstances(false)
            .SetDuration(DurationType.UntilLongRest, 1, false)
            .AddToDB();

        var powerSharedPoolLifeTransmuterDarkvision = BuildActionTransmuteConditionPower(
                powerSharedPoolLifeTransmuterHealingPool,
                RechargeRate.LongRest,
                ActivationTime.BonusAction,
                1, RangeType.Touch,
                2,
                TargetType.Individuals,
                ActionDefinitions.ItemSelectionType.None,
                DurationType.UntilLongRest,
                1,
                TurnOccurenceType.EndOfTurn,
                AttributeDefinitions.Intelligence, conditionLifeTransmuterDarkvision,
                "PowerSharedPoolLifeTransmuterDarkvision")
            .SetGuiPresentation(
                Category.Feature,
                FeatureDefinitionPowers.PowerDomainBattleDivineWrath.GuiPresentation.SpriteReference)
            .AddToDB();

        var conditionLifeTransmuterPoison = ConditionDefinitionBuilder
            .Create("ConditionLifeTransmuterPoison")
            .SetGuiPresentation(
                Category.Condition,
                ConditionDefinitions.ConditionProtectedFromPoison.GuiPresentation.SpriteReference)
            .SetFeatures(
                DamageAffinityPoisonResistance,
                DamageAffinityAcidResistance,
                DamageAffinityColdResistance,
                DamageAffinityFireResistance,
                DamageAffinityThunderResistance,
                DamageAffinityLightningResistance,
                DamageAffinityNecroticResistance)
            .SetConditionType(ConditionType.Beneficial)
            .SetAllowMultipleInstances(false)
            .SetDuration(DurationType.UntilLongRest, 1, false)
            .AddToDB();

        var powerSharedPoolLifeTransmuterPoison = BuildActionTransmuteConditionPower(
                powerSharedPoolLifeTransmuterHealingPool,
                RechargeRate.LongRest,
                ActivationTime.BonusAction,
                1, RangeType.Touch,
                2,
                TargetType.Individuals,
                ActionDefinitions.ItemSelectionType.None,
                DurationType.UntilLongRest,
                1,
                TurnOccurenceType.EndOfTurn,
                AttributeDefinitions.Intelligence, conditionLifeTransmuterPoison,
                "PowerSharedPoolLifeTransmuterPoison")
            .SetGuiPresentation(
                "PowerLifeTransmuterElementalResistance",
                Category.Feature,
                FeatureDefinitionPowers.PowerDomainElementalFireBurst.GuiPresentation.SpriteReference)
            .AddToDB();

        var conditionLifeTransmuterConstitution = ConditionDefinitionBuilder
            .Create("ConditionLifeTransmuterConstitution")
            .SetGuiPresentation(Category.Condition,
                ConditionDefinitions.ConditionBearsEndurance.GuiPresentation.SpriteReference)
            .SetFeatures(FeatureDefinitionSavingThrowAffinitys.SavingThrowAffinityCreedOfArun)
            .SetConditionType(ConditionType.Beneficial)
            .SetAllowMultipleInstances(false)
            .SetDuration(DurationType.UntilLongRest, 1, false)
            .AddToDB();

        var powerSharedPoolLifeTransmuterConstitution = BuildActionTransmuteConditionPower(
                powerSharedPoolLifeTransmuterHealingPool,
                RechargeRate.LongRest,
                ActivationTime.BonusAction,
                1, RangeType.Touch,
                2,
                TargetType.Individuals,
                ActionDefinitions.ItemSelectionType.None,
                DurationType.UntilLongRest,
                1,
                TurnOccurenceType.EndOfTurn,
                AttributeDefinitions.Intelligence, conditionLifeTransmuterConstitution,
                "PowerSharedPoolLifeTransmuterConstitution")
            .SetGuiPresentation(
                Category.Feature,
                FeatureDefinitionPowers.PowerPaladinAuraOfCourage.GuiPresentation.SpriteReference)
            .AddToDB();

        var powerPoolModifierLifeTransmuterHealingPoolExtra = FeatureDefinitionPowerPoolModifierBuilder
            .Create("PowerPoolModifierLifeTransmuterHealingPoolExtra")
            .SetGuiPresentation(Category.Feature)
            .Configure(
                2,
                UsesDetermination.Fixed,
                AttributeDefinitions.Intelligence,
                powerSharedPoolLifeTransmuterHealingPool)
            .AddToDB();

        var powerSharedPoolLifeTransmuterFly = BuildActionTransmuteConditionPower(
                powerSharedPoolLifeTransmuterHealingPool,
                RechargeRate.LongRest,
                ActivationTime.BonusAction,
                1, RangeType.Touch,
                2,
                TargetType.IndividualsUnique,
                ActionDefinitions.ItemSelectionType.None,
                DurationType.UntilLongRest,
                1,
                TurnOccurenceType.EndOfTurn,
                AttributeDefinitions.Intelligence,
                ConditionDefinitions.ConditionFlying12,
                "PowerSharedPoolLifeTransmuterFly")
            .SetGuiPresentation(
                Category.Feature,
                Fly.GuiPresentation.SpriteReference)
            .AddToDB();

        var powerSharedPoolLifeTransmuterHeal = FeatureDefinitionPowerSharedPoolBuilder
            .Create("PowerSharedPoolLifeTransmuterHeal")
            .SetGuiPresentation(Category.Feature, MassHealingWord.GuiPresentation.SpriteReference)
            .Configure(
                powerSharedPoolLifeTransmuterHealingPool,
                RechargeRate.LongRest,
                ActivationTime.BonusAction,
                1,
                false,
                false,
                AttributeDefinitions.Intelligence,
                MassHealingWord.EffectDescription,
                false /* unique instance */)
            .AddToDB();

        var powerSharedPoolLifeTransmuterRevive = FeatureDefinitionPowerSharedPoolBuilder
            .Create("PowerSharedPoolLifeTransmuterRevive")
            .SetGuiPresentation(Category.Feature, Revivify.GuiPresentation.SpriteReference)
            .Configure(
                powerSharedPoolLifeTransmuterHealingPool,
                RechargeRate.LongRest,
                ActivationTime.BonusAction,
                1,
                false,
                false,
                AttributeDefinitions.Intelligence,
                Revivify.EffectDescription,
                false)
            .AddToDB();

        var powerPoolModifierLifeTransmuterHealingPoolBonus = FeatureDefinitionPowerPoolModifierBuilder
            .Create("PowerPoolModifierLifeTransmuterHealingPoolBonus")
            .SetGuiPresentation(Category.Feature)
            .Configure(
                4,
                UsesDetermination.Fixed,
                AttributeDefinitions.Intelligence,
                powerSharedPoolLifeTransmuterHealingPool)
            .AddToDB();

        Subclass = CharacterSubclassDefinitionBuilder
            .Create("WizardLifeTransmuter")
            .SetGuiPresentation(Category.Subclass,
                RoguishDarkweaver.GuiPresentation.SpriteReference)
            .AddFeaturesAtLevel(2,
                magicAffinityLifeTransmuterHeightened)
            .AddFeaturesAtLevel(6,
                powerSharedPoolLifeTransmuterHealingPool,
                powerSharedPoolLifeTransmuterDarkvision,
                powerSharedPoolLifeTransmuterPoison,
                powerSharedPoolLifeTransmuterConstitution)
            .AddFeaturesAtLevel(10,
                powerPoolModifierLifeTransmuterHealingPoolExtra,
                powerSharedPoolLifeTransmuterFly,
                powerSharedPoolLifeTransmuterHeal,
                powerSharedPoolLifeTransmuterRevive)
            .AddFeaturesAtLevel(14,
                powerPoolModifierLifeTransmuterHealingPoolBonus,
                DamageAffinityNecroticResistance)
            .AddToDB();
    }

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceWizardArcaneTraditions;

    private static FeatureDefinitionPowerSharedPoolBuilder BuildActionTransmuteConditionPower(
        FeatureDefinitionPower poolPower,
        RechargeRate recharge,
        ActivationTime activationTime,
        int costPerUse,
        RangeType rangeType,
        int rangeParameter,
        TargetType targetType,
        ActionDefinitions.ItemSelectionType itemSelectionType,
        DurationType durationType,
        int durationParameter,
        TurnOccurenceType endOfEffect,
        string abilityScore, ConditionDefinition condition,
        string name)
    {
        return FeatureDefinitionPowerSharedPoolBuilder
            .Create(name)
            .Configure(
                poolPower,
                recharge,
                activationTime,
                costPerUse,
                false,
                false,
                abilityScore,
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(
                        Side.Ally,
                        rangeType,
                        rangeParameter,
                        targetType, 1, 0,
                        itemSelectionType)
                    .SetCreatedByCharacter()
                    .SetDurationData(durationType, durationParameter, endOfEffect)
                    .AddEffectForm(EffectFormBuilder
                        .Create()
                        .SetConditionForm(condition, ConditionForm.ConditionOperation.Add, false, false)
                        .CreatedByCharacter()
                        .Build())
                    .SetEffectAdvancement(EffectIncrementMethod.None)
                    .SetParticleEffectParameters(MagicWeapon.EffectDescription.EffectParticleParameters)
                    .Build(),
                false);
    }
}
