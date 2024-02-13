using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Behaviors;
using SolastaUnfinishedBusiness.Behaviors.Specific;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.Classes;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Properties;
using SolastaUnfinishedBusiness.Spells;
using SolastaUnfinishedBusiness.Validators;
using UnityEngine.AddressableAssets;
using static RuleDefinitions;
using static RuleDefinitions.EffectIncrementMethod;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;

namespace SolastaUnfinishedBusiness.Subclasses;

[UsedImplicitly]
public sealed class InnovationAlchemy : AbstractSubclass
{
    private const string BombsFeatureName = "FeatureInnovationAlchemyBombs";

    public InnovationAlchemy()
    {
        AlchemyPool = BuildAlchemyPool();

        Subclass = CharacterSubclassDefinitionBuilder
            .Create("InnovationAlchemy")
            .SetGuiPresentation(Category.Subclass,
                Sprites.GetSprite("InventorAlchemist", Resources.InventorAlchemist, 256))
            .AddFeaturesAtLevel(3, AlchemyPool, BuildBombs(), BuildFastHands(), BuildAutoPreparedSpells())
            .AddFeaturesAtLevel(5, ElementalBombs, BuildOverchargeFeature(), BuildRefundPool(AlchemyPool))
            .AddFeaturesAtLevel(9, AdvancedBombs, BuildExtraOverchargeFeature())
            .AddFeaturesAtLevel(15, BuildMasterOverchargeFeature())
            .AddToDB();
    }

    private static FeatureDefinitionPower AlchemyPool { get; set; }
    private static FeatureDefinitionPower ElementalBombs { get; set; }
    private static FeatureDefinitionPower AdvancedBombs { get; set; }

    internal override CharacterSubclassDefinition Subclass { get; }
    internal override CharacterClassDefinition Klass => InventorClass.Class;
    internal override FeatureDefinitionSubclassChoice SubclassChoice => InventorClass.SubclassChoice;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }

    private static FeatureDefinitionAutoPreparedSpells BuildAutoPreparedSpells()
    {
        return FeatureDefinitionAutoPreparedSpellsBuilder
            .Create("AutoPreparedSpellsInnovationAlchemy")
            .SetGuiPresentation(Category.Feature)
            .SetSpellcastingClass(InventorClass.Class)
            .SetAutoTag("InventorGrenadier")
            .AddPreparedSpellGroup(3, MagicMissile, Thunderwave)
            .AddPreparedSpellGroup(5, Shatter, Blindness)
            .AddPreparedSpellGroup(9, Fireball, StinkingCloud)
            .AddPreparedSpellGroup(13, Confusion, FireShield)
            .AddPreparedSpellGroup(17, CloudKill, ConeOfCold)
            .AddToDB();
    }

    private static FeatureDefinitionActionAffinity BuildFastHands()
    {
        return FeatureDefinitionActionAffinityBuilder
            .Create("ActionAffinityInnovationAlchemyFastHands")
            .SetGuiPresentation(Category.Feature)
            .SetAuthorizedActions(ActionDefinitions.Id.UseItemBonus)
            .AddToDB();
    }

    private static FeatureDefinition BuildBombs()
    {
        var deviceDescriptionBuilder = new UsableDeviceDescriptionBuilder()
            .SetUsage(EquipmentDefinitions.ItemUsage.Charges)
            .SetRecharge(RechargeRate.ShortRest)
            .SetSaveDc(EffectHelpers.BasedOnUser);

        BuildFireBombs(deviceDescriptionBuilder);

        ElementalBombs = FeatureDefinitionPowerBuilder
            .Create("PowerInnovationAlchemyBombsElemental")
            .SetGuiPresentation(Category.Feature,
                Sprites.GetSprite("AlchemyBombElement", Resources.AlchemyBombElement, 256, 128))
            .SetUsesFixed(ActivationTime.NoCost)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Permanent)
                    .Build())
            .SetUniqueInstance()
            .AddToDB();

        PowerBundle.RegisterPowerBundle(ElementalBombs, true,
            MakeBombFireDamageToggle(),
            BuildColdBombs(deviceDescriptionBuilder),
            BuildLightningBombs(deviceDescriptionBuilder),
            BuildAcidBombs(deviceDescriptionBuilder),
            BuildPoisonBombs(deviceDescriptionBuilder)
        );

        //TODO: maybe make elemental and advanced bombs not exclusive?
        //Like show bomb forms for 1 element and 1 advanced element at same time?
        //Or maybe make global limit of 2 elements at once, regardless of whether they are simple or advanced?
        AdvancedBombs = FeatureDefinitionPowerBuilder
            .Create("PowerInnovationAlchemyBombsAdvanced")
            .SetGuiPresentation(Category.Feature,
                Sprites.GetSprite("AlchemyBombAdvanced", Resources.AlchemyBombAdvanced, 256, 128))
            .SetUsesFixed(ActivationTime.NoCost)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Permanent)
                    .Build())
            .SetUniqueInstance()
            .AddToDB();

        PowerBundle.RegisterPowerBundle(AdvancedBombs, true,
            BuildForceBombs(deviceDescriptionBuilder),
            BuildRadiantBombs(deviceDescriptionBuilder),
            BuildNecroBombs(deviceDescriptionBuilder),
            BuildThunderBombs(deviceDescriptionBuilder),
            BuildPsychicBombs(deviceDescriptionBuilder)
        );

        var deviceDescription = deviceDescriptionBuilder.Build();
        var bombItem = ItemDefinitionBuilder
            .Create("ItemInnovationAlchemyBomb")
            .SetGuiPresentation(BombsFeatureName, Category.Feature,
                Sprites.GetSprite("AlchemyFlask", Resources.AlchemyFlask, 128))
            .SetRequiresIdentification(false)
            .SetWeight(0)
            .SetItemPresentation(CustomWeaponsContext.BuildPresentation("ItemAlchemyFunctorUnidentified",
                ItemDefinitions.ScrollFly.itemPresentation))
            .SetUsableDeviceDescription(deviceDescription)
            // required for multiclass use cases
            .AddCustomSubFeatures(ModifyAdditionalDamageClassLevelInventor.Instance)
            .AddToDB();

        return FeatureDefinitionBuilder
            .Create(BombsFeatureName)
            .SetGuiPresentation(Category.Feature)
            .AddCustomSubFeatures(new PowerPoolDevice(bombItem, AlchemyPool))
            .AddToDB();
    }

    private static void BuildFireBombs(UsableDeviceDescriptionBuilder deviceDescription)
    {
        // ReSharper disable once InlineTemporaryVariable
        const string DAMAGE = DamageTypeFire;
        const string SAVE = AttributeDefinitions.Dexterity;
        const DieType DIE_TYPE = DieType.D8;
        var validator =
            new ValidatorsValidatePowerUse(character =>
                !character.HasConditionWithSubFeatureOfType<ModifiedBombElement>());

        var sprite = Sprites.GetSprite("AlchemyBombFireSplash", Resources.AlchemyBombFireSplash, 128);
        var particle = ProduceFlameHurl.EffectDescription.effectParticleParameters;
        var powerBombSplash = MakeSplashBombPower(DAMAGE, DIE_TYPE, SAVE, sprite, particle, validator);

        sprite = Sprites.GetSprite("AlchemyBombFireBreath", Resources.AlchemyBombFireBreath, 128);
        particle = BurningHands.EffectDescription.effectParticleParameters;
        var powerBombBreath = MakeBreathBombPower(DAMAGE, DIE_TYPE, SAVE, sprite, particle, validator);

        sprite = Sprites.GetSprite("AlchemyBombFirePrecise", Resources.AlchemyBombFirePrecise, 128);
        particle = ProduceFlameHurl.EffectDescription.effectParticleParameters;
        var powerBombPrecise = MakePreciseBombPower(DAMAGE, DIE_TYPE, sprite, particle, validator);

        AddBombFunctions(deviceDescription, powerBombPrecise, powerBombSplash, powerBombBreath);
    }

    private static FeatureDefinitionPower BuildColdBombs(UsableDeviceDescriptionBuilder deviceDescription)
    {
        // ReSharper disable once InlineTemporaryVariable
        const string DAMAGE = DamageTypeCold;
        const string SAVE = AttributeDefinitions.Constitution;
        const DieType DIE_TYPE = DieType.D6;
        var (toggle, validator) = MakeElementToggleMarker(DAMAGE);
        var effect = EffectFormBuilder.Create()
            .HasSavingThrow(EffectSavingThrowType.Negates)
            .SetConditionForm(ConditionDefinitions.ConditionHindered_By_Frost, ConditionForm.ConditionOperation.Add)
            .Build();

        var sprite = Sprites.GetSprite("AlchemyBombColdSplash", Resources.AlchemyBombColdSplash, 128);
        var particle = ConeOfCold.EffectDescription.effectParticleParameters;
        var powerBombSplash = MakeSplashBombPower(DAMAGE, DIE_TYPE, SAVE, sprite, particle, validator, effect);

        sprite = Sprites.GetSprite("AlchemyBombColdBreath", Resources.AlchemyBombColdBreath, 128);
        particle = ConeOfCold.EffectDescription.effectParticleParameters;
        var powerBombBreath = MakeBreathBombPower(DAMAGE, DIE_TYPE, SAVE, sprite, particle, validator, effect);

        sprite = Sprites.GetSprite("AlchemyBombColdPrecise", Resources.AlchemyBombColdPrecise, 128);
        particle = RayOfFrost.EffectDescription.effectParticleParameters;
        var powerBombPrecise = MakePreciseBombPower(DAMAGE, DIE_TYPE, sprite, particle, validator, effect);

        AddBombFunctions(deviceDescription, powerBombPrecise, powerBombSplash, powerBombBreath);

        return toggle;
    }

    private static FeatureDefinitionPower BuildLightningBombs(UsableDeviceDescriptionBuilder deviceDescription)
    {
        // ReSharper disable once InlineTemporaryVariable
        const string DAMAGE = DamageTypeLightning;
        const string SAVE = AttributeDefinitions.Dexterity;
        const DieType DIE_TYPE = DieType.D6;
        var (toggle, validator) = MakeElementToggleMarker(DAMAGE);
        var effect = EffectFormBuilder
            .Create()
            .HasSavingThrow(EffectSavingThrowType.Negates)
            .SetConditionForm(
                ConditionDefinitionBuilder
                    .Create($"ConditionInnovationAlchemy{DAMAGE}")
                    .SetFeatures(FeatureDefinitionActionAffinitys.ActionAffinityConditionShocked)
                    .SetGuiPresentation(ConditionDefinitions.ConditionShocked.GuiPresentation)
                    .SetConditionType(ConditionType.Detrimental)
                    .SetSpecialDuration(DurationType.Round, 1)
                    .AddToDB(), ConditionForm.ConditionOperation.Add)
            .Build();

        var sprite = Sprites.GetSprite("AlchemyBombLightningSplash", Resources.AlchemyBombLightningSplash, 128);
        var particle = ShockingGrasp.EffectDescription.effectParticleParameters;
        var powerBombSplash = MakeSplashBombPower(DAMAGE, DIE_TYPE, SAVE, sprite, particle, validator, effect);

        sprite = Sprites.GetSprite("AlchemyBombLightningBreath", Resources.AlchemyBombLightningBreath, 128);
        particle = LightningBolt.EffectDescription.effectParticleParameters;
        var powerBombBreath = MakeBreathBombPower(DAMAGE, DIE_TYPE, SAVE, sprite, particle, validator, effect);

        sprite = Sprites.GetSprite("AlchemyBombLightningPrecise", Resources.AlchemyBombLightningPrecise, 128);
        particle = CallLightning.EffectDescription.effectParticleParameters;
        var powerBombPrecise = MakePreciseBombPower(DAMAGE, DIE_TYPE, sprite, particle, validator, effect);

        AddBombFunctions(deviceDescription, powerBombPrecise, powerBombSplash, powerBombBreath);

        return toggle;
    }

    private static FeatureDefinitionPower BuildPoisonBombs(UsableDeviceDescriptionBuilder deviceDescription)
    {
        // ReSharper disable once InlineTemporaryVariable
        const string DAMAGE = DamageTypePoison;
        const string SAVE = AttributeDefinitions.Constitution;
        const DieType DIE_TYPE = DieType.D6;
        var (toggle, validator) = MakeElementToggleMarker(DAMAGE);
        var poisoned = ConditionDefinitions.ConditionPoisoned.GuiPresentation;
        var effect = EffectFormBuilder.Create()
            .HasSavingThrow(EffectSavingThrowType.Negates)
            .SetConditionForm(
                ConditionDefinitionBuilder
                    .Create($"ConditionInnovationAlchemy{DAMAGE}")
                    .SetGuiPresentation(poisoned.Title, "Condition/&ConditionInnovationAlchemyDamagePoisonDescription",
                        poisoned.SpriteReference)
                    .SetConditionType(ConditionType.Detrimental)
                    .SetFeatures(FeatureDefinitionCombatAffinitys.CombatAffinityPoisoned)
                    .SetSpecialDuration(DurationType.Round, 1)
                    .SetSpecialInterruptions(ConditionInterruption.Attacks)
                    .AddToDB(), ConditionForm.ConditionOperation.Add)
            .Build();

        var spray = PoisonSpray.EffectDescription.effectParticleParameters;

        var sprite = Sprites.GetSprite("AlchemyBombPoisonSplash", Resources.AlchemyBombPoisonSplash, 128);
        var particle = new EffectParticleParameters();
        particle.Copy(FeatureDefinitionPowers.PowerSpiderQueenPoisonCloud.EffectDescription.effectParticleParameters);
        particle.targetParticleReference = spray.targetParticleReference;
        particle.casterParticleReference = new AssetReference();
        var powerBombSplash = MakeSplashBombPower(DAMAGE, DIE_TYPE, SAVE, sprite, particle, validator, effect);

        sprite = Sprites.GetSprite("AlchemyBombPoisonBreath", Resources.AlchemyBombPoisonBreath, 128);
        particle = FeatureDefinitionPowers.PowerDragonBreath_Poison.EffectDescription.effectParticleParameters;
        var powerBombBreath = MakeBreathBombPower(DAMAGE, DIE_TYPE, SAVE, sprite, particle, validator, effect);

        sprite = Sprites.GetSprite("AlchemyBombPoisonPrecise", Resources.AlchemyBombPoisonPrecise, 128);
        particle = spray;
        var powerBombPrecise = MakePreciseBombPower(DAMAGE, DIE_TYPE, sprite, particle, validator, effect);

        AddBombFunctions(deviceDescription, powerBombPrecise, powerBombSplash, powerBombBreath);

        return toggle;
    }

    private static FeatureDefinitionPower BuildAcidBombs(UsableDeviceDescriptionBuilder deviceDescription)
    {
        // ReSharper disable once InlineTemporaryVariable
        const string DAMAGE = DamageTypeAcid;
        const string SAVE = AttributeDefinitions.Constitution;
        const DieType DIE_TYPE = DieType.D6;
        var (toggle, validator) = MakeElementToggleMarker(DAMAGE);
        var effect = EffectFormBuilder.Create()
            .HasSavingThrow(EffectSavingThrowType.Negates)
            .SetConditionForm(SpellBuilders.AcidClawCondition, ConditionForm.ConditionOperation.Add)
            .Build();

        var splash = AcidSplash.EffectDescription.effectParticleParameters;

        var sprite = Sprites.GetSprite("AlchemyBombAcidSplash", Resources.AlchemyBombAcidSplash, 128);
        var particle = splash;
        var powerBombSplash = MakeSplashBombPower(DAMAGE, DIE_TYPE, SAVE, sprite, particle, validator, effect);

        sprite = Sprites.GetSprite("AlchemyBombAcidBreath", Resources.AlchemyBombAcidBreath, 128);
        particle = FeatureDefinitionPowers.PowerDragonBreath_Acid.EffectDescription.effectParticleParameters;
        var powerBombBreath = MakeBreathBombPower(DAMAGE, DIE_TYPE, SAVE, sprite, particle, validator, effect);

        sprite = Sprites.GetSprite("AlchemyBombAcidPrecise", Resources.AlchemyBombAcidPrecise, 128);
        particle = splash;
        var powerBombPrecise = MakePreciseBombPower(DAMAGE, DIE_TYPE, sprite, particle, validator, effect);

        AddBombFunctions(deviceDescription, powerBombPrecise, powerBombSplash, powerBombBreath);

        return toggle;
    }

    private static FeatureDefinitionPower BuildForceBombs(UsableDeviceDescriptionBuilder deviceDescription)
    {
        // ReSharper disable once InlineTemporaryVariable
        const string DAMAGE = DamageTypeForce;
        const string SAVE = AttributeDefinitions.Strength;
        const DieType DIE_TYPE = DieType.D6;
        var (toggle, validator) = MakeElementToggleMarker(DAMAGE);
        var effect = EffectFormBuilder.Create()
            .HasSavingThrow(EffectSavingThrowType.Negates)
            .SetMotionForm(MotionForm.MotionType.PushFromOrigin, 2)
            .Build();

        var splash = MagicMissile.EffectDescription.effectParticleParameters;

        var sprite = Sprites.GetSprite("AlchemyBombForceSplash", Resources.AlchemyBombForceSplash, 128);
        var particle = splash;
        var powerBombSplash = MakeSplashBombPower(DAMAGE, DIE_TYPE, SAVE, sprite, particle, validator, effect);
        powerBombSplash.AddCustomSubFeatures(ForcePushOrDragFromEffectPoint.Marker);

        sprite = Sprites.GetSprite("AlchemyBombForceBreath", Resources.AlchemyBombForceBreath, 128);
        particle = WallOfForce.EffectDescription.effectParticleParameters;
        var powerBombBreath = MakeBreathBombPower(DAMAGE, DIE_TYPE, SAVE, sprite, particle, validator, effect);

        sprite = Sprites.GetSprite("AlchemyBombForcePrecise", Resources.AlchemyBombForcePrecise, 128);
        particle = splash;
        var powerBombPrecise = MakePreciseBombPower(DAMAGE, DIE_TYPE, sprite, particle, validator, effect);

        AddBombFunctions(deviceDescription, powerBombPrecise, powerBombSplash, powerBombBreath);

        return toggle;
    }

    private static FeatureDefinitionPower BuildRadiantBombs(UsableDeviceDescriptionBuilder deviceDescription)
    {
        var branded = ConditionDefinitions.ConditionBranded.GuiPresentation;
        // ReSharper disable once InlineTemporaryVariable
        const string DAMAGE = DamageTypeRadiant;
        const string SAVE = AttributeDefinitions.Charisma;
        const DieType DIE_TYPE = DieType.D6;
        var (toggle, validator) = MakeElementToggleMarker(DAMAGE);
        var effect = EffectFormBuilder.Create()
            .HasSavingThrow(EffectSavingThrowType.Negates)
            .SetConditionForm(
                ConditionDefinitionBuilder
                    .Create($"ConditionInnovationAlchemy{DAMAGE}")
                    .SetGuiPresentation(branded)
                    .SetConditionType(ConditionType.Detrimental)
                    .SetFeatures(FeatureDefinitionCombatAffinitys.CombatAffinityParalyzedAdvantage)
                    .SetSpecialDuration(DurationType.Round, 1)
                    .SetSpecialInterruptions(ConditionInterruption.Attacked)
                    .AddToDB(),
                ConditionForm.ConditionOperation.Add)
            .Build();

        var splash = Sparkle.EffectDescription.effectParticleParameters;

        var sprite = Sprites.GetSprite("AlchemyBombRadiantSplash", Resources.AlchemyBombRadiantSplash, 128);
        var particle = splash;
        var powerBombSplash = MakeSplashBombPower(DAMAGE, DIE_TYPE, SAVE, sprite, particle, validator, effect);
        powerBombSplash.AddCustomSubFeatures(ForcePushOrDragFromEffectPoint.Marker);

        sprite = Sprites.GetSprite("AlchemyBombRadiantBreath", Resources.AlchemyBombRadiantBreath, 128);
        particle = BurningHands_B.EffectDescription.effectParticleParameters;
        var powerBombBreath = MakeBreathBombPower(DAMAGE, DIE_TYPE, SAVE, sprite, particle, validator, effect);

        sprite = Sprites.GetSprite("AlchemyBombRadiantPrecise", Resources.AlchemyBombRadiantPrecise, 128);
        particle = splash;
        var powerBombPrecise = MakePreciseBombPower(DAMAGE, DIE_TYPE, sprite, particle, validator, effect);

        AddBombFunctions(deviceDescription, powerBombPrecise, powerBombSplash, powerBombBreath);

        return toggle;
    }

    private static FeatureDefinitionPower BuildNecroBombs(UsableDeviceDescriptionBuilder deviceDescription)
    {
        // ReSharper disable once InlineTemporaryVariable
        const string DAMAGE = DamageTypeNecrotic;
        const string SAVE = AttributeDefinitions.Constitution;
        const DieType DIE_TYPE = DieType.D6;
        var (toggle, validator) = MakeElementToggleMarker(DAMAGE);
        var effect = EffectFormBuilder
            .Create()
            .HasSavingThrow(EffectSavingThrowType.Negates)
            .SetConditionForm(
                ConditionDefinitionBuilder
                    .Create($"ConditionInnovationAlchemy{DAMAGE}")
                    .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionDoomLaughter)
                    .SetConditionType(ConditionType.Detrimental)
                    .SetFeatures(FeatureDefinitionHealingModifiers.HealingModifierChilledByTouch)
                    .SetRecurrentEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetDamageForm(DAMAGE, dieType: DIE_TYPE, diceNumber: 2)
                            .SetCreatedBy()
                            .Build())
                    .SetSpecialDuration(DurationType.Round, 1)
                    .AddToDB(), ConditionForm.ConditionOperation.Add)
            .Build();

        var splash = VampiricTouch.EffectDescription.effectParticleParameters;

        var sprite = Sprites.GetSprite("AlchemyBombNecroticSplash", Resources.AlchemyBombNecroticSplash, 128);
        var particle = splash;
        var powerBombSplash = MakeSplashBombPower(DAMAGE, DIE_TYPE, SAVE, sprite, particle, validator, effect);
        powerBombSplash.AddCustomSubFeatures(ForcePushOrDragFromEffectPoint.Marker);

        sprite = Sprites.GetSprite("AlchemyBombNecroticBreath", Resources.AlchemyBombNecroticBreath, 128);
        particle = VampiricTouch.EffectDescription.effectParticleParameters;
        var powerBombBreath = MakeBreathBombPower(DAMAGE, DIE_TYPE, SAVE, sprite, particle, validator, effect);

        sprite = Sprites.GetSprite("AlchemyBombNecroticPrecise", Resources.AlchemyBombNecroticPrecise, 128);
        particle = splash;
        var powerBombPrecise = MakePreciseBombPower(DAMAGE, DIE_TYPE, sprite, particle, validator, effect);

        AddBombFunctions(deviceDescription, powerBombPrecise, powerBombSplash, powerBombBreath);

        return toggle;
    }

    private static FeatureDefinitionPower BuildThunderBombs(UsableDeviceDescriptionBuilder deviceDescription)
    {
        // ReSharper disable once InlineTemporaryVariable
        const string DAMAGE = DamageTypeThunder;
        const string SAVE = AttributeDefinitions.Constitution;
        const DieType DIE_TYPE = DieType.D6;
        var (toggle, validator) = MakeElementToggleMarker(DAMAGE);
        var effect = EffectFormBuilder.Create()
            .HasSavingThrow(EffectSavingThrowType.Negates)
            .SetConditionForm(
                ConditionDefinitionBuilder
                    .Create($"ConditionInnovationAlchemy{DAMAGE}")
                    .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionDazzled)
                    .SetConditionType(ConditionType.Detrimental)
                    .SetFeatures(
                        FeatureDefinitionSavingThrowAffinityBuilder
                            .Create($"SavingThrowAffinityInnovationAlchemy{DAMAGE}")
                            .SetGuiPresentation($"ConditionInnovationAlchemy{DAMAGE}", Category.Condition,
                                Gui.NoLocalization)
                            .SetModifiers(FeatureDefinitionSavingThrowAffinity.ModifierType.RemoveDice, DieType.D4, 1,
                                false,
                                AttributeDefinitions.Intelligence, AttributeDefinitions.Wisdom,
                                AttributeDefinitions.Charisma)
                            .AddToDB())
                    .SetSpecialDuration(DurationType.Round, 1)
                    .AddToDB(),
                ConditionForm.ConditionOperation.Add)
            .Build();

        var splash = Shatter.EffectDescription.effectParticleParameters;

        var sprite = Sprites.GetSprite("AlchemyBombThunderSplash", Resources.AlchemyBombThunderSplash, 128);
        var particle = splash;
        var powerBombSplash = MakeSplashBombPower(DAMAGE, DIE_TYPE, SAVE, sprite, particle, validator, effect);
        powerBombSplash.AddCustomSubFeatures(ForcePushOrDragFromEffectPoint.Marker);

        sprite = Sprites.GetSprite("AlchemyBombThunderBreath", Resources.AlchemyBombThunderBreath, 128);
        particle = Thunderwave.EffectDescription.effectParticleParameters;
        var powerBombBreath = MakeBreathBombPower(DAMAGE, DIE_TYPE, SAVE, sprite, particle, validator, effect);

        sprite = Sprites.GetSprite("AlchemyBombThunderPrecise", Resources.AlchemyBombThunderPrecise, 128);
        particle = splash;
        var powerBombPrecise = MakePreciseBombPower(DAMAGE, DIE_TYPE, sprite, particle, validator, effect);

        AddBombFunctions(deviceDescription, powerBombPrecise, powerBombSplash, powerBombBreath);

        return toggle;
    }

    private static FeatureDefinitionPower BuildPsychicBombs(UsableDeviceDescriptionBuilder deviceDescription)
    {
        // ReSharper disable once InlineTemporaryVariable
        const string DAMAGE = DamageTypePsychic;
        const string SAVE = AttributeDefinitions.Wisdom;
        const DieType DIE_TYPE = DieType.D6;
        var (toggle, validator) = MakeElementToggleMarker(DAMAGE);
        var effect = EffectFormBuilder.Create()
            .HasSavingThrow(EffectSavingThrowType.Negates)
            .SetConditionForm(
                ConditionDefinitionBuilder
                    .Create($"ConditionInnovationAlchemy{DAMAGE}")
                    .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionConfused)
                    .SetConditionType(ConditionType.Detrimental)
                    .SetFeatures(
                        FeatureDefinitionSavingThrowAffinityBuilder
                            .Create($"SavingThrowAffinityInnovationAlchemy{DAMAGE}")
                            .SetGuiPresentation($"ConditionInnovationAlchemy{DAMAGE}", Category.Condition,
                                Gui.NoLocalization)
                            .SetModifiers(FeatureDefinitionSavingThrowAffinity.ModifierType.RemoveDice, DieType.D4, 1,
                                false,
                                AttributeDefinitions.Strength, AttributeDefinitions.Dexterity,
                                AttributeDefinitions.Constitution)
                            .AddToDB())
                    .SetSpecialDuration(DurationType.Round, 1)
                    .AddToDB(),
                ConditionForm.ConditionOperation.Add)
            .Build();

        var splash = new EffectParticleParameters();
        splash.Copy(PhantasmalKiller.EffectDescription.effectParticleParameters);
        splash.targetParticleReference = splash.effectParticleReference;
        splash.effectParticleReference = new AssetReference();

        var sprite = Sprites.GetSprite("AlchemyBombPsychicSplash", Resources.AlchemyBombPsychicSplash, 128);
        var particle = splash;
        var powerBombSplash = MakeSplashBombPower(DAMAGE, DIE_TYPE, SAVE, sprite, particle, validator, effect);
        powerBombSplash.AddCustomSubFeatures(ForcePushOrDragFromEffectPoint.Marker);

        sprite = Sprites.GetSprite("AlchemyBombPsychicBreath", Resources.AlchemyBombPsychicBreath, 128);
        particle = splash; //PhantasmalKiller.EffectDescription.effectParticleParameters;
        var powerBombBreath = MakeBreathBombPower(DAMAGE, DIE_TYPE, SAVE, sprite, particle, validator, effect);

        sprite = Sprites.GetSprite("AlchemyBombPsychicPrecise", Resources.AlchemyBombPsychicPrecise, 128);
        particle = splash;
        var powerBombPrecise = MakePreciseBombPower(DAMAGE, DIE_TYPE, sprite, particle, validator, effect);

        AddBombFunctions(deviceDescription, powerBombPrecise, powerBombSplash, powerBombBreath);

        return toggle;
    }

    private static (FeatureDefinitionPower, IValidatePowerUse) MakeElementToggleMarker(string damage)
    {
        var marker = ConditionDefinitionBuilder
            .Create($"ConditionInnovationAlchemyMarker{damage}")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .AddCustomSubFeatures(ModifiedBombElement.Marker)
            .AddToDB();

        var power = FeatureDefinitionPowerBuilder
            .Create($"PowerInnovationAlchemyMarker{damage}")
            .SetGuiPresentation(Category.Feature)
            .SetShowCasting(false)
            .SetUniqueInstance()
            .SetUsesFixed(ActivationTime.NoCost)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Permanent)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(marker, ConditionForm.ConditionOperation.Add)
                            .Build())
                    .Build())
            .AddToDB();

        ForceGlobalUniqueEffects.AddToGroup(ForceGlobalUniqueEffects.Group.GrenadierGrenadeMode, power);

        return (power, new ValidatorsValidatePowerUse(ValidatorsCharacter.HasAnyOfConditions(marker.name)));
    }

    private static FeatureDefinitionPower MakeBombFireDamageToggle()
    {
        var power = FeatureDefinitionPowerBuilder
            .Create($"PowerInnovationAlchemyMarker{DamageTypeFire}")
            .SetGuiPresentation(Category.Feature)
            .SetShowCasting(false)
            .SetUniqueInstance()
            .SetUsesFixed(ActivationTime.NoCost)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Permanent)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(
                                ConditionDefinitionBuilder
                                    .Create($"ConditionInnovationAlchemyMarker{DamageTypeFire}")
                                    .SetGuiPresentationNoContent(true)
                                    .SetSilent(Silent.WhenAddedOrRemoved)
                                    .AddToDB(), ConditionForm.ConditionOperation.Add)
                            .Build())
                    .Build())
            .AddToDB();

        ForceGlobalUniqueEffects.AddToGroup(ForceGlobalUniqueEffects.Group.GrenadierGrenadeMode, power);

        return power;
    }

    private static void AddBombFunctions(UsableDeviceDescriptionBuilder device, FeatureDefinitionPower precise,
        FeatureDefinitionPower splash, FeatureDefinitionPower breath)
    {
        device.AddFunctions(
            new DeviceFunctionDescriptionBuilder()
                .SetUsage(useAmount: 2, useAffinity: DeviceFunctionDescription.FunctionUseAffinity.ChargeCost)
                .SetPower(precise, true)
                .Build(),
            new DeviceFunctionDescriptionBuilder()
                .SetUsage(useAmount: 2, useAffinity: DeviceFunctionDescription.FunctionUseAffinity.ChargeCost)
                .SetPower(splash, true)
                .Build(),
            new DeviceFunctionDescriptionBuilder()
                .SetUsage(useAmount: 2, useAffinity: DeviceFunctionDescription.FunctionUseAffinity.ChargeCost)
                .SetPower(breath, true)
                .Build()
        );
    }

    private static FeatureDefinitionPower MakePreciseBombPower(string damageType,
        DieType dieType,
        AssetReferenceSprite sprite,
        EffectParticleParameters particleParameters,
        IValidatePowerUse validator,
        params EffectForm[] effects)
    {
        const string NAME = "PowerInnovationAlchemyBombPrecise";

        var power = FeatureDefinitionPowerBuilder.Create($"{NAME}{damageType}")
            .SetGuiPresentation(NAME, Category.Feature, sprite)
            .SetUsesFixed(ActivationTime.Action)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetAnimationMagicEffect(AnimationDefinitions.AnimationMagicEffect.Animation1)
                    .SetTargetingData(Side.Enemy, RangeType.RangeHit, 12, TargetType.IndividualsUnique)
                    .SetEffectAdvancement(PerAdditionalSlotLevel, additionalTargetsPerIncrement: 1)
                    .SetParticleEffectParameters(particleParameters)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.None)
                            .SetDamageForm(damageType, 3, dieType)
                            .Build())
                    .AddEffectForms(effects)
                    .SetSpeed(SpeedType.CellsPerSeconds, 12)
                    .SetupImpactOffsets(offsetImpactTimePerTarget: 0.3f)
                    .Build())
            .SetUseSpellAttack()
            .AddToDB();

        power.AddCustomSubFeatures(
            ModifyPowerVisibility.Visible,
            new AddPBToDamage(power),
            new Overcharge(),
            validator,
            ModifyAdditionalDamageClassLevelInventor.Instance);

        return power;
    }

    private static FeatureDefinitionPower MakeBreathBombPower(string damageType,
        DieType dieType,
        string savingThrowAbility,
        AssetReferenceSprite sprite,
        EffectParticleParameters particleParameters,
        IValidatePowerUse validator,
        params EffectForm[] effects)
    {
        const string NAME = "PowerInnovationAlchemyBombBreath";

        var power = FeatureDefinitionPowerBuilder.Create($"{NAME}{damageType}")
            .SetGuiPresentation(NAME, Category.Feature, sprite)
            .SetUsesFixed(ActivationTime.Action)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.All, RangeType.Self, 0, TargetType.Cone, 4)
                    .ExcludeCaster()
                    .SetEffectAdvancement(PerAdditionalSlotLevel, additionalDicePerIncrement: 1)
                    .SetSavingThrowData(
                        false,
                        savingThrowAbility,
                        false,
                        EffectDifficultyClassComputation.SpellCastingFeature,
                        AttributeDefinitions.Intelligence)
                    .SetAnimationMagicEffect(AnimationDefinitions.AnimationMagicEffect.Animation0)
                    .SetParticleEffectParameters(particleParameters)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.HalfDamage)
                            .SetDamageForm(damageType, 2, dieType)
                            .Build())
                    .AddEffectForms(effects)
                    .Build())
            .AddToDB();

        power.AddCustomSubFeatures(
            ModifyPowerVisibility.Visible,
            new AddPBToDamage(power),
            new Overcharge(),
            validator,
            ModifyAdditionalDamageClassLevelInventor.Instance);

        return power;
    }

    private static FeatureDefinitionPower MakeSplashBombPower(string damageType,
        DieType dieType,
        string savingThrowAbility,
        AssetReferenceSprite sprite,
        EffectParticleParameters particleParameters,
        IValidatePowerUse validator,
        params EffectForm[] effects)
    {
        const string NAME = "PowerInnovationAlchemyBombSplash";

        var power = FeatureDefinitionPowerBuilder.Create($"{NAME}{damageType}")
            .SetGuiPresentation(NAME, Category.Feature, sprite)
            .SetUsesFixed(ActivationTime.Action)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetAnimationMagicEffect(AnimationDefinitions.AnimationMagicEffect.Animation1)
                    .SetTargetingData(Side.All, RangeType.Distance, 6, TargetType.Sphere)
                    .SetEffectAdvancement(PerAdditionalSlotLevel, additionalTargetCellsPerIncrement: 1)
                    .SetSavingThrowData(
                        false,
                        savingThrowAbility,
                        false,
                        EffectDifficultyClassComputation.SpellCastingFeature,
                        AttributeDefinitions.Intelligence)
                    .SetParticleEffectParameters(particleParameters)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.HalfDamage)
                            .SetDamageForm(damageType, 2, dieType)
                            .Build())
                    .AddEffectForms(effects)
                    .SetSpeed(SpeedType.CellsPerSeconds, 8)
                    .Build())
            .AddToDB();

        power.AddCustomSubFeatures(
            ModifyPowerVisibility.Visible,
            new AddPBToDamage(power),
            new Overcharge(),
            validator,
            ModifyAdditionalDamageClassLevelInventor.Instance);

        return power;
    }

    private static FeatureDefinitionPower BuildAlchemyPool()
    {
        var power = FeatureDefinitionPowerBuilder
            .Create("PowerInnovationAlchemyPool")
            .SetGuiPresentation(Category.Feature)
            .AddCustomSubFeatures(ModifyPowerVisibility.Hidden, HasModifiedUses.Marker)
            .SetUsesFixed(ActivationTime.Action, RechargeRate.ShortRest, 1, 3)
            .AddToDB();

        power.AddCustomSubFeatures(new ModifyPowerPoolAmount
        {
            PowerPool = power, Type = PowerPoolBonusCalculationType.ClassLevel, Attribute = InventorClass.ClassName
        });
        return power;
    }

    private static FeatureDefinitionPower BuildRefundPool(FeatureDefinitionPower powerPool)
    {
        var powerRefundPool = FeatureDefinitionPowerBuilder
            .Create("PowerInnovationAlchemyRefundPool")
            .SetGuiPresentation(Category.Feature, FeatureDefinitionPowers.PowerDomainInsightForeknowledge)
            .SetUsesProficiencyBonus(ActivationTime.BonusAction, RechargeRate.ShortRest)
            .AddToDB();

        var powerRefundFromSlotList = new List<FeatureDefinitionPower>();

        for (var i = 5; i >= 1; i--)
        {
            var title = Gui.Format("Feature/&PowerInnovationAlchemyRefundFromSlotTitle", i.ToString());
            var description = Gui.Format("Feature/&PowerInnovationAlchemyRefundFromSlotDescription", i.ToString());
            var powerRefundFromSlot = FeatureDefinitionPowerSharedPoolBuilder
                .Create($"PowerInnovationAlchemyRefundFromSlot{i}")
                .SetGuiPresentation(title, description)
                .SetSharedPool(ActivationTime.BonusAction, powerRefundPool)
                .AddCustomSubFeatures(new CustomBehaviorRefundAlchemyPool(powerPool, i))
                .AddToDB();

            powerRefundFromSlotList.Add(powerRefundFromSlot);
        }

        PowerBundle.RegisterPowerBundle(powerRefundPool, false, powerRefundFromSlotList);

        return powerRefundPool;
    }

    private static FeatureDefinition BuildOverchargeFeature()
    {
        return FeatureDefinitionBuilder
            .Create("FeatureInnovationAlchemyOverchargeBombs")
            .SetGuiPresentation(Category.Feature)
            .AddCustomSubFeatures(OverchargeFeature.Marker)
            .AddToDB();
    }

    private static FeatureDefinition BuildExtraOverchargeFeature()
    {
        return FeatureDefinitionBuilder
            .Create("FeatureInnovationAlchemyExtraOverchargeBombs")
            .SetGuiPresentation(Category.Feature)
            .AddCustomSubFeatures(OverchargeFeature.Marker)
            .AddToDB();
    }

    private static FeatureDefinition BuildMasterOverchargeFeature()
    {
        return FeatureDefinitionBuilder
            .Create("FeatureInnovationAlchemyMasterOverchargeBombs")
            .SetGuiPresentation(Category.Feature)
            .AddCustomSubFeatures(OverchargeFeature.Marker)
            .AddToDB();
    }

    private sealed class CustomBehaviorRefundAlchemyPool(FeatureDefinitionPower powerAlchemyPool, int slotLevel)
        : IValidatePowerUse, IMagicEffectFinishedByMe
    {
        public IEnumerator OnMagicEffectFinishedByMe(CharacterActionMagicEffect action, BaseDefinition baseDefinition)
        {
            var rulesetCharacter = action.ActingCharacter.RulesetCharacter;
            var rulesetRepertoire = rulesetCharacter.GetClassSpellRepertoire(InventorClass.Class);
            var rulesetUsablePower = PowerProvider.Get(powerAlchemyPool, rulesetCharacter);

            rulesetRepertoire!.SpendSpellSlot(slotLevel);
            rulesetUsablePower.remainingUses += slotLevel;

            yield break;
        }

        public bool CanUsePower(RulesetCharacter rulesetCharacter, FeatureDefinitionPower power)
        {
            var rulesetRepertoire = rulesetCharacter.GetClassSpellRepertoire(InventorClass.Class);
            var used =
                rulesetCharacter.GetMaxUsesForPool(powerAlchemyPool) -
                rulesetCharacter.GetRemainingPowerUses(powerAlchemyPool);

            rulesetRepertoire!.GetSlotsNumber(slotLevel, out var remaining, out _);

            return remaining > 0 && slotLevel <= used;
        }
    }

    private sealed class OverchargeFeature
    {
        private OverchargeFeature()
        {
        }

        public static OverchargeFeature Marker { get; } = new();
    }

    private sealed class ModifiedBombElement
    {
        private ModifiedBombElement()
        {
        }

        public static ModifiedBombElement Marker { get; } = new();
    }

    private sealed class Overcharge : ICustomOverchargeProvider
    {
        private static readonly (int, int)[] None = [];
        private static readonly (int, int)[] Once = [(1, 1)];
        private static readonly (int, int)[] Twice = [(1, 1), (2, 2)];
        private static readonly (int, int)[] Trice = [(1, 1), (2, 2), (3, 3)];

        public (int, int)[] OverchargeSteps(RulesetCharacter character)
        {
            var overcharges = character.GetSubFeaturesByType<OverchargeFeature>().Count;

            return overcharges switch
            {
                >= 3 => Trice,
                >= 2 => Twice,
                _ => overcharges >= 1 ? Once : None
            };
        }
    }
}

// ReSharper disable once SuggestBaseTypeForParameterInConstructor
internal sealed class AddPBToDamage(FeatureDefinitionPower baseDefinition) : IModifyEffectDescription
{
    public bool IsValid(
        BaseDefinition definition,
        RulesetCharacter character,
        EffectDescription effectDescription)
    {
        return definition == baseDefinition;
    }

    public EffectDescription GetEffectDescription(
        BaseDefinition definition,
        EffectDescription effectDescription,
        RulesetCharacter character,
        RulesetEffect rulesetEffect)
    {
        var damage = effectDescription.FindFirstDamageForm();

        if (damage != null)
        {
            damage.bonusDamage += character.TryGetAttributeValue(AttributeDefinitions.ProficiencyBonus);
        }

        return effectDescription;
    }
}
