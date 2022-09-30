using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomDefinitions;
using static FeatureDefinitionAttributeModifier.AttributeModifierOperation;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Builders.Features.AutoPreparedSpellsGroupBuilder;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterSubclassDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;

namespace SolastaUnfinishedBusiness.Subclasses;

internal sealed class CircleOfTheForestGuardian : AbstractSubclass
{
    private const string ForestGuardianName = "CircleOfTheForestGuardian";

    internal CircleOfTheForestGuardian()
    {
        // Create Auto-prepared Spell list
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
            .SetModifier(Additive, AttributeDefinitions.AttacksNumber, 1)
            .AddToDB();

        var attributeModifierForestGuardianSylvanDurability = FeatureDefinitionAttributeModifierBuilder
            .Create("AttributeModifierForestGuardianSylvanDurability")
            .SetGuiPresentation(Category.Feature)
            .SetModifier(Additive, AttributeDefinitions.HitPointBonusPerLevel, 1)
            .AddToDB();

        // Create Sylvan War Magic
        var magicAffinityForestGuardianSylvanWarMagic = FeatureDefinitionMagicAffinityBuilder
            .Create(FeatureDefinitionMagicAffinitys.MagicAffinityBattleMagic,
                "MagicAffinityForestGuardianSylvanWarMagic")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        var (barkWard, improvedBarkWard, superiorBarkWard) = CreateBarkWard();

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(ForestGuardianName)
            .SetGuiPresentation(Category.Subclass, MartialMountaineer.GuiPresentation.SpriteReference)
            .AddFeaturesAtLevel(2,
                autoPreparedSpellsForestGuardian,
                attributeModifierForestGuardianSylvanDurability,
                magicAffinityForestGuardianSylvanWarMagic,
                barkWard)
            .AddFeaturesAtLevel(6,
                attributeModifierForestGuardianExtraAttack)
            .AddFeaturesAtLevel(10,
                improvedBarkWard)
            .AddFeaturesAtLevel(14,
                superiorBarkWard)
            .AddToDB();
    }

    internal override CharacterSubclassDefinition Subclass { get; set; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceDruidCircle;

    // Create Bark Ward Wild Shape Power (and the two higher variants, improved and superior)
    private static (
        FeatureDefinitionPowerSharedPool barkWard,
        FeatureDefinitionPowerSharedPool improvedBarkWard,
        FeatureDefinitionPowerSharedPool superiorBarkWard) CreateBarkWard()
    {
        var tempHpEffect = EffectFormBuilder
            .Create()
            .SetTempHPForm(4)
            .SetLevelAdvancement(EffectForm.LevelApplianceType.MultiplyBonus, LevelSourceType.ClassLevel)
            .CreatedByCharacter()
            .Build();

        var barkWardBuff = EffectFormBuilder
            .Create()
            .SetConditionForm(CreateConditionForestGuardianBarkWard(), ConditionForm.ConditionOperation.Add, true, true)
            .Build();

        var improvedBarkWardBuff = EffectFormBuilder
            .Create()
            .SetConditionForm(CreateConditionConditionForestGuardianImprovedBarkWard(),
                ConditionForm.ConditionOperation.Add, true,
                true)
            .Build();

        var superiorBarkWardBuff = EffectFormBuilder
            .Create()
            .SetConditionForm(CreateConditionForestGuardianSuperiorBarkWard(), ConditionForm.ConditionOperation.Add,
                true,
                true)
            .Build();

        var barkWardEffectDescription = EffectDescriptionBuilder
            .Create()
            .SetTargetingData(Side.Ally, RangeType.Self, 1, TargetType.Self)
            .SetCreatedByCharacter()
            .SetDurationData(DurationType.Minute, 10, TurnOccurenceType.EndOfTurn)
            .AddEffectForm(tempHpEffect)
            .AddEffectForm(barkWardBuff)
            .SetEffectAdvancement(EffectIncrementMethod.None)
            .Build();

        var improvedBarkWardEffectDescription = EffectDescriptionBuilder
            .Create()
            .SetTargetingData(Side.Ally, RangeType.Self, 1, TargetType.Self)
            .SetCreatedByCharacter()
            .SetDurationData(DurationType.Minute, 10, TurnOccurenceType.EndOfTurn)
            .AddEffectForm(tempHpEffect)
            .AddEffectForm(improvedBarkWardBuff)
            .SetEffectAdvancement(EffectIncrementMethod.None)
            .Build();

        var superiorBarkWardEffectDescription = EffectDescriptionBuilder
            .Create()
            .SetTargetingData(Side.Ally, RangeType.Self, 1, TargetType.Self)
            .SetCreatedByCharacter()
            .SetDurationData(DurationType.Minute, 10, TurnOccurenceType.EndOfTurn)
            .AddEffectForm(tempHpEffect)
            .AddEffectForm(superiorBarkWardBuff)
            .SetEffectAdvancement(EffectIncrementMethod.None)
            .Build();

        var powerSharedPoolForestGuardianBarkWard = FeatureDefinitionPowerSharedPoolBuilder
            .Create("PowerSharedPoolForestGuardianBarkWard")
            .SetGuiPresentation(Category.Feature, PowerDruidWildShape.GuiPresentation.SpriteReference)
            .Configure(
                PowerDruidWildShape, RechargeRate.ShortRest, ActivationTime.BonusAction, 1, false, false,
                AttributeDefinitions.Wisdom, barkWardEffectDescription, true)
            .AddToDB();

        var powerSharedPoolForestGuardianImprovedBarkWard = FeatureDefinitionPowerSharedPoolBuilder
            .Create("PowerSharedPoolForestGuardianImprovedBarkWard")
            .SetGuiPresentation(Category.Feature, PowerDruidWildShape.GuiPresentation.SpriteReference)
            .Configure(
                PowerDruidWildShape, RechargeRate.ShortRest, ActivationTime.BonusAction, 1, false, false,
                AttributeDefinitions.Wisdom, improvedBarkWardEffectDescription, true)
            .SetOverriddenPower(powerSharedPoolForestGuardianBarkWard)
            .AddToDB();

        var powerSharedPoolForestGuardianSuperiorBarkWard = FeatureDefinitionPowerSharedPoolBuilder
            .Create("PowerSharedPoolForestGuardianSuperiorBarkWard")
            .SetGuiPresentation(Category.Feature, PowerDruidWildShape.GuiPresentation.SpriteReference)
            .Configure(
                PowerDruidWildShape, RechargeRate.ShortRest, ActivationTime.BonusAction, 1, false, false,
                AttributeDefinitions.Wisdom, superiorBarkWardEffectDescription, true)
            .SetOverriddenPower(powerSharedPoolForestGuardianImprovedBarkWard)
            .AddToDB();

        return (
            powerSharedPoolForestGuardianBarkWard,
            powerSharedPoolForestGuardianImprovedBarkWard,
            powerSharedPoolForestGuardianSuperiorBarkWard);

        static ConditionDefinition CreateConditionForestGuardianBarkWard()
        {
            return ConditionDefinitionBuilder
                .Create(ConditionDefinitions.ConditionBarkskin, "ConditionForestGuardianBarkWard")
                .SetOrUpdateGuiPresentation(Category.Condition)
                .ClearFeatures()
                .SetAllowMultipleInstances(false)
                .SetDuration(DurationType.Minute, 10)
                .SetTurnOccurence(TurnOccurenceType.EndOfTurn)
                .AddToDB();
        }

        static ConditionDefinition CreateConditionConditionForestGuardianImprovedBarkWard()
        {
            var damageEffect = EffectFormBuilder
                .Create()
                .SetDamageForm(false, DieType.D8, DamageTypePiercing, 0, DieType.D8, 2)
                .CreatedByCondition()
                .Build();

            var improvedBarkWardRetaliationEffect = EffectDescriptionBuilder
                .Create()
                .AddEffectForm(damageEffect)
                .Build();

            var powerForestGuardianImprovedBarkWard = FeatureDefinitionPowerBuilder
                .Create("PowerForestGuardianImprovedBarkWard")
                .SetGuiPresentationNoContent()
                .Configure(
                    0,
                    UsesDetermination.Fixed,
                    AttributeDefinitions.Wisdom,
                    ActivationTime.NoCost,
                    0,
                    RechargeRate.AtWill,
                    false,
                    false,
                    AttributeDefinitions.Wisdom,
                    improvedBarkWardRetaliationEffect,
                    true)
                .AddToDB();

            var damageAffinityForestGuardianImprovedBarkWard = FeatureDefinitionDamageAffinityBuilder
                .Create("DamageAffinityForestGuardianImprovedBarkWard")
                .SetGuiPresentationNoContent()
                .SetDamageAffinityType(DamageAffinityType.None)
                .SetDamageType(DamageTypePoison)
                .SetRetaliate(powerForestGuardianImprovedBarkWard, 1, true)
                .SetAncestryDefinesDamageType(false)
                .AddToDB();

            return ConditionDefinitionBuilder
                .Create(ConditionDefinitions.ConditionBarkskin, "ConditionForestGuardianImprovedBarkWard")
                .SetOrUpdateGuiPresentation(Category.Condition)
                .SetAllowMultipleInstances(false)
                .SetDuration(DurationType.Minute, 10)
                .SetTurnOccurence(TurnOccurenceType.EndOfTurn)
                .SetFeatures(damageAffinityForestGuardianImprovedBarkWard)
                .AddToDB();
        }

        static ConditionDefinition CreateConditionForestGuardianSuperiorBarkWard()
        {
            var damageEffect = EffectFormBuilder
                .Create()
                .SetDamageForm(false, DieType.D8, DamageTypePiercing, 0, DieType.D8, 3)
                .CreatedByCondition()
                .Build();

            var superiorBarkWardRetaliationEffect = EffectDescriptionBuilder
                .Create()
                .AddEffectForm(damageEffect)
                .Build();

            var powerForestGuardianSuperiorBarkWard = FeatureDefinitionPowerBuilder
                .Create("PowerForestGuardianSuperiorBarkWard")
                .SetGuiPresentationNoContent()
                .Configure(
                    0, UsesDetermination.Fixed, AttributeDefinitions.Wisdom, ActivationTime.NoCost,
                    0, RechargeRate.AtWill, false, false,
                    AttributeDefinitions.Wisdom, superiorBarkWardRetaliationEffect, true)
                .AddToDB();

            var damageAffinityForestGuardianSuperiorBarkWard = FeatureDefinitionDamageAffinityBuilder
                .Create("DamageAffinityForestGuardianSuperiorBarkWard")
                .SetGuiPresentationNoContent()
                .SetDamageAffinityType(DamageAffinityType.Immunity)
                .SetDamageType(DamageTypePoison)
                .SetRetaliate(powerForestGuardianSuperiorBarkWard, 1, true)
                .SetAncestryDefinesDamageType(false)
                .AddToDB();

            return ConditionDefinitionBuilder
                .Create(ConditionDefinitions.ConditionBarkskin, "ConditionForestGuardianSuperiorBarkWard")
                .SetOrUpdateGuiPresentation(Category.Condition)
                .SetFeatures(damageAffinityForestGuardianSuperiorBarkWard)
                .SetAllowMultipleInstances(false)
                .SetDuration(DurationType.Minute, 10)
                .SetTurnOccurence(TurnOccurenceType.EndOfTurn)
                .AddToDB();
        }
    }
}
