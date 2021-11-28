using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using static SolastaModApi.DatabaseHelper;

namespace SolastaCommunityExpansion.Models
{
    internal static class InventoryManagementContext
    {
        private static int previousFilterDropDownValue = 0;

        private static bool previousSortAscending = true;

        private static int previousSortDropDownValue = 0;

        private static int currentFilterDropDownValue = 0;

        private static bool currentSortAscending = true;

        private static int currentSortDropDownValue = 0;

        private static readonly List<RulesetItem> FilteredOutItems = new List<RulesetItem>();

        private static readonly List<MerchantCategoryDefinition> FilterCategories = new List<MerchantCategoryDefinition>();

        internal static void Load()
        {
            if (!Main.Settings.EnableInventoryFilterAndSort)
            {
                return;
            }

            var characterInspectionScreen = Gui.GuiService.GetScreen<CharacterInspectionScreen>();
            var rightGroup = characterInspectionScreen.transform.FindChildRecursive("RightGroup");
            var containerPanel = rightGroup.GetComponentInChildren<ContainerPanel>();

            var dropdownPrefab = Resources.Load<GameObject>("GUI/Prefabs/Component/Dropdown");
            var sortGroupPrefab = Gui.GuiService.GetScreen<MainMenuScreen>().transform.FindChildRecursive("SortGroupAlphabetical");

            var filter = Object.Instantiate(dropdownPrefab, rightGroup);
            var filterRect = filter.GetComponent<RectTransform>();
            var filterGuiDropdown = filter.GetComponent<GuiDropdown>();

            var by = Object.Instantiate(sortGroupPrefab, rightGroup);
            var byTextMesh = by.GetComponentInChildren<TextMeshProUGUI>();
            var bySortGroup = by.GetComponent<SortGroup>();

            var sort = Object.Instantiate(dropdownPrefab, rightGroup);
            var sortRect = sort.GetComponent<RectTransform>();
            var sortGuiDropdown = sort.GetComponent<GuiDropdown>();

            var reorder = rightGroup.transform.Find("ReorderPersonalContainerButton");
            var reorderButton = reorder.GetComponent<UnityEngine.UI.Button>();
            var reorderTextMesh = reorder.GetComponentInChildren<TextMeshProUGUI>();

            // caches categories

            var merchantCategoryDefinitions = DatabaseRepository.GetDatabase<MerchantCategoryDefinition>();
            var filteredCategoryDefinitions = merchantCategoryDefinitions.Where(x => x != MerchantCategoryDefinitions.All).ToList();

            filteredCategoryDefinitions.Sort((a, b) => a.FormatTitle().CompareTo(b.FormatTitle()));

            FilterCategories.Add(MerchantCategoryDefinitions.All);
            FilterCategories.AddRange(filteredCategoryDefinitions);

            // adds the filter dropdown

            var filterOptions = new List<TMP_Dropdown.OptionData>();

            filter.name = "FilterDropdown";
            filter.transform.localPosition = new Vector3(-422f, 370f, 0f);

            filterRect.sizeDelta = new Vector2(150f, 28f);

            filterGuiDropdown.ClearOptions();
            filterGuiDropdown.onValueChanged.AddListener(delegate
            {
                currentFilterDropDownValue = filterGuiDropdown.value;
                Refresh(containerPanel);
            });

            FilterCategories.ForEach(x => filterOptions.Add(new TMP_Dropdown.OptionData() { text = x.FormatTitle() }));

            filterGuiDropdown.AddOptions(filterOptions);
            filterGuiDropdown.template.sizeDelta = new Vector2(1f, 208f);

            // adds the sort direction toggle

            by.name = "SortGroup";
            by.transform.localPosition = new Vector3(-302f, 370f, 0f);

            bySortGroup.Inverted = !currentSortAscending;
            bySortGroup.Selected = true;
            bySortGroup.SortRequested = new SortGroup.SortRequestedHandler((sortCategory, inverted) =>
            {
                bySortGroup.Inverted = inverted;
                bySortGroup.Refresh();

                currentSortAscending = !inverted;
                Refresh(containerPanel);
            });

            byTextMesh.SetText("by");

            // adds the sort dropdown

            sort.name = "SortDropdown";
            sort.transform.localPosition = new Vector3(-205f, 370f, 0f);

            sortRect.sizeDelta = new Vector2(150f, 28f);

            sortGuiDropdown.ClearOptions();
            sortGuiDropdown.onValueChanged.AddListener(delegate
            {
                currentSortDropDownValue = sortGuiDropdown.value;
                Refresh(containerPanel);
            });

            //
            // TODO: move hard-coded texts to translations-en
            //

            sortGuiDropdown.AddOptions(new List<TMP_Dropdown.OptionData>()
            {
                new TMP_Dropdown.OptionData() { text = "Default" },
                new TMP_Dropdown.OptionData() { text = "Category" },
                new TMP_Dropdown.OptionData() { text = "Name" },
                new TMP_Dropdown.OptionData() { text = "Cost" },
                new TMP_Dropdown.OptionData() { text = "Weight" },
                new TMP_Dropdown.OptionData() { text = "Cost per Weight" },
            });

            // captures and changes the reorder button behavior
            reorder.localPosition = new Vector3(-32f, 358f, 0f);
            reorderButton.onClick.AddListener(delegate
            {
                previousFilterDropDownValue = 0;
                previousSortAscending = true;
                previousSortDropDownValue = 0;

                currentFilterDropDownValue = 0;
                currentSortAscending = true;
                currentSortDropDownValue = 0;

                filterGuiDropdown.value = 0;
                bySortGroup.Inverted = false;
                sortGuiDropdown.value = 0;

                bySortGroup.Refresh();

                Refresh(containerPanel, clearState: true);
            });

            reorderTextMesh.text = "Reset";
        }

        internal static void MarkAsDirty() => previousSortAscending = !previousSortAscending;

        private static void Sort(List<RulesetItem> items)
        {
            int SortOrder() => currentSortAscending ? 1 : -1;

            switch (currentSortDropDownValue)
            {
                case 0: // Default
                    items.Sort((a, b) =>
                    {
                        int asi = a.ItemDefinition.SortingIndex;
                        int bsi = b.ItemDefinition.SortingIndex;
                        return SortOrder() * (asi == bsi ? Gui.Localize(a.ItemDefinition.FormatTitle()).CompareTo(Gui.Localize(a.ItemDefinition.FormatTitle())) : asi.CompareTo(bsi));
                    });
                    break;

                case 1: // Name
                    items.Sort((a, b) =>
                    {
                        return SortOrder() * a.ItemDefinition.FormatTitle().CompareTo(b.ItemDefinition.FormatTitle());
                    });
                    break;

                case 2: // Category
                    items.Sort((a, b) =>
                    {
                        var merchantCategoryDefinitions = DatabaseRepository.GetDatabase<MerchantCategoryDefinition>();

                        var amc = merchantCategoryDefinitions.GetElement(a.ItemDefinition.MerchantCategory).FormatTitle();
                        var bmc = merchantCategoryDefinitions.GetElement(b.ItemDefinition.MerchantCategory).FormatTitle();

                        if (amc == bmc)
                        {
                            return SortOrder() * a.ItemDefinition.FormatTitle().CompareTo(b.ItemDefinition.FormatTitle());
                        }

                        return SortOrder() * amc.CompareTo(bmc);
                    });
                    break;

                case 3: // Cost
                    items.Sort((a, b) =>
                    {
                        var ac = a.ComputeCost();
                        var bc = b.ComputeCost();

                        if (ac == bc)
                        {
                            return SortOrder() * a.ItemDefinition.FormatTitle().CompareTo(b.ItemDefinition.FormatTitle());
                        }

                        return SortOrder() * EquipmentDefinitions.CompareCosts(ac, bc);
                    });
                    break;

                case 4: // Weight
                    items.Sort((a, b) =>
                    {
                        var aw = a.ComputeWeight();
                        var bw = b.ComputeWeight();

                        if (Mathf.Abs(aw - bw) < .0E-5f)
                        {
                            return SortOrder() * a.ItemDefinition.FormatTitle().CompareTo(b.ItemDefinition.FormatTitle());
                        }

                        return SortOrder() * aw.CompareTo(bw);
                    });
                    break;

                case 5: // Cost per Weight
                    items.Sort((a, b) =>
                    {
                        var acpw = EquipmentDefinitions.GetApproximateCostInGold(a.ItemDefinition.Costs) / a.ComputeWeight();
                        var bcpw = EquipmentDefinitions.GetApproximateCostInGold(b.ItemDefinition.Costs) / b.ComputeWeight();

                        if (Mathf.Abs(acpw - bcpw) < .0E-4f)
                        {
                            return SortOrder() * a.ItemDefinition.FormatTitle().CompareTo(b.ItemDefinition.FormatTitle());
                        }

                        return SortOrder() * acpw.CompareTo(bcpw);
                    });
                    break;
            }
        }

        internal static void Refresh(ContainerPanel containerPanel, bool clearState = false)
        {
            var clean = previousFilterDropDownValue == currentFilterDropDownValue && previousSortAscending == currentSortAscending && previousSortDropDownValue == currentSortDropDownValue;
            var container = containerPanel.Container;

            if ((clearState || !clean) && container != null)
            {
                var items = new List<RulesetItem>();

                container.EnumerateAllItems(items);
                container.InventorySlots.ForEach(x => x.UnequipItem(silent: true));

                items.AddRange(FilteredOutItems);
                FilteredOutItems.Clear();

                Sort(items);

                foreach (var item in items)
                {
                    if (clearState || currentFilterDropDownValue == 0 || item.ItemDefinition.MerchantCategory == FilterCategories[currentFilterDropDownValue].Name)
                    {
                        container.AddSubItem(item, true);
                    }
                    else
                    {
                        FilteredOutItems.Add(item);
                    }
                }

                previousFilterDropDownValue = currentFilterDropDownValue;
                previousSortAscending = clearState ? !currentSortAscending : currentSortAscending; // bypass here forces a refresh on next bind as it creates an unclean state. this is required when swaping heroes
                previousSortDropDownValue = currentSortDropDownValue;

                containerPanel.InspectedCharacter?.RulesetCharacterHero?.CharacterRefreshed?.Invoke(containerPanel.InspectedCharacter.RulesetCharacterHero);
            }
        }
    }
}
