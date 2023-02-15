namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface IOnComputeAttackModifier
{
    public void ComputeAttackModifier(
        RulesetCharacter myself,
        RulesetCharacter defender,
        BattleDefinitions.AttackProximity attackProximity,
        RulesetAttackMode attackMode,
        ref ActionModifier attackModifier);
}
