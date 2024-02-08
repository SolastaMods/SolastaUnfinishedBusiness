using System.Collections;
using JetBrains.Annotations;
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.Interfaces;

public interface IPhysicalAttackFinishedOnMe
{
    [UsedImplicitly]
    IEnumerator OnPhysicalAttackFinishedOnMe(
        GameLocationBattleManager battleManager,
        CharacterAction action,
        GameLocationCharacter attacker,
        GameLocationCharacter defender,
        RulesetAttackMode attackMode,
        RollOutcome rollOutcome,
        int damageAmount);
}
