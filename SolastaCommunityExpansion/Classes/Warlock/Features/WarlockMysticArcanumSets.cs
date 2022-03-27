using System.Collections.Generic;
using System.Linq;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaModApi.Extensions;
using static SolastaCommunityExpansion.Classes.Warlock.ClassWarlockSpellList;
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
            return levels.SelectMany(level => ClassWarlockSpellList.WarlockSpellList.SpellsByLevel[level].Spells);
        }

        private static FeatureDefinitionFeatureSet Create(int setLevel, params int[] spellLevels)
        {
            return FeatureDefinitionFeatureSetBuilder
                .Create(TerrainTypeAffinityRangerNaturalExplorerChoice, $"ClassWarlockMysticArcanumSetLevel{setLevel}", DefinitionBuilder.CENamespaceGuid)
                .SetGuiPresentation("ClassWarlockMysticArcanumSet", Category.Feature)
                .SetFeatureSet(GetSpells(spellLevels).Select(spell => CreatePower($"DH_MysticArcanum{setLevel}_", spell)))
                .SetUniqueChoices(true)
                .AddToDB();
        }

        public static readonly FeatureDefinitionFeatureSet WarlockMysticArcanumSetLevel11 = Create(11, 6);
        public static readonly FeatureDefinitionFeatureSet WarlockMysticArcanumSetLevel13 = Create(13, 7, 6);
        public static readonly FeatureDefinitionFeatureSet WarlockMysticArcanumSetLevel15 = Create(15, 8, 7, 6);
        public static readonly FeatureDefinitionFeatureSet WarlockMysticArcanumSetLevel17 = Create(17, 9, 8, 7, 6);
    }
}
