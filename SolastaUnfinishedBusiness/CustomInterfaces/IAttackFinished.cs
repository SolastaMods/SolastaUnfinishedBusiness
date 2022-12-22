using System.Collections;

namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface IAttackFinished
{
    IEnumerator OnAttackFinished(GameLocationBattleManager battleManager,
        CharacterAction action,
        GameLocationCharacter attacker,
        GameLocationCharacter defender,
        RulesetAttackMode attackerAttackMode,
        RuleDefinitions.RollOutcome attackRollOutcome,
        int damageAmount);
}
