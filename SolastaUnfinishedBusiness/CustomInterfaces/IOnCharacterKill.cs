namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface IOnCharacterKill
{
    public void OnCharacterKill(GameLocationCharacter character);
}

public delegate void OnCharacterKillDelegate(GameLocationCharacter character);
