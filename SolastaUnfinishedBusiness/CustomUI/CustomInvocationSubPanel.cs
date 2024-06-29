using System;
using System.Collections.Generic;
using System.Linq;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Feats;
using SolastaUnfinishedBusiness.Models;
using UnityEngine;

namespace SolastaUnfinishedBusiness.CustomUI;

public class CustomInvocationSubPanel : MonoBehaviour
{
    private InvocationPoolTypeCustom Type { get; set; }

    private void Setup(InvocationPoolTypeCustom type)
    {
        Type = type;
        name = $"CustomSubPanel<{type.Name}>";

        var title = transform.Find("ProficiencySectionHeader/Title");

        if (title)
        {
            title.GetComponent<GuiLabel>().Text = type.PanelTitle;
        }
    }

    internal static List<string> CustomInvocationsProficiencies(RulesetCharacterHero hero, InvocationSubPanel panel)
    {
        if (panel.TryGetComponent<CustomInvocationSubPanel>(out var custom))
        {
            var customInvocations = DatabaseRepository.GetDatabase<InvocationDefinition>()
                .OfType<InvocationDefinitionCustom>()
                .Where(i => i.PoolType == custom.Type)
                .Select(i => i.Name);

            return hero.invocationProficiencies
                .Intersect(customInvocations)
                .ToList();
        }
        else
        {
            var customInvocations = DatabaseRepository.GetDatabase<InvocationDefinition>()
                .OfType<InvocationDefinitionCustom>()
                .Select(i => i.Name);

            return hero.invocationProficiencies
                .Except(customInvocations)
                .ToList();
        }
    }

    internal static List<string> OnlyStandardInvocationProficiencies(RulesetCharacterHero hero)
    {
        var selectedClass = LevelUpContext.GetSelectedClass(hero);

        if (selectedClass != DatabaseHelper.CharacterClassDefinitions.Warlock
            && !hero.TrainedFeats.Exists(x => x.Name == OtherFeats.FeatEldritchAdept))
        {
            return [];
        }

        var customInvocations = DatabaseRepository.GetDatabase<InvocationDefinition>()
            .OfType<InvocationDefinitionCustom>()
            .Select(i => i.Name);

        return hero.invocationProficiencies
            .Except(customInvocations)
            .ToList();
    }

    public static void UpdateRelevantInvocations(InvocationSubPanel panel)
    {
        var all = DatabaseRepository.GetDatabase<InvocationDefinition>()
            .Where(x => !x.GuiPresentation.Hidden);

        IEnumerable<InvocationDefinition> invocations;

        var custom = panel.GetComponent<CustomInvocationSubPanel>();

        if (custom)
        {
            invocations = all.OfType<InvocationDefinitionCustom>()
                .Where(x => x.PoolType == custom.Type);
        }
        else
        {
            invocations = all.Where(x => x is not InvocationDefinitionCustom);
        }

        var table = panel.Table;
        var relevantInvocations = panel.relevantInvocations;

        relevantInvocations.SetRange(invocations);

        // get missing children from pool
        while (table.childCount < relevantInvocations.Count)
        {
            Gui.GetPrefabFromPool(panel.ItemPrefab, table);
        }

        // release extra children to pool
        while (table.childCount > relevantInvocations.Count)
        {
            Gui.ReleaseInstanceToPool(table.GetChild(table.childCount - 1).gameObject);
        }
    }

    public static void AddCustomSubPanels(ProficienciesPanel panel)
    {
        if (!panel.toggleGroup || !Main.Settings.EnableExtendedProficienciesPanelDisplay)
        {
            return;
        }

        var invocationsSubPanel = panel.subPanels[(int)ProficienciesPanel.ProficiencyType.Invocation];

        var invocationTransform = invocationsSubPanel.transform;
        var root = invocationTransform.parent;
        var siblingIndex = invocationTransform.GetSiblingIndex() + 1;

        var index = panel.subPanels.Length;
        var poolTypes = InvocationPoolTypeCustom.Pools.All.Where(p => !p.Hidden).ToList();

        Array.Resize(ref panel.subPanels, index + poolTypes.Count);

        foreach (var pool in poolTypes)
        {
            var extra = Instantiate(invocationsSubPanel, root);

            extra.gameObject.AddComponent<CustomInvocationSubPanel>().Setup(pool);
            extra.transform.SetSiblingIndex(siblingIndex);
            panel.subPanels[index] = extra;

            var groupPrefab = panel.proficiencyTogglePrefab;
            var group = Gui.GetPrefabFromPool(groupPrefab, panel.toggleGroup);

            group.transform.SetSiblingIndex(siblingIndex);

            var proficiencyToggle = group.GetComponent<ProficiencyToggle>();

            proficiencyToggle.Bind(pool.PanelTitle, false, index, panel.ProficiencyToggleChanged);
            panel.subPanelsMap.Add(proficiencyToggle, extra);

            index++;
            siblingIndex++;
        }
    }
}
