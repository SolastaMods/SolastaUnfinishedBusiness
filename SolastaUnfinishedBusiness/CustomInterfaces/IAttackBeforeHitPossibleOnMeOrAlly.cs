using System.Collections;

namespace SolastaUnfinishedBusiness.CustomInterfaces;

// triggers on any attack, magical or physical, if there is an attack roll
// rulesetEffect != null is a magical attack
public interface IAttackBeforeHitPossibleOnMeOrAlly
{
    IEnumerator OnAttackBeforeHitPossibleOnMeOrAlly(
        GameLocationBattleManager battleManager,
        GameLocationCharacter featureOwner,
        GameLocationCharacter attacker,
        GameLocationCharacter defender,
        RulesetAttackMode attackMode,
        RulesetEffect rulesetEffect,
        ActionModifier attackModifier,
        int attackRoll);
}
