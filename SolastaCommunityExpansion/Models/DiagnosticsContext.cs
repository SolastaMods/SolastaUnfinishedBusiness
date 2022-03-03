using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HarmonyLib;
using I2.Loc;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.DataMiner;

namespace SolastaCommunityExpansion.Models
{
    internal static class DiagnosticsContext
    {
        private const string OFFICIAL_BP_FOLDER = "OfficialBlueprints";
        private const string COMMUNITY_EXPANSION_BP_FOLDER = "CommunityExpansionBlueprints";

        private static BaseDefinition[] TABaseDefinitions;
        private static Dictionary<Type, BaseDefinition[]> TABaseDefinitionsMap;
        private static BaseDefinition[] CEBaseDefinitions;
        private static Dictionary<Type, BaseDefinition[]> CEBaseDefinitionsMap;

        internal const string GAME_FOLDER = ".";
        internal const int TA = 0;
        internal const int CE = 1;
        internal const string ProjectEnvironmentVariable = "SolastaCEProjectDir";

        internal static readonly string ProjectFolder = Environment.GetEnvironmentVariable(ProjectEnvironmentVariable, EnvironmentVariableTarget.Machine);

        internal static readonly string DiagnosticsFolder = GetDiagnosticsFolder();

        private static string GetDiagnosticsFolder()
        {
            var path = Path.Combine(ProjectFolder ?? GAME_FOLDER, @"SolastaCommunityExpansion\Diagnostics");

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

            TABaseDefinitionsMap = definitions.OrderBy(db => db.Key.FullName).ToDictionary(v => v.Key, v => v.Value);
            TABaseDefinitions = TABaseDefinitionsMap.Values
                .SelectMany(v => v)
                .Where(x => Array.IndexOf(Main.Settings.ExcludeFromExport, x.GetType().Name) < 0)
                .OrderBy(x => x.Name)
                .ThenBy(x => x.GetType().Name)
                .ToArray();

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
                .Where(x => Array.IndexOf(Main.Settings.ExcludeFromExport, x.GetType().Name) < 0)
                .OrderBy(x => x.Name)
                .ThenBy(x => x.GetType().Name)
                .ToArray();

            Main.Log($"Cached {CEBaseDefinitions.Length} CE definitions");
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
                    var termData = languageSourceData.GetTermData(d.Key);
                    return string.IsNullOrWhiteSpace(termData?.Languages[languageIndex]);
                })
                .Distinct()
                .Select(d => $"{d.Name}\t{d.Type}='{d.Key}'.");

            File.WriteAllLines(Path.Combine(DiagnosticsFolder, $"{baseFilename}-GuiPresentation-MissingTranslation-{currentLanguage}.txt"), allLines);
        }

        internal static void ExportTADefinitions()
        {
            var path = Path.Combine(DiagnosticsFolder, OFFICIAL_BP_FOLDER);

            BlueprintExporter.ExportBlueprints(TA, TABaseDefinitions, TABaseDefinitionsMap, path);
        }

        internal static void ExportCEDefinitions()
        {
            var path = Path.Combine(DiagnosticsFolder, COMMUNITY_EXPANSION_BP_FOLDER);

            BlueprintExporter.ExportBlueprints(CE, CEBaseDefinitions, CEBaseDefinitionsMap, path);
        }

        internal static void CreateTADefinitionDiagnostics()
        {
            CreateDefinitionDiagnostics(TABaseDefinitions, "TA-Definitions");
        }

        internal static void CreateCEDefinitionDiagnostics()
        {
            CreateDefinitionDiagnostics(CEBaseDefinitions, "CE-Definitions");
        }

        internal static List<string> KnownDuplicateDefinitionNames { get; } = new()
        {
            "SummonProtectorConstruct"
        };
    }
}
