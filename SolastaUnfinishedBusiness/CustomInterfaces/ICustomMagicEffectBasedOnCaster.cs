namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface ICustomMagicEffectBasedOnCaster
{
    public EffectDescription GetCustomEffect(RulesetCharacter caster);
}
