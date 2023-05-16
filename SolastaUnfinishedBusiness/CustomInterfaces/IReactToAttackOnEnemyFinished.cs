using System.Collections;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface IReactToAttackOnEnemyFinished
{
    [UsedImplicitly]
    public IEnumerator HandleReactToAttackOnEnemyFinished(
        GameLocationCharacter ally,
        GameLocationCharacter me,
        GameLocationCharacter enemy,
        RuleDefinitions.RollOutcome outcome,
        CharacterActionParams actionParams,
        RulesetAttackMode mode,
        ActionModifier modifier);
}
