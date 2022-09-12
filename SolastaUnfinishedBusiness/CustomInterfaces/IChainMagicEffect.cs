namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface IChainMagicEffect
{
    public CharacterActionMagicEffect GetNextMagicEffect(CharacterActionMagicEffect baseEffect,
        CharacterActionAttack triggeredAttack, RuleDefinitions.RollOutcome attackOutcome);
}
