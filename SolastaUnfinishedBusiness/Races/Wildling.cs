using System.Collections.Generic;
using System.Linq;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Properties;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Races.RaceWyrmkinBuilder;

namespace SolastaUnfinishedBusiness.Races;
internal class RaceWildlingBuilder
{
    public const string ConditionWildlingTiredName = "ConditionWildlingTired";
    public const string ConditionWildlingFeralDashName = "ConditionWildlingFeralDash";

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
                FeatureDefinitionAttributeModifiers.AttributeModifierHalfElfAbilityScoreIncreaseCha)
            .AddToDB();
        var featureWildlingClaws = FeatureDefinitionBuilder
            .Create($"Feature{RaceName}Claws")
            .SetGuiPresentation(Category.Feature)
            .AddCustomSubFeatures(new ModifyWeaponAttackModeClaws())
            .AddToDB();

        var proficiencyWildlingNaturalInstincts = FeatureDefinitionProficiencyBuilder
            .Create($"Proficiency{RaceName}NaturalInstincts ")
            .SetGuiPresentation(Category.Feature)
            .SetProficiencies(RuleDefinitions.ProficiencyType.Skill, SkillDefinitions.Stealth, SkillDefinitions.Perception)
            .AddToDB();

        var actionAffinityWildlingFeralAgility = FeatureDefinitionActionAffinityBuilder
            .Create($"ActionAffinity{RaceName}Agility")
            .SetGuiPresentation(Category.Feature)
            .SetAuthorizedActions((ActionDefinitions.Id)ExtraActionId.WildlingDash)
            .AddToDB();

        ActionDefinitionBuilder.Create("WildlingDash")
            .SetOrUpdateGuiPresentation(Category.Action, DatabaseHelper.ActionDefinitions.DashMain)
            .RequiresAuthorization()
            .SetActionId(ExtraActionId.WildlingDash)
            .SetFormType(ActionDefinitions.ActionFormType.Large)
            .SetActionScope(ActionDefinitions.ActionScope.Battle)
            .SetActionType(ActionDefinitions.ActionType.NoCost)
            .AddToDB();

        var actionAffinityWildlingTired = FeatureDefinitionActionAffinityBuilder
            .Create($"ActionAffinity{RaceName}Tired")
            .SetGuiPresentationNoContent()
            .SetForbiddenActions((ActionDefinitions.Id)ExtraActionId.WildlingDash)
            .AddCustomSubFeatures(new TiredOnTurnEnd())
            .AddToDB();

        ConditionDefinitionBuilder
            .Create(ConditionWildlingTiredName)
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionRecklessVulnerability)
            .SetConditionType(RuleDefinitions.ConditionType.Detrimental)
            .SetFeatures(actionAffinityWildlingTired)
            .AddToDB();

        ConditionDefinitionBuilder
            .Create(ConditionWildlingFeralDashName)
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionReckless)
            .SetConditionType(RuleDefinitions.ConditionType.Beneficial)
            .SetFeatures(FeatureDefinitionMovementAffinityBuilder
                .Create($"MovementAffinity{RaceName}FeralDash")
                .SetGuiPresentationNoContent()
                .SetBaseSpeedMultiplicativeModifier(2f)
                .AddToDB())
            .AddToDB();

        var raceWildling = CharacterRaceDefinitionBuilder
            .Create(CharacterRaceDefinitions.Human, $"Race{RaceName}")
            .SetGuiPresentation(Category.Race, Sprites.GetSprite(RaceName, Resources.Wildling, 1024, 512))
            .SetSizeDefinition(CharacterSizeDefinitions.Medium)
            .SetBaseHeight(92)
            .SetBaseWeight(185)
            .SetMinimalAge(18)
            .SetMaximalAge(750)
            .SetFeaturesAtLevel(1,
                FeatureDefinitionMoveModes.MoveModeMove6,
                FeatureDefinitionMoveModes.MoveModeClimb6,
                FeatureDefinitionSenses.SenseDarkvision,
                FeatureDefinitionSenses.SenseNormalVision,
                FlexibleRacesContext.FeatureSetLanguageCommonPlusOne,
                featureSetWildlingAbilityScoreIncrease,
                actionAffinityWildlingFeralAgility,
                proficiencyWildlingNaturalInstincts,
                featureWildlingClaws)
            .AddToDB();

        var racePresentation = raceWildling.RacePresentation;
        racePresentation.originOptions.RemoveRange(1, racePresentation.originOptions.Count - 1);

        var list = racePresentation.AvailableMorphotypeCategories.ToList();
        list.Add(MorphotypeElementDefinition.ElementCategory.Horns);
        racePresentation.availableMorphotypeCategories = list.ToArray();
        racePresentation.maleHornsOptions = new List<string>();
        racePresentation.hornsTailAssetPrefix = 
            CharacterRaceDefinitions.Tiefling.RacePresentation.hornsTailAssetPrefix;
        racePresentation.maleHornsOptions.AddRange(CharacterRaceDefinitions.Tiefling.RacePresentation.maleHornsOptions);
        racePresentation.femaleHornsOptions = new List<string>();
        racePresentation.femaleHornsOptions.AddRange(CharacterRaceDefinitions.Tiefling.RacePresentation.femaleHornsOptions);
        return raceWildling;
    }
}

internal class TiredOnTurnEnd : ICharacterTurnEndListener
{

    public void OnCharacterTurnEnded(GameLocationCharacter locationCharacter)
    {
        // remove tired condition
        if (!locationCharacter.RulesetCharacter.HasConditionOfType(RaceWildlingBuilder.ConditionWildlingTiredName)) return;
        if (locationCharacter.UsedTacticalMoves > 0) return;
        locationCharacter.RulesetCharacter.RemoveAllConditionsOfType(RaceWildlingBuilder.ConditionWildlingTiredName);
    }
}
