using SolastaModApi;
using SolastaModApi.BuilderHelpers;
using System.Collections.Generic;

namespace SolastaCommunityExpansion.Models
{
    internal static class FlexibleRacesContext
    {
        private static readonly GuiPresentationBuilder attributeThreeGui = new GuiPresentationBuilder(
            "FlexibleRaces/&PointPoolAbilityScore3Description",
            "FlexibleRaces/&PointPoolAbilityScore3Title");

        private static readonly FeatureUnlockByLevel attributeChoiceThree = new FeatureUnlockByLevel(new FeatureDefinitionPointPoolBuilder("PointPoolAbilityScore3",
            "89708d7d-a16a-44a1-b480-733d1ae932a4", HeroDefinitions.PointsPoolType.AbilityScore, 3, attributeThreeGui.Build()).AddToDB(), 1);

        private static readonly GuiPresentationBuilder attributeFourGui = new GuiPresentationBuilder(
            "FlexibleRaces/&PointPoolAbilityScore4Description",
            "FlexibleRaces/&PointPoolAbilityScore4Title");

        private static readonly FeatureUnlockByLevel attributeChoiceFour = new FeatureUnlockByLevel(new FeatureDefinitionPointPoolBuilder("PointPoolAbilityScore4",
            "dcdd35a8-f1ca-475a-b5a4-a0426292688c", HeroDefinitions.PointsPoolType.AbilityScore, 4, attributeFourGui.Build()).AddToDB(), 1);

        private static readonly Dictionary<string, FeatureUnlockByLevel> addedFeatures = new Dictionary<string, FeatureUnlockByLevel>
        {
            { "Dwarf", attributeChoiceThree },
            { "Elf", attributeChoiceThree },
            { "Halfling", attributeChoiceThree },
            { "HalfElf", attributeChoiceFour },
            { "HalfOrc", attributeChoiceThree },
            // unofficial races
            { "FirbolgRace", attributeChoiceThree },
            { "GnomeRace", attributeChoiceThree }
        };

        private static readonly Dictionary<string, List<string>> removedFeatures = new Dictionary<string, List<string>>
        {
            { "Dwarf", new List<string> { "AttributeModifierDwarfAbilityScoreIncrease" } },
            { "Elf", new List<string> { "AttributeModifierElfAbilityScoreIncrease" } },
            { "Halfling", new List<string> { "AttributeModifierHalflingAbilityScoreIncrease" } },
            { "HalfElf", new List<string> { "FeatureSetHalfElfAbilityScoreIncrease" } },
            { "DwarfHill", new List<string> { "AttributeModifierDwarfHillAbilityScoreIncrease" } },
            { "DwarfSnow", new List<string> { "AttributeModifierDwarfSnowAbilityScoreIncrease" } },
            { "ElfHigh", new List<string> { "AttributeModifierElfHighAbilityScoreIncrease" } },
            { "ElfSylvan", new List<string> { "AttributeModifierElfSylvanAbilityScoreIncrease" } },
            { "HalflingIsland", new List<string> { "AttributeModifierHalflingIslandAbilityScoreIncrease" } },
            { "HalflingMarsh", new List<string> { "AttributeModifierHalflingMarshAbilityScoreIncrease" } },
            { "HalfOrc", new List<string> { "FeatureSetHalfOrcAbilityScoreIncrease" } },
            // unofficial races
            { "FirbolgRace", new List<string> { "AttributeModifierFirbolgStrengthAbilityScoreIncrease", "AttributeModifierFirbolgWisdomAbilityScoreIncrease" } },
            { "Gnome", new List<string> { "AttributeModifierGnomeAbilityScoreIncrease", "AttributeModifierForestGnomeAbilityScoreIncrease" } }
        };

        private static void RemoveMatchingFeature(List<FeatureUnlockByLevel> unlocks, FeatureDefinition toRemove)
        {
            for (int i = 0; i < unlocks.Count; i++)
            {
                if (unlocks[i].FeatureDefinition.GUID == toRemove.GUID)
                {
                    unlocks.RemoveAt(i);
                }
            }
        }

        internal static void SwitchFlexibleRaces()
        {
            var enabled = Main.Settings.EnableFlexibleRaces;
            var dbCharacterRaceDefinition = DatabaseRepository.GetDatabase<CharacterRaceDefinition>();
            var dbFeatureDefinition = DatabaseRepository.GetDatabase<FeatureDefinition>();

            foreach (var keyValuePair in addedFeatures)
            {
                var characterClassDefinition = dbCharacterRaceDefinition.GetElement(keyValuePair.Key, true);

                if (characterClassDefinition == null)
                {
                    Main.Log($"Race {keyValuePair.Key} not loaded.");

                    continue;
                }

                var exists = characterClassDefinition.FeatureUnlocks.Exists(x => x.FeatureDefinition == keyValuePair.Value.FeatureDefinition);

                if (!exists && enabled)
                {
                    characterClassDefinition.FeatureUnlocks.Add(keyValuePair.Value);
                }
                else if (exists && !enabled)
                {
                    characterClassDefinition.FeatureUnlocks.Remove(keyValuePair.Value);
                }
            }

            foreach (var keyValuePair in removedFeatures)
            {
                var characterClassDefinition = dbCharacterRaceDefinition.GetElement(keyValuePair.Key, true);

                if (characterClassDefinition == null)
                {
                    Main.Log($"Race {keyValuePair.Key} not loaded.");

                    continue;
                }

                foreach (var featureDefinitionName in keyValuePair.Value)
                {
                    var featureDefinition = dbFeatureDefinition.GetElement(featureDefinitionName, true);

                    if (featureDefinition == null)
                    {
                        Main.Log($"Feature {keyValuePair.Value} not loaded.");

                        continue;
                    }

                    var exists = characterClassDefinition.FeatureUnlocks.Exists(x => x.FeatureDefinition == featureDefinition);

                    if (exists && enabled)
                    {
                        RemoveMatchingFeature(characterClassDefinition.FeatureUnlocks, featureDefinition);
                    }
                    else if (!exists && !enabled)
                    {
                        characterClassDefinition.FeatureUnlocks.Add(new FeatureUnlockByLevel(featureDefinition, 1));
                    }
                }
            }
        }
    }
}
