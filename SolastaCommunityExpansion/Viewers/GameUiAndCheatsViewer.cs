using UnityModManagerNet;
using ModKit;
using static SolastaCommunityExpansion.Viewers.Displays.GameUIAndCheatsDisplay;

namespace SolastaCommunityExpansion.Viewers
{
    public class GameUiAndCheatsViewer : IMenuSelectablePage
    {
        public string Name => "Game UI & Cheats";

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
                    new NamedAction("Game UI", DisplayGameUiSettings),
                    new NamedAction("Cheats", DisplayCheats),
                });
            }
        }
    }
}
