using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface IModifyRecurrentMagicEffect
{
    public void ModifyEffect(
        RulesetCondition rulesetCondition,
        EffectForm effectForm,
        [UsedImplicitly] RulesetActor rulesetActor);
}
