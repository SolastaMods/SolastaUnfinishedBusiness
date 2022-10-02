namespace SolastaUnfinishedBusiness.CustomInterfaces;

internal interface IOnCharacterKill
{
    public void OnCharacterKill(GameLocationCharacter character);
}

internal delegate void OnCharacterKillDelegate(GameLocationCharacter character);
