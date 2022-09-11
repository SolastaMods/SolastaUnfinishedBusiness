namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface IOnCharacterKill
{
    void OnCharacterKill(GameLocationCharacter character);
}

public delegate void OnCharacterKillDelegate(GameLocationCharacter character);
