using UnityModManagerNet;
using ModKit;
using static SolastaCommunityExpansion.Viewers.Displays.ItemsAndCraftingDisplay;
using static SolastaCommunityExpansion.Viewers.Displays.RulesDisplay;
using static SolastaCommunityExpansion.Viewers.Displays.ToolsDisplay;

namespace SolastaCommunityExpansion.Viewers
{
    public class GameplayViewer : IMenuSelectablePage
    {
        public string Name => "Gameplay";

        public int Priority => 20;

        private static int selectedPane = 0;

        public void OnGUI(UnityModManager.ModEntry modEntry)
        {
            UI.Label("Welcome to Solasta Community Expansion".yellow().bold());
            UI.Div();

            if (Main.Enabled)
            {
                UI.TabBar(ref selectedPane, null, new NamedAction[]
                {
                    new NamedAction("Rules", DisplayRules),
                    new NamedAction("Items & Crafting", DisplayItemsAndCrafting),
                    new NamedAction("Tools", DisplayTools),
                });
            }
        }
    }
}