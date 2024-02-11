using System.Collections;

namespace SolastaUnfinishedBusiness.Interfaces;

// triggers on any attack, magical or physical, if there is an attack roll
// rulesetEffect != null is a magical attack
public interface IAttackBeforeHitPossibleOnMeOrAlly
{
    IEnumerator OnAttackBeforeHitPossibleOnMeOrAlly(GameLocationBattleManager battleManager,
        GameLocationCharacter attacker,
        GameLocationCharacter defender,
        GameLocationCharacter helper,
        ActionModifier actionModifier,
        RulesetAttackMode attackMode,
        RulesetEffect rulesetEffect,
        int attackRoll);
}
