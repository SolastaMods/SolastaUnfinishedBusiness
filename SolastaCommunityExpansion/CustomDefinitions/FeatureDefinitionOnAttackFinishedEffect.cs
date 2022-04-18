namespace SolastaCommunityExpansion.CustomDefinitions
{
    public interface IOnAttackFinishedEffect
    {
        void OnAttackFinished(
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RulesetAttackMode attackerAttackMode);
    }

    public delegate void OnAttackFinishedDelegate(
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RulesetAttackMode attackerAttackMode);

    /**
     * Before using this, please consider if FeatureDefinitionAdditionalDamage can cover the desired use case.
     * This has much greater flexibility, so there are cases where it is appropriate, but when possible it is
     * better for future maintainability of features to use the features provided by TA.
     */
    public class FeatureDefinitionOnAttackFinishedEffect : FeatureDefinition, IOnAttackFinishedEffect
    {
        private OnAttackFinishedDelegate onAttackFinished;

        internal void SetOnAttackFinishedDelegate(OnAttackFinishedDelegate del)
        {
            onAttackFinished = del;
        }

        public void OnAttackFinished(
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RulesetAttackMode attackerAttackMode)
        {
            onAttackFinished?.Invoke(attacker, defender, attackerAttackMode);
        }
    }
}
