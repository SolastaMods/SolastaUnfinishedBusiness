using System;
using System.Collections.Generic;
using SolastaCommunityExpansion.Api;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaCommunityExpansion.CustomDefinitions;
using static SolastaCommunityExpansion.Api.DatabaseHelper.ConditionDefinitions;
using static SolastaCommunityExpansion.Api.DatabaseHelper.FeatureDefinitionFightingStyleChoices;

namespace SolastaCommunityExpansion.FightingStyles;

internal class Crippling : AbstractFightingStyle
{
    private readonly Guid Namespace = new("3f7f25de-0ff9-4b63-b38d-8cd7f3a381fc");
    private FightingStyleDefinitionCustomizable instance;

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
            .Create(ConditionHindered_By_Frost, "CripplingConditionDefinition", Namespace)
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
                "CripplingAdditionalDamage", Namespace)
            .SetGuiPresentation(Category.Modifier)
            .SetDamageDice(RuleDefinitions.DieType.D1, 0)
            .SetFrequencyLimit(RuleDefinitions.FeatureLimitedUsage.None)
            .SetNotificationTag("CripplingFightingStyle")
            .SetRequiredProperty(RuleDefinitions.AdditionalDamageRequiredProperty.MeleeWeapon)
            .SetTriggerCondition(RuleDefinitions.AdditionalDamageTriggerCondition.AlwaysActive)
            .SetConditionOperations(conditionOperation)
            .AddToDB();

        instance = CustomizableFightingStyleBuilder
            .Create("Crippling", "b570d166-c65c-4a68-ab78-aeb16d491fce")
            .SetGuiPresentation("Crippling", Category.FightingStyle,
                DatabaseHelper.CharacterSubclassDefinitions.RangerShadowTamer.GuiPresentation.SpriteReference)
            .SetFeatures(additionalDamage)
            .AddToDB();

        return instance;
    }
}
