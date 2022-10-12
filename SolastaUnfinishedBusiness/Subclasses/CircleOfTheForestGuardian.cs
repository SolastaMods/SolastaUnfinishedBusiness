using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using static FeatureDefinitionAttributeModifier;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Builders.Features.AutoPreparedSpellsGroupBuilder;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterSubclassDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;

namespace SolastaUnfinishedBusiness.Subclasses;

internal sealed class CircleOfTheForestGuardian : AbstractSubclass
{
    private const string CircleOfTheForestGuardianName = "CircleOfTheForestGuardian";

    internal CircleOfTheForestGuardian()
    {
        var autoPreparedSpellsForestGuardian = FeatureDefinitionAutoPreparedSpellsBuilder
            .Create("AutoPreparedSpellsForestGuardian")
            .SetGuiPresentation(Category.Feature)
            .SetPreparedSpellGroups(
                BuildSpellGroup(2, Shield, FogCloud),
                BuildSpellGroup(3, Blur, FlameBlade),
                BuildSpellGroup(5, ProtectionFromEnergy, DispelMagic),
                BuildSpellGroup(7, FireShield, DeathWard),
                BuildSpellGroup(9, HoldMonster, GreaterRestoration))
            .SetSpellcastingClass(CharacterClassDefinitions.Druid)
            .AddToDB();

        var attributeModifierForestGuardianExtraAttack = FeatureDefinitionAttributeModifierBuilder
            .Create("AttributeModifierForestGuardianExtraAttack")
            .SetGuiPresentation(Category.Feature)
            .SetModifier(AttributeModifierOperation.ForceIfBetter, AttributeDefinitions.AttacksNumber, 2)
            .AddToDB();

        var attributeModifierForestGuardianSylvanDurability = FeatureDefinitionAttributeModifierBuilder
            .Create("AttributeModifierForestGuardianSylvanDurability")
            .SetGuiPresentation(Category.Feature)
            .SetModifier(AttributeModifierOperation.Additive, AttributeDefinitions.HitPointBonusPerLevel, 1)
            .AddToDB();

        var magicAffinityForestGuardianSylvanWarMagic = FeatureDefinitionMagicAffinityBuilder
            .Create(FeatureDefinitionMagicAffinitys.MagicAffinityBattleMagic,
                "MagicAffinityForestGuardianSylvanWarMagic")
            .SetGuiPresentation(Category.Feature)
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
            .SetUsesFixed(
                ActivationTime.NoCost,
                RechargeRate.AtWill,
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
            .SetUsesFixed(
                ActivationTime.NoCost,
                RechargeRate.AtWill,
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

        var powerSharedPoolForestGuardianBarkWard = FeatureDefinitionPowerSharedPoolBuilder
            .Create("PowerSharedPoolForestGuardianBarkWard")
            .SetGuiPresentation(Category.Feature, PowerDruidWildShape.GuiPresentation.SpriteReference)
            .Configure(
                PowerDruidWildShape,
                RechargeRate.ShortRest,
                ActivationTime.BonusAction,
                1,
                false,
                false,
                AttributeDefinitions.Wisdom,
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Ally, RangeType.Self, 1, TargetType.Self)
                    .SetCreatedByCharacter()
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
                                    .SetAllowMultipleInstances(false)
                                    .SetDuration(DurationType.Minute, 10)
                                    .SetTurnOccurence(TurnOccurenceType.EndOfTurn)
                                    .AddToDB(),
                                ConditionForm.ConditionOperation.Add, true, true)
                            .Build())
                    .SetEffectAdvancement(EffectIncrementMethod.None)
                    .Build(),
                true)
            .AddToDB();

        var powerSharedPoolForestGuardianImprovedBarkWard = FeatureDefinitionPowerSharedPoolBuilder
            .Create("PowerSharedPoolForestGuardianImprovedBarkWard")
            .SetGuiPresentation(Category.Feature, PowerDruidWildShape.GuiPresentation.SpriteReference)
            .Configure(
                PowerDruidWildShape,
                RechargeRate.ShortRest,
                ActivationTime.BonusAction,
                1,
                false,
                false,
                AttributeDefinitions.Wisdom,
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Ally, RangeType.Self, 1, TargetType.Self)
                    .SetCreatedByCharacter()
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
                                    .SetAllowMultipleInstances(false)
                                    .SetDuration(DurationType.Minute, 10)
                                    .SetTurnOccurence(TurnOccurenceType.EndOfTurn)
                                    .SetFeatures(
                                        FeatureDefinitionDamageAffinityBuilder
                                            .Create("DamageAffinityForestGuardianImprovedBarkWard")
                                            .SetGuiPresentationNoContent()
                                            .SetDamageAffinityType(DamageAffinityType.None)
                                            .SetDamageType(DamageTypePoison)
                                            .SetRetaliate(powerForestGuardianImprovedBarkWard, 1, true)
                                            .SetAncestryDefinesDamageType(false)
                                            .AddToDB())
                                    .AddToDB(),
                                ConditionForm.ConditionOperation.Add,
                                true,
                                true)
                            .Build())
                    .SetEffectAdvancement(EffectIncrementMethod.None)
                    .Build(),
                true)
            .SetOverriddenPower(powerSharedPoolForestGuardianBarkWard)
            .AddToDB();

        var powerSharedPoolForestGuardianSuperiorBarkWard = FeatureDefinitionPowerSharedPoolBuilder
            .Create("PowerSharedPoolForestGuardianSuperiorBarkWard")
            .SetGuiPresentation(Category.Feature, PowerDruidWildShape.GuiPresentation.SpriteReference)
            .Configure(
                PowerDruidWildShape,
                RechargeRate.ShortRest,
                ActivationTime.BonusAction,
                1,
                false,
                false,
                AttributeDefinitions.Wisdom,
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Ally, RangeType.Self, 1, TargetType.Self)
                    .SetCreatedByCharacter()
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
                                            .SetAncestryDefinesDamageType(false)
                                            .AddToDB())
                                    .SetAllowMultipleInstances(false)
                                    .SetDuration(DurationType.Minute, 10)
                                    .SetTurnOccurence(TurnOccurenceType.EndOfTurn)
                                    .AddToDB(),
                                ConditionForm.ConditionOperation.Add,
                                true,
                                true)
                            .Build())
                    .SetEffectAdvancement(EffectIncrementMethod.None)
                    .Build(),
                true)
            .SetOverriddenPower(powerSharedPoolForestGuardianImprovedBarkWard)
            .AddToDB();

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(CircleOfTheForestGuardianName)
            .SetGuiPresentation(Category.Subclass, MartialMountaineer.GuiPresentation.SpriteReference)
            .AddFeaturesAtLevel(2,
                autoPreparedSpellsForestGuardian,
                attributeModifierForestGuardianSylvanDurability,
                magicAffinityForestGuardianSylvanWarMagic,
                powerSharedPoolForestGuardianBarkWard)
            .AddFeaturesAtLevel(6,
                attributeModifierForestGuardianExtraAttack)
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
