using System;
using System.Collections.Generic;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaCommunityExpansion.CustomDefinitions;
using static RuleDefinitions;
using static SolastaModApi.DatabaseHelper;

namespace SolastaCommunityExpansion.Feats;

public static class EWFeats
{
    private static readonly Guid GUID = new("B4ED480F-2D06-4EB1-8732-9A721D80DD1A");

    public static void CreateFeats(List<FeatDefinition> feats)
    {
        var restrained = ConditionDefinitions.ConditionRestrained;

        var stopMovementCondition = ConditionDefinitionBuilder
            .Create("SentinelStopMovementCondition", GUID)
            .SetGuiPresentation(Category.Condition, Gui.NoLocalization, restrained.GuiPresentation.SpriteReference)
            .SetConditionType(ConditionType.Detrimental)
            .SetFeatures(
                FeatureDefinitionMovementAffinitys.MovementAffinityConditionRestrained,
                FeatureDefinitionActionAffinitys.ActionAffinityConditionRestrained
            )
            .AddToDB();

        feats.Add(FeatDefinitionBuilder
            .Create("FeatSentinel", GUID)
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(FeatureDefinitionOnAttackHitEffectBuilder
                .Create("FeatSentinelFeature", GUID)
                .SetGuiPresentationNoContent(true)
                .SetOnAttackHitDelegates(null, (attacker, defender, outcome, actionParams, mode, modifier) =>
                {
                    if (outcome != RollOutcome.Success && outcome != RollOutcome.CriticalSuccess)
                    {
                        return;
                    }

                    if (mode is not {ActionType: ActionDefinitions.ActionType.Reaction})
                    {
                        return;
                    }

                    if (mode.AttackTags.Contains(AttacksOfOpportunity.NotAoOTag))
                    {
                        return;
                    }

                    var character = defender.RulesetCharacter;

                    character.AddConditionOfCategory(AttributeDefinitions.TagCombat,
                        RulesetCondition.CreateActiveCondition(character.Guid,
                            stopMovementCondition,
                            DurationType.Round,
                            1,
                            TurnOccurenceType.StartOfTurn,
                            attacker.Guid,
                            string.Empty
                        ));
                })
                .SetCustomSubFeatures(
                    AttacksOfOpportunity.CanIgnoreDisengage,
                    AttacksOfOpportunity.SentinelFeatMarker
                )
                .AddToDB())
            .AddToDB());
    }
}