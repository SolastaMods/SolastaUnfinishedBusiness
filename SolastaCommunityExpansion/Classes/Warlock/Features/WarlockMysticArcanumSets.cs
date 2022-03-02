using System.Collections.Generic;
using System.Linq;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaModApi.Extensions;
using static SolastaCommunityExpansion.Classes.Warlock.WarlockSpellList;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionFeatureSets;

namespace SolastaCommunityExpansion.Classes.Warlock.Features
{
    internal static class WarlockMysticArcanumSets
    {
        private static FeatureDefinitionPower CreatePower(string baseName, SpellDefinition spell)
        {
            return FeatureDefinitionPowerBuilder
                .Create(baseName + spell.name, DefinitionBuilder.CENamespaceGuid)
                .SetGuiPresentation(spell.GuiPresentation)
                .Configure(
                    1,
                    RuleDefinitions.UsesDetermination.Fixed,
                    AttributeDefinitions.Charisma,
                    spell.ActivationTime,
                    1,
                    RuleDefinitions.RechargeRate.LongRest,
                    false,
                    false,
                    AttributeDefinitions.Charisma,
                    spell.EffectDescription,
                    true)
                .AddToDB();
        }

        private static IEnumerable<SpellDefinition> GetSpells(params int[] levels)
        {
            return levels.SelectMany(level => ClassWarlockSpellList.SpellsByLevel[level].Spells);
        }

        private static FeatureDefinitionFeatureSet Create(string name, string powerName, params int[] levels)
        {
            return FeatureDefinitionFeatureSetBuilder
                .Create(TerrainTypeAffinityRangerNaturalExplorerChoice, name, DefinitionBuilder.CENamespaceGuid)
                .SetGuiPresentation("ClassWarlockMysticArcanumSet", Category.Feature)
                .SetFeatureSet(GetSpells(levels).Select(spell => CreatePower(powerName, spell)))
                .SetUniqueChoices(true)
                .AddToDB();
        }

        public static readonly FeatureDefinitionFeatureSet WarlockMysticArcanumSetLevel11 
            = Create("ClassWarlockMysticArcanumSetLevel11", "DH_MysticArcanum11_", 6);

        public static readonly FeatureDefinitionFeatureSet WarlockMysticArcanumSetLevel13
            = Create("ClassWarlockMysticArcanumSetLevel13", "DH_MysticArcanum13_", 7, 6);

        public static readonly FeatureDefinitionFeatureSet WarlockMysticArcanumSetLevel15
            = Create("ClassWarlockMysticArcanumSetLevel15", "DH_MysticArcanum15_", 8, 7, 6);

        public static readonly FeatureDefinitionFeatureSet WarlockMysticArcanumSetLevel17
            = Create("ClassWarlockMysticArcanumSetLevel17", "DH_MysticArcanum17_", 9, 8, 7, 6);
    }
}
