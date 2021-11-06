using SolastaModApi.Extensions;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionCastSpells;

namespace SolastaCommunityExpansion.Level20.SubClasses
{
    internal static class ShadowcasterBuilder
    {
        internal static void Load()
        {
            CastSpellShadowcaster.SetSpellCastingLevel(4);

            CastSpellShadowcaster.SlotsPerLevels.Clear();
            CastSpellShadowcaster.SlotsPerLevels.AddRange(SpellsHelper.OneThirdCastingSlots);

            CastSpellShadowcaster.ReplacedSpells.Clear();
            CastSpellShadowcaster.ReplacedSpells.AddRange(SpellsHelper.OneThirdCasterReplacedSpells);
        }
    }
}