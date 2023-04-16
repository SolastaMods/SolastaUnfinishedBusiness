using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface IChangeConcentrationAttribute
{
    public bool IsValid(RulesetActor rulesetActor);

    public string ConcentrationAttribute([UsedImplicitly] RulesetActor rulesetActor);
}
