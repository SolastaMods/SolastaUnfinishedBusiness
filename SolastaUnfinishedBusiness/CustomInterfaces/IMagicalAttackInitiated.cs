using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface IMagicalAttackInitiated
{
    [UsedImplicitly]
    IEnumerator OnMagicalAttackInitiated(
        GameLocationCharacter attacker,
        GameLocationCharacter defender,
        ActionModifier magicModifier,
        RulesetEffect rulesetEffect,
        List<EffectForm> actualEffectForms,
        bool firstTarget,
        bool criticalHit);
}
