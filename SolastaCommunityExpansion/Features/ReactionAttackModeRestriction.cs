using System.Linq;

namespace SolastaCommunityExpansion.Features;

public interface IReactionAttackModeRestriction
{
    bool ValidReactionMode(RulesetAttackMode attackMode, RulesetCharacter character, RulesetCharacter target);
}

public delegate bool ValidReactionModeHandler(RulesetAttackMode attackMode, RulesetCharacter character,
    RulesetCharacter target);

public class ReactionAttackModeRestriction : IReactionAttackModeRestriction
{
    private readonly ValidReactionModeHandler[] validators;

    public static readonly ValidReactionModeHandler MeleeOnly = (mode, _, _) =>
        mode.Reach && !mode.Ranged && !mode.Thrown;

    public static ValidReactionModeHandler TargenHasNoCondition(ConditionDefinition condition)
    {
        return (_, _, target) => !target.HasConditionOfType(condition.Name);
    }

    public ReactionAttackModeRestriction(params  ValidReactionModeHandler[] validators)
    {
        this.validators = validators;
    }

    public bool ValidReactionMode(RulesetAttackMode attackMode, RulesetCharacter character, RulesetCharacter target)
    {
        return validators.All(v => v(attackMode, character, target));
    }
}