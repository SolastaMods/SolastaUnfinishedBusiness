using System.Collections;

namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface IPhysicalAttackTryAlterOutcome
{
    IEnumerator OnAttackTryAlterOutcome(
        GameLocationBattleManager instance,
        CharacterAction action,
        GameLocationCharacter attacker,
        GameLocationCharacter target,
        ActionModifier attackModifier);
}
