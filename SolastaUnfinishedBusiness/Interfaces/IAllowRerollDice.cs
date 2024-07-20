namespace SolastaUnfinishedBusiness.Interfaces;

public interface IAllowRerollDice
{
    public bool IsValid(RulesetActor rulesetActor, DamageForm damageForm);
}
