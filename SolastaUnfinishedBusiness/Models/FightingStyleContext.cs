using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Feats;
using SolastaUnfinishedBusiness.FightingStyles;

namespace SolastaUnfinishedBusiness.Models;

internal static class FightingStyleContext
{
    internal static readonly HashSet<string> DemotedFightingStyles =
    [
        Merciless.MercilessName,
        MonkShieldExpert.ShieldExpertName,
        PolearmExpert.PolearmExpertName,
        RopeItUp.RopeItUpName,
        Sentinel.SentinelName,
        ShieldExpert.ShieldExpertName
    ];

    private static Dictionary<FightingStyleDefinition, List<FeatureDefinitionFightingStyleChoice>>
        FightingStylesChoiceList { get; } = [];

    internal static HashSet<FightingStyleDefinition> FightingStyles { get; private set; } = [];

    internal static void Load()
    {
        // kept for backward compatibility
        _ = new Merciless();
        _ = new MonkShieldExpert();
        _ = new PolearmExpert();
        _ = new RopeItUp();
        _ = new ShieldExpert();
        _ = new Sentinel();

        LoadStyle(new AstralReach());
        LoadStyle(new BlindFighting());
        LoadStyle(new Crippling());
        LoadStyle(new Executioner());
        LoadStyle(new HandAndAHalf());
        LoadStyle(new Interception());
        LoadStyle(new Lunger());
        LoadStyle(new Pugilist());
        LoadStyle(new RemarkableTechnique());
        LoadStyle(new Torchbearer());

        // sorting
        FightingStyles = [.. FightingStyles.OrderBy(x => x.FormatTitle())];

        // settings paring
        foreach (var name in Main.Settings.FightingStyleEnabled
                     .Where(name => FightingStyles.All(x => x.Name != name))
                     .ToList())
        {
            Main.Settings.FightingStyleEnabled.Remove(name);
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
        var feat = DatabaseRepository.GetDatabase<FeatDefinition>().GetElement($"Feat{name}");

        if (active)
        {
            Main.Settings.FightingStyleEnabled.TryAdd(name);
            GroupFeats.FeatGroupFightingStyle.AddFeats(feat);
        }
        else
        {
            Main.Settings.FightingStyleEnabled.Remove(name);
            GroupFeats.FeatGroupFightingStyle.RemoveFeats(feat);
        }

        feat.GuiPresentation.hidden = !active;
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
