using System.Collections.Generic;
using ModKit;

namespace SolastaCommunityExpansion.Viewers.Displays
{
    internal static class CreditsDisplay
    {
        internal static readonly Dictionary<string, string> CreditsTable = new()
        {
            { "ChrisJohnDigital", "head developer, crafting, faction relations, feats, fighting styles, items, subclasses, progression" },
            { "Zappastuff", "multiclass, level 20, respec, level down, encounters, dungeon maker editor & pro, party size, screen map, inventory sorting, adventure log, epic points, teleport, mod UI, diagnostics, integration, arcane defense/precision, brutal thug, charismatic defense/precision, fighting surge, metamagic, primal, shady and wise defense/precision feats, races and spells migration" },
            { "ImpPhil", "adv/dis rules, conjurations control, auto-equip, location rotation in DM, monster's health, pause UI, sorting, stocks prices, no attunement, xp scaling, character export, save by location, combat camera, diagnostics, custom icons, refactor, screen map" },
            { "SilverGriffon", "PickPocket, lore friendly names, crafty feats, face unlocks, sylvan armor unlock, empress garb skins, arcane foci items, belt of dwarvenkin, merchants" },
            { "AceHigh", "Warlock class, multiclass idea, Tactician subclass, Power Attack and Reckless Fury feats, no identification" },
            { "DubhHerder", "higher level spells, feats migration, bug models replacement, Warlock subclasses and invocations, Tinkerer Artillerist subclass" },
            { "View619", "Darkvision, Superior Dark Vision" },
            { "Bazou", "Witch class" },
            { "ElAntonious", "Arcanist subclass, Torchbearer and Dual Flurry feats" },
            { "RedOrca", "Path of the Light subclass, Indomitable Might" },
            { "Dreadmaker", "Circle of the Forest Guardian subclass" },
            { "Scarlex", "Royal Knight subclass" },
            { "Krisys", "Thug subclass" },
            { "Narria", "modKit creator, developer" },
            { "Boofat", "alwaysAlt" },
            { "Spacehamster", "dataminer" },
            { "Holic75", "pact magic slots, Warlock class sprite, bolgrif and gnome races, acid claw, air blast burst of radiance, thunder strike, earth tremor and winter's breath spells" },
            { "sinai-dev", "Unity Explorer UI standalone" },
            { "Burtsev-Alexey", "deep copy algorithm" }
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
