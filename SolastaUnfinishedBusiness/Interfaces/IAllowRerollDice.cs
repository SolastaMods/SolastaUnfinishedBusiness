using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Interfaces;

public interface IAllowRerollDice
{
    public bool IsValid(
        [UsedImplicitly] RulesetActor rulesetActor, DamageForm damageForm);
}
