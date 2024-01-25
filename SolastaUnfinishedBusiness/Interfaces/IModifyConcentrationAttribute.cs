using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Interfaces;

public interface IModifyConcentrationAttribute
{
    public bool IsValid(RulesetActor rulesetActor);

    public string ConcentrationAttribute([UsedImplicitly] RulesetActor rulesetActor);
}
