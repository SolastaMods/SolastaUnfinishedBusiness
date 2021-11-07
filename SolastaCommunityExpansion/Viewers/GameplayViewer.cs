using UnityModManagerNet;
using ModKit;
using static SolastaCommunityExpansion.Viewers.Displays.GameplayDisplay;
using static SolastaCommunityExpansion.Viewers.Displays.FeatsDisplay;
using static SolastaCommunityExpansion.Viewers.Displays.ItemsAndCraftingDisplay;
using static SolastaCommunityExpansion.Viewers.Displays.SubClassesDisplay;

namespace SolastaCommunityExpansion.Viewers
{
    public class GameplayViewer : IMenuSelectablePage
    {
        public string Name => "Settings";

        public int Priority => 10;

        private static int selectedPane = 0;

        public void OnGUI(UnityModManager.ModEntry modEntry)
        {
            UI.Label("Welcome to Solasta Community Expansion".yellow().bold());
            UI.Div();

            if (Main.Enabled)
            {
                UI.TabBar(ref selectedPane, null, new NamedAction[]
                {
                    new NamedAction("Gameplay", DisplayGameplaySettings),
                    new NamedAction("Feats", DisplayFeatsSettings),
                    new NamedAction("Subclasses", DisplaySubclassesSettings),
                    new NamedAction("Items & Crafting", DisplayItemsAndCraftingSettings),
                });
            }
        }
    }
}