using System.Collections.Generic;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterSubclassDefinitions;

namespace SolastaUnfinishedBusiness.Subclasses;

internal sealed class WizardArcaneFighter : AbstractSubclass
{
    internal WizardArcaneFighter()
    {
        // Make Melee Wizard subclass

        var proficiencyArcaneFighterSimpleWeapons = FeatureDefinitionProficiencyBuilder
            .Create("ProficiencyArcaneFighterSimpleWeapons")
            .SetGuiPresentation(Category.Feature)
            .SetProficiencies(
                RuleDefinitions.ProficiencyType.Weapon,
                EquipmentDefinitions.SimpleWeaponCategory,
                EquipmentDefinitions.MartialWeaponCategory)
            .AddToDB();

        var magicAffinityArcaneFighterConcentrationAdvantage = FeatureDefinitionMagicAffinityBuilder
            .Create("MagicAffinityArcaneFighterConcentrationAdvantage")
            .SetGuiPresentation(Category.Feature)
            .SetConcentrationModifiers(RuleDefinitions.ConcentrationAffinity.Advantage)
            .AddToDB();

        var attributeModifierArcaneFighterExtraAttack = FeatureDefinitionAttributeModifierBuilder
            .Create("AttributeModifierArcaneFighterExtraAttack")
            .SetGuiPresentation(Category.Feature)
            .SetModifier(
                FeatureDefinitionAttributeModifier.AttributeModifierOperation.Additive,
                AttributeDefinitions.AttacksNumber, 1)
            .AddToDB();

        var additionalActionArcaneFighter = FeatureDefinitionAdditionalActionBuilder
            .Create("AdditionalActionArcaneFighter")
            .SetGuiPresentation(Category.Feature)
            .SetActionType(ActionDefinitions.ActionType.Main)
            .SetRestrictedActions(ActionDefinitions.Id.CastMain)
            .SetMaxAttacksNumber(-1)
            .SetTriggerCondition(RuleDefinitions.AdditionalActionTriggerCondition.HasDownedAnEnemy)
            .AddToDB();

        var additionalDamageArcaneFighterBonusWeapon = FeatureDefinitionAdditionalDamageBuilder
            .Create("AdditionalDamageArcaneFighterBonusWeapon")
            .Configure(
                "AdditionalDamageArcaneFighterBonusWeapon",
                RuleDefinitions.FeatureLimitedUsage.OncePerTurn,
                RuleDefinitions.AdditionalDamageValueDetermination.Die,
                RuleDefinitions.AdditionalDamageTriggerCondition.AlwaysActive,
                RuleDefinitions.RestrictedContextRequiredProperty.None,
                true /* attack only */,
                RuleDefinitions.DieType.D8,
                1 /* dice number */,
                RuleDefinitions.AdditionalDamageType.SameAsBaseDamage,
                string.Empty,
                RuleDefinitions.AdditionalDamageAdvancement.None,
                new List<DiceByRank>())
            .SetNotificationTag("ArcaneFighter")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        var weaponUseIntModifier = FeatureDefinitionAttackModifierBuilder
            .Create("AttackModifierArcaneFighterIntBonus")
            .SetGuiPresentation(Category.Feature,
                FeatureDefinitionAttackModifiers.AttackModifierMagicWeapon.GuiPresentation.SpriteReference)
            .SetAbilityScoreReplacement(RuleDefinitions.AbilityScoreReplacement.SpellcastingAbility)
            .SetAdditionalAttackTag(TagsDefinitions.Magical)
            .AddToDB();

        var effect = EffectDescriptionBuilder
            .Create()
            .SetTargetingData(RuleDefinitions.Side.Ally, RuleDefinitions.RangeType.Touch, 1 /* range */,
                RuleDefinitions.TargetType.Item, 1, 2, ActionDefinitions.ItemSelectionType.Weapon)
            .SetCreatedByCharacter()
            .SetDurationData(RuleDefinitions.DurationType.Minute, 10 /* duration */,
                RuleDefinitions.TurnOccurenceType.EndOfTurn)
            .AddEffectForm(
                EffectFormBuilder
                    .Create()
                    .SetItemPropertyForm(RuleDefinitions.ItemPropertyUsage.Unlimited, 0,
                        new FeatureUnlockByLevel(weaponUseIntModifier, 0))
                    .Build()
            )
            .Build();

        var powerArcaneFighterEnchantWeapon = FeatureDefinitionPowerBuilder
            .Create("PowerArcaneFighterEnchantWeapon")
            .SetGuiPresentation("AttackModifierArcaneFighterIntBonus", Category.Feature,
                FeatureDefinitionPowers.PowerDomainElementalLightningBlade.GuiPresentation.SpriteReference)
            .Configure(0, RuleDefinitions.UsesDetermination.ProficiencyBonus, AttributeDefinitions.Intelligence,
                RuleDefinitions.ActivationTime.BonusAction, 1, RuleDefinitions.RechargeRate.ShortRest, false, false,
                AttributeDefinitions.Intelligence, effect)
            .SetCustomSubFeatures(FeatureDefinitionSkipEffectRemovalOnLocationChange.Always)
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
