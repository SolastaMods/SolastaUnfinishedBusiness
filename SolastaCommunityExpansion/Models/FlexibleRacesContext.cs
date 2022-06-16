using System.Collections.Generic;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;

namespace SolastaCommunityExpansion.Models;

internal static class FlexibleRacesContext
{
    private static readonly FeatureUnlockByLevel attributeChoiceThree = new(
        FeatureDefinitionPointPoolBuilder
            .Create("PointPoolAbilityScore3", "89708d7d-a16a-44a1-b480-733d1ae932a4")
            .SetGuiPresentation(Category.FlexibleRaces)
            .SetPool(HeroDefinitions.PointsPoolType.AbilityScore, 3)
            .AddToDB(),
        1);

    private static readonly FeatureUnlockByLevel attributeChoiceFour = new(
        FeatureDefinitionPointPoolBuilder
            .Create("PointPoolAbilityScore4", "dcdd35a8-f1ca-475a-b5a4-a0426292688c")
            .SetGuiPresentation(Category.FlexibleRaces)
            .SetPool(HeroDefinitions.PointsPoolType.AbilityScore, 4)
            .AddToDB(),
        1);

    private static readonly Dictionary<string, FeatureUnlockByLevel> addedFeatures = new()
    {
        {"Dwarf", attributeChoiceThree},
        {"Elf", attributeChoiceThree},
        {"Halfling", attributeChoiceThree},
        {"HalfElf", attributeChoiceFour},
        {"HalfOrc", attributeChoiceThree},
        // unofficial races
        {"BolgrifRace", attributeChoiceThree},
        {"GnomeRace", attributeChoiceThree}
    };

    private static readonly Dictionary<string, List<string>> removedFeatures = new()
    {
        {"Dwarf", new List<string> {"AttributeModifierDwarfAbilityScoreIncrease"}},
        {"Elf", new List<string> {"AttributeModifierElfAbilityScoreIncrease"}},
        {"Halfling", new List<string> {"AttributeModifierHalflingAbilityScoreIncrease"}},
        {"HalfElf", new List<string> {"FeatureSetHalfElfAbilityScoreIncrease"}},
        {"DwarfHill", new List<string> {"AttributeModifierDwarfHillAbilityScoreIncrease"}},
        {"DwarfSnow", new List<string> {"AttributeModifierDwarfSnowAbilityScoreIncrease"}},
        {"ElfHigh", new List<string> {"AttributeModifierElfHighAbilityScoreIncrease"}},
        {"ElfSylvan", new List<string> {"AttributeModifierElfSylvanAbilityScoreIncrease"}},
        {"HalflingIsland", new List<string> {"AttributeModifierHalflingIslandAbilityScoreIncrease"}},
        {"HalflingMarsh", new List<string> {"AttributeModifierHalflingMarshAbilityScoreIncrease"}},
        {"HalfOrc", new List<string> {"FeatureSetHalfOrcAbilityScoreIncrease"}},
        // unofficial races
        {
            "BolgrifRace",
            new List<string>
            {
                "AttributeModifierBolgrifStrengthAbilityScoreIncrease",
                "AttributeModifierBolgrifWisdomAbilityScoreIncrease"
            }
        },
        {"DarkelfRace", new List<string> {"AttributeModifierDarkelfCharismaAbilityScoreIncrease"}},
        {
            "GnomeRace",
            new List<string>
            {
                "AttributeModifierGnomeAbilityScoreIncrease", "AttributeModifierForestGnomeAbilityScoreIncrease"
            }
        }
    };

    private static void RemoveMatchingFeature(List<FeatureUnlockByLevel> unlocks, FeatureDefinition toRemove)
    {
        unlocks.RemoveAll(u => u.FeatureDefinition.GUID == toRemove.GUID);
    }

    internal static void LateLoad()
    {
        Switch();
    }

    internal static void Switch()
    {
        var enabled = Main.Settings.EnableFlexibleRaces;
        var dbCharacterRaceDefinition = DatabaseRepository.GetDatabase<CharacterRaceDefinition>();
        var dbFeatureDefinition = DatabaseRepository.GetDatabase<FeatureDefinition>();

        foreach (var keyValuePair in addedFeatures)
        {
            var characterRaceDefinition = dbCharacterRaceDefinition.GetElement(keyValuePair.Key, true);

            if (characterRaceDefinition == null)
            {
                continue;
            }

            var exists = characterRaceDefinition.FeatureUnlocks.Exists(x =>
                x.FeatureDefinition == keyValuePair.Value.FeatureDefinition);

            if (!exists && enabled)
            {
                characterRaceDefinition.FeatureUnlocks.Add(keyValuePair.Value);
            }
            else if (exists && !enabled)
            {
                characterRaceDefinition.FeatureUnlocks.Remove(keyValuePair.Value);
            }
        }

        foreach (var keyValuePair in removedFeatures)
        {
            var characterRaceDefinition = dbCharacterRaceDefinition.GetElement(keyValuePair.Key, true);

            if (characterRaceDefinition == null)
            {
                continue;
            }

            foreach (var featureDefinitionName in keyValuePair.Value)
            {
                var featureDefinition = dbFeatureDefinition.GetElement(featureDefinitionName, true);

                if (featureDefinition == null)
                {
                    continue;
                }

                var exists =
                    characterRaceDefinition.FeatureUnlocks.Exists(x => x.FeatureDefinition == featureDefinition);

                if (exists && enabled)
                {
                    RemoveMatchingFeature(characterRaceDefinition.FeatureUnlocks, featureDefinition);
                }
                else if (!exists && !enabled)
                {
                    characterRaceDefinition.FeatureUnlocks.Add(new FeatureUnlockByLevel(featureDefinition, 1));
                }
            }
        }
    }
}
