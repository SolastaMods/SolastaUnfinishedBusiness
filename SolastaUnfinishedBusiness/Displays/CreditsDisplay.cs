using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using SolastaUnfinishedBusiness.Api.Infrastructure;
using SolastaUnfinishedBusiness.Api.ModKit;
using SolastaUnfinishedBusiness.Models;
using UnityExplorer;
using static SolastaUnfinishedBusiness.Displays.PatchesDisplay;

namespace SolastaUnfinishedBusiness.Displays;

internal static class CreditsDisplay
{
    private static bool _displayPatches;

    private static readonly Dictionary<string, string> ThanksTable = new()
    {
        { "Critical Hit", "<b>M. Miller</b>, <b>C. Alvarez</b>, <b>J. Cohen</b>, <b>L. Goldiner</b>" },
        { "D20", "D. Fenter, B. Amorsen, B. Lane, J. Loustaunau" }
    };

    // used in DEBUG mode (don't make private)
    internal static readonly List<(string, string)> CreditsTable = new()
    {
        ("AceHigh", "gameplay, Tactician subclass"),
        ("Bazou", "fighting styles, spells"),
        ("ChrisJohnDigital", "gameplay, feats, fighting styles, spells, subclasses"),
        ("DemonSlayer730", "Rage Mage subclass"),
        ("Dreadmaker", "Forest Guardian subclass"),
        ("DubhHerder", "quality of life, spells"),
        ("ElAntonious", "feats, Arcanist subclass"),
        ("Holic75", "spells"),
        ("ImpPhil", "gameplay, quality of life, infrastructure"),
        ("Nd", "Marshal, Opportunist and Raven subclasses"),
        //("RedOrca", "Path of the Light subclass"),
        ("SilverGriffon",
            "gameplay, quality of life, spells, Dark Elf and Grey Dwarf races, Divine Heart subclass"),
        ("TPABOBAP", "gameplay, quality of life, infrastructure, feats, spells"),
        ("Zappastuff",
            "multiclass, gameplay, quality of life, infrastructure, feats, Half-elf subraces, Dead Master and Blade Dancer subclasses"),
        ("Esker", "ruleset support, quality assurance"),
        ("Lyraele", "ruleset support, quality assurance"),
        ("PraiseThyBus", "quality assurance"),
        ("Nyowwww", "Chinese translations"),
        ("Narria", "modKit creator, developer")
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
            UI.ActionButton("Donations".Bold().Khaki(), () =>
            {
                BootContext.OpenUrl(BootContext.DonateUrl);
            }, UI.Width(150));

            UI.ActionButton("Wiki".Bold().Khaki(), () =>
            {
                BootContext.OpenUrl("https://github.com/SolastaMods/SolastaUnfinishedBusiness/wiki");
            }, UI.Width(150));

            if (!IsUnityExplorerEnabled && IsUnityExplorerInstalled)
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

        // credits
        foreach (var (author, content) in CreditsTable)
        {
            using (UI.HorizontalScope())
            {
                UI.Label(author.Orange(), UI.Width(150));
                UI.Label(content, UI.Width(600));
            }
        }

        UI.Label("");

        // credits
        foreach (var kvp in ThanksTable)
        {
            using (UI.HorizontalScope())
            {
                UI.Label(kvp.Key.Orange(), UI.Width(150));
                UI.Label(kvp.Value, UI.Width(600));
            }
        }

        UI.Label("");
    }
}
