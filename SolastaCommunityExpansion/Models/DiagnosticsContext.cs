using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HarmonyLib;
using I2.Loc;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Json;

namespace SolastaCommunityExpansion.Models
{
    internal static class DiagnosticsContext
    {
        private static HashSet<BaseDefinition> TABaseDefinitions;
        private static Dictionary<Type, List<BaseDefinition>> TABaseDefinitionsMap;
        private static HashSet<BaseDefinition> CEBaseDefinitions;

        internal const string DiagnosticsEnvironmentVariable = "SolastaCEDiagnosticsDir";

        internal static string DiagnosticsOutputFolder { get; } = GetDiagnosticsFolder();

        internal static bool HasDiagnosticsFolder => !string.IsNullOrWhiteSpace(DiagnosticsOutputFolder);

        private static string GetDiagnosticsFolder()
        {
            var folder = Environment.GetEnvironmentVariable(DiagnosticsEnvironmentVariable, EnvironmentVariableTarget.Machine);

            if (string.IsNullOrWhiteSpace(folder))
            {
                Main.Log($"[{DiagnosticsEnvironmentVariable}] is not set.");
            }

            return folder;
        }

        private static HashSet<BaseDefinition> GetAllDefinitions()
        {
            var definitions = new HashSet<BaseDefinition>();

            foreach (var db in (Dictionary<Type, object>)AccessTools.Field(typeof(DatabaseRepository), "databases").GetValue(null))
            {
                foreach (var bd in (IEnumerable)db.Value)
                {
                    definitions.Add((BaseDefinition)bd);
                }
            }

            return definitions;
        }

        private static void BuildTADefinitionsMap()
        {
            var definitions = new Dictionary<Type, List<BaseDefinition>>();

            foreach (var db in (Dictionary<Type, object>)AccessTools.Field(typeof(DatabaseRepository), "databases").GetValue(null))
            {
                var list = new List<BaseDefinition>();

                foreach (var bd in (IEnumerable)db.Value)
                {
                    list.Add((BaseDefinition)bd);
                }

                definitions.Add(db.Key, list);
            }

            TABaseDefinitionsMap = definitions;
        }

        public static void PostDatabaseLoad()
        {
            #region Preconditions
            if (TABaseDefinitions != null)
            {
                // Only load TABaseDefinitions once
                return;
            }

            TABaseDefinitions = GetAllDefinitions();
            BuildTADefinitionsMap();

            var taDefinitions = TABaseDefinitions.OrderBy(x => x.Name).ThenBy(x => x.GetType().Name).ToList();

            if (!Main.Settings.DebugEnableTADefinitionDiagnostics)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(DiagnosticsOutputFolder))
            {
                Main.Log("No diagnostics output folder configured - aborting PostDatabaseLoad diagnostics.");
                return;
            }

            if (!Directory.Exists(DiagnosticsOutputFolder))
            {
                Directory.CreateDirectory(DiagnosticsOutputFolder);
            }
            #endregion

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

            File.WriteAllLines(Path.Combine(DiagnosticsOutputFolder, $"TA-Definitions-GuiPresentation-MissingTranslation-{currentLanguage}.txt"), allLines);
        }

        public static void PostCELoad()
        {
            #region Preconditions
            if (TABaseDefinitions == null)
            {
                return;
            }

            if (!Main.Settings.DebugEnableCEDefinitionDiagnostics)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(DiagnosticsOutputFolder))
            {
                Main.Log("No diagnostics output folder configured - aborting PostCELoad diagnostics.");
                return;
            }

            if (!Directory.Exists(DiagnosticsOutputFolder))
            {
                Directory.CreateDirectory(DiagnosticsOutputFolder);
            }

            if (CEBaseDefinitions == null)
            {
                CEBaseDefinitions = GetAllDefinitions().Except(TABaseDefinitions).ToHashSet();
            }
            #endregion

            var ceDefinitions = CEBaseDefinitions.OrderBy(x => x.Name).ThenBy(x => x.GetType().Name);

            /////////////////////////////////////////////////////////////////////////////////////////////////
            // Write all CE definitions name/guid to file (txt)
            File.WriteAllLines(Path.Combine(DiagnosticsOutputFolder, "CE-Definitions.txt"),
                ceDefinitions.Select(d => $"{d.Name}, {d.GUID}"));

            // Write all CE definitions to file (json)
            //ExportDefinitions(Path.Combine(DiagnosticsOutputFolder, "CE-Definitions.json"), ceDefinitions);

            /*
            Main.Log("Exporting PickPocket");
            ExportDefinitions(Path.Combine(DiagnosticsOutputFolder, "CE-Definitions-Test.json"), ceDefinitions.Where(d => d.Name.Contains("PickPocket")));

            Main.Log("Exporting AbilityCheckAffinityFeatPickPocket");
            ExportDefinition(Path.Combine(DiagnosticsOutputFolder, "CE-Definitions-AbilityCheckAffinityFeatPickPocket.json"), ceDefinitions.Single(d => d.Name == "AbilityCheckAffinityFeatPickPocket"));
            
            Main.Log("Exporting ProficiencyFeatPickPocket");
            ExportDefinition(Path.Combine(DiagnosticsOutputFolder, "CE-Definitions-ProficiencyFeatPickPocket.json"), ceDefinitions.Single(d => d.Name == "ProficiencyFeatPickPocket"));
            
            Main.Log("Exporting PickPocketFeat");
            ExportDefinition(Path.Combine(DiagnosticsOutputFolder, "CE-Definitions-PickPocketFeat.json"), ceDefinitions.Single(d => d.Name == "PickPocketFeat"));
            */

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

        /*        private static void ExportDefinition(string path, BaseDefinition definition)
                {
                    JsonUtil.Dump(definition, path);
                }

                private static void ExportDefinitions(string path, params BaseDefinition[] definitions)
                {
                    ExportDefinitions(path, definitions.AsEnumerable());
                }*/

        const string BlueprintFolder = "SolastaBlueprints2";

        internal static void ExportTABlueprints()
        {
            if (!Directory.Exists(BlueprintFolder))
            {
                Directory.CreateDirectory(BlueprintFolder);
            }

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

                foreach(var db in TABaseDefinitionsMap.OrderBy(db => db.Key.FullName))
                {
                    foreach (var definition in db.Value)
                    {
                        sw.WriteLine("{0}\t{1}\t{2}\t{3}", definition.Name, definition.GetType().FullName, db.Key.FullName, definition.GUID);
                    }
                }
            }

            // Blueprints/definitions
/*            foreach (var definition in TABaseDefinitions)
            {
                var dbType = definition.GetType();
                var value = definition;
                var subfolder = value.GetType().Name;
                if (value.GetType() != dbType)
                {
                    subfolder = $"{dbType.FullName}/{subfolder}";
                }

                if (!Directory.Exists($"{BlueprintFolder}/{subfolder}"))
                {
                    Directory.CreateDirectory($"{BlueprintFolder}/{subfolder}");
                }

                JsonUtil.TABlueprintDump(definition, $"{BlueprintFolder}/{subfolder}/{value.Name}.{value.GUID}.json");
            }*/
        }

        private static void ExportDefinitions(string path, IEnumerable<BaseDefinition> definitions)
        {
            JsonUtil.CEBlueprintDump(definitions, path);

            // remove id/ref for simplicity of detecting changes using diff tool
            //File.WriteAllLines(path, File.ReadAllLines(path).Where(l => !l.Trim().StartsWith("\"$id\":") && !l.Trim().StartsWith("\"$ref\":")));
        }

        internal static List<string> KnownDuplicateDefinitionNames { get; } = new()
        {
            "SummonProtectorConstruct"
        };
    }
}
