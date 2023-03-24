using System.Collections;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface IReactToMyAttackFinished
{
    [UsedImplicitly]
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
    [UsedImplicitly]
    public IEnumerator HandleReactToAttackOnMeFinished(
        GameLocationCharacter attacker,
        GameLocationCharacter me,
        RuleDefinitions.RollOutcome outcome,
        CharacterActionParams actionParams,
        RulesetAttackMode mode,
        ActionModifier modifier);
}

public interface IReactToAttackOnAllyFinished
{
    [UsedImplicitly]
    public IEnumerator HandleReactToAttackOnAllyFinished(
        GameLocationCharacter attacker,
        GameLocationCharacter me,
        RuleDefinitions.RollOutcome outcome,
        CharacterActionParams actionParams,
        RulesetAttackMode mode,
        ActionModifier modifier);
}
