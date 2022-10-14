using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterSubclassDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionDamageAffinitys;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ConditionDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;

namespace SolastaUnfinishedBusiness.Subclasses;

internal sealed class WizardLifeTransmuter : AbstractSubclass
{
    internal WizardLifeTransmuter()
    {
        // LEVEL 02
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

        // LEVEL 06
        
        var powerSharedPoolLifeTransmuterHealingPool = FeatureDefinitionPowerBuilder
            .Create("PowerSharedPoolLifeTransmuterHealingPool")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.Permanent, RechargeRate.LongRest, 1, 2)
            .SetIsPowerPool()
            .AddToDB();

        var conditionLifeTransmuterDarkvision = ConditionDefinitionBuilder
            .Create("ConditionLifeTransmuterDarkvision")
            .SetGuiPresentation("PowerSharedPoolLifeTransmuterDarkvision", Category.Feature, ConditionDarkvision)
            .SetFeatures(FeatureDefinitionSenses.SenseSuperiorDarkvision)
            .SetConditionType(ConditionType.Beneficial)
            .SetAllowMultipleInstances(false)
            .SetDuration(DurationType.UntilLongRest)
            .AddToDB();

        var powerSharedPoolLifeTransmuterDarkvision = BuildActionTransmuteConditionPower(
                "PowerSharedPoolLifeTransmuterDarkvision",
                powerSharedPoolLifeTransmuterHealingPool,
                conditionLifeTransmuterDarkvision)
            .SetGuiPresentation(Category.Feature, PowerDomainBattleDivineWrath)
            .AddToDB();

        var conditionLifeTransmuterPoison = ConditionDefinitionBuilder
            .Create("ConditionLifeTransmuterElementalResistance")
            .SetGuiPresentation(Category.Condition, ConditionProtectedFromPoison)
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
            .SetDuration(DurationType.UntilLongRest)
            .AddToDB();

        var powerSharedPoolLifeTransmuterPoison = BuildActionTransmuteConditionPower(
                "PowerSharedPoolLifeTransmuterElementalResistance", 
                powerSharedPoolLifeTransmuterHealingPool,
                conditionLifeTransmuterPoison)
            .SetGuiPresentation(Category.Feature, PowerDomainElementalFireBurst)
            .AddToDB();

        var conditionLifeTransmuterConstitution = ConditionDefinitionBuilder
            .Create("ConditionLifeTransmuterConstitution")
            .SetGuiPresentation(Category.Condition, ConditionBearsEndurance)
            .SetFeatures(FeatureDefinitionSavingThrowAffinitys.SavingThrowAffinityCreedOfArun)
            .SetConditionType(ConditionType.Beneficial)
            .SetAllowMultipleInstances(false)
            .SetDuration(DurationType.UntilLongRest)
            .AddToDB();

        var powerSharedPoolLifeTransmuterConstitution = BuildActionTransmuteConditionPower(
                "PowerSharedPoolLifeTransmuterConstitution",
                powerSharedPoolLifeTransmuterHealingPool,
                conditionLifeTransmuterConstitution)
            .SetGuiPresentation(Category.Feature, PowerPaladinAuraOfCourage)
            .AddToDB();

        // LEVEL 10
        
        var powerSharedPoolLifeTransmuterFly = BuildActionTransmuteConditionPower(
                "PowerSharedPoolLifeTransmuterFly",
                powerSharedPoolLifeTransmuterHealingPool, 
                ConditionFlying12)
            .SetGuiPresentation(Category.Feature, Fly)
            .AddToDB();

        var powerSharedPoolLifeTransmuterHeal = FeatureDefinitionPowerSharedPoolBuilder
            .Create("PowerSharedPoolLifeTransmuterHeal")
            .SetGuiPresentation(Category.Feature, MassHealingWord)
            .SetUsesFixed(ActivationTime.BonusAction, RechargeRate.LongRest)
            .SetEffectDescription(MassHealingWord.EffectDescription, true)
            .SetSharedPool(powerSharedPoolLifeTransmuterHealingPool)
            .AddToDB();

        var powerSharedPoolLifeTransmuterRevive = FeatureDefinitionPowerSharedPoolBuilder
            .Create("PowerSharedPoolLifeTransmuterRevive")
            .SetGuiPresentation(Category.Feature, Revivify)
            .SetUsesFixed(ActivationTime.BonusAction, RechargeRate.LongRest)
            .SetEffectDescription(Revivify.EffectDescription, true)
            .SetSharedPool(powerSharedPoolLifeTransmuterHealingPool)
            .AddToDB();

        var powerPoolModifierLifeTransmuterHealingPoolExtra = FeatureDefinitionPowerUseModifierBuilder
            .Create("PowerPoolModifierLifeTransmuterHealingPoolExtra")
            .SetGuiPresentation(Category.Feature)
            .SetFixedValue(powerSharedPoolLifeTransmuterHealingPool, 2)
            .AddToDB();
        
        // LEVEL 14
        
        var powerPoolModifierLifeTransmuterHealingPoolBonus = FeatureDefinitionPowerUseModifierBuilder
            .Create("PowerPoolModifierLifeTransmuterHealingPoolBonus")
            .SetGuiPresentation(Category.Feature)
            .SetFixedValue(powerSharedPoolLifeTransmuterHealingPool, 4)
            .AddToDB();

        Subclass = CharacterSubclassDefinitionBuilder
            .Create("WizardLifeTransmuter")
            .SetGuiPresentation(Category.Subclass,
                RoguishDarkweaver)
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
        string name,
        FeatureDefinitionPower poolPower,
        ConditionDefinition condition)
    {
        return FeatureDefinitionPowerSharedPoolBuilder
            .Create(name)
            .SetUsesFixed(ActivationTime.BonusAction, RechargeRate.LongRest)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Ally, RangeType.Touch, 2, TargetType.IndividualsUnique)
                    .SetCreatedByCharacter()
                    .SetDurationData(DurationType.UntilLongRest)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .CreatedByCharacter()
                            .SetConditionForm(condition, ConditionForm.ConditionOperation.Add, false, false)
                            .Build())
                    .Build())
            .SetSharedPool(poolPower);
    }
}
