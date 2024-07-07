using System.Collections.Generic;
using SolastaUnfinishedBusiness.Behaviors;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Properties;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionFightingStyleChoices;

namespace SolastaUnfinishedBusiness.FightingStyles;

internal class ShieldExpert : AbstractFightingStyle
{
    internal const string ShieldExpertName = "ShieldExpert";

    internal override FightingStyleDefinition FightingStyle { get; } = FightingStyleBuilder
        .Create(ShieldExpertName)
        .SetGuiPresentation(Category.FightingStyle, Sprites.GetSprite(ShieldExpertName, Resources.ShieldExpert, 256))
        .SetFeatures(
            FeatureDefinitionBuilder
                .Create("AddExtraAttackShieldExpert")
                .SetGuiPresentationNoContent(true)
                .AddCustomSubFeatures(new AddBonusShieldAttack())
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
