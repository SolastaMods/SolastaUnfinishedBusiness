using System.Collections.Generic;
using System.Linq;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Models;
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
                        ValidatorsWeapon.IsWeaponType(
                            mode?.SourceObject as RulesetItem ?? weapon, CustomWeaponsContext.PolearmWeaponTypes)
                },
                CustomWeaponsContext.PolearmWeaponTypes
                    .Select(weaponType => new AddPolearmFollowUpAttack(weaponType))
                    .ToArray())
            .AddToDB())
        .AddToDB();

    internal override List<FeatureDefinitionFightingStyleChoice> FightingStyleChoice => new()
    {
        FightingStyleChampionAdditional, FightingStyleFighter, FightingStylePaladin, FightingStyleRanger
    };
}
