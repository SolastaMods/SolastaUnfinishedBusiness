using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.FightingStyles;

namespace SolastaUnfinishedBusiness.Models;

internal static class FightingStyleContext
{
    private static Dictionary<FightingStyleDefinition, List<FeatureDefinitionFightingStyleChoice>>
        FightingStylesChoiceList { get; } = new();

    internal static HashSet<FightingStyleDefinition> FightingStyles { get; private set; } = new();

    internal static void Load()
    {
        LoadStyle(new BlindFighting());
        LoadStyle(new Crippling());
        LoadStyle(new Executioner());
        LoadStyle(new HandAndAHalf());
        LoadStyle(new Merciless());
        LoadStyle(new PolearmExpert());
        LoadStyle(new Pugilist());
        LoadStyle(new Sentinel());
        LoadStyle(new ShieldExpert());
        LoadStyle(new Torchbearer());

        // sorting
        FightingStyles = FightingStyles.OrderBy(x => x.FormatTitle()).ToHashSet();

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
            FightingStylesChoiceList.Add(style, styleBuilder.FightingStyleChoice);
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

        if (active)
        {
            Main.Settings.FightingStyleEnabled.TryAdd(name);
        }
        else
        {
            Main.Settings.FightingStyleEnabled.Remove(name);
        }

        UpdateStyleVisibility(fightingStyleDefinition);
    }

    internal static void RefreshFightingStylesPatch(RulesetCharacterHero hero)
    {
        foreach (var trainedFightingStyle in hero.trainedFightingStyles)
        {
            var isActive = trainedFightingStyle.contentPack == CeContentPackContext.CeContentPack;

            // activate all modded fighting styles by default
            if (isActive)
            {
                hero.activeFightingStyles.TryAdd(trainedFightingStyle);

                continue;
            }

            // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
            switch (trainedFightingStyle.Condition)
            {
                // handles this in a different place [AddCustomWeaponValidatorToFightingStyleArchery()] so always allow here
                case FightingStyleDefinition.TriggerCondition.RangedWeaponAttack:
                {
                    isActive = true;

                    break;
                }

                // allow Shield Expert benefit from Two Weapon Fighting Style
                case FightingStyleDefinition.TriggerCondition.TwoMeleeWeaponsWielded:
                {
                    var mainHandSlot = hero.GetMainWeapon();
                    var offHandSlot = hero.GetOffhandWeapon();
                    var hasShieldExpert =
                        hero.TrainedFeats.Any(x => x.Name.Contains(ShieldExpert.ShieldExpertName)) ||
                        hero.TrainedFightingStyles.Any(x => x.Name.Contains(ShieldExpert.ShieldExpertName));

                    isActive = hasShieldExpert &&
                               mainHandSlot != null && mainHandSlot.ItemDefinition.IsWeapon &&
                               offHandSlot != null && offHandSlot.ItemDefinition.IsArmor;

                    break;
                }
            }

            if (isActive)
            {
                hero.activeFightingStyles.TryAdd(trainedFightingStyle);
            }
        }
    }
}
