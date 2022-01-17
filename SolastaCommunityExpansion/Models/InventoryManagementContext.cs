using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using static SolastaModApi.DatabaseHelper;

namespace SolastaCommunityExpansion.Models
{
    internal static class InventoryManagementContext
    {
        private static readonly List<string> SortCategories = new List<string>
        {
            "Name",
            "Category",
            "Cost",
            "Weight",
            "Cost per Weight",
        };

        private static readonly List<RulesetItem> FilteredItems = new List<RulesetItem>();

        private static readonly List<MerchantCategoryDefinition> ItemCategories = new List<MerchantCategoryDefinition>();

        private static GuiDropdown FilterGuiDropdown { get; set; }

        private static SortGroup BySortGroup { get; set; }

        private static GuiDropdown SortGuiDropdown { get; set; }

        private static Transform Reorder;

        internal static void Load()
        {
            if (!Main.Settings.EnableInventoryFilteringAndSorting)
            {
                return;
            }

            var characterInspectionScreen = Gui.GuiService.GetScreen<CharacterInspectionScreen>();
            var rightGroup = characterInspectionScreen.transform.FindChildRecursive("RightGroup");
            var containerPanel = rightGroup.GetComponentInChildren<ContainerPanel>();

            void SelectionChanged()
            {
                var container = containerPanel.Container;

                containerPanel.Unbind();
                containerPanel.Bind(container, containerPanel.InspectedCharacter, containerPanel.DropAreaClicked, containerPanel.VisibleSlotsRefreshed);
            }

            var dropdownPrefab = Resources.Load<GameObject>("GUI/Prefabs/Component/Dropdown");
            var sortGroupPrefab = Gui.GuiService.GetScreen<MainMenuScreen>().transform.FindChildRecursive("SortGroupAlphabetical");

            var filter = Object.Instantiate(dropdownPrefab, rightGroup);
            var filterRect = filter.GetComponent<RectTransform>();
            
            FilterGuiDropdown = filter.GetComponent<GuiDropdown>();

            var by = Object.Instantiate(sortGroupPrefab, rightGroup);
            var byTextMesh = by.GetComponentInChildren<TextMeshProUGUI>();
            
            BySortGroup = by.GetComponent<SortGroup>();

            var sort = Object.Instantiate(dropdownPrefab, rightGroup);
            var sortRect = sort.GetComponent<RectTransform>();
            
            SortGuiDropdown = sort.GetComponent<GuiDropdown>();

            Reorder = rightGroup.transform.Find("ReorderPersonalContainerButton");

            // caches categories

            var merchantCategoryDefinitions = DatabaseRepository.GetDatabase<MerchantCategoryDefinition>();
            var filteredCategoryDefinitions = merchantCategoryDefinitions.Where(x => x != MerchantCategoryDefinitions.All).ToList();

            filteredCategoryDefinitions.Sort((a, b) => a.FormatTitle().CompareTo(b.FormatTitle()));

            ItemCategories.Add(MerchantCategoryDefinitions.All);
            ItemCategories.AddRange(filteredCategoryDefinitions);

            // adds the filter dropdown

            var filterOptions = new List<TMP_Dropdown.OptionData>();

            filter.name = "FilterDropdown";
            filter.transform.localPosition = new Vector3(-422f, 370f, 0f);

            filterRect.sizeDelta = new Vector2(150f, 28f);

            FilterGuiDropdown.ClearOptions();
            FilterGuiDropdown.onValueChanged.AddListener(delegate { SelectionChanged(); });

            ItemCategories.ForEach(x => filterOptions.Add(new TMP_Dropdown.OptionData() { text = x.FormatTitle() }));

            FilterGuiDropdown.AddOptions(filterOptions);
            FilterGuiDropdown.template.sizeDelta = new Vector2(1f, 208f);

            // adds the sort direction toggle

            by.name = "SortGroup";
            by.transform.localPosition = new Vector3(-302f, 370f, 0f);

            BySortGroup.Inverted = false;
            BySortGroup.Selected = true;
            BySortGroup.SortRequested = new SortGroup.SortRequestedHandler((sortCategory, inverted) =>
            {
                BySortGroup.Inverted = inverted;
                BySortGroup.Refresh();
                SelectionChanged();
            });

            byTextMesh.SetText("by");

            // adds the sort dropdown

            var sortOptions = new List<TMP_Dropdown.OptionData>();

            sort.name = "SortDropdown";
            sort.transform.localPosition = new Vector3(-205f, 370f, 0f);

            sortRect.sizeDelta = new Vector2(150f, 28f);

            SortGuiDropdown.ClearOptions();
            SortGuiDropdown.onValueChanged.AddListener(delegate { SelectionChanged(); });

            SortCategories.ForEach(x => sortOptions.Add(new TMP_Dropdown.OptionData() { text = x }));

            SortGuiDropdown.AddOptions(sortOptions);
            SortGuiDropdown.template.sizeDelta = new Vector2(1f, 208f);
        }

        internal static void RefreshControlsVisibility()
        {
            var active = Main.Settings.EnableInventoryFilteringAndSorting;

            FilterGuiDropdown.gameObject.SetActive(active);
            BySortGroup.gameObject.SetActive(active);
            SortGuiDropdown.gameObject.SetActive(active);
            Reorder.gameObject.SetActive(!active);
        }

        private static void Sort(List<RulesetItem> items)
        {
            var sortOrder = BySortGroup.Inverted ? -1 : 1;

            int SortByName(RulesetItem a, RulesetItem b)
            {
                var at = Gui.Format(a.ItemDefinition.GuiPresentation.Title);
                var bt = Gui.Format(b.ItemDefinition.GuiPresentation.Title);

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

                        var amct = Gui.Format(merchantCategoryDefinitions.GetElement(a.ItemDefinition.MerchantCategory).GuiPresentation.Title);
                        var bmct = Gui.Format(merchantCategoryDefinitions.GetElement(b.ItemDefinition.MerchantCategory).GuiPresentation.Title);

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
                        var acpw = EquipmentDefinitions.GetApproximateCostInGold(a.ItemDefinition.Costs) / a.ComputeWeight();
                        var bcpw = EquipmentDefinitions.GetApproximateCostInGold(b.ItemDefinition.Costs) / b.ComputeWeight();

                        if (Mathf.Abs(acpw - bcpw) < .0E-5f)
                        {
                            return SortByName(a, b);
                        }

                        return sortOrder * acpw.CompareTo(bcpw);
                    });

                    break;
            }
        }

        internal static void SortAndFilter(ContainerPanel containerPanel, RulesetContainer container = null)
        {
            container = container ?? containerPanel.Container;

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
                if (FilterGuiDropdown.value == 0 || item.ItemDefinition.MerchantCategory == ItemCategories[FilterGuiDropdown.value].Name)
                {
                    container.AddSubItem(item, silent: true);
                }
                else
                {
                    FilteredItems.Add(item);
                }
            });
        }

        internal static void Flush(ContainerPanel containerPanel)
        {
            var container = containerPanel.Container;

            if (container == null)
            {
                return;
            }

            FilteredItems.ForEach(item => container.AddSubItem(item, silent: true));
            FilteredItems.Clear();
        }
    }
}
