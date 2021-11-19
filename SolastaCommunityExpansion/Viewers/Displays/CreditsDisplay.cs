using ModKit;
using System.Collections.Generic;

namespace SolastaCommunityExpansion.Viewers.Displays
{
    internal static class CreditsDisplay
    {
        internal static Dictionary<string, string> CreditsTable = new Dictionary<string, string>
        {
            { "Zappastuff", "mod UI work, integration, community organization, level 20, respec" },
            { "ImpPhil", "monster's health, pause UI, stocks prices, no attunement, xp scaling" },
            { "DubhHerder", "crafty feats migration" },
            { "View619", "darkvision, superior dark vision" },
            { "SilverGriffon", "pickpocket, lore friendly names, crafty feats" },
            { "Boofat", "alwaysAlt" },
            { "Myztikrice", "faster time scale" },
            { "AceHigh", "power attack, reckless fury, no identification, Tactician Subclass" },
            { "ElAntonious", "Torchbearer and Dual Flurry, Arcanist Subclass" },
            { "Narria", "ModKit creator, developer" }
        };

        internal static void DisplayCredits()
        {
            UI.Div();
            UI.Label("");
            UI.Label("Credits:".yellow());
            UI.Label("");
            using (UI.HorizontalScope())
            {
                UI.Label("ChrisJohnDigital".orange().bold(), UI.Width(110));
                UI.Label("head developer, feats, items, subclasses, progression, etc.", UI.Width(400));
            }
            foreach (var kvp in CreditsTable)
            {
                using (UI.HorizontalScope())
                {
                    UI.Label(kvp.Key.orange(), UI.Width(110));
                    UI.Label(kvp.Value, UI.Width(400));
                }
            }
            UI.Label("");
        }
    }
}
