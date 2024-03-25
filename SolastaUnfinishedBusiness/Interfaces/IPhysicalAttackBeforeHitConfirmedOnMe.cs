using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.Interfaces;

// triggers on any weapon attack
public interface IPhysicalAttackBeforeHitConfirmedOnMe
{
    [UsedImplicitly]
    IEnumerator OnPhysicalAttackBeforeHitConfirmedOnMe(
        GameLocationBattleManager battleManager,
        GameLocationCharacter attacker,
        GameLocationCharacter defender,
        ActionModifier actionModifier,
        RulesetAttackMode attackMode,
        bool rangedAttack,
        AdvantageType advantageType,
        List<EffectForm> actualEffectForms,
        bool firstTarget,
        bool criticalHit);
}
