using System.Collections.Generic;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Properties;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ConditionDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionFightingStyleChoices;
using static RuleDefinitions;
using static FeatureDefinitionAttributeModifier;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionMovementAffinitys;

namespace SolastaUnfinishedBusiness.FightingStyles;

internal sealed class Crippling : AbstractFightingStyle
{
    private const string Name = "Crippling";

    internal override FightingStyleDefinition FightingStyle { get; } = FightingStyleBuilder
        .Create(Name)
        .SetGuiPresentation(Category.FightingStyle, Sprites.GetSprite("Crippling", Resources.Crippling, 256))
        .SetFeatures(
            FeatureDefinitionAdditionalDamageBuilder
                .Create("AdditionalDamageCrippling")
                .SetGuiPresentationNoContent(true)
                .SetNotificationTag(Name)
                .SetRequiredProperty(RestrictedContextRequiredProperty.MeleeWeapon)
                .SetAttackModeOnly()
                .SetConditionOperations(
                    new ConditionOperationDescription
                    {
                        conditionDefinition = ConditionDefinitionBuilder
                            .Create(ConditionHindered_By_Frost, "ConditionFightingStyleCrippling")
                            .SetGuiPresentation(Category.Condition, ConditionSlowed)
                            .SetSpecialDuration(DurationType.Round, 2)
                            .SetParentCondition(ConditionHindered)
                            .SetFeatures(
                                MovementAffinityConditionHindered,
                                FeatureDefinitionAttributeModifierBuilder
                                    .Create("AttributeModifierCripplingACDebuff")
                                    .SetGuiPresentationNoContent(true)
                                    .SetModifier(AttributeModifierOperation.Additive,
                                        AttributeDefinitions.ArmorClass, -1)
                                    .AddToDB())
                            .AddToDB(),
                        operation = ConditionOperationDescription.ConditionOperation.Add
                    })
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
