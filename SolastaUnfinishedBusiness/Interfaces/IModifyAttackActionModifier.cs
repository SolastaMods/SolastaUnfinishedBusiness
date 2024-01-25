namespace SolastaUnfinishedBusiness.Interfaces;

public interface IModifyAttackActionModifier
{
    public void OnAttackComputeModifier(
        RulesetCharacter myself,
        RulesetCharacter defender,
        BattleDefinitions.AttackProximity attackProximity,
        RulesetAttackMode attackMode,
        string effectName,
        ref ActionModifier attackModifier);
}
