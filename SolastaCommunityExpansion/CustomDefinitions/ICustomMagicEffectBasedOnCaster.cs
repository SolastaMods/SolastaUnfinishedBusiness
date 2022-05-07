namespace SolastaCommunityExpansion.CustomDefinitions
{
    public interface ICustomMagicEffectBasedOnCaster
    {
        EffectDescription GetCustomEffect(RulesetCharacter caster);
    }
}
