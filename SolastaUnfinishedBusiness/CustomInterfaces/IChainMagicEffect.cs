using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface IChainMagicEffect
{
    [UsedImplicitly]
    public CharacterActionMagicEffect GetNextMagicEffect(
        CharacterActionMagicEffect baseEffect,
        CharacterActionAttack triggeredAttack,
        RuleDefinitions.RollOutcome attackOutcome);
}
