using System.Collections.Generic;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ConditionDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionFightingStyleChoices;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionAdditionalDamages;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterSubclassDefinitions;

namespace SolastaUnfinishedBusiness.FightingStyles;

internal sealed class Crippling : AbstractFightingStyle
{
    internal override FightingStyleDefinition FightingStyle { get; } = CustomizableFightingStyleBuilder
        .Create("Crippling")
        .SetGuiPresentation(Category.FightingStyle, RangerShadowTamer.GuiPresentation.SpriteReference)
        .SetFeatures(
            FeatureDefinitionAdditionalDamageBuilder
                .Create(AdditionalDamageCircleBalanceColdEmbrace, "AdditionalDamageCrippling")
                .SetGuiPresentation(Category.Feature)
                .SetDamageDice(RuleDefinitions.DieType.D1, 0)
                .SetFrequencyLimit(RuleDefinitions.FeatureLimitedUsage.None)
                .SetNotificationTag("Crippling")
                .SetRequiredProperty(RuleDefinitions.RestrictedContextRequiredProperty.MeleeWeapon)
                .SetTriggerCondition(RuleDefinitions.AdditionalDamageTriggerCondition.AlwaysActive)
                .SetConditionOperations(
                    new ConditionOperationDescription
                    {
                        canSaveToCancel = false,
                        conditionDefinition = ConditionDefinitionBuilder
                            .Create(ConditionHindered_By_Frost, "ConditionFightingStyleCrippling")
                            .SetGuiPresentationNoContent()
                            .SetAllowMultipleInstances(true)
                            .AddToDB(),
                        hasSavingThrow = false,
                        operation = ConditionOperationDescription.ConditionOperation.Add,
                        saveAffinity = RuleDefinitions.EffectSavingThrowType.None,
                        saveOccurence = RuleDefinitions.TurnOccurenceType.EndOfTurn
                    })
                .AddToDB())
        .AddToDB();

    internal override List<FeatureDefinitionFightingStyleChoice> FightingStyleChoice => new()
    {
        FightingStyleChampionAdditional, FightingStyleFighter, FightingStyleRanger
    };
}
