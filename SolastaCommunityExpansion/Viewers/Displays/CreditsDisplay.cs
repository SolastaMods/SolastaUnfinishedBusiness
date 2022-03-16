using System.Collections.Generic;
using ModKit;

namespace SolastaCommunityExpansion.Viewers.Displays
{
    internal static class CreditsDisplay
    {
        internal static readonly Dictionary<string, string> CreditsTable = new()
        {
            { "ChrisJohnDigital".orange().bold(), "head developer, crafting, faction relations, feats, fighting styles, items, subclasses, progression" },
            { "Zappastuff", "multiclass, level 20, respec, encounters, dungeon maker editor & pro, inventory sorting, adventure log, epic points, surprise rules, teleport, mod UI & integration, fighting surge, practiced expert, primal and shady feats" },
            { "ImpPhil", "adv/dis rules, conjurations control, auto-equip, location rotation in DM, monster's health, pause UI, sorting, stocks prices, no attunement, xp scaling, character export, save by location, combat camera, diagnostics" },
            { "DubhHerder", "High level spells, Crafty Feats migration, bug models replacement" },
            { "View619", "Darkvision, Superior Dark Vision" },
            { "SilverGriffon", "PickPocket, lore friendly names, crafty feats, face unlocks, sylvan armor unlock, empress garb skins, arcane foci items, belt of dwarvenkin, merchants" },
            { "Bazou", "Witch class" },
            { "Boofat", "alwaysAlt" },
            { "AceHigh", "Tactician subclass, Power Attack and Reckless Fury feats, no identification" },
            { "ElAntonious", "Arcanist subclass, Torchbearer and Dual Flurry feats" },
            { "Scarlex", "Royal Knight subclass" },
            { "RedOrca", "Path of the Light subclass, Indomitable Might" },
            { "Krisys", "Thug subclass" },
            { "Dreadmaker", "Circle of the Forest Guardian subclass" },
            { "Narria", "modKit creator, developer" },
            { "Spacehamster", "dataminer" },
            { "sinai-dev", "Unity Explorer UI standalone" }
        };

        internal static void DisplayCredits()
        {
            UI.Label("");

            foreach (var kvp in CreditsTable)
            {
                using (UI.HorizontalScope())
                {
                    UI.Label(kvp.Key.orange(), UI.Width(110));
                    UI.Label(kvp.Value, UI.Width(500));
                }
            }
            UI.Label("");
        }
    }
}
