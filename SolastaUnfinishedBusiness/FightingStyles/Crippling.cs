using System.Collections.Generic;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ConditionDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionFightingStyleChoices;

namespace SolastaUnfinishedBusiness.FightingStyles;

internal sealed class Crippling : AbstractFightingStyle
{
    internal Crippling()
    {
        //? Prevent Dash until end of next turn -> how? it's not an action, but has a lot of dedicated code
        //+ Reduce speed by 10 until end of next turn
        //+ Must be a successful melee attack
        //+ NO LIMIT per round (wow!)
        var conditionFightingStyleCrippling = ConditionDefinitionBuilder
            .Create(ConditionHindered_By_Frost, "ConditionFightingStyleCrippling")
            .SetGuiPresentationNoContent()
            .SetAllowMultipleInstances(true)
            .AddToDB();

        var conditionOperation = new ConditionOperationDescription
        {
            canSaveToCancel = false,
            conditionDefinition = conditionFightingStyleCrippling,
            hasSavingThrow = false,
            operation = ConditionOperationDescription.ConditionOperation.Add,
            saveAffinity = RuleDefinitions.EffectSavingThrowType.None,
            saveOccurence = RuleDefinitions.TurnOccurenceType.EndOfTurn
        };

        var additionalDamageCrippling = FeatureDefinitionAdditionalDamageBuilder
            .Create(DatabaseHelper.FeatureDefinitionAdditionalDamages.AdditionalDamageCircleBalanceColdEmbrace,
                "AdditionalDamageCrippling")
            .SetGuiPresentation(Category.Feature)
            .SetDamageDice(RuleDefinitions.DieType.D1, 0)
            .SetFrequencyLimit(RuleDefinitions.FeatureLimitedUsage.None)
            .SetNotificationTag("Crippling")
            .SetRequiredProperty(RuleDefinitions.RestrictedContextRequiredProperty.MeleeWeapon)
            .SetTriggerCondition(RuleDefinitions.AdditionalDamageTriggerCondition.AlwaysActive)
            .SetConditionOperations(conditionOperation)
            .AddToDB();

        FightingStyle = CustomizableFightingStyleBuilder
            .Create("Crippling")
            .SetGuiPresentation(
                Category.FightingStyle,
                DatabaseHelper.CharacterSubclassDefinitions.RangerShadowTamer.GuiPresentation.SpriteReference)
            .SetFeatures(additionalDamageCrippling)
            .AddToDB();
    }

    internal override FightingStyleDefinition FightingStyle { get; }

    internal override List<FeatureDefinitionFightingStyleChoice> FightingStyleChoice => new()
    {
        FightingStyleChampionAdditional, FightingStyleFighter, FightingStyleRanger
    };
}
