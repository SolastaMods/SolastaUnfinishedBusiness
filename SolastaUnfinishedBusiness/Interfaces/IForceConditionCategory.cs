using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Interfaces;

public interface IForceConditionCategory
{
    [UsedImplicitly]
    string GetForcedCategory(RulesetActor actor, RulesetCondition newCondition, string category);
}
