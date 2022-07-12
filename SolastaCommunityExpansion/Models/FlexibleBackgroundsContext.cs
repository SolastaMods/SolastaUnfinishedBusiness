using System;
using System.Collections.Generic;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using static SolastaCommunityExpansion.Api.DatabaseHelper.CharacterBackgroundDefinitions;
using static SolastaCommunityExpansion.Api.DatabaseHelper.FeatureDefinitionProficiencys;

namespace SolastaCommunityExpansion.Models;

public static class FlexibleBackgroundsContext
{
    private const string FlexibleBackgroundsGuid = "f7713746-2028-410d-9df6-20f261f4d6aa";

    private static readonly Guid FlexBackBaseGuid = new(FlexibleBackgroundsGuid);

    private static readonly FeatureDefinition SkillThree = FeatureDefinitionPointPoolBuilder
        .Create("BackgroundSkillSelect3", "e6f2ed65-a44e-4314-b38c-393abb4ad900")
        .SetGuiPresentation(Category.FlexibleBackgrounds)
        .SetPool(HeroDefinitions.PointsPoolType.Skill, 3)
        .AddToDB();

    private static readonly FeatureDefinition SkillTwo = FeatureDefinitionPointPoolBuilder
        .Create("BackgroundSkillSelect2", "77d6eb2c-d99f-4256-9bb6-c6395e440629")
        .SetGuiPresentation(Category.FlexibleBackgrounds)
        .SetPool(HeroDefinitions.PointsPoolType.Skill, 2)
        .AddToDB();

    private static readonly FeatureDefinition ToolChoice = FeatureDefinitionPointPoolBuilder
        .Create("BackgroundToolSelect", "989ddb03-b915-42cc-9612-bc8be96b7476")
        .SetGuiPresentation(Category.FlexibleBackgrounds)
        .SetPool(HeroDefinitions.PointsPoolType.Tool, 1)
        .AddToDB();

    private static readonly FeatureDefinition ToolChoiceTwo = FeatureDefinitionPointPoolBuilder
        .Create("BackgroundToolSelect2", "07d30e58-eddc-43eb-a24c-71f107b8d76a")
        .SetGuiPresentation(Category.FlexibleBackgrounds)
        .SetPool(HeroDefinitions.PointsPoolType.Tool, 2)
        .AddToDB();

    private static readonly FeatureDefinition AcademicSuggestedSkills = FeatureDefinitionBuilder
        .Create("AcademicBackgroundSuggestedSkills", FlexBackBaseGuid)
        .SetGuiPresentation("AcademicBackgroundSuggestedSkills", Category.FlexibleBackgrounds)
        .AddToDB();

    private static readonly FeatureDefinition AcolyteSuggestedSkills = FeatureDefinitionBuilder
        .Create("AcolyteBackgroundSuggestedSkills", FlexBackBaseGuid)
        .SetGuiPresentation("AcolyteBackgroundSuggestedSkills", Category.FlexibleBackgrounds)
        .AddToDB();

    private static readonly FeatureDefinition AristocratSuggestedSkills = FeatureDefinitionBuilder
        .Create("AristocratBackgroundSuggestedSkills", FlexBackBaseGuid)
        .SetGuiPresentation("AristocratBackgroundSuggestedSkills", Category.FlexibleBackgrounds)
        .AddToDB();

    private static readonly FeatureDefinition LawkeeperSuggestedSkills = FeatureDefinitionBuilder
        .Create("LawkeeperBackgroundSuggestedSkills", FlexBackBaseGuid)
        .SetGuiPresentation("LawkeeperBackgroundSuggestedSkills", Category.FlexibleBackgrounds)
        .AddToDB();

    private static readonly FeatureDefinition LowlifeSuggestedSkills = FeatureDefinitionBuilder
        .Create("LowlifeBackgroundSuggestedSkills", FlexBackBaseGuid)
        .SetGuiPresentation("LowlifeBackgroundSuggestedSkills", Category.FlexibleBackgrounds)
        .AddToDB();

    private static readonly FeatureDefinition PhilosopherSuggestedSkills = FeatureDefinitionBuilder
        .Create("PhilosopherBackgroundSuggestedSkills", FlexBackBaseGuid)
        .SetGuiPresentation("PhilosopherBackgroundSuggestedSkills", Category.FlexibleBackgrounds)
        .AddToDB();

    private static readonly FeatureDefinition SellswordSuggestedSkills = FeatureDefinitionBuilder
        .Create("SellswordBackgroundSuggestedSkills", FlexBackBaseGuid)
        .SetGuiPresentation("SellswordBackgroundSuggestedSkills", Category.FlexibleBackgrounds)
        .AddToDB();

    private static readonly FeatureDefinition SpySuggestedSkills = FeatureDefinitionBuilder
        .Create("SpyBackgroundSuggestedSkills", FlexBackBaseGuid)
        .SetGuiPresentation("SpyBackgroundSuggestedSkills", Category.FlexibleBackgrounds)
        .AddToDB();

    private static readonly FeatureDefinition WandererSuggestedSkills = FeatureDefinitionBuilder
        .Create("WandererBackgroundSuggestedSkills", FlexBackBaseGuid)
        .SetGuiPresentation("WandererBackgroundSuggestedSkills", Category.FlexibleBackgrounds)
        .AddToDB();

    private static readonly Dictionary<CharacterBackgroundDefinition, List<FeatureDefinition>> AddedFeatures = new()
    {
        {Academic, new List<FeatureDefinition> {SkillThree, AcademicSuggestedSkills, ToolChoice}},
        {Acolyte, new List<FeatureDefinition> {SkillThree, AcolyteSuggestedSkills, ToolChoice}},
        {Aristocrat, new List<FeatureDefinition> {SkillThree, AristocratSuggestedSkills}},
        {Lawkeeper, new List<FeatureDefinition> {SkillTwo, LawkeeperSuggestedSkills}},
        {Lowlife, new List<FeatureDefinition> {SkillThree, LowlifeSuggestedSkills, ToolChoice}},
        {Philosopher, new List<FeatureDefinition> {SkillTwo, PhilosopherSuggestedSkills, ToolChoice}},
        {SellSword, new List<FeatureDefinition> {SkillTwo, SellswordSuggestedSkills, ToolChoice}},
        {Spy, new List<FeatureDefinition> {SkillThree, SpySuggestedSkills, ToolChoice}},
        {Wanderer, new List<FeatureDefinition> {SkillTwo, WandererSuggestedSkills, ToolChoiceTwo}}
    };

    private static readonly Dictionary<CharacterBackgroundDefinition, List<FeatureDefinition>> RemovedFeatures =
        new()
        {
            {Academic, new List<FeatureDefinition> {ProficiencyAcademicSkills, ProficiencyAcademicSkillsTool}},
            {Acolyte, new List<FeatureDefinition> {ProficiencyAcolyteSkills, ProficiencyAcolyteToolsSkills}},
            {Aristocrat, new List<FeatureDefinition> {ProficiencyAristocratSkills}},
            {Lawkeeper, new List<FeatureDefinition> {ProficiencyLawkeeperSkills}},
            {Lowlife, new List<FeatureDefinition> {ProficiencyLowlifeSkills, ProficiencyLowLifeSkillsTools}},
            {Philosopher, new List<FeatureDefinition> {ProficiencyPhilosopherSkills, ProficiencyPhilosopherTools}},
            {SellSword, new List<FeatureDefinition> {ProficiencySellSwordSkills, ProficiencySmithTools}},
            {Spy, new List<FeatureDefinition> {ProficiencySpySkills, ProficienctSpySkillsTool}},
            {Wanderer, new List<FeatureDefinition> {ProficiencyWandererSkills, ProficiencyWandererTools}}
        };

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
