using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Properties;
using static RuleDefinitions;
using static FeatureDefinitionAttributeModifier;
using static SolastaUnfinishedBusiness.Builders.Features.AutoPreparedSpellsGroupBuilder;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static SolastaUnfinishedBusiness.Subclasses.CommonBuilders;

namespace SolastaUnfinishedBusiness.Subclasses;

[UsedImplicitly]
public sealed class CircleOfTheForestGuardian : AbstractSubclass
{
    private const string Name = "ForestGuardian";

    public CircleOfTheForestGuardian()
    {
        var autoPreparedSpellsForestGuardian = FeatureDefinitionAutoPreparedSpellsBuilder
            .Create($"AutoPreparedSpells{Name}")
            .SetGuiPresentation("ExpandedSpells", Category.Feature)
            .SetAutoTag("Circle")
            .SetPreparedSpellGroups(
                BuildSpellGroup(2, Shield, FogCloud),
                BuildSpellGroup(3, Blur, FlameBlade),
                BuildSpellGroup(5, ProtectionFromEnergy, DispelMagic),
                BuildSpellGroup(7, FireShield, DeathWard),
                BuildSpellGroup(9, HoldMonster, GreaterRestoration))
            .SetSpellcastingClass(CharacterClassDefinitions.Druid)
            .AddToDB();

        var attributeModifierForestGuardianSylvanDurability = FeatureDefinitionAttributeModifierBuilder
            .Create($"AttributeModifier{Name}SylvanDurability")
            .SetGuiPresentation(Category.Feature)
            .SetModifier(AttributeModifierOperation.Additive, AttributeDefinitions.HitPointBonusPerLevel, 1)
            .AddToDB();

        var effectFormTemporaryHitPoints = EffectFormBuilder
            .Create()
            .SetTempHpForm(4)
            .SetLevelAdvancement(EffectForm.LevelApplianceType.MultiplyBonus, LevelSourceType.ClassLevel)
            .Build();

        var powerForestGuardianImprovedBarkWard = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}ImprovedBarkWard")
            .SetGuiPresentationNoContent()
            .SetUsesFixed(ActivationTime.NoCost)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetDamageForm(DamageTypePiercing, 2, DieType.D8)
                            .Build())
                    .Build())
            .SetUniqueInstance()
            .AddToDB();

        powerForestGuardianImprovedBarkWard.EffectDescription.EffectParticleParameters.impactParticleReference =
            PowerPatronTreeExplosiveGrowth.EffectDescription.EffectParticleParameters.impactParticleReference;

        var powerForestGuardianSuperiorBarkWard = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}SuperiorBarkWard")
            .SetGuiPresentationNoContent()
            .SetUsesFixed(ActivationTime.NoCost)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetDamageForm(DamageTypePiercing, 3, DieType.D8)
                            .Build())
                    .Build())
            .SetUniqueInstance()
            .AddToDB();

        powerForestGuardianSuperiorBarkWard.EffectDescription.EffectParticleParameters.impactParticleReference =
            PowerPatronTreeExplosiveGrowth.EffectDescription.EffectParticleParameters.impactParticleReference;

        var powerSharedPoolForestGuardianBarkWard = FeatureDefinitionPowerBuilder
            .Create($"PowerSharedPool{Name}BarkWard")
            .SetGuiPresentation(Category.Feature, PowerDruidWildShape)
            .SetUsesProficiencyBonus(ActivationTime.BonusAction)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetDurationData(DurationType.Minute, 10)
                    .SetEffectForms(effectFormTemporaryHitPoints)
                    .SetEffectAdvancement(EffectIncrementMethod.None)
                    .Build())
            .AddToDB();

        powerSharedPoolForestGuardianBarkWard.EffectDescription.EffectParticleParameters.casterParticleReference =
            SpikeGrowth.EffectDescription.EffectParticleParameters.casterParticleReference;

        var powerSharedPoolForestGuardianImprovedBarkWard = FeatureDefinitionPowerBuilder
            .Create($"PowerSharedPool{Name}ImprovedBarkWard")
            .SetGuiPresentation(Category.Feature, PowerDruidWildShape)
            .SetUsesProficiencyBonus(ActivationTime.BonusAction)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetDurationData(DurationType.Minute, 10)
                    .SetEffectForms(
                        effectFormTemporaryHitPoints,
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(
                                ConditionDefinitionBuilder
                                    .Create(ConditionDefinitions.ConditionBarkskin, $"Condition{Name}ImprovedBarkWard")
                                    .SetOrUpdateGuiPresentation(Category.Condition)
                                    .SetFeatures(
                                        FeatureDefinitionDamageAffinityBuilder
                                            .Create($"DamageAffinity{Name}ImprovedBarkWard")
                                            .SetGuiPresentationNoContent()
                                            .SetDamageAffinityType(DamageAffinityType.None)
                                            .SetDamageType(DamageTypePoison)
                                            .SetRetaliate(powerForestGuardianImprovedBarkWard, 1, true)
                                            .AddToDB())
                                    .AddToDB(),
                                ConditionForm.ConditionOperation.Add,
                                true,
                                true)
                            .Build())
                    .SetEffectAdvancement(EffectIncrementMethod.None)
                    .Build())
            .SetOverriddenPower(powerSharedPoolForestGuardianBarkWard)
            .AddToDB();

        powerSharedPoolForestGuardianImprovedBarkWard.EffectDescription.EffectParticleParameters
                .casterParticleReference =
            SpikeGrowth.EffectDescription.EffectParticleParameters.casterParticleReference;

        var powerSharedPoolForestGuardianSuperiorBarkWard = FeatureDefinitionPowerBuilder
            .Create($"PowerSharedPool{Name}SuperiorBarkWard")
            .SetGuiPresentation(Category.Feature, PowerDruidWildShape)
            .SetUsesProficiencyBonus(ActivationTime.BonusAction)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetDurationData(DurationType.Minute, 10)
                    .SetEffectForms(
                        effectFormTemporaryHitPoints,
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(
                                ConditionDefinitionBuilder
                                    .Create(ConditionDefinitions.ConditionBarkskin, $"Condition{Name}SuperiorBarkWard")
                                    .SetOrUpdateGuiPresentation(Category.Condition)
                                    .SetFeatures(
                                        FeatureDefinitionDamageAffinityBuilder
                                            .Create($"DamageAffinity{Name}SuperiorBarkWard")
                                            .SetGuiPresentationNoContent()
                                            .SetDamageAffinityType(DamageAffinityType.Immunity)
                                            .SetDamageType(DamageTypePoison)
                                            .SetRetaliate(powerForestGuardianSuperiorBarkWard, 1, true)
                                            .AddToDB())
                                    .AddToDB(),
                                ConditionForm.ConditionOperation.Add,
                                true,
                                true)
                            .Build())
                    .SetEffectAdvancement(EffectIncrementMethod.None)
                    .Build())
            .SetOverriddenPower(powerSharedPoolForestGuardianImprovedBarkWard)
            .AddToDB();

        powerSharedPoolForestGuardianSuperiorBarkWard.EffectDescription.EffectParticleParameters
                .casterParticleReference =
            SpikeGrowth.EffectDescription.EffectParticleParameters.casterParticleReference;

        Subclass = CharacterSubclassDefinitionBuilder
            .Create($"CircleOfThe{Name}")
            .SetGuiPresentation(Category.Subclass,
                Sprites.GetSprite(Name, Resources.CircleOfTheForestGuardian, 256))
            .AddFeaturesAtLevel(2,
                autoPreparedSpellsForestGuardian,
                attributeModifierForestGuardianSylvanDurability,
                powerSharedPoolForestGuardianBarkWard)
            .AddFeaturesAtLevel(6,
                AttributeModifierCasterFightingExtraAttack,
                AttackReplaceWithCantripCasterFighting)
            .AddFeaturesAtLevel(10,
                powerSharedPoolForestGuardianImprovedBarkWard)
            .AddFeaturesAtLevel(14,
                powerSharedPoolForestGuardianSuperiorBarkWard)
            .AddToDB();
    }


    internal override CharacterClassDefinition Klass => CharacterClassDefinitions.Druid;

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceDruidCircle;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }
}
