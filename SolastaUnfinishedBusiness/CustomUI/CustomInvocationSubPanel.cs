using System;
using System.Collections.Generic;
using System.Linq;
using SolastaUnfinishedBusiness.Api.Infrastructure;
using SolastaUnfinishedBusiness.CustomDefinitions;
using UnityEngine;

namespace SolastaUnfinishedBusiness.CustomUI;

public class CustomInvocationSubPanel : MonoBehaviour
{
    public CustomInvocationPoolType Type { get; private set; }

    public void Setup(CustomInvocationPoolType type)
    {
        Type = type;
        name = $"CustomSubPanel<{type.Name}>";

        var title = transform.Find("ProficiencySectionHeader/Title");

        if (title != null)
        {
            title.GetComponent<GuiLabel>().Text = type.PanelTitle;
        }
    }

    public static void UpdateRelevantInvocations(InvocationSubPanel panel)
    {
        var all = DatabaseRepository.GetDatabase<InvocationDefinition>().GetAllElements()
            .Where(x => !x.GuiPresentation.Hidden);

        IEnumerable<InvocationDefinition> invocations;

        var custom = panel.GetComponent<CustomInvocationSubPanel>();

        if (custom != null)
        {
            invocations = all.OfType<CustomInvocationDefinition>()
                .Where(x => x.PoolType == custom.Type);
        }
        else
        {
            invocations = all.Where(x => x is not CustomInvocationDefinition);
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

    public static void AddCustomSubpanels(ProficienciesPanel panel)
    {
        if (panel.toggleGroup == null)
        {
            return;
        }

        var invocationsSubPanel = panel.subPanels[(int)ProficienciesPanel.ProficiencyType.Invocation];

        var invocationTransform = invocationsSubPanel.transform;
        var root = invocationTransform.parent;
        var siblingIndex = invocationTransform.GetSiblingIndex() + 1;

        var index = panel.subPanels.Length;
        var poolTypes = CustomInvocationPoolType.Pools.All;

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
