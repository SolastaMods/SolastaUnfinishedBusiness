using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static GuiDropdown;
using static SolastaCommunityExpansion.Api.DatabaseHelper;
using Object = UnityEngine.Object;

namespace SolastaCommunityExpansion.Models;

internal static class InventoryManagementContext
{
    private static readonly List<string> SortCategories = new()
    {
        "Name",
        "Category",
        "Cost",
        "Weight",
        "Cost per Weight"
    };

    private static readonly List<RulesetItem> FilteredItems = new();

    private static readonly List<MerchantCategoryDefinition> ItemCategories = new();

    private static GuiDropdown FilterGuiDropdown { get; set; }

    private static SortGroup BySortGroup { get; set; }

    private static GuiDropdown SortGuiDropdown { get; set; }

    internal static Action SelectionChanged { get; private set; }

    internal static void Load()
    {
        var characterInspectionScreen = Gui.GuiService.GetScreen<CharacterInspectionScreen>();
        var rightGroup = characterInspectionScreen.transform.FindChildRecursive("RightGroup");
        var containerPanel = rightGroup.GetComponentInChildren<ContainerPanel>();

        var dropdownPrefab = Resources.Load<GameObject>("GUI/Prefabs/Component/Dropdown");
        var sortGroupPrefab = Gui.GuiService.GetScreen<MainMenuScreen>().transform
            .FindChildRecursive("SortGroupAlphabetical");

        var filter = Object.Instantiate(dropdownPrefab, rightGroup);
        var filterRect = filter.GetComponent<RectTransform>();

        FilterGuiDropdown = filter.GetComponent<GuiDropdown>();

        var by = Object.Instantiate(sortGroupPrefab, rightGroup);
        var byTextMesh = by.GetComponentInChildren<TextMeshProUGUI>();

        BySortGroup = by.GetComponent<SortGroup>();

        var sort = Object.Instantiate(dropdownPrefab, rightGroup);
        var sortRect = sort.GetComponent<RectTransform>();

        SortGuiDropdown = sort.GetComponent<GuiDropdown>();

        //
        // on any control change we need to unbind / bind the entire panel to refresh all the additional items gizmos
        //

        SelectionChanged = () =>
        {
            var container = containerPanel.Container;
            var inspectedCharacter = containerPanel.InspectedCharacter;
            var dropAreaClicked = containerPanel.DropAreaClicked;
            var visibleSlotsRefreshed = containerPanel.VisibleSlotsRefreshed;

            containerPanel.Unbind();
            Flush(container);
            SortAndFilter(container);
            containerPanel.Bind(container, inspectedCharacter, dropAreaClicked, visibleSlotsRefreshed);
            containerPanel.RefreshNow();
        };

        // changes the reorder button label and refactor the listener

        var reorder = rightGroup.transform.Find("ReorderPersonalContainerButton");
        var reorderButton = reorder.GetComponent<Button>();
        var reorderTextMesh = reorder.GetComponentInChildren<TextMeshProUGUI>();

        reorder.localPosition = new Vector3(-32f, 358f, 0f);
        reorderTextMesh.text = "Reset";
        reorderButton.onClick.RemoveAllListeners();
        reorderButton.onClick.AddListener(delegate
        {
            if (Main.Settings.EnableInventoryFilteringAndSorting
                && !Global.IsMultiplayer)
            {
                ResetControls();
                SelectionChanged();
            }
            else
            {
                containerPanel.OnReorderCb();
            }
        });

        // creates the categories in alphabetical sort order

        var merchantCategoryDefinitions = DatabaseRepository.GetDatabase<MerchantCategoryDefinition>();
        var filteredCategoryDefinitions = merchantCategoryDefinitions
            .Where(x => x != MerchantCategoryDefinitions.All).OrderBy(x => x.FormatTitle());

        ItemCategories.Add(MerchantCategoryDefinitions.All);
        ItemCategories.AddRange(filteredCategoryDefinitions);

        // adds the filter dropdown

        var filterOptions = new List<TMP_Dropdown.OptionData>();

        filter.name = "FilterDropdown";
        filter.transform.localPosition = new Vector3(-422f, 370f, 0f);

        filterRect.sizeDelta = new Vector2(150f, 28f);

        FilterGuiDropdown.ClearOptions();
        FilterGuiDropdown.onValueChanged.AddListener(delegate { SelectionChanged(); });

        ItemCategories.ForEach(x => filterOptions.Add(new OptionDataAdvanced {text = x.FormatTitle()}));

        FilterGuiDropdown.AddOptions(filterOptions);
        FilterGuiDropdown.template.sizeDelta = new Vector2(1f, 208f);

        // adds the by sort group

        by.name = "SortGroup";
        by.transform.localPosition = new Vector3(-302f, 370f, 0f);

        BySortGroup.Inverted = false;
        BySortGroup.Selected = true;
        BySortGroup.SortRequested = (_, inverted) =>
        {
            BySortGroup.Inverted = inverted;
            BySortGroup.Refresh();
            SelectionChanged();
        };

        byTextMesh.SetText("by");

        // adds the sort dropdown

        var sortOptions = new List<TMP_Dropdown.OptionData>();

        sort.name = "SortDropdown";
        sort.transform.localPosition = new Vector3(-205f, 370f, 0f);

        sortRect.sizeDelta = new Vector2(150f, 28f);

        SortGuiDropdown.ClearOptions();
        SortGuiDropdown.onValueChanged.AddListener(delegate { SelectionChanged(); });

        SortCategories.ForEach(x => sortOptions.Add(new OptionDataAdvanced {text = x}));

        SortGuiDropdown.AddOptions(sortOptions);
        SortGuiDropdown.template.sizeDelta = new Vector2(1f, 208f);
    }

    internal static void ResetControls()
    {
        if (BySortGroup == null)
        {
            return;
        }

        FilterGuiDropdown.value = 0;
        SortGuiDropdown.value = 0;
        BySortGroup.Inverted = false;
        BySortGroup.Refresh();
    }

    internal static void RefreshControlsVisibility()
    {
        var active = Main.Settings.EnableInventoryFilteringAndSorting && !Global.IsMultiplayer;

        FilterGuiDropdown.gameObject.SetActive(active);
        BySortGroup.gameObject.SetActive(active);
        SortGuiDropdown.gameObject.SetActive(active);
    }

    private static void Sort(List<RulesetItem> items)
    {
        var sortOrder = BySortGroup.Inverted ? -1 : 1;

        int SortByName(RulesetItem a, RulesetItem b)
        {
            var at = Gui.Localize(a.ItemDefinition.GuiPresentation.Title);
            var bt = Gui.Localize(b.ItemDefinition.GuiPresentation.Title);

            if (at == bt)
            {
                return sortOrder * (a.StackCount - b.StackCount);
            }

            return sortOrder * at.CompareTo(bt);
        }

        switch (SortGuiDropdown.value)
        {
            case 0: // Name
                items.Sort(SortByName);

                break;

            case 1: // Category
                items.Sort((a, b) =>
                {
                    var merchantCategoryDefinitions = DatabaseRepository.GetDatabase<MerchantCategoryDefinition>();

                    var amct = Gui.Localize(merchantCategoryDefinitions.GetElement(a.ItemDefinition.MerchantCategory)
                        .GuiPresentation.Title);
                    var bmct = Gui.Localize(merchantCategoryDefinitions.GetElement(b.ItemDefinition.MerchantCategory)
                        .GuiPresentation.Title);

                    if (amct == bmct)
                    {
                        return SortByName(a, b);
                    }

                    return sortOrder * bmct.CompareTo(bmct);
                });

                break;

            case 2: // Cost
                items.Sort((a, b) =>
                {
                    var ac = a.ComputeCost();
                    var bc = b.ComputeCost();

                    if (ac == bc)
                    {
                        return SortByName(a, b);
                    }

                    return sortOrder * EquipmentDefinitions.CompareCosts(ac, bc);
                });

                break;

            case 3: // Weight
                items.Sort((a, b) =>
                {
                    var aw = a.ComputeWeight();
                    var bw = b.ComputeWeight();

                    if (Mathf.Abs(aw - bw) < .0E-5f)
                    {
                        return SortByName(a, b);
                    }

                    return sortOrder * aw.CompareTo(bw);
                });

                break;

            case 4: // Cost per Weight
                items.Sort((a, b) =>
                {
                    var acpw = EquipmentDefinitions.GetApproximateCostInGold(a.ItemDefinition.Costs) /
                               a.ComputeWeight();
                    var bcpw = EquipmentDefinitions.GetApproximateCostInGold(b.ItemDefinition.Costs) /
                               b.ComputeWeight();

                    if (Mathf.Abs(acpw - bcpw) < .0E-5f)
                    {
                        return SortByName(a, b);
                    }

                    return sortOrder * acpw.CompareTo(bcpw);
                });

                break;
        }
    }

    internal static void SortAndFilter(RulesetContainer container)
    {
        if (container == null)
        {
            return;
        }

        var allItems = new List<RulesetItem>();

        container.EnumerateAllItems(allItems);
        container.InventorySlots.ForEach(slot => slot.UnequipItem(silent: true));

        Sort(allItems);

        allItems.ForEach(item =>
        {
            var value = FilterGuiDropdown.value;

            if (value == 0 || item.ItemDefinition.MerchantCategory == ItemCategories[value].Name)
            {
                container.AddSubItem(item, true);
            }
            else
            {
                FilteredItems.Add(item);
            }
        });
    }

    internal static void Flush(RulesetContainer container)
    {
        if (container == null)
        {
            return;
        }

        FilteredItems.ForEach(item => container.AddSubItem(item, true));
        FilteredItems.Clear();
    }
}
