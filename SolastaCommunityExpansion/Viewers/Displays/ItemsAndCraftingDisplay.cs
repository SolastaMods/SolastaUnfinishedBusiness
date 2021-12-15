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

                toggle = Main.Settings.InStore.Contains(key);
                if (UI.Toggle("Add to store", ref toggle, UI.Width(125)))
                {
                    if (toggle)
                    {
                        Main.Settings.InStore.Add(key);
                    }
                    else
                    {
                        Main.Settings.InStore.Remove(key);
                    }
                    ItemCraftingContext.AddToStore(key);
                }

                toggle = Main.Settings.ItemsInDM.Contains(key);
                if (UI.Toggle("Items in DM", ref toggle, UI.Width(125)))
                {
                    if (toggle)
                    {
                        Main.Settings.ItemsInDM.Add(key);
                    }
                    else
                    {
                        Main.Settings.ItemsInDM.Remove(key);
                    }
                    ItemCraftingContext.UpdateItemsInDMState(key);
                }

                toggle = Main.Settings.RecipesInDM.Contains(key);
                if (UI.Toggle("Recipes in DM", ref toggle, UI.Width(125)))
                {
                    if (toggle)
                    {
                        Main.Settings.RecipesInDM.Add(key);
                    }
                    else
                    {
                        Main.Settings.RecipesInDM.Remove(key);
                    }
                    ItemCraftingContext.UpdateRecipesInDMState(key);
                }
            }
        }

        internal static void DisplayItemsAndCrafting()
        {
            bool toggle;
            int intValue;

            UI.Label("");
            UI.Label("General:".yellow());
            UI.Label("");

            toggle = Main.Settings.NoAttunement;
            if (UI.Toggle("Remove attunement requirements " + RequiresRestart, ref toggle, UI.AutoWidth()))
            {
                Main.Settings.NoAttunement = toggle;
                RemoveIdentificationContext.Load();
            }

            toggle = Main.Settings.NoIdentification;
            if (UI.Toggle("Remove identification requirements " + RequiresRestart, ref toggle, UI.AutoWidth()))
            {
                Main.Settings.NoIdentification = toggle;
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

            intValue = Main.Settings.BeltOfDwarvenKindBeardChances;
            if (UI.Slider("Set the chances of a beard appearing while using the Belt of Dwarvenkin".white(), ref intValue, 0, 100, 50, "%", UI.Width(500)))
            {
                Main.Settings.BeltOfDwarvenKindBeardChances = intValue;
                ItemOptionsContext.SwitchBeltOfDwarvenKindBeardChances();
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

                    toggle = ItemCraftingContext.RecipeBooks.Keys.Count == Main.Settings.InStore.Count;
                    if (UI.Toggle("Add all to store", ref toggle, UI.Width(125)))
                    {
                        Main.Settings.InStore.Clear();

                        if (toggle)
                        {
                            Main.Settings.InStore.AddRange(ItemCraftingContext.RecipeBooks.Keys);
                        }
                    }

                    toggle = ItemCraftingContext.RecipeBooks.Keys.Count == Main.Settings.ItemsInDM.Count;
                    if (UI.Toggle("All items in DM", ref toggle, UI.Width(125)))
                    {
                        Main.Settings.ItemsInDM.Clear();

                        if (toggle)
                        {
                            Main.Settings.ItemsInDM.AddRange(ItemCraftingContext.RecipeBooks.Keys);
                        }
                    }

                    toggle = ItemCraftingContext.RecipeBooks.Keys.Count == Main.Settings.RecipesInDM.Count;
                    if (UI.Toggle("All recipes in DM", ref toggle, UI.Width(125)))
                    {
                        Main.Settings.RecipesInDM.Clear();

                        if (toggle)
                        {
                            Main.Settings.RecipesInDM.AddRange(ItemCraftingContext.RecipeBooks.Keys);
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

                toggle = Main.Settings.EnableClothingGorimStock;
                if (UI.Toggle("Stocks Gorim's store with all non-magical clothing " + RequiresRestart, ref toggle, UI.AutoWidth()))
                {
                    Main.Settings.EnableClothingGorimStock = toggle;
                }

                toggle = Main.Settings.CreateAdditionalFoci;
                if (UI.Toggle("Stocks Hugo's store with new foci items " + "[Arcane Staff / Druid Neck, Staff and Club]".italic().yellow(), ref toggle, UI.AutoWidth()))
                {
                    Main.Settings.CreateAdditionalFoci = toggle;
                    ItemOptionsContext.SwitchFociItems();
                }

                if (Main.Settings.CreateAdditionalFoci)
                {
                    toggle = Main.Settings.EnableAdditionalFociDungeonMaker;
                    if (UI.Toggle("Add new foci items to Dungeon Maker ", ref toggle, UI.AutoWidth()))
                    {
                        Main.Settings.EnableAdditionalFociDungeonMaker = toggle;
                        ItemOptionsContext.SwitchFociItemsDungeonMaker();
                    }
                }

                UI.Label("");
                UI.Label(". Enables all merchant's stock to restock over time except for Manuals and Tomes. Note that some items can take up to 7 game days to restock");
                UI.Label("");

                toggle = Main.Settings.EnableRestockAntiquarians;
                if (UI.Toggle("Restock Antiquarians " + "[Halman Summer]".italic().yellow(), ref toggle, UI.AutoWidth()))
                {
                    Main.Settings.EnableRestockAntiquarians = toggle;
                    ItemOptionsContext.SwitchRestockAntiquarian();
                }

                toggle = Main.Settings.EnableRestockArcaneum;
                if (UI.Toggle("Restock Arcaneum " + "[Heddlon Surespell]".italic().yellow(), ref toggle, UI.AutoWidth()))
                {
                    Main.Settings.EnableRestockArcaneum = toggle;
                    ItemOptionsContext.SwitchRestockArcaneum();
                }

                toggle = Main.Settings.EnableRestockCircleOfDanantar;
                if (UI.Toggle("Restock Circle of Danantar " + "[Joriel Foxeye]".italic().yellow(), ref toggle, UI.AutoWidth()))
                {
                    Main.Settings.EnableRestockCircleOfDanantar = toggle;
                    ItemOptionsContext.SwitchRestockCircleOfDanantar();
                }

                toggle = Main.Settings.EnableRestockTowerOfKnowledge;
                if (UI.Toggle("Restock Tower of Knowledge " + "[Maddy Greenisle]".italic().yellow(), ref toggle, UI.AutoWidth()))
                {
                    Main.Settings.EnableRestockTowerOfKnowledge = toggle;
                    ItemOptionsContext.SwitchRestockTowerOfKnowledge();
                }
            }
        }
    }
}
