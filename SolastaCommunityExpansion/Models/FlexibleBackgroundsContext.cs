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

    private static readonly Guid FLEX_BACK_BASE_GUID = new(FlexibleBackgroundsGuid);

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

    private static readonly FeatureDefinition toolChoiceTwo = FeatureDefinitionPointPoolBuilder
        .Create("BackgroundToolSelect2", "07d30e58-eddc-43eb-a24c-71f107b8d76a")
        .SetGuiPresentation(Category.FlexibleBackgrounds)
        .SetPool(HeroDefinitions.PointsPoolType.Tool, 2)
        .AddToDB();

    private static readonly FeatureDefinition academicSuggestedSkills = FeatureDefinitionBuilder
        .Create("AcademicBackgroundSuggestedSkills", FLEX_BACK_BASE_GUID)
        .SetGuiPresentation("AcademicBackgroundSuggestedSkills", Category.FlexibleBackgrounds)
        .AddToDB();

    private static readonly FeatureDefinition acolyteSuggestedSkills = FeatureDefinitionBuilder
        .Create("AcolyteBackgroundSuggestedSkills", FLEX_BACK_BASE_GUID)
        .SetGuiPresentation("AcolyteBackgroundSuggestedSkills", Category.FlexibleBackgrounds)
        .AddToDB();

    private static readonly FeatureDefinition aristocratSuggestedSkills = FeatureDefinitionBuilder
        .Create("AristocratBackgroundSuggestedSkills", FLEX_BACK_BASE_GUID)
        .SetGuiPresentation("AristocratBackgroundSuggestedSkills", Category.FlexibleBackgrounds)
        .AddToDB();

    private static readonly FeatureDefinition lawkeeperSuggestedSkills = FeatureDefinitionBuilder
        .Create("LawkeeperBackgroundSuggestedSkills", FLEX_BACK_BASE_GUID)
        .SetGuiPresentation("LawkeeperBackgroundSuggestedSkills", Category.FlexibleBackgrounds)
        .AddToDB();

    private static readonly FeatureDefinition lowlifeSuggestedSkills = FeatureDefinitionBuilder
        .Create("LowlifeBackgroundSuggestedSkills", FLEX_BACK_BASE_GUID)
        .SetGuiPresentation("LowlifeBackgroundSuggestedSkills", Category.FlexibleBackgrounds)
        .AddToDB();

    private static readonly FeatureDefinition philosopherSuggestedSkills = FeatureDefinitionBuilder
        .Create("PhilosopherBackgroundSuggestedSkills", FLEX_BACK_BASE_GUID)
        .SetGuiPresentation("PhilosopherBackgroundSuggestedSkills", Category.FlexibleBackgrounds)
        .AddToDB();

    private static readonly FeatureDefinition sellswordSuggestedSkills = FeatureDefinitionBuilder
        .Create("SellswordBackgroundSuggestedSkills", FLEX_BACK_BASE_GUID)
        .SetGuiPresentation("SellswordBackgroundSuggestedSkills", Category.FlexibleBackgrounds)
        .AddToDB();

    private static readonly FeatureDefinition spySuggestedSkills = FeatureDefinitionBuilder
        .Create("SpyBackgroundSuggestedSkills", FLEX_BACK_BASE_GUID)
        .SetGuiPresentation("SpyBackgroundSuggestedSkills", Category.FlexibleBackgrounds)
        .AddToDB();

    private static readonly FeatureDefinition wandererSuggestedSkills = FeatureDefinitionBuilder
        .Create("WandererBackgroundSuggestedSkills", FLEX_BACK_BASE_GUID)
        .SetGuiPresentation("WandererBackgroundSuggestedSkills", Category.FlexibleBackgrounds)
        .AddToDB();

    private static readonly Dictionary<CharacterBackgroundDefinition, List<FeatureDefinition>> addedFeatures = new()
    {
        {Academic, new List<FeatureDefinition> {skillThree, academicSuggestedSkills, toolChoice}},
        {Acolyte, new List<FeatureDefinition> {skillThree, acolyteSuggestedSkills, toolChoice}},
        {Aristocrat, new List<FeatureDefinition> {skillThree, aristocratSuggestedSkills}},
        {Lawkeeper, new List<FeatureDefinition> {skillTwo, lawkeeperSuggestedSkills}},
        {Lowlife, new List<FeatureDefinition> {skillThree, lowlifeSuggestedSkills, toolChoice}},
        {Philosopher, new List<FeatureDefinition> {skillTwo, philosopherSuggestedSkills, toolChoice}},
        {SellSword, new List<FeatureDefinition> {skillTwo, sellswordSuggestedSkills, toolChoice}},
        {Spy, new List<FeatureDefinition> {skillThree, spySuggestedSkills, toolChoice}},
        {Wanderer, new List<FeatureDefinition> {skillTwo, wandererSuggestedSkills, toolChoiceTwo}}
    };

    private static readonly Dictionary<CharacterBackgroundDefinition, List<FeatureDefinition>> removedFeatures =
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
