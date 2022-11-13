using System.Collections.Generic;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionFightingStyleChoices;

namespace SolastaUnfinishedBusiness.FightingStyles;

internal sealed class PolearmExpert : AbstractFightingStyle
{
    internal override FightingStyleDefinition FightingStyle { get; } = FightingStyleBuilder
        .Create("PolearmExpert")
        .SetGuiPresentation(Category.FightingStyle, DatabaseHelper.CharacterSubclassDefinitions.SorcerousChildRift)
        .SetFeatures(FeatureDefinitionBuilder
            .Create("FeaturePolearm")
            .SetGuiPresentationNoContent(true)
            .SetCustomSubFeatures(
                new CanMakeAoOOnReachEntered(ValidatorsCharacter.HasPolearm),
                new AddPolearmFollowupAttack())
            .AddToDB())
        .AddToDB();

    internal override List<FeatureDefinitionFightingStyleChoice> FightingStyleChoice => new()
    {
        FightingStyleChampionAdditional, FightingStyleFighter, FightingStylePaladin, FightingStyleRanger
    };
}
