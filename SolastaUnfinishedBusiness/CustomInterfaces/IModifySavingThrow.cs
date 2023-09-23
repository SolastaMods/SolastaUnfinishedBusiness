using System.Collections.Generic;

namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface IModifySavingThrow
{
    public bool IsValid(
        RulesetActor rulesetActor,
        RulesetActor rulesetCaster,
        IEnumerable<EffectForm> effectForms,
        string attributeScore);

    public string AttributeAndActionModifier(
        RulesetActor rulesetActor,
        ActionModifier actionModifier,
        string attribute);
}
