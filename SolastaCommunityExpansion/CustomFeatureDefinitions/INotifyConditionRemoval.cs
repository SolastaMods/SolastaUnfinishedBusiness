namespace SolastaCommunityExpansion.CustomFeatureDefinitions
{
    /// <summary>
    /// Implement on a ConditionDefinition to be notified when a condition is removed, or when a creature is about to die with a condition.
    /// </summary>
    public interface INotifyConditionRemoval
    {
        void AfterConditionRemoved(RulesetActor removedFrom, RulesetCondition rulesetCondition);
        void BeforeDyingWithCondition(RulesetActor rulesetActor, RulesetCondition rulesetCondition);
    }
}
