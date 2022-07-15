namespace SolastaCommunityExpansion.CustomInterfaces;

public interface IOnCharacterKill
{
    void OnCharacterKill(
        GameLocationCharacter character,
        bool dropLoot,
        bool removeBody,
        bool forceRemove,
        bool considerDead,
        bool becomesDying);
}

public delegate void OnCharacterKillDelegate(
    GameLocationCharacter character,
    bool dropLoot,
    bool removeBody,
    bool forceRemove,
    bool considerDead,
    bool becomesDying);
