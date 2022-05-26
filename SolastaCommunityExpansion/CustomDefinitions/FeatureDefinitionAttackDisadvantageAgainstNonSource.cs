using System;
using SolastaCommunityExpansion.Builders.Features;

namespace SolastaCommunityExpansion.CustomDefinitions
{
    /// <summary>
    ///     Imposes disadvantage when attacking anyone but the source of the specified condition.
    /// </summary>
    public class FeatureDefinitionAttackDisadvantageAgainstNonSource : FeatureDefinition, ICombatAffinityProvider
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
            foreach (var keyValuePair in myself.ConditionsByCategory)
            {
                foreach (var rulesetCondition in keyValuePair.Value)
                {
                    if (rulesetCondition.ConditionDefinition.IsSubtypeOf(ConditionName) &&
                        rulesetCondition.SourceGuid != defender.Guid)
                    {
                        attackModifier.AttackAdvantageTrends.Add(new RuleDefinitions.TrendInfo(-1,
                            featureOrigin.sourceType, featureOrigin.sourceName, featureOrigin.source));
                        return;
                    }
                }
            }
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
            return false;
        }
    }

    internal class FeatureDefinitionAttackDisadvantageAgainstNonSourceBuilder
        : FeatureDefinitionBuilder<FeatureDefinitionAttackDisadvantageAgainstNonSource,
            FeatureDefinitionAttackDisadvantageAgainstNonSourceBuilder>
    {
        protected FeatureDefinitionAttackDisadvantageAgainstNonSourceBuilder(string name, Guid namespaceGuid) : base(
            name, namespaceGuid)
        {
        }

        public FeatureDefinitionAttackDisadvantageAgainstNonSourceBuilder SetConditionName(string conditionName)
        {
            Definition.ConditionName = conditionName;
            return this;
        }
    }
}
