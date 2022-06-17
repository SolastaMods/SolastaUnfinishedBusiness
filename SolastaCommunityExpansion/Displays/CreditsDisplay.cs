using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using ModKit;
using UnityExplorer;
using static SolastaCommunityExpansion.Displays.PatchesDisplay;

namespace SolastaCommunityExpansion.Displays;

internal static class CreditsDisplay
{
    private static bool displayPatches;

    internal static readonly Dictionary<string, string> ThanksTable = new()
    {
        {"Tactical Adventures", "early access to DLC builds and community support"},
        {"JetBrains", "one year Rider IDE subscription for 3 developers"},
        {"", ""},
        {"Critical Hit", "<b>J. Cohen</b>, <b>L. Goldiner</b>"},
        {"D20", "D. Fenter, B. Lane, J. Loustaunau"},
        {"D12", "E. Antonio, C. Aardappel"},
        {"D8", "R. Baker, R. Maxim, D. Boggs"},
        {
            "D6", "M. Brandmaier, F. Lorenz, M. Despard, J. Ball, J. Smedley, B. Amorsen, J. Bendoski, M. Oliveira,\n" +
                  "M. Harck, D. Schoop, K. Cooper, M. Thompson"
        }
    };

    internal static readonly Dictionary<string, string> CreditsTable = new()
    {
        {"AceHigh", "SoulBlade subclass, Tactician subclass, feats, no identification"},
        {"Bazou", "Witch class, fighting styles"},
        {"Boofat", "alwaysAlt"},
        {"Burtsev-Alexey", "deep copy algorithm"},
        {
            "ChrisJohnDigital",
            "Tinkerer class, crafting, faction relations, feats, fighting styles, items, subclasses, progression"
        },
        {"Dreadmaker", "Forest Guardian subclass"},
        {
            "DubhHerder",
            "high level spells, feats migration, bug models replacement, high level monsters, Warlock class and subclasses"
        },
        {"ElAntonious", "Arcanist subclass, feats"},
        {"Esker", "Warlock class design, quality assurance"},
        {"Holic75", "SolastaModHelpers, SolastaExtraContent"},
        {
            "ImpPhil",
            "adv/dis rules, conjurations control, auto-equip, monster's health, pause UI, sorting, stocks prices, no attunement, xp scaling, character export, save by location, combat camera, diagnostics, custom icons, refactor, screen map"
        },
        {"Lyraele", "Warlock class design, quality assurance"},
        {"Narria", "modKit creator, developer"},
        {"Nd", "Marshal and Opportunist subclasses"},
        {"Nyowwww", "Chinese translations"},
        {"PraiseThyBus", "quality assurance"},
        {"RedOrca", "Path of the Light subclass, Indomitable Might"},
        {
            "SilverGriffon",
            "PickPocket, lore friendly names, feats, face unlocks, sylvan armor unlock, empress garb skins, arcane foci items, belt of dwarvenkin, merchants, spells, DarkElf race"
        },
        {"Sinai-dev", "Unity Explorer UI standalone"},
        {"Spacehamster", "dataminer"},
        {
            "TPABOBAP",
            "Monk class and subclasses, Warlock improvements, Tinkerer improvements, custom level up, feats, spells, infrastructure patches, Holic75's code integration"
        },
        {"View619", "Darkvision, Superior Dark Vision"},
        {"Vylantze", "English terms review, tweaks, bug fixes"},
        {
            "Zappastuff",
            "repository maintenance, translations, multiclass, level 20, respec, level down, default party, encounters, dungeon maker pro, party size, screen gadgets highlights, inventory sorting, epic points, teleport, mod UI, diagnostics, feats, pact magic, infrastructure patches, Holic75's code integration"
        }
    };

    private static bool IsUnityExplorerInstalled { get; } =
        File.Exists(Path.Combine(Main.MOD_FOLDER, "UnityExplorer.STANDALONE.Mono.dll")) &&
        File.Exists(Path.Combine(Main.MOD_FOLDER, "UniverseLib.Mono.dll"));

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
                Process.Start(new ProcessStartInfo(url) {UseShellExecute = true});
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
            UI.ActionButton("Donations".bold().yellow(), () =>
            {
                OpenUrl(
                    "https://www.paypal.com/donate/?business=JG4FX47DNHQAG&item_name=Support+Solasta+Community+Expansion");
            }, UI.Width(150));

            UI.ActionButton("Wiki".bold().yellow(), () =>
            {
                OpenUrl(
                    "https://github.com/SolastaMods/SolastaCommunityExpansion/wiki");
            }, UI.Width(150));

            if (IsUnityExplorerInstalled)
            {
                UI.ActionButton("Unity Explorer UI".bold().yellow(), () =>
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
                }, UI.Width(150));
            }
        }


#if false
            UI.Label("");

            var toggle = Main.Settings.EnableBetaContent;
            if (UI.Toggle(Gui.Localize("ModUI/&EnableBetaContent"), ref toggle, UI.AutoWidth()))
            {
                Main.Settings.EnableBetaContent = toggle;
            }
#endif

        UI.Label("");
        UI.DisclosureToggle(Gui.Localize("ModUi/&Patches"), ref displayPatches, 200);

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
                UI.Label(kvp.Key.orange(), UI.Width(150));
                UI.Label(kvp.Value, UI.Width(600));
            }
        }

        UI.Label("");

        // credits
        foreach (var kvp in ThanksTable)
        {
            using (UI.HorizontalScope())
            {
                UI.Label(kvp.Key.orange(), UI.Width(150));
                UI.Label(kvp.Value, UI.Width(600));
            }
        }

        UI.Label("");
    }
}
