using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface IForceConditionCategory
{
    [UsedImplicitly]
    string GetForcedCategory(RulesetActor actor, RulesetCondition newCondition, string category);
}
