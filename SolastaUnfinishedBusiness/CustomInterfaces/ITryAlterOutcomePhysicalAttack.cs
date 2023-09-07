using System.Collections;

namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface ITryAlterOutcomePhysicalAttack
{
    IEnumerator OnAttackTryAlterOutcome(
        GameLocationBattleManager instance,
        CharacterAction action,
        GameLocationCharacter attacker,
        GameLocationCharacter target,
        ActionModifier attackModifier);
}
