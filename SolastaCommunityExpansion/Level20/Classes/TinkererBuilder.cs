using SolastaModApi.Extensions;

namespace SolastaCommunityExpansion.Level20.Classes
{
    internal static class TinkererBuilder
    {
        internal static void Load()
        {
            DatabaseRepository.GetDatabase<FeatureDefinitionCastSpell>().TryGetElement("CastSpellTinkerer", out FeatureDefinitionCastSpell featureDefinitionCastSpell);

            if (featureDefinitionCastSpell != null)
            {
                featureDefinitionCastSpell.SetSpellCastingLevel(5);

                featureDefinitionCastSpell.SlotsPerLevels.Clear();
                featureDefinitionCastSpell.SlotsPerLevels.AddRange(SpellsHelper.ArtificerCastingSlots);

                featureDefinitionCastSpell.ReplacedSpells.Clear();
                featureDefinitionCastSpell.ReplacedSpells.AddRange(SpellsHelper.FullCasterReplacedSpells);
            }
        }
    }
}