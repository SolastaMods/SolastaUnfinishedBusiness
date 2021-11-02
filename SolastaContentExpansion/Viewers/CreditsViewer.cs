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
            UI.Label(". ChrisPJohnDigital".bold() + " - feats, items, subclasses, progression, etc.");

            UI.Label("");
            UI.Label("Credits:".yellow());
            UI.Label("");
            UI.Label(". Zappastuff    - mod UI work, integration, community organization");
            UI.Label(". ImpPhil       - monster's health, pause UI after battle");
            UI.Label(". View619       - darkvision / superior dark vision");
            UI.Label(". SilverGriffon - pickpocket, lore friendly names");
            UI.Label(". boofat        - alwaysAlt, faster time scale");
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