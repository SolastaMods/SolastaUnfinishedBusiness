using SolastaModApi.Extensions;

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

                featureDefinitionCastSpell.SlotsPerLevels.Clear();
                featureDefinitionCastSpell.SlotsPerLevels.AddRange(SpellsHelper.OneThirdCastingSlots);

                featureDefinitionCastSpell.ReplacedSpells.Clear();
                featureDefinitionCastSpell.ReplacedSpells.AddRange(SpellsHelper.OneThirdCasterReplacedSpells);
            }
        }
    }
}