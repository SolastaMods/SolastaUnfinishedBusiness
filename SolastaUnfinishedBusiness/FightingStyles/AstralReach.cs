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
    internal const string AstralReachFeatureName = $"Feature{AstralReachName}";

    internal override FightingStyleDefinition FightingStyle { get; } = FightingStyleBuilder
        .Create(AstralReachName)
        .SetGuiPresentation(Category.FightingStyle, Sprites.GetSprite(AstralReachName, Resources.Lunger, 256))
        .SetFeatures(
            FeatureDefinitionBuilder
                .Create(AstralReachFeatureName)
                .SetGuiPresentationNoContent(true)
                .AddCustomSubFeatures(
                    new IncreaseWeaponReach(1, (attackMode, _, character) =>
                        ValidatorsCharacter.HasFreeHandConsiderGrapple(character) &&
                        ValidatorsWeapon.IsUnarmed(attackMode)))
                .AddToDB())
        .AddToDB();

    internal override List<FeatureDefinitionFightingStyleChoice> FightingStyleChoice =>
    [
        ClassesContext.FightingStyleChoiceBarbarian,
        ClassesContext.FightingStyleChoiceMonk,
        ClassesContext.FightingStyleChoiceRogue,
        FightingStyleChampionAdditional,
        FightingStyleFighter,
        FightingStylePaladin,
        FightingStyleRanger
    ];
}
