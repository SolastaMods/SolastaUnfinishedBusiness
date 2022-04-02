using SolastaModApi.Extensions;
using SolastaModApi.Infrastructure;

namespace SolastaCommunityExpansion.Level20.SubClasses
{
    internal static class ConArtistBuilder
    {
        internal static void Load()
        {
            DatabaseRepository.GetDatabase<FeatureDefinitionCastSpell>().TryGetElement("CastSpellConArtist", out FeatureDefinitionCastSpell featureDefinitionCastSpell);

            if (featureDefinitionCastSpell != null)
            {
                featureDefinitionCastSpell.SetSpellCastingLevel(4);

                featureDefinitionCastSpell.SlotsPerLevels.SetRange(SpellsHelper.OneThirdCastingSlots);

                featureDefinitionCastSpell.ReplacedSpells.SetRange(SpellsHelper.OneThirdCasterReplacedSpells);
            }
        }
    }
}
