namespace SolastaCommunityExpansion.CustomInterfaces;

public interface IDisableImmunityAndResistanceToDamageType
{
    bool DisableImmunityAndResistanceToDamageType(string damageType);
}

public interface IDisableImmunityToCondition
{
    bool DisableImmunityToCondition(string conditionName);
}
