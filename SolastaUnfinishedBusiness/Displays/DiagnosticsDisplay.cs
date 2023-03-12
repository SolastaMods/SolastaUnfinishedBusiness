#if DEBUG
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Api.ModKit;
using SolastaUnfinishedBusiness.DataMiner;
using SolastaUnfinishedBusiness.Models;
using static SolastaUnfinishedBusiness.Displays.CreditsDisplay;

namespace SolastaUnfinishedBusiness.Displays
{
    internal static class DiagnosticsDisplay
    {
        private const string ModDescription = @"
[size=5][b][i]Solasta Unfinished Business[/i][/b][/size]

[url=https://www.paypal.com/donate/?business=JG4FX47DNHQAG&item_name=Support+Solasta+Community+Expansion]DONATE[/url]

This is a collection of work from the Solasta modding community. It includes multiclass, races, classes, subclasses, feats, fighting styles, spells, items, crafting recipes, gameplay options, UI improvements, Dungeon Maker improvements and more. The general philosophy is everything is optional to enable, so you can install the mod and then enable the pieces you want. There are some minor bug fixes that are enabled by default.

[size=4][b]How to Report Bugs[/b][/size]

[list]
[*] The versions of Solasta and this mod.
[*] A list of other mods you have installed [you shouldn't have any].
[*] A short description of the bug.
[*] A step-by-step procedure to reproduce it.
[*] The save, character and log files [b](check HINT below)[/b].
[/list]

[b]HINT:[/b] Check the folder [i]C:\Users\[b]YOUR_USER_NAME[/b]\AppData\LocalLow\Tactical Adventures\Solasta[/i] for the info we need.

[size=4][b]How to Contribute[/b][/size]

You can contribute to this work at [url=https://github.com/SolastaMods/SolastaUnfinishedBusiness]Source Code (MIT License)[/url].

[size=4][b]How to Install - Windows[/b][/size]

[b]STEP 1:[/b] Install Unity Mod Manager (UMM):

[list]
[*] Download Unity Mod Manager from [url=https://www.nexusmods.com/site/mods/21]Nexus Mods[/url] and extract the zip to a folder of your preference [don't use the Solasta game folder].
[*] Start the program [i]UnityModManager.exe[/i]
[*] Select [i]Solasta: Crown of the Magister[/i] from the list of available games
[*] Manually select the game folder in case UMM fails to auto detect it
[*] The Unity Mod Manager should now launch when game is launched
[/list]

[img]https://github.com/SolastaMods/SolastaUnfinishedBusiness/blob/master/Media/UMM-01.png?raw=true[/img]

[b]HINT:[/b] Doorstop Proxy is the preferred install method but won't work on some Windows configurations or MacOS. Use the Assembly method as an alternative in case you don't see the UMM UI upon game launch.

[b]STEP 2:[/b] Install Unfinished Business (UB):

[list]
[*] Download UB from the files section and drag and drop the zip over the UMM Mods tab
[/list]

[img]https://github.com/SolastaMods/SolastaUnfinishedBusiness/blob/master/Media/UMM-02.png?raw=true[/img]

[size=4][b]How to Install - MacOS[/b][/size]

[b]STEP 1:[/b] Install Unity Mod Manager (UMM):

[list]
[*] Download the custom MacOS UMM from optional files on this mod, unzip and copy the [b]Contents[/b] folder to the clipboard
[*] Open Steam or GOG client, navigate to your game library, select Solasta and [i]Browse Local Files[/i]

    [img]https://github.com/SolastaMods/SolastaUnfinishedBusiness/blob/master/Media/macos-00.png?raw=true[/img]

[*] Right-click [b]Solasta[/b] and [i]Show Package Contents[/i]

    [img]https://github.com/SolastaMods/SolastaUnfinishedBusiness/blob/master/Media/macos-01.png?raw=true[/img]

[*] Paste here the contents folder from the clipboard. CHOOSE MERGE!

    [img]https://github.com/SolastaMods/SolastaUnfinishedBusiness/blob/master/Media/macos-02.png?raw=true[/img]

[/list]

[b]STEP 2:[/b] Install Unfinished Business (UB):

[list]
[*] Create the folder [b]/Applications/Mods[/b]
[*] Download UB from the files section, unzip and copy it under above folder
[/list]

[size=4][b]Features[/b][/size]

All settings start disabled by default.

[size=3][b]Races[/b][/size]

[list=1]
{1}
[/list]

[size=3][b]Classes[/b][/size]

[list=1]
{2}
{3}
[/list]

[size=3][b]Subclasses[/b][/size]

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

[size=3][b]Invocations[/b][/size]

[list=1]
{7}
[/list]

[size=3][b]Spells[/b][/size]

[list=1]
{8}
[/list]

[size=3][b]Items & Crafting[/b][/size]

[list=1]
{9}
[/list]

[size=4][b]Credits[/b][/size]

[list]
{0}
[/list]
";

/*
[size=3][b]All Settings[/b][/size]

[img]https://github.com/SolastaMods/SolastaUnfinishedBusiness/blob/master/Media/01-Character-General.png?raw=true[/img]
[line]
[img]https://github.com/SolastaMods/SolastaUnfinishedBusiness/blob/master/Media/02-Character-ClassesSubclasses.png?raw=true[/img]
[line]
[img]https://github.com/SolastaMods/SolastaUnfinishedBusiness/blob/master/Media/03-Character-FeatsFightingStyles.png?raw=true[/img]
[line]
[img]https://github.com/SolastaMods/SolastaUnfinishedBusiness/blob/master/Media/04-Character-Spells.png?raw=true[/img]
[line]
[img]https://github.com/SolastaMods/SolastaUnfinishedBusiness/blob/master/Media/05-Gameplay-Rules.png?raw=true[/img]
[line]
[img]https://github.com/SolastaMods/SolastaUnfinishedBusiness/blob/master/Media/06-Gameplay-ItemsCraftingMerchants.png?raw=true[/img]
[line]
[img]https://github.com/SolastaMods/SolastaUnfinishedBusiness/blob/master/Media/07-Gameplay-Tools.png?raw=true[/img]
[line]
[img]https://github.com/SolastaMods/SolastaUnfinishedBusiness/blob/master/Media/08-Interface-GameUi.png?raw=true[/img]
[line]
[img]https://github.com/SolastaMods/SolastaUnfinishedBusiness/blob/master/Media/09-Interface-DungeonMaker.png?raw=true[/img]
[line]
[img]https://github.com/SolastaMods/SolastaUnfinishedBusiness/blob/master/Media/10-Interface-Translations.png?raw=true[/img]
[line]
*/

        internal static void DisplayDiagnostics()
        {
            UI.Label();
            UI.Label(". You can set the environment variable " +
                     DiagnosticsContext.ProjectEnvironmentVariable.Italic().Khaki() +
                     " to customize the output folder");

            if (DiagnosticsContext.ProjectFolder == null)
            {
                UI.Label(". The output folder is set to " + "your game folder".Khaki().Bold());
            }
            else
            {
                UI.Label(". The output folder is set to " + DiagnosticsContext.DiagnosticsFolder.Khaki().Bold());
            }

            UI.Label();

            string exportTaLabel;
            string exportTaLabel2;
            string exportCeLabel;
            var percentageCompleteTa = BlueprintExporter.CurrentExports[DiagnosticsContext.Ta].PercentageComplete;
            var percentageCompleteTa2 = BlueprintExporter.CurrentExports[DiagnosticsContext.Ta2].PercentageComplete;
            var percentageCompleteCe = BlueprintExporter.CurrentExports[DiagnosticsContext.Ce].PercentageComplete;

            if (percentageCompleteTa == 0)
            {
                exportTaLabel = "Export TA blueprints";
            }
            else
            {
                exportTaLabel = "Cancel TA export at " + $"{percentageCompleteTa:00.0%}".Bold().Khaki();
            }

            if (percentageCompleteTa2 == 0)
            {
                exportTaLabel2 = "Export TA blueprints (modded)";
            }
            else
            {
                exportTaLabel2 = "Cancel TA export at " + $"{percentageCompleteTa2:00.0%}".Bold().Khaki();
            }

            if (percentageCompleteCe == 0)
            {
                exportCeLabel = "Export UB blueprints";
            }
            else
            {
                exportCeLabel = "Cancel UB export at " + $"{percentageCompleteCe:00.0%}".Bold().Khaki();
            }

            using (UI.HorizontalScope())
            {
                UI.ActionButton(exportTaLabel, () =>
                {
                    if (percentageCompleteTa == 0)
                    {
                        DiagnosticsContext.ExportTaDefinitions();
                    }
                    else
                    {
                        BlueprintExporter.Cancel(DiagnosticsContext.Ta);
                    }
                }, 200.Width());

                UI.ActionButton(exportCeLabel, () =>
                {
                    if (percentageCompleteCe == 0)
                    {
                        DiagnosticsContext.ExportCeDefinitions();
                    }
                    else
                    {
                        BlueprintExporter.Cancel(DiagnosticsContext.Ce);
                    }
                }, 200.Width());

                UI.ActionButton(exportTaLabel2, () =>
                {
                    if (percentageCompleteTa2 == 0)
                    {
                        DiagnosticsContext.ExportTaDefinitionsAfterCeLoaded();
                    }
                    else
                    {
                        BlueprintExporter.Cancel(DiagnosticsContext.Ta2);
                    }
                }, 200.Width());
            }

            using (UI.HorizontalScope())
            {
                UI.ActionButton("Create TA diagnostics", DiagnosticsContext.CreateTaDefinitionDiagnostics,
                    200.Width());
                UI.ActionButton("Create UB diagnostics", DiagnosticsContext.CreateCeDefinitionDiagnostics,
                    200.Width());
                UI.ActionButton("Dump Descriptions", DisplayDumpDescription, 200.Width());
            }

            UI.Label();

            var logVariantMisuse = Main.Settings.DebugLogVariantMisuse;

            if (UI.Toggle("Log misuse of EffectForm and ItemDefinition [requires restart]", ref logVariantMisuse))
            {
                Main.Settings.DebugLogVariantMisuse = logVariantMisuse;
            }

            UI.Label();
        }

        [NotNull]
        private static string GenerateDescription<T>([NotNull] IEnumerable<T> definitions) where T : BaseDefinition
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

        private static void DisplayDumpDescription()
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
                GenerateDescription(
                    DatabaseRepository.GetDatabase<CharacterSubclassDefinition>()
                        .Where(x => !SubclassesContext.Subclasses.Contains(x))
                        .Where(DiagnosticsContext.IsCeDefinition)),
                GenerateDescription(SubclassesContext.Subclasses),
                GenerateDescription(FeatsContext.Feats),
                GenerateDescription(FightingStyleContext.FightingStyles),
                GenerateDescription(InvocationsContext.Invocations),
                GenerateDescription(SpellsContext.Spells
                    .Where(x => x.ContentPack == CeContentPackContext.CeContentPack)),
                CraftingContext.GenerateItemsDescription());

            using var sw = new StreamWriter($"{DiagnosticsContext.DiagnosticsFolder}/NexusDescription.txt");
            sw.WriteLine(descriptionData);
        }
    }
}
#endif
