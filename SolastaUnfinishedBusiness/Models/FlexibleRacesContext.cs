using System.Collections.Generic;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;

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
        { "RaceBolgrif", AttributeChoiceThree },
        { "RaceHalfElfVariant", AttributeChoiceFour },
        { "RaceKobold", AttributeChoiceThree },
        { "RaceFairy", AttributeChoiceThree },
        { "RaceTiefling", AttributeChoiceThree }
    };

    private static readonly Dictionary<string, List<string>> RemovedFeatures = new()
    {
        { "Dragonborn", new List<string> { "FeatureSetDragonbornAbilityScoreIncrease" } },
        { "Dwarf", new List<string> { "AttributeModifierDwarfAbilityScoreIncrease" } },
        { "Elf", new List<string> { "AttributeModifierElfAbilityScoreIncrease" } },
        { "Gnome", new List<string> { "AttributeModifierGnomeAbilityScoreIncreaseInt" } },
        {
            "GnomeRock",
            new List<string>
            {
                "AttributeModifierGnomeRockAbilityScoreIncreaseCon",
                "AttributeModifierGnomeShadowAbilityScoreIncreaseDex" //BUGFIX: until TA doesn't fix Gnome Rock
            }
        },
        { "GnomeShadow", new List<string> { "AttributeModifierGnomeShadowAbilityScoreIncreaseDex" } },
        { "Halfling", new List<string> { "AttributeModifierHalflingAbilityScoreIncrease" } },
        { "HalfElf", new List<string> { "FeatureSetHalfElfAbilityScoreIncrease" } },
        { "DwarfHill", new List<string> { "AttributeModifierDwarfHillAbilityScoreIncrease" } },
        { "DwarfSnow", new List<string> { "AttributeModifierDwarfSnowAbilityScoreIncrease" } },
        { "ElfHigh", new List<string> { "AttributeModifierElfHighAbilityScoreIncrease" } },
        { "ElfSylvan", new List<string> { "AttributeModifierElfSylvanAbilityScoreIncrease" } },
        { "HalflingIsland", new List<string> { "AttributeModifierHalflingIslandAbilityScoreIncrease" } },
        { "HalflingMarsh", new List<string> { "AttributeModifierHalflingMarshAbilityScoreIncrease" } },
        { "HalfOrc", new List<string> { "FeatureSetHalfOrcAbilityScoreIncrease" } },
        {
            "Tiefling",
            new List<string>
            {
                "AttributeModifierTieflingAbilityScoreIncreaseCha",
                "AttributeModifierTieflingAbilityScoreIncreaseInt"
            }
        },
        // unofficial races
        {
            "RaceBolgrif",
            new List<string>
            {
                "AttributeModifierBolgrifStrengthAbilityScoreIncrease",
                "AttributeModifierBolgrifWisdomAbilityScoreIncrease"
            }
        },
        { "RaceDarkelf", new List<string> { "AttributeModifierDarkelfCharismaAbilityScoreIncrease" } },
        { "RaceHalfElfVariant", new List<string> { "FeatureSetHalfElfAbilityScoreIncrease" } },
        { "RaceGrayDwarf", new List<string> { "AttributeModifierGrayDwarfStrengthAbilityScoreIncrease" } },
        { "RaceDarkKobold", new List<string> { "AttributeModifierElfAbilityScoreIncrease" } },
        { "RaceDraconicKobold", new List<string> { "PointPoolDraconicKoboldAbilityScoreIncrease" } },
        { "RaceFairy", new List<string> { "FeatureSetFairyAbilityScoreIncrease" } },
        { "RaceTiefling", new List<string> { "AttributeModifierTieflingAbilityScoreIncreaseCha" } },
        { "RaceTieflingAsmodeus", new List<string> { "AttributeModifierTieflingIntelligenceAbilityScoreIncrease" } },
        { "RaceTieflingMephistopheles", new List<string> { "AttributeModifierTieflingDexterityAbilityScoreIncrease" } },
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

            if (characterRaceDefinition == null)
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

            if (characterRaceDefinition == null)
            {
                continue;
            }

            foreach (var featureDefinitionName in keyValuePair.Value)
            {
                if (!DatabaseHelper.TryGetDefinition<FeatureDefinition>(featureDefinitionName,
                        out var featureDefinition))
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
