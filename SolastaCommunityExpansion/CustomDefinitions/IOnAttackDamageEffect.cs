using System.Collections.Generic;

namespace SolastaCommunityExpansion.CustomDefinitions
{
    public interface IOnAttackDamageEffect
    {
        void BeforeOnAttackDamage(
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

        void AfterOnAttackDamage(
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

    public delegate void OnAttackDamageDelegate(
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
