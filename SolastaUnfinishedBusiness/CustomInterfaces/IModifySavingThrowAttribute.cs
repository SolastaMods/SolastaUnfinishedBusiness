using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface IModifySavingThrowAttribute
{
    public bool IsValid(RulesetActor rulesetActor, string attributeScore);

    public string SavingThrowAttribute([UsedImplicitly] RulesetActor rulesetActor);
}
