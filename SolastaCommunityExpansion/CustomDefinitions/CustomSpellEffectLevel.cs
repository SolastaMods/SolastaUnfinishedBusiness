namespace SolastaCommunityExpansion.CustomDefinitions
{
    public interface ICustomSpellEffectLevel
    {
        public int GetEffectLevel(RulesetActor caster);
    }

    public static class CustomSpellEffectLevel
    {
        public static readonly ICustomSpellEffectLevel ByCasterLevel = new SpellEffectLevelFromCasterLevel();
    }

    internal class SpellEffectLevelFromCasterLevel: ICustomSpellEffectLevel
    {
        public int GetEffectLevel(RulesetActor caster)
        {
            return caster.GetAttribute(AttributeDefinitions.CharacterLevel).CurrentValue;
        }
    }
}
