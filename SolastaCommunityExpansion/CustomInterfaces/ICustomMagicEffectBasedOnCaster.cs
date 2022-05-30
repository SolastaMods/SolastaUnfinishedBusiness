namespace SolastaCommunityExpansion.CustomInterfaces;

public interface ICustomMagicEffectBasedOnCaster
{
    EffectDescription GetCustomEffect(RulesetCharacter caster);
}
