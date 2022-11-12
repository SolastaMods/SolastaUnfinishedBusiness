using System.Collections;

namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface IReactToMyAttackFinished
{
    public IEnumerator HandleReactToMyAttackFinished(
        GameLocationCharacter me,
        GameLocationCharacter defender,
        RuleDefinitions.RollOutcome outcome,
        CharacterActionParams actionParams,
        RulesetAttackMode mode,
        ActionModifier modifier);
}

public interface IReactToAttackOnMeFinished
{
    public IEnumerator HandleReactToAttackOnMeFinished(
        GameLocationCharacter attacker,
        GameLocationCharacter me,
        RuleDefinitions.RollOutcome outcome,
        CharacterActionParams actionParams,
        RulesetAttackMode mode,
        ActionModifier modifier);
}
