namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface IModifySavingThrow
{
    public bool IsValid(RulesetActor rulesetActor, RulesetActor rulesetCaster, string attributeScore);

    public string AttributeAndActionModifier(
        RulesetActor rulesetActor,
        ActionModifier actionModifier,
        string attribute);
}
