using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.CustomInterfaces;

/**
 * Called after damage is applied (or would have been applied if it was a hit).
 * Called regardless of whether attack hits or not.
 */
public interface IAttackEffectAfterDamage
{
    [UsedImplicitly]
    public void OnAttackEffectAfterDamage(
        GameLocationCharacter attacker,
        GameLocationCharacter defender,
        RuleDefinitions.RollOutcome outcome,
        CharacterActionParams actionParams,
        RulesetAttackMode attackMode,
        ActionModifier attackModifier);
}
