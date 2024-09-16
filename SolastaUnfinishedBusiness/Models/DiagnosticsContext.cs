using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
#if DEBUG
using System.IO;
using I2.Loc;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.DataMiner;
using Object = UnityEngine.Object;
#endif

namespace SolastaUnfinishedBusiness.Models;

internal static class DiagnosticsContext
{
    // very large or not very useful definitions
    private static readonly string[] ExcludeFromExport =
    [
        "AdventureLogDefinition", "ConsoleTableDefinition", "CreditsGroupDefinition", "CreditsTableDefinition",
        "DocumentTableDefinition", "NarrativeEventTableDefinition", "NarrativeTreeDefinition",
        "SoundbanksDefinition", "SubtitleTableDefinition", "TravelJournalDefinition", "TutorialSectionDefinition",
        "TutorialStepDefinition", "TutorialSubsectionDefinition", "TutorialTocDefinition",
        "TutorialTableDefinition", "QuestTreeDefinition"
    ];

    private static readonly string[] ExcludeFromCeExport =
    [
        "BlueprintCategory", "GadgetBlueprint", "RoomBlueprint", "PropBlueprint"
    ];

    private static Dictionary<Type, BaseDefinition[]> _taBaseDefinitionsMap;
    private static BaseDefinition[] _ceBaseDefinitions;
    private static HashSet<BaseDefinition> _ceBaseDefinitions2;
    private static Dictionary<Type, BaseDefinition[]> _ceBaseDefinitionsMap;
    internal static List<string> KnownDuplicateDefinitionNames { get; } = ["SummonProtectorConstruct"];

    internal static void CacheTaDefinitions()
    {
        if (_taBaseDefinitionsMap != null)
        {
            return;
        }

        var definitions = new Dictionary<Type, BaseDefinition[]>();

        foreach (var db in DatabaseRepository.databases)
        {
            var arr = ((IEnumerable)db.Value).Cast<BaseDefinition>().ToArray();

            definitions.Add(db.Key, arr);
        }

        _taBaseDefinitionsMap = definitions
            .OrderBy(db => db.Key.FullName)
            .ToDictionary(v => v.Key, v => v.Value);

#if DEBUG
        _taBaseDefinitions = _taBaseDefinitionsMap.Values
            .SelectMany(v => v)
            .Where(x => Array.IndexOf(ExcludeFromExport, x.GetType().Name) < 0)
            .Distinct()
            .OrderBy(x => x.Name)
            .ThenBy(x => x.GetType().Name)
            .ToArray();

        // Get a copy of definitions so we can export the originals.
        // Note not copying the excluded definitions to save memory.
        _taBaseDefinitionAndCopy = _taBaseDefinitions
            .ToDictionary(x => x, x =>
            {
                var copy = Object.Instantiate(x);
                copy.name = x.Name;
                return copy;
            });
#endif
    }

    internal static void CacheCeDefinitions()
    {
        if (_taBaseDefinitionsMap == null)
        {
            return;
        }

        if (_ceBaseDefinitionsMap != null)
        {
            return;
        }

        var definitions = new Dictionary<Type, BaseDefinition[]>();

        foreach (var db in DatabaseRepository.databases)
        {
            var arr = ((IEnumerable)db.Value).Cast<BaseDefinition>().ToArray();

            if (_taBaseDefinitionsMap.TryGetValue(db.Key, out var taDefinitions))
            {
                arr = arr.Except(taDefinitions).ToArray();
            }

            definitions.Add(db.Key, arr);
        }

        _ceBaseDefinitionsMap = definitions.OrderBy(db => db.Key.FullName).ToDictionary(v => v.Key, v => v.Value);
        _ceBaseDefinitions = _ceBaseDefinitionsMap.Values
            .SelectMany(v => v)
            .Where(x => Array.IndexOf(ExcludeFromExport, x.GetType().Name) < 0)
            .Where(x => Array.IndexOf(ExcludeFromCeExport, x.GetType().Name) < 0)
            .Distinct()
            .OrderBy(x => x.Name)
            .ThenBy(x => x.GetType().Name)
            .ToArray();
        _ceBaseDefinitions2 = _ceBaseDefinitions.ToHashSet();
    }

    internal static bool IsCeDefinition(BaseDefinition definition)
    {
        return _ceBaseDefinitions2?.Contains(definition) ?? false;
    }

#if DEBUG
    private const string GameFolder = ".";
    internal const string ProjectEnvironmentVariable = "SolastaCEProjectDir";
    internal const int Ta = 0;
    internal const int Ce = 1;
    internal const int Ta2 = 2;
    private static Dictionary<BaseDefinition, BaseDefinition> _taBaseDefinitionAndCopy;
    private static BaseDefinition[] _taBaseDefinitions;
    internal static readonly string DiagnosticsFolder = GetDiagnosticsFolder();

    internal static readonly string ProjectFolder =
        Environment.GetEnvironmentVariable(ProjectEnvironmentVariable, EnvironmentVariableTarget.Machine);

    [NotNull]
    private static string GetDiagnosticsFolder()
    {
        var path = Path.Combine(ProjectFolder ?? GameFolder, "Diagnostics");

        Main.EnsureFolderExists(path);

        return path;
    }

    private const string OfficialBpFolder = "OfficialBlueprints";
    private const string UnfinishedBusinessBpFolder = "UnfinishedBusinessBlueprints";

    internal static void ExportTaDefinitions()
    {
        var path = Path.Combine(DiagnosticsFolder, OfficialBpFolder);

        BlueprintExporter.ExportBlueprints(Ta, _taBaseDefinitions, _taBaseDefinitionsMap, _taBaseDefinitionAndCopy,
            true, path);
    }

    /// <summary>
    ///     Export all TA definitions with any modifications made by CE.
    /// </summary>
    internal static void ExportTaDefinitionsAfterCeLoaded()
    {
        var path = Path.Combine(DiagnosticsFolder, OfficialBpFolder);

        BlueprintExporter.ExportBlueprints(Ta2, _taBaseDefinitions, _taBaseDefinitionsMap, _taBaseDefinitionAndCopy,
            false, path);
    }

    internal static void ExportCeDefinitions()
    {
        var path = Path.Combine(DiagnosticsFolder, UnfinishedBusinessBpFolder);

        BlueprintExporter.ExportBlueprints(Ce, _ceBaseDefinitions, _ceBaseDefinitionsMap, null, false, path);
    }

    internal static void CreateTaDefinitionDiagnostics()
    {
        CreateDefinitionDiagnostics(_taBaseDefinitions, "TA-Definitions");
    }

    internal static void CreateCeDefinitionDiagnostics()
    {
        const string BASE_FILENAME = "CE-Definitions";

        CreateDefinitionDiagnostics(_ceBaseDefinitions, BASE_FILENAME);
    }

    private static void CreateDefinitionDiagnostics([CanBeNull] BaseDefinition[] baseDefinitions, string baseFilename)
    {
        if (baseDefinitions == null)
        {
            return;
        }

        Main.EnsureFolderExists(DiagnosticsFolder);

        /////////////////////////////////////////////////////////////////////////////////////////////////
        // Write all definitions with no GUI presentation to file
        File.WriteAllLines(Path.Combine(DiagnosticsFolder, $"{baseFilename}-GuiPresentation-MissingValue.txt"),
            baseDefinitions
                .Where(d => string.IsNullOrWhiteSpace(d.GuiPresentation?.Title) ||
                            string.IsNullOrWhiteSpace(d.GuiPresentation?.Description))
                .Distinct()
                .Select(d =>
                    $"{d.Name}:\tTitle='{d.GuiPresentation?.Title ?? string.Empty}', Desc='{d.GuiPresentation?.Description ?? string.Empty}'"));

        /////////////////////////////////////////////////////////////////////////////////////////////////
        // Write all definitions with GUI presentation but missing translation to file
        var languageSourceData = LocalizationManager.Sources[0];
        var currentLanguage = LocalizationManager.CurrentLanguageCode;
        var languageIndex = languageSourceData.GetLanguageIndexFromCode(currentLanguage);
        var invalidSyntaxTerms = new List<string>();

        var allLines = baseDefinitions
            .Select(d => new[]
            {
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

        File.WriteAllLines(
            Path.Combine(DiagnosticsFolder,
                $"{baseFilename}-GuiPresentation-MissingTranslation-{currentLanguage}.txt"), allLines);
        File.WriteAllLines(
            Path.Combine(DiagnosticsFolder,
                $"{baseFilename}-GuiPresentation-InvalidSyntaxTranslation-{currentLanguage}.txt"),
            invalidSyntaxTerms);
    }
#endif
}
