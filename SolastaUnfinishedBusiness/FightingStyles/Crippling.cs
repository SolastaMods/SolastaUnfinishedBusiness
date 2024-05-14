using System.Collections.Generic;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Properties;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ConditionDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionFightingStyleChoices;
using static RuleDefinitions;

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
                .AddConditionOperation(
                    ConditionOperationDescription.ConditionOperation.Add,
                    ConditionDefinitionBuilder
                        .Create(ConditionHindered, "ConditionFightingStyleCrippling")
                        .SetGuiPresentation(Category.Condition, ConditionSlowed)
                        .SetParentCondition(ConditionHindered)
                        .SetFeatures()
                        .SetSpecialDuration(DurationType.Round, 1, TurnOccurenceType.EndOfSourceTurn)
                        .AddToDB())
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
