using SolastaModApi.Extensions;
using SolastaModApi.Infrastructure;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionCastSpells;

namespace SolastaCommunityExpansion.Level20.SubClasses
{
    internal static class MartialSpellBladeBuilder
    {
        internal static void Load()
        {
            CastSpellMartialSpellBlade.SetSpellCastingLevel(4);

            CastSpellMartialSpellBlade.SlotsPerLevels.SetRange(SpellsHelper.OneThirdCastingSlots);

            CastSpellMartialSpellBlade.ReplacedSpells.SetRange(SpellsHelper.OneThirdCasterReplacedSpells);
        }
    }
}
