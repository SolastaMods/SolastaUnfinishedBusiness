namespace SolastaUnfinishedBusiness.CustomInterfaces;

// allows a condition to have a soft parent
// a soft parent allows the condition to be considered on checks but don't inherit parent features
public interface IForceConditionParent
{
    string Parent(ConditionDefinition source);
}
