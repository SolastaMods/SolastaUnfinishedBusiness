using System.Collections.Generic;

namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface IOnMagicalAttackDamageEffect
{
    public void BeforeOnMagicalAttackDamage(
        GameLocationCharacter attacker,
        GameLocationCharacter defender,
        ActionModifier magicModifier,
        RulesetEffect rulesetEffect,
        List<EffectForm> actualEffectForms,
        bool firstTarget,
        bool criticalHit);

    public void AfterOnMagicalAttackDamage(
        GameLocationCharacter attacker,
        GameLocationCharacter defender,
        ActionModifier magicModifier,
        RulesetEffect rulesetEffect,
        List<EffectForm> actualEffectForms,
        bool firstTarget,
        bool criticalHit);
}

public delegate void OnMagicalAttackDamageDelegate(
    GameLocationCharacter attacker,
    GameLocationCharacter defender,
    ActionModifier magicModifier,
    RulesetEffect rulesetEffect,
    List<EffectForm> actualEffectForms,
    bool firstTarget,
    bool criticalHit);
