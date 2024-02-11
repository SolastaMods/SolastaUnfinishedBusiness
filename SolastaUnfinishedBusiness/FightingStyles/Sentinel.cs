using System.Collections;
using System.Collections.Generic;
using SolastaUnfinishedBusiness.Behaviors.Specific;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Properties;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionFightingStyleChoices;
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.FightingStyles;

internal sealed class Sentinel : AbstractFightingStyle
{
    private const string SentinelName = "Sentinel";

    internal override FightingStyleDefinition FightingStyle { get; } = FightingStyleBuilder
        .Create(SentinelName)
        .SetGuiPresentation(Category.FightingStyle, Sprites.GetSprite("Sentinel", Resources.Sentinel, 256))
        .SetFeatures(
            FeatureDefinitionBuilder
                .Create("OnAttackHitEffectFeatSentinel")
                .SetGuiPresentationNoContent(true)
                .AddCustomSubFeatures(
                    AttacksOfOpportunity.IgnoreDisengage,
                    AttacksOfOpportunity.SentinelFeatMarker,
                    new PhysicalAttackFinishedByMeFeatSentinel(
                        ConditionDefinitionBuilder
                            .Create(CustomConditionsContext.StopMovement, "ConditionStopMovementSentinel")
                            .SetSpecialInterruptions(ConditionInterruption.AnyBattleTurnEnd)
                            .AddToDB()))
                .AddToDB())
        .AddToDB();

    internal override List<FeatureDefinitionFightingStyleChoice> FightingStyleChoice =>
    [
        CharacterContext.FightingStyleChoiceBarbarian,
        FightingStyleChampionAdditional,
        FightingStyleFighter,
        FightingStylePaladin,
        FightingStyleRanger
    ];

    private sealed class PhysicalAttackFinishedByMeFeatSentinel : IPhysicalAttackFinishedByMe
    {
        private readonly ConditionDefinition _conditionSentinelStopMovement;

        internal PhysicalAttackFinishedByMeFeatSentinel(ConditionDefinition conditionSentinelStopMovement)
        {
            _conditionSentinelStopMovement = conditionSentinelStopMovement;
        }

        public IEnumerator OnPhysicalAttackFinishedByMe(
            GameLocationBattleManager battleManager,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RulesetAttackMode attackMode,
            RollOutcome rollOutcome,
            int damageAmount)
        {
            if (rollOutcome is not (RollOutcome.Success or RollOutcome.CriticalSuccess))
            {
                yield break;
            }

            if (attackMode is not { ActionType: ActionDefinitions.ActionType.Reaction })
            {
                yield break;
            }

            if (attackMode.AttackTags.Contains(AttacksOfOpportunity.NotAoOTag))
            {
                yield break;
            }

            var rulesetAttacker = attacker.RulesetCharacter;
            var rulesetDefender = defender.RulesetCharacter;

            if (rulesetDefender is not { IsDeadOrDyingOrUnconscious: false } ||
                rulesetAttacker is not { IsDeadOrDyingOrUnconscious: false })
            {
                yield break;
            }

            rulesetDefender.InflictCondition(
                _conditionSentinelStopMovement.Name,
                DurationType.Round,
                0,
                TurnOccurenceType.EndOfSourceTurn,
                AttributeDefinitions.TagEffect,
                rulesetAttacker.guid,
                rulesetAttacker.CurrentFaction.Name,
                1,
                _conditionSentinelStopMovement.Name,
                0,
                0,
                0);
        }
    }
}
