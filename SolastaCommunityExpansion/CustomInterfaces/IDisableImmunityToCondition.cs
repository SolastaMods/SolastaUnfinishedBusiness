namespace SolastaCommunityExpansion.CustomInterfaces;

public interface IDisableImmunityToCondition
{
    bool DisableImmunityToCondition(string conditionName, ulong sourceGuid);
}
