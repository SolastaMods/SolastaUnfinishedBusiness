using System.Collections;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.BehaviorsGeneric;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Properties;
using static RuleDefinitions;
using static FeatureDefinitionAttributeModifier;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static SolastaUnfinishedBusiness.Builders.Features.AutoPreparedSpellsGroupBuilder;

namespace SolastaUnfinishedBusiness.Subclasses;

[UsedImplicitly]
public sealed class DomainSmith : AbstractSubclass
{
    public DomainSmith()
    {
        const string NAME = "DomainSmith";

        // LEVEL 01

        // Auto Prepared Spells

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

        // Heavy Armor Proficiency

        var bonusProficiencyArmorDomainForge = FeatureDefinitionProficiencyBuilder
            .Create($"BonusProficiency{NAME}")
            .SetGuiPresentation(Category.Feature)
            .SetProficiencies(ProficiencyType.Armor, EquipmentDefinitions.HeavyArmorCategory)
            .AddToDB();

        // Artisan Tools Proficiency

        var bonusProficiencyArtisanToolTypeDomainForge = FeatureDefinitionProficiencyBuilder
            .Create($"BonusProficiency{NAME}ArtisanToolType")
            .SetGuiPresentation(Category.Feature)
            .SetProficiencies(ProficiencyType.Tool, ToolDefinitions.ArtisanToolType)
            .AddToDB();

        // Reinforce Armor - 1, 6, 11, 16

        var spriteReference = MageArmor.guiPresentation.SpriteReference;

        var powerReinforceArmor1 = FeatureDefinitionPowerBuilder
            .Create($"Power{NAME}ReinforceArmor1")
            .SetGuiPresentation(
                $"Power{NAME}ReinforceArmor", Category.Feature, PowerReinforceDescription(1), spriteReference)
            .SetUniqueInstance()
            .AddCustomSubFeatures(
                RestrictEffectToNotTerminateWhileUnconscious.Marker,
                TrackItemsCarefully.Marker,
                SkipEffectRemovalOnLocationChange.Always,
                new CustomItemFilter(CanArmorBeReinforced))
            .SetUsesFixed(ActivationTime.Action, RechargeRate.ShortRest)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.All, RangeType.Self, 0, TargetType.Item,
                        itemSelectionType: ActionDefinitions.ItemSelectionType.Carried)
                    .SetDurationData(DurationType.Permanent)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetItemPropertyForm(
                                ItemPropertyUsage.Unlimited,
                                1,
                                new FeatureUnlockByLevel(
                                    FeatureDefinitionAttributeModifierBuilder
                                        .Create($"AttributeModifier{NAME}ReinforceArmor1")
                                        .SetGuiPresentation($"AttributeModifier{NAME}ReinforceArmor", Category.Feature,
                                            AttributeReinforceDescription(1), spriteReference)
                                        .AddCustomSubFeatures(TrackItemsCarefully.Marker)
                                        .SetModifier(AttributeModifierOperation.Additive,
                                            AttributeDefinitions.ArmorClass, 1)
                                        .AddToDB(),
                                    0))
                            .Build())
                    .Build())
            .AddToDB();

        var powerReinforceArmor6 = FeatureDefinitionPowerBuilder
            .Create($"Power{NAME}ReinforceArmor6")
            .SetGuiPresentation(
                $"Power{NAME}ReinforceArmor", Category.Feature, PowerReinforceDescription(2), spriteReference)
            .SetUniqueInstance()
            .AddCustomSubFeatures(
                RestrictEffectToNotTerminateWhileUnconscious.Marker,
                TrackItemsCarefully.Marker,
                SkipEffectRemovalOnLocationChange.Always,
                new CustomItemFilter(CanArmorBeReinforced))
            .SetUsesFixed(ActivationTime.Action, RechargeRate.ShortRest)
            .SetOverriddenPower(powerReinforceArmor1)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.All, RangeType.Self, 0, TargetType.Item,
                        itemSelectionType: ActionDefinitions.ItemSelectionType.Carried)
                    .SetDurationData(DurationType.Permanent)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetItemPropertyForm(
                                ItemPropertyUsage.Unlimited,
                                1,
                                new FeatureUnlockByLevel(
                                    FeatureDefinitionAttributeModifierBuilder
                                        .Create($"AttributeModifier{NAME}ReinforceArmor2")
                                        .SetGuiPresentation($"AttributeModifier{NAME}ReinforceArmor", Category.Feature,
                                            AttributeReinforceDescription(2), spriteReference)
                                        .AddCustomSubFeatures(TrackItemsCarefully.Marker)
                                        .SetModifier(
                                            AttributeModifierOperation.Additive, AttributeDefinitions.ArmorClass, 2)
                                        .AddToDB(),
                                    0))
                            .Build())
                    .Build())
            .AddToDB();

        var powerReinforceArmor11 = FeatureDefinitionPowerBuilder
            .Create($"Power{NAME}ReinforceArmor11")
            .SetGuiPresentation(
                $"Power{NAME}ReinforceArmor", Category.Feature, PowerReinforceDescription(3), spriteReference)
            .SetUniqueInstance()
            .AddCustomSubFeatures(
                RestrictEffectToNotTerminateWhileUnconscious.Marker,
                TrackItemsCarefully.Marker,
                SkipEffectRemovalOnLocationChange.Always,
                new CustomItemFilter(CanArmorBeReinforced))
            .SetUsesFixed(ActivationTime.Action, RechargeRate.ShortRest)
            .SetOverriddenPower(powerReinforceArmor6)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.All, RangeType.Self, 0, TargetType.Item,
                        itemSelectionType: ActionDefinitions.ItemSelectionType.Carried)
                    .SetDurationData(DurationType.Permanent)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetItemPropertyForm(
                                ItemPropertyUsage.Unlimited,
                                1,
                                new FeatureUnlockByLevel(
                                    FeatureDefinitionAttributeModifierBuilder
                                        .Create($"AttributeModifier{NAME}ReinforceArmor3")
                                        .SetGuiPresentation($"AttributeModifier{NAME}ReinforceArmor", Category.Feature,
                                            AttributeReinforceDescription(3), spriteReference)
                                        .AddCustomSubFeatures(TrackItemsCarefully.Marker)
                                        .SetModifier(
                                            AttributeModifierOperation.Additive, AttributeDefinitions.ArmorClass, 3)
                                        .AddToDB(),
                                    0))
                            .Build())
                    .Build())
            .AddToDB();

        var powerReinforceArmor16 = FeatureDefinitionPowerBuilder
            .Create($"Power{NAME}ReinforceArmor16")
            .SetGuiPresentation(
                $"Power{NAME}ReinforceArmor", Category.Feature, PowerReinforceDescription(4), spriteReference)
            .SetUniqueInstance()
            .AddCustomSubFeatures(
                RestrictEffectToNotTerminateWhileUnconscious.Marker,
                TrackItemsCarefully.Marker,
                SkipEffectRemovalOnLocationChange.Always,
                new CustomItemFilter(CanArmorBeReinforced))
            .SetUsesFixed(ActivationTime.Action, RechargeRate.ShortRest)
            .SetOverriddenPower(powerReinforceArmor11)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.All, RangeType.Self, 0, TargetType.Item,
                        itemSelectionType: ActionDefinitions.ItemSelectionType.Carried)
                    .SetDurationData(DurationType.Permanent)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetItemPropertyForm(
                                ItemPropertyUsage.Unlimited,
                                1,
                                new FeatureUnlockByLevel(
                                    FeatureDefinitionAttributeModifierBuilder
                                        .Create($"AttributeModifier{NAME}ReinforceArmor4")
                                        .SetGuiPresentation($"AttributeModifier{NAME}ReinforceArmor", Category.Feature,
                                            AttributeReinforceDescription(4), spriteReference)
                                        .AddCustomSubFeatures(TrackItemsCarefully.Marker)
                                        .SetModifier(
                                            AttributeModifierOperation.Additive, AttributeDefinitions.ArmorClass, 4)
                                        .AddToDB(),
                                    0))
                            .Build())
                    .Build())
            .AddToDB();

        // LEVEL 02

        var divinePowerPrefix = Gui.Localize("Feature/&ClericChannelDivinityTitle") + ": ";

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
                    .SetParticleEffectParameters(PowerOathOfDevotionTurnUnholy)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Sphere, 6)
                    .SetTargetFiltering(TargetFilteringMethod.CharacterOnly)
                    .SetDurationData(DurationType.Minute, 1)
                    .SetEffectForms(EffectFormBuilder.ConditionForm(conditionAdamantBenediction))
                    .Build())
            .AddToDB();

        var featureSetAdamantBenediction = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{NAME}DefileLife")
            .SetGuiPresentation(
                divinePowerPrefix + powerAdamantBenediction.FormatTitle(), powerAdamantBenediction.FormatDescription())
            .AddFeatureSet(powerAdamantBenediction)
            .AddToDB();

        // LEVEL 06

        var attributeModifierForgeMastery = FeatureDefinitionAttributeModifierBuilder
            .Create($"AttributeModifier{NAME}ForgeMastery")
            .SetGuiPresentation(Category.Feature)
            .SetSituationalContext(SituationalContext.WearingArmor)
            .SetModifier(AttributeModifierOperation.Additive, AttributeDefinitions.ArmorClass, 1)
            .AddToDB();

        var damageAffinityForgeMastery = FeatureDefinitionDamageAffinityBuilder
            .Create("DamageAffinityForgeMastery")
            .SetGuiPresentationNoContent(true)
            .SetDamageAffinityType(DamageAffinityType.Resistance)
            .SetDamageType(DamageTypeFire)
            .AddToDB();

        // LEVEL 08

        var additionalDamageDivineStrike = FeatureDefinitionAdditionalDamageBuilder
            .Create($"AdditionalDamage{NAME}DivineStrike")
            .SetGuiPresentation(Category.Feature)
            .SetNotificationTag("DivineStrike")
            .SetDamageDice(DieType.D8, 1)
            .SetAdvancement(AdditionalDamageAdvancement.ClassLevel, 1, 1, 8, 6)
            .SetSpecificDamageType(DamageTypeFire)
            .SetFrequencyLimit(FeatureLimitedUsage.OnceInMyTurn)
            .SetAttackModeOnly()
            .AddToDB();

        // LEVEL 17

        const string BLESSED_METAL = $"Feature{NAME}BlessedMetal";

        var damageAffinityBlessedMetalFireImmunity = FeatureDefinitionDamageAffinityBuilder
            .Create($"DamageAffinity{NAME}BlessedMetalFireImmunity")
            .SetGuiPresentation(BLESSED_METAL, Category.Feature)
            .SetDamageType(DamageTypeFire)
            .SetDamageAffinityType(DamageAffinityType.Immunity)
            .AddToDB();

        var damageAffinityBlessedMetalBludgeoningResistance = FeatureDefinitionDamageAffinityBuilder
            .Create($"DamageAffinity{NAME}BlessedMetalBludgeoningResistance")
            .SetGuiPresentation(BLESSED_METAL, Category.Feature)
            .SetDamageType(DamageTypeBludgeoning)
            .SetDamageAffinityType(DamageAffinityType.Resistance)
            .AddToDB();

        var damageAffinityBlessedMetalPiercingResistance = FeatureDefinitionDamageAffinityBuilder
            .Create($"DamageAffinity{NAME}BlessedMetalPiercingResistance")
            .SetGuiPresentation(BLESSED_METAL, Category.Feature)
            .SetDamageType(DamageTypePiercing)
            .SetDamageAffinityType(DamageAffinityType.Resistance)
            .AddToDB();

        var damageAffinityBlessedMetalSlashingResistance = FeatureDefinitionDamageAffinityBuilder
            .Create($"DamageAffinity{NAME}BlessedMetalSlashingResistance")
            .SetGuiPresentation(BLESSED_METAL, Category.Feature)
            .SetDamageType(DamageTypeSlashing)
            .SetDamageAffinityType(DamageAffinityType.Resistance)
            .AddToDB();

        var conditionBlessedMetal = ConditionDefinitionBuilder
            .Create($"Condition{NAME}BlessedMetal")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetSpecialDuration(DurationType.Round, 0, TurnOccurenceType.StartOfTurn)
            .SetFeatures(
                damageAffinityBlessedMetalFireImmunity,
                damageAffinityBlessedMetalBludgeoningResistance,
                damageAffinityBlessedMetalPiercingResistance,
                damageAffinityBlessedMetalSlashingResistance)
            .SetSpecialInterruptions(ConditionInterruption.AnyBattleTurnEnd)
            .AddToDB();

        var featureBlessedMetal = FeatureDefinitionBuilder
            .Create(BLESSED_METAL)
            .SetGuiPresentation(Category.Feature)
            .AddCustomSubFeatures(new PhysicalAttackInitiatedOnMeBlessedMetal(conditionBlessedMetal))
            .AddToDB();

        // MAIN

        ForceGlobalUniqueEffects.AddToGroup(ForceGlobalUniqueEffects.Group.DomainSmithReinforceArmor,
            powerReinforceArmor1,
            powerReinforceArmor6,
            powerReinforceArmor11,
            powerReinforceArmor16);

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Subclass, Sprites.GetSprite(NAME, Resources.DomainSmith, 256))
            .AddFeaturesAtLevel(1,
                autoPreparedSpellsDomainSmith,
                bonusProficiencyArmorDomainForge,
                bonusProficiencyArtisanToolTypeDomainForge,
                powerReinforceArmor1)
            .AddFeaturesAtLevel(2,
                featureSetAdamantBenediction)
            .AddFeaturesAtLevel(6,
                attributeModifierForgeMastery,
                damageAffinityForgeMastery,
                powerReinforceArmor6)
            .AddFeaturesAtLevel(8,
                additionalDamageDivineStrike)
            .AddFeaturesAtLevel(10,
                PowerClericDivineInterventionPaladin)
            .AddFeaturesAtLevel(11,
                powerReinforceArmor11)
            .AddFeaturesAtLevel(16,
                powerReinforceArmor16)
            .AddFeaturesAtLevel(17,
                featureBlessedMetal)
            .AddToDB();

        return;

        static string PowerReinforceDescription(int x)
        {
            return Gui.Format("Feature/&PowerDomainSmithReinforceArmorDescription", x.ToString());
        }

        static string AttributeReinforceDescription(int x)
        {
            return Gui.Format("Feature/&AttributeModifierDomainSmithReinforceArmorDescription", x.ToString());
        }
    }

    internal override CharacterClassDefinition Klass => CharacterClassDefinitions.Cleric;

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

    private sealed class PhysicalAttackInitiatedOnMeBlessedMetal(ConditionDefinition conditionBlessedMetal)
        : IPhysicalAttackInitiatedOnMe
    {
        public IEnumerator OnPhysicalAttackInitiatedOnMe(
            GameLocationBattleManager __instance,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier attackModifier,
            RulesetAttackMode attackerAttackMode)
        {
            var rulesetDefender = defender.RulesetCharacter;

            if (!defender.RulesetCharacter.IsWearingArmor())
            {
                yield break;
            }

            rulesetDefender.InflictCondition(
                conditionBlessedMetal.Name,
                conditionBlessedMetal.DurationType,
                conditionBlessedMetal.DurationParameter,
                conditionBlessedMetal.TurnOccurence,
                AttributeDefinitions.TagEffect,
                rulesetDefender.guid,
                rulesetDefender.CurrentFaction.Name,
                1,
                conditionBlessedMetal.Name,
                0,
                0,
                0);
        }
    }
}
