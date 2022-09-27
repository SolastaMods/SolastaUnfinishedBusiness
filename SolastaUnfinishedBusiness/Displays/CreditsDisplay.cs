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
            "maintenance, gameplay, feats, multiclass, rules, quality of life, Half-elf variants, Dead Master, Blade Dancer"),
        ("ImpPhil", "api, builders, gameplay, rules, quality of life"),
        ("TPABOBAP", "gameplay, infrastructure, feats, spells"),
        ("ChrisJohnDigital",
            "gameplay, feats, Arcane Fighter, Con Artist, Life Transmuter, Master Manipulator, Spell Master, Spell Shield"),
        ("SilverGriffon", "gameplay, spells, Dark Elf, Grey Dwarf, Divine Heart"),
        ("DubhHerder", "gameplay, spells, Elementalist, Moonlit, Rift Walker"),
        ("Nd", "Marshal, Opportunist, Raven"),
        ("AceHigh", "SoulBlade, Tactician"),
        ("ElAntonious", "feats, Ranger Arcanist"),
        ("RedOrca", "Path Of The Light"),
        ("Dreadmaker", "Circle Of The Forest Guardian"),
        ("DemonSlayer730", "Path Of The Rage Mage"),
        ("Holic75", "spells, Bolgrif"),
        ("Bazou", "rules, spells"),
        ("Esker", "ruleset support, quality assurance"),
        ("Lyraele", "ruleset support, quality assurance"),
        ("Nyowwww", "Chinese"),
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
                UI.Label(content, UI.Width(650));
            }
        }

        UI.Label("");
    }
}
