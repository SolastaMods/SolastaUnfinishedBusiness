namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface IForceMaxDamageTypeDependent
{
    bool IsValid(RulesetActor rulesetActor, DamageForm damageForm);
}
