namespace SolastaUnfinishedBusiness.CustomInterfaces;

internal interface IOnCharacterKill
{
    void OnCharacterKill(GameLocationCharacter character);
}

internal delegate void OnCharacterKillDelegate(GameLocationCharacter character);
