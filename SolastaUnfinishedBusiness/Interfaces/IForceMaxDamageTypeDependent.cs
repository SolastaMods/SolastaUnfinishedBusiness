namespace SolastaUnfinishedBusiness.Interfaces;

public interface IForceMaxDamageTypeDependent
{
    // ReSharper disable once UnusedParameter.Global
    bool IsValid(RulesetActor rulesetActor, DamageForm damageForm);
}
