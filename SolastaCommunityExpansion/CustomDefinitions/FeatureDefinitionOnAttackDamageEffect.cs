using System.Collections.Generic;

namespace SolastaCommunityExpansion.CustomDefinitions
{
    public interface IOnAttackDamageEffect
    {
        void OnAttackDamage(
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

    /**
     * Before using this, please consider if FeatureDefinitionAdditionalDamage can cover the desired use case.
     * This has much greater flexibility, so there are cases where it is appropriate, but when possible it is
     * better for future maintainability of features to use the features provided by TA.
     */
    public class FeatureDefinitionOnAttackDamageEffect : FeatureDefinition, IOnAttackDamageEffect
    {
        private OnAttackDamageDelegate onAttackDamage;

        internal void SetOnAttackDamageDelegate(OnAttackDamageDelegate del)
        {
            onAttackDamage = del;
        }

        public void OnAttackDamage(
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier attackModifier,
            RulesetAttackMode attackMode,
            bool rangedAttack,
            RuleDefinitions.AdvantageType advantageType,
            List<EffectForm> actualEffectForms,
            RulesetEffect rulesetEffect,
            bool criticalHit,
            bool firstTarget)
        {
            onAttackDamage?.Invoke(attacker, defender, attackModifier, attackMode, rangedAttack, advantageType, actualEffectForms, rulesetEffect, criticalHit, firstTarget);
        }
    }
}
