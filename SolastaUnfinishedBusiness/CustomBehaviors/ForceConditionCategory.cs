using SolastaUnfinishedBusiness.CustomInterfaces;

namespace SolastaUnfinishedBusiness.CustomValidators;

public class ForceConditionCategory : IForceConditionCategory
{
    private readonly string _category;

    public ForceConditionCategory(string category)
    {
        _category = category;
    }

    public string GetForcedCategory(RulesetActor actor, RulesetCondition newCondition, string category)
    {
        return _category;
    }
}
