using System.Collections;
using JetBrains.Annotations;
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface IPhysicalAttackFinishedOnMeOrAlly
{
    [UsedImplicitly]
    IEnumerator OnPhysicalAttackFinishedOnMeOrAlly(
        GameLocationBattleManager battleManager,
        CharacterAction action,
        GameLocationCharacter attacker,
        GameLocationCharacter defender,
        GameLocationCharacter me,
        RulesetAttackMode attackerAttackMode,
        RollOutcome attackRollOutcome,
        int damageAmount);
}
