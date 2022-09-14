using System.Collections.Generic;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ConditionDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionFightingStyleChoices;

namespace SolastaUnfinishedBusiness.FightingStyles;

internal sealed class Crippling : AbstractFightingStyle
{
    private CustomFightingStyleDefinition instance;

    [NotNull]
    internal override List<FeatureDefinitionFightingStyleChoice> GetChoiceLists()
    {
        return new List<FeatureDefinitionFightingStyleChoice>
        {
            FightingStyleChampionAdditional, FightingStyleFighter, FightingStyleRanger
        };
    }

    internal override FightingStyleDefinition GetStyle()
    {
        if (instance != null)
        {
            return instance;
        }

        //? Prevent Dash until end of next turn -> how? it's not an action, but has a lot of dedicated code
        //+ Reduce speed by 10 until end of next turn
        //+ Must be a successful melee attack
        //+ NO LIMIT per round (wow!)
        var conditionDefinition = ConditionDefinitionBuilder
            .Create(ConditionHindered_By_Frost, "ConditionFightingStyleCrippling", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentationNoContent()
            .AddToDB();

        conditionDefinition.allowMultipleInstances = true;

        var conditionOperation = new ConditionOperationDescription
        {
            canSaveToCancel = false,
            conditionDefinition = conditionDefinition,
            hasSavingThrow = false,
            operation = ConditionOperationDescription.ConditionOperation.Add,
            saveAffinity = RuleDefinitions.EffectSavingThrowType.None,
            saveOccurence = RuleDefinitions.TurnOccurenceType.EndOfTurn
        };

        var additionalDamage = FeatureDefinitionAdditionalDamageBuilder
            .Create(DatabaseHelper.FeatureDefinitionAdditionalDamages.AdditionalDamageCircleBalanceColdEmbrace,
                "AdditionalDamageCrippling", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation(Category.Feature)
            .SetDamageDice(RuleDefinitions.DieType.D1, 0)
            .SetFrequencyLimit(RuleDefinitions.FeatureLimitedUsage.None)
            .SetNotificationTag("Crippling")
            .SetRequiredProperty(RuleDefinitions.RestrictedContextRequiredProperty.MeleeWeapon)
            .SetTriggerCondition(RuleDefinitions.AdditionalDamageTriggerCondition.AlwaysActive)
            .SetConditionOperations(conditionOperation)
            .AddToDB();

        instance = CustomizableFightingStyleBuilder
            .Create("Crippling", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation(
                Category.FightingStyle,
                DatabaseHelper.CharacterSubclassDefinitions.RangerShadowTamer.GuiPresentation.SpriteReference)
            .SetFeatures(additionalDamage)
            .AddToDB();

        return instance;
    }
}
