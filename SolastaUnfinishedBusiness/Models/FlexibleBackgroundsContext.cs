using System.Collections.Generic;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterBackgroundDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionProficiencys;

namespace SolastaUnfinishedBusiness.Models;

public static class FlexibleBackgroundsContext
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

    private static readonly FeatureDefinition ToolChoice = FeatureDefinitionPointPoolBuilder
        .Create("PointPoolBackgroundToolSelect")
        .SetGuiPresentation(Category.Background)
        .SetPool(HeroDefinitions.PointsPoolType.Tool, 1)
        .AddToDB();

    private static readonly FeatureDefinition ToolChoiceTwo = FeatureDefinitionPointPoolBuilder
        .Create("PointPoolBackgroundToolSelect2")
        .SetGuiPresentation(Category.Background)
        .SetPool(HeroDefinitions.PointsPoolType.Tool, 2)
        .AddToDB();

    private static readonly FeatureDefinition AcademicSuggestedSkills = FeatureDefinitionBuilder
        .Create("SuggestedSkillsAcademicBackground")
        .SetGuiPresentation(Category.Background)
        .AddToDB();

    private static readonly FeatureDefinition AcolyteSuggestedSkills = FeatureDefinitionBuilder
        .Create("SuggestedSkillsAcolyteBackground")
        .SetGuiPresentation(Category.Background)
        .AddToDB();

    private static readonly FeatureDefinition AristocratSuggestedSkills = FeatureDefinitionBuilder
        .Create("SuggestedSkillsAristocratBackground")
        .SetGuiPresentation(Category.Background)
        .AddToDB();

    private static readonly FeatureDefinition LawkeeperSuggestedSkills = FeatureDefinitionBuilder
        .Create("SuggestedSkillsLawkeeperBackground")
        .SetGuiPresentation(Category.Background)
        .AddToDB();

    private static readonly FeatureDefinition LowlifeSuggestedSkills = FeatureDefinitionBuilder
        .Create("SuggestedSkillsLowlifeBackground")
        .SetGuiPresentation(Category.Background)
        .AddToDB();

    private static readonly FeatureDefinition PhilosopherSuggestedSkills = FeatureDefinitionBuilder
        .Create("SuggestedSkillsPhilosopherBackground")
        .SetGuiPresentation(Category.Background)
        .AddToDB();

    private static readonly FeatureDefinition SellswordSuggestedSkills = FeatureDefinitionBuilder
        .Create("SuggestedSkillsSellswordBackground")
        .SetGuiPresentation(Category.Background)
        .AddToDB();

    private static readonly FeatureDefinition SpySuggestedSkills = FeatureDefinitionBuilder
        .Create("SuggestedSkillsSpyBackground")
        .SetGuiPresentation(Category.Background)
        .AddToDB();

    private static readonly FeatureDefinition WandererSuggestedSkills = FeatureDefinitionBuilder
        .Create("SuggestedSkillsWandererBackground")
        .SetGuiPresentation(Category.Background)
        .AddToDB();

    private static readonly FeatureDefinition AesceticSuggestedSkills = FeatureDefinitionBuilder
        .Create("SuggestedSkillsAesceticBackground")
        .SetGuiPresentation(Category.Background)
        .AddToDB();

    private static readonly FeatureDefinition ArtistSuggestedSkills = FeatureDefinitionBuilder
        .Create("SuggestedSkillsArtistBackground")
        .SetGuiPresentation(Category.Background)
        .AddToDB();

    private static readonly FeatureDefinition OccultistSuggestedSkills = FeatureDefinitionBuilder
        .Create("SuggestedSkillsOccultistBackground")
        .SetGuiPresentation(Category.Background)
        .AddToDB();

    private static readonly Dictionary<CharacterBackgroundDefinition, List<FeatureDefinition>> AddedFeatures = new()
    {
        { Academic, new List<FeatureDefinition> { SkillThree, AcademicSuggestedSkills, ToolChoice } },
        { Acolyte, new List<FeatureDefinition> { SkillThree, AcolyteSuggestedSkills, ToolChoice } },
        { Aristocrat, new List<FeatureDefinition> { SkillThree, AristocratSuggestedSkills } },
        { Lawkeeper, new List<FeatureDefinition> { SkillTwo, LawkeeperSuggestedSkills } },
        { Lowlife, new List<FeatureDefinition> { SkillThree, LowlifeSuggestedSkills, ToolChoice } },
        { Philosopher, new List<FeatureDefinition> { SkillTwo, PhilosopherSuggestedSkills, ToolChoice } },
        { SellSword, new List<FeatureDefinition> { SkillTwo, SellswordSuggestedSkills, ToolChoice } },
        { Spy, new List<FeatureDefinition> { SkillThree, SpySuggestedSkills, ToolChoice } },
        { Wanderer, new List<FeatureDefinition> { SkillTwo, WandererSuggestedSkills, ToolChoiceTwo } },
        { Aescetic_Background, new List<FeatureDefinition> { SkillTwo, AesceticSuggestedSkills, ToolChoice } },
        { Artist_Background, new List<FeatureDefinition> { SkillThree, ArtistSuggestedSkills } },
        { Occultist_Background, new List<FeatureDefinition> { SkillTwo, OccultistSuggestedSkills, ToolChoice } }
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
        Switch();
    }

    internal static void Switch()
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
