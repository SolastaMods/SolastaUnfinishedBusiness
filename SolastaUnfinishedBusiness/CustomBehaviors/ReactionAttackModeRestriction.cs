using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.CustomInterfaces;

namespace SolastaUnfinishedBusiness.CustomBehaviors;

internal sealed class ReactionAttackModeRestriction : IReactionAttackModeRestriction
{
    internal static readonly ValidReactionModeHandler MeleeOnly = (mode, ranged, _, _) =>
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

    internal ReactionAttackModeRestriction(params ValidReactionModeHandler[] validators)
    {
        this.validators = validators;
    }

    public bool ValidReactionMode(RulesetAttackMode attackMode, bool rangedAttack,
        GameLocationCharacter character, GameLocationCharacter target)
    {
        return validators.All(v => v(attackMode, rangedAttack, character, target));
    }

    [NotNull]
    internal static ValidReactionModeHandler TargetHasNoCondition(ConditionDefinition condition)
    {
        return (_, _, _, target) =>
        {
            var rulesetCharacter = target.RulesetCharacter;

            return rulesetCharacter != null && !rulesetCharacter.HasConditionOfType(condition.Name);
        };
    }
}
