using SolastaModApi;
using SolastaModApi.Extensions;
using System.Collections.Generic;
using static SolastaModApi.DatabaseHelper.CharacterRaceDefinitions;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionAttributeModifiers;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionFeatureSets;

namespace SolastaContentExpansion.Models
{
    internal static class FlexibleRacesContext
    {
        public class AbilityScoreSelectBuilder : BaseDefinitionBuilder<FeatureDefinitionPointPool>
        {
            public AbilityScoreSelectBuilder(string name, string guid, int pointPoolSize, GuiPresentation guiPresentation) : base(name, guid)
            {
                Definition.SetPoolAmount(pointPoolSize);
                Definition.SetPoolType(HeroDefinitions.PointsPoolType.AbilityScore);
                Definition.SetGuiPresentation(guiPresentation);
            }
        }

        private static readonly GuiPresentationBuilder attributeThreeGui = new GuiPresentationBuilder(
            "FlexibleRaces/&PointPoolAbilityScore3Description",
            "FlexibleRaces/&PointPoolAbilityScore3Title");

        private static readonly FeatureUnlockByLevel attributeChoiceThree = new FeatureUnlockByLevel(new AbilityScoreSelectBuilder("PointPoolAbilityScore3",
            "89708d7d-a16a-44a1-b480-733d1ae932a4", 3, attributeThreeGui.Build()).AddToDB(), 1);

        private static readonly GuiPresentationBuilder attributeFourGui = new GuiPresentationBuilder(
            "FlexibleRaces/&PointPoolAbilityScore4Description",
            "FlexibleRaces/&PointPoolAbilityScore4Title");

        private static readonly FeatureUnlockByLevel attributeChoiceFour = new FeatureUnlockByLevel(new AbilityScoreSelectBuilder("PointPoolAbilityScore4",
            "dcdd35a8-f1ca-475a-b5a4-a0426292688c", 4, attributeFourGui.Build()).AddToDB(), 1);

        private static readonly Dictionary<CharacterRaceDefinition, FeatureUnlockByLevel> addedFeatures = new Dictionary<CharacterRaceDefinition, FeatureUnlockByLevel>
        {
            { Dwarf, attributeChoiceThree },
            { Elf, attributeChoiceThree },
            { Halfling, attributeChoiceThree },
            { HalfElf, attributeChoiceFour },
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

        };

        internal static void ModEntryPoint()
        {
            RemoveMatchingFeature(DatabaseHelper.CharacterRaceDefinitions.Dwarf.FeatureUnlocks, DatabaseHelper.FeatureDefinitionAttributeModifiers.AttributeModifierDwarfAbilityScoreIncrease);
            RemoveMatchingFeature(DatabaseHelper.CharacterRaceDefinitions.Elf.FeatureUnlocks, DatabaseHelper.FeatureDefinitionAttributeModifiers.AttributeModifierElfAbilityScoreIncrease);
            RemoveMatchingFeature(DatabaseHelper.CharacterRaceDefinitions.Halfling.FeatureUnlocks, DatabaseHelper.FeatureDefinitionAttributeModifiers.AttributeModifierHalflingAbilityScoreIncrease);
            RemoveMatchingFeature(DatabaseHelper.CharacterRaceDefinitions.DwarfHill.FeatureUnlocks, DatabaseHelper.FeatureDefinitionAttributeModifiers.AttributeModifierDwarfHillAbilityScoreIncrease);
            RemoveMatchingFeature(DatabaseHelper.CharacterRaceDefinitions.DwarfSnow.FeatureUnlocks, DatabaseHelper.FeatureDefinitionAttributeModifiers.AttributeModifierDwarfSnowAbilityScoreIncrease);
            RemoveMatchingFeature(DatabaseHelper.CharacterRaceDefinitions.ElfHigh.FeatureUnlocks, DatabaseHelper.FeatureDefinitionAttributeModifiers.AttributeModifierElfHighAbilityScoreIncrease);
            RemoveMatchingFeature(DatabaseHelper.CharacterRaceDefinitions.ElfSylvan.FeatureUnlocks, DatabaseHelper.FeatureDefinitionAttributeModifiers.AttributeModifierElfSylvanAbilityScoreIncrease);
            RemoveMatchingFeature(DatabaseHelper.CharacterRaceDefinitions.HalflingIsland.FeatureUnlocks, DatabaseHelper.FeatureDefinitionAttributeModifiers.AttributeModifierHalflingIslandAbilityScoreIncrease);
            RemoveMatchingFeature(DatabaseHelper.CharacterRaceDefinitions.HalflingMarsh.FeatureUnlocks, DatabaseHelper.FeatureDefinitionAttributeModifiers.AttributeModifierHalflingMarshAbilityScoreIncrease);
            RemoveMatchingFeature(DatabaseHelper.CharacterRaceDefinitions.HalfElf.FeatureUnlocks, DatabaseHelper.FeatureDefinitionFeatureSets.FeatureSetHalfElfAbilityScoreIncrease);

            DatabaseHelper.CharacterRaceDefinitions.Dwarf.FeatureUnlocks.Add(attributeChoiceThree);
            DatabaseHelper.CharacterRaceDefinitions.Elf.FeatureUnlocks.Add(attributeChoiceThree);
            DatabaseHelper.CharacterRaceDefinitions.HalfElf.FeatureUnlocks.Add(attributeChoiceFour);
            DatabaseHelper.CharacterRaceDefinitions.Halfling.FeatureUnlocks.Add(attributeChoiceThree);
        }

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