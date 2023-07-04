using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface IModifyAttackCriticalThreshold
{
    [UsedImplicitly]
    public int GetCriticalThreshold(
        int current, RulesetCharacter me, RulesetCharacter target, BaseDefinition attackMethod);
}
