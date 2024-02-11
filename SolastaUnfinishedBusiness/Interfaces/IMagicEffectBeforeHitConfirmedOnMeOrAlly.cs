using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Interfaces;

// triggers on any magical attack regardless of an attack roll or not
public interface IMagicEffectBeforeHitConfirmedOnMeOrAlly
{
    [UsedImplicitly]
    IEnumerator OnMagicEffectBeforeHitConfirmedOnMeOrAlly(
        GameLocationCharacter attacker,
        GameLocationCharacter defender,
        GameLocationCharacter helper,
        ActionModifier actionModifier,
        RulesetEffect rulesetEffect,
        List<EffectForm> actualEffectForms,
        bool firstTarget,
        bool criticalHit);
}
