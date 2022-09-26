using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.Models;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;

namespace SolastaUnfinishedBusiness.Classes.Inventor.Subclasses;

public static class InnovationAlchemy
{
    public static CharacterSubclassDefinition Build()
    {
        return CharacterSubclassDefinitionBuilder
            .Create("InnovationAlchemy")
            .SetGuiPresentation(Category.Subclass, CharacterSubclassDefinitions.DomainElementalFire)
            .AddFeaturesAtLevel(1, BuildAlchemy())
            .AddToDB();
    }

    public static FeatureDefinition BuildAlchemy()
    {
        var alchemyPool = FeatureDefinitionPowerBuilder
            .Create("PowerInnovationAlchemyPool")
            .SetGuiPresentation(Category.Power, hidden: true)
            .SetUsesFixed(20)
            .SetRechargeRate(RuleDefinitions.RechargeRate.ShortRest)
            .AddToDB();

        var bombPower = FeatureDefinitionPowerSharedPoolBuilder.Create("PowerSharedPoolAlchemyBomb")
            .SetGuiPresentation(Category.Power, SpellDefinitions.Fireball)
            .SetActivationTime(RuleDefinitions.ActivationTime.Action)
            .SetCostPerUse(1)
            .SetSharedPool(alchemyPool)
            .SetEffectDescription(new EffectDescriptionBuilder()
                .SetTargetingData(RuleDefinitions.Side.All, RuleDefinitions.RangeType.Distance, 12,
                    RuleDefinitions.TargetType.Sphere)
                .SetSavingThrowData(
                    true,
                    true,
                    AttributeDefinitions.Dexterity,
                    false,
                    RuleDefinitions.EffectDifficultyClassComputation.AbilityScoreAndProficiency)
                .SetParticleEffectParameters(SpellDefinitions.FireBolt)
                .SetDurationData(RuleDefinitions.DurationType.Instantaneous)
                .SetEffectForms(new EffectFormBuilder()
                    .HasSavingThrow(RuleDefinitions.EffectSavingThrowType.HalfDamage)
                    .SetDamageForm(dieType: RuleDefinitions.DieType.D6, diceNumber: 2,
                        damageType: RuleDefinitions.DamageTypeFire)
                    .Build())
                .Build())
            .AddToDB();

        var powerItem = ItemDefinitionBuilder
            .Create("ItemAlchemyFunctor")
            .SetGuiPresentation(Category.Item, ItemDefinitions.AlchemistFire)
            .SetItemPresentation(CustomWeaponsContext.BuildPresentation("ItemAlchemyFunctorUnid",
                ItemDefinitions.ScrollFly.itemPresentation))
            .SetUsableDeviceDescription(new UsableDeviceDescriptionBuilder()
                .SetUsage(EquipmentDefinitions.ItemUsage.Charges)
                .SetRecharge(RuleDefinitions.RechargeRate.ShortRest)
                .AddFunctions(
                    new DeviceFunctionDescriptionBuilder()
                        .SetUsage(useAmount: 2, useAffinity: DeviceFunctionDescription.FunctionUseAffinity.ChargeCost)
                        .SetPower(bombPower)
                        .Build(),
                    new DeviceFunctionDescriptionBuilder()
                        .SetUsage(useAmount: 3, useAffinity: DeviceFunctionDescription.FunctionUseAffinity.ChargeCost)
                        .SetSpell(SpellDefinitions.Invisibility, true)
                        .Build()
                )
                .Build())
            .AddToDB();

        alchemyPool.SetCustomSubFeatures(new PowerPoolDevice(powerItem, alchemyPool));

        return FeatureDefinitionFeatureSetBuilder
            .Create("FeatureSetAlchemyInnovationLevel01")
            .SetMode(FeatureDefinitionFeatureSet.FeatureSetMode.Union)
            .AddFeatureSet(
                alchemyPool
            )
            .AddToDB();
    }
}
