namespace SolastaCommunityExpansion.CustomInterfaces;

/**
     * Provides ways to react to attack (not spell) hits/misses
     */
public interface IOnAttackHitEffect
{
    /**
         * Called after roll is made, but before damage is applied.
         * Called regardless of whether attack hits or not.
         */
    void BeforeOnAttackHit(
        GameLocationCharacter attacker,
        GameLocationCharacter defender,
        RuleDefinitions.RollOutcome outcome,
        CharacterActionParams actionParams,
        RulesetAttackMode attackMode,
        ActionModifier attackModifier);

    /**
         * Called after damage is applied (or would have been applied if it was a hit).
         * Called regardless of whether attack hits or not.
         */
    void AfterOnAttackHit(
        GameLocationCharacter attacker,
        GameLocationCharacter defender,
        RuleDefinitions.RollOutcome outcome,
        CharacterActionParams actionParams,
        RulesetAttackMode attackMode,
        ActionModifier attackModifier);
}

public delegate void OnAttackHitDelegate(
    GameLocationCharacter attacker,
    GameLocationCharacter defender,
    RuleDefinitions.RollOutcome outcome,
    CharacterActionParams actionParams,
    RulesetAttackMode attackMode,
    ActionModifier attackModifier);
