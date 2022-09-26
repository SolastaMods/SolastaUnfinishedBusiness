using System.Collections.Generic;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterSubclassDefinitions;

namespace SolastaUnfinishedBusiness.Subclasses;

internal sealed class WizardArcaneFighter : AbstractSubclass
{
    // private static FeatureDefinitionPower _enchantWeapon;

    // ReSharper disable once InconsistentNaming
    private readonly CharacterSubclassDefinition Subclass;

    internal WizardArcaneFighter()
    {
        // Make Melee Wizard subclass

        var weaponProficiency = FeatureDefinitionProficiencyBuilder
            .Create("ProficiencyArcaneFighterSimpleWeapons")
            .SetGuiPresentation(Category.Feature)
            .SetProficiencies(
                RuleDefinitions.ProficiencyType.Weapon,
                EquipmentDefinitions.SimpleWeaponCategory,
                EquipmentDefinitions.MartialWeaponCategory)
            .AddToDB();

        var concentrationAffinity = FeatureDefinitionMagicAffinityBuilder
            .Create("MagicAffinityArcaneFighterConcentrationAdvantage")
            .SetGuiPresentation(Category.Feature)
            .SetConcentrationModifiers(RuleDefinitions.ConcentrationAffinity.Advantage)
            .AddToDB();

        var extraAttack = FeatureDefinitionAttributeModifierBuilder
            .Create("AttributeModifierArcaneFighterExtraAttack")
            .SetGuiPresentation(Category.Feature)
            .SetModifier(
                FeatureDefinitionAttributeModifier.AttributeModifierOperation.Additive,
                AttributeDefinitions.AttacksNumber, 1)
            .AddToDB();

        var bonusSpell = FeatureDefinitionAdditionalActionBuilder
            .Create("AdditionalActionArcaneFighter")
            .SetGuiPresentation(Category.Feature)
            .SetActionType(ActionDefinitions.ActionType.Main)
            .SetRestrictedActions(ActionDefinitions.Id.CastMain)
            .SetMaxAttacksNumber(-1)
            .SetTriggerCondition(RuleDefinitions.AdditionalActionTriggerCondition.HasDownedAnEnemy)
            .AddToDB();

        var bonusWeaponDamage = FeatureDefinitionAdditionalDamageBuilder
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

        var enchantWeapon = BuildEnchantWeapon();

        Subclass = CharacterSubclassDefinitionBuilder
            .Create("WizardArcaneFighter")
            .SetGuiPresentation(Category.Subclass,
                MartialSpellblade.GuiPresentation.SpriteReference)
            .AddFeaturesAtLevel(2, weaponProficiency, concentrationAffinity, enchantWeapon)
            .AddFeaturesAtLevel(6, extraAttack)
            .AddFeaturesAtLevel(10, bonusSpell)
            .AddFeaturesAtLevel(14, bonusWeaponDamage)
            .AddToDB();
    }

    // private static FeatureDefinitionPower EnchantWeapon => _enchantWeapon ??= BuildEnchantWeapon();

    internal override FeatureDefinitionSubclassChoice GetSubclassChoiceList()
    {
        return FeatureDefinitionSubclassChoices.SubclassChoiceWizardArcaneTraditions;
    }

    internal override CharacterSubclassDefinition GetSubclass()
    {
        return Subclass;
    }

    private static FeatureDefinitionPower BuildEnchantWeapon()
    {
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

        return FeatureDefinitionPowerBuilder
            .Create("PowerArcaneFighterEnchantWeapon")
            .SetGuiPresentation("AttackModifierArcaneFighterIntBonus", Category.Feature,
                FeatureDefinitionPowers.PowerDomainElementalLightningBlade.GuiPresentation.SpriteReference)
            .Configure(0, RuleDefinitions.UsesDetermination.ProficiencyBonus, AttributeDefinitions.Intelligence,
                RuleDefinitions.ActivationTime.BonusAction, 1, RuleDefinitions.RechargeRate.ShortRest, false, false,
                AttributeDefinitions.Intelligence, effect)
            .SetCustomSubFeatures(FeatureDefinitionSkipEffectRemovalOnLocationChange.Always)
            .AddToDB();
    }

    // internal static void UpdateEnchantWeapon()
    // {
    //     EnchantWeapon.rechargeRate = Main.Settings.EnableShortRestRechargeOfArcaneWeaponOnWizardArcaneFighter
    //         ? RuleDefinitions.RechargeRate.ShortRest
    //         : RuleDefinitions.RechargeRate.LongRest;
    // }
}
