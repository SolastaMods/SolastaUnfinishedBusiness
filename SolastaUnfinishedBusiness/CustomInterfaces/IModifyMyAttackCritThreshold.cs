using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface IModifyMyAttackCritThreshold
{
    [UsedImplicitly]
    public int TryModifyMyAttackCritThreshold(
        int current, RulesetCharacter me, RulesetCharacter target, BaseDefinition attackMethod);
}
