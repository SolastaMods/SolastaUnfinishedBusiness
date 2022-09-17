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

    private static readonly Dictionary<string, string> ThanksTable = new()
    {
        { "Critical Hit", "<b>M. Miller</b>, <b>C. Alvarez</b>, <b>J. Cohen</b>, <b>L. Goldiner</b>" },
        { "D20", "D. Fenter, B. Amorsen, B. Lane, J. Loustaunau" }
    };

    // ReSharper disable once MemberCanBePrivate.Global
    internal static readonly List<(string, string)> CreditsTable = new()
    {
        //("DemonSlayer730", "Rage Mage subclass"),
        //("RedOrca", "Path of the Light subclass"),
        ("Zappastuff",
            "gameplay, quality of life, infrastructure, feats, Half-elf Variants, Dead Master, Blade Dancer, Multiclass"),
        ("ImpPhil", "gameplay, quality of life, infrastructure"),
        ("TPABOBAP", "gameplay, quality of life, infrastructure, feats, spells"),
        ("ChrisJohnDigital",
            "gameplay, infrastructure, feats, fighting styles, Arcane Fighter, Con Artist, Life Transmuter, Master Manipulator, Spell Master, Spell Shield"),
        ("SilverGriffon", "gameplay, quality of life, spells, Dark Elf, Grey Dwarf, Divine Heart"),
        ("Nd", "Marshal, Opportunist, Raven"),
        ("ElAntonious", "feats, Arcanist"),
        ("Bazou", "fighting styles, spells"),
        ("AceHigh", "Tactician"),
        ("Dreadmaker", "Forest Guardian"),
        ("DubhHerder", "quality of life, spells"),
        ("Holic75", "spells"),
        ("Esker", "ruleset support, quality assurance"),
        ("Lyraele", "ruleset support, quality assurance"),
        ("PraiseThyBus", "quality assurance"),
        ("Nyowwww", "Chinese translations"),
        ("Narria", "modKit")
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
            UI.ActionButton("Donate".Bold().Khaki(), () =>
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
