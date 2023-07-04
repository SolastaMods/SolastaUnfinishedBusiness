using System.Collections;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface IPhysicalAttackFinishedOnMeOrAlly
{
    [UsedImplicitly]
    IEnumerator OnAttackFinishedOnMeOrAlly(
        GameLocationBattleManager battleManager,
        CharacterAction action,
        GameLocationCharacter attacker,
        GameLocationCharacter defender,
        GameLocationCharacter me,
        RulesetAttackMode attackerAttackMode,
        RuleDefinitions.RollOutcome attackRollOutcome,
        int damageAmount);
}
