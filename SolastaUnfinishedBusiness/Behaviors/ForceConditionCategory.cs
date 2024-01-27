using SolastaUnfinishedBusiness.Interfaces;

namespace SolastaUnfinishedBusiness.Behaviors;

public class ForceConditionCategory(string category) : IForceConditionCategory
{
    public string GetForcedCategory(RulesetActor actor, RulesetCondition newCondition, string category1)
    {
        return category;
    }
}
