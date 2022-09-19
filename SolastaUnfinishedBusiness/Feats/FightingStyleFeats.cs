using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomDefinitions;

namespace SolastaUnfinishedBusiness.Feats;

internal static class FightingStyleFeats
{
    public static void CreateFeats(List<FeatDefinition> feats)
    {
        var styles = DatabaseRepository
            .GetDatabase<FightingStyleDefinition>()
            .Select(BuildFightingStyleFeat)
            .ToList();
        
        FeatGroups.MakeGroup("FeatGroupFightingStyle", styles);
        feats.AddRange(styles);
    }

    private static FeatDefinition BuildFightingStyleFeat([NotNull] BaseDefinition fightingStyle)
    {
        return FeatDefinitionBuilder<FeatDefinitionWithPrerequisites, FeatDefinitionWithPrerequisitesBuilder>
            .Create($"Feat{fightingStyle.Name}", DefinitionBuilder.CENamespaceGuid)
            .SetFeatures(
                FeatureDefinitionProficiencyBuilder
                    .Create($"ProficiencyFeat{fightingStyle.Name}", DefinitionBuilder.CENamespaceGuid)
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
            .SetGuiPresentation(fightingStyle.GuiPresentation)
            .AddToDB();
    }
}