using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using static SolastaModApi.DatabaseHelper;

namespace SolastaCommunityExpansion.Models
{
    internal static class InventoryManagementContext
    {
        private static int currentFilterDropDownValue = 0;

        private static int currentSortDropDownValue = 0;

        private static int previousFilterDropDownValue = 0;

        private static int previousSortDropDownValue = 0;

        internal static readonly List<RulesetItem> FilteredOutItems = new List<RulesetItem>();

        internal static readonly List<MerchantCategoryDefinition> FilterCategories = new List<MerchantCategoryDefinition>();

        internal static void Load()
        {
            //
            // TODO: check the drop-down positioning on other game resolutions
            //

            //
            // TODO: move hard-coded texts to translations-en
            //

            if (!Main.Settings.EnableInventoryFilterAndSort)
            {
                return;
            }

            var characterInspectionScreen = Gui.GuiService.GetScreen<CharacterInspectionScreen>();
            var rightGroup = characterInspectionScreen.transform.FindChildRecursive("RightGroup");

            var containerPanel = rightGroup.GetComponentInChildren<ContainerPanel>();

            var containerTitle = rightGroup.FindChildRecursive("ContainerTitle");
            var byLabel = Object.Instantiate(containerTitle, rightGroup);
            var textMeshLabel = byLabel.GetComponent<TextMeshProUGUI>();

            var dropdownPrefab = Resources.Load<GameObject>("GUI/Prefabs/Component/Dropdown");

            var sortDropdown = Object.Instantiate(dropdownPrefab, rightGroup);
            var rectSortDropdown = sortDropdown.GetComponent<RectTransform>();
            var guiSortDropdown = sortDropdown.GetComponent<GuiDropdown>();

            var filterDropdown = Object.Instantiate(dropdownPrefab, rightGroup);
            var rectfilterDropdown = filterDropdown.GetComponent<RectTransform>();
            var guiFilterDropdown = filterDropdown.GetComponent<GuiDropdown>();

            // caches the categories

            var merchantCategoryDefinitions = DatabaseRepository.GetDatabase<MerchantCategoryDefinition>();
            var filteredCategoryDefinitions = merchantCategoryDefinitions.Where(x => x != MerchantCategoryDefinitions.All).ToList();

            filteredCategoryDefinitions.Sort((a, b) => a.FormatTitle().CompareTo(b.FormatTitle()));

            FilterCategories.Add(MerchantCategoryDefinitions.All);
            FilterCategories.AddRange(filteredCategoryDefinitions);

            // adds the label

            byLabel.transform.localPosition = new Vector3(-332f, 378f, 0f);
            textMeshLabel.SetText("by");

            // adds the sort dropdown

            sortDropdown.name = "SortDropdown";
            sortDropdown.transform.localPosition = new Vector3(-230f, 370f, 0f);
            rectSortDropdown.sizeDelta = new Vector2(150f, 28f);

            guiSortDropdown.ClearOptions();
            guiSortDropdown.onValueChanged.AddListener(delegate 
            {
                previousSortDropDownValue = currentSortDropDownValue;
                currentSortDropDownValue = guiSortDropdown.value;
                RefreshAfterDropdownChange(containerPanel);
            });

            guiSortDropdown.AddOptions(new List<TMP_Dropdown.OptionData>()
            {
                new TMP_Dropdown.OptionData() { text = "Default" },
                new TMP_Dropdown.OptionData() { text = "Name" },
                new TMP_Dropdown.OptionData() { text = "Category" },
                new TMP_Dropdown.OptionData() { text = "Cost" },
                new TMP_Dropdown.OptionData() { text = "Weight" },
            });

            // adds the filter dropdown

            var filterOptions = new List<TMP_Dropdown.OptionData>();

            filterDropdown.name = "FilterDropdown";
            filterDropdown.transform.localPosition = new Vector3(-422f, 370f, 0f);
            rectfilterDropdown.sizeDelta = new Vector2(150f, 28f);

            guiFilterDropdown.ClearOptions();
            guiFilterDropdown.onValueChanged.AddListener(delegate 
            {
                previousFilterDropDownValue = currentFilterDropDownValue;
                currentFilterDropDownValue = guiFilterDropdown.value;
                RefreshAfterDropdownChange(containerPanel);
            });

            foreach (var filterCategory in FilterCategories)
            {
                filterOptions.Add(new TMP_Dropdown.OptionData() { text = filterCategory.FormatTitle() });
            }

            guiFilterDropdown.AddOptions(filterOptions);
        }

        internal static void ResetDropdowns(bool filterDropdown, bool sortDropdown)
        {
            currentFilterDropDownValue = filterDropdown ? 0 : currentFilterDropDownValue; ;
            currentSortDropDownValue = sortDropdown? 0 : currentSortDropDownValue;
            previousFilterDropDownValue = 0;
            previousSortDropDownValue = 0;

            try
            {
                var characterInspectionScreen = Gui.GuiService.GetScreen<CharacterInspectionScreen>();
                var rightGroup = characterInspectionScreen.transform.FindChildRecursive("RightGroup");
                var containerPanel = rightGroup.GetComponentInChildren<ContainerPanel>();
                var filterGuiDropdown = containerPanel.transform.parent.Find("FilterDropdown").GetComponent<GuiDropdown>();
                var sortGuiDropdown = containerPanel.transform.parent.Find("SortDropdown").GetComponent<GuiDropdown>();

                filterGuiDropdown.value = currentFilterDropDownValue;
                sortGuiDropdown.value = currentSortDropDownValue;
            }
            catch
            {
                Main.Warning("inventory system is disabled.");
            }
        }

        internal static void RefreshAfterDrag()
        {
            if (Main.Settings.EnableInventoryFilterAndSort)
            {
                var characterInspectionScreen = Gui.GuiService.GetScreen<CharacterInspectionScreen>();
                var rightGroup = characterInspectionScreen.transform.FindChildRecursive("RightGroup");
                var containerPanel = rightGroup.GetComponentInChildren<ContainerPanel>();

                containerPanel.InspectedCharacter?.RulesetCharacterHero?.CharacterRefreshed?.Invoke(containerPanel.InspectedCharacter.RulesetCharacterHero);
            }
        }

        private static void RefreshAfterDropdownChange(ContainerPanel containerPanel)
        {
            if (previousFilterDropDownValue == currentFilterDropDownValue && previousSortDropDownValue == currentSortDropDownValue)
            {
                return;
            }

            var items = new List<RulesetItem>();
            var container = containerPanel.Container;

            container.EnumerateAllItems(items);
            items.AddRange(FilteredOutItems);

            switch (currentSortDropDownValue)
            {
                case 0: // Default
                    items.Sort(container);
                    break;

                case 1: // Name
                    items.Sort((a, b) =>
                    {
                        return a.ItemDefinition.FormatTitle().CompareTo(b.ItemDefinition.FormatTitle());
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
                            return a.ItemDefinition.FormatTitle().CompareTo(b.ItemDefinition.FormatTitle());
                        }

                        return amc.CompareTo(bmc);
                    });
                    break;

                case 3: // Cost
                    items.Sort((a, b) =>
                    {
                        var ac = a.ComputeCost();
                        var bc = b.ComputeCost();

                        return EquipmentDefinitions.CompareCosts(ac, bc);
                    });
                    break;

                case 4: // Weight
                    items.Sort((a, b) =>
                    {
                        var aw = a.ComputeWeight();
                        var bw = b.ComputeWeight();
                       
                        return aw.CompareTo(bw);
                    });
                    break;
            }

            container.InventorySlots.ForEach(x => x.UnequipItem(silent: true));
            FilteredOutItems.Clear();

            foreach (var item in items)
            {
                if (currentFilterDropDownValue == 0 || item.ItemDefinition.MerchantCategory == FilterCategories[currentFilterDropDownValue].Name)
                {
                    container.AddSubItem(item, false);
                }
                else
                {
                    FilteredOutItems.Add(item);
                }
            }

            containerPanel.InspectedCharacter.RulesetCharacterHero.CharacterRefreshed?.Invoke(containerPanel.InspectedCharacter.RulesetCharacterHero);
        }
    }
}
