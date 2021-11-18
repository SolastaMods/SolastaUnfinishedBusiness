using SolastaCommunityExpansion.Features;
using SolastaModApi;
using SolastaModApi.BuilderHelpers;
using SolastaModApi.Extensions;
using System;
using System.Collections.Generic;

namespace SolastaCommunityExpansion.Subclasses.Wizard
{
    internal class ArcaneFighter : AbstractSubclass
    {
        private static Guid SubclassNamespace = new Guid("cab151dd-cc94-4c4c-bfba-a712b9a0b53d");
        private readonly CharacterSubclassDefinition Subclass;

        internal override FeatureDefinitionSubclassChoice GetSubclassChoiceList()
        {
            return DatabaseHelper.FeatureDefinitionSubclassChoices.SubclassChoiceWizardArcaneTraditions;
        }
        internal override CharacterSubclassDefinition GetSubclass()
        {
            return Subclass;
        }

        internal ArcaneFighter()
        {
            // Make Melee Wizard subclass
            CharacterSubclassDefinitionBuilder meleeWizard = new CharacterSubclassDefinitionBuilder("ArcaneFighter", GuidHelper.Create(SubclassNamespace, "ArcaneFighter").ToString());
            GuiPresentationBuilder meleePresentation = new GuiPresentationBuilder(
                "Subclass/&TraditionArcaneFighterDescription",
                "Subclass/&TraditionArcaneFighterTitle");
            meleePresentation.SetSpriteReference(DatabaseHelper.CharacterSubclassDefinitions.MartialSpellblade.GuiPresentation.SpriteReference);
            meleeWizard.SetGuiPresentation(meleePresentation.Build());

            GuiPresentationBuilder weaponProfPresentation = new GuiPresentationBuilder(
                "Subclass/&WeaponProfArcaneFighterDescription",
                "Subclass/&WeaponProfArcaneFighterTitle");
            FeatureDefinitionProficiency weaponProf = BuildProficiency(RuleDefinitions.ProficiencyType.Weapon,
                new List<string>() { EquipmentDefinitions.SimpleWeaponCategory, EquipmentDefinitions.MartialWeaponCategory },
                "ProficiencyWeaponArcaneFighter", weaponProfPresentation.Build());
            meleeWizard.AddFeatureAtLevel(weaponProf, 2);

            GuiPresentationBuilder attackModGui = new GuiPresentationBuilder(
                "Subclass/&AttackModifierMeleeWizardArcaneWeaponDescription",
                "Subclass/&AttackModifierMeleeWizardArcaneWeaponTitle");
            attackModGui.SetSpriteReference(DatabaseHelper.FeatureDefinitionAttackModifiers.AttackModifierMagicWeapon.GuiPresentation.SpriteReference);

            GuiPresentationBuilder arcaneWeaponGui = new GuiPresentationBuilder(
                "Subclass/&AttackModifierMeleeWizardArcaneWeaponDescription",
                "Subclass/&AttackModifierMeleeWizardArcaneWeaponTitle");
            arcaneWeaponGui.SetSpriteReference(DatabaseHelper.FeatureDefinitionPowers.PowerDomainElementalLightningBlade.GuiPresentation.SpriteReference);
            FeatureDefinitionAttackModifier weaponUseIntModifier = new FeatureDefinitionAttackModifierBuilder("AttackModifierMeleeWizard",
                 GuidHelper.Create(SubclassNamespace, "AttackModifierMeleeWizard").ToString(),
                 RuleDefinitions.AbilityScoreReplacement.SpellcastingAbility, TagsDefinitions.Magical, attackModGui.Build()).AddToDB();
            FeatureDefinitionPower enchantWeapon = BuildActionItemPower(0 /* fixed uses*/, RuleDefinitions.UsesDetermination.ProficiencyBonus, AttributeDefinitions.Intelligence,
                RuleDefinitions.ActivationTime.BonusAction, 1 /* use cost */, RuleDefinitions.RechargeRate.LongRest, RuleDefinitions.RangeType.Touch, 1 /* range */, ActionDefinitions.ItemSelectionType.Weapon,
                RuleDefinitions.DurationType.Minute, 10 /* duration */, RuleDefinitions.TurnOccurenceType.EndOfTurn,
                weaponUseIntModifier, "PowerMeleeWizardArcaneWeapon", arcaneWeaponGui.Build());
            meleeWizard.AddFeatureAtLevel(enchantWeapon, 2);

            GuiPresentationBuilder concentrationAffinityGui = new GuiPresentationBuilder(
                "Subclass/&MagicAffinityMeleeWizardConcentrationDescription",
                "Subclass/&MagicAffinityMeleeWizardConcentrationTitle");
            FeatureDefinitionMagicAffinity concentrationAffinity = BuildMagicAffinityConcentration(RuleDefinitions.ConcentrationAffinity.Advantage, -1,
                "MagicAffinityMeleeWizardConcentration", concentrationAffinityGui.Build());
            meleeWizard.AddFeatureAtLevel(concentrationAffinity, 2);

            GuiPresentationBuilder extraAttackGui = new GuiPresentationBuilder(
                "Subclass/&AttributeModifierMeleeWizardExtraAttackDescription",
                "Subclass/&AttributeModifierMeleeWizardExtraAttackTitle");
            FeatureDefinitionAttributeModifier extraAttack = BuildAttributeModifier(FeatureDefinitionAttributeModifier.AttributeModifierOperation.Additive,
                AttributeDefinitions.AttacksNumber, 1,
                "AttributeModifierMeleeWizardExtraAttack", extraAttackGui.Build());
            meleeWizard.AddFeatureAtLevel(extraAttack, 6);

            GuiPresentationBuilder bonusSpellGui = new GuiPresentationBuilder(
                "Subclass/&ArcaneFighterAdditionalActionDescription",
                "Subclass/&ArcaneFighterAdditionalActionTitle");
            FeatureDefinitionAdditionalAction bonusSpell = new FeatureDefinitionAdditionalActionBuilder("ArcaneFighterAdditionalAction",
                GuidHelper.Create(SubclassNamespace, "ArcaneFighterAdditionalAction").ToString(), ActionDefinitions.ActionType.Main, new List<ActionDefinitions.Id>(),
                new List<ActionDefinitions.Id>(), new List<ActionDefinitions.Id>()
                {
                    ActionDefinitions.Id.CastMain,
                }, -1, RuleDefinitions.AdditionalActionTriggerCondition.HasDownedAnEnemy,
                bonusSpellGui.Build()).AddToDB();
            meleeWizard.AddFeatureAtLevel(bonusSpell, 10);

            GuiPresentationBuilder bonusWeaponDamageGui = new GuiPresentationBuilder(
                "Subclass/&ArcaneFighterBonusWeaponDamageDescription",
                "Subclass/&ArcaneFighterBonusWeaponDamageTitle");
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

        private static FeatureDefinitionProficiency BuildProficiency(RuleDefinitions.ProficiencyType type,
            List<string> proficiencies, string name, GuiPresentation guiPresentation)
        {
            FeatureDefinitionProficiencyBuilder builder = new FeatureDefinitionProficiencyBuilder(name, GuidHelper.Create(SubclassNamespace, name).ToString(), type, proficiencies, guiPresentation);
            return builder.AddToDB();
        }

        private static FeatureDefinitionAttributeModifier BuildAttributeModifier(FeatureDefinitionAttributeModifier.AttributeModifierOperation modifierType,
            string attribute, int amount, string name, GuiPresentation guiPresentation)
        {
            FeatureDefinitionAttributeModifierBuilder builder = new FeatureDefinitionAttributeModifierBuilder(name, GuidHelper.Create(SubclassNamespace, name).ToString(),
                modifierType, attribute, amount, guiPresentation);
            return builder.AddToDB();
        }

        public static FeatureDefinitionMagicAffinity BuildMagicAffinityConcentration(RuleDefinitions.ConcentrationAffinity concentrationAffinity, int threshold, string name, GuiPresentation guiPresentation)
        {
            FeatureDefinitionMagicAffinityBuilder builder = new FeatureDefinitionMagicAffinityBuilder(name, GuidHelper.Create(SubclassNamespace, name).ToString(),
                guiPresentation).SetConcentrationModifiers(RuleDefinitions.ConcentrationAffinity.Advantage, threshold);
            return builder.AddToDB();
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

            FeatureDefinitionPowerBuilder builder = new FeatureDefinitionPowerBuilder(name, GuidHelper.Create(SubclassNamespace, name).ToString(),
                usesPerRecharge, usesDetermination, usesAbilityScoreName, activationTime, costPerUse, recharge, false, false,
                AttributeDefinitions.Intelligence, effectBuilder.Build(), guiPresentation, false /* unique instance */);
            return builder.AddToDB();
        }

        private class FeatureDefinitionAttackModifierBuilder : BaseDefinitionBuilder<FeatureDefinitionAttackModifier>
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
