namespace SolastaUnfinishedBusiness.CustomInterfaces;

internal interface ICustomMagicEffectBasedOnCaster
{
    EffectDescription GetCustomEffect(RulesetCharacter caster);
}
