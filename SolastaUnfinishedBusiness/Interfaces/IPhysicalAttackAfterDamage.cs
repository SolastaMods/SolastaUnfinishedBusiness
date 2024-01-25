using JetBrains.Annotations;
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.Interfaces;

/**
 * Called after damage is applied (or would have been applied if it was a hit).
 * Called regardless of whether attack hits or not.
 */
public interface IPhysicalAttackAfterDamage
{
    [UsedImplicitly]
    public void OnPhysicalAttackAfterDamage(
        GameLocationCharacter attacker,
        GameLocationCharacter defender,
        RollOutcome outcome,
        CharacterActionParams actionParams,
        RulesetAttackMode attackMode,
        ActionModifier attackModifier);
}
