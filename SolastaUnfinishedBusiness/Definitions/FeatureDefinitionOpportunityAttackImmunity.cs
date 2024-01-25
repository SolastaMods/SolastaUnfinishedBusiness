using System.Linq;
using JetBrains.Annotations;
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.Definitions;

/// <summary>
///     Grants you immunity to opportunity attacks when the attacker has the specified condition (inflicted by you).
/// </summary>
internal sealed class FeatureDefinitionOpportunityAttackImmunity : FeatureDefinition, ICombatAffinityProvider
{
    internal string ConditionName { get; set; }

    public void ComputeDefenseModifier(RulesetCharacter myself, RulesetCharacter attacker, int sustainedAttacks,
        bool defenderAlreadyAttackedByAttackerThisTurn, ActionModifier attackModifier,
        FeatureOrigin featureOrigin,
        float distance)
    {
        // Intentionally empty?
    }

    public SituationalContext SituationalContext => SituationalContext.None;
    public bool CanRageToOvercomeSurprise => false;
    public bool AutoCritical => false;
    public bool CriticalHitImmunity => false;
    [CanBeNull] public ConditionDefinition RequiredCondition => null;
    public bool IgnoreCover => false;
    public CoverType PermanentCover => CoverType.None;
    public AdvantageType ReadyAttackAdvantage => AdvantageType.None;
    public bool ShoveOnReadyAttackHit => false;

    public void ComputeAttackModifier(RulesetCharacter myself, RulesetCharacter defender, RulesetAttackMode attackMode,
        ActionModifier attackModifier, FeatureOrigin featureOrigin, int bardicDieRoll, float distance)
    {
        // Intentionally empty?
    }

    public AdvantageType GetAdvantageOnOpportunityAttackOnMe(RulesetCharacter myself,
        RulesetCharacter attacker, float distance)
    {
        return AdvantageType.None;
    }

    public bool IsImmuneToOpportunityAttack(RulesetCharacter myself, [NotNull] RulesetCharacter attacker,
        float distance)
    {
        return attacker.ConditionsByCategory.SelectMany(keyValuePair => keyValuePair.Value).Any(rulesetCondition =>
            rulesetCondition.SourceGuid == myself.Guid &&
            rulesetCondition.ConditionDefinition.IsSubtypeOf(ConditionName));
    }
}
