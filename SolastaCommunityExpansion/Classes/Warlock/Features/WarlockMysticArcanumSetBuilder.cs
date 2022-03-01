using System.Collections.Generic;
using System.Linq;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaModApi.Extensions;
using static SolastaCommunityExpansion.Classes.Warlock.WarlockSpellList;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionFeatureSets;

namespace SolastaCommunityExpansion.Classes.Warlock.Features
{
    internal static class WarlockMysticArcanumSetBuilder
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

        public static FeatureDefinitionFeatureSet CreateSet11AndAddToDB()
        {
            return FeatureDefinitionFeatureSetBuilder
                .Create(TerrainTypeAffinityRangerNaturalExplorerChoice, "ClassWarlockMysticArcanumSetLevel11", DefinitionBuilder.CENamespaceGuid)
                .SetGuiPresentation("ClassWarlockMysticArcanumSet", Category.Feature)
                .SetFeatureSet(GetSpells(6).Select(spell => CreatePower("DH_MysticArcanum11_", spell)))
                .SetUniqueChoices(true)
                .AddToDB();
        }

        public static FeatureDefinitionFeatureSet CreateSet13AndAddToDB()
        {
            return FeatureDefinitionFeatureSetBuilder
                .Create(TerrainTypeAffinityRangerNaturalExplorerChoice, "ClassWarlockMysticArcanumSetLevel13", DefinitionBuilder.CENamespaceGuid)
                .SetGuiPresentation("ClassWarlockMysticArcanumSet", Category.Feature)
                .SetFeatureSet(GetSpells(7, 6).Select(spell => CreatePower("DH_MysticArcanum13_", spell)))
                .SetUniqueChoices(true)
                .AddToDB();
        }

        public static FeatureDefinitionFeatureSet CreateSet15AndAddToDB()
        {
            return FeatureDefinitionFeatureSetBuilder
                .Create(TerrainTypeAffinityRangerNaturalExplorerChoice, "ClassWarlockMysticArcanumSetLevel15", DefinitionBuilder.CENamespaceGuid)
                .SetGuiPresentation("ClassWarlockMysticArcanumSet", Category.Feature)
                .SetFeatureSet(GetSpells(8, 7, 6).Select(spell => CreatePower("DH_MysticArcanum15_", spell)))
                .SetUniqueChoices(true)
                .AddToDB();
        }

        public static FeatureDefinitionFeatureSet CreateSet17AndAddToDB()
        {
            return FeatureDefinitionFeatureSetBuilder
                .Create(TerrainTypeAffinityRangerNaturalExplorerChoice, "ClassWarlockMysticArcanumSetLevel17", DefinitionBuilder.CENamespaceGuid)
                .SetGuiPresentation("ClassWarlockMysticArcanumSet", Category.Feature)
                .SetFeatureSet(GetSpells(9, 8, 7, 6).Select(spell => CreatePower("DH_MysticArcanum17_", spell)))
                .SetUniqueChoices(true)
                .AddToDB();
        }

        public static readonly FeatureDefinitionFeatureSet WarlockMysticArcanumSetLevel11 = CreateSet11AndAddToDB();
        public static readonly FeatureDefinitionFeatureSet WarlockMysticArcanumSetLevel13 = CreateSet13AndAddToDB();
        public static readonly FeatureDefinitionFeatureSet WarlockMysticArcanumSetLevel15 = CreateSet15AndAddToDB();
        public static readonly FeatureDefinitionFeatureSet WarlockMysticArcanumSetLevel17 = CreateSet17AndAddToDB();
    }
}
