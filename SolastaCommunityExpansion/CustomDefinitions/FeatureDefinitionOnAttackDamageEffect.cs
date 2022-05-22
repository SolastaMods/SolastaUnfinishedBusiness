using System.Collections.Generic;
using SolastaCommunityExpansion.CustomInterfaces;

namespace SolastaCommunityExpansion.CustomDefinitions
{
    /**
     * Before using this, please consider if FeatureDefinitionAdditionalDamage can cover the desired use case.
     * This has much greater flexibility, so there are cases where it is appropriate, but when possible it is
     * better for future maintainability of features to use the features provided by TA.
     */
    public class FeatureDefinitionOnAttackDamageEffect : FeatureDefinition, IOnAttackDamageEffect
    {
        private OnAttackDamageDelegate beforeOnAttackDamage;
        private OnAttackDamageDelegate afterOnAttackDamage;

        internal void SetOnAttackDamageDelegates(OnAttackDamageDelegate before = null, OnAttackDamageDelegate after = null)
        {
            beforeOnAttackDamage = before;
            afterOnAttackDamage = after;
        }

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
            bool firstTarget)
        {
            beforeOnAttackDamage?.Invoke(attacker, defender, attackModifier, attackMode, rangedAttack, advantageType, actualEffectForms, rulesetEffect, criticalHit, firstTarget);
        }

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
            bool firstTarget)
        {
            afterOnAttackDamage?.Invoke(attacker, defender, attackModifier, attackMode, rangedAttack, advantageType, actualEffectForms, rulesetEffect, criticalHit, firstTarget);
        }
    }
}
