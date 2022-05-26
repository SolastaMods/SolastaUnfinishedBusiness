using System;
using SolastaCommunityExpansion.Builders.Features;

namespace SolastaCommunityExpansion.CustomDefinitions
{
    /// <summary>
    ///     Grants you immunity to opportunity attacks when the attacker has the specified condition (inflicted by you).
    /// </summary>
    public class FeatureDefinitionOpportunityAttackImmunityIfAttackerHasCondition : FeatureDefinition,
        ICombatAffinityProvider
    {
        public string ConditionName { get; set; }

        public RuleDefinitions.SituationalContext SituationalContext => RuleDefinitions.SituationalContext.None;
        public bool CanRageToOvercomeSurprise => false;
        public bool AutoCritical => false;
        public bool CriticalHitImmunity => false;
        public ConditionDefinition RequiredCondition => null;
        public bool IgnoreCover => false;

        public void ComputeAttackModifier(RulesetCharacter myself, RulesetCharacter defender,
            RulesetAttackMode attackMode, ActionModifier attackModifier, RuleDefinitions.FeatureOrigin featureOrigin)
        {
            // Intentionally empty?
        }

        public void ComputeDefenseModifier(RulesetCharacter myself, RulesetCharacter attacker, int sustainedAttacks,
            bool defenderAlreadyAttackedByAttackerThisTurn, ActionModifier attackModifier,
            RuleDefinitions.FeatureOrigin featureOrigin)
        {
            // Intentionally empty?
        }

        public RuleDefinitions.AdvantageType GetAdvantageOnOpportunityAttackOnMe(RulesetCharacter myself,
            RulesetCharacter attacker)
        {
            return RuleDefinitions.AdvantageType.None;
        }

        public bool IsImmuneToOpportunityAttack(RulesetCharacter myself, RulesetCharacter attacker)
        {
            foreach (var keyValuePair in attacker.ConditionsByCategory)
            {
                foreach (var rulesetCondition in keyValuePair.Value)
                {
                    if (rulesetCondition.SourceGuid == myself.Guid &&
                        rulesetCondition.ConditionDefinition.IsSubtypeOf(ConditionName))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }

    internal class FeatureDefinitionOpportunityAttackImmunityIfAttackerHasConditionBuilder
        : FeatureDefinitionBuilder<FeatureDefinitionOpportunityAttackImmunityIfAttackerHasCondition,
            FeatureDefinitionOpportunityAttackImmunityIfAttackerHasConditionBuilder>
    {
        protected FeatureDefinitionOpportunityAttackImmunityIfAttackerHasConditionBuilder(string name,
            Guid namespaceGuid) : base(name, namespaceGuid)
        {
        }

        public FeatureDefinitionOpportunityAttackImmunityIfAttackerHasConditionBuilder SetConditionName(
            string conditionName)
        {
            Definition.ConditionName = conditionName;
            return this;
        }
    }
}
