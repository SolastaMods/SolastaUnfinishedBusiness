using System.Collections;

namespace SolastaUnfinishedBusiness.Interfaces;

public interface ITryAlterOutcomePhysicalAttackByMe
{
    IEnumerator OnAttackTryAlterOutcomeByMe(
        GameLocationBattleManager instance,
        CharacterAction action,
        GameLocationCharacter attacker,
        GameLocationCharacter target,
        ActionModifier attackModifier);
}
