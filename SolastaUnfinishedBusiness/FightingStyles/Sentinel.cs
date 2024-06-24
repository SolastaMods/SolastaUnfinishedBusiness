using System.Collections;
using System.Collections.Generic;
using SolastaUnfinishedBusiness.Behaviors.Specific;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Properties;
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.FightingStyles;

internal sealed class Sentinel : AbstractFightingStyle
{
    internal const string SentinelName = "Sentinel";
    
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
                            CustomConditionsContext.StopMovement,
                            ConditionDefinitionBuilder
                                .Create("ConditionPreventAttackAtReach")
                                .SetGuiPresentationNoContent(true)
                                .SetSilent(Silent.WhenAddedOrRemoved)
                                .SetFeatures(
                                    // this is a hack to ensure game engine won't execute the attack even at reach
                                    // given that game AI will only run an enemy towards an ally with an attack intention
                                    // this should be good enough as enemy won't run next to other allies
                                    FeatureDefinitionActionAffinityBuilder
                                        .Create($"ActionAffinity{SentinelName}StopMovement")
                                        .SetGuiPresentationNoContent(true)
                                        .SetForbiddenActions(
                                            ActionDefinitions.Id.Shove,
                                            ActionDefinitions.Id.ShoveBonus,
                                            ActionDefinitions.Id.AttackMain,
                                            ActionDefinitions.Id.AttackOff,
                                            ActionDefinitions.Id.AttackFree)
                                        .AddToDB())
                                .AddToDB()))
                .AddToDB())
        .AddToDB();

    internal override List<FeatureDefinitionFightingStyleChoice> FightingStyleChoice => [];

    private sealed class PhysicalAttackFinishedByMeFeatSentinel(
        ConditionDefinition conditionSentinelStopMovement,
        ConditionDefinition conditionPreventAttackAtReach) : IPhysicalAttackBeforeHitConfirmedOnEnemy
    {
        public IEnumerator OnPhysicalAttackBeforeHitConfirmedOnEnemy(
            GameLocationBattleManager battleManager,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier actionModifier,
            RulesetAttackMode attackMode,
            bool rangedAttack,
            AdvantageType advantageType,
            List<EffectForm> actualEffectForms,
            bool firstTarget,
            bool criticalHit)
        {
            if (attackMode.ActionType != ActionDefinitions.ActionType.Reaction ||
                attackMode.AttackTags.Contains(AttacksOfOpportunity.NotAoOTag))
            {
                yield break;
            }

            var rulesetAttacker = attacker.RulesetCharacter;
            var rulesetDefender = defender.RulesetActor;

            rulesetDefender.InflictCondition(
                conditionSentinelStopMovement.Name,
                DurationType.Round,
                0,
                TurnOccurenceType.EndOfSourceTurn,
                AttributeDefinitions.TagEffect,
                rulesetAttacker.guid,
                rulesetAttacker.CurrentFaction.Name,
                1,
                conditionSentinelStopMovement.Name,
                0,
                0,
                0);

            if (attackMode.Reach)
            {
                rulesetDefender.InflictCondition(
                    conditionPreventAttackAtReach.Name,
                    DurationType.Round,
                    0,
                    TurnOccurenceType.EndOfTurn,
                    AttributeDefinitions.TagEffect,
                    rulesetAttacker.guid,
                    rulesetAttacker.CurrentFaction.Name,
                    1,
                    conditionPreventAttackAtReach.Name,
                    0,
                    0,
                    0); 
            }
        }
    }
}
