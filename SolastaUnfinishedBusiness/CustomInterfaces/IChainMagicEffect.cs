namespace SolastaUnfinishedBusiness.CustomInterfaces;

internal interface IChainMagicEffect
{
    internal CharacterActionMagicEffect GetNextMagicEffect(CharacterActionMagicEffect baseEffect,
        CharacterActionAttack triggeredAttack, RuleDefinitions.RollOutcome attackOutcome);
}
