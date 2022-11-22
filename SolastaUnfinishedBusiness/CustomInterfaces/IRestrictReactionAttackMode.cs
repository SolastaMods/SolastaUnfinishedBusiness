namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface IRestrictReactionAttackMode
{
    public bool ValidReactionMode(
        RulesetAttackMode attackMode,
        GameLocationCharacter character,
        GameLocationCharacter target);
}

public delegate bool ValidReactionModeHandler(
    RulesetAttackMode attackMode,
    GameLocationCharacter character,
    GameLocationCharacter target);
