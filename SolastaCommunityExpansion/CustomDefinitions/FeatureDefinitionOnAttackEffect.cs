namespace SolastaCommunityExpansion.CustomDefinitions
{
    public interface IOnAttackEffect
    {
        void OnAttack(
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
        private OnAttackDelegate onAttack;

        internal void SetOnAttackDelegate(OnAttackDelegate del)
        {
            onAttack = del;
        }

        public void OnAttack(
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier attackModifier,
            RulesetAttackMode attackerAttackMode)
        {
            onAttack?.Invoke(attacker, defender, attackModifier, attackerAttackMode);
        }
    }
}
