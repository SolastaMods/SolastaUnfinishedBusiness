using System;
using System.Collections.Generic;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
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

            FeatureDefinitionProficiency weaponProf = FeatureDefinitionProficiencyBuilder
                .Create("ProficiencyWeaponArcaneFighter", SubclassNamespace)
                .SetGuiPresentation("WeaponProfArcaneFighter", Category.Subclass)
                .SetProficiencies(RuleDefinitions.ProficiencyType.Weapon, EquipmentDefinitions.SimpleWeaponCategory, EquipmentDefinitions.MartialWeaponCategory)
                .AddToDB();

            FeatureDefinitionMagicAffinity concentrationAffinity = FeatureDefinitionMagicAffinityBuilder
               .Create("MagicAffinityMeleeWizardConcentration", SubclassNamespace)
               .SetGuiPresentation(Category.Subclass)
               .SetConcentrationModifiers(RuleDefinitions.ConcentrationAffinity.Advantage, -1)
               .AddToDB();

            FeatureDefinitionAttributeModifier extraAttack = FeatureDefinitionAttributeModifierBuilder
                .Create("AttributeModifierMeleeWizardExtraAttack", SubclassNamespace)
                .SetGuiPresentation(Category.Subclass)
                .SetModifier(FeatureDefinitionAttributeModifier.AttributeModifierOperation.Additive, AttributeDefinitions.AttacksNumber, 1)
                .AddToDB();

            FeatureDefinitionAdditionalAction bonusSpell = FeatureDefinitionAdditionalActionBuilder
                .Create("ArcaneFighterAdditionalAction", SubclassNamespace)
                .SetGuiPresentation(Category.Subclass)
                .SetActionType(ActionDefinitions.ActionType.Main)
                .SetRestrictedActions(ActionDefinitions.Id.CastMain)
                .SetMaxAttacksNumber(-1)
                .SetTriggerCondition(RuleDefinitions.AdditionalActionTriggerCondition.HasDownedAnEnemy)
                .AddToDB();

            FeatureDefinitionAdditionalDamage bonusWeaponDamage = FeatureDefinitionAdditionalDamageBuilder
                .Create("ArcaneFighterBonusWeaponDamage", SubclassNamespace)
                .Configure(
                    "ArcaneFighterBonusWeaponDamage",
                    RuleDefinitions.FeatureLimitedUsage.OncePerTurn, RuleDefinitions.AdditionalDamageValueDetermination.Die,
                    RuleDefinitions.AdditionalDamageTriggerCondition.AlwaysActive, RuleDefinitions.AdditionalDamageRequiredProperty.None,
                    true /* attack only */, RuleDefinitions.DieType.D8, 1 /* dice number */, RuleDefinitions.AdditionalDamageType.SameAsBaseDamage, "",
                    RuleDefinitions.AdditionalDamageAdvancement.None, new List<DiceByRank>())
                .SetGuiPresentation(Category.Subclass)
                .AddToDB();

            Subclass = CharacterSubclassDefinitionBuilder
                .Create("ArcaneFighter", SubclassNamespace)
                .SetGuiPresentation("TraditionArcaneFighter", Category.Subclass, MartialSpellblade.GuiPresentation.SpriteReference)
                .AddFeaturesAtLevel(2, weaponProf, EnchantWeapon)
                .AddFeatureAtLevel(concentrationAffinity, 2)
                .AddFeatureAtLevel(extraAttack, 6)
                .AddFeatureAtLevel(bonusSpell, 10)
                .AddFeatureAtLevel(bonusWeaponDamage, 14)
                .AddToDB();
        }

        private static FeatureDefinitionPower _enchantWeapon;

        internal static FeatureDefinitionPower EnchantWeapon => _enchantWeapon ??= BuildEnchantWeapon();

        private static FeatureDefinitionPower BuildEnchantWeapon()
        {
            var weaponUseIntModifier = FeatureDefinitionAttackModifierBuilder
                .Create("AttackModifierMeleeWizard", SubclassNamespace)
                .SetGuiPresentation("AttackModifierMeleeWizardArcaneWeapon", Category.Subclass, FeatureDefinitionAttackModifiers.AttackModifierMagicWeapon.GuiPresentation.SpriteReference)
                .SetAbilityScoreReplacement(RuleDefinitions.AbilityScoreReplacement.SpellcastingAbility)
                .SetAdditionalAttackTag(TagsDefinitions.Magical)
                .AddToDB();

            var effect = EffectDescriptionBuilder
                .Create()
                .SetTargetingData(RuleDefinitions.Side.Ally, RuleDefinitions.RangeType.Touch, 1 /* range */, RuleDefinitions.TargetType.Item, 1, 2, ActionDefinitions.ItemSelectionType.Weapon)
                .SetCreatedByCharacter()
                .SetDurationData(RuleDefinitions.DurationType.Minute, 10 /* duration */, RuleDefinitions.TurnOccurenceType.EndOfTurn)
                .AddEffectForm(
                    EffectFormBuilder
                        .Create()
                        .SetItemPropertyForm(RuleDefinitions.ItemPropertyUsage.Unlimited, 0, new FeatureUnlockByLevel(weaponUseIntModifier, 0))
                        .Build()
                    )
                .Build();

            return FeatureDefinitionPowerBuilder
                .Create("PowerMeleeWizardArcaneWeapon", SubclassNamespace)
                .SetGuiPresentation("AttackModifierMeleeWizardArcaneWeapon", Category.Subclass, FeatureDefinitionPowers.PowerDomainElementalLightningBlade.GuiPresentation.SpriteReference)
                .Configure(0, RuleDefinitions.UsesDetermination.ProficiencyBonus, AttributeDefinitions.Intelligence, RuleDefinitions.ActivationTime.BonusAction, 1, RuleDefinitions.RechargeRate.LongRest, false, false,
                    AttributeDefinitions.Intelligence, effect, false /* unique instance */)
                .AddToDB();
        }

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
    }
}
