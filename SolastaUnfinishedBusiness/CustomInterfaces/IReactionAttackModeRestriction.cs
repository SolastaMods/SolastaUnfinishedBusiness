namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface IReactionAttackModeRestriction
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
