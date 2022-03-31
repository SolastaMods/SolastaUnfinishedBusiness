using SolastaModApi.Extensions;
using SolastaModApi.Infrastructure;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionCastSpells;

namespace SolastaCommunityExpansion.Level20.SubClasses
{
    internal static class ShadowcasterBuilder
    {
        internal static void Load()
        {
            CastSpellShadowcaster.SetSpellCastingLevel(4);

            CastSpellShadowcaster.SlotsPerLevels.SetRange(SpellsHelper.OneThirdCastingSlots);

            CastSpellShadowcaster.ReplacedSpells.SetRange(SpellsHelper.OneThirdCasterReplacedSpells);
        }
    }
}
