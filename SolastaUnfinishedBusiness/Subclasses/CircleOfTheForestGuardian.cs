using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using static FeatureDefinitionAttributeModifier;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Builders.Features.AutoPreparedSpellsGroupBuilder;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterSubclassDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static SolastaUnfinishedBusiness.Subclasses.CommonBuilders;

namespace SolastaUnfinishedBusiness.Subclasses;

internal sealed class CircleOfTheForestGuardian : AbstractSubclass
{
    private const string CircleOfTheForestGuardianName = "CircleOfTheForestGuardian";

    internal CircleOfTheForestGuardian()
    {
        var autoPreparedSpellsForestGuardian = FeatureDefinitionAutoPreparedSpellsBuilder
            .Create("AutoPreparedSpellsForestGuardian")
            .SetGuiPresentation("DomainSpells", Category.Feature)
            .SetPreparedSpellGroups(
                BuildSpellGroup(2, Shield, FogCloud),
                BuildSpellGroup(3, Blur, FlameBlade),
                BuildSpellGroup(5, ProtectionFromEnergy, DispelMagic),
                BuildSpellGroup(7, FireShield, DeathWard),
                BuildSpellGroup(9, HoldMonster, GreaterRestoration))
            .SetSpellcastingClass(CharacterClassDefinitions.Druid)
            .AddToDB();

        var attributeModifierForestGuardianSylvanDurability = FeatureDefinitionAttributeModifierBuilder
            .Create("AttributeModifierForestGuardianSylvanDurability")
            .SetGuiPresentation(Category.Feature)
            .SetModifier(AttributeModifierOperation.Additive, AttributeDefinitions.HitPointBonusPerLevel, 1)
            .AddToDB();

        var effectFormTemporaryHitPoints = EffectFormBuilder
            .Create()
            .SetTempHpForm(4)
            .SetLevelAdvancement(EffectForm.LevelApplianceType.MultiplyBonus, LevelSourceType.ClassLevel)
            .CreatedByCharacter()
            .Build();

        var powerForestGuardianImprovedBarkWard = FeatureDefinitionPowerBuilder
            .Create("PowerForestGuardianImprovedBarkWard")
            .SetGuiPresentationNoContent()
            .SetUsesFixed(ActivationTime.NoCost)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetDamageForm(DamageTypePiercing, 2, DieType.D8)
                            .CreatedByCondition()
                            .Build())
                    .Build())
            .SetUniqueInstance()
            .AddToDB();

        var powerForestGuardianSuperiorBarkWard = FeatureDefinitionPowerBuilder
            .Create("PowerForestGuardianSuperiorBarkWard")
            .SetGuiPresentationNoContent()
            .SetUsesFixed(ActivationTime.NoCost)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetDamageForm(DamageTypePiercing, 3, DieType.D8)
                            .CreatedByCondition()
                            .Build())
                    .Build())
            .SetUniqueInstance()
            .AddToDB();

        var powerForestGuardianBarkWardPool = FeatureDefinitionPowerBuilder
            .Create("PowerForestGuardianBarkWardPool")
            .SetGuiPresentation(Category.Feature, hidden: true)
            .SetUsesProficiencyBonus(ActivationTime.BonusAction)
            .AddToDB();

        var powerSharedPoolForestGuardianBarkWard = FeatureDefinitionPowerSharedPoolBuilder
            .Create("PowerSharedPoolForestGuardianBarkWard")
            .SetGuiPresentation(Category.Feature, PowerDruidWildShape)
            .SetSharedPool(ActivationTime.BonusAction, powerForestGuardianBarkWardPool)
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
                                    .Create(ConditionDefinitions.ConditionBarkskin, "ConditionForestGuardianBarkWard")
                                    .SetOrUpdateGuiPresentation(Category.Condition)
                                    .ClearFeatures()
                                    .SetTurnOccurence(TurnOccurenceType.EndOfTurn)
                                    .AddToDB(),
                                ConditionForm.ConditionOperation.Add, true, true)
                            .Build())
                    .SetEffectAdvancement(EffectIncrementMethod.None)
                    .Build())
            .AddToDB();

        var powerSharedPoolForestGuardianImprovedBarkWard = FeatureDefinitionPowerSharedPoolBuilder
            .Create("PowerSharedPoolForestGuardianImprovedBarkWard")
            .SetGuiPresentation(Category.Feature, PowerDruidWildShape)
            .SetSharedPool(ActivationTime.BonusAction, powerForestGuardianBarkWardPool)
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
                                    .Create(ConditionDefinitions.ConditionBarkskin,
                                        "ConditionForestGuardianImprovedBarkWard")
                                    .SetOrUpdateGuiPresentation(Category.Condition)
                                    .SetTurnOccurence(TurnOccurenceType.EndOfTurn)
                                    .SetFeatures(
                                        FeatureDefinitionDamageAffinityBuilder
                                            .Create("DamageAffinityForestGuardianImprovedBarkWard")
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

        var powerSharedPoolForestGuardianSuperiorBarkWard = FeatureDefinitionPowerSharedPoolBuilder
            .Create("PowerSharedPoolForestGuardianSuperiorBarkWard")
            .SetGuiPresentation(Category.Feature, PowerDruidWildShape)
            .SetSharedPool(ActivationTime.BonusAction, powerForestGuardianBarkWardPool)
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
                                    .Create(ConditionDefinitions.ConditionBarkskin,
                                        "ConditionForestGuardianSuperiorBarkWard")
                                    .SetOrUpdateGuiPresentation(Category.Condition)
                                    .SetFeatures(
                                        FeatureDefinitionDamageAffinityBuilder
                                            .Create("DamageAffinityForestGuardianSuperiorBarkWard")
                                            .SetGuiPresentationNoContent()
                                            .SetDamageAffinityType(DamageAffinityType.Immunity)
                                            .SetDamageType(DamageTypePoison)
                                            .SetRetaliate(powerForestGuardianSuperiorBarkWard, 1, true)
                                            .AddToDB())
                                    .SetTurnOccurence(TurnOccurenceType.EndOfTurn)
                                    .AddToDB(),
                                ConditionForm.ConditionOperation.Add,
                                true,
                                true)
                            .Build())
                    .SetEffectAdvancement(EffectIncrementMethod.None)
                    .Build())
            .SetOverriddenPower(powerSharedPoolForestGuardianImprovedBarkWard)
            .AddToDB();

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(CircleOfTheForestGuardianName)
            .SetGuiPresentation(Category.Subclass, MartialMountaineer)
            .AddFeaturesAtLevel(2,
                autoPreparedSpellsForestGuardian,
                attributeModifierForestGuardianSylvanDurability,
                PowerCasterFightingWarMagic,
                powerSharedPoolForestGuardianBarkWard)
            .AddFeaturesAtLevel(6,
                AttributeModifierCasterFightingExtraAttack)
            .AddFeaturesAtLevel(10,
                powerSharedPoolForestGuardianImprovedBarkWard)
            .AddFeaturesAtLevel(14,
                powerSharedPoolForestGuardianSuperiorBarkWard)
            .AddToDB();
    }

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceDruidCircle;
}
