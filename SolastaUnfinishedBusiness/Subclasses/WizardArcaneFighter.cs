using System.Collections.Generic;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterSubclassDefinitions;
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.Subclasses;

internal sealed class WizardArcaneFighter : AbstractSubclass
{
    internal WizardArcaneFighter()
    {
        var proficiencyArcaneFighterSimpleWeapons = FeatureDefinitionProficiencyBuilder
            .Create("ProficiencyArcaneFighterSimpleWeapons")
            .SetGuiPresentation(Category.Feature)
            .SetProficiencies(
                ProficiencyType.Weapon,
                EquipmentDefinitions.SimpleWeaponCategory,
                EquipmentDefinitions.MartialWeaponCategory)
            .AddToDB();

        var magicAffinityArcaneFighterConcentrationAdvantage = FeatureDefinitionMagicAffinityBuilder
            .Create("MagicAffinityArcaneFighterConcentrationAdvantage")
            .SetGuiPresentation(Category.Feature)
            .SetConcentrationModifiers(ConcentrationAffinity.Advantage)
            .AddToDB();

        var attributeModifierArcaneFighterExtraAttack = FeatureDefinitionAttributeModifierBuilder
            .Create("AttributeModifierArcaneFighterExtraAttack")
            .SetGuiPresentation(Category.Feature)
            .SetModifier(
                FeatureDefinitionAttributeModifier.AttributeModifierOperation.ForceIfBetter,
                AttributeDefinitions.AttacksNumber, 2)
            .AddToDB();

        var additionalActionArcaneFighter = FeatureDefinitionAdditionalActionBuilder
            .Create("AdditionalActionArcaneFighter")
            .SetGuiPresentation(Category.Feature)
            .SetActionType(ActionDefinitions.ActionType.Main)
            .SetRestrictedActions(ActionDefinitions.Id.CastMain)
            .SetMaxAttacksNumber(-1)
            .SetTriggerCondition(AdditionalActionTriggerCondition.HasDownedAnEnemy)
            .AddToDB();

        var additionalDamageArcaneFighterBonusWeapon = FeatureDefinitionAdditionalDamageBuilder
            .Create("AdditionalDamageArcaneFighterBonusWeapon")
            .Configure(
                "ArcaneFighter",
                FeatureLimitedUsage.OncePerTurn,
                AdditionalDamageValueDetermination.Die,
                AdditionalDamageTriggerCondition.AlwaysActive,
                RestrictedContextRequiredProperty.None,
                true /* attack only */,
                DieType.D8,
                1 /* dice number */,
                AdditionalDamageType.SameAsBaseDamage,
                string.Empty,
                AdditionalDamageAdvancement.None,
                new List<DiceByRank>())
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        var powerArcaneFighterEnchantWeapon = FeatureDefinitionPowerBuilder
            .Create("PowerArcaneFighterEnchantWeapon")
            .SetGuiPresentation("AttackModifierArcaneFighterIntBonus", Category.Feature,
                FeatureDefinitionPowers.PowerDomainElementalLightningBlade.GuiPresentation.SpriteReference)
            .Configure(
                UsesDetermination.ProficiencyBonus,
                ActivationTime.BonusAction,
                RechargeRate.ShortRest,
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(
                        Side.Ally,
                        RangeType.Touch,
                        1 /* range */,
                        TargetType.Item,
                        1,
                        2,
                        ActionDefinitions.ItemSelectionType.Weapon)
                    .SetCreatedByCharacter()
                    .SetDurationData(
                        DurationType.Minute,
                        10 /* duration */)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetItemPropertyForm(
                                ItemPropertyUsage.Unlimited,
                                0,
                                new FeatureUnlockByLevel(
                                    FeatureDefinitionAttackModifierBuilder
                                        .Create("AttackModifierArcaneFighterIntBonus")
                                        .SetGuiPresentation(Category.Feature,
                                            FeatureDefinitionAttackModifiers.AttackModifierMagicWeapon.GuiPresentation
                                                .SpriteReference)
                                        .SetAbilityScoreReplacement(AbilityScoreReplacement.SpellcastingAbility)
                                        .SetAdditionalAttackTag(TagsDefinitions.Magical)
                                        .AddToDB(),
                                    0))
                            .Build()
                    )
                    .Build())
            .SetCustomSubFeatures(SkipEffectRemovalOnLocationChange.Always)
            .AddToDB();

        Subclass = CharacterSubclassDefinitionBuilder
            .Create("WizardArcaneFighter")
            .SetGuiPresentation(Category.Subclass,
                MartialSpellblade.GuiPresentation.SpriteReference)
            .AddFeaturesAtLevel(2,
                proficiencyArcaneFighterSimpleWeapons,
                magicAffinityArcaneFighterConcentrationAdvantage,
                powerArcaneFighterEnchantWeapon)
            .AddFeaturesAtLevel(6,
                attributeModifierArcaneFighterExtraAttack)
            .AddFeaturesAtLevel(10,
                additionalActionArcaneFighter)
            .AddFeaturesAtLevel(14,
                additionalDamageArcaneFighterBonusWeapon)
            .AddToDB();
    }

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceWizardArcaneTraditions;
}
