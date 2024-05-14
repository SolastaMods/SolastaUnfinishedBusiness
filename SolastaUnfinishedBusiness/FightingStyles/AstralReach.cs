using System.Collections.Generic;
using SolastaUnfinishedBusiness.Behaviors;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Properties;
using SolastaUnfinishedBusiness.Validators;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionFightingStyleChoices;

namespace SolastaUnfinishedBusiness.FightingStyles;

internal sealed class AstralReach : AbstractFightingStyle
{
    private const string AstralReachName = "AstralReach";

    internal override FightingStyleDefinition FightingStyle { get; } = FightingStyleBuilder
        .Create(AstralReachName)
        .SetGuiPresentation(Category.FightingStyle, Sprites.GetSprite(AstralReachName, Resources.Lunger, 256))
        .SetFeatures(
            FeatureDefinitionBuilder
                .Create($"Feature{AstralReachName}")
                .SetGuiPresentationNoContent(true)
                .AddCustomSubFeatures(new IncreaseWeaponReach(1, (attackMode, _, _) =>
                    ValidatorsWeapon.IsUnarmed(attackMode)))
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
