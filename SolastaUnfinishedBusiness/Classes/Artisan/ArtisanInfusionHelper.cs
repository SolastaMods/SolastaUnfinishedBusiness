using System;
using System.Collections.Generic;
using System.Linq;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.Classes.Artisan.Subclasses;
using SolastaUnfinishedBusiness.CustomDefinitions;
using UnityEngine;
using static SolastaUnfinishedBusiness.Classes.Artisan.ArtisanHelpers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ConditionDefinitions;
using FeatureDefinitionMagicAffinityBuilder =
    SolastaUnfinishedBusiness.Builders.Features.FeatureDefinitionMagicAffinityBuilder;

namespace SolastaUnfinishedBusiness.Classes.Artisan;

internal static class ArtisanInfusionHelper
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

    public static FeatureDefinitionPower ArtificialServant => artificialServant ??= BuildArtificialServant();

    public static FeatureDefinitionPower EnhancedFocus => enhancedFocus ??= BuildEnhancedFocus();

    public static FeatureDefinitionPower ImprovedEnhancedFocus =>
        improvedEnhancedFocus ??= BuildImprovedEnhancedFocus();

    public static FeatureDefinitionPower EnhancedDefense => enhancedDefense ??= BuildEnhancedDefense();

    public static FeatureDefinitionPower ImprovedEnhancedDefense =>
        improvedEnhancedDefense ??= BuildImprovedEnhancedDefense();

    public static FeatureDefinitionPower EnhancedWeapon => enhancedWeapon ??= BuildEnhancedWeapon();

    public static FeatureDefinitionPower ImprovedEnhancedWeapon =>
        improvedEnhancedWeapon ??= BuildImprovedEnhancedWeapon();

    public static FeatureDefinitionPower BagOfHolding => bagOfHolding ??= BuildBagOfHolding();

    public static FeatureDefinitionPower GogglesOfNight => gogglesOfNight ??= BuildGogglesOfNight();

    public static FeatureDefinitionPower MindSharpener => mindSharpener ??= BuildMindSharpener();

    public static FeatureDefinitionPower ArmorOfMagicalStrength =>
        armorOfMagicalStrength ??= BuildArmorOfMagicalStrength();

    public static FeatureDefinitionPower ResistantArmor => resistantArmor ??= BuildResistantArmor();

    public static FeatureDefinitionPower SpellRefuelingRing => spellRefuelingRing ??= BuildSpellRefuelingRing();

    public static FeatureDefinitionPower BlindingWeapon => blindingWeapon ??= BuildBlindingWeapon();

    public static FeatureDefinitionPower CloakOfProtection => cloakOfProtection ??=
        PowerMimicsItem(ItemDefinitions.CloakOfProtection, "InfuseCloakOfProtection");

    public static FeatureDefinitionPower BootsOfElvenKind => bootsOfElvenKind ??=
        PowerMimicsItem(ItemDefinitions.BootsOfElvenKind, "InfuseBootsOfElvenKind");

    public static FeatureDefinitionPower CloakOfElvenKind => cloakOfElvenKind ??=
        PowerMimicsItem(ItemDefinitions.CloakOfElvenkind, "InfuseCloakOfElvenKind");

    public static FeatureDefinitionPower BootsOfStridingAndSpringing => bootsOfStridingAndSpringing ??=
        PowerMimicsItem(ItemDefinitions.BootsOfStridingAndSpringing, "InfuseBootsOfStridingAndSpringing");

    public static FeatureDefinitionPower BootsOfTheWinterland => bootsOfTheWinterland ??=
        PowerMimicsItem(ItemDefinitions.BootsOfTheWinterland, "InfuseBootsOfTheWinterland");

    public static FeatureDefinitionPower BracesrOfArchery => bracesrOfArchery ??=
        PowerMimicsItem(ItemDefinitions.Bracers_Of_Archery, "InfuseBracesrOfArchery");

    public static FeatureDefinitionPower BroochOfShielding => broochOfShielding ??=
        PowerMimicsItem(ItemDefinitions.BroochOfShielding, "InfuseBroochOfShielding");

    public static FeatureDefinitionPower GauntletsOfOgrePower => gauntletsOfOgrePower ??=
        PowerMimicsItem(ItemDefinitions.GauntletsOfOgrePower, "InfuseGauntletsOfOgrePower");

    public static FeatureDefinitionPower GlovesOfMissileSnaring => glovesOfMissileSnaring ??=
        PowerMimicsItem(ItemDefinitions.GlovesOfMissileSnaring, "InfuseGlovesOfMissileSnaring");

    public static FeatureDefinitionPower SlippersOfSpiderClimbing => slippersOfSpiderClimbing ??=
        PowerMimicsItem(ItemDefinitions.SlippersOfSpiderClimbing, "InfuseSlippersOfSpiderClimbing");

    public static FeatureDefinitionPower HeadbandOfIntellect => headbandOfIntellect ??=
        PowerMimicsItem(ItemDefinitions.HeadbandOfIntellect, "InfuseHeadbandOfIntellect");

    public static FeatureDefinitionPower AmuletOfHealth => amuletOfHealth ??=
        PowerMimicsItem(ItemDefinitions.AmuletOfHealth, "InfuseAmuletOfHealth");

    public static FeatureDefinitionPower BeltOfGiantHillStrength => beltOfGiantHillStrength ??=
        PowerMimicsItem(ItemDefinitions.BeltOfGiantHillStrength, "InfuseBeltOfGiantHillStrength");

    public static FeatureDefinitionPower BracersOfDefense => bracersOfDefense ??=
        PowerMimicsItem(ItemDefinitions.Bracers_Of_Defense, "InfuseBracersOfDefense");

    public static FeatureDefinitionPower CloakOfBat =>
        cloakOfBat ??= PowerMimicsItem(ItemDefinitions.CloakOfBat, "InfuseCloakOfBat");

    public static FeatureDefinitionPower RingProtectionPlus1 => ringProtectionPlus1 ??=
        PowerMimicsItem(ItemDefinitions.RingProtectionPlus1, "InfuseRingProtectionPlus1");

    private static FeatureDefinitionPowerSharedPoolBuilder BuildBasicInfusionPower(string name,
        EffectDescription effect)
    {
        return FeatureDefinitionPowerSharedPoolBuilder.Create(name, ArtisanClass.GuidNamespace)
            .Configure(
                ArtisanClass.InfusionPool, RuleDefinitions.RechargeRate.LongRest,
                RuleDefinitions.ActivationTime.NoCost,
                1, false, false, AttributeDefinitions.Intelligence, effect, true /* unique instance */);
    }

    private static FeatureDefinitionPower BuildArtificialServant()
    {
        var artificialServantEffect = new EffectDescriptionBuilder();
        artificialServantEffect.SetDurationData(RuleDefinitions.DurationType.UntilLongRest, 1,
            RuleDefinitions.TurnOccurenceType.EndOfTurn);
        artificialServantEffect.SetTargetingData(RuleDefinitions.Side.Ally, RuleDefinitions.RangeType.Distance, 1,
            RuleDefinitions.TargetType.Position, 1, 1, ActionDefinitions.ItemSelectionType.Equiped);
        artificialServantEffect.AddEffectForm(new EffectFormBuilder().SetSummonForm(SummonForm.Type.Creature,
                ScriptableObject.CreateInstance<ItemDefinition>(), 1,
                ArtificialServantBuilder.ArtificialServant.name,
                ConditionFlyingBootsWinged, true, null, ScriptableObject.CreateInstance<EffectProxyDefinition>())
            .Build());

        return BuildBasicInfusionPower("summonArtificialServantPower", artificialServantEffect.Build())
            .SetGuiPresentation("SummonArtificialServant", Category.Feat,
                SpellDefinitions.ConjureGoblinoids.GuiPresentation.SpriteReference)
            .AddToDB();
    }

    private static FeatureDefinitionPower BuildEnhancedFocus()
    {
        var focusPlus1Gui = new GuiPresentationBuilder(
            "Subclass/&AttackModifierArtisanEnhancedFocusTitle",
            "Subclass/&AttackModifierArtisanEnhancedFocusDescription");
        focusPlus1Gui.SetSpriteReference(FeatureDefinitionAttackModifiers.AttackModifierMagicWeapon.GuiPresentation
            .SpriteReference);
        var focusPlus1 = BuildMagicAffinityModifiers("Enhanced Focus", 1, 1, focusPlus1Gui.Build());
        var infusedFocusCondition = BuildCondition("ArtisanInfusedFocus",
            RuleDefinitions.DurationType.UntilLongRest, 1, false, focusPlus1Gui.Build(), focusPlus1);

        var enhanceFocusGui = new GuiPresentationBuilder(
            "Subclass/&AttackModifierArtisanEnhancedFocusTitle",
            "Subclass/&AttackModifierArtisanEnhancedFocusDescription");
        enhanceFocusGui.SetSpriteReference(FeatureDefinitionPowers.PowerDomainElementalLightningBlade
            .GuiPresentation.SpriteReference);
        return BuildItemConditionInfusion(infusedFocusCondition, "ArtisanInfusionEnhancedFocus",
            enhanceFocusGui.Build()).AddToDB();
    }

    private static FeatureDefinitionPower BuildImprovedEnhancedFocus()
    {
        var focusPlus2Gui = new GuiPresentationBuilder(
            "Subclass/&AttackModifierArtisanImprovedEnhancedFocusTitle",
            "Subclass/&AttackModifierArtisanImprovedEnhancedFocusDescription");
        focusPlus2Gui.SetSpriteReference(FeatureDefinitionAttackModifiers.AttackModifierMagicWeapon.GuiPresentation
            .SpriteReference);
        var focusPlus2 = BuildMagicAffinityModifiers("ImprovedEnhancedFocus", 2, 2, focusPlus2Gui.Build());
        var infusedFocusCondition = BuildCondition("ArtisanImprovedInfusedFocus",
            RuleDefinitions.DurationType.UntilLongRest, 1, false,
            focusPlus2Gui.Build(), focusPlus2);

        var enhanceFocusGui = new GuiPresentationBuilder(
            "Subclass/&AttackModifierArtisanImprovedEnhancedFocusTitle",
            "Subclass/&AttackModifierArtisanImprovedEnhancedFocusDescription");
        enhanceFocusGui.SetSpriteReference(FeatureDefinitionPowers.PowerDomainElementalLightningBlade
            .GuiPresentation.SpriteReference);
        return BuildItemConditionInfusion(infusedFocusCondition, "ArtisanInfusionImprovedEnhancedFocus",
                enhanceFocusGui.Build())
            .SetOverriddenPower(EnhancedFocus).AddToDB();
    }

    private static FeatureDefinitionPower BuildEnhancedDefense()
    {
        var enhanceArmorConditionGui = new GuiPresentationBuilder(
            "Subclass/&AttackModifierArtisanEnhancedArmorTitle",
            "Subclass/&AttackModifierArtisanEnhancedArmorDescription");
        enhanceArmorConditionGui.SetSpriteReference(ConditionAuraOfProtection.GuiPresentation.SpriteReference);
        var armorModifier = BuildAttributeModifier("AttributeModifierArmorInfusion",
            FeatureDefinitionAttributeModifier.AttributeModifierOperation.Additive, AttributeDefinitions.ArmorClass,
            1, enhanceArmorConditionGui.Build());

        var enhanceArmorGui = new GuiPresentationBuilder(
            "Subclass/&AttackModifierArtisanEnhancedArmorTitle",
            "Subclass/&AttackModifierArtisanEnhancedArmorDescription");
        enhanceArmorGui.SetSpriteReference(FeatureDefinitionPowers.PowerPaladinAuraOfProtection.GuiPresentation
            .SpriteReference);

        return BuildItemModifierInfusion(armorModifier, ActionDefinitions.ItemSelectionType.Equiped,
            "ArtisanInfusionEnhancedArmor", enhanceArmorGui.Build()).AddToDB();
    }

    private static FeatureDefinitionPower BuildImprovedEnhancedDefense()
    {
        var enhanceArmorConditionGui = new GuiPresentationBuilder(
            "Subclass/&AttackModifierArtisanImprovedEnhancedArmorTitle",
            "Subclass/&AttackModifierArtisanImprovedEnhancedArmorDescription");
        enhanceArmorConditionGui.SetSpriteReference(ConditionAuraOfProtection.GuiPresentation.SpriteReference);
        var armorModifier = BuildAttributeModifier("AttributeModifierImprovedArmorInfusion",
            FeatureDefinitionAttributeModifier.AttributeModifierOperation.Additive, AttributeDefinitions.ArmorClass,
            2, enhanceArmorConditionGui.Build());

        var enhanceArmorGui = new GuiPresentationBuilder(
            "Subclass/&AttackModifierArtisanImprovedEnhancedArmorTitle",
            "Subclass/&AttackModifierArtisanImprovedEnhancedArmorDescription");
        enhanceArmorGui.SetSpriteReference(FeatureDefinitionPowers.PowerPaladinAuraOfProtection.GuiPresentation
            .SpriteReference);

        return BuildItemModifierInfusion(armorModifier, ActionDefinitions.ItemSelectionType.Equiped,
                "ArtisanInfusionImprovedEnhancedArmor", enhanceArmorGui.Build())
            .SetOverriddenPower(EnhancedDefense).AddToDB();
    }

    private static FeatureDefinitionPower BuildEnhancedWeapon()
    {
        var enhanceWeaponGui = new GuiPresentationBuilder(
            "Subclass/&AttackModifierArtisanEnhancedWeaponTitle",
            "Subclass/&AttackModifierArtisanEnhancedWeaponDescription");
        enhanceWeaponGui.SetSpriteReference(FeatureDefinitionPowers.PowerDomainElementalLightningBlade
            .GuiPresentation.SpriteReference);

        return BuildItemModifierInfusion(FeatureDefinitionAttackModifiers.AttackModifierMagicWeapon,
            ActionDefinitions.ItemSelectionType.WeaponNonMagical, "ArtisanInfusionEnhancedWeapon",
            enhanceWeaponGui.Build()).AddToDB();
    }

    private static FeatureDefinitionPower BuildImprovedEnhancedWeapon()
    {
        var enhanceWeaponGui = new GuiPresentationBuilder(
            "Subclass/&AttackModifierArtisanImprovedEnhancedWeaponTitle",
            "Subclass/&AttackModifierArtisanImprovedEnhancedWeaponDescription");
        enhanceWeaponGui.SetSpriteReference(FeatureDefinitionPowers.PowerDomainElementalLightningBlade
            .GuiPresentation.SpriteReference);

        return BuildItemModifierInfusion(FeatureDefinitionAttackModifiers.AttackModifierMagicWeapon2,
                ActionDefinitions.ItemSelectionType.WeaponNonMagical, "ArtisanInfusionImprovedEnhancedWeapon",
                enhanceWeaponGui.Build())
            .SetOverriddenPower(EnhancedWeapon).AddToDB();
    }

    private static FeatureDefinitionPower BuildBagOfHolding()
    {
        var affinity = FeatureDefinitionEquipmentAffinityBuilder
            .Create("InfusionBagOfHolding", ArtisanClass.GuidNamespace)
            .SetGuiPresentation("EquipmentModifierArtisanBagOfHolder", Category.Subclass,
                ConditionBullsStrength.GuiPresentation.SpriteReference)
            .SetCarryingCapacityMultiplier(1.0f, 500.0f)
            .AddToDB();

        var bagOfHoldingCondition = BuildCondition("ArtisanInfusedConditionBagOfHolding",
            RuleDefinitions.DurationType.UntilLongRest, 1, false, affinity.GuiPresentation, affinity);

        var bagOfHoldingGui = new GuiPresentationBuilder(
            "Subclass/&EquipmentModifierArtisanBagOfHolderTitle",
            "Subclass/&EquipmentModifierArtisanBagOfHolderDescription");
        bagOfHoldingGui.SetSpriteReference(FeatureDefinitionPowers.PowerFunctionPotionOfGiantStrengthCloud
            .GuiPresentation.SpriteReference);
        return BuildItemConditionInfusion(bagOfHoldingCondition, "ArtisanInfusionBagOfHolding",
            bagOfHoldingGui.Build()).AddToDB();
    }

    private static FeatureDefinitionPower BuildGogglesOfNight()
    {
        var InfuseDarkvisionCondition = new GuiPresentationBuilder(
            "Subclass/&PowerInfuseDarkvisionTitle",
            "Subclass/&PowerInfuseDarkvisionDescription");
        InfuseDarkvisionCondition.SetSpriteReference(ConditionSeeInvisibility.GuiPresentation.SpriteReference);
        var darkvisionCondition = BuildCondition("ArtisanInfusedConditionDarkvision",
            RuleDefinitions.DurationType.UntilLongRest, 1, false,
            InfuseDarkvisionCondition.Build(), FeatureDefinitionSenses.SenseSuperiorDarkvision);

        var InfuseDarkvision = new GuiPresentationBuilder(
            "Subclass/&PowerInfuseDarkvisionTitle",
            "Subclass/&PowerInfuseDarkvisionDescription");
        InfuseDarkvision.SetSpriteReference(FeatureDefinitionPowers.PowerDomainBattleDivineWrath.GuiPresentation
            .SpriteReference);

        return BuildItemConditionInfusion(darkvisionCondition, "PowerInfuseDarkvision", InfuseDarkvision.Build())
            .AddToDB();
    }

    private static FeatureDefinitionPower BuildMindSharpener()
    {
        var affinity = FeatureDefinitionMagicAffinityBuilder
            .Create("MagicAffinityMindSharpener", ArtisanClass.GuidNamespace)
            .SetConcentrationModifiers(RuleDefinitions.ConcentrationAffinity.Advantage, 20)
            .SetGuiPresentation("PowerInfuseMindSharpener", Category.Subclass,
                ConditionBearsEndurance.GuiPresentation.SpriteReference)
            .AddToDB();

        var infusedMindSharpenerCondition = BuildCondition("ArtisanInfusedConditionMindSharpener",
            RuleDefinitions.DurationType.UntilLongRest, 1, false, affinity.GuiPresentation, affinity);

        var InfuseMindSharpener = new GuiPresentationBuilder(
            "Subclass/&PowerInfuseMindSharpenerTitle",
            "Subclass/&PowerInfuseMindSharpenerDescription");
        InfuseMindSharpener.SetSpriteReference(FeatureDefinitionPowers.PowerFunctionTomeOfQuickThought
            .GuiPresentation.SpriteReference);

        return BuildItemConditionInfusion(infusedMindSharpenerCondition, "ArtisanInfusionMindSharpener",
            InfuseMindSharpener.Build()).AddToDB();
    }

    private static FeatureDefinitionPower BuildArmorOfMagicalStrength()
    {
        var InfuseArmorMagicalStrengthCondition = new GuiPresentationBuilder(
            "Subclass/&PowerInfuseArmorMagicalStrengthTitle",
            "Subclass/&PowerInfuseArmorMagicalStrengthDescription");
        InfuseArmorMagicalStrengthCondition.SetSpriteReference(ConditionBullsStrength.GuiPresentation
            .SpriteReference);
        var strengthAbilityAffinity = BuildAbilityAffinity("AbilityAffinityInfusionMagicalStrength",
            new List<Tuple<string, string>> { new(AttributeDefinitions.Strength, "") }, 0, RuleDefinitions.DieType.D1,
            RuleDefinitions.CharacterAbilityCheckAffinity.Advantage, InfuseArmorMagicalStrengthCondition.Build());
        var strengthSaveAffinity = BuildSavingThrowAffinity("SaveAffinityInfusionMagicalStrength",
            new List<string> { AttributeDefinitions.Strength },
            RuleDefinitions.CharacterSavingThrowAffinity.Advantage,
            FeatureDefinitionSavingThrowAffinity.ModifierType.AddDice, 0, RuleDefinitions.DieType.D1, false,
            InfuseArmorMagicalStrengthCondition.Build());
        var armorMagicalStrengthCondition = BuildCondition("ArtisanInfusionArmorMagicalStrengthCondition",
            RuleDefinitions.DurationType.UntilLongRest, 1, false, InfuseArmorMagicalStrengthCondition.Build(),
            strengthAbilityAffinity, strengthSaveAffinity,
            FeatureDefinitionConditionAffinitys.ConditionAffinityProneImmunity);

        var InfuseArmorMagicalStrength = new GuiPresentationBuilder(
            "Subclass/&PowerInfuseArmorMagicalStrengthTitle",
            "Subclass/&PowerInfuseArmorMagicalStrengthDescription");
        InfuseArmorMagicalStrength.SetSpriteReference(FeatureDefinitionPowers.PowerFunctionManualGainfulExercise
            .GuiPresentation.SpriteReference);
        return BuildItemConditionInfusion(armorMagicalStrengthCondition, "ArtisanInfusionArmorMagicalStrength",
            InfuseArmorMagicalStrength.Build()).AddToDB();
    }

    private static FeatureDefinitionPower BuildResistantArmor()
    {
        var InfuseResistantArmor = new GuiPresentationBuilder(
            "Subclass/&PowerInfuseResistantArmorTitle",
            "Subclass/&PowerInfuseResistantArmorDescription");
        InfuseResistantArmor.SetSpriteReference(FeatureDefinitionPowers
            .PowerDomainElementalDiscipleOfTheElementsLightning.GuiPresentation.SpriteReference);
        var ConditionArmorResistance = new GuiPresentationBuilder(
            "Subclass/&ConditionResistantArmorTitle",
            "Subclass/&ConditionResistnatArmorDescription");
        ConditionArmorResistance.SetSpriteReference(ConditionAuraOfProtection.GuiPresentation.SpriteReference);
        var ArmorResistance = BuildCondition("ConditionPowerArtisanResistantArmor",
            RuleDefinitions.DurationType.UntilLongRest, 1, false, ConditionArmorResistance.Build(),
            FeatureDefinitionDamageAffinitys.DamageAffinityAcidResistance,
            FeatureDefinitionDamageAffinitys.DamageAffinityColdResistance,
            FeatureDefinitionDamageAffinitys.DamageAffinityFireResistance,
            FeatureDefinitionDamageAffinitys.DamageAffinityForceDamageResistance,
            FeatureDefinitionDamageAffinitys.DamageAffinityLightningResistance,
            FeatureDefinitionDamageAffinitys.DamageAffinityNecroticResistance,
            FeatureDefinitionDamageAffinitys.DamageAffinityPoisonResistance,
            FeatureDefinitionDamageAffinitys.DamageAffinityPsychicResistance,
            FeatureDefinitionDamageAffinitys.DamageAffinityRadiantResistance,
            FeatureDefinitionDamageAffinitys.DamageAffinityThunderResistance);
        return BuildItemConditionInfusion(ArmorResistance, "ArtisanInfusionResistantArmor",
            InfuseResistantArmor.Build()).AddToDB();
    }

    private static FeatureDefinitionPower BuildSpellRefuelingRing()
    {
        var InfuseSpellRefuelingRing = new GuiPresentationBuilder(
            "Subclass/&PowerSpellRefuelingRingTitle",
            "Subclass/&PowerSpellRefuelingRingDescription");
        InfuseSpellRefuelingRing.SetSpriteReference(FeatureDefinitionPowers
            .PowerDomainElementalDiscipleOfTheElementsLightning.GuiPresentation.SpriteReference);

        var spellEffect = new EffectDescriptionBuilder();
        spellEffect.SetDurationData(RuleDefinitions.DurationType.UntilLongRest, 1,
            RuleDefinitions.TurnOccurenceType.EndOfTurn);
        spellEffect.SetTargetingData(RuleDefinitions.Side.Ally, RuleDefinitions.RangeType.Self, 1,
            RuleDefinitions.TargetType.Self);
        spellEffect.AddEffectForm(new EffectFormBuilder().SetSpellForm(9).Build());

        return new FeatureDefinitionPowerSharedPoolBuilder("ArtisanInfusionSpellRefuelingRing",
            GuidHelper.Create(ArtisanClass.GuidNamespace, "ArtisanInfusionSpellRefuelingRing").ToString(),
            ArtisanClass.InfusionPool, RuleDefinitions.RechargeRate.LongRest,
            RuleDefinitions.ActivationTime.NoCost, 1, false, false, AttributeDefinitions.Intelligence,
            spellEffect.Build(), InfuseSpellRefuelingRing.Build(), true /* unique instance */).AddToDB();
    }

    private static FeatureDefinitionPower BuildBlindingWeapon()
    {
        var radiantWeaponEffectGui = new GuiPresentationBuilder(
            "Subclass/&AttackModifierArtisanBlindingWeaponTitle",
            "Subclass/&AttackModifierArtisanBlindingWeaponDescription");
        radiantWeaponEffectGui.SetSpriteReference(FeatureDefinitionAttackModifiers.AttackModifierMagicWeapon
            .GuiPresentation.SpriteReference);

        var addBlindingCondition = new ConditionOperationDescription
        {
            Operation = ConditionOperationDescription.ConditionOperation.Add,
            ConditionDefinition = ConditionBlinded,
            saveAffinity = RuleDefinitions.EffectSavingThrowType.Negates,
            conditionName = ConditionBlinded.Name
        };

        var radiantDamage = new ArtisanHelpers.FeatureDefinitionAdditionalDamageBuilder(
                "AdditionalDamageRadiantWeapon",
                ArtisanClass.GuidNamespace, "BlindingWeaponStrike",
                RuleDefinitions.FeatureLimitedUsage.OncePerTurn,
                RuleDefinitions.AdditionalDamageValueDetermination.Die,
                RuleDefinitions.AdditionalDamageTriggerCondition.AlwaysActive,
                RuleDefinitions.RestrictedContextRequiredProperty.None, true, RuleDefinitions.DieType.D4, 1,
                RuleDefinitions.AdditionalDamageType.Specific,
                "DamageRadiant", RuleDefinitions.AdditionalDamageAdvancement.None, new List<DiceByRank>(), true,
                AttributeDefinitions.Constitution, 15, RuleDefinitions.EffectSavingThrowType.None,
                new List<ConditionOperationDescription> { addBlindingCondition }, radiantWeaponEffectGui.Build())
            .AddToDB();

        var radiantWeaponGui = new GuiPresentationBuilder(
            "Subclass/&AttackModifierArtisanBlindingWeaponTitle",
            "Subclass/&AttackModifierArtisanBlindingWeaponDescription");
        radiantWeaponGui.SetSpriteReference(FeatureDefinitionPowers.PowerDomainSunIndomitableLight.GuiPresentation
            .SpriteReference);

        return BuildItemModifierInfusion(radiantDamage,
                ActionDefinitions.ItemSelectionType.Weapon, "ArtisanInfusionBlindingWeapon",
                radiantWeaponGui.Build())
            .AddToDB();
    }

    public static FeatureDefinitionPowerSharedPoolBuilder BuildItemModifierInfusion(FeatureDefinition itemFeature,
        ActionDefinitions.ItemSelectionType itemType,
        string name, GuiPresentation gui)
    {
        var itemEffect = new EffectDescriptionBuilder();
        itemEffect.SetDurationData(RuleDefinitions.DurationType.UntilLongRest, 1,
            RuleDefinitions.TurnOccurenceType.EndOfTurn);
        itemEffect.SetTargetingData(RuleDefinitions.Side.Ally, RuleDefinitions.RangeType.Touch, 1,
            RuleDefinitions.TargetType.Item, 1, 1, itemType);
        itemEffect.AddEffectForm(new EffectFormBuilder()
            .SetItemPropertyForm(new List<FeatureUnlockByLevel> { new(itemFeature, 0) },
                RuleDefinitions.ItemPropertyUsage.Unlimited, 1).Build());

        return BuildBasicInfusionPower(name, itemEffect.Build())
            .SetGuiPresentation(gui)
            .SetCustomSubFeatures(FeatureDefinitionSkipEffectRemovalOnLocationChange.Always);
    }

    private static FeatureDefinitionPowerSharedPoolBuilder BuildItemConditionInfusion(ConditionDefinition condition,
        string name, GuiPresentation gui)
    {
        var conditionEffect = new EffectDescriptionBuilder();
        conditionEffect.SetDurationData(RuleDefinitions.DurationType.UntilLongRest, 1,
            RuleDefinitions.TurnOccurenceType.EndOfTurn);
        conditionEffect.SetTargetingData(RuleDefinitions.Side.Ally, RuleDefinitions.RangeType.Touch, 1,
            RuleDefinitions.TargetType.Individuals, 1, 1, ActionDefinitions.ItemSelectionType.Equiped);
        conditionEffect.AddEffectForm(new EffectFormBuilder().SetConditionForm(condition,
            ConditionForm.ConditionOperation.Add, false, false, new List<ConditionDefinition>()).Build());
        return BuildBasicInfusionPower(name, conditionEffect.Build())
            .SetGuiPresentation(gui)
            .SetCustomSubFeatures(FeatureDefinitionSkipEffectRemovalOnLocationChange.Always);
    }

    private static FeatureDefinitionPower PowerMimicsItem(ItemDefinition item, string name)
    {
        var itemCondition = BuildCondition("Condition" + name, RuleDefinitions.DurationType.UntilLongRest, 1, false,
            item.GuiPresentation, item.StaticProperties.Select(p => p.FeatureDefinition).ToArray());

        var itemEffect = new EffectDescriptionBuilder();
        itemEffect.SetDurationData(RuleDefinitions.DurationType.UntilLongRest, 1,
            RuleDefinitions.TurnOccurenceType.EndOfTurn);
        itemEffect.SetTargetingData(RuleDefinitions.Side.Ally, RuleDefinitions.RangeType.Touch, 1,
            RuleDefinitions.TargetType.Individuals, 1, 1, ActionDefinitions.ItemSelectionType.Equiped);
        itemEffect.AddEffectForm(new EffectFormBuilder().SetConditionForm(itemCondition,
            ConditionForm.ConditionOperation.Add, false, false, new List<ConditionDefinition>()).Build());

        return BuildBasicInfusionPower(TagsDefinitions.Power + name, itemEffect.Build())
            .SetGuiPresentation(item.GuiPresentation)
            .SetCustomSubFeatures(FeatureDefinitionSkipEffectRemovalOnLocationChange.Always)
            .AddToDB();
    }
}
