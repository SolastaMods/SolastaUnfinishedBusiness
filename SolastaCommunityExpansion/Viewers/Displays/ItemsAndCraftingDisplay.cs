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
            bool toggle = Main.Settings.InStore.Contains(key);
            using (UI.HorizontalScope(UI.Width(350)))
            {
                if (UI.Toggle("Add to store", ref toggle, 0, UI.Width(75)))
                {
                    Main.Settings.InStore.Add(key);
                    ItemCraftingContext.AddToStore(key);
                }
                UI.ActionButton(ItemCraftingContext.RecipeTitles[key], () => ItemCraftingContext.LearnRecipes(key), UI.Width(200));
            }
        }

        internal static void DisplayItemsAndCraftingSettings()
        {
            int intValue = Main.Settings.RecipeCost;

            UI.Label("");
            UI.Label("Settings:".yellow());

            UI.Label("");
            if (UI.Slider("Recipes' Cost".white(), ref intValue, 1, 500, 200, "", UI.AutoWidth()))
            {
                Main.Settings.RecipeCost = intValue;
            }

            UI.Label("");
            UI.Label(". Press the button to learn recipes instantly");
            UI.Label(". Items added to stores might need the party to travel away from the location and come back");
            UI.Label("");

            if (ServiceRepository.GetService<IGameService>()?.Game == null)
            {
                UI.Label("Start a game to display the recipes list...");
            }
            else
            {
                var keys = ItemCraftingContext.RecipeBooks.Keys;
                var current = 0;
                var count = keys.Count;
                int cols;

                while (current < count)
                {
                    cols = 0;

                    using (UI.HorizontalScope())
                    {
                        while (current < count && cols < 3)
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
}