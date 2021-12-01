using System.Collections.Generic;

namespace SolastaCommunityExpansion.CustomFeatureDefinitions
{
    /// <summary>
    /// Grants you immunity to opportunity attacks when the attacker has the specified condition (inflicted by you).
    /// </summary>
    public class OpportunityAttackImmunityIfAttackerHasCondition : FeatureDefinition, ICombatAffinityProvider
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
            return;
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
            foreach (KeyValuePair<string, List<RulesetCondition>> keyValuePair in attacker.ConditionsByCategory)
            {
                foreach (RulesetCondition rulesetCondition in keyValuePair.Value)
                {
                    if (rulesetCondition.SourceGuid == myself.Guid && rulesetCondition.ConditionDefinition.IsSubtypeOf(ConditionName))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
