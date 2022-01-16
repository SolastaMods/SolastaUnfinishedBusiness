using System.Collections.Generic;
using System.Linq;
using SolastaModApi.Infrastructure;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static SolastaModApi.DatabaseHelper;

namespace SolastaCommunityExpansion.Models
{
    internal static class InventoryManagementContext
    {
        private static readonly List<RulesetItem> FilteredItems = new List<RulesetItem>();

        private static readonly List<MerchantCategoryDefinition> ItemCategories = new List<MerchantCategoryDefinition>();

        private static GuiDropdown FilterGuiDropdown { get; set; }

        private static SortGroup BySortGroup { get; set; }

        private static GuiDropdown SortGuiDropdown { get; set; }

        internal static void Load()
        {
            if (!Main.Settings.EnableInventoryFilteringAndSorting)
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
            
            FilterGuiDropdown = filter.GetComponent<GuiDropdown>();

            var by = Object.Instantiate(sortGroupPrefab, rightGroup);
            var byTextMesh = by.GetComponentInChildren<TextMeshProUGUI>();
            
            BySortGroup = by.GetComponent<SortGroup>();

            var sort = Object.Instantiate(dropdownPrefab, rightGroup);
            var sortRect = sort.GetComponent<RectTransform>();
            
            SortGuiDropdown = sort.GetComponent<GuiDropdown>();

            var reorder = rightGroup.transform.Find("ReorderPersonalContainerButton");
            var reorderButton = reorder.GetComponent<Button>();
            var reorderTextMesh = reorder.GetComponentInChildren<TextMeshProUGUI>();

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
            FilterGuiDropdown.onValueChanged.AddListener(delegate { Refresh(containerPanel); });

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
                Refresh(containerPanel);
            });

            byTextMesh.SetText("by");

            // adds the sort dropdown

            sort.name = "SortDropdown";
            sort.transform.localPosition = new Vector3(-205f, 370f, 0f);

            sortRect.sizeDelta = new Vector2(150f, 28f);

            SortGuiDropdown.ClearOptions();
            SortGuiDropdown.onValueChanged.AddListener(delegate { Refresh(containerPanel); });

            SortGuiDropdown.AddOptions(new List<TMP_Dropdown.OptionData>()
            {
                new TMP_Dropdown.OptionData() { text = "Default" },
                new TMP_Dropdown.OptionData() { text = "Category" },
                new TMP_Dropdown.OptionData() { text = "Name" },
                new TMP_Dropdown.OptionData() { text = "Cost" },
                new TMP_Dropdown.OptionData() { text = "Weight" },
                new TMP_Dropdown.OptionData() { text = "Cost per Weight" },
            });

            // changes the reorder button behavior

            reorder.localPosition = new Vector3(-32f, 358f, 0f);
            reorderButton.onClick.RemoveAllListeners();
            reorderButton.onClick.AddListener(delegate
            {
                FilterGuiDropdown.value = 0;
                SortGuiDropdown.value = 0;
                BySortGroup.Inverted = false;
                BySortGroup.Refresh();
                Refresh(containerPanel);
            });
            reorderTextMesh.text = "Reset";
        }

        private static void Sort(List<RulesetItem> items)
        {
            int SortOrder() => BySortGroup.Inverted ? -1 : 1;

            switch (SortGuiDropdown.value)
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

        internal static void Refresh(ContainerPanel containerPanel, bool drainFilter = false)
        {
            var container = containerPanel?.Container;

            if (container == null)
            {
                return;
            }

            var items = new List<RulesetItem>();
            var inspectedCharacter = containerPanel.InspectedCharacter;
            var rulesetCharacterHero = inspectedCharacter.RulesetCharacterHero;
            var dropAreaClicked = containerPanel.DropAreaClicked;
            var visibleSlotsRefreshed = containerPanel.VisibleSlotsRefreshed;

            container.EnumerateAllItems(items);
            container.InventorySlots.ForEach(x => x.UnequipItem(silent: true));
            containerPanel.Unbind();

            items.AddRange(FilteredItems);
            FilteredItems.Clear();
            Sort(items);

            foreach (var item in items)
            {
                if (drainFilter || FilterGuiDropdown.value == 0 || item.ItemDefinition.MerchantCategory == ItemCategories[FilterGuiDropdown.value].Name)
                {
                    container.AddSubItem(item, silent: true);
                }
                else
                {
                    FilteredItems.Add(item);
                }
            }

            containerPanel.Bind(container, inspectedCharacter, dropAreaClicked, visibleSlotsRefreshed);
            rulesetCharacterHero.CharacterRefreshed?.Invoke(rulesetCharacterHero);

            // use red background for non proficient items
            foreach (var inventorySlotBox in containerPanel.BoundSlotBoxes)
            {
                var itemDefinition = inventorySlotBox.InventorySlot.EquipedItem?.ItemDefinition;
                var slotBackgroundImage = inventorySlotBox.transform.Find("SlotBackgroundImage").GetComponent<Image>();

                //
                // TODO: we could tweak the image frame color instead but this was a bit buggy...
                //

                // var EquippedFrameImage = inventorySlotBox.transform.parent.FindChildRecursive("EquipedFrame").GetComponent<Image>();

                if (itemDefinition != null && !rulesetCharacterHero.IsProficientWithItem(itemDefinition))
                {
                    slotBackgroundImage.color = new Color(1, 0, 0, 1);
                }
                else
                {
                    slotBackgroundImage.color = new Color(1, 1, 1, 1);
                }
            }
        }
    }
}
