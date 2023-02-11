using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface IModifyMagicEffectOnTarget
{
    [UsedImplicitly]
    public EffectDescription ModifyEffect(
        BaseDefinition definition,
        EffectDescription effect,
        RulesetCharacter caster,
        RulesetCharacter target);
}
