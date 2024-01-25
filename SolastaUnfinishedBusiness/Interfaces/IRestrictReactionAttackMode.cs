namespace SolastaUnfinishedBusiness.Interfaces;

public interface IRestrictReactionAttackMode
{
    public bool ValidReactionMode(
        CharacterAction action,
        GameLocationCharacter attacker,
        GameLocationCharacter defender,
        RulesetAttackMode attackMode,
        RulesetEffect rulesetEffect);
}

public delegate bool ValidReactionModeHandler(
    CharacterAction action,
    GameLocationCharacter attacker,
    GameLocationCharacter defender,
    RulesetAttackMode attackMode,
    RulesetEffect rulesetEffect);
