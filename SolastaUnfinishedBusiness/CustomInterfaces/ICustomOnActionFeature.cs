namespace SolastaUnfinishedBusiness.CustomInterfaces;

internal interface ICustomOnActionFeature
{
    public void OnBeforeAction(CharacterAction characterAction);
    public void OnAfterAction(CharacterAction characterAction);
}
