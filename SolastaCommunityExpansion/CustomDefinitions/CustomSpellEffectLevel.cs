using JetBrains.Annotations;

namespace SolastaCommunityExpansion.CustomDefinitions;

public interface ICustomSpellEffectLevel
{
    public int GetEffectLevel(RulesetActor caster);
}

public static class CustomSpellEffectLevel
{
    public static readonly ICustomSpellEffectLevel ByCasterLevel = new SpellEffectLevelFromCasterLevel();
}

internal sealed class SpellEffectLevelFromCasterLevel : ICustomSpellEffectLevel
{
    public int GetEffectLevel([NotNull] RulesetActor caster)
    {
        return caster.GetAttribute(AttributeDefinitions.CharacterLevel).CurrentValue;
    }
}
