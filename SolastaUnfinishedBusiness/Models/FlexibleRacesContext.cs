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
        { "RaceBolgrif", AttributeChoiceThree },
        { "RaceHalfElfVariant", AttributeChoiceFour },
        { "RaceImp", AttributeChoiceThree },
        { "RaceKobold", AttributeChoiceThree },
        { "RaceMalakh", AttributeChoiceThree },
        { "RaceFairy", AttributeChoiceThree },
        { "RaceOligath", AttributeChoiceThree },
        { "RaceWendigo", AttributeChoiceThree },
        { "RaceTiefling", AttributeChoiceThree },
        { "RaceWyrmkin", AttributeChoiceThree }
    };

    private static readonly Dictionary<string, List<string>> RemovedFeatures = new()
    {
        { "Dragonborn", new List<string> { "FeatureSetDragonbornAbilityScoreIncrease" } },
        { "Dwarf", new List<string> { "AttributeModifierDwarfAbilityScoreIncrease" } },
        { "Elf", new List<string> { "AttributeModifierElfAbilityScoreIncrease" } },
        { "Gnome", new List<string> { "AttributeModifierGnomeAbilityScoreIncreaseInt" } },
        { "GnomeRock", new List<string> { "AttributeModifierGnomeRockAbilityScoreIncreaseCon" } },
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
        { "RaceMalakh", new List<string> { "FeatureSetMalakhAbilityScoreIncrease" } },
        { "RaceImpInfernal", new List<string> { "FeatureSetImpInfernalAbilityScoreIncrease" } },
        { "RaceImpForest", new List<string> { "FeatureSetImpForestAbilityScoreIncrease" } },
        { "RaceIronbornDwarf", new List<string> { "AttributeModifierIronbornDwarfStrengthAbilityScoreIncrease" } },
        { "RaceObsidianDwarf", new List<string> { "AttributeModifierObsidianDwarfStrengthAbilityScoreIncrease" } },
        {
            "RaceOligath",
            new List<string>
            {
                "AttributeModifierOligathStrengthAbilityScoreIncrease",
                "AttributeModifierOligathConstitutionAbilityScoreIncrease"
            }
        },
        {
            "RaceWendigo",
            new List<string>
            {
                "AttributeModifierWendigoStrengthAbilityScoreIncrease",
                "AttributeModifierWendigoDexterityAbilityScoreIncrease"
            }
        },
        {
            "RaceCaveWyrmkin",
            new List<string>
            {
                "AttributeModifierCaveWyrmkinConstitutionAbilityScoreIncrease",
                "AttributeModifierCaveWyrmkinStrengthAbilityScoreIncrease"
            }
        },
        {
            "RaceHighWyrmkin",
            new List<string>
            {
                "AttributeModifierHighWyrmkinIntelligenceAbilityScoreIncrease",
                "AttributeModifierHighWyrmkinStrengthAbilityScoreIncrease"
            }
        },
        { "RaceCrystalWyrmkin", new List<string> { "FeatureSetCrystalWyrmkinAbilityScoreIncrease" } },
        { "RaceTiefling", new List<string> { "AttributeModifierTieflingAbilityScoreIncreaseCha" } },
        { "RaceTieflingDevilTongue", new List<string> { "AttributeModifierTieflingIntelligenceAbilityScoreIncrease" } },
        { "RaceTieflingFeral", new List<string> { "AttributeModifierTieflingDexterityAbilityScoreIncrease" } },
        { "RaceTieflingMephistopheles", new List<string> { "AttributeModifierTieflingDexterityAbilityScoreIncrease" } },
        { "RaceTieflingZariel", new List<string> { "AttributeModifierTieflingStrengthAbilityScoreIncrease" } }
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
