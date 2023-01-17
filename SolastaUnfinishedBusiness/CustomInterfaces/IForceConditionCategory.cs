namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface IForceConditionCategory
{
    string GetForcedCategory(RulesetActor actor, RulesetCondition newCondition, string category);
}
