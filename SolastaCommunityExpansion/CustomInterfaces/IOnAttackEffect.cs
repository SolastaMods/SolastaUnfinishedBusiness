namespace SolastaCommunityExpansion.CustomInterfaces
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
}
