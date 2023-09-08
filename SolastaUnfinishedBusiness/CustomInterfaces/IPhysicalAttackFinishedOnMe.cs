using System.Collections;
using JetBrains.Annotations;
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface IPhysicalAttackFinishedOnMe
{
    [UsedImplicitly]
    IEnumerator OnAttackFinishedOnMe(
        GameLocationBattleManager battleManager,
        CharacterAction action,
        GameLocationCharacter attacker,
        GameLocationCharacter defender,
        RulesetAttackMode attackerAttackMode,
        RollOutcome attackRollOutcome,
        int damageAmount);
}
