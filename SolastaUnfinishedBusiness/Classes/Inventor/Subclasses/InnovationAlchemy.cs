using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Properties;
using SolastaUnfinishedBusiness.Utils;
using UnityEngine.AddressableAssets;
using static RuleDefinitions;
using static RuleDefinitions.EffectIncrementMethod;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;

namespace SolastaUnfinishedBusiness.Classes.Inventor.Subclasses;

public static class InnovationAlchemy
{
    private const string BombsFeatureName = "FeatureInnovationAlchemyBombs";
    private static FeatureDefinitionPower AlchemyPool { get; set; }

    public static CharacterSubclassDefinition Build()
    {
        AlchemyPool = BuildAlchemyPool();

        return CharacterSubclassDefinitionBuilder
            .Create("InnovationAlchemy")
            .SetGuiPresentation(Category.Subclass, CharacterSubclassDefinitions.DomainElementalFire)
            .AddFeaturesAtLevel(3, AlchemyPool, BuildBombs(), BuildFastHands())
            .AddToDB();
    }

    private static FeatureDefinitionActionAffinity BuildFastHands()
    {
        return FeatureDefinitionActionAffinityBuilder
            .Create("ActionAffinityInnovationAlchemyFastHands")
            .SetGuiPresentation(Category.Feature)
            .SetDefaultAllowedActonTypes()
            .SetAuthorizedActions(ActionDefinitions.Id.UseItemBonus)
            .AddToDB();
    }

    private static FeatureDefinition BuildBombs()
    {
        var deviceDescription = new UsableDeviceDescriptionBuilder()
            .SetUsage(EquipmentDefinitions.ItemUsage.Charges)
            .SetRecharge(RechargeRate.ShortRest)
            .SetSaveDc(EffectHelpers.BasedOnUser);

        BuildFireBombs(deviceDescription);

        var bombItem = ItemDefinitionBuilder
            .Create("ItemInnovationAlchemyBomb")
            .SetGuiPresentation(BombsFeatureName, Category.Feature,
                CustomIcons.CreateAssetReferenceSprite("AlchemyFlask", Resources.AlchemyFlask, 128))
            .SetRequiresIdentification(false)
            .SetWeight(0)
            .SetItemPresentation(CustomWeaponsContext.BuildPresentation("ItemAlchemyFunctorUnid",
                ItemDefinitions.ScrollFly.itemPresentation))
            .SetUsableDeviceDescription(deviceDescription.Build())
            .AddToDB();

        return FeatureDefinitionBuilder
            .Create(BombsFeatureName)
            .SetGuiPresentation(Category.Feature)
            .SetCustomSubFeatures(new PowerPoolDevice(bombItem, AlchemyPool))
            .AddToDB();
    }

    private static void BuildFireBombs(UsableDeviceDescriptionBuilder deviceDescription)
    {
        var damage = DamageTypeFire;
        var savingThrowAbility = AttributeDefinitions.Dexterity;
        var dieType = DieType.D8;

        var sprite = SpellDefinitions.Fireball.GuiPresentation.SpriteReference;
        var particleParameters = SpellDefinitions.ProduceFlameHurl.EffectDescription.effectParticleParameters;
        var powerBombSplash = MakeSplashBombPower(damage, dieType, savingThrowAbility, sprite, particleParameters);

        sprite = SpellDefinitions.BurningHands.GuiPresentation.SpriteReference;
        particleParameters = SpellDefinitions.BurningHands.EffectDescription.effectParticleParameters;
        var powerBombBreath = MakeBreathBombPower(damage, dieType, savingThrowAbility, sprite, particleParameters);

        sprite = SpellDefinitions.ProduceFlame.GuiPresentation.SpriteReference;
        particleParameters = SpellDefinitions.ProduceFlameHurl.EffectDescription.effectParticleParameters;
        var powerBombPrecise = MakePreciseBombPower(damage, dieType, savingThrowAbility, sprite, particleParameters);

        deviceDescription.AddFunctions(
            new DeviceFunctionDescriptionBuilder()
                .SetUsage(useAmount: 2, useAffinity: DeviceFunctionDescription.FunctionUseAffinity.ChargeCost)
                .SetPower(powerBombPrecise, true)
                .Build(),
            new DeviceFunctionDescriptionBuilder()
                .SetUsage(useAmount: 2, useAffinity: DeviceFunctionDescription.FunctionUseAffinity.ChargeCost)
                .SetPower(powerBombSplash, true)
                .Build(),
            new DeviceFunctionDescriptionBuilder()
                .SetUsage(useAmount: 3, useAffinity: DeviceFunctionDescription.FunctionUseAffinity.ChargeCost)
                .SetPower(powerBombBreath, true)
                .Build()
        );
    }

    private static FeatureDefinitionPower MakePreciseBombPower(string damageType, DieType dieType,
        string savingThrowAbility, AssetReferenceSprite sprite,
        EffectParticleParameters particleParameters)
    {
        const string name = "PowerInnovationAlchemyBombPrecise";
        return FeatureDefinitionPowerBuilder.Create($"{name}{damageType}")
            .SetGuiPresentation(name, Category.Feature, sprite)
            .SetActivationTime(ActivationTime.Action)
            .SetCostPerUse(1)
            .SetAttackAbilityToHit(true, true)
            .SetExplicitAbilityScore(AttributeDefinitions.Dexterity)
            .SetCustomSubFeatures(PowerVisibilityModifier.Visible, new AddPBToDamage(), new Overcharge())
            .SetEffectDescription(EffectDescriptionBuilder.Create()
                .SetAnimationMagicEffect(AnimationDefinitions.AnimationMagicEffect.Animation1)
                .SetTargetingData(Side.Enemy, RangeType.RangeHit, 8, TargetType.Individuals)
                .SetEffectAdvancement(PerAdditionalSlotLevel, additionalTargetsPerIncrement: 1)
                .SetSavingThrowData(
                    false,
                    true,
                    savingThrowAbility,
                    false,
                    EffectDifficultyClassComputation.AbilityScoreAndProficiency,
                    AttributeDefinitions.Intelligence)
                .SetParticleEffectParameters(particleParameters)
                .SetDurationData(DurationType.Instantaneous)
                .SetEffectForms(EffectFormBuilder
                    .Create()
                    .HasSavingThrow(EffectSavingThrowType.None)
                    .SetDamageForm(dieType: dieType, diceNumber: 3, damageType: damageType)
                    .Build())
                .Build())
            .AddToDB();
    }

    private static FeatureDefinitionPower MakeBreathBombPower(string damageType, DieType dieType,
        string savingThrowAbility,
        AssetReferenceSprite sprite,
        EffectParticleParameters particleParameters)
    {
        const string name = "PowerInnovationAlchemyBombBreath";
        return FeatureDefinitionPowerBuilder.Create($"{name}{damageType}")
            .SetGuiPresentation(name, Category.Feature, sprite)
            .SetActivationTime(ActivationTime.Action)
            .SetCostPerUse(1)
            .SetCustomSubFeatures(PowerVisibilityModifier.Visible, new AddPBToDamage(), new Overcharge())
            .SetEffectDescription(EffectDescriptionBuilder.Create()
                .SetAnimationMagicEffect(AnimationDefinitions.AnimationMagicEffect.Animation0)
                .SetTargetingData(Side.All, RangeType.Self, 0, TargetType.Cone, 4)
                .SetEffectAdvancement(PerAdditionalSlotLevel, additionalDicePerIncrement: 1)
                .SetSavingThrowData(
                    true,
                    true,
                    savingThrowAbility,
                    false,
                    EffectDifficultyClassComputation.AbilityScoreAndProficiency,
                    AttributeDefinitions.Intelligence)
                .SetParticleEffectParameters(particleParameters)
                .SetDurationData(DurationType.Instantaneous)
                .SetEffectForms(EffectFormBuilder.Create()
                    .HasSavingThrow(EffectSavingThrowType.HalfDamage)
                    .SetDamageForm(dieType: dieType, diceNumber: 2, damageType: damageType)
                    .Build())
                .Build())
            .AddToDB();
    }

    private static FeatureDefinitionPower MakeSplashBombPower(string damageType, DieType dieType,
        string savingThrowAbility, AssetReferenceSprite sprite,
        EffectParticleParameters particleParameters)
    {
        const string name = "PowerInnovationAlchemyBombSplash";
        return FeatureDefinitionPowerBuilder.Create($"{name}{damageType}")
            .SetGuiPresentation(name, Category.Feature, sprite)
            .SetActivationTime(ActivationTime.Action)
            .SetCostPerUse(1)
            .SetCustomSubFeatures(PowerVisibilityModifier.Visible, new AddPBToDamage(), new Overcharge())
            .SetEffectDescription(EffectDescriptionBuilder.Create()
                .SetAnimationMagicEffect(AnimationDefinitions.AnimationMagicEffect.Animation1)
                .SetTargetingData(Side.All, RangeType.Distance, 4, TargetType.Sphere, 1)
                .SetEffectAdvancement(PerAdditionalSlotLevel, additionalTargetCellsPerIncrement: 1)
                .SetSavingThrowData(
                    true,
                    true,
                    savingThrowAbility,
                    false,
                    EffectDifficultyClassComputation.AbilityScoreAndProficiency,
                    AttributeDefinitions.Intelligence)
                .SetParticleEffectParameters(particleParameters)
                .SetDurationData(DurationType.Instantaneous)
                .SetEffectForms(EffectFormBuilder.Create()
                    .HasSavingThrow(EffectSavingThrowType.HalfDamage)
                    .SetDamageForm(dieType: dieType, diceNumber: 2, damageType: damageType)
                    .Build())
                .Build())
            .AddToDB();
    }

    private static FeatureDefinitionPower BuildAlchemyPool()
    {
        return FeatureDefinitionPowerBuilder
            .Create("PowerInnovationAlchemyPool")
            .SetGuiPresentation(Category.Feature)
            .SetCustomSubFeatures(PowerVisibilityModifier.Hidden)
            .SetUsesFixed(6)
            .SetRechargeRate(RechargeRate.ShortRest)
            .AddToDB();
    }
}

internal sealed class Overcharge : ICustomOverchargeProvider
{
    private static readonly (int, int)[] None = { };
    private static readonly (int, int)[] Once = {(1, 1)};
    private static readonly (int, int)[] Twice = {(1, 1), (2, 2)};

    public (int, int)[] OverchargeSteps(RulesetCharacter character)
    {
        //TODO: maybe rework to use features instead of levels?
        var classLevel = character.GetClassLevel(InventorClass.Class);
        if (classLevel >= 11)
        {
            return Twice;
        }
        else if (classLevel >= 5)
        {
            return Once;
        }

        return None;
    }
}

internal sealed class AddPBToDamage : IModifyMagicEffect
{
    public EffectDescription ModifyEffect(BaseDefinition definition, EffectDescription effect, RulesetCharacter caster)
    {
        var damage = effect.FindFirstDamageForm();
        if (damage != null)
        {
            damage.bonusDamage += caster.TryGetAttributeValue(AttributeDefinitions.ProficiencyBonus);
        }

        return effect;
    }
}

//TODO: maybe it is better to create full different powers, instead of modifying effect?
internal sealed class MakeCold : IModifyMagicEffect
{
    public EffectDescription ModifyEffect(BaseDefinition definition, EffectDescription effect, RulesetCharacter caster)
    {
        var damage = effect.FindFirstDamageForm();
        if (damage != null)
        {
            damage.damageType = DamageTypeCold;

            var ray = SpellDefinitions.RayOfFrost.EffectDescription.EffectParticleParameters;
            var ice = SpellDefinitions.SleetStorm.EffectDescription.EffectParticleParameters;

            effect.speedType = SpeedType.CellsPerSeconds;
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
