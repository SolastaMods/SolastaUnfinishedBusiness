//DEPRECATED: use IPhysicalAttackInitiated instead
#if false
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.CustomInterfaces;

/**
 * Called after roll is made, but before damage is applied.
 * Called regardless of whether attack hits or not.
 */
public interface IAttackEffectBeforeDamage
{
    [UsedImplicitly]
    public void OnAttackEffectBeforeDamage(
        GameLocationCharacter attacker,
        GameLocationCharacter defender,
        RuleDefinitions.RollOutcome outcome,
        CharacterActionParams actionParams,
        RulesetAttackMode attackMode,
        ActionModifier attackModifier);
}
#endif
