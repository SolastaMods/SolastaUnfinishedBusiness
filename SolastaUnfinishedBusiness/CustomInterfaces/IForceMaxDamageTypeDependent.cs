namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface IForceMaxDamageTypeDependent
{
    // ReSharper disable once UnusedParameter.Global
    bool IsValid(RulesetActor rulesetActor, DamageForm damageForm);
}
