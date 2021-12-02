using System.Collections.Generic;

namespace SolastaCommunityExpansion.CustomFeatureDefinitions
{
    /// <summary>
    /// Imposes disadvantage when attacking anyone but the source of the specified condition.
    /// </summary>
    public class AttackDisadvantageAgainstNonSource : FeatureDefinition, ICombatAffinityProvider
    {
        public string ConditionName { get; set; }

        public RuleDefinitions.SituationalContext SituationalContext => RuleDefinitions.SituationalContext.None;
        public bool CanRageToOvercomeSurprise => false;
        public bool AutoCritical => false;
        public bool CriticalHitImmunity => false;
        public ConditionDefinition RequiredTargetCondition => null;
        public bool IgnoreCover => false;

        public void ComputeAttackModifier(RulesetCharacter myself, RulesetCharacter defender, RulesetAttackMode attackMode, ActionModifier attackModifier, RuleDefinitions.FeatureOrigin featureOrigin)
        {
            foreach (KeyValuePair<string, List<RulesetCondition>> keyValuePair in myself.ConditionsByCategory)
            {
                foreach (RulesetCondition rulesetCondition in keyValuePair.Value)
                {
                    if (rulesetCondition.ConditionDefinition.IsSubtypeOf(ConditionName) && rulesetCondition.SourceGuid != defender.Guid)
                    {
                        attackModifier.AttackAdvantageTrends.Add(new RuleDefinitions.TrendInfo(-1, featureOrigin.sourceType, featureOrigin.sourceName, featureOrigin.source));
                        return;
                    }
                }
            }
        }

        public void ComputeDefenseModifier(RulesetCharacter myself, RulesetCharacter attacker, int sustainedAttacks, bool defenderAlreadyAttackedByAttackerThisTurn, ActionModifier attackModifier, RuleDefinitions.FeatureOrigin featureOrigin)
        {
            return;
        }

        public RuleDefinitions.AdvantageType GetAdvantageOnOpportunityAttackOnMe(RulesetCharacter myself, RulesetCharacter attacker)
        {
            return RuleDefinitions.AdvantageType.None;
        }

        public bool IsImmuneToOpportunityAttack(RulesetCharacter myself, RulesetCharacter attacker)
        {
            return false;
        }
    }
}
