using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.Api.Infrastructure;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.Feats;
using UnityEngine;
using UnityEngine.UI;

namespace SolastaUnfinishedBusiness.Models;

internal static class FeatsContext
{
    internal static HashSet<FeatDefinition> Feats { get; private set; } = new();
    
    private const int COLUMNS = 3;
    public const int WIDTH = 300;
    public const int HEIGHT = 44;
    public const int SPACING = 5;

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

        //Creates groups for some feats, needs to be last one to be called
        FeatGroups.CreateFeats(feats);

        feats.ForEach(LoadFeat);

        Feats = Feats.OrderBy(x => x.FormatTitle()).ToHashSet();
    }

    private static void LoadFeat([NotNull] FeatDefinition featDefinition)
    {
        if (!Feats.Contains(featDefinition))
        {
            Feats.Add(featDefinition);
        }

        UpdateFeatsVisibility(featDefinition);
    }

    private static void UpdateFeatsVisibility([NotNull] BaseDefinition featDefinition)
    {
        featDefinition.GuiPresentation.hidden = !Main.Settings.FeatEnabled.Contains(featDefinition.Name);
    }

    internal static void Switch(FeatDefinition featDefinition, bool active)
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

    public static void UpdatePanelChildren(FeatSubPanel panel)
    {
        //get missing children from pool
        while (panel.table.childCount < panel.relevantFeats.Count)
        {
            Gui.GetPrefabFromPool(panel.itemPrefab, panel.table);
        }

        //release extra children to pool
        while (panel.table.childCount > panel.relevantFeats.Count)
        {
            Gui.ReleaseInstanceToPool(panel.table.GetChild(panel.table.childCount - 1).gameObject);
        }
    }

    //Called before Bind to hide sub-feats
    public static void ProcessFeatGroups(FeatSubPanel panel)
    {
        var toRemove = new List<FeatDefinition>();
        foreach (var group in panel.relevantFeats
                     .Select(feat => feat.GetFirstSubFeatureOfType<IGroupedFeat>())
                     .Where(group => group != null))
        {
            toRemove.AddRange(group.GetSubFeats());
        }

        panel.relevantFeats.RemoveAll(f => toRemove.Contains(f));
    }

    public static void SortFeats(FeatSubPanel panel)
    {
        if (Main.Settings.EnableSortingFeats)
        {
            panel.relevantFeats.Sort(CompareFeats);
        }
    }

    public static int CompareFeats(FeatDefinition a, FeatDefinition b) => string.Compare(a.FormatTitle(), b.FormatTitle(),
        StringComparison.CurrentCultureIgnoreCase);

    public static void UpdateRelevantFeatList(FeatSubPanel panel)
    {
        var dbFeatDefinition = DatabaseRepository.GetDatabase<FeatDefinition>();

        panel.relevantFeats.SetRange(dbFeatDefinition.Where(x => !x.GuiPresentation.Hidden));
    }

    public static void ForceSameWidth(RectTransform table, bool active)
    {
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

            rect.sizeDelta = new Vector2(rect.sizeDelta.x, ((table.childCount / COLUMNS) + 1) * (HEIGHT + SPACING));

            for (var i = 0; i < table.childCount; i++)
            {
                var child = table.GetChild(i);
                var featItem = child.GetComponent<FeatItem>();

                if (trainedFeats.Contains(featItem.GuiFeatDefinition.FeatDefinition))
                {
                    continue;
                }

                var x = j % COLUMNS;
                var y = j / COLUMNS;
                var posX = x * (WIDTH + (SPACING * 2));
                var posY = -y * (HEIGHT + SPACING);

                rect = child.GetComponent<RectTransform>();
                rect.anchoredPosition = new Vector2(posX, posY);
                rect.sizeDelta = new Vector2(WIDTH, HEIGHT);

                j++;
            }
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(table);
    }
}
