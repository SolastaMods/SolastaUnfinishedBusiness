using UnityModManagerNet;
using ModKit;

namespace SolastaContentExpansion.Viewers
{
    public class CreditsViewer : IMenuSelectablePage
    {
        public string Name => "Credits";

        public int Priority => 10;

        public void DisplayHelp()
        {
            UI.Label("");
            UI.Label("Author:".yellow());
            UI.Label("");
            UI.Label(". Chris P. John".bold() + " - feats, items, subclasses, progression, etc.");

            UI.Label("");
            UI.Label("Credits:".yellow());
            UI.Label("");
            UI.Label(". View619       - Darkvision");
            UI.Label(". SilverGriffon - Solastanomicon");
            UI.Label("");
        }

        public void OnGUI(UnityModManager.ModEntry modEntry)
        {
            UI.Label("Welcome to Solasta Content Expansion".yellow().bold());
            UI.Div();

            DisplayHelp();
        }
    }
}