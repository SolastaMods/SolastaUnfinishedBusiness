namespace SolastaCommunityExpansion.Features;

public interface ICharacterTurnStartListener
{
    void OnChracterTurnStarted(GameLocationCharacter locationCharacter);
}

public interface ICharacterTurnEndListener
{
    void OnChracterTurnEnded(GameLocationCharacter locationCharacter);
}

public interface ICharacterBattlStartedListener
{
    void OnChracterBattleStarted(GameLocationCharacter locationCharacter, bool surprise);
}

public interface ICharacterBattlEndedListener
{
    void OnChracterBattleEnded(GameLocationCharacter locationCharacter);
}