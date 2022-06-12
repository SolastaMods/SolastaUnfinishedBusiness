using System;
using System.Collections.Generic;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaCommunityExpansion.Properties;
using SolastaCommunityExpansion.Utils;
using SolastaCommunityExpansion.Models;
using SolastaCommunityExpansion.Api.Infrastructure;
using TA;
using static SolastaCommunityExpansion.Api.DatabaseHelper;
using static SolastaCommunityExpansion.Api.DatabaseHelper.CharacterRaceDefinitions;
using static SolastaCommunityExpansion.Api.DatabaseHelper.MorphotypeElementDefinitions;

namespace SolastaCommunityExpansion.Races;

internal static class DarkelfRaceBuilder
{
    internal static CharacterRaceDefinition DarkelfRace { get; } = BuildDarkelf();

    private static CharacterRaceDefinition BuildDarkelf()
	{
        var darkelfSpriteReference =
            CustomIcons.CreateAssetReferenceSprite("Darkelf", Resources.Darkelf, 1024, 512);

        var darkelfAbilityScoreModifierCharisma = FeatureDefinitionAttributeModifierBuilder
            .Create("AttributeModifierDarkelfCharismaAbilityScoreIncrease", "21fbf034-47bd-436b-b92d-95883549a6d8")
            .SetGuiPresentation(Category.Feature)
            .SetModifier(FeatureDefinitionAttributeModifier.AttributeModifierOperation.Additive,
                AttributeDefinitions.Charisma, 1)
            .AddToDB();

        var darkElfPerception = FeatureDefinitionAbilityCheckAffinityBuilder
            .Create("AbilityCheckAffinityDarkelfLightSensitivity", "a4f82743-0d75-4178-ba25-b3707420e17e")
            .SetGuiPresentation(Category.Feature)
            .BuildAndSetAffinityGroups(
                RuleDefinitions.CharacterAbilityCheckAffinity.Disadvantage, RuleDefinitions.DieType.D1, 0,
                (AttributeDefinitions.Wisdom, SkillDefinitions.Perception))
            .AddToDB();
        darkElfPerception.AffinityGroups[0].SetField("lightingContext", RuleDefinitions.LightingContext.BrightLight);

        var darkelfConditionLightSensitive = ConditionDefinitionBuilder
            .Create("ConditionDarkelfLightSensitive", "8c7cb851-6810-4101-8a6a-e932e9cc3896")
            .SetGuiPresentation(Category.Condition)
            .SetFeatures(FeatureDefinitionCombatAffinitys.CombatAffinitySensitiveToLight, darkElfPerception)
            .AddToDB();

        var darkelfLightingEffectAndCondition = new FeatureDefinitionLightAffinity.LightingEffectAndCondition
        {
            lightingState = LocationDefinitions.LightingState.Bright, condition = darkelfConditionLightSensitive
        };

        var darkelfLightAffinity = FeatureDefinitionLightAffinityBuilder
            .Create(FeatureDefinitionLightAffinitys.LightAffinityLightSensitivity, "LightAffinityDarkelfLightSensitivity", "707231ea-e34d-4e26-9af8-4e52c0cb85c3")
            .SetGuiPresentation(Category.Feature)
            .AddLightingEffectAndCondition(darkelfLightingEffectAndCondition)
            .AddToDB();

        var darkelfDarkelfMagicSpellList = SpellListDefinitionBuilder
            .Create(SpellListDefinitions.SpellListWizard, "DarkelfMagicSpellList", "7e84092e-8b26-4870-8244-ce435a95b67f")
            .SetGuiPresentationNoContent()
            .SetSpellsAtLevel(0, SpellDefinitions.DancingLights)
            .FinalizeSpells()
            .AddToDB();

        var darkelfDarkMagic = FeatureDefinitionCastSpellBuilder
            .Create(FeatureDefinitionCastSpells.CastSpellElfHigh, "DarkelfMagic", "0271c652-b4aa-4346-806d-9711e634271b")
            .SetGuiPresentation(Category.Feature)
            .SetSpellList(darkelfDarkelfMagicSpellList)
            .SetSpellCastingAbility(AttributeDefinitions.Charisma)
            .AddToDB();

        var darkelfFaerieFirePower = FeatureDefinitionPowerBuilder
            .Create("PowerDarkelfFaerieFire", "68c2a6d0-cbac-4185-bdf5-b0f5b22ef73a")
            .SetGuiPresentation(Category.Feature, SpellDefinitions.FaerieFire.GuiPresentation.SpriteReference)
            .SetEffectDescription(SpellDefinitions.FaerieFire.EffectDescription)
            .SetActivationTime(RuleDefinitions.ActivationTime.Action)
            .SetFixedUsesPerRecharge(1)
            .SetRechargeRate(RuleDefinitions.RechargeRate.LongRest)
            .SetCostPerUse(1)
            .SetShowCasting(true)
            .AddToDB();

        var darkelfDarknessPower = FeatureDefinitionPowerBuilder
            .Create("PowerDarkelfDarkness", "0c413f86-2fa8-4f3d-999a-89e6c7b5b14d")
            .SetGuiPresentation(Category.Feature, SpellDefinitions.Darkness.GuiPresentation.SpriteReference)
            .SetEffectDescription(SpellDefinitions.Darkness.EffectDescription)
            .SetActivationTime(RuleDefinitions.ActivationTime.Action)
            .SetFixedUsesPerRecharge(1)
            .SetRechargeRate(RuleDefinitions.RechargeRate.LongRest)
            .SetCostPerUse(1)
            .SetShowCasting(true)
            .AddToDB();

        var darkelfWeaponTraining = FeatureDefinitionProficiencyBuilder
            .Create("DarkelfWeaponTraining", "ec6e4a4a-5635-4378-a370-5a5ab7dab2ea")
            .SetGuiPresentation(Category.Feature)
            .SetProficiencies(RuleDefinitions.ProficiencyType.Weapon,
                CustomWeaponsContext.HandXbowWeaponType.Name,
                WeaponTypeDefinitions.RapierType.Name,
                WeaponTypeDefinitions.ShortswordType.Name)
            .AddToDB();

        var darkelfSkin1 = MorphotypeElementDefinitionBuilder
            .Create(FaceAndSkin_01, "DarkelfSkin1", "92a6831a-883b-4251-b768-5fd368554006")
            .SetMainColor(HairColorBlack.MainColor)
            .SetSortOrder(47)
            .AddToDB();

        var darkelfSkin2 = MorphotypeElementDefinitionBuilder
            .Create(FaceAndSkin_01, "DarkelfSkin2", "d26c8ce0-884f-4abd-90fd-dc961802c48d")
            .SetMainColor(BodyDecorationColor_Default_00.MainColor)
            .SetSortOrder(47)
            .AddToDB();

        var darkelfSkin5 = MorphotypeElementDefinitionBuilder
            .Create(FaceAndSkin_01, "DarkelfSkin5", "cb364149-007a-4b0a-8e4e-40797510343b")
            .SetMainColor(HairColor_40.MainColor)
            .SetSortOrder(47)
            .AddToDB();

        var darkelfHairColor1 = MorphotypeElementDefinitionBuilder
            .Create(HairColorSilver, "DarkelfHair1", "7dd1a932-69d3-4d51-b9c6-eccaaed90007")
            .SetMainColor(FaceAndSkin_Neutral.MainColor)
            .SetSortOrder(1)
            .AddToDB();
        
        //move existing skin at targeted sort order
        FaceAndSkin_20.GuiPresentation.sortOrder = 16;

        //doing a deep copy of Elf did not allow me to set prefered skin colors, neither did creating Darkelf as a subrace of Elf
        var darkelfRacePresentation = new RacePresentation();

        darkelfRacePresentation.bodyAssetPrefix = Elf.RacePresentation.BodyAssetPrefix;
        darkelfRacePresentation.morphotypeAssetPrefix = Elf.RacePresentation.MorphotypeAssetPrefix;
        darkelfRacePresentation.hasSurName = true;
        darkelfRacePresentation.surNameTitle = Elf.RacePresentation.SurNameTitle;
        darkelfRacePresentation.needBeard = false;
        darkelfRacePresentation.canModifyMusculature = true;
        darkelfRacePresentation.originOptions = Elf.RacePresentation.OriginOptions;
        darkelfRacePresentation.maleFaceShapeOptions = Elf.RacePresentation.MaleFaceShapeOptions;
        darkelfRacePresentation.femaleFaceShapeOptions = Elf.RacePresentation.FemaleFaceShapeOptions;
        darkelfRacePresentation.maleHairShapeOptions = Elf.RacePresentation.MaleHairShapeOptions;
        darkelfRacePresentation.femaleHairShapeOptions = Elf.RacePresentation.FemaleHairShapeOptions;
        darkelfRacePresentation.equipmentLayoutPath = Elf.RacePresentation.EquipmentLayoutPath;
        darkelfRacePresentation.maleVoiceDefinition = Elf.RacePresentation.MaleVoiceDefinition;
        darkelfRacePresentation.femaleVoiceDefinition = Elf.RacePresentation.FemaleVoiceDefinition;
        darkelfRacePresentation.portraitShieldOffset = Elf.RacePresentation.PortraitShieldOffset;
        darkelfRacePresentation.preferedSkinColors = new RangedInt(47, 48);
        darkelfRacePresentation.preferedHairColors = new RangedInt(0, 2);

        var darkelf = CharacterRaceDefinitionBuilder
            .Create(ElfHigh, "DarkelfRace", "7f44816c-d076-4513-bf8f-22dc6582f7d5")
            .SetGuiPresentation(Category.Race, darkelfSpriteReference)
            .SetRacePresentation(darkelfRacePresentation)
            .SetFeaturesAtLevel(1,
            FeatureDefinitionMoveModes.MoveModeMove6,
                FeatureDefinitionAttributeModifiers.AttributeModifierElfAbilityScoreIncrease,
                FeatureDefinitionFeatureSets.FeatureSetElfFeyAncestry,
                FeatureDefinitionSenses.SenseNormalVision,
                FeatureDefinitionSenses.SenseDarkvision,
                FeatureDefinitionProficiencys.ProficiencyElfKeenSenses,
                FeatureDefinitionCampAffinitys.CampAffinityElfTrance,
                FeatureDefinitionProficiencys.ProficiencyElfStaticLanguages,
                darkelfAbilityScoreModifierCharisma,
                FeatureDefinitionSenses.SenseSuperiorDarkvision,
                darkelfDarkMagic,
                darkelfWeaponTraining,
                darkelfLightAffinity)
            .AddFeatureAtLevel(darkelfFaerieFirePower, 3)
            .AddFeatureAtLevel(darkelfDarknessPower, 5)
            .AddToDB();

        darkelf.subRaces.Clear();
        darkelf.inventoryDefinition = InventoryDefinitions.HumanoidInventory;
        darkelf.personalityFlagOccurences = Elf.PersonalityFlagOccurences;
        darkelf.languageAutolearnPreference = Elf.LanguageAutolearnPreference;
        darkelf.audioRaceRTPCValue = Elf.AudioRaceRTPCValue;

        return darkelf;
    }
	
}

