using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.Interfaces;

// triggers on any attack, magical or physical, if there is an attack roll
// rulesetEffect != null is a magical attack
public interface IAttackBeforeHitConfirmedOnMeOrAlly
{
    [UsedImplicitly]
    IEnumerator OnAttackBeforeHitConfirmedOnMeOrAlly(
        GameLocationBattleManager battleManager,
        GameLocationCharacter attacker,
        GameLocationCharacter defender,
        GameLocationCharacter helper,
        ActionModifier actionModifier,
        RulesetAttackMode attackMode,
        bool rangedAttack,
        AdvantageType advantageType,
        List<EffectForm> actualEffectForms,
        RulesetEffect rulesetEffect,
        bool firstTarget,
        bool criticalHit);
}
