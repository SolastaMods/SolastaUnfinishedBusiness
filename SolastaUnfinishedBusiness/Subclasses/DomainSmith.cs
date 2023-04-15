using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomValidators;
using SolastaUnfinishedBusiness.Models;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static SolastaUnfinishedBusiness.Builders.Features.AutoPreparedSpellsGroupBuilder;

namespace SolastaUnfinishedBusiness.Subclasses;

internal sealed class DomainSmith : AbstractSubclass
{
    internal DomainSmith()
    {
        const string NAME = "DomainSmith";

        //
        // 1 (6, 11, 16)
        //

        var autoPreparedSpellsDomainSmith = FeatureDefinitionAutoPreparedSpellsBuilder
            .Create($"AutoPreparedSpells{NAME}")
            .SetGuiPresentation("ExpandedSpells", Category.Feature)
            .SetAutoTag("Domain")
            .SetPreparedSpellGroups(
                BuildSpellGroup(1, Identify, SpellsContext.SearingSmite),
                BuildSpellGroup(3, HeatMetal, MagicWeapon),
                BuildSpellGroup(5, ProtectionFromEnergy, SpellsContext.ElementalWeapon),
                BuildSpellGroup(7, WallOfFire, Stoneskin),
                BuildSpellGroup(9, FlameStrike, HoldMonster))
            .SetSpellcastingClass(CharacterClassDefinitions.Cleric)
            .AddToDB();

        var bonusProficiencyDomainForge = FeatureDefinitionProficiencyBuilder
            .Create(FeatureDefinitionProficiencys.ProficiencySmithTools, $"BonusProficiency{NAME}")
            .SetGuiPresentation(Category.Feature)
            .SetProficiencies(ProficiencyType.Armor, EquipmentDefinitions.HeavyArmorCategory)
            .AddToDB();

        const string REINFORCE_ARMOR_DESCRIPTION = "Feature/&PowerDomainSmithReinforceArmorDescription";

        const string REINFORCE_ARMOR_ATTRIBUTE_DESCRIPTION =
            "Feature/&AttributeModifierDomainSmithReinforceArmorDescription";

        static string PowerReinforceDescription(int x)
        {
            return Gui.Format(REINFORCE_ARMOR_DESCRIPTION, x.ToString());
        }

        static string AttributeReinforceDescription(int x)
        {
            return Gui.Format(REINFORCE_ARMOR_ATTRIBUTE_DESCRIPTION, x.ToString());
        }

        var powerReinforceArmor1 = FeatureDefinitionPowerBuilder
            .Create($"Power{NAME}ReinforceArmor1")
            .SetGuiPresentation($"Power{NAME}ReinforceArmor", Category.Feature, PowerReinforceDescription(1),
                MageArmor.guiPresentation.SpriteReference)
            .SetUniqueInstance()
            .SetCustomSubFeatures(
                DoNotTerminateWhileUnconscious.Marker,
                ExtraCarefulTrackedItem.Marker,
                SkipEffectRemovalOnLocationChange.Always,
                new CustomItemFilter(CanArmorBeReinforced))
            .SetUsesFixed(ActivationTime.Action, RechargeRate.ShortRest)
            .SetEffectDescription(EffectDescriptionBuilder
                .Create()
                .SetTargetingData(Side.All, RangeType.Self, 0, TargetType.Item,
                    itemSelectionType: ActionDefinitions.ItemSelectionType.Carried)
                .SetDurationData(DurationType.Permanent)
                .SetEffectForms(EffectFormBuilder
                    .Create()
                    .SetItemPropertyForm(
                        ItemPropertyUsage.Unlimited,
                        1, new FeatureUnlockByLevel(
                            FeatureDefinitionAttributeModifierBuilder
                                .Create($"AttributeModifier{NAME}ReinforceArmor1")
                                .SetGuiPresentation($"AttributeModifier{NAME}ReinforceArmor", Category.Feature,
                                    AttributeReinforceDescription(1), MageArmor.guiPresentation.SpriteReference)
                                .SetCustomSubFeatures(ExtraCarefulTrackedItem.Marker)
                                .SetModifier(FeatureDefinitionAttributeModifier.AttributeModifierOperation.Additive,
                                    AttributeDefinitions.ArmorClass, 1)
                                .AddToDB()
                            , 0)
                    )
                    .Build())
                .Build())
            .AddToDB();

        var powerReinforceArmor6 = FeatureDefinitionPowerBuilder
            .Create($"Power{NAME}ReinforceArmor6")
            .SetGuiPresentation($"Power{NAME}ReinforceArmor", Category.Feature, PowerReinforceDescription(2),
                MageArmor.guiPresentation.SpriteReference)
            .SetUniqueInstance()
            .SetCustomSubFeatures(
                DoNotTerminateWhileUnconscious.Marker,
                ExtraCarefulTrackedItem.Marker,
                SkipEffectRemovalOnLocationChange.Always,
                new CustomItemFilter(CanArmorBeReinforced))
            .SetUsesFixed(ActivationTime.Action, RechargeRate.ShortRest)
            .SetOverriddenPower(powerReinforceArmor1)
            .SetEffectDescription(EffectDescriptionBuilder
                .Create()
                .SetTargetingData(Side.All, RangeType.Self, 0, TargetType.Item,
                    itemSelectionType: ActionDefinitions.ItemSelectionType.Carried)
                .SetDurationData(DurationType.Permanent)
                .SetEffectForms(EffectFormBuilder
                    .Create()
                    .SetItemPropertyForm(
                        ItemPropertyUsage.Unlimited,
                        1, new FeatureUnlockByLevel(
                            FeatureDefinitionAttributeModifierBuilder
                                .Create($"AttributeModifier{NAME}ReinforceArmor2")
                                .SetGuiPresentation($"AttributeModifier{NAME}ReinforceArmor", Category.Feature,
                                    AttributeReinforceDescription(2), MageArmor.guiPresentation.SpriteReference)
                                .SetCustomSubFeatures(ExtraCarefulTrackedItem.Marker)
                                .SetModifier(FeatureDefinitionAttributeModifier.AttributeModifierOperation.Additive,
                                    AttributeDefinitions.ArmorClass, 2)
                                .AddToDB()
                            , 0)
                    )
                    .Build())
                .Build())
            .AddToDB();

        var powerReinforceArmor11 = FeatureDefinitionPowerBuilder
            .Create($"Power{NAME}ReinforceArmor11")
            .SetGuiPresentation($"Power{NAME}ReinforceArmor", Category.Feature, PowerReinforceDescription(3),
                MageArmor.guiPresentation.SpriteReference)
            .SetUniqueInstance()
            .SetCustomSubFeatures(
                DoNotTerminateWhileUnconscious.Marker,
                ExtraCarefulTrackedItem.Marker,
                SkipEffectRemovalOnLocationChange.Always,
                new CustomItemFilter(CanArmorBeReinforced))
            .SetUsesFixed(ActivationTime.Action, RechargeRate.ShortRest)
            .SetOverriddenPower(powerReinforceArmor6)
            .SetEffectDescription(EffectDescriptionBuilder
                .Create()
                .SetTargetingData(Side.All, RangeType.Self, 0, TargetType.Item,
                    itemSelectionType: ActionDefinitions.ItemSelectionType.Carried)
                .SetDurationData(DurationType.Permanent)
                .SetEffectForms(EffectFormBuilder
                    .Create()
                    .SetItemPropertyForm(
                        ItemPropertyUsage.Unlimited,
                        1, new FeatureUnlockByLevel(
                            FeatureDefinitionAttributeModifierBuilder
                                .Create($"AttributeModifier{NAME}ReinforceArmor3")
                                .SetGuiPresentation($"AttributeModifier{NAME}ReinforceArmor", Category.Feature,
                                    AttributeReinforceDescription(3), MageArmor.guiPresentation.SpriteReference)
                                .SetCustomSubFeatures(ExtraCarefulTrackedItem.Marker)
                                .SetModifier(FeatureDefinitionAttributeModifier.AttributeModifierOperation.Additive,
                                    AttributeDefinitions.ArmorClass, 3)
                                .AddToDB()
                            , 0)
                    )
                    .Build())
                .Build())
            .AddToDB();

        var powerReinforceArmor16 = FeatureDefinitionPowerBuilder
            .Create($"Power{NAME}ReinforceArmor16")
            .SetGuiPresentation($"Power{NAME}ReinforceArmor", Category.Feature, PowerReinforceDescription(4),
                MageArmor.guiPresentation.SpriteReference)
            .SetUniqueInstance()
            .SetCustomSubFeatures(
                DoNotTerminateWhileUnconscious.Marker,
                ExtraCarefulTrackedItem.Marker,
                SkipEffectRemovalOnLocationChange.Always,
                new CustomItemFilter(CanArmorBeReinforced))
            .SetUsesFixed(ActivationTime.Action, RechargeRate.ShortRest)
            .SetOverriddenPower(powerReinforceArmor11)
            .SetEffectDescription(EffectDescriptionBuilder
                .Create()
                .SetTargetingData(Side.All, RangeType.Self, 0, TargetType.Item,
                    itemSelectionType: ActionDefinitions.ItemSelectionType.Carried)
                .SetDurationData(DurationType.Permanent)
                .SetEffectForms(EffectFormBuilder
                    .Create()
                    .SetItemPropertyForm(
                        ItemPropertyUsage.Unlimited,
                        1, new FeatureUnlockByLevel(
                            FeatureDefinitionAttributeModifierBuilder
                                .Create($"AttributeModifier{NAME}ReinforceArmor4")
                                .SetGuiPresentation($"AttributeModifier{NAME}ReinforceArmor", Category.Feature,
                                    AttributeReinforceDescription(4), MageArmor.guiPresentation.SpriteReference)
                                .SetCustomSubFeatures(ExtraCarefulTrackedItem.Marker)
                                .SetModifier(FeatureDefinitionAttributeModifier.AttributeModifierOperation.Additive,
                                    AttributeDefinitions.ArmorClass, 4)
                                .AddToDB()
                            , 0)
                    )
                    .Build())
                .Build())
            .AddToDB();

        //
        // 2
        //

        var conditionAdamantBenediction = ConditionDefinitionBuilder
            .Create($"Condition{NAME}AdamantBenediction")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionShielded)
            .SetFeatures(FeatureDefinitionCombatAffinitys.CombatAffinityAdamantinePlateArmor)
            .AddToDB();

        var powerAdamantBenediction = FeatureDefinitionPowerBuilder
            .Create($"Power{NAME}AdamantBenediction")
            .SetGuiPresentation(Category.Feature, Shield)
            .SetUsesFixed(ActivationTime.Action, RechargeRate.ChannelDivinity)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetParticleEffectParameters(PowerOathOfDevotionTurnUnholy.EffectDescription
                        .effectParticleParameters)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Sphere, 6)
                    .SetTargetFiltering(TargetFilteringMethod.CharacterOnly)
                    .SetDurationData(DurationType.Minute, 1)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(
                                conditionAdamantBenediction,
                                ConditionForm.ConditionOperation.Add)
                            .Build())
                    .Build())
            .AddToDB();

        //
        // 6
        //

        const string DIVINE_STRIKE_DESCRIPTION = "Feature/&AdditionalDamageDomainSmithDivineStrikeDescription";

        static string PowerDivineStrikeDescription(int x)
        {
            return Gui.Format(DIVINE_STRIKE_DESCRIPTION, x.ToString());
        }

        var additionalDamageDivineStrike6 = FeatureDefinitionAdditionalDamageBuilder
            .Create($"AdditionalDamage{NAME}DivineStrike6")
            .SetGuiPresentation($"AdditionalDamage{NAME}DivineStrike", Category.Feature,
                PowerDivineStrikeDescription(1))
            .SetNotificationTag("DivineStrike")
            .SetSpecificDamageType(DamageTypeFire)
            .SetDamageDice(DieType.D8, 1)
            .SetAdvancement(AdditionalDamageAdvancement.ClassLevel, 1, 1, 8, 6)
            .SetFrequencyLimit(FeatureLimitedUsage.OnceInMyTurn)
            .SetAttackModeOnly()
            .AddToDB();

        var additionalDamageDivineStrike14 = FeatureDefinitionBuilder
            .Create($"AdditionalDamage{NAME}DivineStrike14")
            .SetGuiPresentation($"AdditionalDamage{NAME}DivineStrike", Category.Feature,
                PowerDivineStrikeDescription(2))
            .AddToDB();

        //
        // 8
        //

        var attributeModifierForgeMastery = FeatureDefinitionAttributeModifierBuilder
            .Create($"AttributeModifier{NAME}ForgeMastery")
            .SetGuiPresentation(Category.Feature)
            .SetSituationalContext(SituationalContext.WearingArmor)
            .SetModifier(FeatureDefinitionAttributeModifier.AttributeModifierOperation.Additive,
                AttributeDefinitions.ArmorClass,
                1)
            .AddToDB();

        var damageAffinityForgeMastery = FeatureDefinitionDamageAffinityBuilder
            .Create(FeatureDefinitionDamageAffinitys.DamageAffinityFireResistance, "DamageAffinityForgeMastery")
            .SetDamageAffinityType(DamageAffinityType.Resistance)
            .SetDamageType(DamageTypeFire)
            .AddToDB();

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Subclass, FightingStyleDefinitions.GreatWeapon)
            .AddFeaturesAtLevel(1, autoPreparedSpellsDomainSmith, bonusProficiencyDomainForge, powerReinforceArmor1)
            .AddFeaturesAtLevel(2, powerAdamantBenediction)
            .AddFeaturesAtLevel(6, additionalDamageDivineStrike6, powerReinforceArmor6)
            .AddFeaturesAtLevel(8, attributeModifierForgeMastery, damageAffinityForgeMastery)
            .AddFeaturesAtLevel(10, PowerClericDivineInterventionPaladin)
            .AddFeaturesAtLevel(11, powerReinforceArmor11)
            .AddFeaturesAtLevel(14, additionalDamageDivineStrike14)
            .AddFeaturesAtLevel(16, powerReinforceArmor16)
            .AddToDB();
    }

    internal override CharacterSubclassDefinition Subclass { get; }

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override FeatureDefinitionSubclassChoice SubclassChoice { get; }
    internal override DeityDefinition DeityDefinition => DeityDefinitions.Pakri;

    private static bool CanArmorBeReinforced(RulesetCharacter character, RulesetItem item)
    {
        var definition = item.ItemDefinition;

        if (!definition.IsArmor || !character.IsProficientWithItem(definition))
        {
            return false;
        }

        return !definition.Magical;
    }
}
