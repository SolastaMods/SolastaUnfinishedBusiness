namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface ICustomConditionFeature
{
    // rename method to differ to IDefinitionCustomCode
    public void OnApplyCondition(RulesetCharacter target, RulesetCondition rulesetCondition);
    public void OnRemoveCondition(RulesetCharacter target, RulesetCondition rulesetCondition);
}
