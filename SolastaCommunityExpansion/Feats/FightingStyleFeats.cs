using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaCommunityExpansion.Api.Infrastructure;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaCommunityExpansion.CustomDefinitions;
using SolastaCommunityExpansion.Models;

namespace SolastaCommunityExpansion.Feats;

internal static class FightingStyleFeats
{
    private static readonly Guid FightingStyleFeatsNamespace = new("db157827-0f8a-4fbb-bb87-6d54689a587a");

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
                .Select(BuildFightingStyleFeat));
    }

    private static FeatDefinition BuildFightingStyleFeat(string style)
    {
        var fightingStyle = DatabaseRepository
            .GetDatabase<FightingStyleDefinition>()
            .GetElement(style);

        return FeatDefinitionBuilder<FeatDefinitionWithPrerequisites, FeatDefinitionWithPrerequisitesBuilder>
            .Create($"Feat{style}", FightingStyleFeatsNamespace)
            .SetFeatures(
                FeatureDefinitionProficiencyBuilder
                    .Create($"ProficiencyFeat{style}", FightingStyleFeatsNamespace)
                    .SetProficiencies(RuleDefinitions.ProficiencyType.FightingStyle, style)
                    .SetGuiPresentation($"FightingStyle{style}", Category.Feat)
                    .AddToDB()
            )
            .SetValidators((_, hero) =>
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
            .SetGuiPresentation(Category.Feat)
            .AddToDB();
    }

    private static FeatDefinition BuildFightingStyleFeat([NotNull] BaseDefinition fightingStyle)
    {
        return FeatDefinitionBuilder<FeatDefinitionWithPrerequisites, FeatDefinitionWithPrerequisitesBuilder>
            .Create($"Feat{fightingStyle.Name}", FightingStyleFeatsNamespace)
            .SetFeatures(
                FeatureDefinitionProficiencyBuilder
                    .Create($"ProficiencyFeat{fightingStyle.Name}", FightingStyleFeatsNamespace)
                    .SetProficiencies(RuleDefinitions.ProficiencyType.FightingStyle, fightingStyle.Name)
                    .SetGuiPresentation(fightingStyle.GuiPresentation)
                    .AddToDB()
            )
            .SetValidators((_, hero) =>
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
