namespace SolastaUnfinishedBusiness.CustomInterfaces;

internal interface IDisableImmunityToCondition
{
    public bool DisableImmunityToCondition(string conditionName, ulong sourceGuid);
}
