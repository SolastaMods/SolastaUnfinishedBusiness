namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface IAttackComputeModifier
{
    public void OnAttackComputeModifier(
        RulesetCharacter myself,
        RulesetCharacter defender,
        BattleDefinitions.AttackProximity attackProximity,
        RulesetAttackMode attackMode,
        ref ActionModifier attackModifier);
}
