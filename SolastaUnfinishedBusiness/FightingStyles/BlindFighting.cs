using System.Collections.Generic;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Properties;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionFightingStyleChoices;

namespace SolastaUnfinishedBusiness.FightingStyles;

internal class BlindFighting : AbstractFightingStyle
{
    internal override FightingStyleDefinition FightingStyle { get; } = FightingStyleBuilder
        .Create("BlindFighting")
        .SetGuiPresentation(Category.FightingStyle, Sprites.GetSprite("BlindFighting", Resources.BlindFighting, 256))
        .SetFeatures(
            FeatureDefinitionSenseBuilder
                .Create("SenseBlindFighting")
                .SetGuiPresentationNoContent(true)
                .SetSense(SenseMode.Type.Blindsight, 2)
                .AddToDB())
        .AddToDB();

    internal override List<FeatureDefinitionFightingStyleChoice> FightingStyleChoice =>
    [
        CharacterContext.FightingStyleChoiceBarbarian,
        CharacterContext.FightingStyleChoiceMonk,
        CharacterContext.FightingStyleChoiceRogue,
        FightingStyleChampionAdditional,
        FightingStyleFighter,
        FightingStylePaladin,
        FightingStyleRanger
    ];
}
