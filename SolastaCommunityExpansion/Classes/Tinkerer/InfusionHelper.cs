using System;
using System.Collections.Generic;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaCommunityExpansion.Classes.Tinkerer.Subclasses;
using SolastaModApi;
using SolastaModApi.Extensions;
using UnityEngine;
using static SolastaCommunityExpansion.Classes.Tinkerer.FeatureHelpers;
using static SolastaModApi.DatabaseHelper;

namespace SolastaCommunityExpansion.Classes.Tinkerer
{
    internal static class InfusionHelpers
    {
        private static FeatureDefinitionPower artificialServant;
        private static FeatureDefinitionPower enhancedFocus;
        private static FeatureDefinitionPower enhancedDefense;
        private static FeatureDefinitionPower enhancedWeapon;
        private static FeatureDefinitionPower mindSharpener;
        private static FeatureDefinitionPower armorOfMagicalStrength;
        private static FeatureDefinitionPower bagOfHolding;
        private static FeatureDefinitionPower gogglesOfNight;
        private static FeatureDefinitionPower resistantArmor;
        private static FeatureDefinitionPower spellRefuelingRing;
        private static FeatureDefinitionPower blindingWeapon;
        private static FeatureDefinitionPower improvedEnhancedFocus;
        private static FeatureDefinitionPower improvedEnhancedDefense;
        private static FeatureDefinitionPower cloakOfProtection;
        private static FeatureDefinitionPower bootsOfElvenKind;
        private static FeatureDefinitionPower cloakOfElvenKind;
        private static FeatureDefinitionPower bootsOfStridingAndSpringing;
        private static FeatureDefinitionPower bootsOfTheWinterland;
        private static FeatureDefinitionPower bracesrOfArchery;
        private static FeatureDefinitionPower broochOfShielding;
        private static FeatureDefinitionPower gauntletsOfOgrePower;
        private static FeatureDefinitionPower glovesOfMissileSnaring;
        private static FeatureDefinitionPower slippersOfSpiderClimbing;
        private static FeatureDefinitionPower headbandOfIntellect;
        private static FeatureDefinitionPower amuletOfHealth;
        private static FeatureDefinitionPower beltOfGiantHillStrength;
        private static FeatureDefinitionPower bracersOfDefense;
        private static FeatureDefinitionPower cloakOfBat;
        private static FeatureDefinitionPower ringProtectionPlus1;
        private static FeatureDefinitionPower improvedEnhancedWeapon;

        private static FeatureDefinitionPowerSharedPoolBuilder BuildBasicInfusionPower(string name, EffectDescription effect, GuiPresentation presentation)
        {
            return new FeatureDefinitionPowerSharedPoolBuilder(name,
                GuidHelper.Create(TinkererClass.GuidNamespace, name).ToString(),
                TinkererClass.InfusionPool, RuleDefinitions.RechargeRate.LongRest, RuleDefinitions.ActivationTime.NoCost, 1, false, false, AttributeDefinitions.Intelligence,
                effect, presentation, true /* unique instance */);
        }

        public static FeatureDefinitionPower ArtificialServant => artificialServant ??= BuildArtificialServant();

        private static FeatureDefinitionPower BuildArtificialServant()
        {
            GuiPresentationBuilder summonArtificialServantGui = new GuiPresentationBuilder(
                "Feat/&SummonArtificialServantTitle",
                "Feat/&SummonArtificialServantDescription");
            summonArtificialServantGui.SetSpriteReference(SpellDefinitions.ConjureGoblinoids.GuiPresentation.SpriteReference);

            EffectDescriptionBuilder artificialServantEffect = new EffectDescriptionBuilder();
            artificialServantEffect.SetDurationData(RuleDefinitions.DurationType.UntilLongRest, 1, RuleDefinitions.TurnOccurenceType.EndOfTurn);
            artificialServantEffect.SetTargetingData(RuleDefinitions.Side.Ally, RuleDefinitions.RangeType.Distance, 1, RuleDefinitions.TargetType.Position, 1, 1, ActionDefinitions.ItemSelectionType.Equiped);
            artificialServantEffect.AddEffectForm(new EffectFormBuilder().SetSummonForm(SummonForm.Type.Creature, ScriptableObject.CreateInstance<ItemDefinition>(), 1, ArtificialServantBuilder.ArtificialServant.name, ConditionDefinitions.ConditionFlyingBootsWinged, true, null, ScriptableObject.CreateInstance<EffectProxyDefinition>()).Build());

            return BuildBasicInfusionPower("summonArtificialServantPower",
                artificialServantEffect.Build(), summonArtificialServantGui.Build()).AddToDB();
        }

        public static FeatureDefinitionPower EnhancedFocus => enhancedFocus ??= BuildEnhancedFocus();

        private static FeatureDefinitionPower BuildEnhancedFocus()
        {
            GuiPresentationBuilder focusPlus1Gui = new GuiPresentationBuilder(
                "Subclass/&AttackModifierArtificerEnhancedFocusTitle",
                "Subclass/&AttackModifierArtificerEnhancedFocusDescription");
            focusPlus1Gui.SetSpriteReference(FeatureDefinitionAttackModifiers.AttackModifierMagicWeapon.GuiPresentation.SpriteReference);
            FeatureDefinitionMagicAffinity focusPlus1 = BuildMagicAffinityModifiers(1, 1, "Enhanced Focus", focusPlus1Gui.Build());
            ConditionDefinition infusedFocusCondition = BuildCondition(new List<FeatureDefinition>() { focusPlus1 }, RuleDefinitions.DurationType.UntilLongRest, 1, false, "ArtificerInfusedFocus", focusPlus1Gui.Build());

            GuiPresentationBuilder enhanceFocusGui = new GuiPresentationBuilder(
                "Subclass/&AttackModifierArtificerEnhancedFocusTitle",
                "Subclass/&AttackModifierArtificerEnhancedFocusDescription");
            enhanceFocusGui.SetSpriteReference(FeatureDefinitionPowers.PowerDomainElementalLightningBlade.GuiPresentation.SpriteReference);
            return BuildItemConditionInfusion(infusedFocusCondition, "ArtificerInfusionEnhancedFocus", enhanceFocusGui.Build()).AddToDB();
        }

        public static FeatureDefinitionPower ImprovedEnhancedFocus => improvedEnhancedFocus ??= BuildImprovedEnhancedFocus();

        private static FeatureDefinitionPower BuildImprovedEnhancedFocus()
        {
            GuiPresentationBuilder focusPlus2Gui = new GuiPresentationBuilder(
                "Subclass/&AttackModifierArtificerImprovedEnhancedFocusTitle",
                "Subclass/&AttackModifierArtificerImprovedEnhancedFocusDescription");
            focusPlus2Gui.SetSpriteReference(FeatureDefinitionAttackModifiers.AttackModifierMagicWeapon.GuiPresentation.SpriteReference);
            FeatureDefinitionMagicAffinity focusPlus2 = BuildMagicAffinityModifiers(2, 2, "ImprovedEnhancedFocus", focusPlus2Gui.Build());
            ConditionDefinition infusedFocusCondition = BuildCondition(new List<FeatureDefinition>() { focusPlus2 }, RuleDefinitions.DurationType.UntilLongRest, 1, false,
                "ArtificerImprovedInfusedFocus", focusPlus2Gui.Build());

            GuiPresentationBuilder enhanceFocusGui = new GuiPresentationBuilder(
                "Subclass/&AttackModifierArtificerImprovedEnhancedFocusTitle",
                "Subclass/&AttackModifierArtificerImprovedEnhancedFocusDescription");
            enhanceFocusGui.SetSpriteReference(FeatureDefinitionPowers.PowerDomainElementalLightningBlade.GuiPresentation.SpriteReference);
            return BuildItemConditionInfusion(infusedFocusCondition, "ArtificerInfusionImprovedEnhancedFocus", enhanceFocusGui.Build())
                .SetOverriddenPower(EnhancedFocus).AddToDB();
        }

        public static FeatureDefinitionPower EnhancedDefense => enhancedDefense ??= BuildEnhancedDefense();

        private static FeatureDefinitionPower BuildEnhancedDefense()
        {
            GuiPresentationBuilder enhanceArmorConditionGui = new GuiPresentationBuilder(
                "Subclass/&AttackModifierArtificerEnhancedArmorTitle",
                "Subclass/&AttackModifierArtificerEnhancedArmorDescription");
            enhanceArmorConditionGui.SetSpriteReference(ConditionDefinitions.ConditionAuraOfProtection.GuiPresentation.SpriteReference);
            FeatureDefinitionAttributeModifier armorModifier = BuildAttributeModifier(FeatureDefinitionAttributeModifier.AttributeModifierOperation.Additive, AttributeDefinitions.ArmorClass, 1,
                "AttributeModifierArmorInfusion", enhanceArmorConditionGui.Build());

            GuiPresentationBuilder enhanceArmorGui = new GuiPresentationBuilder(
                "Subclass/&AttackModifierArtificerEnhancedArmorTitle",
                "Subclass/&AttackModifierArtificerEnhancedArmorDescription");
            enhanceArmorGui.SetSpriteReference(FeatureDefinitionPowers.PowerPaladinAuraOfProtection.GuiPresentation.SpriteReference);

            return BuildItemModifierInfusion(armorModifier, ActionDefinitions.ItemSelectionType.Equiped, "ArtificerInfusionEnhancedArmor", enhanceArmorGui.Build()).AddToDB();
        }

        public static FeatureDefinitionPower ImprovedEnhancedDefense => improvedEnhancedDefense ??= BuildImprovedEnhancedDefense();

        private static FeatureDefinitionPower BuildImprovedEnhancedDefense()
        {
            GuiPresentationBuilder enhanceArmorConditionGui = new GuiPresentationBuilder(
                "Subclass/&AttackModifierArtificerImprovedEnhancedArmorTitle",
                "Subclass/&AttackModifierArtificerImprovedEnhancedArmorDescription");
            enhanceArmorConditionGui.SetSpriteReference(ConditionDefinitions.ConditionAuraOfProtection.GuiPresentation.SpriteReference);
            FeatureDefinitionAttributeModifier armorModifier = BuildAttributeModifier(FeatureDefinitionAttributeModifier.AttributeModifierOperation.Additive, AttributeDefinitions.ArmorClass, 2,
                "AttributeModifierImprovedArmorInfusion", enhanceArmorConditionGui.Build());

            GuiPresentationBuilder enhanceArmorGui = new GuiPresentationBuilder(
                "Subclass/&AttackModifierArtificerImprovedEnhancedArmorTitle",
                "Subclass/&AttackModifierArtificerImprovedEnhancedArmorDescription");
            enhanceArmorGui.SetSpriteReference(FeatureDefinitionPowers.PowerPaladinAuraOfProtection.GuiPresentation.SpriteReference);

            return BuildItemModifierInfusion(armorModifier, ActionDefinitions.ItemSelectionType.Equiped, "ArtificerInfusionImprovedEnhancedArmor", enhanceArmorGui.Build())
                .SetOverriddenPower(EnhancedDefense).AddToDB();
        }

        public static FeatureDefinitionPower EnhancedWeapon => enhancedWeapon ??= BuildEnhancedWeapon();

        private static FeatureDefinitionPower BuildEnhancedWeapon()
        {
            GuiPresentationBuilder enhanceWeaponGui = new GuiPresentationBuilder(
                "Subclass/&AttackModifierArtificerEnhancedWeaponTitle",
                "Subclass/&AttackModifierArtificerEnhancedWeaponDescription");
            enhanceWeaponGui.SetSpriteReference(FeatureDefinitionPowers.PowerDomainElementalLightningBlade.GuiPresentation.SpriteReference);

            return BuildItemModifierInfusion(FeatureDefinitionAttackModifiers.AttackModifierMagicWeapon,
                ActionDefinitions.ItemSelectionType.WeaponNonMagical, "ArtificerInfusionEnhancedWeapon", enhanceWeaponGui.Build()).AddToDB();
        }

        public static FeatureDefinitionPower ImprovedEnhancedWeapon => improvedEnhancedWeapon ??= BuildImprovedEnhancedWeapon();

        private static FeatureDefinitionPower BuildImprovedEnhancedWeapon()
        {
            GuiPresentationBuilder enhanceWeaponGui = new GuiPresentationBuilder(
                "Subclass/&AttackModifierArtificerImprovedEnhancedWeaponTitle",
                "Subclass/&AttackModifierArtificerImprovedEnhancedWeaponDescription");
            enhanceWeaponGui.SetSpriteReference(FeatureDefinitionPowers.PowerDomainElementalLightningBlade.GuiPresentation.SpriteReference);

            return BuildItemModifierInfusion(FeatureDefinitionAttackModifiers.AttackModifierMagicWeapon2,
                ActionDefinitions.ItemSelectionType.WeaponNonMagical, "ArtificerInfusionImprovedEnhancedWeapon", enhanceWeaponGui.Build())
                .SetOverriddenPower(EnhancedWeapon).AddToDB();
        }

        public static FeatureDefinitionPower BagOfHolding => bagOfHolding ??= BuildBagOfHolding();

        private static FeatureDefinitionPower BuildBagOfHolding()
        {
            GuiPresentationBuilder bagOfHoldingConditionGui = new GuiPresentationBuilder(
                "Subclass/&EquipmentModifierArtificerBagOfHolderTitle",
                "Subclass/&EquipmentModifierArtificerBagOfHolderDescription");
            bagOfHoldingConditionGui.SetSpriteReference(ConditionDefinitions.ConditionBullsStrength.GuiPresentation.SpriteReference);
            ConditionDefinition bagOfHoldingCondition = BuildCondition(new List<FeatureDefinition>() {
                    BuildEquipmentAffinity(1.0f, 500.0f, "InfusionBagOfHolding", bagOfHoldingConditionGui.Build())
                    }, RuleDefinitions.DurationType.UntilLongRest, 1, false, "ArtificerInfusedConditionBagOfHolding", bagOfHoldingConditionGui.Build());

            GuiPresentationBuilder bagOfHoldingGui = new GuiPresentationBuilder(
                "Subclass/&EquipmentModifierArtificerBagOfHolderTitle",
                "Subclass/&EquipmentModifierArtificerBagOfHolderDescription");
            bagOfHoldingGui.SetSpriteReference(FeatureDefinitionPowers.PowerFunctionPotionOfGiantStrengthCloud.GuiPresentation.SpriteReference);
            return BuildItemConditionInfusion(bagOfHoldingCondition, "ArtificerInfusionBagOfHolding", bagOfHoldingGui.Build()).AddToDB();
        }

        public static FeatureDefinitionPower GogglesOfNight => gogglesOfNight ??= BuildGogglesOfNight();

        private static FeatureDefinitionPower BuildGogglesOfNight()
        {
            GuiPresentationBuilder InfuseDarkvisionCondition = new GuiPresentationBuilder(
                "Subclass/&PowerInfuseDarkvisionTitle",
                "Subclass/&PowerInfuseDarkvisionDescription");
            InfuseDarkvisionCondition.SetSpriteReference(ConditionDefinitions.ConditionSeeInvisibility.GuiPresentation.SpriteReference);
            ConditionDefinition darkvisionCondition = BuildCondition(new List<FeatureDefinition>() {
                FeatureDefinitionSenses.SenseSuperiorDarkvision,
                }, RuleDefinitions.DurationType.UntilLongRest, 1, false, "ArtificerInfusedConditionDarkvision", InfuseDarkvisionCondition.Build());

            GuiPresentationBuilder InfuseDarkvision = new GuiPresentationBuilder(
                "Subclass/&PowerInfuseDarkvisionTitle",
                "Subclass/&PowerInfuseDarkvisionDescription");
            InfuseDarkvision.SetSpriteReference(FeatureDefinitionPowers.PowerDomainBattleDivineWrath.GuiPresentation.SpriteReference);

            return BuildItemConditionInfusion(darkvisionCondition, "PowerInfuseDarkvision", InfuseDarkvision.Build()).AddToDB();
        }

        public static FeatureDefinitionPower MindSharpener => mindSharpener ??= BuildMindSharpener();

        private static FeatureDefinitionPower BuildMindSharpener()
        {
            GuiPresentationBuilder InfuseMindSharpenerCondition = new GuiPresentationBuilder(
                "Subclass/&PowerInfuseMindSharpenerTitle",
                "Subclass/&PowerInfuseMindSharpenerDescription");
            InfuseMindSharpenerCondition.SetSpriteReference(ConditionDefinitions.ConditionBearsEndurance.GuiPresentation.SpriteReference);
            ConditionDefinition infusedMindSharpenerCondition = BuildCondition(new List<FeatureDefinition>() {
                BuildMagicAffinityConcentration(RuleDefinitions.ConcentrationAffinity.Advantage, 20, "MagicAffinityMindSharpener", InfuseMindSharpenerCondition.Build()),
                }, RuleDefinitions.DurationType.UntilLongRest, 1, false, "ArtificerInfusedConditionMindSharpener", InfuseMindSharpenerCondition.Build());

            GuiPresentationBuilder InfuseMindSharpener = new GuiPresentationBuilder(
                "Subclass/&PowerInfuseMindSharpenerTitle",
                "Subclass/&PowerInfuseMindSharpenerDescription");
            InfuseMindSharpener.SetSpriteReference(FeatureDefinitionPowers.PowerFunctionTomeOfQuickThought.GuiPresentation.SpriteReference);

            return BuildItemConditionInfusion(infusedMindSharpenerCondition, "ArtificerInfusionMindSharpener", InfuseMindSharpener.Build()).AddToDB();
        }

        public static FeatureDefinitionPower ArmorOfMagicalStrength => armorOfMagicalStrength ??= BuildArmorOfMagicalStrength();

        private static FeatureDefinitionPower BuildArmorOfMagicalStrength()
        {
            GuiPresentationBuilder InfuseArmorMagicalStrengthCondition = new GuiPresentationBuilder(
                "Subclass/&PowerInfuseArmorMagicalStrengthTitle",
                "Subclass/&PowerInfuseArmorMagicalStrengthDescription");
            InfuseArmorMagicalStrengthCondition.SetSpriteReference(ConditionDefinitions.ConditionBullsStrength.GuiPresentation.SpriteReference);
            FeatureDefinitionAbilityCheckAffinity strengthAbilityAffinity = BuildAbilityAffinity(new List<Tuple<string, string>>()
            {
                new Tuple<string, string>(AttributeDefinitions.Strength, ""),
            }, 0, RuleDefinitions.DieType.D1, RuleDefinitions.CharacterAbilityCheckAffinity.Advantage, "AbilityAffinityInfusionMagicalStrength", InfuseArmorMagicalStrengthCondition.Build());
            FeatureDefinitionSavingThrowAffinity strengthSaveAffinity = BuildSavingThrowAffinity(new List<string>() { AttributeDefinitions.Strength },
                RuleDefinitions.CharacterSavingThrowAffinity.Advantage, FeatureDefinitionSavingThrowAffinity.ModifierType.AddDice, 0, RuleDefinitions.DieType.D1, false, "SaveAffinityInfusionMagicalStrength", InfuseArmorMagicalStrengthCondition.Build());
            ConditionDefinition armorMagicalStrengthCondition = BuildCondition(new List<FeatureDefinition>() {
                strengthAbilityAffinity,
                strengthSaveAffinity,
                FeatureDefinitionConditionAffinitys.ConditionAffinityProneImmunity,
                }, RuleDefinitions.DurationType.UntilLongRest, 1, false, "ArtificerInfusionArmorMagicalStrengthCondition", InfuseArmorMagicalStrengthCondition.Build());

            GuiPresentationBuilder InfuseArmorMagicalStrength = new GuiPresentationBuilder(
                "Subclass/&PowerInfuseArmorMagicalStrengthTitle",
                "Subclass/&PowerInfuseArmorMagicalStrengthDescription");
            InfuseArmorMagicalStrength.SetSpriteReference(FeatureDefinitionPowers.PowerFunctionManualGainfulExercise.GuiPresentation.SpriteReference);
            return BuildItemConditionInfusion(armorMagicalStrengthCondition, "ArtificerInfusionArmorMagicalStrength", InfuseArmorMagicalStrength.Build()).AddToDB();
        }

        public static FeatureDefinitionPower ResistantArmor => resistantArmor ??= BuildResistantArmor();

        private static FeatureDefinitionPower BuildResistantArmor()
        {
            GuiPresentationBuilder InfuseResistantArmor = new GuiPresentationBuilder(
                "Subclass/&PowerInfuseResistantArmorTitle",
                "Subclass/&PowerInfuseResistantArmorDescription");
            InfuseResistantArmor.SetSpriteReference(FeatureDefinitionPowers.PowerDomainElementalDiscipleOfTheElementsLightning.GuiPresentation.SpriteReference);
            GuiPresentationBuilder ConditionArmorResistance = new GuiPresentationBuilder(
                "Subclass/&ConditionResistantArmorTitle",
                "Subclass/&ConditionResistnatArmorDescription");
            ConditionArmorResistance.SetSpriteReference(ConditionDefinitions.ConditionAuraOfProtection.GuiPresentation.SpriteReference);
            ConditionDefinition ArmorResistance = BuildCondition(new List<FeatureDefinition>() {
                FeatureDefinitionDamageAffinitys.DamageAffinityAcidResistance,
                FeatureDefinitionDamageAffinitys.DamageAffinityColdResistance,
                FeatureDefinitionDamageAffinitys.DamageAffinityFireResistance,
                FeatureDefinitionDamageAffinitys.DamageAffinityForceDamageResistance,
                FeatureDefinitionDamageAffinitys.DamageAffinityLightningResistance,
                FeatureDefinitionDamageAffinitys.DamageAffinityNecroticResistance,
                FeatureDefinitionDamageAffinitys.DamageAffinityPoisonResistance,
                FeatureDefinitionDamageAffinitys.DamageAffinityPsychicResistance,
                FeatureDefinitionDamageAffinitys.DamageAffinityRadiantResistance,
                FeatureDefinitionDamageAffinitys.DamageAffinityThunderResistance
            },
                RuleDefinitions.DurationType.UntilLongRest, 1, false, "ConditionPowerArtificerResistantArmor", ConditionArmorResistance.Build());
            return BuildItemConditionInfusion(ArmorResistance, "ArtificerInfusionResistantArmor", InfuseResistantArmor.Build()).AddToDB();
        }

        public static FeatureDefinitionPower SpellRefuelingRing => spellRefuelingRing ??= BuildSpellRefuelingRing();

        private static FeatureDefinitionPower BuildSpellRefuelingRing()
        {
            GuiPresentationBuilder InfuseSpellRefuelingRing = new GuiPresentationBuilder(
                "Subclass/&PowerSpellRefuelingRingTitle",
                "Subclass/&PowerSpellRefuelingRingDescription");
            InfuseSpellRefuelingRing.SetSpriteReference(FeatureDefinitionPowers.PowerDomainElementalDiscipleOfTheElementsLightning.GuiPresentation.SpriteReference);

            EffectDescriptionBuilder spellEffect = new EffectDescriptionBuilder();
            spellEffect.SetDurationData(RuleDefinitions.DurationType.UntilLongRest, 1, RuleDefinitions.TurnOccurenceType.EndOfTurn);
            spellEffect.SetTargetingData(RuleDefinitions.Side.Ally, RuleDefinitions.RangeType.Self, 1, RuleDefinitions.TargetType.Self, 1, 1, ActionDefinitions.ItemSelectionType.None);
            spellEffect.AddEffectForm(new EffectFormBuilder().SetSpellForm(9).Build());

            return new FeatureDefinitionPowerSharedPoolBuilder("ArtificerInfusionSpellRefuelingRing",
                GuidHelper.Create(TinkererClass.GuidNamespace, "ArtificerInfusionSpellRefuelingRing").ToString(),
                TinkererClass.InfusionPool, RuleDefinitions.RechargeRate.LongRest, RuleDefinitions.ActivationTime.NoCost, 1, false, false, AttributeDefinitions.Intelligence,
                spellEffect.Build(), InfuseSpellRefuelingRing.Build(), true /* unique instance */).AddToDB();
        }

        public static FeatureDefinitionPower BlindingWeapon => blindingWeapon ??= BuildBlindingWeapon();

        private static FeatureDefinitionPower BuildBlindingWeapon()
        {
            GuiPresentationBuilder radiantWeaponEffectGui = new GuiPresentationBuilder(
                "Subclass/&AttackModifierArtificerBlindingWeaponTitle",
                "Subclass/&AttackModifierArtificerBlindingWeaponDescription");
            radiantWeaponEffectGui.SetSpriteReference(FeatureDefinitionAttackModifiers.AttackModifierMagicWeapon.GuiPresentation.SpriteReference);

            var addBlindingCondition = new ConditionOperationDescription
            {
                Operation = ConditionOperationDescription.ConditionOperation.Add,
                ConditionDefinition = ConditionDefinitions.ConditionBlinded
            };

            addBlindingCondition.SetSaveAffinity(RuleDefinitions.EffectSavingThrowType.Negates);
            addBlindingCondition.SetConditionName(ConditionDefinitions.ConditionBlinded.Name);

            FeatureDefinitionAdditionalDamage radiantDamage = new FeatureHelpers.FeatureDefinitionAdditionalDamageBuilder("AdditionalDamageRadiantWeapon",
                GuidHelper.Create(TinkererClass.GuidNamespace, "AdditionalDamageRadiantWeapon").ToString(), "BlindingWeaponStrike",
                RuleDefinitions.FeatureLimitedUsage.OncePerTurn, RuleDefinitions.AdditionalDamageValueDetermination.Die,
                RuleDefinitions.AdditionalDamageTriggerCondition.AlwaysActive, RuleDefinitions.AdditionalDamageRequiredProperty.None, true, RuleDefinitions.DieType.D4, 1, RuleDefinitions.AdditionalDamageType.Specific,
                "DamageRadiant", RuleDefinitions.AdditionalDamageAdvancement.None, new List<DiceByRank>(), true, AttributeDefinitions.Constitution, 15, RuleDefinitions.EffectSavingThrowType.None,
                new List<ConditionOperationDescription>() {
                    addBlindingCondition,
                }, radiantWeaponEffectGui.Build()).AddToDB();

            GuiPresentationBuilder radiantWeaponGui = new GuiPresentationBuilder(
                "Subclass/&AttackModifierArtificerBlindingWeaponTitle",
                "Subclass/&AttackModifierArtificerBlindingWeaponDescription");
            radiantWeaponGui.SetSpriteReference(FeatureDefinitionPowers.PowerDomainSunIndomitableLight.GuiPresentation.SpriteReference);

            return BuildItemModifierInfusion(radiantDamage,
                ActionDefinitions.ItemSelectionType.Weapon, "ArtificerInfusionBlindingWeapon", radiantWeaponGui.Build()).AddToDB();
        }

        public static FeatureDefinitionPower CloakOfProtection => cloakOfProtection ??= PowerMimicsItem(ItemDefinitions.CloakOfProtection, "InfuseCloakOfProtection");

        public static FeatureDefinitionPower BootsOfElvenKind => bootsOfElvenKind ??= PowerMimicsItem(ItemDefinitions.BootsOfElvenKind, "InfuseBootsOfElvenKind");

        public static FeatureDefinitionPower CloakOfElvenKind => cloakOfElvenKind ??= PowerMimicsItem(ItemDefinitions.CloakOfElvenkind, "InfuseCloakOfElvenKind");

        public static FeatureDefinitionPower BootsOfStridingAndSpringing => bootsOfStridingAndSpringing ??= PowerMimicsItem(ItemDefinitions.BootsOfStridingAndSpringing, "InfuseBootsOfStridingAndSpringing");

        public static FeatureDefinitionPower BootsOfTheWinterland => bootsOfTheWinterland ??= PowerMimicsItem(ItemDefinitions.BootsOfTheWinterland, "InfuseBootsOfTheWinterland");

        public static FeatureDefinitionPower BracesrOfArchery => bracesrOfArchery ??= PowerMimicsItem(ItemDefinitions.Bracers_Of_Archery, "InfuseBracesrOfArchery");

        public static FeatureDefinitionPower BroochOfShielding => broochOfShielding ??= PowerMimicsItem(ItemDefinitions.BroochOfShielding, "InfuseBroochOfShielding");

        public static FeatureDefinitionPower GauntletsOfOgrePower => gauntletsOfOgrePower ??= PowerMimicsItem(ItemDefinitions.GauntletsOfOgrePower, "InfuseGauntletsOfOgrePower");

        public static FeatureDefinitionPower GlovesOfMissileSnaring => glovesOfMissileSnaring ??= PowerMimicsItem(ItemDefinitions.GlovesOfMissileSnaring, "InfuseGlovesOfMissileSnaring");

        public static FeatureDefinitionPower SlippersOfSpiderClimbing => slippersOfSpiderClimbing ??= PowerMimicsItem(ItemDefinitions.SlippersOfSpiderClimbing, "InfuseSlippersOfSpiderClimbing");

        public static FeatureDefinitionPower HeadbandOfIntellect => headbandOfIntellect ??= PowerMimicsItem(ItemDefinitions.HeadbandOfIntellect, "InfuseHeadbandOfIntellect");

        public static FeatureDefinitionPower AmuletOfHealth => amuletOfHealth ??= PowerMimicsItem(ItemDefinitions.AmuletOfHealth, "InfuseAmuletOfHealth");

        public static FeatureDefinitionPower BeltOfGiantHillStrength => beltOfGiantHillStrength ??= PowerMimicsItem(ItemDefinitions.BeltOfGiantHillStrength, "InfuseBeltOfGiantHillStrength");

        public static FeatureDefinitionPower BracersOfDefense => bracersOfDefense ??= PowerMimicsItem(ItemDefinitions.Bracers_Of_Defense, "InfuseBracersOfDefense");

        public static FeatureDefinitionPower CloakOfBat => cloakOfBat ??= PowerMimicsItem(ItemDefinitions.CloakOfBat, "InfuseCloakOfBat");

        public static FeatureDefinitionPower RingProtectionPlus1 => ringProtectionPlus1 ??= PowerMimicsItem(ItemDefinitions.RingProtectionPlus1, "InfuseRingProtectionPlus1");

        public static FeatureDefinitionPowerSharedPoolBuilder BuildItemModifierInfusion(FeatureDefinition itemFeature, ActionDefinitions.ItemSelectionType itemType,
            string name, GuiPresentation gui)
        {
            EffectDescriptionBuilder itemEffect = new EffectDescriptionBuilder();
            itemEffect.SetDurationData(RuleDefinitions.DurationType.UntilLongRest, 1, RuleDefinitions.TurnOccurenceType.EndOfTurn);
            itemEffect.SetTargetingData(RuleDefinitions.Side.Ally, RuleDefinitions.RangeType.Touch, 1, RuleDefinitions.TargetType.Item, 1, 1, itemType);
            itemEffect.AddEffectForm(new EffectFormBuilder().SetItemPropertyForm(new List<FeatureUnlockByLevel>() {
                new FeatureUnlockByLevel(itemFeature, 0),
            }, RuleDefinitions.ItemPropertyUsage.Unlimited, 1).Build());

            return BuildBasicInfusionPower(name, itemEffect.Build(), gui);
        }

        private static FeatureDefinitionPowerSharedPoolBuilder BuildItemConditionInfusion(ConditionDefinition condition, string name, GuiPresentation gui)
        {
            EffectDescriptionBuilder conditionEffect = new EffectDescriptionBuilder();
            conditionEffect.SetDurationData(RuleDefinitions.DurationType.UntilLongRest, 1, RuleDefinitions.TurnOccurenceType.EndOfTurn);
            conditionEffect.SetTargetingData(RuleDefinitions.Side.Ally, RuleDefinitions.RangeType.Touch, 1, RuleDefinitions.TargetType.Individuals, 1, 1, ActionDefinitions.ItemSelectionType.Equiped);
            conditionEffect.AddEffectForm(new EffectFormBuilder().SetConditionForm(condition, ConditionForm.ConditionOperation.Add, false, false, new List<ConditionDefinition>()).Build());
            return BuildBasicInfusionPower(name, conditionEffect.Build(), gui);
        }

        private static FeatureDefinitionPower PowerMimicsItem(ItemDefinition item, string name)
        {
            List<FeatureDefinition> features = new List<FeatureDefinition>();
            foreach (ItemPropertyDescription property in item.StaticProperties)
            {
                features.Add(property.FeatureDefinition);
            }
            ConditionDefinition itemCondition = BuildCondition(features, RuleDefinitions.DurationType.UntilLongRest, 1, false,
                "Condition" + name, item.GuiPresentation);

            EffectDescriptionBuilder itemEffect = new EffectDescriptionBuilder();
            itemEffect.SetDurationData(RuleDefinitions.DurationType.UntilLongRest, 1, RuleDefinitions.TurnOccurenceType.EndOfTurn);
            itemEffect.SetTargetingData(RuleDefinitions.Side.Ally, RuleDefinitions.RangeType.Touch, 1, RuleDefinitions.TargetType.Individuals, 1, 1, ActionDefinitions.ItemSelectionType.Equiped);
            itemEffect.AddEffectForm(new EffectFormBuilder().SetConditionForm(itemCondition, ConditionForm.ConditionOperation.Add, false, false, new List<ConditionDefinition>()).Build());

            return BuildBasicInfusionPower("Power" + name,
                itemEffect.Build(), item.GuiPresentation).AddToDB();
        }
    }
}
