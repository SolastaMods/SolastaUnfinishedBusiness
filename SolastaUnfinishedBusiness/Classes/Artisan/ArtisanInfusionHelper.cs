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
    private static FeatureDefinitionPower _artificialServant;
    private static FeatureDefinitionPower _enhancedFocus;
    private static FeatureDefinitionPower _enhancedDefense;
    private static FeatureDefinitionPower _enhancedWeapon;
    private static FeatureDefinitionPower _mindSharpener;
    private static FeatureDefinitionPower _armorOfMagicalStrength;
    private static FeatureDefinitionPower _bagOfHolding;
    private static FeatureDefinitionPower _gogglesOfNight;
    private static FeatureDefinitionPower _resistantArmor;
    private static FeatureDefinitionPower _spellRefuelingRing;
    private static FeatureDefinitionPower _blindingWeapon;
    private static FeatureDefinitionPower _improvedEnhancedFocus;
    private static FeatureDefinitionPower _improvedEnhancedDefense;
    private static FeatureDefinitionPower _cloakOfProtection;
    private static FeatureDefinitionPower _bootsOfElvenKind;
    private static FeatureDefinitionPower _cloakOfElvenKind;
    private static FeatureDefinitionPower _bootsOfStridingAndSpringing;
    private static FeatureDefinitionPower _bootsOfTheWinterland;
    private static FeatureDefinitionPower _bracersOfArchery;
    private static FeatureDefinitionPower _broochOfShielding;
    private static FeatureDefinitionPower _gauntletsOfOgrePower;
    private static FeatureDefinitionPower _glovesOfMissileSnaring;
    private static FeatureDefinitionPower _slippersOfSpiderClimbing;
    private static FeatureDefinitionPower _headbandOfIntellect;
    private static FeatureDefinitionPower _amuletOfHealth;
    private static FeatureDefinitionPower _beltOfGiantHillStrength;
    private static FeatureDefinitionPower _bracersOfDefense;
    private static FeatureDefinitionPower _cloakOfBat;
    private static FeatureDefinitionPower _ringProtectionPlus1;
    private static FeatureDefinitionPower _improvedEnhancedWeapon;

    public static FeatureDefinitionPower ArtificialServant => _artificialServant ??= BuildArtificialServant();

    public static FeatureDefinitionPower EnhancedFocus => _enhancedFocus ??= BuildEnhancedFocus();

    public static FeatureDefinitionPower ImprovedEnhancedFocus =>
        _improvedEnhancedFocus ??= BuildImprovedEnhancedFocus();

    public static FeatureDefinitionPower EnhancedDefense => _enhancedDefense ??= BuildEnhancedDefense();

    public static FeatureDefinitionPower ImprovedEnhancedDefense =>
        _improvedEnhancedDefense ??= BuildImprovedEnhancedDefense();

    public static FeatureDefinitionPower EnhancedWeapon => _enhancedWeapon ??= BuildEnhancedWeapon();

    public static FeatureDefinitionPower ImprovedEnhancedWeapon =>
        _improvedEnhancedWeapon ??= BuildImprovedEnhancedWeapon();

    public static FeatureDefinitionPower BagOfHolding => _bagOfHolding ??= BuildBagOfHolding();

    public static FeatureDefinitionPower GogglesOfNight => _gogglesOfNight ??= BuildGogglesOfNight();

    public static FeatureDefinitionPower MindSharpener => _mindSharpener ??= BuildMindSharpener();

    public static FeatureDefinitionPower ArmorOfMagicalStrength =>
        _armorOfMagicalStrength ??= BuildArmorOfMagicalStrength();

    public static FeatureDefinitionPower ResistantArmor => _resistantArmor ??= BuildResistantArmor();

    public static FeatureDefinitionPower SpellRefuelingRing => _spellRefuelingRing ??= BuildSpellRefuelingRing();

    public static FeatureDefinitionPower BlindingWeapon => _blindingWeapon ??= BuildBlindingWeapon();

    public static FeatureDefinitionPower CloakOfProtection => _cloakOfProtection ??=
        PowerMimicsItem(ItemDefinitions.CloakOfProtection, "InfuseCloakOfProtection");

    public static FeatureDefinitionPower BootsOfElvenKind => _bootsOfElvenKind ??=
        PowerMimicsItem(ItemDefinitions.BootsOfElvenKind, "InfuseBootsOfElvenKind");

    public static FeatureDefinitionPower CloakOfElvenKind => _cloakOfElvenKind ??=
        PowerMimicsItem(ItemDefinitions.CloakOfElvenkind, "InfuseCloakOfElvenKind");

    public static FeatureDefinitionPower BootsOfStridingAndSpringing => _bootsOfStridingAndSpringing ??=
        PowerMimicsItem(ItemDefinitions.BootsOfStridingAndSpringing, "InfuseBootsOfStridingAndSpringing");

    public static FeatureDefinitionPower BootsOfTheWinterland => _bootsOfTheWinterland ??=
        PowerMimicsItem(ItemDefinitions.BootsOfTheWinterland, "InfuseBootsOfTheWinterland");

    public static FeatureDefinitionPower BracersOfArchery => _bracersOfArchery ??=
        PowerMimicsItem(ItemDefinitions.Bracers_Of_Archery, "InfuseBracersOfArchery");

    public static FeatureDefinitionPower BroochOfShielding => _broochOfShielding ??=
        PowerMimicsItem(ItemDefinitions.BroochOfShielding, "InfuseBroochOfShielding");

    public static FeatureDefinitionPower GauntletsOfOgrePower => _gauntletsOfOgrePower ??=
        PowerMimicsItem(ItemDefinitions.GauntletsOfOgrePower, "InfuseGauntletsOfOgrePower");

    public static FeatureDefinitionPower GlovesOfMissileSnaring => _glovesOfMissileSnaring ??=
        PowerMimicsItem(ItemDefinitions.GlovesOfMissileSnaring, "InfuseGlovesOfMissileSnaring");

    public static FeatureDefinitionPower SlippersOfSpiderClimbing => _slippersOfSpiderClimbing ??=
        PowerMimicsItem(ItemDefinitions.SlippersOfSpiderClimbing, "InfuseSlippersOfSpiderClimbing");

    public static FeatureDefinitionPower HeadbandOfIntellect => _headbandOfIntellect ??=
        PowerMimicsItem(ItemDefinitions.HeadbandOfIntellect, "InfuseHeadbandOfIntellect");

    public static FeatureDefinitionPower AmuletOfHealth => _amuletOfHealth ??=
        PowerMimicsItem(ItemDefinitions.AmuletOfHealth, "InfuseAmuletOfHealth");

    public static FeatureDefinitionPower BeltOfGiantHillStrength => _beltOfGiantHillStrength ??=
        PowerMimicsItem(ItemDefinitions.BeltOfGiantHillStrength, "InfuseBeltOfGiantHillStrength");

    public static FeatureDefinitionPower BracersOfDefense => _bracersOfDefense ??=
        PowerMimicsItem(ItemDefinitions.Bracers_Of_Defense, "InfuseBracersOfDefense");

    public static FeatureDefinitionPower CloakOfBat =>
        _cloakOfBat ??= PowerMimicsItem(ItemDefinitions.CloakOfBat, "InfuseCloakOfBat");

    public static FeatureDefinitionPower RingProtectionPlus1 => _ringProtectionPlus1 ??=
        PowerMimicsItem(ItemDefinitions.RingProtectionPlus1, "InfuseRingProtectionPlus1");

    private static FeatureDefinitionPowerSharedPoolBuilder BuildBasicInfusionPower(
        string name,
        EffectDescription effectDescription)
    {
        return FeatureDefinitionPowerSharedPoolBuilder
            .Create(name)
            .Configure(
                ArtisanClass.InfusionPool,
                RuleDefinitions.RechargeRate.LongRest,
                RuleDefinitions.ActivationTime.NoCost,
                1,
                false,
                false,
                AttributeDefinitions.Intelligence,
                effectDescription,
                true);
    }

    private static FeatureDefinitionPower BuildArtificialServant()
    {
        var artificialServantEffect = new EffectDescriptionBuilder();
        
        artificialServantEffect
            .SetDurationData(
                RuleDefinitions.DurationType.UntilLongRest,
                1,
                RuleDefinitions.TurnOccurenceType.EndOfTurn);
        artificialServantEffect
            .SetTargetingData(
                RuleDefinitions.Side.Ally,
                RuleDefinitions.RangeType.Distance,
                1,
                RuleDefinitions.TargetType.Position,
                1,
                1,
                ActionDefinitions.ItemSelectionType.Equiped);
        artificialServantEffect
            .AddEffectForm(new EffectFormBuilder()
                .SetSummonForm(
                    SummonForm.Type.Creature,
                ScriptableObject.CreateInstance<ItemDefinition>(),
                    1,
                    ArtificialServantBuilder.ArtificialServant.name,
                    ConditionFlyingBootsWinged,
                    true,
                    null,
                    ScriptableObject.CreateInstance<EffectProxyDefinition>())
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
        var infuseDarkvisionCondition = new GuiPresentationBuilder(
            "Subclass/&PowerInfuseDarkvisionTitle",
            "Subclass/&PowerInfuseDarkvisionDescription");
        infuseDarkvisionCondition.SetSpriteReference(ConditionSeeInvisibility.GuiPresentation.SpriteReference);
        var darkvisionCondition = BuildCondition("ArtisanInfusedConditionDarkvision",
            RuleDefinitions.DurationType.UntilLongRest, 1, false,
            infuseDarkvisionCondition.Build(), FeatureDefinitionSenses.SenseSuperiorDarkvision);

        var infuseDarkvision = new GuiPresentationBuilder(
            "Subclass/&PowerInfuseDarkvisionTitle",
            "Subclass/&PowerInfuseDarkvisionDescription");
        infuseDarkvision.SetSpriteReference(FeatureDefinitionPowers.PowerDomainBattleDivineWrath.GuiPresentation
            .SpriteReference);

        return BuildItemConditionInfusion(darkvisionCondition, "PowerInfuseDarkvision", infuseDarkvision.Build())
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

        var infuseMindSharpener = new GuiPresentationBuilder(
            "Subclass/&PowerInfuseMindSharpenerTitle",
            "Subclass/&PowerInfuseMindSharpenerDescription");
        infuseMindSharpener.SetSpriteReference(FeatureDefinitionPowers.PowerFunctionTomeOfQuickThought
            .GuiPresentation.SpriteReference);

        return BuildItemConditionInfusion(infusedMindSharpenerCondition, "ArtisanInfusionMindSharpener",
            infuseMindSharpener.Build()).AddToDB();
    }

    private static FeatureDefinitionPower BuildArmorOfMagicalStrength()
    {
        var infuseArmorMagicalStrengthCondition = new GuiPresentationBuilder(
            "Subclass/&PowerInfuseArmorMagicalStrengthTitle",
            "Subclass/&PowerInfuseArmorMagicalStrengthDescription");
        infuseArmorMagicalStrengthCondition.SetSpriteReference(ConditionBullsStrength.GuiPresentation
            .SpriteReference);
        var strengthAbilityAffinity = BuildAbilityAffinity("AbilityAffinityInfusionMagicalStrength",
            new List<Tuple<string, string>> { new(AttributeDefinitions.Strength, "") }, 0, RuleDefinitions.DieType.D1,
            RuleDefinitions.CharacterAbilityCheckAffinity.Advantage, infuseArmorMagicalStrengthCondition.Build());
        var strengthSaveAffinity = BuildSavingThrowAffinity("SaveAffinityInfusionMagicalStrength",
            new List<string> { AttributeDefinitions.Strength },
            RuleDefinitions.CharacterSavingThrowAffinity.Advantage,
            FeatureDefinitionSavingThrowAffinity.ModifierType.AddDice, 0, RuleDefinitions.DieType.D1, false,
            infuseArmorMagicalStrengthCondition.Build());
        var armorMagicalStrengthCondition = BuildCondition("ArtisanInfusionArmorMagicalStrengthCondition",
            RuleDefinitions.DurationType.UntilLongRest, 1, false, infuseArmorMagicalStrengthCondition.Build(),
            strengthAbilityAffinity, strengthSaveAffinity,
            FeatureDefinitionConditionAffinitys.ConditionAffinityProneImmunity);

        var infuseArmorMagicalStrength = new GuiPresentationBuilder(
            "Subclass/&PowerInfuseArmorMagicalStrengthTitle",
            "Subclass/&PowerInfuseArmorMagicalStrengthDescription");
        infuseArmorMagicalStrength.SetSpriteReference(FeatureDefinitionPowers.PowerFunctionManualGainfulExercise
            .GuiPresentation.SpriteReference);
        return BuildItemConditionInfusion(armorMagicalStrengthCondition, "ArtisanInfusionArmorMagicalStrength",
            infuseArmorMagicalStrength.Build()).AddToDB();
    }

    private static FeatureDefinitionPower BuildResistantArmor()
    {
        var infuseResistantArmor = new GuiPresentationBuilder(
            "Subclass/&PowerInfuseResistantArmorTitle",
            "Subclass/&PowerInfuseResistantArmorDescription");
        infuseResistantArmor.SetSpriteReference(FeatureDefinitionPowers
            .PowerDomainElementalDiscipleOfTheElementsLightning.GuiPresentation.SpriteReference);
        var conditionArmorResistance = new GuiPresentationBuilder(
            "Subclass/&ConditionResistantArmorTitle",
            "Subclass/&ConditionResistantArmorDescription");
        conditionArmorResistance.SetSpriteReference(ConditionAuraOfProtection.GuiPresentation.SpriteReference);
        var armorResistance = BuildCondition("ConditionPowerArtisanResistantArmor",
            RuleDefinitions.DurationType.UntilLongRest, 1, false, conditionArmorResistance.Build(),
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
        return BuildItemConditionInfusion(armorResistance, "ArtisanInfusionResistantArmor",
            infuseResistantArmor.Build()).AddToDB();
    }

    private static FeatureDefinitionPower BuildSpellRefuelingRing()
    {
        var infuseSpellRefuelingRing = new GuiPresentationBuilder(
            "Subclass/&PowerSpellRefuelingRingTitle",
            "Subclass/&PowerSpellRefuelingRingDescription");
        infuseSpellRefuelingRing.SetSpriteReference(FeatureDefinitionPowers
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
            spellEffect.Build(), infuseSpellRefuelingRing.Build(), true /* unique instance */).AddToDB();
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

    private static FeatureDefinitionPowerSharedPoolBuilder BuildItemConditionInfusion(
        ConditionDefinition condition,
        string name,
        GuiPresentation gui)
    {
        var conditionEffect = new EffectDescriptionBuilder();
        
        conditionEffect.SetDurationData(
            RuleDefinitions.DurationType.UntilLongRest,
            1,
            RuleDefinitions.TurnOccurenceType.EndOfTurn);
        conditionEffect.SetTargetingData(
            RuleDefinitions.Side.Ally,
            RuleDefinitions.RangeType.Touch,
            1,
            RuleDefinitions.TargetType.Individuals,
            1,
            1,
            ActionDefinitions.ItemSelectionType.Equiped);
        conditionEffect.AddEffectForm(
            new EffectFormBuilder()
                .SetConditionForm(
                    condition,
                    ConditionForm.ConditionOperation.Add,
                    false,
                    false,
                    new List<ConditionDefinition>()).Build());
        
        return BuildBasicInfusionPower(name, conditionEffect.Build())
            .SetGuiPresentation(gui)
            .SetCustomSubFeatures(FeatureDefinitionSkipEffectRemovalOnLocationChange.Always);
    }

    private static FeatureDefinitionPower PowerMimicsItem(ItemDefinition item, string name)
    {
        var itemCondition = BuildCondition(
            "Condition" + "Artisan" + name,
            RuleDefinitions.DurationType.UntilLongRest,
            1,
            false,
            item.GuiPresentation,
            item.StaticProperties.Select(p => p.FeatureDefinition).ToArray());

        var itemEffect = new EffectDescriptionBuilder();
        
        itemEffect.SetDurationData(
            RuleDefinitions.DurationType.UntilLongRest,
            1,
            RuleDefinitions.TurnOccurenceType.EndOfTurn);
        itemEffect.SetTargetingData(
            RuleDefinitions.Side.Ally,
            RuleDefinitions.RangeType.Touch,
            1,
            RuleDefinitions.TargetType.Individuals,
            1,
            1,
            ActionDefinitions.ItemSelectionType.Equiped);
        itemEffect.AddEffectForm(
            new EffectFormBuilder()
                .SetConditionForm(
                    itemCondition,
                    ConditionForm.ConditionOperation.Add,
                    false,
                    false,
                    new List<ConditionDefinition>())
                .Build());

        return BuildBasicInfusionPower(TagsDefinitions.Power + "Artisan" + name, itemEffect.Build())
            .SetGuiPresentation(item.GuiPresentation)
            .SetCustomSubFeatures(FeatureDefinitionSkipEffectRemovalOnLocationChange.Always)
            .AddToDB();
    }
}
