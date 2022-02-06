using System.Collections.Generic;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using static SolastaModApi.DatabaseHelper.CharacterBackgroundDefinitions;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionProficiencys;

namespace SolastaCommunityExpansion.Models
{
    public static class FlexibleBackgroundsContext
    {
        private static readonly FeatureDefinition skillThree = FeatureDefinitionPointPoolBuilder
            .Create("BackgroundSkillSelect3", "e6f2ed65-a44e-4314-b38c-393abb4ad900")
            .SetGuiPresentation(Category.FlexibleBackgrounds)
            .SetPool(HeroDefinitions.PointsPoolType.Skill, 3)
            .AddToDB();

        private static readonly FeatureDefinition skillTwo = FeatureDefinitionPointPoolBuilder
            .Create("BackgroundSkillSelect2", "77d6eb2c-d99f-4256-9bb6-c6395e440629")
            .SetGuiPresentation(Category.FlexibleBackgrounds)
            .SetPool(HeroDefinitions.PointsPoolType.Skill, 2)
            .AddToDB();

        private static readonly FeatureDefinition toolChoice = FeatureDefinitionPointPoolBuilder
            .Create("BackgroundToolSelect", "989ddb03-b915-42cc-9612-bc8be96b7476")
            .SetGuiPresentation(Category.FlexibleBackgrounds)
            .SetPool(HeroDefinitions.PointsPoolType.Tool, 1)
            .AddToDB();

        private static readonly Dictionary<CharacterBackgroundDefinition, List<FeatureDefinition>> addedFeatures = new()
        {
            { Academic, new List<FeatureDefinition> { skillThree, toolChoice } },
            { Acolyte, new List<FeatureDefinition> { skillThree, toolChoice } },
            { Aristocrat, new List<FeatureDefinition> { skillThree } },
            { Lawkeeper, new List<FeatureDefinition> { skillThree } },
            { Lowlife, new List<FeatureDefinition> { skillTwo, toolChoice } },
            { Philosopher, new List<FeatureDefinition> { skillThree, toolChoice } },
            { SellSword, new List<FeatureDefinition> { skillTwo, toolChoice } },
            { Spy, new List<FeatureDefinition> { skillTwo, toolChoice } },
            { Wanderer, new List<FeatureDefinition> { skillThree, toolChoice } },
        };

        private static readonly Dictionary<CharacterBackgroundDefinition, List<FeatureDefinition>> removedFeatures = new()
        {
            { Academic, new List<FeatureDefinition> { ProficiencyAcademicSkills, ProficiencyAcademicSkillsTool } },
            { Acolyte, new List<FeatureDefinition> { ProficiencyAcolyteSkills, ProficiencyAcolyteToolsSkills } },
            { Aristocrat, new List<FeatureDefinition> { ProficiencyAristocratSkills } },
            { Lawkeeper, new List<FeatureDefinition> { ProficiencyLawkeeperSkills } },
            { Lowlife, new List<FeatureDefinition> { ProficiencyLowlifeSkills, ProficiencyLowLifeSkillsTools } },
            { Philosopher, new List<FeatureDefinition> { ProficiencyPhilosopherSkills, ProficiencyPhilosopherTools } },
            { SellSword, new List<FeatureDefinition> { ProficiencySellSwordSkills, ProficiencySmithTools } },
            { Spy, new List<FeatureDefinition> { ProficiencySpySkills, ProficienctSpySkillsTool } },
            { Wanderer, new List<FeatureDefinition> { ProficiencyWandererSkills, ProficiencyWandererTools } },
        };

        internal static void Switch()
        {
            var enabled = Main.Settings.EnableFlexibleBackgrounds;

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
    }
}
