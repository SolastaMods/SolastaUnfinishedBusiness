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
            Academic, [
                SkillThree,
                FeatureDefinitionBuilder
                    .Create("SuggestedSkillsAcademicBackground")
                    .SetGuiPresentation(Category.Background)
                    .AddToDB(),

                ToolChoice
            ]
        },
        {
            Acolyte, [
                SkillThree,
                FeatureDefinitionBuilder
                    .Create("SuggestedSkillsAcolyteBackground")
                    .SetGuiPresentation(Category.Background)
                    .AddToDB(),

                ToolChoice
            ]
        },
        {
            Aristocrat, [
                SkillThree,
                FeatureDefinitionBuilder
                    .Create("SuggestedSkillsAristocratBackground")
                    .SetGuiPresentation(Category.Background)
                    .AddToDB()
            ]
        },
        {
            Lawkeeper, [
                SkillTwo,
                FeatureDefinitionBuilder
                    .Create("SuggestedSkillsLawkeeperBackground")
                    .SetGuiPresentation(Category.Background)
                    .AddToDB()
            ]
        },
        {
            Lowlife, [
                SkillThree,
                FeatureDefinitionBuilder
                    .Create("SuggestedSkillsLowlifeBackground")
                    .SetGuiPresentation(Category.Background)
                    .AddToDB(),

                ToolChoice
            ]
        },
        {
            Philosopher, [
                SkillTwo,
                FeatureDefinitionBuilder
                    .Create("SuggestedSkillsPhilosopherBackground")
                    .SetGuiPresentation(Category.Background)
                    .AddToDB(),

                ToolChoice
            ]
        },
        {
            SellSword, [
                SkillTwo,
                FeatureDefinitionBuilder
                    .Create("SuggestedSkillsSellswordBackground")
                    .SetGuiPresentation(Category.Background)
                    .AddToDB(),

                ToolChoice
            ]
        },
        {
            Spy, [
                SkillThree,
                FeatureDefinitionBuilder
                    .Create("SuggestedSkillsSpyBackground")
                    .SetGuiPresentation(Category.Background)
                    .AddToDB(),

                ToolChoice
            ]
        },
        {
            Wanderer, [
                SkillTwo,
                FeatureDefinitionBuilder
                    .Create("SuggestedSkillsWandererBackground")
                    .SetGuiPresentation(Category.Background)
                    .AddToDB(),

                ToolChoiceTwo
            ]
        },
        {
            Aescetic_Background, [
                SkillTwo,
                FeatureDefinitionBuilder
                    .Create("SuggestedSkillsAesceticBackground")
                    .SetGuiPresentation(Category.Background)
                    .AddToDB(),

                ToolChoice
            ]
        },
        {
            Artist_Background, [
                SkillThree,
                FeatureDefinitionBuilder
                    .Create("SuggestedSkillsArtistBackground")
                    .SetGuiPresentation(Category.Background)
                    .AddToDB()
            ]
        },
        {
            Occultist_Background, [
                SkillTwo,
                FeatureDefinitionBuilder
                    .Create("SuggestedSkillsOccultistBackground")
                    .SetGuiPresentation(Category.Background)
                    .AddToDB(),

                ToolChoice
            ]
        }
    };

    private static readonly Dictionary<CharacterBackgroundDefinition, List<FeatureDefinition>> RemovedFeatures =
        new()
        {
            { Academic, [ProficiencyAcademicSkills, ProficiencyAcademicSkillsTool] },
            { Acolyte, [ProficiencyAcolyteSkills, ProficiencyAcolyteToolsSkills] },
            { Aristocrat, [ProficiencyAristocratSkills] },
            { Lawkeeper, [ProficiencyLawkeeperSkills] },
            { Lowlife, [ProficiencyLowlifeSkills, ProficiencyLowLifeSkillsTools] },
            { Philosopher, [ProficiencyPhilosopherSkills, ProficiencyPhilosopherTools] },
            { SellSword, [ProficiencySellSwordSkills, ProficiencySmithTools] },
            { Spy, [ProficiencySpySkills, ProficienctSpySkillsTool] },
            { Wanderer, [ProficiencyWandererSkills, ProficiencyWandererTools] },
            { Aescetic_Background, [ProficiencyAesceticSkills, ProficiencyAesceticToolsSkills] },
            { Artist_Background, [ProficiencyArtistSkills] },
            { Occultist_Background, [ProficiencyOccultistSkills, ProficiencyOccultistToolsSkills] }
        };

    internal static void Load()
    {
        var backgrounds = new List<string> { "Devoted", "Farmer", "Militia", "Troublemaker" };

        foreach (var background in backgrounds)
        {
            var backgroundDefinition =
                DatabaseHelper.GetDefinition<CharacterBackgroundDefinition>($"Background{background}");

            AddedFeatures.Add(
                backgroundDefinition,
                [
                    SkillThree,
                    FeatureDefinitionBuilder
                        .Create($"SuggestedSkills{background}Background")
                        .SetGuiPresentation(Category.Background)
                        .AddToDB()
                ]);

            RemovedFeatures.Add(
                backgroundDefinition,
                [
                    DatabaseHelper.GetDefinition<FeatureDefinitionProficiency>(
                        $"ProficiencyBackground{background}Skills")
                ]);
        }
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
