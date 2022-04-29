namespace SolastaCommunityExpansion.CustomDefinitions
{
    public interface IOnAttackHitEffect
    {
        void BeforeOnAttackHit(
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier attackModifier,
            int attackRoll,
            int successDelta,
            bool ranged);

        void AfterOnAttackHit(
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier attackModifier,
            int attackRoll,
            int successDelta,
            bool ranged);
    }

    public delegate void OnAttackHitDelegate(
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier attackModifier,
            int attackRoll,
            int successDelta,
            bool ranged);
}
