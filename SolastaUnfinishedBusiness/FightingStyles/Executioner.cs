using System.Collections.Generic;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterSubclassDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionFightingStyleChoices;

namespace SolastaUnfinishedBusiness.FightingStyles;

internal sealed class Executioner : AbstractFightingStyle
{
    private const string ExecutionerName = "Executioner";

    internal override FightingStyleDefinition FightingStyle { get; } = FightingStyleBuilder
        .Create(ExecutionerName)
        .SetGuiPresentation(Category.FightingStyle, PathMagebane)
        .SetFeatures(FeatureDefinitionAdditionalDamageBuilder
            .Create("AdditionalDamageFightingStyleExecutioner")
            .SetGuiPresentationNoContent(true)
            .SetNotificationTag(ExecutionerName)
            .SetDamageValueDetermination(AdditionalDamageValueDetermination.ProficiencyBonus)
            .SetRequiredProperty(RestrictedContextRequiredProperty.Weapon)
            .SetCustomSubFeatures(
                ValidatorsCharacter.HasAnyOfConditions(
                    ConditionBlinded,
                    ConditionFrightened,
                    ConditionRestrained,
                    ConditionIncapacitated,
                    ConditionParalyzed,
                    ConditionProne,
                    ConditionStunned))
            .AddToDB())
        .AddToDB();

    internal override List<FeatureDefinitionFightingStyleChoice> FightingStyleChoice => new()
    {
        FightingStyleChampionAdditional, FightingStyleFighter, FightingStylePaladin, FightingStyleRanger
    };
}
