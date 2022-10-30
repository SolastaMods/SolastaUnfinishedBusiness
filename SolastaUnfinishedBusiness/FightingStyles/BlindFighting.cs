using System.Collections.Generic;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.Properties;
using SolastaUnfinishedBusiness.Utils;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionFightingStyleChoices;

namespace SolastaUnfinishedBusiness.FightingStyles;

internal class BlindFighting : AbstractFightingStyle
{
    internal override FightingStyleDefinition FightingStyle { get; } = CustomizableFightingStyleBuilder
        .Create("BlindFighting")
        .SetGuiPresentation(Category.FightingStyle,
            CustomIcons.GetSprite("BlindFighting", Resources.BlindFighting, 256))
        .SetFeatures(FeatureDefinitionSenseBuilder
            .Create("SenseBlindFighting")
            .SetGuiPresentationNoContent(true)
            .SetSense(SenseMode.Type.Blindsight, 2)
            .AddToDB())
        .AddToDB();

    internal override List<FeatureDefinitionFightingStyleChoice> FightingStyleChoice => new()
    {
        FightingStyleChampionAdditional, FightingStyleFighter, FightingStyleRanger
    };
}
