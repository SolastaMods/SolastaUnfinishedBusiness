using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface IModifyMagicEffectRecurrent
{
    public void ModifyEffect(
        RulesetCondition rulesetCondition,
        EffectForm effectForm,
        [UsedImplicitly] RulesetActor rulesetActor);
}
