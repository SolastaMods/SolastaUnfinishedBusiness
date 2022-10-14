using System;
using System.Collections.Generic;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomDefinitions;
using SolastaUnfinishedBusiness.CustomInterfaces;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionFightingStyleChoices;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterSubclassDefinitions;
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.FightingStyles;

internal sealed class HandAndAHalf : AbstractFightingStyle
{
    internal override FightingStyleDefinition FightingStyle { get; } = CustomizableFightingStyleBuilder
        .Create("HandAndAHalf")
        .SetGuiPresentation(Category.FightingStyle, RangerShadowTamer)
        .SetFeatures(
            FeatureDefinitionAttackModifierBuilder
                .Create("AttackModifierHandAndAHalf")
                .SetGuiPresentation("HandAndAHalf", Category.FightingStyle)
                .Configure(
                    AttackModifierMethod.FlatValue,
                    1,
                    string.Empty,
                    AttackModifierMethod.FlatValue,
                    1,
                    String.Empty)
                .SetCustomSubFeatures(new RestrictedContextValidator(
                    OperationType.Set, ValidatorsCharacter.MainHandIsVersatileWeapon))
                .AddToDB())
        .AddToDB();

    internal override List<FeatureDefinitionFightingStyleChoice> FightingStyleChoice => new()
    {
        FightingStyleChampionAdditional, FightingStyleFighter, FightingStylePaladin, FightingStyleRanger
    };
}
