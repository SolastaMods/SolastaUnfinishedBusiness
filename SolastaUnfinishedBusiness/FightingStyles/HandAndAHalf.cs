using System.Collections.Generic;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomInterfaces;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionFightingStyleChoices;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterSubclassDefinitions;

namespace SolastaUnfinishedBusiness.FightingStyles;

internal sealed class HandAndAHalf : AbstractFightingStyle
{
    private const string HandAndAHalfName = "HandAndAHalf";

    internal override FightingStyleDefinition FightingStyle { get; } = FightingStyleBuilder
        .Create(HandAndAHalfName)
        .SetGuiPresentation(Category.FightingStyle, RangerShadowTamer)
        .SetFeatures(
            FeatureDefinitionAttackModifierBuilder
                .Create("AttackModifierHandAndAHalf")
                .SetGuiPresentation(HandAndAHalfName, Category.FightingStyle)
                .SetAttackRollModifier(1)
                .SetDamageRollModifier(2)
                .SetCustomSubFeatures(new RestrictedContextValidator(
                    OperationType.Set, ValidatorsCharacter.MainHandIsVersatileWeapon))
                .AddToDB())
        .AddToDB();

    internal override List<FeatureDefinitionFightingStyleChoice> FightingStyleChoice => new()
    {
        FightingStyleChampionAdditional, FightingStyleFighter, FightingStylePaladin, FightingStyleRanger
    };
}
