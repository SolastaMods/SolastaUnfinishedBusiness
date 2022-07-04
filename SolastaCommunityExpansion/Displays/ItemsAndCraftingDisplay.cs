using System.Linq;
using ModKit;
using SolastaCommunityExpansion.Models;

namespace SolastaCommunityExpansion.Displays;

internal static class ItemsAndCraftingDisplay
{
    private const int MAX_COLUMNS = 1;

    private static void AddUIForWeaponKey(string key)
    {
        bool toggle;
        using (UI.HorizontalScope(UI.AutoWidth()))
        {
            UI.ActionButton(ItemCraftingContext.RecipeTitles[key], () => ItemCraftingContext.LearnRecipes(key),
                UI.Width(180));
            UI.Space(20);

            toggle = Main.Settings.CraftingInStore.Contains(key);
            if (UI.Toggle(Gui.Localize("ModUi/&AddToStore"), ref toggle, UI.Width(125)))
            {
                if (toggle)
                {
                    Main.Settings.CraftingInStore.Add(key);
                }
                else
                {
                    Main.Settings.CraftingInStore.Remove(key);
                }

                ItemCraftingContext.AddToStore(key);
            }

            toggle = Main.Settings.CraftingRecipesInDM.Contains(key);
            if (UI.Toggle(Gui.Localize("ModUi/&RecipesInDm"), ref toggle, UI.Width(125)))
            {
                if (toggle)
                {
                    Main.Settings.CraftingRecipesInDM.Add(key);
                }
                else
                {
                    Main.Settings.CraftingRecipesInDM.Remove(key);
                }

                ItemCraftingContext.UpdateCraftingRecipesInDMState(key);
            }

            if (!ItemCraftingContext.BASE_GAME_ITEMS_CATEGORIES.Contains(key))
            {
                toggle = Main.Settings.CraftingItemsInDM.Contains(key);
                if (UI.Toggle(Gui.Localize("ModUi/&ItemInDm"), ref toggle, UI.Width(125)))
                {
                    if (toggle)
                    {
                        Main.Settings.CraftingItemsInDM.Add(key);
                    }
                    else
                    {
                        Main.Settings.CraftingItemsInDM.Remove(key);
                    }

                    ItemCraftingContext.UpdateCraftingItemsInDMState(key);
                }
            }
            else
            {
                UI.Space(128f);
            }
        }
    }

    internal static void DisplayItemsAndCrafting()
    {
        bool toggle;
        int intValue;

        UI.Label("");
        UI.Label(Gui.Localize("ModUi/&General"));
        UI.Label("");

        toggle = Main.Settings.AddNewWeaponsAndRecipesToShops;
        if (UI.Toggle(Gui.Localize(Gui.Localize("ModUi/&AddNewWeaponsAndRecipesToShops")), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.AddNewWeaponsAndRecipesToShops = toggle;
            Main.Settings.AddNewWeaponsAndRecipesToEditor = toggle;
        }

        if (Main.Settings.AddNewWeaponsAndRecipesToShops)
        {
            toggle = Main.Settings.AddNewWeaponsAndRecipesToEditor;
            if (UI.Toggle(Gui.Localize(Gui.Localize("ModUi/&EnableAdditionalItemsInDungeonMaker")), ref toggle,
                    UI.AutoWidth()))
            {
                Main.Settings.AddNewWeaponsAndRecipesToEditor = toggle;
            }
        }

        UI.Label("");

        toggle = Main.Settings.RemoveAttunementRequirements;
        if (UI.Toggle(Gui.Localize("ModUi/&RemoveAttunementRequirements"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.RemoveAttunementRequirements = toggle;
        }

        toggle = Main.Settings.RemoveIdentificationRequirements;
        if (UI.Toggle(Gui.Localize("ModUi/&RemoveIdentifcationRequirements"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.RemoveIdentificationRequirements = toggle;
        }

        UI.Label("");

        intValue = Main.Settings.TotalCraftingTimeModifier;
        if (UI.Slider(Gui.Localize("ModUi/&TotalCraftingTimeModifier"), ref intValue, 0, 100, 0, "%", UI.AutoWidth()))
        {
            Main.Settings.TotalCraftingTimeModifier = intValue;
        }

        UI.Label("");

        toggle = Main.Settings.ShowCraftingRecipeInDetailedTooltips;
        if (UI.Toggle(Gui.Localize("ModUi/&ShowCraftingRecipeInDetailedTooltips"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.ShowCraftingRecipeInDetailedTooltips = toggle;
        }

        UI.Label("");

        intValue = Main.Settings.RecipeCost;
        if (UI.Slider(Gui.Localize("ModUi/&RecipeCost"), ref intValue, 1, 500, 200, "G", UI.AutoWidth()))
        {
            Main.Settings.RecipeCost = intValue;
            ItemCraftingContext.UpdateRecipeCost();
        }

        UI.Label("");

        intValue = Main.Settings.SetBeltOfDwarvenKindBeardChances;
        if (UI.Slider(Gui.Localize("ModUi/&SetBeltOfDwarvenKindBeardChances"), ref intValue,
                0, 100, 50, "%", UI.Width(500)))
        {
            Main.Settings.SetBeltOfDwarvenKindBeardChances = intValue;
            ItemOptionsContext.SwitchSetBeltOfDwarvenKindBeardChances();
        }

        UI.Label("");

        toggle = Main.Settings.DisplayCraftingToggle;
        if (UI.DisclosureToggle(Gui.Localize("ModUi/&Crafting"), ref toggle, 200))
        {
            Main.Settings.DisplayCraftingToggle = toggle;
        }

        if (Main.Settings.DisplayCraftingToggle)
        {
            UI.Label("");
            UI.Label(Gui.Localize("ModUi/&CraftingHelp"));
            UI.Label("");

            using (UI.HorizontalScope(UI.AutoWidth()))
            {
                UI.Space(204);

                toggle = ItemCraftingContext.RecipeBooks.Keys.Count == Main.Settings.CraftingInStore.Count;
                if (UI.Toggle(Gui.Localize("ModUi/&AddAllToStore"), ref toggle, UI.Width(125)))
                {
                    Main.Settings.CraftingInStore.Clear();

                    if (toggle)
                    {
                        Main.Settings.CraftingInStore.AddRange(ItemCraftingContext.RecipeBooks.Keys);
                    }
                }

                toggle = ItemCraftingContext.RecipeBooks.Keys.Count == Main.Settings.CraftingRecipesInDM.Count;
                if (UI.Toggle(Gui.Localize("ModUi/&AllRecipesInDm"), ref toggle, UI.Width(125)))
                {
                    Main.Settings.CraftingRecipesInDM.Clear();

                    if (toggle)
                    {
                        Main.Settings.CraftingRecipesInDM.AddRange(ItemCraftingContext.RecipeBooks.Keys);
                    }
                }

                toggle = ItemCraftingContext.RecipeBooks.Keys.Count == Main.Settings.CraftingItemsInDM.Count;
                if (UI.Toggle(Gui.Localize("ModUi/&AllItemInDm"), ref toggle, UI.Width(125)))
                {
                    Main.Settings.CraftingItemsInDM.Clear();

                    if (toggle)
                    {
                        Main.Settings.CraftingItemsInDM.AddRange(ItemCraftingContext.RecipeBooks.Keys);
                    }
                }
            }

            UI.Label("");

            var keys = ItemCraftingContext.RecipeBooks.Keys;
            var current = 0;
            var count = keys.Count;
            int cols;

            while (current < count)
            {
                cols = 0;

                using (UI.HorizontalScope())
                {
                    while (current < count && cols < MAX_COLUMNS)
                    {
                        AddUIForWeaponKey(keys.ElementAt(current));

                        cols++;
                        current++;
                    }
                }
            }
        }

        UI.Label("");

        toggle = Main.Settings.DisplayMerchantsToggle;
        if (UI.DisclosureToggle(Gui.Localize("ModUi/&Merchants"), ref toggle, 200))
        {
            Main.Settings.DisplayMerchantsToggle = toggle;
        }

        if (Main.Settings.DisplayMerchantsToggle)
        {
            UI.Label("");

            toggle = Main.Settings.StockGorimStoreWithAllNonMagicalClothing;
            if (UI.Toggle(Gui.Localize("ModUi/&StockGorimStoreWithAllNonMagicalClothing"), ref toggle,
                    UI.AutoWidth()))
            {
                Main.Settings.StockGorimStoreWithAllNonMagicalClothing = toggle;
            }

            toggle = Main.Settings.StockHugoStoreWithAdditionalFoci;
            if (UI.Toggle(Gui.Localize("ModUi/&StockHugoStoreWithAdditionalFoci"), ref toggle, UI.AutoWidth()))
            {
                Main.Settings.StockHugoStoreWithAdditionalFoci = toggle;
                Main.Settings.EnableAdditionalFociInDungeonMaker = toggle;
                ItemOptionsContext.SwitchFociItems();
            }

            if (Main.Settings.StockHugoStoreWithAdditionalFoci)
            {
                toggle = Main.Settings.EnableAdditionalFociInDungeonMaker;
                if (UI.Toggle(Gui.Localize("ModUi/&EnableAdditionalItemsInDungeonMaker"), ref toggle, UI.AutoWidth()))
                {
                    Main.Settings.EnableAdditionalFociInDungeonMaker = toggle;
                    ItemOptionsContext.SwitchFociItemsDungeonMaker();
                }
            }

            UI.Label("");
            UI.Label(Gui.Localize("ModUi/&RestockHelp"));
            UI.Label("");

            toggle = Main.Settings.RestockAntiquarians;
            if (UI.Toggle(Gui.Localize("ModUi/&RestockAntiquarians"), ref toggle, UI.AutoWidth()))
            {
                Main.Settings.RestockAntiquarians = toggle;
                ItemOptionsContext.SwitchRestockAntiquarian();
            }

            toggle = Main.Settings.RestockArcaneum;
            if (UI.Toggle(Gui.Localize("ModUi/&RestockArcaneum"), ref toggle, UI.AutoWidth()))
            {
                Main.Settings.RestockArcaneum = toggle;
                ItemOptionsContext.SwitchRestockArcaneum();
            }

            toggle = Main.Settings.RestockCircleOfDanantar;
            if (UI.Toggle(Gui.Localize("ModUi/&RestockCircleOfDanantar"), ref toggle, UI.AutoWidth()))
            {
                Main.Settings.RestockCircleOfDanantar = toggle;
                ItemOptionsContext.SwitchRestockCircleOfDanantar();
            }

            toggle = Main.Settings.RestockTowerOfKnowledge;
            if (UI.Toggle(Gui.Localize("ModUi/&RestockTowerOfKnowledge"), ref toggle, UI.AutoWidth()))
            {
                Main.Settings.RestockTowerOfKnowledge = toggle;
                ItemOptionsContext.SwitchRestockTowerOfKnowledge();
            }
        }

        UI.Label("");
    }
}
