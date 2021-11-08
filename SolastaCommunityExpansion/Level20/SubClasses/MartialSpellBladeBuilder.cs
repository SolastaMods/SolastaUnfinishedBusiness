using SolastaModApi.Extensions;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionCastSpells;

namespace SolastaCommunityExpansion.Level20.SubClasses
{
    internal static class MartialSpellBladeBuilder
    {
        internal static void Load()
        {
            CastSpellMartialSpellBlade.SetSpellCastingLevel(4);

            CastSpellMartialSpellBlade.SlotsPerLevels.Clear();
            CastSpellMartialSpellBlade.SlotsPerLevels.AddRange(SpellsHelper.OneThirdCastingSlots);

            CastSpellMartialSpellBlade.ReplacedSpells.Clear();
            CastSpellMartialSpellBlade.ReplacedSpells.AddRange(SpellsHelper.OneThirdCasterReplacedSpells);
        }
    }
}