using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.Feats;
using SolastaUnfinishedBusiness.FightingStyles;

namespace SolastaUnfinishedBusiness.Models;

internal static class FightingStyleContext
{
    internal static readonly List<string> DemotedFightingStyles =
    [
        "Merciless",
        "PolearmExpert",
        "RopeItUp",
        "Sentinel",
        "ShieldExpert"
    ];

    private static Dictionary<FightingStyleDefinition, List<FeatureDefinitionFightingStyleChoice>>
        FightingStylesChoiceList { get; } = [];

    internal static HashSet<FightingStyleDefinition> FightingStyles { get; private set; } = [];

    internal static void Load()
    {
        // load FS
        KeepDemotedFightingStylesBackwardCompatibility();

        LoadStyle(new AstralReach());
        LoadStyle(new BlessedWarrior());
        LoadStyle(new BlindFighting());
        LoadStyle(new Crippling());
        LoadStyle(new DruidicWarrior());
        LoadStyle(new Executioner());
        LoadStyle(new HandAndAHalf());
        LoadStyle(new Interception());
        LoadStyle(new Lunger());
        LoadStyle(new RemarkableTechnique());
        LoadStyle(new Torchbearer());

        // sorting
        FightingStyles = [.. FightingStyles.OrderBy(x => x.FormatTitle())];

        // settings paring
        foreach (var name in Main.Settings.FightingStyleEnabled
                     .Where(name => FightingStyles.All(x => x.Name != name))
                     .ToArray())
        {
            Main.Settings.FightingStyleEnabled.Remove(name);
        }
    }

    private static void KeepDemotedFightingStylesBackwardCompatibility()
    {
        foreach (var name in DemotedFightingStyles)
        {
            _ = FightingStyleBuilder
                .Create(name)
                .SetGuiPresentation(Category.FightingStyle, hidden: true)
                .SetFeatures(FeatureDefinitionProficiencyBuilder
                    .Create($"ProficiencyFeat{name}")
                    .SetGuiPresentationNoContent(true)
                    .SetProficiencies(RuleDefinitions.ProficiencyType.Feat, $"Feat{name}")
                    .AddToDB())
                .AddToDB();
        }
    }

    private static void LoadStyle([NotNull] AbstractFightingStyle styleBuilder)
    {
        var style = styleBuilder.FightingStyle;

        if (!FightingStyles.Contains(style))
        {
            FightingStylesChoiceList.TryAdd(style, styleBuilder.FightingStyleChoice);
            FightingStyles.Add(style);
        }

        UpdateStyleVisibility(style);
    }

    private static void UpdateStyleVisibility([NotNull] FightingStyleDefinition fightingStyleDefinition)
    {
        var name = fightingStyleDefinition.Name;
        var choiceLists = FightingStylesChoiceList[fightingStyleDefinition];

        foreach (var fightingStyles in choiceLists.Select(cl => cl.FightingStyles))
        {
            if (Main.Settings.FightingStyleEnabled.Contains(name))
            {
                fightingStyles.TryAdd(name);
            }
            else
            {
                fightingStyles.Remove(name);
            }
        }
    }

    internal static void Switch(FightingStyleDefinition fightingStyleDefinition, bool active)
    {
        if (!FightingStyles.Contains(fightingStyleDefinition))
        {
            return;
        }

        var name = fightingStyleDefinition.Name;
        // Druidic and Paladin FS don't have a feat
        var hasFeat = DatabaseRepository.GetDatabase<FeatDefinition>().TryGetElement($"Feat{name}", out var feat);

        if (active)
        {
            Main.Settings.FightingStyleEnabled.TryAdd(name);

            if (hasFeat)
            {
                GroupFeats.FeatGroupFightingStyle.AddFeats(feat);
            }
        }
        else
        {
            Main.Settings.FightingStyleEnabled.Remove(name);

            if (hasFeat)
            {
                GroupFeats.FeatGroupFightingStyle.RemoveFeats(feat);
            }
        }

        if (hasFeat)
        {
            feat.GuiPresentation.hidden = !active;
        }

        GuiWrapperContext.RecacheFeats();
        UpdateStyleVisibility(fightingStyleDefinition);
    }

    internal static void RefreshFightingStylesPatch(RulesetCharacterHero hero)
    {
        foreach (var trainedFightingStyle in hero.trainedFightingStyles
                     .Where(x =>
                         // activate all modded fighting styles by default
                         x.contentPack == CeContentPackContext.CeContentPack ||
                         // handles this in a different place [AddCustomWeaponValidatorToFightingStyleArchery()] so always allow here
                         x.Condition == FightingStyleDefinition.TriggerCondition.RangedWeaponAttack))
        {
            hero.activeFightingStyles.TryAdd(trainedFightingStyle);
        }
    }
}
