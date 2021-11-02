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
            UI.Label(". ChrisJohnDigital".bold() + " - feats, items, subclasses, progression, etc.");

            UI.Label("");
            UI.Label("Credits:".yellow());
            UI.Label("");
            UI.Label(". Zappastuff    - Mod UI Work, Integration, Community Organization");
            UI.Label(". View619       - Darkvision");
            UI.Label(". SilverGriffon - Pickpocket, Solastanomicon");
            UI.Label(". Acehigh       - Power Attack, Reckless Fury");
            UI.Label(". ImpPhil       - Pause on victory, Hide monster health");
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