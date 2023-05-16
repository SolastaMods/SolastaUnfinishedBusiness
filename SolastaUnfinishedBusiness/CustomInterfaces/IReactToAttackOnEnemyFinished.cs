using System.Collections;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface IReactToAttackOnEnemyFinished
{
    [UsedImplicitly]
    public IEnumerator HandleReactToAttackOnEnemyFinished(
        GameLocationCharacter attacker,
        GameLocationCharacter me,
        GameLocationCharacter ally,
        RuleDefinitions.RollOutcome outcome,
        CharacterActionParams actionParams,
        RulesetAttackMode mode,
        ActionModifier modifier);
}
