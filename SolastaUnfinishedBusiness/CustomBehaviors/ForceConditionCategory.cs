namespace SolastaUnfinishedBusiness.CustomBehaviors;

public interface IForceConditionCategory
{
    string GetForcedCategory(RulesetActor actor, RulesetCondition newCondition, string category);
}

public class ForceConditionCategory : IForceConditionCategory
{
    private readonly string category;

    public ForceConditionCategory(string category)
    {
        this.category = category;
    }

    public string GetForcedCategory(RulesetActor actor, RulesetCondition newCondition, string category)
    {
        return this.category;
    }
}
