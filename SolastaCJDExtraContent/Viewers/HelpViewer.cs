using UnityModManagerNet;
using ModKit;

namespace SolastaCJDExtraContent.Menus.Viewers
{
    public class HelpViewer : IMenuSelectablePage
    {
        public string Name => "Help";

        public int Priority => 0;

        public void DisplayHelp()
        {

        }

        public void OnGUI(UnityModManager.ModEntry modEntry)
        {
            UI.Label("Welcome to Solasta Content Expansion".yellow().bold());
            UI.Div();

            DisplayHelp();
        }
    }
}