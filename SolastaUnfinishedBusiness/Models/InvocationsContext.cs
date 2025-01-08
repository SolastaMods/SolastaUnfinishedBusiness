using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;
using static SolastaUnfinishedBusiness.Subclasses.Builders.InvocationsBuilders;

namespace SolastaUnfinishedBusiness.Models;

internal static class InvocationsContext
{
    private const int Columns = 4;
    private const int Width = 230;
    private const int Height = 44;
    private const int SpacingX = 7;
    private const int SpacingY = 5;

    internal static HashSet<InvocationDefinition> Invocations { get; private set; } = [];

    internal static void LateLoad()
    {
        LoadInvocation(BuildBreathOfTheNight());
        LoadInvocation(BuildBurningHex());
        LoadInvocation(BuildChillingBlast());
        LoadInvocation(BuildChillingHex());
        LoadInvocation(BuildCorrosiveBlast());
        LoadInvocation(BuildFieryBlast());
        LoadInvocation(BuildFulminateBlast());
        LoadInvocation(BuildNecroticBlast());
        LoadInvocation(BuildPsychicBlast());
        LoadInvocation(BuildRadiantBlast());
        LoadInvocation(BuildThunderBlast());
        LoadInvocation(BuildAbilitiesOfTheChainMaster());
        LoadInvocation(BuildAspectOfTheMoon());
        LoadInvocation(BuildBondOfTheTalisman());
        LoadInvocation(BuildBreakerAndBanisher());
        LoadInvocation(BuildCallOfTheBeast());
        LoadInvocation(BuildDevouringBlade());
        LoadInvocation(BuildDiscerningGaze());
        LoadInvocation(EldritchMind);
        LoadInvocation(BuildEldritchSmite());
        LoadInvocation(BuildGiftOfTheEverLivingOnes());
        LoadInvocation(BuildGiftOfTheHunter());
        LoadInvocation(BuildGiftOfTheProtectors());
        LoadInvocation(GraspingBlast);
        LoadInvocation(BuildHinderingBlast());
        LoadInvocation(BuildImprovedPactWeapon());
        LoadInvocation(BuildInexorableHex());
        LoadInvocation(BuildKinesis());
        LoadInvocation(BuildPerniciousCloak());
        LoadInvocation(BuildPoisonousBlast());
        LoadInvocation(BuildShroudOfShadow());
        LoadInvocation(BuildSpectralShield());
        LoadInvocation(BuildStasis());
        LoadInvocation(BuildSuperiorPactWeapon());
        LoadInvocation(BuildTenaciousPlague());
        LoadInvocation(BuildTombOfFrost());
        LoadInvocation(BuildTrickstersEscape());
        LoadInvocation(BuildUltimatePactWeapon());
        LoadInvocation(BuildUndyingServitude());
        LoadInvocation(BuildVerdantArmor());
        LoadInvocation(BuildVexingHex());

        // sorting
        Invocations = Invocations.OrderBy(x => x.FormatTitle()).ToHashSet();

        // settings paring
        foreach (var name in Main.Settings.InvocationEnabled
                     .Where(name => Invocations.All(x => x.Name != name))
                     .ToArray())
        {
            Main.Settings.InvocationEnabled.Remove(name);
        }

        // avoids restart on level up UI
        GuiWrapperContext.RecacheInvocations();
    }

    private static void LoadInvocation([NotNull] InvocationDefinition invocationDefinition)
    {
        Invocations.Add(invocationDefinition);
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

    private static int CompareInvocations(InvocationDefinition a, InvocationDefinition b)
    {
        var compare = Math.Max(a.RequiredLevel, 1) - Math.Max(b.RequiredLevel, 1);

        return compare == 0
            ? string.Compare(a.FormatTitle(), b.FormatTitle(), StringComparison.CurrentCultureIgnoreCase)
            : compare;
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

            var j = 0;
            var level = 1;
            var gap = 0;
            RectTransform rect;

            for (var i = 0; i < table.childCount; i++)
            {
                var child = table.GetChild(i);

                if (!child.gameObject.activeSelf)
                {
                    continue;
                }

                var requiredLevel = child.GetComponent<InvocationItem>()
                    .GuiInvocationDefinition
                    .InvocationDefinition
                    .RequiredLevel;

                if (requiredLevel < 1)
                {
                    requiredLevel = 1;
                }

                if (requiredLevel != level)
                {
                    var mod = j % Columns;

                    if (mod != 0)
                    {
                        j += Columns - mod;
                    }

                    level = requiredLevel;
                    gap += 2 * SpacingY;
                }

                var x = j % Columns;
                var y = j / Columns;
                var posX = x * (Width + SpacingX);
                var posY = (-y * (Height + SpacingY)) - gap;

                rect = child.GetComponent<RectTransform>();
                rect.anchoredPosition = new Vector2(posX, posY);
                rect.sizeDelta = new Vector2(Width, Height);

                j++;
            }

            rect = table.GetComponent<RectTransform>();
            rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,
                ((int)Math.Ceiling(j / (float)Columns) * (Height + SpacingY)) + gap);
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(table);
    }
}
