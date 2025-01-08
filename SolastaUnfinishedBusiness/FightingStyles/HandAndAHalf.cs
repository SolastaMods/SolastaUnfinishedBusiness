using System.Collections.Generic;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Properties;
using SolastaUnfinishedBusiness.Validators;
using static FeatureDefinitionAttributeModifier;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionFightingStyleChoices;

namespace SolastaUnfinishedBusiness.FightingStyles;

internal sealed class HandAndAHalf : AbstractFightingStyle
{
    private const string HandAndAHalfName = "HandAndAHalf";

    internal override FightingStyleDefinition FightingStyle { get; } = FightingStyleBuilder
        .Create(HandAndAHalfName)
        .SetGuiPresentation(Category.FightingStyle, Sprites.GetSprite("HandAndAHalf", Resources.HandAndAHalf, 256))
        .SetFeatures(
            FeatureDefinitionAttributeModifierBuilder
                .Create("AttributeModifierHandAndAHalf")
                .SetGuiPresentation(HandAndAHalfName, Category.FightingStyle)
                .SetModifier(AttributeModifierOperation.Additive, AttributeDefinitions.ArmorClass, 1)
                .SetSituationalContext(ExtraSituationalContext.HasMeleeWeaponInMainHandAndFreeOffhand)
                .AddToDB(),
            FeatureDefinitionAttackModifierBuilder
                .Create("AttackModifierHandAndAHalf")
                .SetGuiPresentation(HandAndAHalfName, Category.FightingStyle)
                .SetAttackRollModifier(1)
                .AddCustomSubFeatures(ValidatorsCharacter.HasMeleeWeaponInMainHandAndFreeOffhand)
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
