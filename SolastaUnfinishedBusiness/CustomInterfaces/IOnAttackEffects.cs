using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.CustomInterfaces;

/**
 * Called after roll is made, but before damage is applied.
 * Called regardless of whether attack hits or not.
 */
public interface IBeforeAttackEffect
{
    [UsedImplicitly]
    public void BeforeOnAttackHit(
        GameLocationCharacter attacker,
        GameLocationCharacter defender,
        RuleDefinitions.RollOutcome outcome,
        CharacterActionParams actionParams,
        RulesetAttackMode attackMode,
        ActionModifier attackModifier);
}

/**
 * Called after damage is applied (or would have been applied if it was a hit).
 * Called regardless of whether attack hits or not.
 */
public interface IAfterAttackEffect
{
    [UsedImplicitly]
    public void AfterOnAttackHit(
        GameLocationCharacter attacker,
        GameLocationCharacter defender,
        RuleDefinitions.RollOutcome outcome,
        CharacterActionParams actionParams,
        RulesetAttackMode attackMode,
        ActionModifier attackModifier);
}
