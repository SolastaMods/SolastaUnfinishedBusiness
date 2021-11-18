using System.Linq;
using ModKit;
using SolastaCommunityExpansion.Models;

namespace SolastaCommunityExpansion.Viewers.Displays
{
    internal static class ItemsAndCraftingDisplay
    {
        private static readonly string reqRestart = "[requires restart to disable]".italic().red();

        private static void AddUIForWeaponKey(string key)
        {
            bool toggle;
            using (UI.HorizontalScope(UI.Width(550)))
            {
                UI.ActionButton(ItemCraftingContext.RecipeTitles[key], () => ItemCraftingContext.LearnRecipes(key), UI.Width(175));

                toggle = Main.Settings.InStore.Contains(key);
                if (UI.Toggle("Add to store", ref toggle, 0, UI.Width(75)))
                {
                    if (toggle)
                    {
                        Main.Settings.InStore.Add(key);
                    } else
                    {
                        Main.Settings.InStore.Remove(key);
                    }
                    ItemCraftingContext.AddToStore(key);
                }

                toggle = Main.Settings.ItemsInDM.Contains(key);
                if (UI.Toggle("Items in DM", ref toggle, 0, UI.Width(75)))
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
                if (UI.Toggle("Recipes in DM", ref toggle, 0, UI.Width(75)))
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
            UI.Label("Settings:".yellow());
            UI.Label("");

            toggle = Main.Settings.NoAttunement;
            if (UI.Toggle("Removes attunement requirements " + reqRestart, ref toggle, 0, UI.AutoWidth()))
            {
                Main.Settings.NoAttunement = toggle;
                RemoveIdentificationContext.Load();
            }

            toggle = Main.Settings.NoIdentification;
            if (UI.Toggle("Removes identification requirements " + reqRestart, ref toggle, 0, UI.AutoWidth()))
            {
                Main.Settings.NoIdentification = toggle;
                RemoveIdentificationContext.Load();
            }

            UI.Label("");
            intValue = Main.Settings.RecipeCost;
            if (UI.Slider("Recipes' Cost".white(), ref intValue, 1, 500, 200, "", UI.AutoWidth()))
            {
                Main.Settings.RecipeCost = intValue;
            }

            UI.Label("");
            UI.Label("Crafting:".yellow());
            UI.Label("");

            UI.Label(". Press the button to learn recipes instantly on the active party");
            UI.Label(". Items added to stores might need the party to travel away from the location and come back");
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
    }
}