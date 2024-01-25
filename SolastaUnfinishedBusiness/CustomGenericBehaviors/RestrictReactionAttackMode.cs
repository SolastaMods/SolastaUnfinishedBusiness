using System.Linq;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.CustomInterfaces;

namespace SolastaUnfinishedBusiness.CustomGenericBehaviors;

internal sealed class RestrictReactionAttackMode : IRestrictReactionAttackMode
{
    internal static (CharacterAction, GameLocationCharacter, GameLocationCharacter, RulesetAttackMode, RulesetEffect)
        ReactionContext = (null, null, null, null, null);

    private readonly ValidReactionModeHandler[] _validators;

    internal RestrictReactionAttackMode(params ValidReactionModeHandler[] validators)
    {
        _validators = validators;
    }

    public bool ValidReactionMode(
        CharacterAction action,
        GameLocationCharacter attacker,
        GameLocationCharacter defender,
        RulesetAttackMode attackMode,
        RulesetEffect rulesetEffect)
    {
        return _validators.All(v => v(action, attacker, defender, attackMode, rulesetEffect));
    }

#if false
    internal static ValidReactionModeHandler TargetHasNoCondition(ConditionDefinition condition)
    {
        return (_, _, target) =>
        {
            var rulesetCharacter = target.RulesetCharacter;

            return rulesetCharacter != null && !rulesetCharacter.HasConditionOfType(condition.Name);
        };
    }
#endif

    internal static bool CanCharacterReactWithPower(RulesetUsablePower usablePower)
    {
        var (action, attacker, defender, attackMode, rulesetEffect) = ReactionContext;

        if (attacker == null || defender == null)
        {
            return true;
        }

        var validator = usablePower.PowerDefinition.GetFirstSubFeatureOfType<IRestrictReactionAttackMode>();

        return validator == null || validator.ValidReactionMode(action, attacker, defender, attackMode, rulesetEffect);
    }
}
