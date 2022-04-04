using System;
using System.Collections.Generic;
using System.Linq;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaCommunityExpansion.Models;
using SolastaModApi.Infrastructure;

namespace SolastaCommunityExpansion.Feats
{
    internal static class FightingStyleFeats
    {
        public static readonly Guid FightingStyleFeatsNamespace = new("db157827-0f8a-4fbb-bb87-6d54689a587a");

        public static void CreateFeats(List<FeatDefinition> feats)
        {
            feats.AddRange(
                BuildFightingStyleFeat("TwoWeapon"),
                BuildFightingStyleFeat("Protection"),
                BuildFightingStyleFeat("GreatWeapon"),
                BuildFightingStyleFeat("Dueling"),
                BuildFightingStyleFeat("Defense"),
                BuildFightingStyleFeat("Archery")
            );

            feats.AddRange(
                FightingStyleContext.FightingStyles.Values
                    .Select(fs => BuildFightingStyleFeat(fs)));
        }

        private static FeatDefinition BuildFightingStyleFeat(string style)
        {
            return FeatDefinitionBuilder
                .Create($"FeatFightingStyle{style}", FightingStyleFeatsNamespace)
                .SetFeatures(
                    FeatureDefinitionProficiencyBuilder
                        .Create($"FeatFightingStyle{style}Proficiency", FightingStyleFeatsNamespace)
                        .SetProficiencies(RuleDefinitions.ProficiencyType.FightingStyle, style)
                        .SetGuiPresentation($"FightingStyle{style}", Category.Feat)
                        .AddToDB()
            )
            .SetGuiPresentation($"FightingStyle{style}", Category.Feat)
            .AddToDB();
        }

        private static FeatDefinition BuildFightingStyleFeat(FightingStyleDefinition fightingStyle)
        {
            return FeatDefinitionBuilder
                .Create($"Feat{fightingStyle.Name}", FightingStyleFeatsNamespace)
                .SetFeatures(
                    FeatureDefinitionProficiencyBuilder
                        .Create($"Feat{fightingStyle.Name}Proficiency", FightingStyleFeatsNamespace)
                        .SetProficiencies(RuleDefinitions.ProficiencyType.FightingStyle, fightingStyle.Name)
                        .SetGuiPresentation(fightingStyle.GuiPresentation)
                        .AddToDB()
            )
            .SetGuiPresentation(Category.Feat)
            .AddToDB();
        }
    }
}
