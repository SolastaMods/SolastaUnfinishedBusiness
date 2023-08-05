namespace SolastaUnfinishedBusiness.CustomInterfaces;

// Used in RulesetConditionCustom
internal interface IBindToRulesetConditionCustom
{
    void ReplaceRulesetCondition(RulesetCondition originalRulesetCondition,
        out RulesetCondition replacedRulesetCondition);
}
