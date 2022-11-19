namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface ICustomConditionFeature
{
    public void ApplyFeature(RulesetCharacter target, RulesetCondition rulesetCondition);
    public void RemoveFeature(RulesetCharacter target, RulesetCondition rulesetCondition);
}
