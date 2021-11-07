using UnityModManagerNet;
using ModKit;
using System.Linq;

namespace SolastaCommunityExpansion.Viewers
{
    public class ItemsAndCraftingViewer : IMenuSelectablePage
    {
        public string Name => "Items & Crafting";

        public int Priority => 4;

        private static readonly string reqRestart = "[requires restart to disable]".italic().red();

        private void AddUIForWeaponKey(string key)
        {
            bool toggle = Main.Settings.InStore.Contains(key);
            using (UI.HorizontalScope(UI.Width(350)))
            {
                if (UI.Toggle("Add to store", ref toggle, 0, UI.Width(75)))
                {
                    Main.Settings.InStore.Add(key);
                    Models.ItemCraftingContext.AddToStore(key);
                }
                UI.ActionButton(Models.ItemCraftingContext.RecipeTitles[key], () => Models.ItemCraftingContext.LearnRecipes(key), UI.Width(200));
            }
        }

        public void DisplayRecipesCostSettings()
        {
            int intValue = Main.Settings.RecipeCost;

            UI.Label("");
            UI.Label("Settings:".yellow());

            UI.Label("");

            bool toggle = Main.Settings.NoIdentification;
            if (UI.Toggle("Remove identification requirements " + reqRestart, ref toggle, 0, UI.AutoWidth()))
            {
                Main.Settings.NoIdentification = toggle;
                Models.RemoveIdentificationContext.Load();
            }

            toggle = Main.Settings.NoAttunement;
            if (UI.Toggle("Remove attunement requirements " + reqRestart, ref toggle, 0, UI.AutoWidth()))
            {
                Main.Settings.NoAttunement = toggle;
                Models.RemoveIdentificationContext.Load();
            }

            UI.Label("");
            if (UI.Slider("Recipes' Cost".white(), ref intValue, 1, 500, 200, "", UI.AutoWidth()))
            {
                Main.Settings.RecipeCost = intValue;
            }

            UI.Label("");
            UI.Label(". Press the button to learn recipes instantly");
            UI.Label(". Items added to stores might need the party to travel away from the location and come back");
            UI.Label("");


            var keys = Models.ItemCraftingContext.RecipeBooks.Keys;
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

        public void OnGUI(UnityModManager.ModEntry modEntry)
        {
            UI.Label("Welcome to Solasta Community Expansion".yellow().bold());
            UI.Div();

            if (!Main.Enabled) return;

            DisplayRecipesCostSettings();
        }
    }
}

