using System;
using System.Collections.Generic;
using System.Linq;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaCommunityExpansion.CustomDefinitions;
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
                FightingStyleContext.FightingStyles
                    .Select(fs => BuildFightingStyleFeat(fs)));
        }

        private static FeatDefinition BuildFightingStyleFeat(string style)
        {
            var fightingStyle = DatabaseRepository
                .GetDatabase<FightingStyleDefinition>()
                .GetElement(style);

            return FeatDefinitionBuilder<FeatDefinitionWithPrerequisites, FeatDefinitionWithPrerequisitesBuilder>
                .Create($"FeatFightingStyle{style}", FightingStyleFeatsNamespace)
                .SetFeatures(
                    FeatureDefinitionProficiencyBuilder
                        .Create($"FeatFightingStyle{style}Proficiency", FightingStyleFeatsNamespace)
                        .SetProficiencies(RuleDefinitions.ProficiencyType.FightingStyle, style)
                        .SetGuiPresentation($"FightingStyle{style}", Category.Feat)
                        .AddToDB()
                )
                .SetValidators((feat, hero) =>
                {
                    var hasFightingStyle = hero.TrainedFightingStyles
                        .Any(x => x.Name == style);

                    if (!hasFightingStyle)
                    {
                        return (true,
                            Gui.Format("Tooltip/&FeatPrerequisiteDoesNotHaveFightingStyle",
                                fightingStyle.FormatTitle()));
                    }

                    return (false,
                        Gui.Colorize(
                            Gui.Format("Tooltip/&FeatPrerequisiteDoesNotHaveFightingStyle",
                                fightingStyle.FormatTitle()),
                            "EA7171"));
                })
                .SetGuiPresentation($"FightingStyle{style}", Category.Feat)
                .AddToDB();
        }

        private static FeatDefinition BuildFightingStyleFeat(FightingStyleDefinition fightingStyle)
        {
            return FeatDefinitionBuilder<FeatDefinitionWithPrerequisites, FeatDefinitionWithPrerequisitesBuilder>
                .Create($"Feat{fightingStyle.Name}", FightingStyleFeatsNamespace)
                .SetFeatures(
                    FeatureDefinitionProficiencyBuilder
                        .Create($"Feat{fightingStyle.Name}Proficiency", FightingStyleFeatsNamespace)
                        .SetProficiencies(RuleDefinitions.ProficiencyType.FightingStyle, fightingStyle.Name)
                        .SetGuiPresentation(fightingStyle.GuiPresentation)
                        .AddToDB()
                )
                .SetValidators((feat, hero) =>
                {
                    var hasFightingStyle = hero.TrainedFightingStyles
                        .Any(x => x.Name == fightingStyle.Name);

                    if (!hasFightingStyle)
                    {
                        return (true,
                            Gui.Format("Tooltip/&FeatPrerequisiteDoesNotHaveFightingStyle",
                                fightingStyle.FormatTitle()));
                    }

                    return (false,
                        Gui.Colorize(
                            Gui.Format("Tooltip/&FeatPrerequisiteDoesNotHaveFightingStyle",
                                fightingStyle.FormatTitle()),
                            "EA7171"));
                })
                .SetGuiPresentation(Category.Feat)
                .AddToDB();
        }
    }
}
