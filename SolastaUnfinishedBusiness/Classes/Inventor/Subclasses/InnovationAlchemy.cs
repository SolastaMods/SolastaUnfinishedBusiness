using SolastaUnfinishedBusiness.Api.Helpers;
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
            .AddFeaturesAtLevel(1, BuildBombs())
            //temporary test feature to grant use device as bonus action
            .AddFeaturesAtLevel(1, FeatureDefinitionActionAffinityBuilder
                .Create("TMPBonusAlchemy")
                .SetGuiPresentationNoContent(true)
                .SetDefaultAllowedActonTypes()
                .SetAuthorizedActions(ActionDefinitions.Id.UseItemBonus)
                .AddToDB())
            .AddToDB();
    }

    private static FeatureDefinition BuildBombs()
    {
        var powerBombSplash = FeatureDefinitionPowerBuilder.Create("PowerAlchemyBombSplash")
            .SetGuiPresentation(Category.Feature, SpellDefinitions.Fireball)
            .SetActivationTime(RuleDefinitions.ActivationTime.Action)
            .SetCostPerUse(1)
            // .SetCustomSubFeatures(new AddDie(), new MakeCone())
            .SetEffectDescription(EffectDescriptionBuilder
                .Create()
                .SetAnimationMagicEffect(AnimationDefinitions.AnimationMagicEffect.Animation1)
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
                    AttributeDefinitions.Intelligence)
                .SetParticleEffectParameters(SpellDefinitions.FireBolt)
                .SetDurationData(RuleDefinitions.DurationType.Instantaneous)
                .SetEffectForms(EffectFormBuilder
                    .Create()
                    .HasSavingThrow(RuleDefinitions.EffectSavingThrowType.HalfDamage)
                    .SetDamageForm(dieType: RuleDefinitions.DieType.D6, diceNumber: 2,
                        damageType: RuleDefinitions.DamageTypeFire)
                    .Build())
                .Build())
            .AddToDB();

        var powerBombBreath = FeatureDefinitionPowerBuilder.Create("PowerAlchemyBombBreath")
            .SetGuiPresentation(Category.Feature, SpellDefinitions.BurningHands)
            .SetActivationTime(RuleDefinitions.ActivationTime.Action)
            .SetCostPerUse(1)
            .SetCustomSubFeatures(new Overcharge())
            .SetEffectDescription(EffectDescriptionBuilder
                .Create()
                .SetAnimationMagicEffect(AnimationDefinitions.AnimationMagicEffect.Animation0)
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
                    AttributeDefinitions.Intelligence)
                .SetParticleEffectParameters(SpellDefinitions.BurningHands)
                .SetDurationData(RuleDefinitions.DurationType.Instantaneous)
                .SetEffectForms(EffectFormBuilder
                    .Create()
                    .HasSavingThrow(RuleDefinitions.EffectSavingThrowType.HalfDamage)
                    .SetDamageForm(dieType: RuleDefinitions.DieType.D6, diceNumber: 2,
                        damageType: RuleDefinitions.DamageTypeFire)
                    .Build())
                .Build())
            .AddToDB();

        var powerBombSingle = FeatureDefinitionPowerBuilder.Create("PowerAlchemyBombSingle")
            .SetGuiPresentation(Category.Feature, SpellDefinitions.ProduceFlame)
            .SetActivationTime(RuleDefinitions.ActivationTime.Action)
            .SetCostPerUse(1)
            .SetAttackAbilityToHit(true, true)
            .SetExplicitAbilityScore(AttributeDefinitions.Dexterity)
            // .SetCustomSubFeatures(new AddDie(), new MakeCone())
            // .SetCustomSubFeatures(new ValidatorPowerUse(_ => false))
            .SetEffectDescription(EffectDescriptionBuilder
                .Create()
                .SetAnimationMagicEffect(AnimationDefinitions.AnimationMagicEffect.Animation1)
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
                    AttributeDefinitions.Intelligence)
                .SetParticleEffectParameters(SpellDefinitions.FireBolt)
                .SetDurationData(RuleDefinitions.DurationType.Instantaneous)
                .SetEffectForms(EffectFormBuilder
                    .Create()
                    .HasSavingThrow(RuleDefinitions.EffectSavingThrowType.None)
                    .SetDamageForm(dieType: RuleDefinitions.DieType.D6, diceNumber: 3,
                        damageType: RuleDefinitions.DamageTypeFire)
                    .Build())
                .Build())
            .AddToDB();

        var bombItem = ItemDefinitionBuilder
            .Create("ItemAlchemyBomb")
            .SetGuiPresentation("FeatureAlchemyBombs", Category.Feature, ItemDefinitions.AlchemistFire)
            .SetRequiresIdentification(true)
            .SetWeight(0)
            .SetItemPresentation(CustomWeaponsContext.BuildPresentation("ItemAlchemyFunctorUnid",
                ItemDefinitions.ScrollFly.itemPresentation))
            .SetUsableDeviceDescription(new UsableDeviceDescriptionBuilder()
                .SetUsage(EquipmentDefinitions.ItemUsage.Charges)
                .SetRecharge(RuleDefinitions.RechargeRate.ShortRest)
                .SetSaveDc(EffectHelpers.BasedOnUser)
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

        return FeatureDefinitionBuilder
            .Create("FeatureAlchemyBombs")
            .SetGuiPresentation(Category.Feature)
            .SetCustomSubFeatures(new PowerPoolDevice(bombItem, InventorClass.InfusionPool))
            .AddToDB();
    }


    //Below is some temp test stuff
    private sealed class Overcharge : ICustomOverchargeProvider
    {
        private static readonly (int, int)[] Steps = { (2, 1), (3, 2) };

        public (int, int)[] OverchargeSteps(RulesetCharacter character)
        {
            return Steps;
        }
    }

    internal sealed class AddDie : IModifyMagicEffect
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

    internal sealed class MakeCold : IModifyMagicEffect
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

    internal sealed class MakeCone : IModifyMagicEffect
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
