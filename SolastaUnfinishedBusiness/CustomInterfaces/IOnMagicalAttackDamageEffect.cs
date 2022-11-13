using System.Collections.Generic;

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

    public void AfterOnMagicalAttackDamage(
        GameLocationCharacter attacker,
        GameLocationCharacter defender,
        ActionModifier magicModifier,
        RulesetEffect rulesetEffect,
        List<EffectForm> actualEffectForms,
        bool firstTarget,
        bool criticalHit);
}
