using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HarmonyLib;
#if DEBUG
using SolastaCommunityExpansion.DataMiner;
#endif

namespace SolastaCommunityExpansion.Models
{
    internal static class DiagnosticsContext
    {
        // very large or not very useful definitions
        private static readonly string[] ExcludeFromExport = new[]
        {
            "AdventureLogDefinition",
            "ConsoleTableDefinition",
            "CreditsGroupDefinition",
            "CreditsTableDefinition",
            "DocumentTableDefinition",
            "NarrativeEventTableDefinition",
            "NarrativeTreeDefinition", // NarrativeTreeDefinition causes crash with PreserveReferencesHandling.None
            "SoundbanksDefinition",
            "SubtitleTableDefinition",
            "TravelJournalDefinition",
            "TutorialSectionDefinition",
            "TutorialStepDefinition",
            "TutorialSubsectionDefinition",
            "TutorialTocDefinition",
            "TutorialTableDefinition",
            "QuestTreeDefinition",
        };

        private static readonly string[] ExcludeFromCEExport = new[]
        {
            "BlueprintCategory",
            "GadgetBlueprint",
            "RoomBlueprint",
            "PropBlueprint"
        };

        private static Dictionary<BaseDefinition, BaseDefinition> TABaseDefinitionAndCopy;
        private static BaseDefinition[] TABaseDefinitions;
        private static Dictionary<Type, BaseDefinition[]> TABaseDefinitionsMap;
        private static BaseDefinition[] CEBaseDefinitions;
        private static HashSet<BaseDefinition> CEBaseDefinitions2;
        private static Dictionary<Type, BaseDefinition[]> CEBaseDefinitionsMap;

        internal const string GAME_FOLDER = ".";
        internal const int TA = 0;
        internal const int CE = 1;
        internal const int TA2 = 2;
        internal const string ProjectEnvironmentVariable = "SolastaCEProjectDir";

        internal static readonly string ProjectFolder = Environment.GetEnvironmentVariable(ProjectEnvironmentVariable, EnvironmentVariableTarget.Machine);

        internal static readonly string DiagnosticsFolder = GetDiagnosticsFolder();

        private static string GetDiagnosticsFolder()
        {
            var path = Path.Combine(ProjectFolder ?? GAME_FOLDER, "Diagnostics");

            EnsureFolderExists(path);

            return path;
        }

        private static void EnsureFolderExists(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        internal static void CacheTADefinitions()
        {
            if (TABaseDefinitionsMap != null)
            {
                return;
            }

            var definitions = new Dictionary<Type, BaseDefinition[]>();

            foreach (var db in (Dictionary<Type, object>)AccessTools.Field(typeof(DatabaseRepository), "databases").GetValue(null))
            {
                var arr = ((IEnumerable)db.Value).Cast<BaseDefinition>().ToArray();

                definitions.Add(db.Key, arr);
            }

            TABaseDefinitionsMap = definitions
                .OrderBy(db => db.Key.FullName)
                .ToDictionary(v => v.Key, v => v.Value);

            TABaseDefinitions = TABaseDefinitionsMap.Values
                .SelectMany(v => v)
                .Where(x => Array.IndexOf(ExcludeFromExport, x.GetType().Name) < 0)
                .Distinct()
                .OrderBy(x => x.Name)
                .ThenBy(x => x.GetType().Name)
                .ToArray();

            // Get a copy of definitions so we can export the originals.
            // Note not copying the excluded definitions to save memory.
            TABaseDefinitionAndCopy = TABaseDefinitions
                .ToDictionary(x => x, x =>
                {
                    var copy = UnityEngine.Object.Instantiate(x);
                    copy.name = x.Name;
                    return copy;
                });

            Main.Log($"Cached {TABaseDefinitions.Length} TA definitions");
        }

        internal static void CacheCEDefinitions()
        {
            if (TABaseDefinitionsMap == null)
            {
                return;
            }

            if (CEBaseDefinitionsMap != null)
            {
                return;
            }

            var definitions = new Dictionary<Type, BaseDefinition[]>();

            foreach (var db in (Dictionary<Type, object>)AccessTools.Field(typeof(DatabaseRepository), "databases").GetValue(null))
            {
                var arr = ((IEnumerable)db.Value).Cast<BaseDefinition>().ToArray();

                if (TABaseDefinitionsMap.TryGetValue(db.Key, out var taDefinitions))
                {
                    arr = arr.Except(taDefinitions).ToArray();
                }

                definitions.Add(db.Key, arr);
            }

            CEBaseDefinitionsMap = definitions.OrderBy(db => db.Key.FullName).ToDictionary(v => v.Key, v => v.Value);
            CEBaseDefinitions = CEBaseDefinitionsMap.Values
                .SelectMany(v => v)
                .Where(x => Array.IndexOf(ExcludeFromExport, x.GetType().Name) < 0)
                .Where(x => Array.IndexOf(ExcludeFromCEExport, x.GetType().Name) < 0)
                .Distinct()
                .OrderBy(x => x.Name)
                .ThenBy(x => x.GetType().Name)
                .ToArray();
            CEBaseDefinitions2 = CEBaseDefinitions.ToHashSet();

            Main.Log($"Cached {CEBaseDefinitions.Length} CE definitions");
        }

        internal static List<string> KnownDuplicateDefinitionNames { get; } = new()
        {
            "SummonProtectorConstruct"
        };

        internal static bool IsCeDefinition(BaseDefinition definition)
        {
            return CEBaseDefinitions2.Contains(definition);
        }

#if DEBUG
        private const string OFFICIAL_BP_FOLDER = "OfficialBlueprints";
        private const string COMMUNITY_EXPANSION_BP_FOLDER = "CommunityExpansionBlueprints";

        internal static void ExportTADefinitions()
        {
            var path = Path.Combine(DiagnosticsFolder, OFFICIAL_BP_FOLDER);

            BlueprintExporter.ExportBlueprints(TA, TABaseDefinitions, TABaseDefinitionsMap, TABaseDefinitionAndCopy, true, path);
        }

        /// <summary>
        /// Export all TA definitions with any modifications made by CE.
        /// </summary>
        internal static void ExportTADefinitionsAfterCELoaded()
        {
            var path = Path.Combine(DiagnosticsFolder, OFFICIAL_BP_FOLDER);

            BlueprintExporter.ExportBlueprints(TA2, TABaseDefinitions, TABaseDefinitionsMap, TABaseDefinitionAndCopy, false, path);
        }

        internal static void ExportCEDefinitions()
        {
            var path = Path.Combine(DiagnosticsFolder, COMMUNITY_EXPANSION_BP_FOLDER);

            BlueprintExporter.ExportBlueprints(CE, CEBaseDefinitions, CEBaseDefinitionsMap, null, false, path);
        }

        internal static void CreateTADefinitionDiagnostics()
        {
            CreateDefinitionDiagnostics(TABaseDefinitions, "TA-Definitions");
        }

        internal static void CreateCEDefinitionDiagnostics()
        {
            var baseFilename = "CE-Definitions";

            CreateDefinitionDiagnostics(CEBaseDefinitions, baseFilename);

            CheckOrphanedTerms(Path.Combine(DiagnosticsFolder, $"{baseFilename}-Translations-OrphanedTerms-en.txt"));
        }

        internal static void CheckOrphanedTerms(string outputFile)
        {
            var terms = new Dictionary<string, string>();
            var sourceFile = Path.Combine(Main.MOD_FOLDER, "Translations-en.txt");

            foreach (var line in File.ReadLines(sourceFile))
            {
                try
                {
                    var splitted = line.Split(new[] { '\t', ' ' }, 2);

                    terms.Add(splitted[0], splitted[1]);
                }
                catch
                {
                    continue;
                }
            }

            foreach (var definition in CEBaseDefinitions)
            {
                var title = definition.GuiPresentation.Title;
                var description = definition.GuiPresentation.Description;

                if (title != null && terms.ContainsKey(title))
                {
                    terms.Remove(title);
                }

                if (description != null && !description.Contains("{") && terms.ContainsKey(description))
                {
                    terms.Remove(description);
                }
            }

            using var writer = new StreamWriter(outputFile);

            foreach (var kvp in terms)
            {
                writer.WriteLine($"{kvp.Key}\t{kvp.Value}");
            }
        }


        private static void CreateDefinitionDiagnostics(BaseDefinition[] baseDefinitions, string baseFilename)
        {
            if (baseDefinitions == null)
            {
                return;
            }

            EnsureFolderExists(DiagnosticsFolder);

            /////////////////////////////////////////////////////////////////////////////////////////////////
            // Write all definitions with no GUI presentation to file
            File.WriteAllLines(Path.Combine(DiagnosticsFolder, $"{baseFilename}-GuiPresentation-MissingValue.txt"),
                baseDefinitions
                    .Where(d => string.IsNullOrWhiteSpace(d.GuiPresentation?.Title) || string.IsNullOrWhiteSpace(d.GuiPresentation?.Description))
                    .Distinct()
                    .Select(d => $"{d.Name}:\tTitle='{d.GuiPresentation?.Title ?? string.Empty}', Desc='{d.GuiPresentation?.Description ?? string.Empty}'"));

            /////////////////////////////////////////////////////////////////////////////////////////////////
            // Write all definitions with GUI presentation but missing translation to file
            var languageSourceData = LocalizationManager.Sources[0];
            var currentLanguage = LocalizationManager.CurrentLanguageCode;
            var languageIndex = languageSourceData.GetLanguageIndexFromCode(currentLanguage);
            var invalidSyntaxTerms = new List<string>();

            var allLines = baseDefinitions
                .Select(d => new[] {
                    new { d.Name, Key = d.GuiPresentation?.Title, Type = "Title" },
                    new { d.Name, Key = d.GuiPresentation?.Description, Type = "Description" }
                })
                .SelectMany(d => d)
                .Where(d => !d.Name.StartsWith("Telema", StringComparison.OrdinalIgnoreCase))
                .Where(d => !d.Name.StartsWith("HairShape", StringComparison.OrdinalIgnoreCase))
                .Where(d => !d.Name.StartsWith("HairColor", StringComparison.OrdinalIgnoreCase))
                .Where(d => !d.Name.StartsWith("FaceShape", StringComparison.OrdinalIgnoreCase))
                .Where(d => d.Key != GuiPresentationBuilder.EmptyString)
                .Where(d => !string.IsNullOrWhiteSpace(d.Key))
                .Where(d =>
                {
                    if (!d.Key.Contains("/&"))
                    {
                        invalidSyntaxTerms.Add($"{d.Name}\t{d.Type}='{d.Key}'.");
                        return false;
                    }
                    var termData = languageSourceData.GetTermData(d.Key);
                    return string.IsNullOrWhiteSpace(termData?.Languages[languageIndex]);
                })
                .Distinct()
                .Select(d => $"{d.Name}\t{d.Type}='{d.Key}'.");

            File.WriteAllLines(Path.Combine(DiagnosticsFolder, $"{baseFilename}-GuiPresentation-MissingTranslation-{currentLanguage}.txt"), allLines);
            File.WriteAllLines(Path.Combine(DiagnosticsFolder, $"{baseFilename}-GuiPresentation-InvalidSyntaxTranslation-{currentLanguage}.txt"), invalidSyntaxTerms);
        }
#endif
    }
}
