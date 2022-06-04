using System.Linq;
using SolastaCommunityExpansion.CustomInterfaces;

namespace SolastaCommunityExpansion.CustomDefinitions;

public class ReactionAttackModeRestriction : IReactionAttackModeRestriction
{
    public static readonly ValidReactionModeHandler MeleeOnly = (mode, ranged, _, _) =>
    {
        if (ranged)
        {
            return false;
        }

        var item = mode.SourceDefinition as ItemDefinition;
        if (item == null)
        {
            return false;
        }

        var weapon = item.WeaponDescription;
        if (weapon == null)
        {
            return false;
        }

        return weapon.WeaponTypeDefinition.WeaponProximity == RuleDefinitions.AttackProximity.Melee;
    };

    private readonly ValidReactionModeHandler[] validators;

    public ReactionAttackModeRestriction(params ValidReactionModeHandler[] validators)
    {
        this.validators = validators;
    }

    public bool ValidReactionMode(RulesetAttackMode attackMode, bool rangedAttack, 
        GameLocationCharacter character, GameLocationCharacter target)
    {
        return validators.All(v => v(attackMode, rangedAttack, character, target));
    }

    public static ValidReactionModeHandler TargenHasNoCondition(ConditionDefinition condition)
    {
        return (_, _, _, target) =>
        {
            var rulesetCharacter = target.RulesetCharacter;
            return rulesetCharacter != null && !rulesetCharacter.HasConditionOfType(condition.Name);
        };
    }
}
