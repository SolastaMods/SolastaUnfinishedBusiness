using System.Collections.Generic;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;

namespace SolastaUnfinishedBusiness.Models;

internal static class FlexibleRacesContext
{
    internal static readonly FeatureUnlockByLevel AttributeChoiceThree = new(
        FeatureDefinitionPointPoolBuilder
            .Create("PointPoolAbilityScore3")
            .SetGuiPresentation(Category.Feature)
            .SetPool(HeroDefinitions.PointsPoolType.AbilityScore, 3)
            .AddToDB(),
        1);

    private static readonly FeatureUnlockByLevel AttributeChoiceFour = new(
        FeatureDefinitionPointPoolBuilder
            .Create("PointPoolAbilityScore4")
            .SetGuiPresentation(Category.Feature)
            .SetPool(HeroDefinitions.PointsPoolType.AbilityScore, 4)
            .AddToDB(),
        1);

    private static readonly FeatureDefinitionPointPool PointPoolLanguageChoiceOne =
        FeatureDefinitionPointPoolBuilder
            .Create("PointPoolLanguageChoice_one")
            .SetGuiPresentationNoContent(true)
            .SetPool(HeroDefinitions.PointsPoolType.Language, 1)
            .RestrictChoices(
                "Language_Gnomish",
                "Language_Infernal",
                "Language_Dwarvish",
                "Language_Halfling",
                "Language_Orc",
                "Language_Goblin",
                "Language_Giant",
                "Language_Terran",
                "Language_Tirmarian",
                "Language_Elvish",
                "Language_Draconic")
            .AddToDB();

    internal static readonly FeatureDefinitionFeatureSet FeatureSetLanguageCommonPlusOne
        = FeatureDefinitionFeatureSetBuilder
            .Create(FeatureDefinitionFeatureSets.FeatureSetHumanLanguages, "FeatureSetLanguageCommonPlusOne")
            .SetFeatureSet(
                FeatureDefinitionProficiencys.ProficiencyHumanStaticLanguages,
                PointPoolLanguageChoiceOne)
            .AddToDB();

    private static readonly Dictionary<string, FeatureUnlockByLevel> AddedFeatures = new()
    {
        { "Dragonborn", AttributeChoiceThree },
        { "Dwarf", AttributeChoiceThree },
        { "Elf", AttributeChoiceThree },
        { "Gnome", AttributeChoiceThree },
        { "Halfling", AttributeChoiceThree },
        { "HalfElf", AttributeChoiceFour },
        { "HalfOrc", AttributeChoiceThree },
        { "Tiefling", AttributeChoiceThree },
        // unofficial races
        { "RaceBattleborn", AttributeChoiceThree },
        { "RaceBolgrif", AttributeChoiceThree },
        { "RaceHalfElfVariant", AttributeChoiceFour },
        { "RaceImp", AttributeChoiceThree },
        { "RaceKobold", AttributeChoiceThree },
        { "RaceMalakh", AttributeChoiceThree },
        { "RaceFairy", AttributeChoiceThree },
        { "RaceOligath", AttributeChoiceThree },
        { "RaceWendigo", AttributeChoiceThree },
        { "RaceTiefling", AttributeChoiceThree },
        { "RaceWyrmkin", AttributeChoiceThree },
        { "RaceWildling", AttributeChoiceThree },
        { "RaceOni", AttributeChoiceThree }
    };

    private static readonly Dictionary<string, List<string>> RemovedFeatures = new()
    {
        { "Dragonborn", ["FeatureSetDragonbornAbilityScoreIncrease"] },
        { "Dwarf", ["AttributeModifierDwarfAbilityScoreIncrease"] },
        { "Elf", ["AttributeModifierElfAbilityScoreIncrease"] },
        { "Gnome", ["AttributeModifierGnomeAbilityScoreIncreaseInt"] },
        { "GnomeRock", ["AttributeModifierGnomeRockAbilityScoreIncreaseCon"] },
        { "GnomeShadow", ["AttributeModifierGnomeShadowAbilityScoreIncreaseDex"] },
        { "Halfling", ["AttributeModifierHalflingAbilityScoreIncrease"] },
        { "HalfElf", ["FeatureSetHalfElfAbilityScoreIncrease"] },
        { "DwarfHill", ["AttributeModifierDwarfHillAbilityScoreIncrease"] },
        { "DwarfSnow", ["AttributeModifierDwarfSnowAbilityScoreIncrease"] },
        { "ElfHigh", ["AttributeModifierElfHighAbilityScoreIncrease"] },
        { "ElfSylvan", ["AttributeModifierElfSylvanAbilityScoreIncrease"] },
        { "HalflingIsland", ["AttributeModifierHalflingIslandAbilityScoreIncrease"] },
        { "HalflingMarsh", ["AttributeModifierHalflingMarshAbilityScoreIncrease"] },
        { "HalfOrc", ["FeatureSetHalfOrcAbilityScoreIncrease"] },
        {
            "Tiefling", [
                "AttributeModifierTieflingAbilityScoreIncreaseCha",
                "AttributeModifierTieflingAbilityScoreIncreaseInt"
            ]
        },
        // unofficial races
        {
            "RaceBolgrif", [
                "AttributeModifierBolgrifStrengthAbilityScoreIncrease",
                "AttributeModifierBolgrifWisdomAbilityScoreIncrease"
            ]
        },
        { "RaceDarkelf", ["AttributeModifierDarkelfCharismaAbilityScoreIncrease"] },
        { "RaceHalfElfVariant", ["FeatureSetHalfElfAbilityScoreIncrease"] },
        { "RaceGrayDwarf", ["AttributeModifierGrayDwarfStrengthAbilityScoreIncrease"] },
        { "RaceDarkKobold", ["AttributeModifierElfAbilityScoreIncrease"] },
        { "RaceDraconicKobold", ["PointPoolDraconicKoboldAbilityScoreIncrease"] },
        { "RaceFairy", ["FeatureSetFairyAbilityScoreIncrease"] },
        { "RaceMalakh", ["FeatureSetMalakhAbilityScoreIncrease"] },
        { "RaceWildling", ["FeatureSetWildlingAbilityScoreIncrease"] },
        { "RaceBattleborn", ["FeatureSetBattlebornAbilityScoreIncrease"] },
        { "RaceOni", ["FeatureSetOniAbilityScoreIncrease"] },
        { "RaceImpInfernal", ["FeatureSetImpInfernalAbilityScoreIncrease"] },
        { "RaceImpForest", ["FeatureSetImpForestAbilityScoreIncrease"] },
        { "RaceIronbornDwarf", ["AttributeModifierIronbornDwarfStrengthAbilityScoreIncrease"] },
        { "RaceObsidianDwarf", ["AttributeModifierObsidianDwarfStrengthAbilityScoreIncrease"] },
        {
            "RaceOligath", [
                "AttributeModifierOligathStrengthAbilityScoreIncrease",
                "AttributeModifierOligathConstitutionAbilityScoreIncrease"
            ]
        },
        {
            "RaceWendigo", [
                "AttributeModifierWendigoStrengthAbilityScoreIncrease",
                "AttributeModifierWendigoDexterityAbilityScoreIncrease"
            ]
        },
        {
            "RaceCaveWyrmkin", [
                "AttributeModifierCaveWyrmkinConstitutionAbilityScoreIncrease",
                "AttributeModifierCaveWyrmkinStrengthAbilityScoreIncrease"
            ]
        },
        {
            "RaceHighWyrmkin", [
                "AttributeModifierHighWyrmkinIntelligenceAbilityScoreIncrease",
                "AttributeModifierHighWyrmkinStrengthAbilityScoreIncrease"
            ]
        },
        { "RaceCrystalWyrmkin", ["FeatureSetCrystalWyrmkinAbilityScoreIncrease"] },
        { "RaceTiefling", ["AttributeModifierTieflingAbilityScoreIncreaseCha"] },
        { "RaceTieflingDevilTongue", ["AttributeModifierTieflingIntelligenceAbilityScoreIncrease"] },
        { "RaceTieflingFeral", ["AttributeModifierTieflingDexterityAbilityScoreIncrease"] },
        { "RaceTieflingMephistopheles", ["AttributeModifierTieflingDexterityAbilityScoreIncrease"] },
        { "RaceTieflingZariel", ["AttributeModifierTieflingStrengthAbilityScoreIncrease"] }
    };

    private static void RemoveMatchingFeature([NotNull] List<FeatureUnlockByLevel> unlocks, BaseDefinition toRemove)
    {
        unlocks.RemoveAll(u => u.FeatureDefinition.GUID == toRemove.GUID);
    }

    internal static void SwitchFlexibleRaces()
    {
        var enabled = Main.Settings.EnableFlexibleRaces;
        var dbCharacterRaceDefinition = DatabaseRepository.GetDatabase<CharacterRaceDefinition>();

        foreach (var keyValuePair in AddedFeatures)
        {
            var characterRaceDefinition = dbCharacterRaceDefinition.GetElement(keyValuePair.Key, true);

            if (!characterRaceDefinition)
            {
                continue;
            }

            var exists = characterRaceDefinition.FeatureUnlocks.Exists(x =>
                x.FeatureDefinition == keyValuePair.Value.FeatureDefinition);

            switch (exists)
            {
                case false when enabled:
                    characterRaceDefinition.FeatureUnlocks.Add(keyValuePair.Value);
                    break;
                case true when !enabled:
                    characterRaceDefinition.FeatureUnlocks.Remove(keyValuePair.Value);
                    break;
            }
        }

        foreach (var keyValuePair in RemovedFeatures)
        {
            var characterRaceDefinition = dbCharacterRaceDefinition.GetElement(keyValuePair.Key, true);

            if (!characterRaceDefinition)
            {
                continue;
            }

            foreach (var featureDefinitionName in keyValuePair.Value)
            {
                if (!TryGetDefinition<FeatureDefinition>(featureDefinitionName, out var featureDefinition))
                {
                    continue;
                }

                var exists =
                    characterRaceDefinition.FeatureUnlocks.Exists(x => x.FeatureDefinition == featureDefinition);

                switch (exists)
                {
                    case true when enabled:
                        RemoveMatchingFeature(characterRaceDefinition.FeatureUnlocks, featureDefinition);
                        break;
                    case false when !enabled:
                        characterRaceDefinition.FeatureUnlocks.Add(new FeatureUnlockByLevel(featureDefinition, 1));
                        break;
                }
            }
        }
    }
}
