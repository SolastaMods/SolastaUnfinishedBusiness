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
        "UI/&InventoryFilterNone",
        "UI/&InventoryFilterName",
        "UI/&InventoryFilterCategory",
        "UI/&InventoryFilterCost",
        "UI/&InventoryFilterWeight",
        "UI/&InventoryFilterCostPerWeight"
    ];

    private static readonly List<MerchantCategoryDefinition> ItemCategories = [];

    private static readonly List<RulesetInventorySlot> Filtered = [];
    private static bool _dirty = true;

    public static bool Enabled => Main.Settings.EnableInventoryFilteringAndSorting;
    // && (!Global.IsMultiplayer || Main.Settings.AllowSortingInMultiplayer);

    private static GuiDropdown FilterGuiDropdown { get; set; }

    private static SortGroup BySortGroup { get; set; }

    private static GuiDropdown SortGuiDropdown { get; set; }

    private static GuiDropdown TaggedGuiDropdown { get; set; }

    private static Toggle UnidentifiedToggle { get; set; }
    private static GameObject UnidentifiedText { get; set; }

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

        // repositions the reorder button and refactor the listener

        var reorder = rightGroup.transform.Find("ReorderPersonalContainerButton");
        var reorderButton = reorder.GetComponent<Button>();

        reorder.localPosition = new Vector3(-10f, 358f, 0f);
        reorderButton.onClick.RemoveAllListeners();
        reorderButton.onClick.AddListener(delegate
        {
            containerPanel.OnReorderCb();

            if (!Enabled)
            {
                return;
            }

            ResetControls();
            Refresh(containerPanel);
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
        FilterGuiDropdown.onValueChanged.AddListener(delegate { Refresh(containerPanel); });

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
            Refresh(containerPanel);
        };

        byTextMesh.SetText(Gui.Localize("UI/&InventoryFilterBy"));

        // adds the sort dropdown

        var sortOptions = new List<TMP_Dropdown.OptionData>();

        sort.name = "SortDropdown";
        sort.transform.localPosition = new Vector3(-205f, 370f, 0f);

        sortRect.sizeDelta = new Vector2(150f, 28f);

        SortGuiDropdown.ClearOptions();
        SortGuiDropdown.onValueChanged.AddListener(delegate { Refresh(containerPanel); });

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

        TaggedGuiDropdown.onValueChanged.AddListener(delegate { Refresh(containerPanel); });
        TaggedGuiDropdown.AddOptions(taggedOptions);
        TaggedGuiDropdown.template.sizeDelta = new Vector2(1f, 208f);

        UnidentifiedToggle.transform.localPosition = new Vector3(-162f, 330f, 0f);
        UnidentifiedToggle.onValueChanged = new Toggle.ToggleEvent();
        UnidentifiedToggle.isOn = false;
        UnidentifiedToggle.onValueChanged.AddListener(delegate { Refresh(containerPanel); });

        UnidentifiedText.GetComponentInChildren<TextMeshProUGUI>()
            .SetText(Gui.Localize("UI/&InventoryFilterUnidentifiedMagical"));
        UnidentifiedText.transform.localPosition = new Vector3(-332f, 340f, 0f);
    }

    private static void ResetControls()
    {
        if (!BySortGroup)
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
        FilterGuiDropdown.gameObject.SetActive(Enabled);
        BySortGroup.gameObject.SetActive(Enabled);
        SortGuiDropdown.gameObject.SetActive(Enabled);
        TaggedGuiDropdown.gameObject.SetActive(Enabled);
        UnidentifiedToggle.gameObject.SetActive(Enabled);
        UnidentifiedText.gameObject.SetActive(Enabled);
    }

    private static int ItemSort(RulesetInventorySlot slotA, RulesetInventorySlot slotB)
    {
        var itemA = slotA.EquipedItem;
        var itemB = slotB.EquipedItem;

        if (itemA == null)
        {
            return itemB == null ? 0 : 1;
        }

        if (itemB == null)
        {
            return -1;
        }

        var result = SortGuiDropdown.value switch
        {
            1 => SortByName(itemA, itemB),
            2 => SortByCategory(itemA, itemB),
            3 => SortByCost(itemA, itemB),
            4 => SortByWeight(itemA, itemB),
            5 => SortByCostPerWeight(itemA, itemB),
            _ => SortByName(itemA, itemB) //we shouldn't get here
        };
        return BySortGroup.Inverted ? -result : result;
    }

    private static int SortByName([NotNull] RulesetItem a, [NotNull] RulesetItem b)
    {
        var at = Gui.Localize(a.ItemDefinition.GuiPresentation.Title);
        var bt = Gui.Localize(b.ItemDefinition.GuiPresentation.Title);

        return at == bt
            ? a.StackCount.CompareTo(b.StackCount)
            : string.Compare(at, bt, StringComparison.CurrentCultureIgnoreCase);
    }

    private static int SortByCategory([NotNull] RulesetItem itemA, [NotNull] RulesetItem itemB)
    {
        var categoryDefinitions = DatabaseRepository.GetDatabase<MerchantCategoryDefinition>();

        var categoryA = categoryDefinitions.GetElement(itemA.ItemDefinition.MerchantCategory).FormatTitle();
        var categoryB = categoryDefinitions.GetElement(itemB.ItemDefinition.MerchantCategory).FormatTitle();

        return categoryA == categoryB
            ? SortByName(itemA, itemB)
            : string.Compare(categoryB, categoryB, StringComparison.CurrentCultureIgnoreCase);
    }

    private static int SortByCost([NotNull] RulesetItem itemA, [NotNull] RulesetItem itemB)
    {
        var ac = itemA.ComputeCost();
        var bc = itemB.ComputeCost();

        return ac == bc
            ? SortByName(itemA, itemB)
            : EquipmentDefinitions.CompareCosts(ac, bc);
    }

    private static int SortByWeight([NotNull] RulesetItem itemA, [NotNull] RulesetItem itemB)
    {
        var aw = itemA.ComputeWeight();
        var bw = itemB.ComputeWeight();

        return Mathf.Abs(aw - bw) < .0E-5f
            ? SortByName(itemA, itemB)
            : aw.CompareTo(bw);
    }

    private static int SortByCostPerWeight([NotNull] RulesetItem itemA, [NotNull] RulesetItem itemB)
    {
        // ReSharper disable once IdentifierTypo
        var acpw = EquipmentDefinitions.GetApproximateCostInGold(itemA.ItemDefinition.Costs) /
                   Math.Max(itemA.ComputeWeight(), 0.01f);
        // ReSharper disable once IdentifierTypo
        var bcpw = EquipmentDefinitions.GetApproximateCostInGold(itemB.ItemDefinition.Costs) /
                   Math.Max(itemB.ComputeWeight(), 0.01f);

        return Mathf.Abs(acpw - bcpw) < .0E-5f ? SortByName(itemA, itemB) : acpw.CompareTo(bcpw);
    }


    private static bool FilterItem(RulesetItem item, [CanBeNull] ISerializable container)
    {
        if (item == null) { return true; }

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

    //`container` parameter is required for the transpile patch
    // ReSharper disable once UnusedParameter.Global
    public static List<RulesetInventorySlot> GetFilteredSlots(RulesetContainer container, ContainerPanel panel)
    {
        return GetFilteredAndSorted(panel);
    }

    public static void BindInventory(ContainerPanel panel)
    {
        if (!panel.TryGetComponent<InventoryContainerPanelMarker>(out _))
        {
            panel.gameObject.AddComponent<InventoryContainerPanelMarker>();
        }

        Reset();
    }

    public static void UnbindInventory(ContainerPanel panel)
    {
        if (panel.Container == null) { return; }

        if (panel.TryGetComponent<InventoryContainerPanelMarker>(out var marker))
        {
            Object.DestroyImmediate(marker);
        }

        Reset();
    }

    public static void Refresh(ContainerPanel panel, bool light = false)
    {
        Reset();
        if (light) { return; }

        var container = panel.Container;
        var character = panel.InspectedCharacter;
        var dropHandler = panel.DropAreaClicked;
        var slotsRefreshed = panel.VisibleSlotsRefreshed;

        panel.Unbind();
        panel.Bind(container, character, dropHandler, slotsRefreshed);
    }

    private static void Reset()
    {
        _dirty = true;
        Filtered.Clear();
    }

    private static List<RulesetInventorySlot> GetFilteredAndSorted(ContainerPanel panel)
    {
        return panel.TryGetComponent<InventoryContainerPanelMarker>(out _)
            ? FilterAndSort(panel.Container)
            : panel.Container.InventorySlots;
    }

    private static List<RulesetInventorySlot> FilterAndSort(RulesetContainer container)
    {
        if (!_dirty)
        {
            return Filtered;
        }

        Filtered.Clear();
        Filtered.AddRange(container.InventorySlots.Where(slot => FilterItem(slot.EquipedItem, container)));

        if (SortGuiDropdown.value > 0)
        {
            Filtered.Sort(ItemSort);
        }

        _dirty = false;

        return Filtered;
    }
}

public class InventoryContainerPanelMarker : MonoBehaviour;
