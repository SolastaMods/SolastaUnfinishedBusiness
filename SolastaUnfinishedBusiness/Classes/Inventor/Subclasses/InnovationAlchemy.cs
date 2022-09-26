using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomInterfaces;
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

        var powerBombSplash = FeatureDefinitionPowerSharedPoolBuilder.Create("PowerSharedPoolAlchemyBombSplash")
            .SetGuiPresentation(Category.Power, SpellDefinitions.Fireball)
            .SetActivationTime(RuleDefinitions.ActivationTime.Action)
            .SetCostPerUse(1)
            .SetSharedPool(alchemyPool)
            // .SetCustomSubFeatures(new AddDie(), new MakeCone())
            .SetEffectDescription(new EffectDescriptionBuilder()
                .SetTargetingData(RuleDefinitions.Side.All, RuleDefinitions.RangeType.Distance, 6,
                    RuleDefinitions.TargetType.Sphere)
                .SetEffectAdvancement(RuleDefinitions.EffectIncrementMethod.PerAdditionalSlotLevel,
                    additionalTargetCellsPerIncrement: 1)
                .SetSavingThrowData(
                    true,
                    true,
                    AttributeDefinitions.Dexterity,
                    false,
                    RuleDefinitions.EffectDifficultyClassComputation.AbilityScoreAndProficiency,
                    savingThrowDifficultyAbility: AttributeDefinitions.Intelligence)
                .SetParticleEffectParameters(SpellDefinitions.FireBolt)
                .SetDurationData(RuleDefinitions.DurationType.Instantaneous)
                .SetEffectForms(new EffectFormBuilder()
                    .HasSavingThrow(RuleDefinitions.EffectSavingThrowType.HalfDamage)
                    .SetDamageForm(dieType: RuleDefinitions.DieType.D6, diceNumber: 2,
                        damageType: RuleDefinitions.DamageTypeFire)
                    .Build())
                .Build())
            .AddToDB();

        var powerBombBreath = FeatureDefinitionPowerSharedPoolBuilder.Create("PowerSharedPoolAlchemyBombBreath")
            .SetGuiPresentation(Category.Power, SpellDefinitions.BurningHands)
            .SetActivationTime(RuleDefinitions.ActivationTime.Action)
            .SetCostPerUse(1)
            .SetSharedPool(alchemyPool)
            .SetEffectDescription(new EffectDescriptionBuilder()
                .SetTargetingData(RuleDefinitions.Side.All, RuleDefinitions.RangeType.Distance, 6,
                    RuleDefinitions.TargetType.Cone, 4)
                .SetEffectAdvancement(RuleDefinitions.EffectIncrementMethod.PerAdditionalSlotLevel,
                    additionalTargetCellsPerIncrement: 1)
                .SetSavingThrowData(
                    true,
                    true,
                    AttributeDefinitions.Dexterity,
                    false,
                    RuleDefinitions.EffectDifficultyClassComputation.AbilityScoreAndProficiency,
                    savingThrowDifficultyAbility: AttributeDefinitions.Intelligence)
                .SetParticleEffectParameters(SpellDefinitions.BurningHands)
                .SetDurationData(RuleDefinitions.DurationType.Instantaneous)
                .SetEffectForms(new EffectFormBuilder()
                    .HasSavingThrow(RuleDefinitions.EffectSavingThrowType.HalfDamage)
                    .SetDamageForm(dieType: RuleDefinitions.DieType.D6, diceNumber: 2,
                        damageType: RuleDefinitions.DamageTypeFire)
                    .Build())
                .Build())
            .AddToDB();

        var powerBombSingle = FeatureDefinitionPowerSharedPoolBuilder.Create("PowerSharedPoolAlchemyBombSingle")
            .SetGuiPresentation(Category.Power, SpellDefinitions.ProduceFlame, hidden: true)
            .SetActivationTime(RuleDefinitions.ActivationTime.Action)
            .SetCostPerUse(1)
            .SetSharedPool(alchemyPool)
            .SetAttackAbilityToHit(true, true)
            .SetExplicitAbilityScore(AttributeDefinitions.Dexterity)
            // .SetCustomSubFeatures(new AddDie(), new MakeCone())
            // .SetCustomSubFeatures(new ValidatorPowerUse(_ => false))
            .SetEffectDescription(new EffectDescriptionBuilder()
                .SetTargetingData(RuleDefinitions.Side.Enemy, RuleDefinitions.RangeType.RangeHit, 6,
                    RuleDefinitions.TargetType.Individuals)
                .SetEffectAdvancement(RuleDefinitions.EffectIncrementMethod.PerAdditionalSlotLevel,
                    additionalTargetsPerIncrement: 1)
                .SetSavingThrowData(
                    false,
                    true,
                    AttributeDefinitions.Dexterity,
                    false,
                    RuleDefinitions.EffectDifficultyClassComputation.AbilityScoreAndProficiency,
                    savingThrowDifficultyAbility: AttributeDefinitions.Intelligence)
                .SetParticleEffectParameters(SpellDefinitions.FireBolt)
                .SetDurationData(RuleDefinitions.DurationType.Instantaneous)
                .SetEffectForms(new EffectFormBuilder()
                    .HasSavingThrow(RuleDefinitions.EffectSavingThrowType.None)
                    .SetDamageForm(dieType: RuleDefinitions.DieType.D6, diceNumber: 3,
                        damageType: RuleDefinitions.DamageTypeFire)
                    .Build())
                .Build())
            .AddToDB();

        var bombItem = ItemDefinitionBuilder
            .Create("ItemAlchemyBomb")
            .SetGuiPresentation(Category.Item, ItemDefinitions.AlchemistFire)
            .SetRequiresIdentification(true)
            .SetWeight(0)
            .SetItemPresentation(CustomWeaponsContext.BuildPresentation("ItemAlchemyFunctorUnid",
                ItemDefinitions.ScrollFly.itemPresentation))
            .SetUsableDeviceDescription(new UsableDeviceDescriptionBuilder()
                .SetUsage(EquipmentDefinitions.ItemUsage.Charges)
                .SetRecharge(RuleDefinitions.RechargeRate.ShortRest)
                .SetSaveDc(-1) //Set to -1 so that it will calculate based on actual powers
                .AddFunctions(
                    new DeviceFunctionDescriptionBuilder()
                        .SetUsage(useAmount: 2, useAffinity: DeviceFunctionDescription.FunctionUseAffinity.ChargeCost)
                        .SetPower(powerBombSplash, true)
                        .Build(),
                    new DeviceFunctionDescriptionBuilder()
                        .SetUsage(useAmount: 3, useAffinity: DeviceFunctionDescription.FunctionUseAffinity.ChargeCost)
                        .SetPower(powerBombBreath, true)
                        .Build(),
                    new DeviceFunctionDescriptionBuilder()
                        .SetUsage(useAmount: 2, useAffinity: DeviceFunctionDescription.FunctionUseAffinity.ChargeCost)
                        .SetPower(powerBombSingle, true)
                        .Build()
                )
                .Build())
            .AddToDB();

        alchemyPool.SetCustomSubFeatures(new PowerPoolDevice(bombItem, alchemyPool));

        return FeatureDefinitionFeatureSetBuilder
            .Create("FeatureSetAlchemyInnovationLevel01")
            .SetMode(FeatureDefinitionFeatureSet.FeatureSetMode.Union)
            .AddFeatureSet(
                alchemyPool,
                FeatureDefinitionActionAffinityBuilder //temporary test feature to grant use device as bonus action
                    .Create("TMPBonusAlcheny")
                    .SetDefaultAllowedActonTypes()
                    // .SetCustomSubFeatures(new MakeCold())
                    .SetAuthorizedActions(ActionDefinitions.Id.UseItemBonus)
                    .AddToDB()
            )
            .AddToDB();
    }


    //Below is some temp test stuff
    internal sealed class AddDie : IModifySpellEffect
    {
        public EffectDescription ModifyEffect(BaseDefinition definition, EffectDescription effect,
            RulesetCharacter caster)
        {
            var damage = effect.FindFirstDamageForm();
            if (damage != null)
            {
                damage.diceNumber += 1;
            }

            return effect;
        }
    }

    internal sealed class MakeCold : IModifySpellEffect
    {
        public EffectDescription ModifyEffect(BaseDefinition definition, EffectDescription effect,
            RulesetCharacter caster)
        {
            var damage = effect.FindFirstDamageForm();
            if (damage != null)
            {
                damage.damageType = RuleDefinitions.DamageTypeCold;

                var ray = SpellDefinitions.RayOfFrost.EffectDescription.EffectParticleParameters;
                var ice = SpellDefinitions.SleetStorm.EffectDescription.EffectParticleParameters;

                effect.speedType = RuleDefinitions.SpeedType.CellsPerSeconds;
                effect.speedParameter = 8;

                var particles = new EffectParticleParameters();
                particles.Copy(ray);
                particles.casterParticleReference = null; //new AssetReference();
                particles.activeEffectCellStartParticleReference = ice.activeEffectCellStartParticleReference;
                particles.activeEffectCellParticleReference = ice.activeEffectCellParticleReference;
                particles.activeEffectSurfaceParticleReference = ice.activeEffectSurfaceParticleReference;
                particles.emissiveBorderSurfaceParticleReference = ice.activeEffectSurfaceParticleReference;
                // particles.forceApplyZoneParticle = true;
                effect.animationMagicEffect = AnimationDefinitions.AnimationMagicEffect.Animation1;

                effect.effectParticleParameters = particles;
                // effect.SetEffectParticleParameters(SpellDefinitions.RayOfFrost);
                // effect.SetEffectParticleParameters(SpellDefinitions.IceStorm);
            }

            return effect;
        }
    }

    internal sealed class MakeCone : IModifySpellEffect
    {
        public EffectDescription ModifyEffect(BaseDefinition definition, EffectDescription effect,
            RulesetCharacter caster)
        {
            effect.TargetType = RuleDefinitions.TargetType.Cone;
            effect.targetParameter = 3;
            effect.targetParameter2 = 2;

            return effect;
        }
    }
}