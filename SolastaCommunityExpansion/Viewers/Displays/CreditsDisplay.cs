using ModKit;
using System.Collections.Generic;

namespace SolastaCommunityExpansion.Viewers.Displays
{
    internal static class CreditsDisplay
    {
        internal static readonly Dictionary<string, string> CreditsTable = new Dictionary<string, string>
        {
            { "ChrisJohnDigital".orange().bold(), "head developer, feats, items, subclasses, progression, etc." },
            { "Zappastuff", "mod UI work, integration, community organization, level 20, respec" },
            { "ImpPhil", "monster's health, pause UI, stocks prices, no attunement, xp scaling" },
            { "DubhHerder", "Crafty Feats Migration" },
            { "View619", "Darkvision, Superior Dark Vision" },
            { "SilverGriffon", "PickPocket, lore friendly names, crafty feats, face unlocks" },
            { "Boofat", "alwaysAlt" },
            { "Myztikrice", "faster time scale" },
            { "AceHigh", "Power Attack, Reckless Fury, no identification, Tactician Subclass" },
            { "ElAntonious", "Torchbearer and Dual Flurry, Arcanist Subclass" },
            { "Scarlex", "Royal Knight Subclass" },
            { "RedOrca", "Path of Light Subclass" },
            { "Narria", "ModKit creator, developer" }
        };

        internal static void DisplayCredits()
        {
            UI.Div();
            UI.Label("");
            UI.Label("Credits:".yellow());
            UI.Label("");

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
