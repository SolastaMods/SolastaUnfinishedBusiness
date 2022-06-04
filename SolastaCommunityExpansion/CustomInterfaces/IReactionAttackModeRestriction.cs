namespace SolastaCommunityExpansion.CustomInterfaces;

public interface IReactionAttackModeRestriction
{
    bool ValidReactionMode(RulesetAttackMode attackMode, bool rangedAttack,
        GameLocationCharacter character, GameLocationCharacter target);
}

public delegate bool ValidReactionModeHandler(RulesetAttackMode attackMode, bool rangedAttack,
    GameLocationCharacter character, GameLocationCharacter target);
