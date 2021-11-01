using SolastaModApi;
using SolastaModApi.BuilderHelpers;
using System.Collections.Generic;
using static SolastaModApi.DatabaseHelper.CharacterRaceDefinitions;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionAttributeModifiers;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionFeatureSets;

namespace SolastaContentExpansion.Models
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

        private static readonly Dictionary<CharacterRaceDefinition, FeatureUnlockByLevel> addedFeatures = new Dictionary<CharacterRaceDefinition, FeatureUnlockByLevel>
        {
            { Dwarf, attributeChoiceThree },
            { Elf, attributeChoiceThree },
            { Halfling, attributeChoiceThree },
            { HalfElf, attributeChoiceFour },
            // TODO- verify this doesn't break for folks missing the dlc
            { HalfOrc, attributeChoiceThree },
            // TODO - look into support for user races (like Gnome)
        };

        private static readonly Dictionary<CharacterRaceDefinition, FeatureDefinition> removedFeatures = new Dictionary<CharacterRaceDefinition, FeatureDefinition>
        {
            { Dwarf, AttributeModifierDwarfAbilityScoreIncrease },
            { Elf, AttributeModifierElfAbilityScoreIncrease },
            { Halfling, AttributeModifierHalflingAbilityScoreIncrease },
            { HalfElf, FeatureSetHalfElfAbilityScoreIncrease },
            { DwarfHill, AttributeModifierDwarfHillAbilityScoreIncrease },
            { DwarfSnow, AttributeModifierDwarfSnowAbilityScoreIncrease },
            { ElfHigh, AttributeModifierElfHighAbilityScoreIncrease },
            { ElfSylvan, AttributeModifierElfSylvanAbilityScoreIncrease },
            { HalflingIsland, AttributeModifierHalflingIslandAbilityScoreIncrease },
            { HalflingMarsh, AttributeModifierHalflingMarshAbilityScoreIncrease },
            { HalfOrc, FeatureSetHalfOrcAbilityScoreIncrease},
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

        internal static void Switch(bool enabled)
        {
            foreach (var keyValuePair in addedFeatures)
            {
                var exists = keyValuePair.Key.FeatureUnlocks.Exists(x => x.FeatureDefinition == keyValuePair.Value.FeatureDefinition);

                if (!exists && enabled)
                {
                    keyValuePair.Key.FeatureUnlocks.Add(keyValuePair.Value);
                }
                else if (exists && !enabled)
                {
                   keyValuePair.Key.FeatureUnlocks.Remove(keyValuePair.Value);
                }
            }

            foreach (var keyValuePair in removedFeatures)
            {
                var exists = keyValuePair.Key.FeatureUnlocks.Exists(x => x.FeatureDefinition == keyValuePair.Value);

                if (exists && enabled)
                {
                    RemoveMatchingFeature(keyValuePair.Key.FeatureUnlocks, keyValuePair.Value);
                }
                else if (!exists && !enabled)
                {
                    keyValuePair.Key.FeatureUnlocks.Add(new FeatureUnlockByLevel(keyValuePair.Value, 1));
                }
            }
        }

        internal static void Load()
        {
            Switch(Main.Settings.EnableFlexibleRaces);
        }
    }
}
