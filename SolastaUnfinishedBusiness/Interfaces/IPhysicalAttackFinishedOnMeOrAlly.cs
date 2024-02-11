using System.Collections;
using JetBrains.Annotations;
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.Interfaces;

public interface IPhysicalAttackFinishedOnMeOrAlly
{
    [UsedImplicitly]
    IEnumerator OnPhysicalAttackFinishedOnMeOrAlly(
        GameLocationBattleManager battleManager,
        CharacterAction action,
        GameLocationCharacter attacker,
        GameLocationCharacter defender,
        GameLocationCharacter helper,
        RulesetAttackMode attackMode,
        RollOutcome rollOutcome,
        int damageAmount);
}
