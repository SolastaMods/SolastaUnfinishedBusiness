using System.Collections;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface IPhysicalAttackFinishedOnEnemy
{
    [UsedImplicitly]
    public IEnumerator OnPhysicalAttackFinishedOnEnemy(
        GameLocationBattleManager battleManager,
        CharacterAction action,
        GameLocationCharacter attacker,
        GameLocationCharacter defender,
        GameLocationCharacter ally,
        RulesetAttackMode attackerAttackMode,
        RuleDefinitions.RollOutcome attackRollOutcome,
        int damageAmount);
}
