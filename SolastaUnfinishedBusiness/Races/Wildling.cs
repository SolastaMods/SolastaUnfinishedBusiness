using System.Linq;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Properties;
using SolastaUnfinishedBusiness.Subclasses;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;

namespace SolastaUnfinishedBusiness.Races;

internal static class RaceWildlingBuilder
{
    internal const string ConditionWildlingTiredName = "ConditionWildlingTired";
    internal const string ConditionWildlingAgileName = "ConditionWildlingAgile";

    private const string RaceName = "Wildling";

    internal static CharacterRaceDefinition RaceWildling { get; } = BuildWildling();

    private static CharacterRaceDefinition BuildWildling()
    {
        var featureSetWildlingAbilityScoreIncrease = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{RaceName}AbilityScoreIncrease")
            .SetGuiPresentation(Category.Feature)
            .SetMode(FeatureDefinitionFeatureSet.FeatureSetMode.Union)
            .AddFeatureSet(
                FeatureDefinitionAttributeModifiers.AttributeModifierHalflingAbilityScoreIncrease,
                FeatureDefinitionAttributeModifiers.AttributeModifierDragonbornAbilityScoreIncreaseCha)
            .AddToDB();

        var featureWildlingClaws = FeatureDefinitionBuilder
            .Create($"Feature{RaceName}Claws")
            .SetGuiPresentation(Category.Feature)
            .AddCustomSubFeatures(new CommonBuilders.AddExtraUnarmedStrikeClawAttack())
            .AddToDB();

        var proficiencyWildlingNaturalInstincts = FeatureDefinitionProficiencyBuilder
            .Create($"Proficiency{RaceName}NaturalInstincts")
            .SetGuiPresentation(Category.Feature)
            .SetProficiencies(RuleDefinitions.ProficiencyType.Skill, SkillDefinitions.Stealth,
                SkillDefinitions.Perception)
            .AddToDB();

        var actionAffinityWildlingFeralAgility = FeatureDefinitionActionAffinityBuilder
            .Create($"ActionAffinity{RaceName}FeralAgility")
            .SetGuiPresentation(Category.Feature)
            .SetAuthorizedActions((ActionDefinitions.Id)ExtraActionId.WildlingFeralAgility)
            .AddToDB();

        ActionDefinitionBuilder
            .Create("WildlingFeralAgility")
            .SetGuiPresentation(Category.Action, DatabaseHelper.ActionDefinitions.DashBonus)
            .RequiresAuthorization()
            .SetActionId(ExtraActionId.WildlingFeralAgility)
            .SetFormType(ActionDefinitions.ActionFormType.Large)
            .SetActionScope(ActionDefinitions.ActionScope.Battle)
            .SetActionType(ActionDefinitions.ActionType.NoCost)
            .AddToDB();

        var actionAffinityWildlingTired = FeatureDefinitionActionAffinityBuilder
            .Create($"ActionAffinity{RaceName}Tired")
            .SetGuiPresentationNoContent(true)
            .SetForbiddenActions((ActionDefinitions.Id)ExtraActionId.WildlingFeralAgility)
            .AddCustomSubFeatures(new WildlingTiredOnTurnEnd())
            .AddToDB();

        ConditionDefinitionBuilder
            .Create(ConditionWildlingTiredName)
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionRecklessVulnerability)
            .SetConditionType(RuleDefinitions.ConditionType.Detrimental)
            .SetFeatures(actionAffinityWildlingTired)
            .AddToDB();

        ConditionDefinitionBuilder
            .Create(ConditionWildlingAgileName)
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionReckless)
            .SetConditionType(RuleDefinitions.ConditionType.Beneficial)
            .SetFeatures(FeatureDefinitionMovementAffinityBuilder
                .Create($"MovementAffinity{RaceName}Agile")
                .SetGuiPresentationNoContent()
                .SetBaseSpeedMultiplicativeModifier(2f)
                .AddToDB())
            .AddToDB();

        var movementAffinityWildlingExpertClimber = FeatureDefinitionMovementAffinityBuilder
            .Create($"MovementAffinity{RaceName}ExpertClimber")
            .SetGuiPresentation(Category.Feature)
            .SetClimbing(true, true)
            .AddToDB();

        var raceWildling = CharacterRaceDefinitionBuilder
            .Create(CharacterRaceDefinitions.Human, $"Race{RaceName}")
            .SetGuiPresentation(Category.Race, Sprites.GetSprite(RaceName, Resources.Wildling, 1024, 512))
            .SetSizeDefinition(CharacterSizeDefinitions.Medium)
            .SetFeaturesAtLevel(1,
                FeatureDefinitionMoveModes.MoveModeMove6,
                movementAffinityWildlingExpertClimber,
                FeatureDefinitionSenses.SenseDarkvision,
                FeatureDefinitionSenses.SenseNormalVision,
                FlexibleRacesContext.FeatureSetLanguageCommonPlusOne,
                featureSetWildlingAbilityScoreIncrease,
                actionAffinityWildlingFeralAgility,
                proficiencyWildlingNaturalInstincts,
                featureWildlingClaws)
            .AddToDB();

        var racePresentation = raceWildling.RacePresentation;

        racePresentation.originOptions = [racePresentation.originOptions[2]];

        var availableMorphotypeCategories = racePresentation.AvailableMorphotypeCategories.ToList();

        availableMorphotypeCategories.Add(MorphotypeElementDefinition.ElementCategory.Horns);

        racePresentation.availableMorphotypeCategories = availableMorphotypeCategories.ToArray();
        racePresentation.maleHornsOptions = [];
        racePresentation.hornsTailAssetPrefix =
            CharacterRaceDefinitions.Tiefling.RacePresentation.hornsTailAssetPrefix;
        racePresentation.maleHornsOptions.AddRange(CharacterRaceDefinitions.Tiefling.RacePresentation.maleHornsOptions);
        racePresentation.femaleHornsOptions = [];
        racePresentation.femaleHornsOptions.AddRange(
            CharacterRaceDefinitions.Tiefling.RacePresentation.femaleHornsOptions);

        return raceWildling;
    }
}

internal class WildlingTiredOnTurnEnd : ICharacterTurnEndListener
{
    public void OnCharacterTurnEnded(GameLocationCharacter locationCharacter)
    {
        // remove tired condition
        if (!locationCharacter.RulesetCharacter.HasConditionOfType(RaceWildlingBuilder.ConditionWildlingTiredName))
        {
            return;
        }

        if (locationCharacter.UsedTacticalMoves > 0)
        {
            return;
        }

        locationCharacter.RulesetCharacter.RemoveAllConditionsOfCategoryAndType(
            AttributeDefinitions.TagEffect, RaceWildlingBuilder.ConditionWildlingTiredName);
    }
}
