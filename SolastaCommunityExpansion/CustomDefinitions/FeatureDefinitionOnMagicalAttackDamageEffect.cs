using System.Collections.Generic;

namespace SolastaCommunityExpansion.CustomDefinitions
{
    public interface IOnMagicalAttackDamageEffect
    {
        void OnMagicalAttackDamage(
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

    /**
     * Before using this, please consider if FeatureDefinitionAdditionalDamage can cover the desired use case.
     * This has much greater flexibility, so there are cases where it is appropriate, but when possible it is
     * better for future maintainability of features to use the features provided by TA.
     */
    public class FeatureDefinitionOnMagicalAttackDamageEffect : FeatureDefinition, IOnMagicalAttackDamageEffect
    {
        private OnMagicalAttackDamageDelegate onMagicalAttackDamage;

        internal void SetOnMagicalAttackDamageDelegate(OnMagicalAttackDamageDelegate del)
        {
            onMagicalAttackDamage = del;
        }

        public void OnMagicalAttackDamage(
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier magicModifier,
            RulesetEffect rulesetEffect,
            List<EffectForm> actualEffectForms,
            bool firstTarget,
            bool criticalHit)
        {
            onMagicalAttackDamage?.Invoke(attacker, defender, magicModifier, rulesetEffect, actualEffectForms, firstTarget, criticalHit);
        }
    }
}
