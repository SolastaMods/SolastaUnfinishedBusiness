namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface IModifyMagicEffectOnTarget
{
    public EffectDescription ModifyEffect(
        BaseDefinition definition,
        EffectDescription effect,
        RulesetCharacter caster,
        RulesetCharacter target);
}
