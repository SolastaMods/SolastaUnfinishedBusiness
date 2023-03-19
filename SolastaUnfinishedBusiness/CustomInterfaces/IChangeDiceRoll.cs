namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface IChangeDiceRoll
{
    public bool IsValid(RuleDefinitions.RollContext rollContext, RulesetCharacter rulesetCharacter);

    public void BeforeRoll(
        RuleDefinitions.RollContext rollContext,
        RulesetCharacter rulesetCharacter,
        ref RuleDefinitions.DieType dieType,
        ref RuleDefinitions.AdvantageType advantageType);

    public void AfterRoll(
        RuleDefinitions.RollContext rollContext,
        RulesetCharacter rulesetCharacter,
        ref int firstRoll,
        ref int secondRoll);
}
