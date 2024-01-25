using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static GuiDropdown;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using Object = UnityEngine.Object;

namespace SolastaUnfinishedBusiness.Models;

internal static class InventoryManagementContext
{
    private static readonly List<string> SortCategories =
    [
        "UI/&InventoryFilterName",
        "UI/&InventoryFilterCategory",
        "UI/&InventoryFilterCost",
        "UI/&InventoryFilterWeight",
        "UI/&InventoryFilterCostPerWeight"
    ];

    private static readonly List<RulesetItem> FilteredItems = [];

    private static readonly List<MerchantCategoryDefinition> ItemCategories = [];

    private static GuiDropdown FilterGuiDropdown { get; set; }

    private static SortGroup BySortGroup { get; set; }

    private static GuiDropdown SortGuiDropdown { get; set; }

    private static GuiDropdown TaggedGuiDropdown { get; set; }

    private static Toggle UnidentifiedToggle { get; set; }
    private static GameObject UnidentifiedText { get; set; }

    internal static Action SelectionChanged { get; private set; }

    internal static void Load()
    {
        var characterInspectionScreen = Gui.GuiService.GetScreen<CharacterInspectionScreen>();
        var rightGroup = characterInspectionScreen.transform.FindChildRecursive("RightGroup");
        var containerPanel = rightGroup.GetComponentInChildren<ContainerPanel>();
        containerPanel.rectTransform.anchoredPosition += new Vector2(0, -80);

        // ReSharper disable once Unity.UnknownResource
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

        var tagged = Object.Instantiate(dropdownPrefab, rightGroup);
        var taggedRect = tagged.GetComponent<RectTransform>();

        TaggedGuiDropdown = tagged.GetComponent<GuiDropdown>();

        // ReSharper disable once Unity.UnknownResource
        var checkboxPrefab = Resources.Load<GameObject>("Gui/Prefabs/Modal/Setting/SettingCheckboxItem");
        var smallToggleNoFrame = checkboxPrefab.transform.Find("SmallToggleNoFrame");

        UnidentifiedToggle = Object.Instantiate(smallToggleNoFrame, rightGroup).GetComponentInChildren<Toggle>();
        UnidentifiedToggle.name = "IdentifiedToggle";
        UnidentifiedToggle.gameObject.SetActive(true);
        UnidentifiedToggle.onValueChanged.RemoveAllListeners();
        UnidentifiedText = Object.Instantiate(byTextMesh.gameObject, rightGroup);

        //
        // on any control change we need to unbind / bind the entire panel to refresh all the additional items gizmos
        //

        SelectionChanged = () =>
        {
            var container = containerPanel.Container;

            if (container == null)
            {
                return;
            }

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
            if (Main.Settings.EnableInventoryFilteringAndSorting && !Global.IsMultiplayer)
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

        ItemCategories.ForEach(x => filterOptions.Add(new OptionDataAdvanced { text = x.FormatTitle() }));

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

        byTextMesh.SetText(Gui.Localize("UI/&InventoryFilterBy"));

        // adds the sort dropdown

        var sortOptions = new List<TMP_Dropdown.OptionData>();

        sort.name = "SortDropdown";
        sort.transform.localPosition = new Vector3(-205f, 370f, 0f);

        sortRect.sizeDelta = new Vector2(150f, 28f);

        SortGuiDropdown.ClearOptions();
        SortGuiDropdown.onValueChanged.AddListener(delegate { SelectionChanged(); });

        SortCategories.ForEach(x => sortOptions.Add(new OptionDataAdvanced { text = Gui.Localize(x) }));

        SortGuiDropdown.AddOptions(sortOptions);
        SortGuiDropdown.template.sizeDelta = new Vector2(1f, 208f);

        // adds the tagged dropdown

        var taggedOptions = new List<TMP_Dropdown.OptionData>();

        tagged.name = "TaggedDropdown";
        tagged.transform.localPosition = new Vector3(-422f, 330f, 0f);
        taggedRect.sizeDelta = new Vector2(150f, 28f);

        TaggedGuiDropdown.ClearOptions();
        taggedOptions.AddRange(new OptionDataAdvanced[]
        {
            new() { text = TagsDefinitions.Document }, new() { text = TagsDefinitions.SpellFocus },
            new() { text = TagsDefinitions.Food }, new() { text = TagsDefinitions.CarryingCapacity },
            new() { text = TagsDefinitions.ItemTagMetal }, new() { text = TagsDefinitions.ItemTagSilver },
            new() { text = TagsDefinitions.ItemTagGold }, new() { text = TagsDefinitions.ItemTagWood },
            new() { text = TagsDefinitions.ItemTagLeather }, new() { text = TagsDefinitions.ItemTagGlass },
            new() { text = TagsDefinitions.ItemTagPaper }, new() { text = TagsDefinitions.ItemTagFlamable },
            new() { text = TagsDefinitions.ItemTagQuest }, new() { text = TagsDefinitions.ItemTagIngredient },
            new() { text = TagsDefinitions.ItemTagGem }, new() { text = TagsDefinitions.ArcaneFocus },
            new() { text = TagsDefinitions.DruidicFocus }, new() { text = TagsDefinitions.ItemTagMonk },
            new() { text = TagsDefinitions.MusicalInstrument }, new() { text = TagsDefinitions.LightSource },
            new() { text = TagsDefinitions.WeaponTagAmmunition }, new() { text = CeContentPackContext.CeTag }
        });

        taggedOptions.Sort((x, y) => String.Compare(x.text, y.text, StringComparison.Ordinal));
        taggedOptions.Insert(0, new OptionDataAdvanced { text = Gui.Localize("UI/&InventoryFilterAnyTags") });

        TaggedGuiDropdown.onValueChanged.AddListener(delegate { SelectionChanged(); });
        TaggedGuiDropdown.AddOptions(taggedOptions);
        TaggedGuiDropdown.template.sizeDelta = new Vector2(1f, 208f);

        UnidentifiedToggle.transform.localPosition = new Vector3(-162f, 330f, 0f);
        UnidentifiedToggle.onValueChanged = new Toggle.ToggleEvent();
        UnidentifiedToggle.isOn = false;
        UnidentifiedToggle.onValueChanged.AddListener(delegate { SelectionChanged(); });

        UnidentifiedText.GetComponentInChildren<TextMeshProUGUI>()
            .SetText(Gui.Localize("UI/&InventoryFilterUnidentifiedMagical"));
        UnidentifiedText.transform.localPosition = new Vector3(-332f, 340f, 0f);
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
        TaggedGuiDropdown.value = 0;
        UnidentifiedToggle.isOn = false;
    }

    internal static void RefreshControlsVisibility()
    {
        var active = Main.Settings.EnableInventoryFilteringAndSorting && !Global.IsMultiplayer;

        FilterGuiDropdown.gameObject.SetActive(active);
        BySortGroup.gameObject.SetActive(active);
        SortGuiDropdown.gameObject.SetActive(active);
        TaggedGuiDropdown.gameObject.SetActive(active);
        UnidentifiedToggle.gameObject.SetActive(active);
        UnidentifiedText.gameObject.SetActive(active);
    }

    private static void Sort(List<RulesetItem> items)
    {
        var sortOrder = BySortGroup.Inverted ? -1 : 1;

        switch (SortGuiDropdown.value)
        {
            case 0: // Name
                items.Sort(SortByName);

                break;

            case 1: // Category
                items.Sort((a, b) =>
                {
                    var merchantCategoryDefinitions = DatabaseRepository.GetDatabase<MerchantCategoryDefinition>();

                    // ReSharper disable once IdentifierTypo
                    var amct = Gui.Localize(merchantCategoryDefinitions.GetElement(a.ItemDefinition.MerchantCategory)
                        .GuiPresentation.Title);
                    // ReSharper disable once IdentifierTypo
                    var bmct = Gui.Localize(merchantCategoryDefinitions.GetElement(b.ItemDefinition.MerchantCategory)
                        .GuiPresentation.Title);

                    if (amct == bmct)
                    {
                        return SortByName(a, b);
                    }

                    return sortOrder * String.Compare(bmct, bmct, StringComparison.CurrentCultureIgnoreCase);
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
                    // ReSharper disable once IdentifierTypo
                    var acpw = EquipmentDefinitions.GetApproximateCostInGold(a.ItemDefinition.Costs) /
                               a.ComputeWeight();
                    // ReSharper disable once IdentifierTypo
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

        return;

        int SortByName([NotNull] RulesetItem a, [NotNull] RulesetItem b)
        {
            var at = Gui.Localize(a.ItemDefinition.GuiPresentation.Title);
            var bt = Gui.Localize(b.ItemDefinition.GuiPresentation.Title);

            if (at == bt)
            {
                return sortOrder * (a.StackCount - b.StackCount);
            }

            return sortOrder * String.Compare(at, bt, StringComparison.CurrentCultureIgnoreCase);
        }
    }

    internal static void SortAndFilter([CanBeNull] RulesetContainer container)
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
            if (FilterItem(item, container))
            {
                container.AddSubItem(item, true);
            }
            else
            {
                FilteredItems.Add(item);
            }
        });
    }

    private static bool FilterItem(RulesetItem item, [CanBeNull] ISerializable container)
    {
        if (UnidentifiedToggle.isOn && item.KnowledgeLevel != EquipmentDefinitions.ItemKnowledge.MagicDetected)
        {
            return false;
        }

        var filterIndex = FilterGuiDropdown.value;

        if (filterIndex != 0 && item.ItemDefinition.MerchantCategory != ItemCategories[filterIndex].Name)
        {
            return false;
        }

        var taggedIndex = TaggedGuiDropdown.value;

        if (taggedIndex == 0)
        {
            return true;
        }

        Dictionary<string, TagsDefinitions.Criticity> tagsMap = new();

        item.FillTags(tagsMap, container);

        return tagsMap.Keys.ToArray().Contains(TaggedGuiDropdown.options[taggedIndex].text);
    }

    internal static void Flush([CanBeNull] RulesetContainer container)
    {
        if (container == null)
        {
            return;
        }

        FilteredItems.ForEach(item => container.AddSubItem(item, true));
        FilteredItems.Clear();
    }
}
