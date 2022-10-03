namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface IOnComputeAttackModifier
{
    public void ComputeAttackModifier(
        RulesetCharacter myself,
        RulesetCharacter defender,
        RulesetAttackMode attackMode,
        ref ActionModifier attackModifier);
}

public delegate void OnComputeAttackModifier(
    RulesetCharacter myself,
    RulesetCharacter defender,
    RulesetAttackMode attackMode,
    ref ActionModifier attackModifier);
