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
        private static HashSet<BaseDefinition> TABaseDefinitions;
        private static Dictionary<Type, List<BaseDefinition>> TABaseDefinitionsMap;
        private static HashSet<BaseDefinition> CEBaseDefinitions;
        private static Dictionary<Type, List<BaseDefinition>> CEBaseDefinitionsMap;

        internal const string DiagnosticsEnvironmentVariable = "SolastaCEDiagnosticsDir";
        internal static string DiagnosticsOutputFolder { get; } = GetDiagnosticsFolder();
        internal static bool HasDiagnosticsFolder => !string.IsNullOrWhiteSpace(DiagnosticsOutputFolder);
        private const string BlueprintFolder = "SolastaBlueprints";

        private static string GetDiagnosticsFolder()
        {
            var folder = Environment.GetEnvironmentVariable(DiagnosticsEnvironmentVariable, EnvironmentVariableTarget.Machine);

            if (string.IsNullOrWhiteSpace(folder))
            {
                Main.Log($"[{DiagnosticsEnvironmentVariable}] is not set.");
            }

            return folder;
        }

        private static void EnsureFolderExists(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        public static void CacheTADefinitions()
        {
            if (TABaseDefinitionsMap != null)
            {
                return;
            }

            var definitions = new Dictionary<Type, List<BaseDefinition>>();

            foreach (var db in (Dictionary<Type, object>)AccessTools.Field(typeof(DatabaseRepository), "databases").GetValue(null))
            {
                var list = ((IEnumerable)db.Value).Cast<BaseDefinition>().ToList();

                definitions.Add(db.Key, list);
            }

            TABaseDefinitionsMap = definitions;
            TABaseDefinitions = TABaseDefinitionsMap.Values.SelectMany(v => v).ToHashSet();

            Main.Log($"Cached {TABaseDefinitions.Count} TA definitions");
        }

        public static void CacheCEDefinitions()
        {
            if (TABaseDefinitionsMap == null)
            {
                return;
            }

            if (CEBaseDefinitionsMap != null)
            {
                return;
            }

            var definitions = new Dictionary<Type, List<BaseDefinition>>();

            foreach (var db in (Dictionary<Type, object>)AccessTools.Field(typeof(DatabaseRepository), "databases").GetValue(null))
            {
                var list = ((IEnumerable)db.Value).Cast<BaseDefinition>().ToList();

                if (TABaseDefinitionsMap.TryGetValue(db.Key, out var taDefinitions))
                {
                    list = list.Except(taDefinitions).ToList();
                }

                definitions.Add(db.Key, list);
            }

            CEBaseDefinitionsMap = definitions;
            CEBaseDefinitions = CEBaseDefinitionsMap.Values.SelectMany(v => v).ToHashSet();

            Main.Log($"Cached {CEBaseDefinitions.Count} CE definitions");
        }

        public static void CreateTADefinitionDiagnostics()
        {
            if (!HasDiagnosticsFolder)
            {
                return;
            }

            if (TABaseDefinitions == null)
            {
                return;
            }

            EnsureFolderExists(DiagnosticsOutputFolder);

            var taDefinitions = TABaseDefinitions.OrderBy(x => x.Name).ThenBy(x => x.GetType().Name).ToList();

            /////////////////////////////////////////////////////////////////////////////////////////////////
            // Write all TA definitions name/guid to file (txt)
            File.WriteAllLines(Path.Combine(DiagnosticsOutputFolder, "TA-Definitions.txt"),
                taDefinitions.Select(d => $"{d.Name}, {d.GUID}"));

            /////////////////////////////////////////////////////////////////////////////////////////////////
            // Write all TA definitions with no GUI presentation to file
            File.WriteAllLines(Path.Combine(DiagnosticsOutputFolder, "TA-Definitions-GuiPresentation-MissingValue.txt"),
                taDefinitions
                    .Where(d => string.IsNullOrWhiteSpace(d.GuiPresentation?.Title) || string.IsNullOrWhiteSpace(d.GuiPresentation?.Description))
                    .Select(d => $"{d.Name}:\tTitle='{d.GuiPresentation?.Title ?? string.Empty}', Desc='{d.GuiPresentation?.Description ?? string.Empty}'"));

            /////////////////////////////////////////////////////////////////////////////////////////////////
            // Write all TA definitions with GUI presentation but missing translation to file
            var languageSourceData = LocalizationManager.Sources[0];
            var currentLanguage = LocalizationManager.CurrentLanguageCode;
            var languageIndex = languageSourceData.GetLanguageIndexFromCode(currentLanguage);

            var allLines = taDefinitions
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
                .Select(d => $"{d.Name}\t{d.Type}='{d.Key}'.");

            File.WriteAllLines(Path.Combine(DiagnosticsOutputFolder, $"TA-Definitions-GuiPresentation-MissingTranslation-{currentLanguage}.txt"), allLines);
        }

        public static void CreateCEDefinitionDiagnostics()
        {
            if (!HasDiagnosticsFolder)
            {
                return;
            }

            EnsureFolderExists(DiagnosticsOutputFolder);

            var ceDefinitions = CEBaseDefinitions.OrderBy(x => x.Name).ThenBy(x => x.GetType().Name);

            /////////////////////////////////////////////////////////////////////////////////////////////////
            // Write all CE definitions name/guid to file (txt)
            File.WriteAllLines(Path.Combine(DiagnosticsOutputFolder, "CE-Definitions.txt"),
                ceDefinitions.Select(d => $"{d.Name}, {d.GUID}"));

            // Write all CE definitions with no GUI presentation to file
            File.WriteAllLines(Path.Combine(DiagnosticsOutputFolder, "CE-Definitions-GuiPresentation-MissingValue.txt"),
                ceDefinitions
                    .Where(d => string.IsNullOrWhiteSpace(d.GuiPresentation?.Title) || string.IsNullOrWhiteSpace(d.GuiPresentation?.Description))
                    .Select(d => $"{d.Name}:\tTitle='{d.GuiPresentation?.Title ?? string.Empty}', Desc='{d.GuiPresentation?.Description ?? string.Empty}'"));

            /////////////////////////////////////////////////////////////////////////////////////////////////
            // Write all CE definitions with GUI presentation but missing translation to file
            var languageSourceData = LocalizationManager.Sources[0];
            var currentLanguage = LocalizationManager.CurrentLanguageCode;
            var languageIndex = languageSourceData.GetLanguageIndexFromCode(currentLanguage);

            var allLines = ceDefinitions
                .Select(d => new[] {
                    new { d.Name, Key = d.GuiPresentation?.Title, Type = "Title" },
                    new { d.Name, Key = d.GuiPresentation?.Description, Type = "Description" }
                })
                .SelectMany(d => d)
                .Where(d => !d.Name.StartsWith("Telema", StringComparison.InvariantCultureIgnoreCase))
                .Where(d =>
                {
                    if (!string.IsNullOrWhiteSpace(d.Key) && d.Key != GuiPresentationBuilder.EmptyString)
                    {
                        var termData = languageSourceData.GetTermData(d.Key);
                        return string.IsNullOrWhiteSpace(termData?.Languages[languageIndex]);
                    }
                    else
                    {
                        return false;
                    }
                })
                .Select(d => $"{d.Name}\t{d.Type}='{d.Key}'.");

            File.WriteAllLines(Path.Combine(DiagnosticsOutputFolder, $"CE-Definitions-GuiPresentation-MissingTranslation-{currentLanguage}.txt"), allLines);
        }

        internal static void ExportTABlueprints(Action<int, int> progress)
        {
            if (TABaseDefinitionsMap == null || TABaseDefinitions == null)
            {
                return;
            }

            EnsureFolderExists(BlueprintFolder);

            // Types.txt
            using (var sw = new StreamWriter($"{BlueprintFolder}/Types.txt"))
            {
                foreach (var type in TABaseDefinitions.Select(t => t.GetType()).Distinct().OrderBy(t => t.Name))
                {
                    sw.WriteLine($"{type.FullName}");
                }
            }

            // Assets.txt
            using (var sw = new StreamWriter($"{BlueprintFolder}/Assets.txt"))
            {
                sw.WriteLine("{0}\t{1}\t{2}\t{3}", "Name", "Type", "DatabaseType", "GUID");

                foreach (var db in TABaseDefinitionsMap.OrderBy(db => db.Key.FullName))
                {
                    foreach (var definition in db.Value)
                    {
                        sw.WriteLine("{0}\t{1}\t{2}\t{3}", definition.Name, definition.GetType().FullName, db.Key.FullName, definition.GUID);
                    }
                }
            }

            var total = TABaseDefinitions.Count;

            // Blueprints/definitions
            foreach (var d in TABaseDefinitions.Select((d, i) => new { Definition = d, Index = i }))
            {
                var dbType = d.Definition.GetType();
                var value = d.Definition;
                var subfolder = value.GetType().Name;
                if (value.GetType() != dbType)
                {
                    subfolder = $"{dbType.FullName}/{subfolder}";
                }

                EnsureFolderExists($"{BlueprintFolder}/{subfolder}");

                JsonUtil.TABlueprintDump(d.Definition, $"{BlueprintFolder}/{subfolder}/{value.Name}.{value.GUID}.json");

                progress(d.Index, total);
            }

            progress(total, total);
        }

        internal static void ExportCEDefinitions(Action<int, int> progress)
        {
            if (!HasDiagnosticsFolder)
            {
                return;
            }

            if (CEBaseDefinitions == null)
            {
                return;
            }

            EnsureFolderExists(DiagnosticsOutputFolder);

            var ceDefinitions = CEBaseDefinitions.OrderBy(d => d.Name).ThenBy(d => d.GetType().Name);

            // Write all CE definitions to file (json)
            var path = Path.Combine(DiagnosticsOutputFolder, "CE-Definitions.json");
            JsonUtil.CEBlueprintDump(ceDefinitions, path, progress);
        }

        internal static List<string> KnownDuplicateDefinitionNames { get; } = new()
        {
            "SummonProtectorConstruct"
        };
    }
}
