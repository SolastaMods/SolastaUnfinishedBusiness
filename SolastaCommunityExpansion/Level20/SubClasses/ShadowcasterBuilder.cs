using SolastaCommunityExpansion.Api.Infrastructure;
using static SolastaCommunityExpansion.Api.DatabaseHelper.FeatureDefinitionCastSpells;

namespace SolastaCommunityExpansion.Level20.SubClasses;

internal static class ShadowcasterBuilder
{
    internal static void Load()
    {
        CastSpellShadowcaster.spellCastingLevel = 4;

        CastSpellShadowcaster.SlotsPerLevels.SetRange(SpellsHelper.OneThirdCastingSlots);

        CastSpellShadowcaster.ReplacedSpells.SetRange(SpellsHelper.OneThirdCasterReplacedSpells);
    }
}
