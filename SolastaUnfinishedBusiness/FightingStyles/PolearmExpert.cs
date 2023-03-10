using System.Collections.Generic;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Properties;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionFightingStyleChoices;

namespace SolastaUnfinishedBusiness.FightingStyles;

internal sealed class PolearmExpert : AbstractFightingStyle
{
    internal override FightingStyleDefinition FightingStyle { get; } = FightingStyleBuilder
        .Create("PolearmExpert")
        .SetGuiPresentation(Category.FightingStyle, Sprites.GetSprite("PolearmExpert", Resources.PolearmExpert, 256))
        .SetFeatures(FeatureDefinitionBuilder
            .Create("FeaturePolearm")
            .SetGuiPresentationNoContent(true)
            .SetCustomSubFeatures(
                new CanMakeAoOOnReachEntered
                {
                    WeaponValidator = (mode, weapon, _) =>
                        ValidatorsWeapon.IsPolearm(weapon ?? mode?.SourceObject as RulesetItem)
                },
                new AddPolearmFollowupAttack())
            .AddToDB())
        .AddToDB();

    internal override List<FeatureDefinitionFightingStyleChoice> FightingStyleChoice => new()
    {
        FightingStyleChampionAdditional, FightingStyleFighter, FightingStylePaladin, FightingStyleRanger
    };
}
