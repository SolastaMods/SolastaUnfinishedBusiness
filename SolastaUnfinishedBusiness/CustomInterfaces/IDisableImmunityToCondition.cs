namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface IDisableImmunityToCondition
{
    bool DisableImmunityToCondition(string conditionName, ulong sourceGuid);
}
