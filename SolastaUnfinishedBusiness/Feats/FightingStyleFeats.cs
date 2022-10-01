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
        var fightingStyles = DatabaseRepository
            .GetDatabase<FightingStyleDefinition>()
            .Select(BuildFightingStyleFeat)
            .ToList();

        GroupFeats.MakeGroup("FeatGroupFightingStyle", null, fightingStyles);
        feats.AddRange(fightingStyles);
    }

    private static FeatDefinition BuildFightingStyleFeat([NotNull] BaseDefinition fightingStyle)
    {
        return FeatDefinitionBuilder<FeatDefinitionWithPrerequisites, FeatDefinitionWithPrerequisitesBuilder>
            .Create($"Feat{fightingStyle.Name}")
            .SetFeatures(
                FeatureDefinitionProficiencyBuilder
                    .Create($"ProficiencyFeat{fightingStyle.Name}")
                    .SetProficiencies(RuleDefinitions.ProficiencyType.FightingStyle, fightingStyle.Name)
                    .SetGuiPresentation(fightingStyle.GuiPresentation)
                    .AddToDB()
            )
            .SetValidators((_, hero) =>
            {
                var hasFightingStyle = hero.TrainedFightingStyles
                    .Any(x => x.Name == fightingStyle.Name);

                var guiFormat = Gui.Format("Tooltip/&FeatPrerequisiteDoesNotHaveFightingStyle",
                    fightingStyle.FormatTitle());

                return hasFightingStyle ? (false, Gui.Colorize(guiFormat, "EA7171")) : (true, guiFormat);
            })
            .SetGuiPresentation(fightingStyle.GuiPresentation)
            .AddToDB();
    }
}
