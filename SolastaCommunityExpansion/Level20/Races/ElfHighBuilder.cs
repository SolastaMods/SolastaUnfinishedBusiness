using SolastaCommunityExpansion.Api.Infrastructure;
using static SolastaCommunityExpansion.Api.DatabaseHelper.FeatureDefinitionCastSpells;

namespace SolastaCommunityExpansion.Level20.Races;

internal static class ElfHighBuilder
{
    internal static void Load()
    {
        CastSpellElfHigh.ReplacedSpells.SetRange(SpellsHelper.EmptyReplacedSpells);
    }
}
