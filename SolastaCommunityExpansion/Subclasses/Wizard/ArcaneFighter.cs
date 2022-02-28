using System;
using System.Collections.Generic;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaModApi;
using SolastaModApi.Extensions;
using static SolastaModApi.DatabaseHelper;
using static SolastaModApi.DatabaseHelper.CharacterSubclassDefinitions;

namespace SolastaCommunityExpansion.Subclasses.Wizard
{
    internal class ArcaneFighter : AbstractSubclass
    {
        private static Guid SubclassNamespace = new("cab151dd-cc94-4c4c-bfba-a712b9a0b53d");
        private readonly CharacterSubclassDefinition Subclass;

        internal override FeatureDefinitionSubclassChoice GetSubclassChoiceList()
        {
            return FeatureDefinitionSubclassChoices.SubclassChoiceWizardArcaneTraditions;
        }
        internal override CharacterSubclassDefinition GetSubclass()
        {
            return Subclass;
        }

        internal ArcaneFighter()
        {
            // Make Melee Wizard subclass
            CharacterSubclassDefinitionBuilder meleeWizard = CharacterSubclassDefinitionBuilder
                .Create("ArcaneFighter", SubclassNamespace)
                .SetGuiPresentation("TraditionArcaneFighter", Category.Subclass, MartialSpellblade.GuiPresentation.SpriteReference);

            GuiPresentationBuilder weaponProfPresentation = new GuiPresentationBuilder(
                "Subclass/&WeaponProfArcaneFighterTitle",
                "Subclass/&WeaponProfArcaneFighterDescription");
            FeatureDefinitionProficiency weaponProf = BuildProficiency(RuleDefinitions.ProficiencyType.Weapon,
                new List<string>() { EquipmentDefinitions.SimpleWeaponCategory, EquipmentDefinitions.MartialWeaponCategory },
                "ProficiencyWeaponArcaneFighter", weaponProfPresentation.Build());
            meleeWizard.AddFeatureAtLevel(weaponProf, 2);

            meleeWizard.AddFeatureAtLevel(EnchantWeapon, 2);

            GuiPresentationBuilder concentrationAffinityGui = new GuiPresentationBuilder(
                "Subclass/&MagicAffinityMeleeWizardConcentrationTitle",
                "Subclass/&MagicAffinityMeleeWizardConcentrationDescription");
            FeatureDefinitionMagicAffinity concentrationAffinity = BuildMagicAffinityConcentration(RuleDefinitions.ConcentrationAffinity.Advantage, -1,
                "MagicAffinityMeleeWizardConcentration", concentrationAffinityGui.Build());
            meleeWizard.AddFeatureAtLevel(concentrationAffinity, 2);

            GuiPresentationBuilder extraAttackGui = new GuiPresentationBuilder(
                "Subclass/&AttributeModifierMeleeWizardExtraAttackTitle",
                "Subclass/&AttributeModifierMeleeWizardExtraAttackDescription");
            FeatureDefinitionAttributeModifier extraAttack = BuildAttributeModifier(FeatureDefinitionAttributeModifier.AttributeModifierOperation.Additive,
                AttributeDefinitions.AttacksNumber, 1,
                "AttributeModifierMeleeWizardExtraAttack", extraAttackGui.Build());
            meleeWizard.AddFeatureAtLevel(extraAttack, 6);

            FeatureDefinitionAdditionalAction bonusSpell = FeatureDefinitionAdditionalActionBuilder
                .Create("ArcaneFighterAdditionalAction", SubclassNamespace)
                .SetGuiPresentation(Category.Subclass)
                .SetActionType(ActionDefinitions.ActionType.Main)
                .SetRestrictedActions(ActionDefinitions.Id.CastMain)
                .SetMaxAttacksNumber(-1)
                .SetTriggerCondition(RuleDefinitions.AdditionalActionTriggerCondition.HasDownedAnEnemy)
                .AddToDB();

            meleeWizard.AddFeatureAtLevel(bonusSpell, 10);

            GuiPresentationBuilder bonusWeaponDamageGui = new GuiPresentationBuilder(
                "Subclass/&ArcaneFighterBonusWeaponDamageTitle",
                "Subclass/&ArcaneFighterBonusWeaponDamageDescription");
            FeatureDefinitionAdditionalDamage bonusWeaponDamage = new FeatureDefinitionAdditionalDamageBuilder("ArcaneFighterBonusWeaponDamage",
                GuidHelper.Create(SubclassNamespace, "ArcaneFighterBonusWeaponDamage").ToString(),
                "ArcaneFighterBonusWeaponDamage",
                RuleDefinitions.FeatureLimitedUsage.OncePerTurn, RuleDefinitions.AdditionalDamageValueDetermination.Die,
                RuleDefinitions.AdditionalDamageTriggerCondition.AlwaysActive, RuleDefinitions.AdditionalDamageRequiredProperty.None,
                true /* attack only */, RuleDefinitions.DieType.D8, 1 /* dice number */, RuleDefinitions.AdditionalDamageType.SameAsBaseDamage, "",
                RuleDefinitions.AdditionalDamageAdvancement.None, new List<DiceByRank>(), bonusWeaponDamageGui.Build()).AddToDB();
            meleeWizard.AddFeatureAtLevel(bonusWeaponDamage, 14);

            Subclass = meleeWizard.AddToDB();
        }

        private static FeatureDefinitionPower _enchantWeapon;

        private static FeatureDefinitionPower BuildEnchantWeapon()
        {
            GuiPresentationBuilder attackModGui = new GuiPresentationBuilder(
                "Subclass/&AttackModifierMeleeWizardArcaneWeaponTitle",
                "Subclass/&AttackModifierMeleeWizardArcaneWeaponDescription");
            attackModGui.SetSpriteReference(FeatureDefinitionAttackModifiers.AttackModifierMagicWeapon.GuiPresentation.SpriteReference);
            GuiPresentationBuilder arcaneWeaponGui = new GuiPresentationBuilder(
                   "Subclass/&AttackModifierMeleeWizardArcaneWeaponTitle",
                   "Subclass/&AttackModifierMeleeWizardArcaneWeaponDescription");
            arcaneWeaponGui.SetSpriteReference(FeatureDefinitionPowers.PowerDomainElementalLightningBlade.GuiPresentation.SpriteReference);
            FeatureDefinitionAttackModifier weaponUseIntModifier = new FeatureDefinitionAttackModifierBuilder("AttackModifierMeleeWizard",
                 GuidHelper.Create(SubclassNamespace, "AttackModifierMeleeWizard").ToString(),
                 RuleDefinitions.AbilityScoreReplacement.SpellcastingAbility, TagsDefinitions.Magical, attackModGui.Build()).AddToDB();
            return BuildActionItemPower(0 /* fixed uses*/, RuleDefinitions.UsesDetermination.ProficiencyBonus, AttributeDefinitions.Intelligence,
                RuleDefinitions.ActivationTime.BonusAction, 1 /* use cost */, RuleDefinitions.RechargeRate.LongRest, RuleDefinitions.RangeType.Touch, 1 /* range */, ActionDefinitions.ItemSelectionType.Weapon,
                RuleDefinitions.DurationType.Minute, 10 /* duration */, RuleDefinitions.TurnOccurenceType.EndOfTurn,
                weaponUseIntModifier, "PowerMeleeWizardArcaneWeapon", arcaneWeaponGui.Build());
        }
        internal static FeatureDefinitionPower EnchantWeapon => _enchantWeapon ??= BuildEnchantWeapon();

        public static void UpdateEnchantWeapon()
        {
            if (Main.Settings.EnableShortRestRechargeOfArcaneWeaponOnWizardArcaneFighter)
            {
                EnchantWeapon.SetRechargeRate(RuleDefinitions.RechargeRate.ShortRest);
            }
            else
            {
                EnchantWeapon.SetRechargeRate(RuleDefinitions.RechargeRate.LongRest);
            }
        }

        private static FeatureDefinitionProficiency BuildProficiency(RuleDefinitions.ProficiencyType type,
            IEnumerable<string> proficiencies, string name, GuiPresentation guiPresentation)
        {
            return FeatureDefinitionProficiencyBuilder
                .Create(name, SubclassNamespace)
                .SetProficiencies(type, proficiencies)
                .SetGuiPresentation(guiPresentation).AddToDB();
        }

        private static FeatureDefinitionAttributeModifier BuildAttributeModifier(FeatureDefinitionAttributeModifier.AttributeModifierOperation modifierType,
            string attribute, int amount, string name, GuiPresentation guiPresentation)
        {
            return FeatureDefinitionAttributeModifierBuilder
                .Create(name, SubclassNamespace)
                .SetGuiPresentation(guiPresentation)
                .SetModifier(modifierType, attribute, amount)
                .AddToDB();
        }

        public static FeatureDefinitionMagicAffinity BuildMagicAffinityConcentration(RuleDefinitions.ConcentrationAffinity concentrationAffinity, int threshold, string name, GuiPresentation guiPresentation)
        {
            return FeatureDefinitionMagicAffinityBuilder
                .Create(name, SubclassNamespace)
                .SetGuiPresentation(guiPresentation)
                .SetConcentrationModifiers(concentrationAffinity, threshold)
                .AddToDB();
        }

        private static FeatureDefinitionPower BuildActionItemPower(int usesPerRecharge, RuleDefinitions.UsesDetermination usesDetermination,
            string usesAbilityScoreName,
            RuleDefinitions.ActivationTime activationTime, int costPerUse, RuleDefinitions.RechargeRate recharge,
            RuleDefinitions.RangeType rangeType, int rangeParameter, ActionDefinitions.ItemSelectionType itemSelectionType,
            RuleDefinitions.DurationType durationType, int durationParameter, RuleDefinitions.TurnOccurenceType endOfEffect,
            FeatureDefinition itemFeature,
            string name, GuiPresentation guiPresentation)
        {
            EffectDescriptionBuilder effectBuilder = new EffectDescriptionBuilder();
            effectBuilder.SetTargetingData(RuleDefinitions.Side.Ally, rangeType, rangeParameter, RuleDefinitions.TargetType.Item, 1, 2, itemSelectionType);
            effectBuilder.SetCreatedByCharacter();
            effectBuilder.SetDurationData(durationType, durationParameter, endOfEffect);
            effectBuilder.AddEffectForm(new EffectFormBuilder().SetItemPropertyForm(new List<FeatureUnlockByLevel>()
            {
                new FeatureUnlockByLevel(itemFeature, 0),
            }, RuleDefinitions.ItemPropertyUsage.Unlimited, 0).Build());

            return FeatureDefinitionPowerBuilder
                .Create(name, SubclassNamespace)
                .SetGuiPresentation(guiPresentation)
                .Configure(usesPerRecharge, usesDetermination, usesAbilityScoreName, activationTime, costPerUse, recharge, false, false,
                    AttributeDefinitions.Intelligence, effectBuilder.Build(), false /* unique instance */).AddToDB();
        }

        private sealed class FeatureDefinitionAttackModifierBuilder : Builders.Features.FeatureDefinitionAttackModifierBuilder
        {
            public FeatureDefinitionAttackModifierBuilder(string name, string guid,
                RuleDefinitions.AbilityScoreReplacement abilityReplacement, string additionalAttackTag,
                GuiPresentation guiPresentation) : base(name, guid)
            {
                Definition.SetAbilityScoreReplacement(abilityReplacement);
                Definition.SetAdditionalAttackTag(additionalAttackTag);
                Definition.SetGuiPresentation(guiPresentation);
            }
        }
    }
}
