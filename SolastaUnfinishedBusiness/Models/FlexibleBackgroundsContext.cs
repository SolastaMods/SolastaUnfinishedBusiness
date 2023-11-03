using System.Collections.Generic;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterBackgroundDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionProficiencys;

namespace SolastaUnfinishedBusiness.Models;

internal static class FlexibleBackgroundsContext
{
    private static readonly FeatureDefinition SkillThree = FeatureDefinitionPointPoolBuilder
        .Create("PointPoolBackgroundSkillSelect3")
        .SetGuiPresentation(Category.Background)
        .SetPool(HeroDefinitions.PointsPoolType.Skill, 3)
        .AddToDB();

    private static readonly FeatureDefinition SkillTwo = FeatureDefinitionPointPoolBuilder
        .Create("PointPoolBackgroundSkillSelect2")
        .SetGuiPresentation(Category.Background)
        .SetPool(HeroDefinitions.PointsPoolType.Skill, 2)
        .AddToDB();

    internal static readonly FeatureDefinition SkillOne = FeatureDefinitionPointPoolBuilder
        .Create("PointPoolBackgroundSkillSelect1")
        .SetGuiPresentation(Category.Background)
        .SetPool(HeroDefinitions.PointsPoolType.Skill, 1)
        .AddToDB();

    internal static readonly FeatureDefinition ToolChoice = FeatureDefinitionPointPoolBuilder
        .Create("PointPoolBackgroundToolSelect")
        .SetGuiPresentation(Category.Background)
        .SetPool(HeroDefinitions.PointsPoolType.Tool, 1)
        .AddToDB();

    private static readonly FeatureDefinition ToolChoiceTwo = FeatureDefinitionPointPoolBuilder
        .Create("PointPoolBackgroundToolSelect2")
        .SetGuiPresentation(Category.Background)
        .SetPool(HeroDefinitions.PointsPoolType.Tool, 2)
        .AddToDB();

    private static readonly Dictionary<CharacterBackgroundDefinition, List<FeatureDefinition>> AddedFeatures = new()
    {
        {
            Academic, new List<FeatureDefinition>
            {
                SkillThree,
                FeatureDefinitionBuilder
                    .Create("SuggestedSkillsAcademicBackground")
                    .SetGuiPresentation(Category.Background)
                    .AddToDB(),
                ToolChoice
            }
        },
        {
            Acolyte, new List<FeatureDefinition>
            {
                SkillThree,
                FeatureDefinitionBuilder
                    .Create("SuggestedSkillsAcolyteBackground")
                    .SetGuiPresentation(Category.Background)
                    .AddToDB(),
                ToolChoice
            }
        },
        {
            Aristocrat, new List<FeatureDefinition>
            {
                SkillThree,
                FeatureDefinitionBuilder
                    .Create("SuggestedSkillsAristocratBackground")
                    .SetGuiPresentation(Category.Background)
                    .AddToDB()
            }
        },
        {
            Lawkeeper, new List<FeatureDefinition>
            {
                SkillTwo,
                FeatureDefinitionBuilder
                    .Create("SuggestedSkillsLawkeeperBackground")
                    .SetGuiPresentation(Category.Background)
                    .AddToDB()
            }
        },
        {
            Lowlife, new List<FeatureDefinition>
            {
                SkillThree,
                FeatureDefinitionBuilder
                    .Create("SuggestedSkillsLowlifeBackground")
                    .SetGuiPresentation(Category.Background)
                    .AddToDB(),
                ToolChoice
            }
        },
        {
            Philosopher, new List<FeatureDefinition>
            {
                SkillTwo,
                FeatureDefinitionBuilder
                    .Create("SuggestedSkillsPhilosopherBackground")
                    .SetGuiPresentation(Category.Background)
                    .AddToDB(),
                ToolChoice
            }
        },
        {
            SellSword, new List<FeatureDefinition>
            {
                SkillTwo,
                FeatureDefinitionBuilder
                    .Create("SuggestedSkillsSellswordBackground")
                    .SetGuiPresentation(Category.Background)
                    .AddToDB(),
                ToolChoice
            }
        },
        {
            Spy, new List<FeatureDefinition>
            {
                SkillThree,
                FeatureDefinitionBuilder
                    .Create("SuggestedSkillsSpyBackground")
                    .SetGuiPresentation(Category.Background)
                    .AddToDB(),
                ToolChoice
            }
        },
        {
            Wanderer, new List<FeatureDefinition>
            {
                SkillTwo,
                FeatureDefinitionBuilder
                    .Create("SuggestedSkillsWandererBackground")
                    .SetGuiPresentation(Category.Background)
                    .AddToDB(),
                ToolChoiceTwo
            }
        },
        {
            Aescetic_Background, new List<FeatureDefinition>
            {
                SkillTwo,
                FeatureDefinitionBuilder
                    .Create("SuggestedSkillsAesceticBackground")
                    .SetGuiPresentation(Category.Background)
                    .AddToDB(),
                ToolChoice
            }
        },
        {
            Artist_Background, new List<FeatureDefinition>
            {
                SkillThree,
                FeatureDefinitionBuilder
                    .Create("SuggestedSkillsArtistBackground")
                    .SetGuiPresentation(Category.Background)
                    .AddToDB()
            }
        },
        {
            Occultist_Background, new List<FeatureDefinition>
            {
                SkillTwo,
                FeatureDefinitionBuilder
                    .Create("SuggestedSkillsOccultistBackground")
                    .SetGuiPresentation(Category.Background)
                    .AddToDB(),
                ToolChoice
            }
        }
    };

    private static readonly Dictionary<CharacterBackgroundDefinition, List<FeatureDefinition>> RemovedFeatures =
        new()
        {
            { Academic, new List<FeatureDefinition> { ProficiencyAcademicSkills, ProficiencyAcademicSkillsTool } },
            { Acolyte, new List<FeatureDefinition> { ProficiencyAcolyteSkills, ProficiencyAcolyteToolsSkills } },
            { Aristocrat, new List<FeatureDefinition> { ProficiencyAristocratSkills } },
            { Lawkeeper, new List<FeatureDefinition> { ProficiencyLawkeeperSkills } },
            { Lowlife, new List<FeatureDefinition> { ProficiencyLowlifeSkills, ProficiencyLowLifeSkillsTools } },
            {
                Philosopher,
                new List<FeatureDefinition> { ProficiencyPhilosopherSkills, ProficiencyPhilosopherTools }
            },
            { SellSword, new List<FeatureDefinition> { ProficiencySellSwordSkills, ProficiencySmithTools } },
            { Spy, new List<FeatureDefinition> { ProficiencySpySkills, ProficienctSpySkillsTool } },
            { Wanderer, new List<FeatureDefinition> { ProficiencyWandererSkills, ProficiencyWandererTools } },
            {
                Aescetic_Background,
                new List<FeatureDefinition> { ProficiencyAesceticSkills, ProficiencyAesceticToolsSkills }
            },
            { Artist_Background, new List<FeatureDefinition> { ProficiencyArtistSkills } },
            {
                Occultist_Background,
                new List<FeatureDefinition> { ProficiencyOccultistSkills, ProficiencyOccultistToolsSkills }
            }
        };

    internal static void Load()
    {
        var backgroundFarmer = DatabaseHelper.GetDefinition<CharacterBackgroundDefinition>("BackgroundFarmer");

        AddedFeatures.Add(backgroundFarmer,
            new List<FeatureDefinition>
            {
                SkillThree,
                FeatureDefinitionBuilder
                    .Create("SuggestedSkillsFarmerBackground")
                    .SetGuiPresentation(Category.Background)
                    .AddToDB()
            });

        RemovedFeatures.Add(backgroundFarmer,
            new List<FeatureDefinition>
            {
                DatabaseHelper.GetDefinition<FeatureDefinitionProficiency>("ProficiencyBackgroundFarmerSkills")
            });
    }

    internal static void SwitchFlexibleBackgrounds()
    {
        var enabled = Main.Settings.EnableFlexibleBackgrounds;

        foreach (var keyValuePair in AddedFeatures)
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

        foreach (var keyValuePair in RemovedFeatures)
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
