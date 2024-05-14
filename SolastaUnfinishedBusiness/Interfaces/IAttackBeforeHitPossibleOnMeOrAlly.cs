using System.Collections;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Interfaces;

// triggers on any attack, magical or physical, if there is an attack roll
// rulesetEffect != null is a magical attack
public interface IAttackBeforeHitPossibleOnMeOrAlly
{
    IEnumerator OnAttackBeforeHitPossibleOnMeOrAlly(
        GameLocationBattleManager battleManager,
        [UsedImplicitly] GameLocationCharacter attacker,
        GameLocationCharacter defender,
        GameLocationCharacter helper,
        ActionModifier actionModifier,
        RulesetAttackMode attackMode,
        RulesetEffect rulesetEffect,
        int attackRoll);
}
