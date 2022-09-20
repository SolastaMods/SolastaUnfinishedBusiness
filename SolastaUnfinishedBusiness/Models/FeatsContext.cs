using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.Api.Infrastructure;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.Feats;
using UnityEngine;
using UnityEngine.UI;

namespace SolastaUnfinishedBusiness.Models;

internal static class FeatsContext
{
    private const int Columns = 3;
    private const int Width = 300;
    private const int Height = 44;
    private const int Spacing = 5;

    internal static HashSet<FeatDefinition> Feats { get; private set; } = new();
    internal static HashSet<FeatDefinition> FeatGroups { get; private set; } = new();

    internal static void LateLoad()
    {
        var feats = new List<FeatDefinition>();

        // Generate feats here and fill the list
        ArmorFeats.CreateArmorFeats(feats);
        CasterFeats.CreateFeats(feats);
        FightingStyleFeats.CreateFeats(feats);
        OtherFeats.CreateFeats(feats);
        HealingFeats.CreateFeats(feats);
        PickPocketContext.CreateFeats(feats);
        CraftyFeats.CreateFeats(feats);
        ElAntoniousFeats.CreateFeats(feats);
        ZappaFeats.CreateFeats(feats);
        EwFeats.CreateFeats(feats);

        feats.ForEach(LoadFeat);

        // create groups for some feats, needs to be last one to be called
        var groups = new List<FeatDefinition>();

        GroupFeats.CreateFeats(groups);

        groups.ForEach(LoadFeatGroup);

        // sorting
        Feats = Feats.OrderBy(x => x.FormatTitle()).ToHashSet();
        FeatGroups = FeatGroups.OrderBy(x => x.FormatTitle()).ToHashSet();
    }

    private static void LoadFeat([NotNull] FeatDefinition featDefinition)
    {
        if (!Feats.Contains(featDefinition))
        {
            Feats.Add(featDefinition);
        }

        UpdateFeatsVisibility(featDefinition);
    }

    private static void LoadFeatGroup([NotNull] FeatDefinition featDefinition)
    {
        if (!FeatGroups.Contains(featDefinition))
        {
            FeatGroups.Add(featDefinition);
        }

        UpdateFeatGroupsVisibility(featDefinition);
    }

    private static void UpdateFeatsVisibility([NotNull] BaseDefinition featDefinition)
    {
        featDefinition.GuiPresentation.hidden = !Main.Settings.FeatEnabled.Contains(featDefinition.Name);
    }

    private static void UpdateFeatGroupsVisibility([NotNull] BaseDefinition featDefinition)
    {
        featDefinition.GuiPresentation.hidden = !Main.Settings.FeatGroupEnabled.Contains(featDefinition.Name);
    }

    internal static void SwitchFeat(FeatDefinition featDefinition, bool active)
    {
        if (!Feats.Contains(featDefinition))
        {
            return;
        }

        var name = featDefinition.Name;

        if (active)
        {
            Main.Settings.FeatEnabled.TryAdd(name);
        }
        else
        {
            Main.Settings.FeatEnabled.Remove(name);
        }

        UpdateFeatsVisibility(featDefinition);
        GuiWrapperContext.RecacheFeats();
    }

    internal static void SwitchFeatGroup(FeatDefinition featDefinition, bool active)
    {
        if (!FeatGroups.Contains(featDefinition))
        {
            return;
        }

        var name = featDefinition.Name;

        if (active)
        {
            Main.Settings.FeatGroupEnabled.TryAdd(name);
        }
        else
        {
            Main.Settings.FeatGroupEnabled.Remove(name);
        }

        UpdateFeatGroupsVisibility(featDefinition);
        GuiWrapperContext.RecacheFeats();
    }

    public static void UpdatePanelChildren(FeatSubPanel panel)
    {
        // get missing children from pool
        while (panel.table.childCount < panel.relevantFeats.Count)
        {
            Gui.GetPrefabFromPool(panel.itemPrefab, panel.table);
        }

        // release extra children to pool
        while (panel.table.childCount > panel.relevantFeats.Count)
        {
            Gui.ReleaseInstanceToPool(panel.table.GetChild(panel.table.childCount - 1).gameObject);
        }
    }

    // called before sorting feats to hide sub-feats during level up
    private static void ProcessFeatGroups(FeatSubPanel panel, bool active, RectTransform table)
    {
        //this is not feat learning - skip manipulations
        if (!active)
        {
            return;
        }

        var toRemove = new List<FeatDefinition>();
        foreach (var group in panel.relevantFeats
                     .Select(feat => feat.GetFirstSubFeatureOfType<IGroupedFeat>())
                     .Where(group => group is {HideSubFeats: true}))
        {
            toRemove.AddRange(group.GetSubFeats());
        }

        for (var i = 0; i < table.childCount; i++)
        {
            var child = table.GetChild(i);
            var featItem = child.GetComponent<FeatItem>();
            if (toRemove.Contains(featItem.GuiFeatDefinition.FeatDefinition))
            {
                child.gameObject.SetActive(false);
            }
        }
    }

    public static void SortFeats(FeatSubPanel panel)
    {
        if (Main.Settings.EnableSortingFeats)
        {
            panel.relevantFeats.Sort(CompareFeats);
        }
    }

    public static int CompareFeats(FeatDefinition a, FeatDefinition b)
    {
        return string.Compare(a.FormatTitle(), b.FormatTitle(),
            StringComparison.CurrentCultureIgnoreCase);
    }

    public static void UpdateRelevantFeatList(FeatSubPanel panel)
    {
        var dbFeatDefinition = DatabaseRepository.GetDatabase<FeatDefinition>();

        var visibleFeats = dbFeatDefinition.Where(x => !x.GuiPresentation.Hidden).ToList();
        panel.relevantFeats.SetRange(visibleFeats.Where(f =>
            f.GetFirstSubFeatureOfType<IGroupedFeat>() is not { } group
            || group.GetSubFeats().Count(s => visibleFeats.Contains(s)) > 1)
        );
    }

    public static void ForceSameWidth(RectTransform table, bool active, FeatSubPanel panel)
    {
        ProcessFeatGroups(panel, active, table);

        if (active && Main.Settings.EnableSameWidthFeatSelection)
        {
            var hero = Global.ActiveLevelUpHero;
            var buildingData = hero?.GetHeroBuildingData();

            if (buildingData == null)
            {
                return;
            }

            var trainedFeats = buildingData.LevelupTrainedFeats.SelectMany(x => x.Value).ToList();

            trainedFeats.AddRange(hero.TrainedFeats);

            var j = 0;
            var rect = table.GetComponent<RectTransform>();

            rect.sizeDelta = new Vector2(rect.sizeDelta.x, ((table.childCount / Columns) + 1) * (Height + Spacing));

            for (var i = 0; i < table.childCount; i++)
            {
                var child = table.GetChild(i);
                var featItem = child.GetComponent<FeatItem>();

                if (!child.gameObject.activeSelf || trainedFeats.Contains(featItem.GuiFeatDefinition.FeatDefinition))
                {
                    continue;
                }

                var x = j % Columns;
                var y = j / Columns;
                var posX = x * (Width + (Spacing * 2));
                var posY = -y * (Height + Spacing);

                rect = child.GetComponent<RectTransform>();
                rect.anchoredPosition = new Vector2(posX, posY);
                rect.sizeDelta = new Vector2(Width, Height);

                j++;
            }
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(table);
    }
}