using System.Collections.Generic;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface IOnMagicalAttackDamageEffect
{
#if false
    public void BeforeOnMagicalAttackDamage(
        GameLocationCharacter attacker,
        GameLocationCharacter defender,
        ActionModifier magicModifier,
        RulesetEffect rulesetEffect,
        List<EffectForm> actualEffectForms,
        bool firstTarget,
        bool criticalHit);
#endif

    [UsedImplicitly]
    public void AfterOnMagicalAttackDamage(
        GameLocationCharacter attacker,
        GameLocationCharacter defender,
        ActionModifier magicModifier,
        RulesetEffect rulesetEffect,
        List<EffectForm> actualEffectForms,
        bool firstTarget,
        bool criticalHit);
}
