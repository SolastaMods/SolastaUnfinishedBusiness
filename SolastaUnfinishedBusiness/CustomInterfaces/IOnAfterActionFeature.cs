namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface IOnAfterActionFeature
{
    public void OnAfterAction(
        GameLocationCharacter actingCharacter,
        CharacterActionParams actionParams,
        ActionDefinition actionDefinition);
}
