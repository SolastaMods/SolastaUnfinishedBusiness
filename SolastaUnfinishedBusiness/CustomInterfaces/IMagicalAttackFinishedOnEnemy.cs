using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.CustomInterfaces;

// triggers on any magical attack regardless of an attack roll or not
public interface IMagicalAttackFinishedOnEnemy
{
    [UsedImplicitly]
    public IEnumerator OnMagicalAttackFinishedOnEnemy(
        GameLocationCharacter attacker,
        GameLocationCharacter defender,
        GameLocationCharacter ally,
        ActionModifier magicModifier,
        RulesetEffect rulesetEffect,
        List<EffectForm> actualEffectForms,
        bool firstTarget,
        bool criticalHit);
}
