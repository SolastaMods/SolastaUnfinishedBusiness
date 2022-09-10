using System.Collections.Generic;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaCommunityExpansion.CustomDefinitions;
using static SolastaCommunityExpansion.Api.DatabaseHelper;
using static SolastaCommunityExpansion.Api.DatabaseHelper.CharacterSubclassDefinitions;

namespace SolastaCommunityExpansion.Subclasses.Wizard;

internal sealed class ArcaneFighter : AbstractSubclass
{
    private static FeatureDefinitionPower _enchantWeapon;

    // ReSharper disable once InconsistentNaming
    private readonly CharacterSubclassDefinition Subclass;

    internal ArcaneFighter()
    {
        // Make Melee Wizard subclass

        var weaponProf = FeatureDefinitionProficiencyBuilder
            .Create("ProficiencyWeaponArcaneFighter", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation(Category.Feature)
            .SetProficiencies(
                RuleDefinitions.ProficiencyType.Weapon,
                EquipmentDefinitions.SimpleWeaponCategory,
                EquipmentDefinitions.MartialWeaponCategory)
            .AddToDB();

        var concentrationAffinity = FeatureDefinitionMagicAffinityBuilder
            .Create("MagicAffinityConcentrationArcaneFighter", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation(Category.Feature)
            .SetConcentrationModifiers(RuleDefinitions.ConcentrationAffinity.Advantage)
            .AddToDB();

        var extraAttack = FeatureDefinitionAttributeModifierBuilder
            .Create("AttributeModifierExtraAttackArcaneFighter", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation(Category.Feature)
            .SetModifier(
                FeatureDefinitionAttributeModifier.AttributeModifierOperation.Additive,
                AttributeDefinitions.AttacksNumber, 1)
            .AddToDB();

        var bonusSpell = FeatureDefinitionAdditionalActionBuilder
            .Create("AdditionalActionArcaneFighter", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation(Category.Feature)
            .SetActionType(ActionDefinitions.ActionType.Main)
            .SetRestrictedActions(ActionDefinitions.Id.CastMain)
            .SetMaxAttacksNumber(-1)
            .SetTriggerCondition(RuleDefinitions.AdditionalActionTriggerCondition.HasDownedAnEnemy)
            .AddToDB();

        var bonusWeaponDamage = FeatureDefinitionAdditionalDamageBuilder
            .Create("AdditionalDamageArcaneFighterBonusWeapon", DefinitionBuilder.CENamespaceGuid)
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
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        Subclass = CharacterSubclassDefinitionBuilder
            .Create("WizardArcaneFighter", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation(Category.Subclass,
                MartialSpellblade.GuiPresentation.SpriteReference)
            .AddFeaturesAtLevel(2, weaponProf, EnchantWeapon)
            .AddFeatureAtLevel(concentrationAffinity, 2)
            .AddFeatureAtLevel(extraAttack, 6)
            .AddFeatureAtLevel(bonusSpell, 10)
            .AddFeatureAtLevel(bonusWeaponDamage, 14)
            .AddToDB();
    }

    private static FeatureDefinitionPower EnchantWeapon => _enchantWeapon ??= BuildEnchantWeapon();

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
            .Create("AttackModifierWeaponArcaneFighter", DefinitionBuilder.CENamespaceGuid)
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
            .Create("PowerWeaponArcaneFighter", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation("AttackModifierWeaponArcaneFighter", Category.Feature,
                FeatureDefinitionPowers.PowerDomainElementalLightningBlade.GuiPresentation.SpriteReference)
            .Configure(0, RuleDefinitions.UsesDetermination.ProficiencyBonus, AttributeDefinitions.Intelligence,
                RuleDefinitions.ActivationTime.BonusAction, 1, RuleDefinitions.RechargeRate.LongRest, false, false,
                AttributeDefinitions.Intelligence, effect, false /* unique instance */)
            .SetCustomSubFeatures(SkipEffectRemovalOnLocationChange.Always)
            .AddToDB();
    }

    internal static void UpdateEnchantWeapon()
    {
        EnchantWeapon.rechargeRate = Main.Settings.EnableShortRestRechargeOfArcaneWeaponOnWizardArcaneFighter
            ? RuleDefinitions.RechargeRate.ShortRest
            : RuleDefinitions.RechargeRate.LongRest;
    }
}
