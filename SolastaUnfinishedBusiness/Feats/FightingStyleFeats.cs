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
    private const string FightingStyle = "FightingStyle";

    internal static void CreateFeats(List<FeatDefinition> feats)
    {
        var fightingStyles = DatabaseRepository
            .GetDatabase<FightingStyleDefinition>()
            .Select(BuildFightingStyleFeat)
            .ToList();

        // these 3 fighting styles got demoted from mod but builder was kept for backward compatibility
        // hack to keep backward compatibility on fighting styles and move these feats to better groups
        var monkShieldExpert = fightingStyles.First(x => x.Name == $"Feat{MonkShieldExpert.ShieldExpertName}");
        var polearmExpert = fightingStyles.First(x => x.Name == $"Feat{PolearmExpert.PolearmExpertName}");
        var sentinel = fightingStyles.First(x => x.Name == $"Feat{Sentinel.SentinelName}");

        feats.AddRange([monkShieldExpert, polearmExpert, sentinel]);
        monkShieldExpert.Validators.Clear();
        monkShieldExpert.familyTag = string.Empty;
        monkShieldExpert.hasFamilyTag = false;
        polearmExpert.Validators.Clear();
        polearmExpert.familyTag = string.Empty;
        polearmExpert.hasFamilyTag = false;
        sentinel.Validators.Clear();
        sentinel.familyTag = string.Empty;
        sentinel.hasFamilyTag = false;

        GroupFeats.FeatGroupDefenseCombat.AddFeats(monkShieldExpert);
        GroupFeats.FeatGroupMeleeCombat.AddFeats(polearmExpert);
        GroupFeats.FeatGroupTwoHandedCombat.AddFeats(polearmExpert);
        GroupFeats.FeatGroupSupportCombat.AddFeats(sentinel);

        GroupFeats.MakeGroup("FeatGroupFightingStyle", FightingStyle,
            fightingStyles
                .OfType<FeatDefinition>()
                .Where(x => x != monkShieldExpert && x != polearmExpert && x != sentinel)
                .ToArray());
    }

    private static FeatDefinitionWithPrerequisites BuildFightingStyleFeat([NotNull] BaseDefinition fightingStyle)
    {
        // we need a brand new one to avoid issues with FS getting hidden
        var guiPresentation = new GuiPresentation(fightingStyle.GuiPresentation);

        return FeatDefinitionWithPrerequisitesBuilder
            .Create($"Feat{fightingStyle.Name}")
            .SetGuiPresentation(guiPresentation)
            .SetFeatures(
                FeatureDefinitionProficiencyBuilder
                    .Create($"ProficiencyFeat{fightingStyle.Name}")
                    .SetProficiencies(ProficiencyType.FightingStyle, fightingStyle.Name)
                    .SetGuiPresentation(guiPresentation)
                    .AddToDB())
            .SetFeatFamily(FightingStyle)
            .SetValidators(ValidatorsFeat.ValidateNotFightingStyle(fightingStyle))
            .AddToDB();
    }
}
