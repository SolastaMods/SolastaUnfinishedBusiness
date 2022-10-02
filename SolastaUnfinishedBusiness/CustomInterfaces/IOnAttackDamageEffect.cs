using System.Collections.Generic;

namespace SolastaUnfinishedBusiness.CustomInterfaces;

internal interface IOnAttackDamageEffect
{
    public void BeforeOnAttackDamage(
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

    public void AfterOnAttackDamage(
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

internal delegate void OnAttackDamageDelegate(
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
