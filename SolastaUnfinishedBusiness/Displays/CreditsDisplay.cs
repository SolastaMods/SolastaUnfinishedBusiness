using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using ModKit;
using SolastaUnfinishedBusiness.Api.Infrastructure;
using UnityExplorer;
using static SolastaUnfinishedBusiness.Displays.PatchesDisplay;

namespace SolastaUnfinishedBusiness.Displays;

internal static class CreditsDisplay
{
    private static bool _displayPatches;

    private static readonly Dictionary<string, string> ThanksTable = new()
    {
        { "Tactical Adventures", "early access to DLC builds and community support" },
        { "JetBrains", "one year Rider IDE subscription for 3 developers" },
        { "", "" },
        { "Critical Hit", "<b>M. Miller</b>, <b>C. Alvarez</b>, <b>J. Cohen</b>, <b>L. Goldiner</b>" },
        { "D20", "D. Fenter, B. Amorsen, B. Lane, J. Loustaunau" },
        { "D12", "E. Antonio, C. Aardappel, M. Klepac" },
        { "D8", "R. Baker, R. Maxim, D. Boggs, P. Marreck" },
        {
            "D6", "M. Brandmaier, F. Lorenz, M. Despard, J. Ball, J. Smedley, J. Bendoski, M. Oliveira, M. Harck\n" +
                  "D. Schoop, K. Cooper, M. Thompson, L. Johnson, M. Piotrowski, E. Meyers, R. Garcia, R. Name\n" +
                  "G. Ruiz, A. Badeaux, S. Braden, E. Gilbert, C. Tontodonati, G. Johnson, J. Batanero, J. Gattis\n" +
                  "J. Lamarre, H. Yes, J. Dileo, L. Barker, N. Zhuxy, M. Arteaga, J. Boyd, C. Badgley, D. Faires\n" +
                  "E. Smith, G. Kinch, A. Searle, R. Hamblin"
        }
    };

    // used in DEBUG mode (don't make private)
    internal static readonly List<(string, string)> CreditsTable = new()
    {
        ("AceHigh", "Tactician subclass"),
        ("Bazou", "fighting styles, spells"),
        ("ChrisJohnDigital", "Tinkerer class, gameplay, feats, fighting styles, subclasses"),
        ("DemonSlayer730", "Rage Mage subclass"),
        ("Dreadmaker", "Forest Guardian subclass"),
        ("DubhHerder", "spells, quality of life"),
        ("ElAntonious", "Arcanist subclass, feats"),
        ("exsonics01", "Oath of Retribution subclass"),
        ("Holic75", "spells"),
        ("ImpPhil", "gameplay, SRD and house rules, quality of life, tools, infrastructure"),
        ("Nd", "Marshal, Opportunist and Raven subclasses"),
        ("RedOrca", "Path of the Light subclass"),
        ("SilverGriffon",
            "gameplay, quality of life, spells, Dark Elf and Grey Dwarf races, Divine Heart subclass"),
        ("TPABOBAP", "gameplay, quality of life, feats, spells, infrastructure"),
        ("Zappastuff",
            "multiclass, gameplay, SRD and house rules, quality of life, tools, infrastructure, feats, Half-elf subraces, Dead Master and Blade Dancer subclasses"),
        ("Esker", "ruleset support, quality assurance"),
        ("Lyraele", "ruleset support, quality assurance"),
        ("PraiseThyBus", "quality assurance"),
        ("Nyowwww", "Chinese translations"),
        ("Prioritizer", "Russian translations"),
        ("Narria", "modKit creator, developer")
    };

    private static readonly bool IsUnityExplorerInstalled =
        File.Exists(Path.Combine(Main.ModFolder, "UnityExplorer.STANDALONE.Mono.dll")) &&
        File.Exists(Path.Combine(Main.ModFolder, "UniverseLib.Mono.dll"));

    private static bool IsUnityExplorerEnabled { get; set; }

    private static void OpenUrl(string url)
    {
        try
        {
            Process.Start(url);
        }
        catch
        {
            // hack because of this: https://github.com/dotnet/corefx/issues/10361
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                url = url.Replace("&", "^&");
                Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Process.Start("xdg-open", url);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                Process.Start("open", url);
            }
            else
            {
                throw;
            }
        }
    }

    internal static void DisplayCredits()
    {
        UI.Label("");

        using (UI.HorizontalScope())
        {
            UI.ActionButton("Donations".Bold().Khaki(), () =>
            {
                OpenUrl(
                    "https://www.paypal.com/donate/?business=JG4FX47DNHQAG&item_name=Support+Solasta+Community+Expansion");
            }, UI.Width(150));

            UI.ActionButton("Wiki".Bold().Khaki(), () =>
            {
                OpenUrl(
                    "https://github.com/SolastaMods/SolastaUnfinishedBusiness/wiki");
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
