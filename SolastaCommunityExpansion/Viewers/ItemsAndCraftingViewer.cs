using UnityModManagerNet;
using ModKit;
using SolastaCommunityExpansion.ItemCrafting;

namespace SolastaCommunityExpansion.Viewers
{
    public class ControlItemGroupsMenu : IMenuSelectablePage
    {
        public string Name => "Items and Crafting";

        public int Priority => 4;

        private void AddUIForWeaponKey(string key)
        {
            bool toggle = Main.Settings.InStore.Contains(key);
            UI.HStack(key, 3,
                () => { UI.ActionButton("Learn Recipes (instant)", () => Models.ItemCraftingContext.LearnRecipes(key), UI.AutoWidth()); },
                () => { UI.Space(25); },
                () =>
                {
                    if (UI.Toggle("In Store (may need travel away and back)", ref toggle, 0, UI.AutoWidth()))
                    {
                        Main.Settings.InStore.Add(key);
                        Models.ItemCraftingContext.AddToStore(key);
                    }
                }
            );
        }

        public void DisplayRecipesCostSettings()
        {
            int intValue = Main.Settings.RecipeCost;

            UI.Label("");
            UI.Label("Settings:".yellow());

            UI.Label("");
            if (UI.Slider("Recipes' Cost".white(), ref intValue, 1, 500, 200, "", UI.AutoWidth()))
            {
                Main.Settings.RecipeCost = intValue;
            }

            foreach (string key in Models.ItemCraftingContext.RecipeBooks.Keys)
            {
                AddUIForWeaponKey(key);
            }
        }



        public void OnGUI(UnityModManager.ModEntry modEntry)
        {
            UI.Label("Welcome to Solasta Community Expansion".yellow().bold());
            UI.Div();

            if (!Main.Enabled) return;

            DisplayRecipesCostSettings();
        }
    }
}

