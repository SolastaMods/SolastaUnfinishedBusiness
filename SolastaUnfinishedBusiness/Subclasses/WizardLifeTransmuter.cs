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
            .SetGuiPresentation(Category.Feature)
            .SetActivationTime(ActivationTime.Permanent)
            .Configure(
                2,
                UsesDetermination.Fixed,
                AttributeDefinitions.Intelligence,
                RechargeRate.LongRest)
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
                conditionLifeTransmuterDarkvision,
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
                conditionLifeTransmuterPoison,
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
                conditionLifeTransmuterConstitution,
                "PowerSharedPoolLifeTransmuterConstitution")
            .SetGuiPresentation(
                Category.Feature,
                FeatureDefinitionPowers.PowerPaladinAuraOfCourage.GuiPresentation.SpriteReference)
            .AddToDB();

        var powerPoolModifierLifeTransmuterHealingPoolExtra = FeatureDefinitionPowerPoolModifierBuilder
            .Create("PowerPoolModifierLifeTransmuterHealingPoolExtra")
            .SetGuiPresentation(Category.Feature)
            .SetActivationTime(ActivationTime.Permanent)
            .Configure(
                2,
                UsesDetermination.Fixed,
                AttributeDefinitions.Intelligence,
                powerSharedPoolLifeTransmuterHealingPool)
            .AddToDB();

        var powerSharedPoolLifeTransmuterFly = BuildActionTransmuteConditionPower(
                powerSharedPoolLifeTransmuterHealingPool,
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
            .SetActivationTime(ActivationTime.Permanent)
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
        ConditionDefinition condition,
        string name)
    {
        return FeatureDefinitionPowerSharedPoolBuilder
            .Create(name)
            .Configure(
                poolPower,
                RechargeRate.LongRest,
                ActivationTime.BonusAction,
                1,
                false,
                false,
                AttributeDefinitions.Intelligence,
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(
                        Side.Ally,
                        RangeType.Touch,
                        2,
                        TargetType.IndividualsUnique,
                        1,
                        0)
                    .SetCreatedByCharacter()
                    .SetDurationData(DurationType.UntilLongRest)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .CreatedByCharacter()
                            .SetConditionForm(condition, ConditionForm.ConditionOperation.Add, false, false)
                            .Build())
                    .Build(),
                false);
    }
}
