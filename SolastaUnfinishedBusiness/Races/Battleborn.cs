using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Properties;
using static FeatureDefinitionAttributeModifier;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;

namespace SolastaUnfinishedBusiness.Races;
internal class RaceBattlebornBuilder
{
    private const string RaceName = "Battleborn";

    internal static CharacterRaceDefinition RaceBattleborn { get; } = BuildBattleborn();

    private static CharacterRaceDefinition BuildBattleborn()
    {
        var featureSetBattlebornAbilityScoreIncrease = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{RaceName}AbilityScoreIncrease")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(
                FeatureDefinitionAttributeModifierBuilder
                    .Create($"AttributeModifier{RaceName}ConstitionTwo")
                    .SetGuiPresentationNoContent(true)
                    .SetModifier(AttributeModifierOperation.Additive, AttributeDefinitions.Constitution, 2)
                    .AddToDB(),
                FeatureDefinitionPointPoolBuilder
                    .Create($"PointPool{RaceName}AbilityScore")
                    .SetGuiPresentationNoContent(true)
                    .SetPool(HeroDefinitions.PointsPoolType.AbilityScore, 1)
                    .RestrictChoices(
                        AttributeDefinitions.Strength,
                        AttributeDefinitions.Dexterity,
                        AttributeDefinitions.Intelligence,
                        AttributeDefinitions.Wisdom,
                        AttributeDefinitions.Charisma)
                    .AddToDB())
            .AddToDB();

        var featureSetBattlebornSpecializedInfusion = FeatureDefinitionFeatureSetBuilder
            .Create($"Featureset{RaceName}SpecializedInfusion")
            .SetGuiPresentation(Category.Feature)
            .SetMode(FeatureDefinitionFeatureSet.FeatureSetMode.Union)
            .SetFeatureSet(
                FlexibleBackgroundsContext.SkillOne, 
                FlexibleBackgroundsContext.ToolChoice)
            .AddToDB();

        var featureSetBattlebornArcaneResilience = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{RaceName}ArcaneResilience")
            .SetGuiPresentation(Category.Feature)
            .SetMode(FeatureDefinitionFeatureSet.FeatureSetMode.Union)
            .SetFeatureSet(
                FeatureDefinitionAttributeModifierBuilder
                    .Create($"AttributeModifier{RaceName}BonusAC")
                    .SetGuiPresentation(Category.Feature)
                    .SetModifier(AttributeModifierOperation.Additive, AttributeDefinitions.ArmorClass, 1)
                    .AddToDB(),
                FeatureDefinitionConditionAffinityBuilder
                    .Create($"ConditionAffinity{RaceName}PoisonResilience")
                    .SetGuiPresentationNoContent()
                    .SetConditionType(ConditionDefinitions.ConditionPoisoned)
                    .SetSavingThrowAdvantageType(RuleDefinitions.AdvantageType.Advantage)
                    .AddToDB(),
                FeatureDefinitionConditionAffinitys.ConditionAffinityElfFeyAncestrySleep,
                FeatureDefinitionConditionAffinitys.ConditionAffinityDiseaseImmunity,
                FeatureDefinitionDamageAffinitys.DamageAffinityPoisonResistance
                )
            .AddToDB();

        var raceBattleborn = CharacterRaceDefinitionBuilder
            .Create(CharacterRaceDefinitions.Human, $"Race{RaceName}")
            .SetGuiPresentation(Category.Race, Sprites.GetSprite(RaceName, Resources.Battleborn, 1024, 512))
            .SetFeaturesAtLevel(1,
                FeatureDefinitionMoveModes.MoveModeMove6,
                FeatureDefinitionSenses.SenseDarkvision,
                FeatureDefinitionSenses.SenseNormalVision,
                FlexibleRacesContext.FeatureSetLanguageCommonPlusOne,
                featureSetBattlebornAbilityScoreIncrease,
                featureSetBattlebornArcaneResilience,
                featureSetBattlebornSpecializedInfusion
                )
            .AddToDB();

        var racePresentation = raceBattleborn.RacePresentation; 
        racePresentation.originOptions = new List<string>() { racePresentation.originOptions[1] };

        return raceBattleborn;
    }
}
