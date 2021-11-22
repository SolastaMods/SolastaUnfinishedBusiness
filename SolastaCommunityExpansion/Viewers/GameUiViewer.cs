using ModKit;
using UnityModManagerNet;
using static SolastaCommunityExpansion.Viewers.Displays.GameUiDisplay;

namespace SolastaCommunityExpansion.Viewers
{
    public class GameUiViewer : IMenuSelectablePage
    {
        public string Name => "Game UI";

        public int Priority => 30;

        public void OnGUI(UnityModManager.ModEntry modEntry)
        {
            UI.Label("Welcome to Solasta Community Expansion".yellow().bold());
            UI.Div();

            if (Main.Enabled)
            {
                DisplayGameUi();
            }
        }
    }
}
