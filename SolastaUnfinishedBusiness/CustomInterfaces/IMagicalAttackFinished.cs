using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface IMagicalAttackFinished
{
    [UsedImplicitly]
    IEnumerator BeforeOnMagicalAttackDamage(
        GameLocationCharacter attacker,
        GameLocationCharacter defender,
        ActionModifier magicModifier,
        RulesetEffect rulesetEffect,
        List<EffectForm> actualEffectForms,
        bool firstTarget,
        bool criticalHit);

    [UsedImplicitly]
    IEnumerator OnMagicalAttackFinished(
        GameLocationCharacter attacker,
        GameLocationCharacter defender,
        ActionModifier magicModifier,
        RulesetEffect rulesetEffect,
        List<EffectForm> actualEffectForms,
        bool firstTarget,
        bool criticalHit);
}
