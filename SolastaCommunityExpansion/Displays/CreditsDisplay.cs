using System.Collections.Generic;
using System.IO;
using ModKit;
using UnityExplorer;
using static SolastaCommunityExpansion.Displays.PatchesDisplay;

namespace SolastaCommunityExpansion.Displays;

internal static class CreditsDisplay
{
    private static bool displayPatches;

    internal static readonly Dictionary<string, string> CreditsTable = new()
    {
        {
            "AceHigh", "SoulBlade subclass, Tactician subclass, Power Attack and Reckless Fury feats, no identification"
        },
        {"Bazou", "Witch class, Fighting Styles"},
        {"Boofat", "alwaysAlt"},
        {"Burtsev-Alexey", "deep copy algorithm"},
        {
            "ChrisJohnDigital",
            "Tinkerer class, crafting, faction relations, feats, fighting styles, items, subclasses, progression"
        },
        {"Dreadmaker", "Forest Guardian subclass"},
        {
            "DubhHerder",
            "7th, 8th and 9th level spells, Crafty Feats migration, bug models replacement, additional high level monsters, Warlock class and subclasses"
        },
        {"ElAntonious", "Arcanist subclass, Torchbearer and Dual Flurry feats"},
        {"Esker", "Warlock class design, quality assurance"},
        {"Holic75", "SolastaModHelpers and SolastaExtraContent"},
        {
            "ImpPhil",
            "adv/dis rules, conjurations control, auto-equip, monster's health, pause UI, sorting, stocks prices, no attunement, xp scaling, character export, save by location, combat camera, diagnostics, custom icons, refactor, screen map"
        },
        {"Lyraele", "Warlock class design, quality assurance"},
        {"Narria", "modKit creator, developer"},
        {"Nd", "Marshal subclass, Opportunist subclass"},
        {"Nyowwww", "Chinese translations"},
        {"PraiseThyBus", "quality assurance"},
        {"RedOrca", "Path of the Light subclass, Indomitable Might"},
        {
            "SilverGriffon",
            "PickPocket, lore friendly names, crafty feats, face unlocks, sylvan armor unlock, empress garb skins, arcane foci items, belt of dwarvenkin, merchants, spells"
        },
        {"Sinai-dev", "Unity Explorer UI standalone"},
        {"Spacehamster", "dataminer"},
        {
            "TPABOBAP",
            "Monk class and subclasses, Warlock improvements, Tinkerer improvements, Level Up improvements, Sentinel Feat, Spells, Infrastructure patches, Holic75's code integration"
        },
        {"View619", "Darkvision, Superior Dark Vision"},
        {
            "Zappastuff",
            "repository maintenance, multiclass, level 20, respec, level down, default party, encounters, dungeon maker pro, party size, screen gadgets highlights, inventory sorting, epic points, teleport, mod UI, diagnostics, feats, pact magic, Holic75's code integration"
        }
    };

    private static bool IsUnityExplorerInstalled { get; } =
        File.Exists(Path.Combine(Main.MOD_FOLDER, "UnityExplorer.STANDALONE.Mono.dll")) &&
        File.Exists(Path.Combine(Main.MOD_FOLDER, "UniverseLib.Mono.dll"));

    private static bool IsUnityExplorerEnabled { get; set; }

    internal static void DisplayCredits()
    {
        if (IsUnityExplorerInstalled)
        {
            UI.Label("");
            UI.ActionButton(Gui.Format("ModUi/&EnableUnityExplorer"), () =>
            {
                if (!IsUnityExplorerEnabled)
                {
                    IsUnityExplorerEnabled = true;

                    try
                    {
                        ExplorerStandalone.CreateInstance();
                    }
                    catch
                    {
                    }
                }
            }, UI.AutoWidth());
        }

#if false
            UI.Label("");

            var toggle = Main.Settings.EnableBetaContent;
            if (UI.Toggle(Gui.Format("ModUI/&EnableBetaContent"), ref toggle, UI.AutoWidth()))
            {
                Main.Settings.EnableBetaContent = toggle;
            }
#endif

        UI.Label("");
        UI.DisclosureToggle(Gui.Format("ModUi/&Patches"), ref displayPatches, 200);

        UI.Label("");

        if (displayPatches)
        {
            DisplayPatches();
        }

        // credits
        foreach (var kvp in CreditsTable)
        {
            using (UI.HorizontalScope())
            {
                UI.Label(kvp.Key.orange(), UI.Width(120));
                UI.Label(kvp.Value, UI.Width(600));
            }
        }

        UI.Label("");
    }
}
