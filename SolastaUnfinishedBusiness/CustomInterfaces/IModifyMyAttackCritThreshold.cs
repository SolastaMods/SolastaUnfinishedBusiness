namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface IModifyMyAttackCritThreshold
{
    public int TryModifyMyAttackCritThreshold(int current, RulesetCharacter me, RulesetCharacter target,
        BaseDefinition attackMethod);
}
