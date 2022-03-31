using SolastaModApi.Infrastructure;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionCastSpells;

namespace SolastaCommunityExpansion.Level20.Races
{
    internal static class ElfHighBuilder
    {
        internal static void Load()
        {
            CastSpellElfHigh.ReplacedSpells.SetRange(SpellsHelper.EmptyReplacedSpells);
        }
    }
}
