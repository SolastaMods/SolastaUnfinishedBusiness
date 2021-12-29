using ModKit;
using SolastaCommunityExpansion.Models;
using System.Linq;
using static SolastaCommunityExpansion.Viewers.Displays.Shared;

namespace SolastaCommunityExpansion.Viewers.Displays
{
    internal static class ItemsAndCraftingDisplay
    {
        private static bool DisplayCrafting { get; set; }
        private static bool DisplayMerchants { get; set; }

        private static void AddUIForWeaponKey(string key)
        {
            bool toggle;
            using (UI.HorizontalScope(UI.AutoWidth()))
            {
                UI.ActionButton(ItemCraftingContext.RecipeTitles[key], () => ItemCraftingContext.LearnRecipes(key), UI.Width(175));

                toggle = Main.Settings.CraftingInStore.Contains(key);
                if (UI.Toggle("Add to store", ref toggle, UI.Width(125)))
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

                //
                // BUG: crafting items and recipes must always be added to DM
                //

                //toggle = Main.Settings.CraftingItemsInDM.Contains(key);
                //if (UI.Toggle("Items in DM", ref toggle, UI.Width(125)))
                //{
                //    if (toggle)
                //    {
                //        Main.Settings.CraftingItemsInDM.Add(key);
                //    }
                //    else
                //    {
                //        Main.Settings.CraftingItemsInDM.Remove(key);
                //    }
                //    ItemCraftingContext.UpdateCraftingItemsInDMState(key);
                //}

                //toggle = Main.Settings.CraftingRecipesInDM.Contains(key);
                //if (UI.Toggle("Recipes in DM", ref toggle, UI.Width(125)))
                //{
                //    if (toggle)
                //    {
                //        Main.Settings.CraftingRecipesInDM.Add(key);
                //    }
                //    else
                //    {
                //        Main.Settings.CraftingRecipesInDM.Remove(key);
                //    }
                //    ItemCraftingContext.UpdateCraftingRecipesInDMState(key);
                //}
            }
        }

        internal static void DisplayItemsAndCrafting()
        {
            bool toggle;
            int intValue;

            UI.Label("");
            UI.Label("General:".yellow());
            UI.Label("");

            toggle = Main.Settings.RemoveAttunementRequirements;
            if (UI.Toggle("Remove attunement requirements " + RequiresRestart, ref toggle, UI.AutoWidth()))
            {
                Main.Settings.RemoveAttunementRequirements = toggle;
                RemoveIdentificationContext.Load();
            }

            toggle = Main.Settings.RemoveIdentifcationRequirements;
            if (UI.Toggle("Remove identification requirements " + RequiresRestart, ref toggle, UI.AutoWidth()))
            {
                Main.Settings.RemoveIdentifcationRequirements = toggle;
                RemoveIdentificationContext.Load();
            }

            UI.Label("");

            intValue = Main.Settings.RecipeCost;
            if (UI.Slider("Recipes' cost".white(), ref intValue, 1, 500, 200, "G", UI.AutoWidth()))
            {
                Main.Settings.RecipeCost = intValue;
                ItemCraftingContext.UpdateRecipeCost();
            }

            UI.Label("");

            intValue = Main.Settings.SetBeltOfDwarvenKindBeardChances;
            if (UI.Slider("Set the chances of a beard appearing while using the ".white() + "Belt of Dwarvenkin".orange(), ref intValue, 0, 100, 50, "%", UI.Width(500)))
            {
                Main.Settings.SetBeltOfDwarvenKindBeardChances = intValue;
                ItemOptionsContext.SwitchSetBeltOfDwarvenKindBeardChances();
            }

            UI.Label("");

            toggle = DisplayCrafting;
            if (UI.DisclosureToggle("Crafting:".yellow(), ref toggle, 200))
            {
                DisplayCrafting = toggle;
            }

            if (DisplayCrafting)
            {
                UI.Label("");
                UI.Label(". Press the button to learn recipes instantly on the active party");
                UI.Label(". Items added to stores might need the party to travel away from the location and come back");
                UI.Label("");

                using (UI.HorizontalScope(UI.AutoWidth()))
                {
                    UI.Space(180);

                    toggle = ItemCraftingContext.RecipeBooks.Keys.Count == Main.Settings.CraftingInStore.Count;
                    if (UI.Toggle("Add all to store", ref toggle, UI.Width(125)))
                    {
                        Main.Settings.CraftingInStore.Clear();

                        if (toggle)
                        {
                            Main.Settings.CraftingInStore.AddRange(ItemCraftingContext.RecipeBooks.Keys);
                        }
                    }

                    //
                    // BUG: crafting items and recipes must always be added to DM
                    //

                    //toggle = ItemCraftingContext.RecipeBooks.Keys.Count == Main.Settings.CraftingItemsInDM.Count;
                    //if (UI.Toggle("All items in DM", ref toggle, UI.Width(125)))
                    //{
                    //    Main.Settings.CraftingItemsInDM.Clear();

                    //    if (toggle)
                    //    {
                    //        Main.Settings.CraftingItemsInDM.AddRange(ItemCraftingContext.RecipeBooks.Keys);
                    //    }
                    //}

                    //toggle = ItemCraftingContext.RecipeBooks.Keys.Count == Main.Settings.CraftingRecipesInDM.Count;
                    //if (UI.Toggle("All recipes in DM", ref toggle, UI.Width(125)))
                    //{
                    //    Main.Settings.CraftingRecipesInDM.Clear();

                    //    if (toggle)
                    //    {
                    //        Main.Settings.CraftingRecipesInDM.AddRange(ItemCraftingContext.RecipeBooks.Keys);
                    //    }
                    //}
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
                        while (current < count && cols < 2)
                        {
                            AddUIForWeaponKey(keys.ElementAt(current));

                            cols++;
                            current++;
                        }
                    }
                }
            }

            UI.Label("");

            toggle = DisplayMerchants;
            if (UI.DisclosureToggle("Merchants:".yellow(), ref toggle, 200))
            {
                DisplayMerchants = toggle;
            }

            if (DisplayMerchants)
            {
                UI.Label("");

                toggle = Main.Settings.StockGorimStoreWithAllNonMagicalClothing;
                if (UI.Toggle("Stocks Gorim's store with all non-magical clothing " + RequiresRestart, ref toggle, UI.AutoWidth()))
                {
                    Main.Settings.StockGorimStoreWithAllNonMagicalClothing = toggle;
                }

                toggle = Main.Settings.StockHugoStoreWithAdditionalFoci;
                if (UI.Toggle("Stocks Hugo's store with new foci items " + "[Arcane Staff, Druid Neck, Staff and Club]".italic().yellow(), ref toggle, UI.AutoWidth()))
                {
                    Main.Settings.StockHugoStoreWithAdditionalFoci = toggle;
                    ItemOptionsContext.SwitchFociItems();
                }

                if (Main.Settings.StockHugoStoreWithAdditionalFoci)
                {
                    toggle = Main.Settings.EnableAdditionalFociInDungeonMaker;
                    if (UI.Toggle("Add new foci items to Dungeon Maker ", ref toggle, UI.AutoWidth()))
                    {
                        Main.Settings.EnableAdditionalFociInDungeonMaker = toggle;
                        ItemOptionsContext.SwitchFociItemsDungeonMaker();
                    }
                }

                UI.Label("");
                UI.Label(". Enables all merchant's stock to restock over time except for Manuals and Tomes. Note that some items can take up to 7 game days to restock");
                UI.Label("");

                toggle = Main.Settings.RestockAntiquarians;
                if (UI.Toggle("Restock Antiquarians " + "[Halman Summer]".italic().yellow(), ref toggle, UI.AutoWidth()))
                {
                    Main.Settings.RestockAntiquarians = toggle;
                    ItemOptionsContext.SwitchRestockAntiquarian();
                }

                toggle = Main.Settings.RestockArcaneum;
                if (UI.Toggle("Restock Arcaneum " + "[Heddlon Surespell]".italic().yellow(), ref toggle, UI.AutoWidth()))
                {
                    Main.Settings.RestockArcaneum = toggle;
                    ItemOptionsContext.SwitchRestockArcaneum();
                }

                toggle = Main.Settings.RestockCircleOfDanantar;
                if (UI.Toggle("Restock Circle of Danantar " + "[Joriel Foxeye]".italic().yellow(), ref toggle, UI.AutoWidth()))
                {
                    Main.Settings.RestockCircleOfDanantar = toggle;
                    ItemOptionsContext.SwitchRestockCircleOfDanantar();
                }

                toggle = Main.Settings.RestockTowerOfKnowledge;
                if (UI.Toggle("Restock Tower of Knowledge " + "[Maddy Greenisle]".italic().yellow(), ref toggle, UI.AutoWidth()))
                {
                    Main.Settings.RestockTowerOfKnowledge = toggle;
                    ItemOptionsContext.SwitchRestockTowerOfKnowledge();
                }
            }
        }
    }
}
