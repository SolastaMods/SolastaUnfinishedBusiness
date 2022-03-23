using System.Collections.Generic;
using ModKit;

namespace SolastaCommunityExpansion.Viewers.Displays
{
    internal static class CreditsDisplay
    {
        internal static readonly Dictionary<string, string> CreditsTable = new()
        {
            { "ChrisJohnDigital".orange().bold(), "head developer, crafting, faction relations, feats, fighting styles, items, subclasses, progression" },
            { "Zappastuff", "level 20, respec, level down, encounters, dungeon maker editor & pro, party size, screen map, inventory sorting, adventure log, epic points, teleport, mod UI, diagnostics, integration, charismatic defense, fighting surge, metamagic, primal, shady and wise defense feats" },
            { "ImpPhil", "adv/dis rules, conjurations control, auto-equip, location rotation in DM, monster's health, pause UI, sorting, stocks prices, no attunement, xp scaling, character export, save by location, combat camera, diagnostics, custom icons, refactor, screen map" },
            { "DubhHerder", "higher level spells, crafty feats migration, bug models replacement, higher level monsters, subclasses" },
            { "View619", "darkvision, superior dark vision" },
            { "SilverGriffon", "pickpocket feat and crafty feats, lore friendly names, face unlocks, sylvan armor unlock, empress garb skins, arcane foci items, belt of dwarvenkin, merchants" },
            { "Bazou", "Witch class" },
            { "Boofat", "alwaysAlt" },
            { "AceHigh", "Tactician subclass, power attack and reckless fury feats, no identification" },
            { "ElAntonious", "Arcanist subclass, torchbearer and dual flurry feats" },
            { "Scarlex", "Royal Knight subclass" },
            { "RedOrca", "Path of the Light subclass, indomitable might feature" },
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
