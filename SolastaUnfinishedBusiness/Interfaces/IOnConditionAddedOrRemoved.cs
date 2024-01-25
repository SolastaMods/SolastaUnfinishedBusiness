namespace SolastaUnfinishedBusiness.Interfaces;

public interface IOnConditionAddedOrRemoved
{
    public void OnConditionAdded(RulesetCharacter target, RulesetCondition rulesetCondition);
    public void OnConditionRemoved(RulesetCharacter target, RulesetCondition rulesetCondition);
}
