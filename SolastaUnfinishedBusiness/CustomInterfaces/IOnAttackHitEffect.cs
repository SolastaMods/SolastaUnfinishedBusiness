namespace SolastaUnfinishedBusiness.CustomInterfaces;

/**
     * Provides ways to react to attack (not spell) hits/misses
     */
public interface IOnAttackHitEffect
{
    /**
         * Called after roll is made, but before damage is applied.
         * Called regardless of whether attack hits or not.
         */
#if false
    public void BeforeOnAttackHit1(
        GameLocationCharacter attacker,
        GameLocationCharacter defender,
        RuleDefinitions.RollOutcome outcome,
        CharacterActionParams actionParams,
        RulesetAttackMode attackMode,
        ActionModifier attackModifier);
#endif

    /**
         * Called after damage is applied (or would have been applied if it was a hit).
         * Called regardless of whether attack hits or not.
         */
    public void AfterOnAttackHit(
        GameLocationCharacter attacker,
        GameLocationCharacter defender,
        RuleDefinitions.RollOutcome outcome,
        CharacterActionParams actionParams,
        RulesetAttackMode attackMode,
        ActionModifier attackModifier);
}
