namespace SolastaCommunityExpansion.CustomInterfaces
{
    public interface IReactionAttackModeRestriction
    {
        bool ValidReactionMode(RulesetAttackMode attackMode, RulesetCharacter character, RulesetCharacter target);
    }

    public delegate bool ValidReactionModeHandler(RulesetAttackMode attackMode, RulesetCharacter character,
        RulesetCharacter target);
}

