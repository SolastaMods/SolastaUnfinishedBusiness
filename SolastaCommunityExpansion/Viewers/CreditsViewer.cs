using UnityModManagerNet;
using ModKit;
using System.Collections.Generic;

namespace SolastaCommunityExpansion.Viewers
{
    public class CreditsViewer : IMenuSelectablePage
    {
        public string Name => "Credits";

        public int Priority => 10;

        private static Dictionary<string, string> creditsTable = new Dictionary<string, string>
        {
            { "Zappastuff", "mod UI work, integration, community organization" },
            { "ImpPhil", "monster's health, pause UI, stocks prices" },
            { "DubhHerder", "crafty feats migration" },
            { "View619", "darkvision, superior dark vision" },
            { "SilverGriffon", "pickpocket, lore friendly names, crafty feats" },
            { "Boofat", "alwaysAlt" },
            { "Myztikrice", "faster time scale" },
            { "AceHigh", "power attack, reckless fury" },
        };

        public void DisplayHelp()
        {
            UI.Label("");
            UI.Label("Author:".yellow());
            UI.Label("");
            UI.Label(". ChrisJohnDigital".bold() + " - feats, items, subclasses, progression, etc.");
            UI.Label("");
            UI.Label("Credits:".yellow());
            UI.Label("");

            foreach (var kvp in creditsTable)
            {
                using (UI.HorizontalScope())
                {
                    UI.Label(kvp.Key.orange(), UI.Width(100));
                    UI.Label(kvp.Value, UI.Width(400));
                }
            }          
            UI.Label("");
        }

        public void OnGUI(UnityModManager.ModEntry modEntry)
        {
            UI.Label("Welcome to Solasta Community Expansion".yellow().bold());
            UI.Div();

            DisplayHelp();
        }
    }
}