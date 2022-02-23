using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HarmonyLib;
using I2.Loc;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Json;
using SolastaModApi.Extensions;
using SolastaModApi.Infrastructure;

namespace SolastaCommunityExpansion.Models
{
    internal static class Diagnostics
    {
        private static HashSet<BaseDefinition> TABaseDefinitions;

        internal const string DiagnosticsEnvironmentVariable = "SolastaCEDiagnosticsDir";

        internal static string DiagnosticsOutputFolder { get; } = GetDiagnosticsFolder();
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

        public static void PostDatabaseLoad()
        {
            #region Preconditions
            if (TABaseDefinitions != null)
            {
                // Only load TABaseDefinitions once
                return;
            }

            TABaseDefinitions = GetAllDefinitions();

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
            #endregion

            var ceDefinitions = GetAllDefinitions().Except(TABaseDefinitions).OrderBy(x => x.Name).ThenBy(x => x.GetType().Name).ToList();

            /////////////////////////////////////////////////////////////////////////////////////////////////
            // Write all CE definitions name/guid to file (txt)
            File.WriteAllLines(Path.Combine(DiagnosticsOutputFolder, "CE-Definitions.txt"),
                ceDefinitions.Select(d => $"{d.Name}, {d.GUID}"));

            // Write all CE definitions to file (json)
            ExportDefinitions(ceDefinitions, Path.Combine(DiagnosticsOutputFolder, "CE-Definitions.json"));

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

        private static void ExportDefinitions(IEnumerable<BaseDefinition> definitions, string path)
        {
            JsonUtil.Dump(definitions, path);

            // remove id/ref for simplicity of detecting changes using diff tool
            File.WriteAllLines(path, File.ReadAllLines(path).Where(l => !l.Trim().StartsWith("\"$id\":") && !l.Trim().StartsWith("\"$ref\":")));
        }

        internal static List<string> KnownDuplicateDefinitionNames { get; } = new()
        {
            "SummonProtectorConstruct"
        };
    }
}
