using System.Diagnostics;
using System.IO;
using System.Text;
using ModKit;
using SolastaCommunityExpansion.DataMiner;
using SolastaCommunityExpansion.Models;
using SolastaCommunityExpansion.Patches.Diagnostic;
using static SolastaCommunityExpansion.Viewers.Displays.CreditsDisplay;

namespace SolastaCommunityExpansion.Viewers.Displays
{
    internal static class DiagnosticsDisplay
    {
        private static bool IsUnityExplorerEnabled { get; set; }

        internal static void DisplayModdingTools()
        {
            UI.Label("");

            using (UI.HorizontalScope())
            {
                UI.ActionButton("Enable the Unity Explorer UI", () =>
                {
                    if (!IsUnityExplorerEnabled)
                    {
                        IsUnityExplorerEnabled = true;
                        UnityExplorer.ExplorerStandalone.CreateInstance();
                    }
                }, UI.Width(200));
#if DEBUG
                DisplayDumpDescription();
#endif
            }

            UI.Label("");
            UI.Label(". You can set the environment variable " + DiagnosticsContext.ProjectEnvironmentVariable.italic().yellow() + " to customize the output folder");

            if (DiagnosticsContext.ProjectFolder == null)
            {
                UI.Label(". The output folder is set to " + "your game folder".yellow().bold());
            }
            else
            {
                UI.Label(". The output folder is set to " + DiagnosticsContext.DiagnosticsFolder.yellow().bold());
            }

            UI.Label("");

            string exportTaLabel;
            string exportTaLabel2;
            string exportCeLabel;
            float percentageCompleteTa = BlueprintExporter.CurrentExports[DiagnosticsContext.TA].percentageComplete;
            float percentageCompleteTa2 = BlueprintExporter.CurrentExports[DiagnosticsContext.TA2].percentageComplete;
            float percentageCompleteCe = BlueprintExporter.CurrentExports[DiagnosticsContext.CE].percentageComplete;

            if (percentageCompleteTa == 0)
            {
                exportTaLabel = "Export TA blueprints";
            }
            else
            {
                exportTaLabel = "Cancel TA export at " + $"{percentageCompleteTa:00.0%}".bold().yellow();
            }

            if (percentageCompleteTa2 == 0)
            {
                exportTaLabel2 = "Export TA blueprints (modded)";
            }
            else
            {
                exportTaLabel2 = "Cancel TA export at " + $"{percentageCompleteTa2:00.0%}".bold().yellow();
            }

            if (percentageCompleteCe == 0)
            {
                exportCeLabel = "Export CE blueprints";
            }
            else
            {
                exportCeLabel = "Cancel CE export at " + $"{percentageCompleteCe:00.0%}".bold().yellow();
            }

            using (UI.HorizontalScope())
            {
                UI.ActionButton(exportTaLabel, () =>
                {
                    if (percentageCompleteTa == 0)
                    {
                        DiagnosticsContext.ExportTADefinitions();
                    }
                    else
                    {
                        BlueprintExporter.Cancel(DiagnosticsContext.TA);
                    }
                }, UI.Width(200));

                UI.ActionButton(exportCeLabel, () =>
                {
                    if (percentageCompleteCe == 0)
                    {
                        DiagnosticsContext.ExportCEDefinitions();
                    }
                    else
                    {
                        BlueprintExporter.Cancel(DiagnosticsContext.CE);
                    }
                }, UI.Width(200));
#if DEBUG
                UI.ActionButton(exportTaLabel2, () =>
                {
                    if (percentageCompleteTa2 == 0)
                    {
                        DiagnosticsContext.ExportTADefinitionsAfterCELoaded();
                    }
                    else
                    {
                        BlueprintExporter.Cancel(DiagnosticsContext.TA2);
                    }
                }, UI.Width(200));
#endif
            }
#if DEBUG
            using (UI.HorizontalScope())
            {
                UI.ActionButton("Create TA diagnostics", () => DiagnosticsContext.CreateTADefinitionDiagnostics(), UI.Width(200));
                UI.ActionButton("Create CE diagnostics", () => DiagnosticsContext.CreateCEDefinitionDiagnostics(), UI.Width(200));
            }

            UI.Label("");

            bool logVariantMisuse = Main.Settings.DebugLogVariantMisuse;

            if (UI.Toggle("Log misuse of EffectForm and ItemDefinition " + Shared.RequiresRestart, ref logVariantMisuse))
            {
                Main.Settings.DebugLogVariantMisuse = logVariantMisuse;
            }

            ItemDefinitionVerification.Mode = Main.Settings.DebugLogVariantMisuse ? ItemDefinitionVerification.Verification.Log : ItemDefinitionVerification.Verification.None;
            EffectFormVerification.Mode = Main.Settings.DebugLogVariantMisuse ? EffectFormVerification.Verification.Log : EffectFormVerification.Verification.None;
#endif
            UI.Label("");
        }

#if DEBUG

        private const string ModDescription = @"
[size=5][b][i]Solasta Community Expansion[/i][/b][/size]

This is a collection of work from the Solasta modding community. It includes races, classes, subclasses, feats, fighting styles, items, crafting recipes, gameplay options, UI improvements, and more. The general philosophy is everything is optional to enable, so you can install the mod and then enable the pieces you want. There are some minor bug fixes that are enabled by default.

[size=4][b]Credits[/b][/size]

{0}

[url=https://github.com/SolastaMods/SolastaCommunityExpansion]Source Code (MIT License)[/url]

[size=4][b]How to Report Bugs[/b][/size]

[list]
[*] The versions of Solasta and Solasta Community Expansion.
[*] A list of other mods you have installed.
[*] A short description of the bug.
[*] A step-by-step procedure to reproduce it.
[*] The save, character and log files.
[/list]

[size=4][b]Features[/b][/size]

All settings start disabled by default. On first start the mod will display an welcome message and open the UMM Mod UI settings again. Multiplayer support is still in beta. We recommend all players to share the same mod [b]Settings.xml[/b] file before a session.

[img]https://github.com/SolastaMods/SolastaCommunityExpansion/blob/Beta/Media/01-Character-General.png?raw=true[/img]
[line]
[img]https://github.com/SolastaMods/SolastaCommunityExpansion/blob/Beta/Media/02-Character-ClassesSubclasses.png?raw=true[/img]
[line]
[img]https://github.com/SolastaMods/SolastaCommunityExpansion/blob/Beta/Media/03-Character-FeatsFightingStyles.png?raw=true[/img]
[line]
[img]https://github.com/SolastaMods/SolastaCommunityExpansion/blob/Beta/Media/04-Character-Spells.png?raw=true[/img]
[line]
[img]https://github.com/SolastaMods/SolastaCommunityExpansion/blob/Beta/Media/05-Gameplay-Rules.png?raw=true[/img]
[line]
[img]https://github.com/SolastaMods/SolastaCommunityExpansion/blob/Beta/Media/06-Gameplay-CampaignsLocations.png?raw=true[/img]
[line]
[img]https://github.com/SolastaMods/SolastaCommunityExpansion/blob/Beta/Media/07-Gameplay-ItemsCraftingMerchants.png?raw=true[/img]
[line]
[img]https://github.com/SolastaMods/SolastaCommunityExpansion/blob/Beta/Media/08-Gameplay-Tools.png?raw=true[/img]
[line]
[img]https://github.com/SolastaMods/SolastaCommunityExpansion/blob/Beta/Media/09-Interface-DungeonMaker.png?raw=true[/img]
[line]
[img]https://github.com/SolastaMods/SolastaCommunityExpansion/blob/Beta/Media/10-Interface-GameUi.png?raw=true[/img]
[line]
[img]https://github.com/SolastaMods/SolastaCommunityExpansion/blob/Beta/Media/11-Interface-KeyboardMouse.png?raw=true[/img]
[line]
[img]https://github.com/SolastaMods/SolastaCommunityExpansion/blob/Beta/Media/12-Encounters-General.png?raw=true[/img]
[line]
[img]https://github.com/SolastaMods/SolastaCommunityExpansion/blob/Beta/Media/13-Encounters-Bestiary.png?raw=true[/img]
[line]
[img]https://github.com/SolastaMods/SolastaCommunityExpansion/blob/Beta/Media/14-Encounters-Pool.png?raw=true[/img]
[line]
[img]https://github.com/SolastaMods/SolastaCommunityExpansion/blob/Beta/Media/15-CreditsTroubleshooting-Credits.png?raw=true[/img]
[line]
[img]https://github.com/SolastaMods/SolastaCommunityExpansion/blob/Beta/Media/16-Multiclass.png?raw=true[/img]
[line]

{1}

{2}

{3}

{4}

{5}

{6}
";
        internal static void DisplayDumpDescription()
        {
            UI.ActionButton("Dump Nexus Description", () =>
            {
                var collectedCredits = new StringBuilder();

                foreach (var kvp in CreditsTable)
                {
                    collectedCredits.Append("\n[*]").Append(kvp.Key).Append(": ").Append(kvp.Value);
                }

                var descriptionData = string.Format(ModDescription,
                    collectedCredits.ToString(),
                    ClassesContext.GenerateClassDescription(),
                    SubclassesContext.GenerateSubclassDescription(),
                    FeatsContext.GenerateFeatsDescription(),
                    FightingStyleContext.GenerateFightingStyleDescription(),
                    SpellsContext.GenerateSpellsDescription(),
                    ItemCraftingContext.GenerateItemsDescription());

                using var sw = new StreamWriter($"{DiagnosticsContext.DiagnosticsFolder}/NexusDescription.txt");
                sw.WriteLine(descriptionData);
            },
            UI.Width(200));
        }
#endif
    }
}
