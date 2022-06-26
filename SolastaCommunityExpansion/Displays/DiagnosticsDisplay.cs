#if DEBUG
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ModKit;
using SolastaCommunityExpansion.DataMiner;
using SolastaCommunityExpansion.Models;
using SolastaCommunityExpansion.Patches.Diagnostic;
using static SolastaCommunityExpansion.Displays.CreditsDisplay;

namespace SolastaCommunityExpansion.Displays
{
    internal static class DiagnosticsDisplay
    {
        private const string ModDescription = @"
[size=5][b][i]Solasta Community Expansion[/i][/b][/size]

This is a collection of work from the Solasta modding community. It includes multiclass, races, classes, subclasses, feats, fighting styles, spells, items, crafting recipes, gameplay options, UI improvements, Dungeon Maker improvements and more. The general philosophy is everything is optional to enable, so you can install the mod and then enable the pieces you want. There are some minor bug fixes that are enabled by default.

[size=4][b]Credits[/b][/size]

[list]
{0}
[/list]

[size=4][b]How to Install - Windows[/b][/size]

[b]STEP 1:[/b] Install Unity Mod Manager (UMM):

[list]
[*] Download Unity Mod Manager from [url=https://www.nexusmods.com/site/mods/21]Nexus Mods[/url] and extract the zip to a folder of your preference [don't use the Solasta game folder].
[*] Start the program [i]UnityModManager.exe[/i]
[*] Select [i]Solasta: Crown of the Magister[/i] from the list of available games
[*] Manually select the game folder in case UMM fails to auto detect it
[*] The Unity Mod Manager should now launch when game is launched
[/list]

[img]https://github.com/SolastaMods/SolastaCommunityExpansion/blob/master/Media/UMM-01.png?raw=true[/img]

[b]HINT:[/b] Doorstop Proxy is the preferred install method but won't work on some Windows configurations or MacOS. Use the Assembly method as an alternative in case you don't see the UMM UI upon game launch.

[b]STEP 2:[/b] Install Community Expansion (CE):

[list]
[*] Download CE from the files section and drag and drop the zip over the UMM Mods tab
[/list]

[img]https://github.com/SolastaMods/SolastaCommunityExpansion/blob/master/Media/UMM-02.png?raw=true[/img]

[size=4][b]How to Install - MacOS[/b][/size]

[b]STEP 1:[/b] Install Unity Mod Manager (UMM):

[list]
[*] Download the custom MacOS UMM from optional files on this mod, unzip and copy the [b]Contents[/b] folder to the clipboard
[*] Open Steam or GOG client, navigate to your game library, select Solasta and [i]Browse Local Files[/i]

    [img]https://github.com/SolastaMods/SolastaCommunityExpansion/blob/master/Media/macos-00.png?raw=true[/img]

[*] Right-click [b]Solasta[/b] and [i]Show Package Contents[/i]

    [img]https://github.com/SolastaMods/SolastaCommunityExpansion/blob/master/Media/macos-01.png?raw=true[/img]

[*] Paste here the contents folder from the clipboard. CHOOSE MERGE!

    [img]https://github.com/SolastaMods/SolastaCommunityExpansion/blob/master/Media/macos-02.png?raw=true[/img]
[/list]

[b]STEP 2:[/b] Install Community Expansion (CE):

[*] Create the folder [b]/Applications/Mods[/b]
[*] Download CE from the files section, unzip and copy it under above folder

[size=4][b]Multiplayer[/b][/size]

We did all possible efforts to ensure this Mod will work under a multiplayer session. You must ensure all players have this mod installed and at least all settings under CE Mod UI > Gameplay matches across all players. The easiest way to achieve that is ask the host to share his Mod settings.xml file with guests before a session.

[size=4][b]How to Report Bugs[/b][/size]

[list]
[*] The versions of Solasta and this mod.
[*] A list of other mods you have installed [you shouldn't have any].
[*] A short description of the bug.
[*] A step-by-step procedure to reproduce it.
[*] The save, character and log files.
[/list]

[b]HINT:[/b] Check the folder [i]C:\Users\[b]YOUR_USER_NAME[/b]\AppData\LocalLow\Tactical Adventures\Solasta[/i] for the info we need.

[size=4][b]Source Code[/b][/size]

You can contribute to this work at [url=https://github.com/SolastaMods/SolastaCommunityExpansion]Source Code (MIT License)[/url].

[size=4][b]Features[/b][/size]

All settings start disabled by default. On first start the mod will display an welcome message and open the UMM Mod UI settings again.

[size=3][b]Races[/b][/size]

[list=1]
{1}
[/list]

[size=3][b]Classes[/b][/size]

[list=1]
{2}
[/list]

[size=3][b]Mod Classes Subclasses[/b][/size]

[list=1]
{3}
[/list]

[size=3][b]Official Classes Subclasses[/b][/size]

[list=1]
{4}
[/list]

[size=3][b]Feats[/b][/size]

[list=1]
{5}
[/list]

[size=3][b]Fighting Styles[/b][/size]

[list=1]
{6}
[/list]

[size=3][b]Spells[/b][/size]

[list=1]
{7}
[/list]

[size=3][b]Items & Crafting[/b][/size]

[list=1]
{8}
[/list]

[size=3][b]All Settings[/b][/size]

[img]https://github.com/SolastaMods/SolastaCommunityExpansion/blob/master/Media/01-Character-General.png?raw=true[/img]
[line]
[img]https://github.com/SolastaMods/SolastaCommunityExpansion/blob/master/Media/02-Character-ClassesSubclasses.png?raw=true[/img]
[line]
[img]https://github.com/SolastaMods/SolastaCommunityExpansion/blob/master/Media/03-Character-FeatsFightingStyles.png?raw=true[/img]
[line]
[img]https://github.com/SolastaMods/SolastaCommunityExpansion/blob/master/Media/04-Character-Spells.png?raw=true[/img]
[line]
[img]https://github.com/SolastaMods/SolastaCommunityExpansion/blob/master/Media/05-Gameplay-Rules.png?raw=true[/img]
[line]
[img]https://github.com/SolastaMods/SolastaCommunityExpansion/blob/master/Media/06-Gameplay-ItemsCraftingMerchants.png?raw=true[/img]
[line]
[img]https://github.com/SolastaMods/SolastaCommunityExpansion/blob/master/Media/07-Gameplay-Tools.png?raw=true[/img]
[line]
[img]https://github.com/SolastaMods/SolastaCommunityExpansion/blob/master/Media/08-Interface-DungeonMaker.png?raw=true[/img]
[line]
[img]https://github.com/SolastaMods/SolastaCommunityExpansion/blob/master/Media/09-Interface-GameUi.png?raw=true[/img]
[line]
[img]https://github.com/SolastaMods/SolastaCommunityExpansion/blob/master/Media/10-Interface-KeyboardMouse.png?raw=true[/img]
[line]
[img]https://github.com/SolastaMods/SolastaCommunityExpansion/blob/master/Media/11-Interface-Translations.png?raw=true[/img]
[line]
[img]https://github.com/SolastaMods/SolastaCommunityExpansion/blob/master/Media/12-Encounters-General.png?raw=true[/img]
[line]
[img]https://github.com/SolastaMods/SolastaCommunityExpansion/blob/master/Media/13-Encounters-Bestiary.png?raw=true[/img]
[line]
[img]https://github.com/SolastaMods/SolastaCommunityExpansion/blob/master/Media/14-Encounters-Pool.png?raw=true[/img]
[line]
[img]https://github.com/SolastaMods/SolastaCommunityExpansion/blob/master/Media/15-CreditsDiagnostics-Credits.png?raw=true[/img]
[line]
";

        internal static void DisplayDiagnostics()
        {
            UI.Label("");
            UI.Label(". You can set the environment variable " +
                     DiagnosticsContext.ProjectEnvironmentVariable.italic().yellow() +
                     " to customize the output folder");

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
            var percentageCompleteTa = BlueprintExporter.CurrentExports[DiagnosticsContext.TA].percentageComplete;
            var percentageCompleteTa2 = BlueprintExporter.CurrentExports[DiagnosticsContext.TA2].percentageComplete;
            var percentageCompleteCe = BlueprintExporter.CurrentExports[DiagnosticsContext.CE].percentageComplete;

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
            }

            using (UI.HorizontalScope())
            {
                UI.ActionButton("Create TA diagnostics", () => DiagnosticsContext.CreateTADefinitionDiagnostics(),
                    UI.Width(200));
                UI.ActionButton("Create CE diagnostics", () => DiagnosticsContext.CreateCEDefinitionDiagnostics(),
                    UI.Width(200));
                UI.ActionButton("Dump Descriptions", () => DisplayDumpDescription(), UI.Width(200));
            }

            UI.Label("");

            var logVariantMisuse = Main.Settings.DebugLogVariantMisuse;

            if (UI.Toggle("Log misuse of EffectForm and ItemDefinition [requires restart]", ref logVariantMisuse))
            {
                Main.Settings.DebugLogVariantMisuse = logVariantMisuse;
            }

            ItemDefinitionVerification.Mode =
                Main.Settings.DebugLogVariantMisuse
                    ? ItemDefinitionVerification.Verification.Log
                    : ItemDefinitionVerification.Verification.None;
            EffectFormVerification.Mode =
                Main.Settings.DebugLogVariantMisuse
                    ? EffectFormVerification.Verification.Log
                    : EffectFormVerification.Verification.None;

            UI.Label("");
        }

        private static string GenerateDescription<T>(IEnumerable<T> definitions) where T : BaseDefinition
        {
            var outString = new StringBuilder();

            foreach (var definition in definitions)
            {
                outString.Append("\n[*][b]");
                outString.Append(definition.FormatTitle());
                outString.Append("[/b]: ");
                outString.Append(definition.FormatDescription());
            }

            return outString.ToString();
        }

        internal static void DisplayDumpDescription()
        {
            var collectedCredits = new StringBuilder();

            foreach (var (author, description) in CreditsTable)
            {
                collectedCredits
                    .Append("\n[*][b]")
                    .Append(author)
                    .Append("[/b]: ")
                    .Append(description);
            }

            var racesAndSubs = new List<CharacterRaceDefinition>();

            foreach (var characterRaceDefinition in RacesContext.Races)
            {
                if (characterRaceDefinition.SubRaces.Count == 0)
                {
                    racesAndSubs.Add(characterRaceDefinition);
                }
                else
                {
                    racesAndSubs.AddRange(characterRaceDefinition.SubRaces);
                }
            }

            var descriptionData = string.Format(ModDescription,
                collectedCredits,
                GenerateDescription(racesAndSubs),
                GenerateDescription(ClassesContext.Classes),
                GenerateDescription(DatabaseRepository.GetDatabase<CharacterSubclassDefinition>()
                    .Where(x => !SubclassesContext.Subclasses.Contains(x))
                    .Where(x => DiagnosticsContext.IsCeDefinition(x))),
                GenerateDescription(SubclassesContext.Subclasses),
                GenerateDescription(FeatsContext.Feats),
                GenerateDescription(FightingStyleContext.FightingStyles),
                GenerateDescription(SpellsContext.Spells),
                ItemCraftingContext.GenerateItemsDescription(),
                GenerateDescription(DungeonMakerContext.ModdedMonsters));

            using var sw = new StreamWriter($"{DiagnosticsContext.DiagnosticsFolder}/NexusDescription.txt");
            sw.WriteLine(descriptionData);
        }
    }
}
#endif
