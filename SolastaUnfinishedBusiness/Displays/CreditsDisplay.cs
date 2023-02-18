using System.Collections.Generic;
using System.IO;
using SolastaUnfinishedBusiness.Api.Infrastructure;
using SolastaUnfinishedBusiness.Api.ModKit;
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
            "maintenance, mod UI, infrastructure, gameplay, feats, fighting styles, invocations, rules, spells, quality of life, Half-elf variants, Blade Dancer, Ancient Forest, College of Guts, College of Life, Dead Master, Field Manipulator, Way of The Discordance, Way of The Silhouette, Wild Master, Multiclass"),
        ("TPABOBAP",
            "game UI, infrastructure, gameplay, feats, invocations, spells, quality of life, Dead Master, Elementalist, Moonlit, RiftWalker, SoulBlade, Tactician, Way of The Distant Hand, Inventor"),
        ("Nd", "College of Harlequin, College of War Dancer, Marshal, Opportunist, Raven, Spell Shield"),
        ("ImpPhil", "api, builders, gameplay, rules, quality of life"),
        ("ChrisJohnDigital",
            "builders, gameplay, feats, fighting styles, original Tinkerer, original Wizard subclasses, Arcane Fighter, Spell Master, Spell Shield"),
        ("SilverGriffon", "gameplay, visuals, spells, Dark Elf, Draconic Kobold, Grey Dwarf, Divine Heart"),
        ("Haxermn", "spells, Oath of Ancient, Oath of Hatred, Way of The Dragon"),
        ("tivie", "Circle of The Night, Path of The Spirits"),
        ("ElAntonius", "feats, fighting styles, Arcanist"),
        ("DubhHerder", "quality of life, spells, original Warlock subclasses"),
        ("Holic75", "spells, Bolgrif, Gnome"),
        ("RedOrca", "Path of The Light"),
        ("DreadMaker", "Forest Guardian"),
        ("Bazou", "fighting styles, rules, spells"),
        ("Pikachar2", "spells"),
        ("Prioritizer", "Russian translations"),
        ("xxy961216", "Chinese translations"),
        ("Taco", "homebrew content design [feats, metamagic, subclasses], power and weapon icons"),
        ("HiddenHax", "homebrew content design [subclasses]"),
        ("Balmz", "some power and spell icons")
    };

    private static readonly bool IsUnityExplorerInstalled =
        File.Exists(Path.Combine(Main.ModFolder, "UnityExplorer.STANDALONE.Mono.dll")) &&
        File.Exists(Path.Combine(Main.ModFolder, "UniverseLib.Mono.dll"));

    private static bool IsUnityExplorerEnabled { get; set; }

    private static void EnableUnityExplorerUi()
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
    }

    internal static void DisplayCredits()
    {
        UI.Label();

        if (IsUnityExplorerInstalled && !IsUnityExplorerEnabled)
        {
            UI.ActionButton("Unity Explorer UI".Bold().Khaki(), EnableUnityExplorerUi, UI.Width(150));
        }

        UI.Label();
        UI.DisclosureToggle(Gui.Localize("ModUi/&Patches"), ref _displayPatches, 200);
        UI.Label();

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
                    UI.Label(content, UI.Width(600));
                }
            }
        }

        UI.Label();
    }
}
