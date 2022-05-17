namespace SolastaCommunityExpansion.Features;

public interface IReactionAttackModeRestriction
{
    bool ValidRactionMode(RulesetAttackMode attackMode, RulesetCharacter character, RulesetCharacter target);
}

public static class ReactionAttackModeRestriction
{
    public static readonly IReactionAttackModeRestriction MeleeOnly = new MeleeOnlyImpl();
    
    private class MeleeOnlyImpl: IReactionAttackModeRestriction
    {
        public bool ValidRactionMode(RulesetAttackMode attackMode, RulesetCharacter character, RulesetCharacter target)
        {
            return !attackMode.Ranged && attackMode.Reach;
        }
    }
}