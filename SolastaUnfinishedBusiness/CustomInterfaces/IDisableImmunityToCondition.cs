namespace SolastaUnfinishedBusiness.CustomInterfaces;

internal interface IDisableImmunityToCondition
{
    bool DisableImmunityToCondition(string conditionName, ulong sourceGuid);
}
