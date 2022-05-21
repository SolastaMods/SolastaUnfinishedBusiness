using System.Linq;
using SolastaCommunityExpansion.CustomInterfaces;

namespace SolastaCommunityExpansion.CustomDefinitions;

public class ReactionAttackModeRestriction : IReactionAttackModeRestriction
{
    private readonly ValidReactionModeHandler[] validators;

    public static readonly ValidReactionModeHandler MeleeOnly = (mode, _, _) =>
        mode is { Reach: true, Ranged: false, Thrown: false };

    public static ValidReactionModeHandler TargenHasNoCondition(ConditionDefinition condition)
    {
        return (_, _, target) => !target.HasConditionOfType(condition.Name);
    }

    public ReactionAttackModeRestriction(params ValidReactionModeHandler[] validators)
    {
        this.validators = validators;
    }

    public bool ValidReactionMode(RulesetAttackMode attackMode, RulesetCharacter character, RulesetCharacter target)
    {
        return validators.All(v => v(attackMode, character, target));
    }
}
