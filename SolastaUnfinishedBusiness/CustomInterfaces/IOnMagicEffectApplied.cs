using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface IOnMagicEffectApplied
{
    [UsedImplicitly]
    public void OnMagicEffectApplied(
        BaseDefinition definition,
        EffectDescription effect,
        RulesetCharacter caster,
        RulesetCharacter target);
}
