using System.Collections.Generic;

namespace SolastaCommunityExpansion.CustomDefinitions
{
    public interface IOnAttackEffect
    {
        void BeforeOnAttack(
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier attackModifier,
            RulesetAttackMode attackerAttackMode);

        void AfterOnAttack(
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier attackModifier,
            RulesetAttackMode attackerAttackMode);
    }

    public delegate void OnAttackDelegate(
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier attackModifier,
            RulesetAttackMode attackerAttackMode);

    /**
     * Before using this, please consider if FeatureDefinitionAdditionalDamage can cover the desired use case.
     * This has much greater flexibility, so there are cases where it is appropriate, but when possible it is
     * better for future maintainability of features to use the features provided by TA.
     */
    public class FeatureDefinitionOnAttackEffect : FeatureDefinition, IOnAttackEffect
    {
        private OnAttackDelegate beforeOnAttack;
        private OnAttackDelegate afterOnAttack;

        internal void SetOnAttackDelegates(OnAttackDelegate before = null, OnAttackDelegate after = null)
        {
            beforeOnAttack = before;
            afterOnAttack = after;
        }

        public void BeforeOnAttack(
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier attackModifier,
            RulesetAttackMode attackerAttackMode)
        {
            beforeOnAttack?.Invoke(attacker, defender, attackModifier, attackerAttackMode);
        }

        public void AfterOnAttack(
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier attackModifier,
            RulesetAttackMode attackerAttackMode)
        {
            afterOnAttack?.Invoke(attacker, defender, attackModifier, attackerAttackMode);
        }
    }
}
