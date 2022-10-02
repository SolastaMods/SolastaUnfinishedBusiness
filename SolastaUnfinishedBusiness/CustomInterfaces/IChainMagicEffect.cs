namespace SolastaUnfinishedBusiness.CustomInterfaces;

internal interface IChainMagicEffect
{
    public CharacterActionMagicEffect GetNextMagicEffect(CharacterActionMagicEffect baseEffect,
        CharacterActionAttack triggeredAttack, RuleDefinitions.RollOutcome attackOutcome);
}
