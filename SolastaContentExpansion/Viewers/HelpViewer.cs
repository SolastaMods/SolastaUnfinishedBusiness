using UnityModManagerNet;
using ModKit;

namespace SolastaContentExpansion.Viewers
{
    public class HelpViewer : IMenuSelectablePage
    {
        public string Name => "Help";

        public int Priority => 0;

        public void DisplayHelp()
        {
            UI.Label("");
            UI.Label("Author:".yellow());
            UI.Label("");
            UI.Label(". Chris P. John".bold() + " - feats, items, subclasses, progression, etc.");

            UI.Label("");
            UI.Label("Credits:".yellow());
            UI.Label("");
            UI.Label(". View619       - Darkvision / COD");
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