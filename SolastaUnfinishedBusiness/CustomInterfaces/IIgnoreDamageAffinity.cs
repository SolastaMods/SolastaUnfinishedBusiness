namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface IIgnoreDamageAffinity
{
    bool CanIgnoreDamageAffinity(IDamageAffinityProvider provider, string damageType);
}
