using System.Collections.Generic;
using System.IO;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
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
            "maintenance, mod UI, infrastructure, gameplay, feats, fighting styles, invocations, rules, spells, quality of life, Half-elf variants, Circle of The Ancient Forest, Arcane Scoundrel, Blade Dancer, College of Guts, College of Life, Dead Master, Duelist, Field Manipulator, Ranger Wild Master, Slayer, Way of The Discordance, Way of The Silhouette, Multiclass"),
        ("TPABOBAP",
            "custom behaviors, game UI, infrastructure, gameplay, feats, fighting styles, invocations, spells, quality of life, Dead Master, Elementalist, Moonlit, RiftWalker, SoulBlade, Tactician, Way of Distant Hand, Inventor"),
        ("ImpPhil", "api, builders, gameplay, rules, quality of life"),
        ("ChrisJohnDigital", "builders, gameplay, feats, fighting styles, Arcane Fighter, Spell Master, Spell Shield"),
        ("Haxermn", "spells, Defiler Domain, Oath of Ancient, Oath of Hatred, Smith Domain, Way of Dragon"),
        ("Nd", "College of Harlequin, College of War Dancer, Marshal, Opportunist, Raven"),
        ("SilverGriffon", "gameplay, visuals, spells, Dark Elf, Draconic Kobold, Grey Dwarf, Divine Heart"),
        ("DubhHerder", "quality of life, spells, homebrew content design [subclasses]"),
        ("Taco", "homebrew content design [feats, metamagic, subclasses], fighting styles, races, subclasses, powers and weapons icons"),
        ("HiddenHax", "homebrew content design [subclasses]"),
        ("tivie", "Circle of The Night, Path of The Spirits"),
        ("ElAntonius", "feats, fighting styles, Ranger Arcanist"),
        ("RedOrca", "Path of The Light"),
        ("DreadMaker", "Circle of The Forest Guardian"),
        ("Holic75", "spells, Bolgrif, Gnome"),
        ("Bazou", "fighting styles, rules, spells"),
        ("Pikachar2", "spells"),
        ("Narria", "modKit, UI Improvements, Party Editor"),
        ("Balmz", "powers and spells icons")
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
            UI.ActionButton("Unity Explorer UI".Bold().Khaki(), EnableUnityExplorerUi, UI.Width((float)150));
            UI.Label();
        }

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
                    UI.Label(author.Orange(), UI.Width((float)150));
                    UI.Label(content, UI.Width((float)600));
                }
            }
        }

        UI.Label();
    }
}
