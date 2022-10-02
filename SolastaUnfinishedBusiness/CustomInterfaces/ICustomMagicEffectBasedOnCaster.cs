namespace SolastaUnfinishedBusiness.CustomInterfaces;

internal interface ICustomMagicEffectBasedOnCaster
{
    public EffectDescription GetCustomEffect(RulesetCharacter caster);
}
