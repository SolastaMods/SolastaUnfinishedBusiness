using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.FightingStyles;
using SolastaUnfinishedBusiness.Validators;
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.Feats;

internal static class FightingStyleFeats
{
    internal static void CreateFeats(List<FeatDefinition> feats)
    {
        var fightingStyles = DatabaseRepository
            .GetDatabase<FightingStyleDefinition>()
            .Select(BuildFightingStyleFeat)
            .ToList();

        // hack to keep backward compatibility on fighting styles and move these feats to better groups
        var monkShieldExpert = fightingStyles.First(x => x.Name == $"Feat{MonkShieldExpert.ShieldExpertName}");
        var polearmExpert = fightingStyles.First(x => x.Name == $"Feat{PolearmExpert.PolearmExpertName}");
        var sentinel = fightingStyles.First(x => x.Name == $"Feat{Sentinel.SentinelName}");

        feats.AddRange(fightingStyles);

        GroupFeats.FeatGroupDefenseCombat.AddFeats(monkShieldExpert);
        GroupFeats.FeatGroupMeleeCombat.AddFeats(polearmExpert);
        GroupFeats.FeatGroupTwoHandedCombat.AddFeats(polearmExpert);
        GroupFeats.FeatGroupSupportCombat.AddFeats(sentinel);
        GroupFeats.FeatGroupFightingStyle.AddFeats(
            fightingStyles
                .Where(x => x != monkShieldExpert && x != polearmExpert && x != sentinel)
                .ToArray());
    }

    private static FeatDefinition BuildFightingStyleFeat([NotNull] BaseDefinition fightingStyle)
    {
        // we need a brand new one to avoid issues with FS getting hidden
        var guiPresentation = new GuiPresentation(fightingStyle.GuiPresentation);

        return FeatDefinitionWithPrerequisitesBuilder
            .Create($"Feat{fightingStyle.Name}")
            .SetFeatures(
                FeatureDefinitionProficiencyBuilder
                    .Create($"ProficiencyFeat{fightingStyle.Name}")
                    .SetProficiencies(ProficiencyType.FightingStyle, fightingStyle.Name)
                    .SetGuiPresentation(guiPresentation)
                    .AddToDB())
            .SetValidators(ValidatorsFeat.ValidateNotFightingStyle(fightingStyle))
            .SetGuiPresentation(guiPresentation)
            .AddToDB();
    }
}
