using System.Collections.Generic;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.CustomBuilders;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Properties;
using SolastaUnfinishedBusiness.Subclasses;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionFightingStyleChoices;

namespace SolastaUnfinishedBusiness.FightingStyles;

internal sealed class RemarkableTechnique : AbstractFightingStyle
{
    private const string RemarkableTechniqueName = "RemarkableTechnique";

    internal override FightingStyleDefinition FightingStyle { get; } = FightingStyleBuilder
        .Create(RemarkableTechniqueName)
        .SetGuiPresentation(Category.FightingStyle,
            Sprites.GetSprite("RemarkableTechnique", Resources.MartialTactician, 256))
        .SetFeatures(
            GambitsBuilders.GambitPool,
            GambitsBuilders.Learn1Gambit,
            MartialTactician.BuildGambitPoolIncrease(1, "RemarkableTechnique"))
        .AddToDB();

    internal override List<FeatureDefinitionFightingStyleChoice> FightingStyleChoice =>
    [
        CharacterContext.FightingStyleChoiceBarbarian,
        FightingStyleChampionAdditional,
        FightingStyleFighter,
        FightingStylePaladin,
        FightingStyleRanger
    ];
}
