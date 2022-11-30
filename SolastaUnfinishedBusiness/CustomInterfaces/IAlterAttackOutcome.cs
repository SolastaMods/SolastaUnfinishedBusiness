using System.Collections;

namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface IAlterAttackOutcome
{
    IEnumerator TryAlterAttackOutcome(GameLocationBattleManager instance, CharacterAction action,
        GameLocationCharacter attacker, GameLocationCharacter target, ActionModifier attackModifier);
}
