using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomDefinitions;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Properties;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterFamilyDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionAttackModifiers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionDamageAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;

namespace SolastaUnfinishedBusiness.Subclasses;

internal static class CommonBuilders
{
    // BACKWARD COMPATIBILITY
    internal static readonly FeatureDefinitionAdditionalDamage AdditionalDamageMarshalFavoredEnemyHumanoid =
        FeatureDefinitionAdditionalDamageBuilder
            .Create("AdditionalDamageMarshalFavoredEnemyHumanoid")
            .SetGuiPresentationNoContent(true)
            .SetNotificationTag("FavoredEnemy")
            .SetTriggerCondition(AdditionalDamageTriggerCondition.SpecificCharacterFamily)
            .SetDamageValueDetermination(AdditionalDamageValueDetermination.TargetKnowledgeLevel)
            .SetAdditionalDamageType(AdditionalDamageType.SameAsBaseDamage)
            .SetRequiredCharacterFamily(Humanoid)
            .AddToDB();

    internal static readonly FeatureDefinitionAttributeModifier AttributeModifierCasterFightingExtraAttack =
        FeatureDefinitionAttributeModifierBuilder
            .Create("AttributeModifierCasterFightingExtraAttack")
            .SetGuiPresentation(Category.Feature)
            .SetModifier(
                FeatureDefinitionAttributeModifier.AttributeModifierOperation.ForceIfBetter,
                AttributeDefinitions.AttacksNumber, 2)
            .AddToDB();

    internal static readonly FeatureDefinitionDamageAffinity DamageAffinityGenericHardenToNecrotic =
        FeatureDefinitionDamageAffinityBuilder
            .Create(DamageAffinityNecroticImmunity, "DamageAffinityGenericHardenToNecrotic")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

    internal static readonly FeatureDefinitionFeatureSet FeatureSetCasterFightingProficiency =
        FeatureDefinitionFeatureSetBuilder
            .Create("FeatureSetCasterFightingProficiency")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(
                FeatureDefinitionProficiencyBuilder
                    .Create("ProficiencyCasterFightingArmor")
                    .SetGuiPresentationNoContent(true)
                    .SetProficiencies(ProficiencyType.Armor,
                        EquipmentDefinitions.LightArmorCategory,
                        EquipmentDefinitions.MediumArmorCategory,
                        EquipmentDefinitions.ShieldCategory)
                    .AddToDB(),
                FeatureDefinitionProficiencyBuilder
                    .Create("ProficiencyCasterFightingWeapon")
                    .SetGuiPresentationNoContent(true)
                    .SetProficiencies(ProficiencyType.Weapon,
                        EquipmentDefinitions.SimpleWeaponCategory,
                        EquipmentDefinitions.MartialWeaponCategory)
                    .AddToDB())
            .AddToDB();

    internal static readonly FeatureDefinitionMagicAffinity MagicAffinityCasterFightingCombatMagic =
        FeatureDefinitionMagicAffinityBuilder
            .Create("MagicAffinityCasterFightingCombatMagic")
            .SetGuiPresentation(Category.Feature)
            .SetConcentrationModifiers(ConcentrationAffinity.Advantage, 0)
            .SetHandsFullCastingModifiers(true, true, true)
            .SetCastingModifiers(0, SpellParamsModifierType.None, 0, SpellParamsModifierType.FlatValue, true)
            .AddToDB();

    internal static readonly FeatureDefinitionPower PowerCasterFightingWarMagic = FeatureDefinitionPowerBuilder
        .Create("PowerCasterFightingWarMagic")
        .SetGuiPresentation(Category.Feature)
        .SetUsesFixed(ActivationTime.OnSpellCast)
        .SetEffectDescription(EffectDescriptionBuilder.Create()
            .SetTargetingData(Side.Enemy, RangeType.Self, 0, TargetType.Self)
            .SetDurationData(DurationType.Round, 1, TurnOccurenceType.StartOfTurn)
            .SetEffectForms(EffectFormBuilder.Create()
                .SetConditionForm(ConditionDefinitionBuilder
                        .Create("ConditionCasterFightingWarMagic")
                        .SetGuiPresentationNoContent(true)
                        .AddFeatures(FeatureDefinitionAttackModifierBuilder
                            .Create("PowerCasterFightingWarMagicAttack")
                            .SetGuiPresentation("PowerCasterFightingWarMagic", Category.Feature)
                            .SetDamageRollModifier(1)
                            .SetCustomSubFeatures(
                                new AddExtraMainHandAttack(ActionDefinitions.ActionType.Bonus))
                            .AddToDB())
                        .AddToDB(),
                    ConditionForm.ConditionOperation.Add)
                .Build()
            )
            .Build())
        .AddToDB();

    internal static readonly FeatureDefinitionPower PowerCasterCommandUndead = FeatureDefinitionPowerBuilder
        .Create("PowerCasterCommandUndead")
        .SetGuiPresentation(Category.Feature,
            Sprites.GetSprite("PowerCommandUndead", Resources.PowerCommandUndead, 256, 128))
        .SetUsesProficiencyBonus(ActivationTime.Action)
        .SetEffectDescription(
            EffectDescriptionBuilder
                .Create(DominateBeast.EffectDescription)
                .SetEffectAdvancement(EffectIncrementMethod.None)
                .SetRestrictedCreatureFamilies(Undead)
                .SetSavingThrowData(
                    false,
                    AttributeDefinitions.Charisma,
                    false,
                    EffectDifficultyClassComputation.SpellCastingFeature)
                .Build())
        .AddToDB();

    internal static readonly FeatureDefinitionReplaceAttackWithCantrip ReplaceAttackWithCantripCasterFighting =
        FeatureDefinitionReplaceAttackWithCantripBuilder
            .Create("ReplaceAttackWithCantripCasterFighting")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

    internal static readonly FeatureDefinitionPower PowerArcaneFighterEnchantWeapon = FeatureDefinitionPowerBuilder
        .Create("PowerArcaneFighterEnchantWeapon")
        .SetGuiPresentation(Category.Feature, PowerDomainElementalLightningBlade)
        .SetUsesFixed(ActivationTime.Action, RechargeRate.ShortRest)
        .SetUniqueInstance()
        .SetEffectDescription(
            EffectDescriptionBuilder
                .Create()
                .SetTargetingData(
                    Side.Ally,
                    RangeType.Touch,
                    0,
                    TargetType.Item,
                    itemSelectionType: ActionDefinitions.ItemSelectionType.Weapon)
                .SetDurationData(DurationType.Permanent)
                .SetEffectForms(
                    EffectFormBuilder
                        .Create()
                        .SetItemPropertyForm(
                            ItemPropertyUsage.Unlimited,
                            0,
                            new FeatureUnlockByLevel(
                                FeatureDefinitionAttackModifierBuilder
                                    .Create("AttackModifierArcaneFighterIntBonus")
                                    .SetGuiPresentation("PowerArcaneFighterEnchantWeapon", Category.Feature,
                                        AttackModifierMagicWeapon)
                                    .SetAbilityScoreReplacement(AbilityScoreReplacement.SpellcastingAbility)
                                    .SetMagicalWeapon()
                                    .SetAdditionalAttackTag(TagsDefinitions.Magical)
                                    .AddToDB(),
                                0))
                        .Build())
                .Build())
        .SetCustomSubFeatures(
            DoNotTerminateWhileUnconscious.Marker,
            ExtraCarefulTrackedItem.Marker,
            SkipEffectRemovalOnLocationChange.Always)
        .AddToDB();
}
