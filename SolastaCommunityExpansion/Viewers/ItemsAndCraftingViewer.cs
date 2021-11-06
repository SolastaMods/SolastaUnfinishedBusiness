using UnityModManagerNet;
using ModKit;

namespace SolastaCommunityExpansion.Viewers
{
    public class ControlItemGroupsMenu : IMenuSelectablePage
    {
        public string Name => "Items and Crafting";

        public int Priority => 4;

        private static readonly string reqRestart = "[requires restart to disable]".italic().red();

        private void AddUIForWeaponKey(string key)
        {
            bool toggle = Main.Settings.InStore.Contains(key);
            using (UI.HorizontalScope())
            {
                UI.ActionButton(Models.ItemCraftingContext.RecipeTitles[key], () => Models.ItemCraftingContext.LearnRecipes(key), UI.Width(200));
                UI.Space(10);
                if (UI.Toggle("Add to store", ref toggle, 0, UI.AutoWidth()))
                {
                    Main.Settings.InStore.Add(key);
                    Models.ItemCraftingContext.AddToStore(key);
                }
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

