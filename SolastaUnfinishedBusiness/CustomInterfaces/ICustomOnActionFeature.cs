namespace SolastaUnfinishedBusiness.CustomInterfaces;

internal interface ICustomOnActionFeature
{
    internal void OnBeforeAction(CharacterAction characterAction);
    internal void OnAfterAction(CharacterAction characterAction);
}
