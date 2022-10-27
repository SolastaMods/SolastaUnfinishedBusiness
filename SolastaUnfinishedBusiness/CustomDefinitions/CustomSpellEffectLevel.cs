using JetBrains.Annotations;
using SolastaUnfinishedBusiness.CustomInterfaces;

namespace SolastaUnfinishedBusiness.CustomDefinitions;

internal static class CustomSpellEffectLevel
{
    internal static readonly ICustomSpellEffectLevel ByCasterLevel = new SpellEffectLevelFromCasterLevel();
}

internal sealed class SpellEffectLevelFromCasterLevel : ICustomSpellEffectLevel
{
    public int GetEffectLevel([NotNull] RulesetActor caster)
    {
        return caster.GetAttribute(AttributeDefinitions.CharacterLevel).CurrentValue;
    }
}
