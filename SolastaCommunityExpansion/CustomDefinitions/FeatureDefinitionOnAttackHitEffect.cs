using SolastaCommunityExpansion.CustomInterfaces;

namespace SolastaCommunityExpansion.CustomDefinitions
{
    /**
     * Before using this, please consider if FeatureDefinitionAdditionalDamage can cover the desired use case.
     * This has much greater flexibility, so there are cases where it is appropriate, but when possible it is
     * better for future maintainability of features to use the features provided by TA.
     */
    public class FeatureDefinitionOnAttackHitEffect : FeatureDefinition, IOnAttackHitEffect
    {
        private OnAttackHitDelegate afterOnAttackHit;
        private OnAttackHitDelegate beforeOnAttackHit;

        public void BeforeOnAttackHit(
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RuleDefinitions.RollOutcome outcome,
            CharacterActionParams actionParams,
            RulesetAttackMode attackMode,
            ActionModifier attackModifier)
        {
            beforeOnAttackHit?.Invoke(attacker, defender,outcome,actionParams, attackMode, attackModifier);
        }

        public void AfterOnAttackHit(
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RuleDefinitions.RollOutcome outcome,
            CharacterActionParams actionParams,
            RulesetAttackMode attackMode,
            ActionModifier attackModifier)
        {
            afterOnAttackHit?.Invoke(attacker, defender,outcome,actionParams, attackMode, attackModifier);
        }

        internal void SetOnAttackHitDelegates(OnAttackHitDelegate before = null, OnAttackHitDelegate after = null)
        {
            beforeOnAttackHit = before;
            afterOnAttackHit = after;
        }
    }
}
