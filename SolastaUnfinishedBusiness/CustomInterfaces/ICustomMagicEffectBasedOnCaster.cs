namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface ICustomMagicEffectBasedOnCaster
{
    EffectDescription GetCustomEffect(RulesetCharacter caster);
}
