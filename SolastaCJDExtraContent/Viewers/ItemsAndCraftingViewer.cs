using UnityModManagerNet;
using ModKit;
using SolastaCJDExtraContent.ItemCrafting;

namespace SolastaCJDExtraContent.Viewers
{
    public class ControlItemGroupsMenu : IMenuSelectablePage
    {
        public string Name => "Item Options";

        public int Priority => 0;



        public void OnGUI(UnityModManager.ModEntry modEntry)
        {
            int intValue = Main.Settings.RecipeCost;
            if (UI.Slider("Cost of Recipes", ref intValue, 1, 500, 200, "", UI.AutoWidth()))
            {
                Main.Settings.RecipeCost = intValue;
            }

            foreach (string key in Models.ItemCraftingContext.RecipeBooks.Keys)
            {
                AddUIForWeaponKey(key);
            }
        }

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
    }
}

