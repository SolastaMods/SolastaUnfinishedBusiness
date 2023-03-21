using System.Collections.Generic;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Properties;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionFightingStyleChoices;

namespace SolastaUnfinishedBusiness.FightingStyles;

internal sealed class HandAndAHalf : AbstractFightingStyle
{
    private const string HandAndAHalfName = "HandAndAHalf";

    internal override FightingStyleDefinition FightingStyle { get; } = FightingStyleBuilder
        .Create(HandAndAHalfName)
        .SetGuiPresentation(Category.FightingStyle, Sprites.GetSprite("HandAndAHalf", Resources.HandAndAHalf, 256))
        .SetFeatures(
            FeatureDefinitionAttackModifierBuilder
                .Create("AttackModifierHandAndAHalf")
                .SetGuiPresentation(HandAndAHalfName, Category.FightingStyle)
                .SetAttackRollModifier(1)
                .SetDamageRollModifier(1)
                .SetCustomSubFeatures(ValidatorsCharacter.HasTwoHandedVersatileWeapon)
                .AddToDB())
        .AddToDB();

    internal override List<FeatureDefinitionFightingStyleChoice> FightingStyleChoice => new()
    {
        FightingStyleChampionAdditional, FightingStyleFighter, FightingStylePaladin, FightingStyleRanger
    };
}
