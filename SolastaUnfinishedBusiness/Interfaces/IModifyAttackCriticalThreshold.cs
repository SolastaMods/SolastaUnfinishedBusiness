using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Interfaces;

public interface IModifyAttackCriticalThreshold
{
    [UsedImplicitly]
    public int GetCriticalThreshold(
        int current, RulesetCharacter me, RulesetCharacter target, BaseDefinition attackMethod);
}
