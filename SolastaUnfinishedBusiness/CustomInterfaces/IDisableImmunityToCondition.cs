namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface IDisableImmunityToCondition
{
    public bool DisableImmunityToCondition(string conditionName, ulong sourceGuid);
}
