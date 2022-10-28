using System.Collections.Generic;
using System.IO;
using SolastaUnfinishedBusiness.Api.Infrastructure;
using SolastaUnfinishedBusiness.Api.ModKit;
using SolastaUnfinishedBusiness.Models;
using UnityExplorer;
using static SolastaUnfinishedBusiness.Displays.PatchesDisplay;

namespace SolastaUnfinishedBusiness.Displays;

internal static class CreditsDisplay
{
    private static bool _displayPatches;

    // ReSharper disable once MemberCanBePrivate.Global
    internal static readonly List<(string, string)> CreditsTable = new()
    {
        ("Zappastuff",
            "maintenance, gameplay, feats, fighting styles, rules, quality of life, Half-elf variants, Dark Kobold, Blade Dancer, Dead Master, Multiclass"),
        ("TPABOBAP", "game UI, infrastructure, gameplay, feats, spells, subclasses improvements, Way of The Distant Hand, Inventor"),
        ("ImpPhil", "api, builders, gameplay, rules, quality of life"),
        ("ChrisJohnDigital",
            "gameplay, feats, fighting styles, Arcane Fighter, Con Artist, Life Transmuter, Master Manipulator, Spell Master, Spell Shield"),
        ("SilverGriffon", "gameplay, visuals, spells, Dark Elf, Draconic Kobold, Grey Dwarf, Divine Heart"),
        ("Nd", "subclasses improvements, Marshal, Opportunist, Raven"),
        ("DubhHerder", "gameplay, spells, Ancient Forest, Elementalist, Moonlit, Rift Walker"),
        ("AceHigh", "SoulBlade, Tactician"),
        ("ElAntonious", "feats, Arcanist"),
        ("Holic75", "spells, Bolgrif, Gnome"),
        ("RedOrca", "Path of The Light"),
        ("DreadMaker", "Circle of The Forest Guardian"),
        ("DemonSlayer730", "Path of The Rage Mage"),
        ("Scarlex", "Royal Knight"),
        ("Bazou", "fighting styles, rules, spells"),
        ("Esker", "ruleset support, qa"),
        ("Lyraele", "ruleset support, qa"),
        ("Nyowwww", "Chinese, qa")
    };

    private static readonly bool IsUnityExplorerInstalled =
        File.Exists(Path.Combine(Main.ModFolder, "UnityExplorer.STANDALONE.Mono.dll")) &&
        File.Exists(Path.Combine(Main.ModFolder, "UniverseLib.Mono.dll"));

    private static bool IsUnityExplorerEnabled { get; set; }

    internal static void DisplayCredits()
    {
        UI.Label("");

        using (UI.HorizontalScope())
        {
            UI.ActionButton("Donate".Bold().Khaki(), BootContext.OpenDonate, UI.Width(150));
            UI.ActionButton("Wiki".Bold().Khaki(), BootContext.OpenWiki, UI.Width(150));

            if (IsUnityExplorerInstalled && !IsUnityExplorerEnabled)
            {
                UI.ActionButton("Unity Explorer UI".Bold().Khaki(), () =>
                {
                    IsUnityExplorerEnabled = true;

                    try
                    {
                        ExplorerStandalone.CreateInstance();
                    }
                    catch
                    {
                        // ignored
                    }
                }, UI.Width(150));
            }
        }

        UI.Label("");
        UI.DisclosureToggle(Gui.Localize("ModUi/&Patches"), ref _displayPatches, 200);
        UI.Label("");

        if (_displayPatches)
        {
            DisplayPatches();
        }
        else
        {
            // credits
            foreach (var (author, content) in CreditsTable)
            {
                using (UI.HorizontalScope())
                {
                    UI.Label(author.Orange(), UI.Width(150));
                    UI.Label(content, UI.Width(900));
                }
            }
        }

        UI.Label("");
    }
}
