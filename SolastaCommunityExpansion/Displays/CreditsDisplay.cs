using System.Collections.Generic;
using System.IO;
using ModKit;
using static SolastaCommunityExpansion.Displays.PatchesDisplay;
using static SolastaCommunityExpansion.Displays.Shared;

namespace SolastaCommunityExpansion.Displays
{
    internal static class CreditsDisplay
    {
        private static bool displayPatches;

        private static bool IsUnityExplorerInstalled { get; } =
            File.Exists(Path.Combine(Main.MOD_FOLDER, "UnityExplorer.STANDALONE.Mono.dll")) &&
            File.Exists(Path.Combine(Main.MOD_FOLDER, "UniverseLib.Mono.dll"));

        private static bool IsUnityExplorerEnabled { get; set; }

        internal static readonly Dictionary<string, string> CreditsTable = new()
        {
            { "AceHigh", "SoulBlade subclass, Tactician subclass, Power Attack and Reckless Fury feats, no identification" },
            { "Bazou", "Witch class" },
            { "Boofat", "alwaysAlt" },
            { "Burtsev-Alexey", "deep copy algorithm" },
            { "ChrisJohnDigital", "head developer, crafting, faction relations, feats, fighting styles, items, subclasses, progression" },
            { "Dreadmaker", "Forest Guardian subclass" },
            { "DubhHerder", "7th, 8th and 9th level spells, Crafty Feats migration, bug models replacement, additional high level monsters, Warlock class, subclasses" },
            { "ElAntonious", "Arcanist subclass, Torchbearer and Dual Flurry feats" },
            { "Esker", "Warlock class design, quality assurance" },
            { "Holic75", "SolastaModHelpers patches, SolastaExtraContent races, cantrips, spells, sprites" },
            { "ImpPhil", "adv/dis rules, conjurations control, auto-equip, location rotation in DM, monster's health, pause UI, sorting, stocks prices, no attunement, xp scaling, character export, save by location, combat camera, diagnostics, custom icons, refactor, screen map" },
            { "Lyraele", "Warlock class design, quality assurance" },
            { "Narria", "modKit creator, developer" },
            { "Nd", "Opportunist subclass" },
            { "PraiseThyBus", "quality assurance" },
            { "RedOrca", "Path of the Light subclass, Indomitable Might" },
            { "SilverGriffon", "PickPocket, lore friendly names, crafty feats, face unlocks, sylvan armor unlock, empress garb skins, arcane foci items, belt of dwarvenkin, merchants" },
            { "Sinai-dev", "Unity Explorer UI standalone" },
            { "Spacehamster", "dataminer" },
            { "TPABOBAP", "Warlock class design, improvements and refactor, Level Up improvements, Tinkerer improvements, Holic patches " },
            { "View619", "Darkvision, Superior Dark Vision" },
            { "Zappastuff", "multiclass, level 20, respec, level down, encounters, dungeon maker editor & pro, party size, screen gadgets highlights, inventory sorting, epic points, teleport, mod UI, diagnostics, integration, arcane defense/precision, brutal thug, charismatic defense/precision, fast hands, fighting surge, marksman, metamagic, primal, shady and wise defense/precision feats, races and spells migration, warlock pact magic, refactor" }
        };

        internal static void DisplayCredits()
        {
            if (IsUnityExplorerInstalled)
            {
                UI.Label("");
                UI.ActionButton("Enable the Unity Explorer UI", () =>
                {
                    if (!IsUnityExplorerEnabled)
                    {
                        IsUnityExplorerEnabled = true;

                        try
                        {
                            UnityExplorer.ExplorerStandalone.CreateInstance();
                        }
                        catch
                        {

                        }
                    }
                }, UI.Width(200));
            }

            UI.Label("");

            bool toggle = Main.Settings.EnableBetaContent;
            if (UI.Toggle("Enable beta content " + "[keep in mind your heroes or saves might break on future updates] ".yellow().italic() + RequiresRestart, ref toggle, UI.AutoWidth()))
            {
                Main.Settings.EnableBetaContent = toggle;
            }

            UI.Label("");
            UI.DisclosureToggle("Patches:".yellow(), ref displayPatches, 200);

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
}
