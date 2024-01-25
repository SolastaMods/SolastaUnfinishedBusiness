using System.Collections;

namespace SolastaUnfinishedBusiness.Interfaces;

public interface ITryAlterOutcomePhysicalAttack
{
    IEnumerator OnAttackTryAlterOutcome(
        GameLocationBattleManager instance,
        CharacterAction action,
        GameLocationCharacter attacker,
        GameLocationCharacter target,
        ActionModifier attackModifier);
}
