using System.Collections.Generic;
using SolastaCommunityExpansion.Api.Infrastructure;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaCommunityExpansion.Models;
using SolastaCommunityExpansion.Utils;
using TA;
using static SolastaCommunityExpansion.Api.DatabaseHelper;
using static SolastaCommunityExpansion.Api.DatabaseHelper.CharacterRaceDefinitions;

using Resources = SolastaCommunityExpansion.Properties.Resources;

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
        darkElfPerception.AffinityGroups[0].lightingContext = RuleDefinitions.LightingContext.BrightLight;

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
            .Create(FeatureDefinitionLightAffinitys.LightAffinityLightSensitivity,
                "LightAffinityDarkelfLightSensitivity", "707231ea-e34d-4e26-9af8-4e52c0cb85c3")
            .SetGuiPresentation(Category.Feature)
            .AddLightingEffectAndCondition(darkelfLightingEffectAndCondition)
            .AddToDB();

        var darkelfDarkelfMagicSpellList = SpellListDefinitionBuilder
            .Create(SpellListDefinitions.SpellListWizard, "DarkelfMagicSpellList",
                "7e84092e-8b26-4870-8244-ce435a95b67f")
            .SetGuiPresentationNoContent()
            .SetSpellsAtLevel(0, SpellDefinitions.DancingLights)
            .FinalizeSpells()
            .AddToDB();

        var darkelfDarkMagic = FeatureDefinitionCastSpellBuilder
            .Create(FeatureDefinitionCastSpells.CastSpellElfHigh, "DarkelfMagic",
                "0271c652-b4aa-4346-806d-9711e634271b")
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

        darkelfFaerieFirePower.EffectDescription.savingThrowDifficultyAbility = AttributeDefinitions.Charisma;

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

        darkelfDarknessPower.EffectDescription.savingThrowDifficultyAbility = AttributeDefinitions.Charisma;

        var darkelfWeaponTraining = FeatureDefinitionProficiencyBuilder
            .Create("DarkelfWeaponTraining", "ec6e4a4a-5635-4378-a370-5a5ab7dab2ea")
            .SetGuiPresentation(Category.Feature)
            .SetProficiencies(RuleDefinitions.ProficiencyType.Weapon,
                CustomWeaponsContext.HandXbowWeaponType.Name,
                WeaponTypeDefinitions.RapierType.Name,
                WeaponTypeDefinitions.ShortswordType.Name)
            .AddToDB();

        var darkelfRacePresentation = Elf.RacePresentation.DeepCopy();

        darkelfRacePresentation.surNameOptions = new List<string>
        {
            "Race/&DarkelfSurName1Title",
            "Race/&DarkelfSurName2Title",
            "Race/&DarkelfSurName3Title",
            "Race/&DarkelfSurName4Title",
            "Race/&DarkelfSurName5Title"
        };

        darkelfRacePresentation.femaleNameOptions = ElfHigh.RacePresentation.FemaleNameOptions;
        darkelfRacePresentation.maleNameOptions = ElfHigh.RacePresentation.MaleNameOptions;
        darkelfRacePresentation.preferedSkinColors = new RangedInt(48, 53);
        darkelfRacePresentation.preferedHairColors = new RangedInt(48, 53);

        var darkelf = CharacterRaceDefinitionBuilder
            .Create(ElfHigh, "DarkelfRace", "7f44816c-d076-4513-bf8f-22dc6582f7d5")
            .SetGuiPresentation(Category.Race, darkelfSpriteReference)
            .SetRacePresentation(darkelfRacePresentation)
            .SetFeaturesAtLevel(1,
                FeatureDefinitionMoveModes.MoveModeMove6,
                FeatureDefinitionSenses.SenseSuperiorDarkvision,
                FeatureDefinitionFeatureSets.FeatureSetElfHighLanguages,
                darkelfAbilityScoreModifierCharisma,
                darkelfWeaponTraining,
                darkelfDarkMagic,
                darkelfLightAffinity)
            .AddFeaturesAtLevel(3, darkelfFaerieFirePower)
            .AddFeaturesAtLevel(5, darkelfDarknessPower)
            .AddToDB();

        darkelf.subRaces.Clear();
        Elf.SubRaces.Add(darkelf);

        return darkelf;
    }
}
