using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.CustomInterfaces;

//Can add similar interfaces in future for after hit confirmed and/or for attacker
public interface IDefenderBeforeAttackHitConfirmed
{
    [UsedImplicitly]
    IEnumerator DefenderBeforeAttackHitConfirmed(
        GameLocationBattleManager battle,
        GameLocationCharacter attacker,
        GameLocationCharacter defender,
        ActionModifier attackModifier,
        RulesetAttackMode attackMode,
        bool rangedAttack,
        RuleDefinitions.AdvantageType advantageType,
        List<EffectForm> actualEffectForms,
        RulesetEffect rulesetEffect,
        bool criticalHit,
        bool firstTarget);
}
