using System;
using System.Collections.Generic;
using HarmonyLib;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.CustomFeatureDefinitions;
using SolastaModApi;
using UnityEngine;
using static SolastaCommunityExpansion.Classes.Tinkerer.FeatureHelpers;

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
            FeatureDefinitionPowerSharedPoolBuilder infusion = new FeatureDefinitionPowerSharedPoolBuilder(name,
                GuidHelper.Create(TinkererClass.GuidNamespace, name).ToString(),
                TinkererClass.InfusionPool, RuleDefinitions.RechargeRate.LongRest, RuleDefinitions.ActivationTime.NoCost, 1, false, false, AttributeDefinitions.Intelligence,
                effect, presentation, true /* unique instance */);
            return infusion;
        }

        public static FeatureDefinitionPower ArtificialServant => artificialServant ?? (artificialServant = BuildArtificialServant());

        private static FeatureDefinitionPower BuildArtificialServant()
        {
            GuiPresentationBuilder summonArtificialServantGui = new GuiPresentationBuilder(
                "Feat/&SummonArtificialServantDescription",
                "Feat/&SummonArtificialServantTitle");
            summonArtificialServantGui.SetSpriteReference(DatabaseHelper.SpellDefinitions.ConjureGoblinoids.GuiPresentation.SpriteReference);

            EffectDescriptionBuilder artificialServantEffect = new EffectDescriptionBuilder();
            artificialServantEffect.SetDurationData(RuleDefinitions.DurationType.UntilLongRest, 1, RuleDefinitions.TurnOccurenceType.EndOfTurn);
            artificialServantEffect.SetTargetingData(RuleDefinitions.Side.Ally, RuleDefinitions.RangeType.Distance, 1, RuleDefinitions.TargetType.Position, 1, 1, ActionDefinitions.ItemSelectionType.Equiped);
            artificialServantEffect.AddEffectForm(new EffectFormBuilder().SetSummonForm(SummonForm.Type.Creature, ScriptableObject.CreateInstance<ItemDefinition>(), 1, ArtificialServantBuilder.ArtificialServant.name, DatabaseHelper.ConditionDefinitions.ConditionFlyingBootsWinged, true, null, ScriptableObject.CreateInstance<EffectProxyDefinition>()).Build());

            FeatureDefinitionPowerSharedPool summonArtificialServantPower = BuildBasicInfusionPower("summonArtificialServantPower",
                artificialServantEffect.Build(), summonArtificialServantGui.Build()).AddToDB();
            return summonArtificialServantPower;
        }

        public static FeatureDefinitionPower EnhancedFocus => enhancedFocus ?? (enhancedFocus = BuildEnhancedFocus());

        private static FeatureDefinitionPower BuildEnhancedFocus()
        {
            GuiPresentationBuilder focusPlus1Gui = new GuiPresentationBuilder(
                "Subclass/&AttackModifierArtificerEnhancedFocusDescription",
                "Subclass/&AttackModifierArtificerEnhancedFocusTitle");
            focusPlus1Gui.SetSpriteReference(DatabaseHelper.FeatureDefinitionAttackModifiers.AttackModifierMagicWeapon.GuiPresentation.SpriteReference);
            FeatureDefinitionMagicAffinity focusPlus1 = BuildMagicAffinityModifiers(1, 1, "Enhanced Focus", focusPlus1Gui.Build());
            ConditionDefinition infusedFocusCondition = BuildCondition(new List<FeatureDefinition>() { focusPlus1 }, RuleDefinitions.DurationType.UntilLongRest, 1, false, "ArtificerInfusedFocus", focusPlus1Gui.Build());

            GuiPresentationBuilder enhanceFocusGui = new GuiPresentationBuilder(
                "Subclass/&AttackModifierArtificerEnhancedFocusDescription",
                "Subclass/&AttackModifierArtificerEnhancedFocusTitle");
            enhanceFocusGui.SetSpriteReference(DatabaseHelper.FeatureDefinitionPowers.PowerDomainElementalLightningBlade.GuiPresentation.SpriteReference);
            return BuildItemConditionInfusion(infusedFocusCondition, "ArtificerInfusionEnhancedFocus", enhanceFocusGui.Build()).AddToDB();
        }

        public static FeatureDefinitionPower ImprovedEnhancedFocus => improvedEnhancedFocus ?? (improvedEnhancedFocus = BuildImprovedEnhancedFocus());

        private static FeatureDefinitionPower BuildImprovedEnhancedFocus()
        {
            GuiPresentationBuilder focusPlus2Gui = new GuiPresentationBuilder(
                "Subclass/&AttackModifierArtificerImprovedEnhancedFocusDescription",
                "Subclass/&AttackModifierArtificerImprovedEnhancedFocusTitle");
            focusPlus2Gui.SetSpriteReference(DatabaseHelper.FeatureDefinitionAttackModifiers.AttackModifierMagicWeapon.GuiPresentation.SpriteReference);
            FeatureDefinitionMagicAffinity focusPlus2 = BuildMagicAffinityModifiers(2, 2, "ImprovedEnhancedFocus", focusPlus2Gui.Build());
            ConditionDefinition infusedFocusCondition = BuildCondition(new List<FeatureDefinition>() { focusPlus2 }, RuleDefinitions.DurationType.UntilLongRest, 1, false,
                "ArtificerImprovedInfusedFocus", focusPlus2Gui.Build());

            GuiPresentationBuilder enhanceFocusGui = new GuiPresentationBuilder(
                "Subclass/&AttackModifierArtificerImprovedEnhancedFocusDescription",
                "Subclass/&AttackModifierArtificerImprovedEnhancedFocusTitle");
            enhanceFocusGui.SetSpriteReference(DatabaseHelper.FeatureDefinitionPowers.PowerDomainElementalLightningBlade.GuiPresentation.SpriteReference);
            return BuildItemConditionInfusion(infusedFocusCondition, "ArtificerInfusionImprovedEnhancedFocus", enhanceFocusGui.Build())
                .AddOverriddenPower(EnhancedFocus).AddToDB();
        }

        public static FeatureDefinitionPower EnhancedDefense => enhancedDefense ?? (enhancedDefense = BuildEnhancedDefense());

        private static FeatureDefinitionPower BuildEnhancedDefense()
        {
            GuiPresentationBuilder enhanceArmorConditionGui = new GuiPresentationBuilder(
                "Subclass/&AttackModifierArtificerEnhancedArmorDescription",
                "Subclass/&AttackModifierArtificerEnhancedArmorTitle");
            enhanceArmorConditionGui.SetSpriteReference(DatabaseHelper.ConditionDefinitions.ConditionAuraOfProtection.GuiPresentation.SpriteReference);
            FeatureDefinitionAttributeModifier armorModifier = BuildAttributeModifier(FeatureDefinitionAttributeModifier.AttributeModifierOperation.Additive, AttributeDefinitions.ArmorClass, 1,
                "AttributeModifierArmorInfusion", enhanceArmorConditionGui.Build());

            GuiPresentationBuilder enhanceArmorGui = new GuiPresentationBuilder(
                "Subclass/&AttackModifierArtificerEnhancedArmorDescription",
                "Subclass/&AttackModifierArtificerEnhancedArmorTitle");
            enhanceArmorGui.SetSpriteReference(DatabaseHelper.FeatureDefinitionPowers.PowerPaladinAuraOfProtection.GuiPresentation.SpriteReference);

            return BuildItemModifierInfusion(armorModifier, ActionDefinitions.ItemSelectionType.Equiped, "ArtificerInfusionEnhancedArmor", enhanceArmorGui.Build()).AddToDB();
        }

        public static FeatureDefinitionPower ImprovedEnhancedDefense => improvedEnhancedDefense ?? (improvedEnhancedDefense = BuildImprovedEnhancedDefense());

        private static FeatureDefinitionPower BuildImprovedEnhancedDefense()
        {
            GuiPresentationBuilder enhanceArmorConditionGui = new GuiPresentationBuilder(
                "Subclass/&AttackModifierArtificerImprovedEnhancedArmorDescription",
                "Subclass/&AttackModifierArtificerImprovedEnhancedArmorTitle");
            enhanceArmorConditionGui.SetSpriteReference(DatabaseHelper.ConditionDefinitions.ConditionAuraOfProtection.GuiPresentation.SpriteReference);
            FeatureDefinitionAttributeModifier armorModifier = BuildAttributeModifier(FeatureDefinitionAttributeModifier.AttributeModifierOperation.Additive, AttributeDefinitions.ArmorClass, 2,
                "AttributeModifierImprovedArmorInfusion", enhanceArmorConditionGui.Build());

            GuiPresentationBuilder enhanceArmorGui = new GuiPresentationBuilder(
                "Subclass/&AttackModifierArtificerImprovedEnhancedArmorDescription",
                "Subclass/&AttackModifierArtificerImprovedEnhancedArmorTitle");
            enhanceArmorGui.SetSpriteReference(DatabaseHelper.FeatureDefinitionPowers.PowerPaladinAuraOfProtection.GuiPresentation.SpriteReference);

            return BuildItemModifierInfusion(armorModifier, ActionDefinitions.ItemSelectionType.Equiped, "ArtificerInfusionImprovedEnhancedArmor", enhanceArmorGui.Build())
                .AddOverriddenPower(EnhancedDefense).AddToDB();
        }

        public static FeatureDefinitionPower EnhancedWeapon => enhancedWeapon ?? (enhancedWeapon = BuildEnhancedWeapon());

        private static FeatureDefinitionPower BuildEnhancedWeapon()
        {
            GuiPresentationBuilder enhanceWeaponGui = new GuiPresentationBuilder(
                "Subclass/&AttackModifierArtificerEnhancedWeaponDescription",
                "Subclass/&AttackModifierArtificerEnhancedWeaponTitle");
            enhanceWeaponGui.SetSpriteReference(DatabaseHelper.FeatureDefinitionPowers.PowerDomainElementalLightningBlade.GuiPresentation.SpriteReference);

            return BuildItemModifierInfusion(DatabaseHelper.FeatureDefinitionAttackModifiers.AttackModifierMagicWeapon,
                ActionDefinitions.ItemSelectionType.WeaponNonMagical, "ArtificerInfusionEnhancedWeapon", enhanceWeaponGui.Build()).AddToDB();
        }

        public static FeatureDefinitionPower ImprovedEnhancedWeapon => improvedEnhancedWeapon ?? (improvedEnhancedWeapon = BuildImprovedEnhancedWeapon());

        private static FeatureDefinitionPower BuildImprovedEnhancedWeapon()
        {
            GuiPresentationBuilder enhanceWeaponGui = new GuiPresentationBuilder(
                "Subclass/&AttackModifierArtificerImprovedEnhancedWeaponDescription",
                "Subclass/&AttackModifierArtificerImprovedEnhancedWeaponTitle");
            enhanceWeaponGui.SetSpriteReference(DatabaseHelper.FeatureDefinitionPowers.PowerDomainElementalLightningBlade.GuiPresentation.SpriteReference);

            return BuildItemModifierInfusion(DatabaseHelper.FeatureDefinitionAttackModifiers.AttackModifierMagicWeapon2,
                ActionDefinitions.ItemSelectionType.WeaponNonMagical, "ArtificerInfusionImprovedEnhancedWeapon", enhanceWeaponGui.Build())
                .AddOverriddenPower(EnhancedWeapon).AddToDB();
        }

        public static FeatureDefinitionPower BagOfHolding => bagOfHolding ?? (bagOfHolding = BuildBagOfHolding());

        private static FeatureDefinitionPower BuildBagOfHolding()
        {
            GuiPresentationBuilder bagOfHoldingConditionGui = new GuiPresentationBuilder(
                "Subclass/&EquipmentModifierArtificerBagOfHolderDescription",
                "Subclass/&EquipmentModifierArtificerBagOfHolderTitle");
            bagOfHoldingConditionGui.SetSpriteReference(DatabaseHelper.ConditionDefinitions.ConditionBullsStrength.GuiPresentation.SpriteReference);
            ConditionDefinition bagOfHoldingCondition = BuildCondition(new List<FeatureDefinition>() {
                    BuildEquipmentAffinity(1.0f, 500.0f, "InfusionBagOfHolding", bagOfHoldingConditionGui.Build())
                    }, RuleDefinitions.DurationType.UntilLongRest, 1, false, "ArtificerInfusedConditionBagOfHolding", bagOfHoldingConditionGui.Build());

            GuiPresentationBuilder bagOfHoldingGui = new GuiPresentationBuilder(
                "Subclass/&EquipmentModifierArtificerBagOfHolderDescription",
                "Subclass/&EquipmentModifierArtificerBagOfHolderTitle");
            bagOfHoldingGui.SetSpriteReference(DatabaseHelper.FeatureDefinitionPowers.PowerFunctionPotionOfGiantStrengthCloud.GuiPresentation.SpriteReference);
            return BuildItemConditionInfusion(bagOfHoldingCondition, "ArtificerInfusionBagOfHolding", bagOfHoldingGui.Build()).AddToDB();
        }

        public static FeatureDefinitionPower GogglesOfNight => gogglesOfNight ?? (gogglesOfNight = BuildGogglesOfNight());

        private static FeatureDefinitionPower BuildGogglesOfNight()
        {
            GuiPresentationBuilder InfuseDarkvisionCondition = new GuiPresentationBuilder(
                "Subclass/&PowerInfuseDarkvisionDescription",
                "Subclass/&PowerInfuseDarkvisionTitle");
            InfuseDarkvisionCondition.SetSpriteReference(DatabaseHelper.ConditionDefinitions.ConditionSeeInvisibility.GuiPresentation.SpriteReference);
            ConditionDefinition darkvisionCondition = BuildCondition(new List<FeatureDefinition>() {
                DatabaseHelper.FeatureDefinitionSenses.SenseSuperiorDarkvision,
                }, RuleDefinitions.DurationType.UntilLongRest, 1, false, "ArtificerInfusedConditionDarkvision", InfuseDarkvisionCondition.Build());

            GuiPresentationBuilder InfuseDarkvision = new GuiPresentationBuilder(
                "Subclass/&PowerInfuseDarkvisionDescription",
                "Subclass/&PowerInfuseDarkvisionTitle");
            InfuseDarkvision.SetSpriteReference(DatabaseHelper.FeatureDefinitionPowers.PowerDomainBattleDivineWrath.GuiPresentation.SpriteReference);

            return BuildItemConditionInfusion(darkvisionCondition, "PowerInfuseDarkvision", InfuseDarkvision.Build()).AddToDB();
        }

        public static FeatureDefinitionPower MindSharpener => mindSharpener ?? (mindSharpener = BuildMindSharpener());

        private static FeatureDefinitionPower BuildMindSharpener()
        {
            GuiPresentationBuilder InfuseMindSharpenerCondition = new GuiPresentationBuilder(
                "Subclass/&PowerInfuseMindSharpenerDescription",
                "Subclass/&PowerInfuseMindSharpenerTitle");
            InfuseMindSharpenerCondition.SetSpriteReference(DatabaseHelper.ConditionDefinitions.ConditionBearsEndurance.GuiPresentation.SpriteReference);
            ConditionDefinition infusedMindSharpenerCondition = BuildCondition(new List<FeatureDefinition>() {
                BuildMagicAffinityConcentration(RuleDefinitions.ConcentrationAffinity.Advantage, 20, "MagicAffinityMindSharpener", InfuseMindSharpenerCondition.Build()),
                }, RuleDefinitions.DurationType.UntilLongRest, 1, false, "ArtificerInfusedConditionMindSharpener", InfuseMindSharpenerCondition.Build());

            GuiPresentationBuilder InfuseMindSharpener = new GuiPresentationBuilder(
                "Subclass/&PowerInfuseMindSharpenerDescription",
                "Subclass/&PowerInfuseMindSharpenerTitle");
            InfuseMindSharpener.SetSpriteReference(DatabaseHelper.FeatureDefinitionPowers.PowerFunctionTomeOfQuickThought.GuiPresentation.SpriteReference);

            return BuildItemConditionInfusion(infusedMindSharpenerCondition, "ArtificerInfusionMindSharpener", InfuseMindSharpener.Build()).AddToDB();
        }

        public static FeatureDefinitionPower ArmorOfMagicalStrength => armorOfMagicalStrength ?? (armorOfMagicalStrength = BuildArmorOfMagicalStrength());

        private static FeatureDefinitionPower BuildArmorOfMagicalStrength()
        {
            GuiPresentationBuilder InfuseArmorMagicalStrengthCondition = new GuiPresentationBuilder(
                "Subclass/&PowerInfuseArmorMagicalStrengthDescription",
                "Subclass/&PowerInfuseArmorMagicalStrengthTitle");
            InfuseArmorMagicalStrengthCondition.SetSpriteReference(DatabaseHelper.ConditionDefinitions.ConditionBullsStrength.GuiPresentation.SpriteReference);
            FeatureDefinitionAbilityCheckAffinity strengthAbilityAffinity = BuildAbilityAffinity(new List<Tuple<string, string>>()
            {
                new Tuple<string, string>(AttributeDefinitions.Strength, ""),
            }, 0, RuleDefinitions.DieType.D1, RuleDefinitions.CharacterAbilityCheckAffinity.Advantage, "AbilityAffinityInfusionMagicalStrength", InfuseArmorMagicalStrengthCondition.Build());
            FeatureDefinitionSavingThrowAffinity strengthSaveAffinity = BuildSavingThrowAffinity(new List<string>() { AttributeDefinitions.Strength },
                RuleDefinitions.CharacterSavingThrowAffinity.Advantage, FeatureDefinitionSavingThrowAffinity.ModifierType.AddDice, 0, RuleDefinitions.DieType.D1, false, "SaveAffinityInfusionMagicalStrength", InfuseArmorMagicalStrengthCondition.Build());
            ConditionDefinition armorMagicalStrengthCondition = BuildCondition(new List<FeatureDefinition>() {
                strengthAbilityAffinity,
                strengthSaveAffinity,
                DatabaseHelper.FeatureDefinitionConditionAffinitys.ConditionAffinityProneImmunity,
                }, RuleDefinitions.DurationType.UntilLongRest, 1, false, "ArtificerInfusionArmorMagicalStrengthCondition", InfuseArmorMagicalStrengthCondition.Build());

            GuiPresentationBuilder InfuseArmorMagicalStrength = new GuiPresentationBuilder(
                "Subclass/&PowerInfuseArmorMagicalStrengthDescription",
                "Subclass/&PowerInfuseArmorMagicalStrengthTitle");
            InfuseArmorMagicalStrength.SetSpriteReference(DatabaseHelper.FeatureDefinitionPowers.PowerFunctionManualGainfulExercise.GuiPresentation.SpriteReference);
            return BuildItemConditionInfusion(armorMagicalStrengthCondition, "ArtificerInfusionArmorMagicalStrength", InfuseArmorMagicalStrength.Build()).AddToDB();
        }

        public static FeatureDefinitionPower ResistantArmor => resistantArmor ?? (resistantArmor = BuildResistantArmor());

        private static FeatureDefinitionPower BuildResistantArmor()
        {
            GuiPresentationBuilder InfuseResistantArmor = new GuiPresentationBuilder(
                "Subclass/&PowerInfuseResistantArmorDescription",
                "Subclass/&PowerInfuseResistantArmorTitle");
            InfuseResistantArmor.SetSpriteReference(DatabaseHelper.FeatureDefinitionPowers.PowerDomainElementalDiscipleOfTheElementsLightning.GuiPresentation.SpriteReference);
            GuiPresentationBuilder ConditionArmorResistance = new GuiPresentationBuilder(
                "Subclass/&ConditionResistnatArmorDescription",
                "Subclass/&ConditionResistantArmorTitle");
            ConditionArmorResistance.SetSpriteReference(DatabaseHelper.ConditionDefinitions.ConditionAuraOfProtection.GuiPresentation.SpriteReference);
            ConditionDefinition ArmorResistance = BuildCondition(new List<FeatureDefinition>() {
                DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityAcidResistance,
                DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityColdResistance,
                DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityFireResistance,
                DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityForceDamageResistance,
                DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityLightningResistance,
                DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityNecroticResistance,
                DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityPoisonResistance,
                DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityPsychicResistance,
                DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityRadiantResistance,
                DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityThunderResistance,
            },
                RuleDefinitions.DurationType.UntilLongRest, 1, false, "ConditionPowerArtificerResistantArmor", ConditionArmorResistance.Build());
            return BuildItemConditionInfusion(ArmorResistance, "ArtificerInfusionResistantArmor", InfuseResistantArmor.Build()).AddToDB();
        }

        public static FeatureDefinitionPower SpellRefuelingRing => spellRefuelingRing ?? (spellRefuelingRing = BuildSpellRefuelingRing());

        private static FeatureDefinitionPower BuildSpellRefuelingRing()
        {
            GuiPresentationBuilder InfuseSpellRefuelingRing = new GuiPresentationBuilder(
                "Subclass/&PowerSpellRefuelingRingDescription",
                "Subclass/&PowerSpellRefuelingRingTitle");
            InfuseSpellRefuelingRing.SetSpriteReference(DatabaseHelper.FeatureDefinitionPowers.PowerDomainElementalDiscipleOfTheElementsLightning.GuiPresentation.SpriteReference);

            EffectDescriptionBuilder spellEffect = new EffectDescriptionBuilder();
            spellEffect.SetDurationData(RuleDefinitions.DurationType.UntilLongRest, 1, RuleDefinitions.TurnOccurenceType.EndOfTurn);
            spellEffect.SetTargetingData(RuleDefinitions.Side.Ally, RuleDefinitions.RangeType.Self, 1, RuleDefinitions.TargetType.Self, 1, 1, ActionDefinitions.ItemSelectionType.None);
            spellEffect.AddEffectForm(new EffectFormBuilder().SetSpellForm(9).Build());

            return new FeatureDefinitionPowerSharedPoolBuilder("ArtificerInfusionSpellRefuelingRing",
                GuidHelper.Create(TinkererClass.GuidNamespace, "ArtificerInfusionSpellRefuelingRing").ToString(),
                TinkererClass.InfusionPool, RuleDefinitions.RechargeRate.LongRest, RuleDefinitions.ActivationTime.NoCost, 1, false, false, AttributeDefinitions.Intelligence,
                spellEffect.Build(), InfuseSpellRefuelingRing.Build(), true /* unique instance */).AddToDB();
        }

        public static FeatureDefinitionPower BlindingWeapon => blindingWeapon ?? (blindingWeapon = BuildBlindingWeapon());

        private static FeatureDefinitionPower BuildBlindingWeapon()
        {
            GuiPresentationBuilder radiantWeaponEffectGui = new GuiPresentationBuilder(
                "Subclass/&AttackModifierArtificerBlindingWeaponDescription",
                "Subclass/&AttackModifierArtificerBlindingWeaponTitle");
            radiantWeaponEffectGui.SetSpriteReference(DatabaseHelper.FeatureDefinitionAttackModifiers.AttackModifierMagicWeapon.GuiPresentation.SpriteReference);

            ConditionOperationDescription addBlindingCondition = new ConditionOperationDescription();
            Traverse.Create(addBlindingCondition).Field("operation").SetValue(ConditionOperationDescription.ConditionOperation.Add);
            Traverse.Create(addBlindingCondition).Field("conditionName").SetValue(DatabaseHelper.ConditionDefinitions.ConditionBlinded.Name);
            Traverse.Create(addBlindingCondition).Field("conditionDefinition").SetValue(DatabaseHelper.ConditionDefinitions.ConditionBlinded);
            Traverse.Create(addBlindingCondition).Field("saveAffinity").SetValue(RuleDefinitions.EffectSavingThrowType.Negates);

            FeatureDefinitionAdditionalDamage radiantDamage = new FeatureDefinitionAdditionalDamageBuilder("AdditionalDamageRadiantWeapon",
                GuidHelper.Create(TinkererClass.GuidNamespace, "AdditionalDamageRadiantWeapon").ToString(), "BlindingWeaponStrike",
                RuleDefinitions.FeatureLimitedUsage.OncePerTurn, RuleDefinitions.AdditionalDamageValueDetermination.Die,
                RuleDefinitions.AdditionalDamageTriggerCondition.AlwaysActive, RuleDefinitions.AdditionalDamageRequiredProperty.None, true, RuleDefinitions.DieType.D4, 1, RuleDefinitions.AdditionalDamageType.Specific,
                "DamageRadiant", RuleDefinitions.AdditionalDamageAdvancement.None, new List<DiceByRank>(), true, AttributeDefinitions.Constitution, 15, RuleDefinitions.EffectSavingThrowType.None,
                new List<ConditionOperationDescription>() {
                    addBlindingCondition,
                }, radiantWeaponEffectGui.Build()).AddToDB();

            GuiPresentationBuilder radiantWeaponGui = new GuiPresentationBuilder(
                "Subclass/&AttackModifierArtificerBlindingWeaponDescription",
                "Subclass/&AttackModifierArtificerBlindingWeaponTitle");
            radiantWeaponGui.SetSpriteReference(DatabaseHelper.FeatureDefinitionPowers.PowerDomainSunIndomitableLight.GuiPresentation.SpriteReference);

            return BuildItemModifierInfusion(radiantDamage,
                ActionDefinitions.ItemSelectionType.Weapon, "ArtificerInfusionBlindingWeapon", radiantWeaponGui.Build()).AddToDB();
        }

        public static FeatureDefinitionPower CloakOfProtection => cloakOfProtection ?? (cloakOfProtection = PowerMimicsItem(DatabaseHelper.ItemDefinitions.CloakOfProtection, "InfuseCloakOfProtection"));

        public static FeatureDefinitionPower BootsOfElvenKind => bootsOfElvenKind ?? (bootsOfElvenKind = PowerMimicsItem(DatabaseHelper.ItemDefinitions.BootsOfElvenKind, "InfuseBootsOfElvenKind"));

        public static FeatureDefinitionPower CloakOfElvenKind => cloakOfElvenKind ?? (cloakOfElvenKind = PowerMimicsItem(DatabaseHelper.ItemDefinitions.CloakOfElvenkind, "InfuseCloakOfElvenKind"));

        public static FeatureDefinitionPower BootsOfStridingAndSpringing => bootsOfStridingAndSpringing ?? (bootsOfStridingAndSpringing = PowerMimicsItem(DatabaseHelper.ItemDefinitions.BootsOfStridingAndSpringing, "InfuseBootsOfStridingAndSpringing"));

        public static FeatureDefinitionPower BootsOfTheWinterland => bootsOfTheWinterland ?? (bootsOfTheWinterland = PowerMimicsItem(DatabaseHelper.ItemDefinitions.BootsOfTheWinterland, "InfuseBootsOfTheWinterland"));

        public static FeatureDefinitionPower BracesrOfArchery => bracesrOfArchery ?? (bracesrOfArchery = PowerMimicsItem(DatabaseHelper.ItemDefinitions.Bracers_Of_Archery, "InfuseBracesrOfArchery"));

        public static FeatureDefinitionPower BroochOfShielding => broochOfShielding ?? (broochOfShielding = PowerMimicsItem(DatabaseHelper.ItemDefinitions.BroochOfShielding, "InfuseBroochOfShielding"));

        public static FeatureDefinitionPower GauntletsOfOgrePower => gauntletsOfOgrePower ?? (gauntletsOfOgrePower = PowerMimicsItem(DatabaseHelper.ItemDefinitions.GauntletsOfOgrePower, "InfuseGauntletsOfOgrePower"));

        public static FeatureDefinitionPower GlovesOfMissileSnaring => glovesOfMissileSnaring ?? (glovesOfMissileSnaring = PowerMimicsItem(DatabaseHelper.ItemDefinitions.GlovesOfMissileSnaring, "InfuseGlovesOfMissileSnaring"));

        public static FeatureDefinitionPower SlippersOfSpiderClimbing => slippersOfSpiderClimbing ?? (slippersOfSpiderClimbing = PowerMimicsItem(DatabaseHelper.ItemDefinitions.SlippersOfSpiderClimbing, "InfuseSlippersOfSpiderClimbing"));

        public static FeatureDefinitionPower HeadbandOfIntellect => headbandOfIntellect ?? (headbandOfIntellect = PowerMimicsItem(DatabaseHelper.ItemDefinitions.HeadbandOfIntellect, "InfuseHeadbandOfIntellect"));

        public static FeatureDefinitionPower AmuletOfHealth => amuletOfHealth ?? (amuletOfHealth = PowerMimicsItem(DatabaseHelper.ItemDefinitions.AmuletOfHealth, "InfuseAmuletOfHealth"));

        public static FeatureDefinitionPower BeltOfGiantHillStrength => beltOfGiantHillStrength ?? (beltOfGiantHillStrength = PowerMimicsItem(DatabaseHelper.ItemDefinitions.BeltOfGiantHillStrength, "InfuseBeltOfGiantHillStrength"));

        public static FeatureDefinitionPower BracersOfDefense => bracersOfDefense ?? (bracersOfDefense = PowerMimicsItem(DatabaseHelper.ItemDefinitions.Bracers_Of_Defense, "InfuseBracersOfDefense"));

        public static FeatureDefinitionPower CloakOfBat => cloakOfBat ?? (cloakOfBat = PowerMimicsItem(DatabaseHelper.ItemDefinitions.CloakOfBat, "InfuseCloakOfBat"));

        public static FeatureDefinitionPower RingProtectionPlus1 => ringProtectionPlus1 ?? (ringProtectionPlus1 = PowerMimicsItem(DatabaseHelper.ItemDefinitions.RingProtectionPlus1, "InfuseRingProtectionPlus1"));

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

            FeatureDefinitionPowerSharedPool powerItem = BuildBasicInfusionPower("Power" + name,
                itemEffect.Build(), item.GuiPresentation).AddToDB();
            return powerItem;
        }
    }
}
