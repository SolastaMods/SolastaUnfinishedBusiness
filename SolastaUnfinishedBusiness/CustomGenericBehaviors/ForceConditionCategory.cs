using SolastaUnfinishedBusiness.CustomInterfaces;

namespace SolastaUnfinishedBusiness.CustomGenericBehaviors;

public class ForceConditionCategory(string category) : IForceConditionCategory
{
    public string GetForcedCategory(RulesetActor actor, RulesetCondition newCondition, string category1)
    {
        return category;
    }
}
