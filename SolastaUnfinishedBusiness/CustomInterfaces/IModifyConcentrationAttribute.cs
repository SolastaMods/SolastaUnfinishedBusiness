using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface IModifyConcentrationAttribute
{
    public bool IsValid(RulesetActor rulesetActor);

    public string ConcentrationAttribute([UsedImplicitly] RulesetActor rulesetActor);
}
