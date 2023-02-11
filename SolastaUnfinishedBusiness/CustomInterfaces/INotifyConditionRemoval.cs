using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.CustomInterfaces;

/// <summary>
///     Implement on a ConditionDefinition to be notified when a condition is removed, or when a creature is about to die
///     with a condition.
/// </summary>
public interface INotifyConditionRemoval
{
    [UsedImplicitly]
    public void AfterConditionRemoved(RulesetActor removedFrom, RulesetCondition rulesetCondition);

    [UsedImplicitly]
    public void BeforeDyingWithCondition(RulesetActor rulesetActor, RulesetCondition rulesetCondition);
}
