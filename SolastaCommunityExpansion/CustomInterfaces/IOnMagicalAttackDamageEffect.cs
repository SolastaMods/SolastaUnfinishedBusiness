using System.Collections.Generic;

namespace SolastaCommunityExpansion.CustomInterfaces
{
    public interface IOnMagicalAttackDamageEffect
    {
        void BeforeOnMagicalAttackDamage(
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier magicModifier,
            RulesetEffect rulesetEffect,
            List<EffectForm> actualEffectForms,
            bool firstTarget,
            bool criticalHit);

        void AfterOnMagicalAttackDamage(
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
}
