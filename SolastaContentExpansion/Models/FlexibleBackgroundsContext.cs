using SolastaModApi;
using SolastaModApi.BuilderHelpers;
using System.Collections.Generic;
using static SolastaModApi.DatabaseHelper.CharacterBackgroundDefinitions;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionProficiencys;

namespace SolastaContentExpansion.Models
{
    public class FlexibleBackgroundsContext
    {
        private static readonly GuiPresentationBuilder skillThreeGui = new GuiPresentationBuilder(
            "FlexibleBackgrounds/&BackgroundSkillSelect3Description",
            "FlexibleBackgrounds/&BackgroundSkillSelect3Title");

        private static readonly FeatureDefinition skillThree = new FeatureDefinitionPointPoolBuilder("BackgroundSkillSelect3",
            "e6f2ed65-a44e-4314-b38c-393abb4ad900", HeroDefinitions.PointsPoolType.Skill, 3, skillThreeGui.Build()).AddToDB();

        private static readonly GuiPresentationBuilder skillTwoGui = new GuiPresentationBuilder(
            "FlexibleBackgrounds/&BackgroundSkillSelect2Description",
            "FlexibleBackgrounds/&BackgroundSkillSelect2Title");

        private static readonly FeatureDefinition skillTwo = new FeatureDefinitionPointPoolBuilder("BackgroundSkillSelect2",
            "77d6eb2c-d99f-4256-9bb6-c6395e440629", HeroDefinitions.PointsPoolType.Skill, 2, skillTwoGui.Build()).AddToDB();

        private static readonly GuiPresentationBuilder toolGui = new GuiPresentationBuilder(
            "FlexibleBackgrounds/&BackgroundToolSelectDescription",
            "FlexibleBackgrounds/&BackgroundToolSelectTitle");

        private static readonly FeatureDefinition toolChoice = new FeatureDefinitionPointPoolBuilder("BackgroundToolSelect",
            "989ddb03-b915-42cc-9612-bc8be96b7476", HeroDefinitions.PointsPoolType.Tool, 1, toolGui.Build()).AddToDB();

        private static Dictionary<CharacterBackgroundDefinition, List<FeatureDefinition>> addedFeatures = new Dictionary<CharacterBackgroundDefinition, List<FeatureDefinition>>
        {
            { Academic, new List<FeatureDefinition> { skillThree, toolChoice } },
            { Acolyte, new List<FeatureDefinition> { skillThree, toolChoice } },
            { Aristocrat, new List<FeatureDefinition> { skillThree } },
            { Lawkeeper, new List<FeatureDefinition> { skillThree } },
            { Lowlife, new List<FeatureDefinition> { skillTwo, toolChoice } },
            { Philosopher, new List<FeatureDefinition> { skillThree, toolChoice } },
            { SellSword, new List<FeatureDefinition> { skillTwo, toolChoice } },
            { Spy, new List<FeatureDefinition> { skillTwo, toolChoice } },
            // TODO- verify this doesn't break users who don't have the DLC
            { Wanderer, new List<FeatureDefinition> { skillThree, toolChoice } },
        };

        private static Dictionary<CharacterBackgroundDefinition, List<FeatureDefinition>> removedFeatures = new Dictionary<CharacterBackgroundDefinition, List<FeatureDefinition>>
        {
            { Academic, new List<FeatureDefinition> { ProficiencyAcademicSkills, ProficiencyAcademicSkillsTool } },
            { Acolyte, new List<FeatureDefinition> { ProficiencyAcolyteSkills, ProficiencyAcolyteToolsSkills } },
            { Aristocrat, new List<FeatureDefinition> { ProficiencyAristocratSkills } },
            { Lawkeeper, new List<FeatureDefinition> { ProficiencyLawkeeperSkills } },
            { Lowlife, new List<FeatureDefinition> { ProficiencyLowlifeSkills, ProficiencyLowLifeSkillsTools } },
            { Philosopher, new List<FeatureDefinition> { ProficiencyPhilosopherSkills, ProficiencyPhilosopherTools } },
            { SellSword, new List<FeatureDefinition> { ProficiencySellSwordSkills, ProficiencySmithTools } },
            { Spy, new List<FeatureDefinition> { ProficienctSpySkillsTool, ProficiencySmithTools } },
            { Wanderer, new List<FeatureDefinition> { ProficiencyWandererSkills, ProficiencyWandererTools } },
        };

        internal static void Switch(bool enabled) 
        {
            foreach (var keyValuePair in addedFeatures)
            {
                foreach (var featureDefinition in keyValuePair.Value)
                {
                    if (!keyValuePair.Key.Features.Contains(featureDefinition) && enabled)
                    {
                        keyValuePair.Key.Features.Add(featureDefinition);
                    }
                    else if (keyValuePair.Key.Features.Contains(featureDefinition) && !enabled)
                    {
                        keyValuePair.Key.Features.Remove(featureDefinition);
                    }
                }
            }

            foreach (var keyValuePair in removedFeatures)
            {
                foreach (var featureDefinition in keyValuePair.Value)
                {
                    if (keyValuePair.Key.Features.Contains(featureDefinition) && enabled)
                    {
                        keyValuePair.Key.Features.Remove(featureDefinition);
                    }
                    else if (!keyValuePair.Key.Features.Contains(featureDefinition) && !enabled)
                    {
                        keyValuePair.Key.Features.Add(featureDefinition);
                    }
                }
            }
        }

        internal static void Load()
        {
            Switch(Main.Settings.EnableFlexibleBackgrounds);
        }
    }
}
