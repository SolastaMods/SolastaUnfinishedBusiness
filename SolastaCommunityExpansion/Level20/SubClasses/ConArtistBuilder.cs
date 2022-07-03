using SolastaCommunityExpansion.Api.Infrastructure;

namespace SolastaCommunityExpansion.Level20.SubClasses;

internal static class ConArtistBuilder
{
    internal static void Load()
    {
        DatabaseRepository.GetDatabase<FeatureDefinitionCastSpell>()
            .TryGetElement("CastSpellConArtist", out var featureDefinitionCastSpell);

        if (featureDefinitionCastSpell == null)
        {
            return;
        }

        featureDefinitionCastSpell.spellCastingLevel = 4;

        featureDefinitionCastSpell.SlotsPerLevels.SetRange(SpellsHelper.OneThirdCastingSlots);

        featureDefinitionCastSpell.ReplacedSpells.SetRange(SpellsHelper.OneThirdCasterReplacedSpells);
    }
}
