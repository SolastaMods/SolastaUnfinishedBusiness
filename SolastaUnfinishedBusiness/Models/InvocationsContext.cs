using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Infrastructure;
using UnityEngine;
using UnityEngine.UI;
using static SolastaUnfinishedBusiness.Invocations.InvocationsBuilders;

namespace SolastaUnfinishedBusiness.Models;

internal static class InvocationsContext
{
    private const int Columns = 3;
    private const int Width = 300;
    private const int Height = 44;
    private const int Spacing = 5;

    internal static HashSet<InvocationDefinition> Invocations { get; private set; } = new();

    internal static void LateLoad()
    {
        LoadInvocation(BuildAspectOfTheMoon());
        LoadInvocation(BuildBondOfTheTalisman());
        LoadInvocation(BuildEldritchMind());
        LoadInvocation(BuildEldritchSmite());
        LoadInvocation(BuildGiftOfTheEverLivingOnes());
        LoadInvocation(BuildGiftOfTheProtectors());
        LoadInvocation(BuildGraspingBlast());
        LoadInvocation(BuildImprovedPactWeapon());
        LoadInvocation(BuildMinionsOfChaos());
        LoadInvocation(BuildShroudOfShadow());
        LoadInvocation(BuildSuperiorPactWeapon());
        LoadInvocation(BuildTrickstersEscape());
        LoadInvocation(BuildUltimatePactWeapon());
        LoadInvocation(BuildUndyingServitude());

        // sorting
        Invocations = Invocations.OrderBy(x => x.FormatTitle()).ToHashSet();

        // settings paring
        foreach (var name in Main.Settings.InvocationEnabled
                     .Where(name => Invocations.All(x => x.Name != name))
                     .ToList())
        {
            Main.Settings.InvocationEnabled.Remove(name);
        }

        // avoids restart on level up UI
        GuiWrapperContext.RecacheInvocations();
    }

    private static void LoadInvocation([NotNull] InvocationDefinition invocationDefinition)
    {
        if (!Invocations.Contains(invocationDefinition))
        {
            Invocations.Add(invocationDefinition);
        }

        UpdateInvocationVisibility(invocationDefinition);
    }

    private static void UpdateInvocationVisibility([NotNull] BaseDefinition invocationDefinition)
    {
        invocationDefinition.GuiPresentation.hidden =
            !Main.Settings.InvocationEnabled.Contains(invocationDefinition.Name);
    }

    internal static void SwitchInvocation(InvocationDefinition invocationDefinition, bool active)
    {
        if (!Invocations.Contains(invocationDefinition))
        {
            return;
        }

        var name = invocationDefinition.Name;

        if (active)
        {
            Main.Settings.InvocationEnabled.TryAdd(name);
        }
        else
        {
            Main.Settings.InvocationEnabled.Remove(name);
        }

        UpdateInvocationVisibility(invocationDefinition);
        GuiWrapperContext.RecacheInvocations();
    }

    internal static void UpdateRelevantInvocationList(InvocationSubPanel panel)
    {
        var dbInvocationDefinition = DatabaseRepository.GetDatabase<InvocationDefinition>();
        var visibleInvocations = dbInvocationDefinition
            .Where(x => !x.GuiPresentation.Hidden)
            .ToList();

        panel.relevantInvocations.SetRange(visibleInvocations);
    }

    private static int CompareInvocations(InvocationDefinition a, InvocationDefinition b)
    {
        return string.Compare(a.FormatTitle(), b.FormatTitle(),
            StringComparison.CurrentCultureIgnoreCase);
    }

    internal static void SortInvocations(InvocationSubPanel panel)
    {
        panel.relevantInvocations.Sort(CompareInvocations);
    }

    internal static void ForceSameWidth(RectTransform table, bool active, InvocationSubPanel panel)
    {
        if (active && Main.Settings.EnableSameWidthInvocationSelection)
        {
            var hero = Global.LevelUpHero;
            var buildingData = hero?.GetHeroBuildingData();

            if (buildingData == null)
            {
                return;
            }

            var trainedInvocations = buildingData.LevelupTrainedInvocations.SelectMany(x => x.Value).ToList();

            trainedInvocations.AddRange(hero.TrainedInvocations);

            var j = 0;
            RectTransform rect;

            for (var i = 0; i < table.childCount; i++)
            {
                var child = table.GetChild(i);
                var invocationItem = child.GetComponent<InvocationItem>();

                if (!child.gameObject.activeSelf ||
                    trainedInvocations.Contains(invocationItem.GuiInvocationDefinition.InvocationDefinition))
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

            rect = table.GetComponent<RectTransform>();
            rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, ((j / Columns) + 1) * (Height + Spacing));
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(table);
    }
}
