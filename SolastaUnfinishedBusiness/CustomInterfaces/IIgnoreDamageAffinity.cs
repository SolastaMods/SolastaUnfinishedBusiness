namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface IIgnoreDamageAffinity
{
    bool CanIgnoreDamageAffinity(IDamageAffinityProvider provider, RulesetActor rulesetActor, string damageType);
}
