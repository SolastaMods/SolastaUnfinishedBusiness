using System;
using System.Linq;
using JetBrains.Annotations;
using SolastaCommunityExpansion.Builders.Features;

namespace SolastaCommunityExpansion.CustomDefinitions;

/// <summary>
///     Imposes disadvantage when attacking anyone but the source of the specified condition.
/// </summary>
public sealed class FeatureDefinitionAttackDisadvantageAgainstNonSource : FeatureDefinition, ICombatAffinityProvider
{
    public string ConditionName { get; set; }

    public RuleDefinitions.SituationalContext SituationalContext => RuleDefinitions.SituationalContext.None;
    public bool CanRageToOvercomeSurprise => false;
    public bool AutoCritical => false;
    public bool CriticalHitImmunity => false;
    [CanBeNull] public ConditionDefinition RequiredCondition => null;
    public bool IgnoreCover => false;

    public void ComputeAttackModifier([NotNull] RulesetCharacter myself, RulesetCharacter defender,
        RulesetAttackMode attackMode, ActionModifier attackModifier, RuleDefinitions.FeatureOrigin featureOrigin)
    {
        if (!myself.ConditionsByCategory.SelectMany(keyValuePair => keyValuePair.Value).Any(rulesetCondition =>
                rulesetCondition.ConditionDefinition.IsSubtypeOf(ConditionName) &&
                rulesetCondition.SourceGuid != defender.Guid))
        {
            return;
        }

        attackModifier.AttackAdvantageTrends.Add(new RuleDefinitions.TrendInfo(-1,
            featureOrigin.sourceType, featureOrigin.sourceName, featureOrigin.source));
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

internal abstract class FeatureDefinitionAttackDisadvantageAgainstNonSourceBuilder
    : FeatureDefinitionBuilder<FeatureDefinitionAttackDisadvantageAgainstNonSource,
        FeatureDefinitionAttackDisadvantageAgainstNonSourceBuilder>
{
    protected FeatureDefinitionAttackDisadvantageAgainstNonSourceBuilder(string name, Guid namespaceGuid) : base(
        name, namespaceGuid)
    {
    }

    [NotNull]
    public FeatureDefinitionAttackDisadvantageAgainstNonSourceBuilder SetConditionName(string conditionName)
    {
        Definition.ConditionName = conditionName;
        return this;
    }
}
