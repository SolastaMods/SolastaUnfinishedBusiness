#if DEBUG
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

namespace SolastaUnfinishedBusiness.DataMiner;

internal class BlueprintExporter : MonoBehaviour
{
    private const int MaxPathLength = 250;

    private static BlueprintExporter _exporter;

    internal static readonly ExportStatus[] CurrentExports = new ExportStatus[3];

    private static BlueprintExporter Exporter
    {
        get
        {
            if (_exporter != null)
            {
                return _exporter;
            }

            _exporter = new GameObject().AddComponent<BlueprintExporter>();
            DontDestroyOnLoad(_exporter.gameObject);

            return _exporter;
        }
    }

    private static void SetExport(int exportId, Coroutine coroutine, float percentageComplete)
    {
        CurrentExports[exportId].Coroutine = coroutine;
        CurrentExports[exportId].PercentageComplete = percentageComplete;
    }

    internal static void Cancel(int exportId)
    {
        if (CurrentExports[exportId].Coroutine == null)
        {
            return;
        }

        Exporter.StopCoroutine(CurrentExports[exportId].Coroutine);
        SetExport(exportId, null, 0f);
    }

    internal static void ExportBlueprints(
        int exportId,
        BaseDefinition[] baseDefinitions,
        Dictionary<Type, BaseDefinition[]> baseDefinitionsMap,
        Dictionary<BaseDefinition, BaseDefinition> baseDefinitionAndCopy,
        bool exportOriginalCopy,
        string path)
    {
        if (baseDefinitionsMap == null || baseDefinitions == null || CurrentExports[exportId].Coroutine != null)
        {
            return;
        }

        var coroutine =
            ExportMany(exportId, baseDefinitions, baseDefinitionsMap, baseDefinitionAndCopy, exportOriginalCopy,
                path);

        SetExport(exportId, Exporter.StartCoroutine(coroutine), 0f);
    }

    // ReSharper disable once SuggestBaseTypeForParameter
    private static IEnumerator ExportMany(
        int exportId,
        BaseDefinition[] baseDefinitions,
        Dictionary<Type, BaseDefinition[]> baseDefinitionsMap,
        Dictionary<BaseDefinition, BaseDefinition> baseDefinitionAndCopy,
        bool exportOriginalCopy,
        string path)
    {
        var start = DateTime.UtcNow;

        Main.Log($"Export started: {DateTime.UtcNow:G}");

        yield return null;

        Main.EnsureFolderExists(path);

        // Types.txt
        using (var sw = new StreamWriter($"{path}/Types.txt"))
        {
            foreach (var type in baseDefinitions.Select(t => t.GetType()).Distinct().OrderBy(t => t.FullName))
            {
                sw.WriteLine($"{type.FullName}");
            }
        }

        yield return null;

        // Assets.txt
        using (var sw = new StreamWriter($"{path}/Assets.txt"))
        {
            sw.WriteLine("{0}\t{1}\t{2}\t{3}", "Name", "Type", "DatabaseType", "GUID");

            foreach (var db in baseDefinitionsMap.OrderBy(kvp => kvp.Key.Name))
            {
                foreach (var definition in db.Value.OrderBy(d => d.Name).ThenBy(d => d.GetType().FullName))
                {
                    var def = GetDefinitionCopy(definition);
                    sw.WriteLine("{0}\t{1}\t{2}\t{3}", def.Name, def.GetType().FullName, db.Key.FullName, def.GUID);
                }
            }
        }

        yield return null;

        var total = baseDefinitions.Length;

        // Blueprints/definitions
        for (var i = 0; i < total; i++)
        {
            var definition = baseDefinitions[i];
            var definitionType = definition.GetType().Name;

            var filename = $"{definition.Name}.json";
            var folderName = $"{path}/{definitionType}";
            var fullname = $"{folderName}/{filename}";

            Main.EnsureFolderExists(folderName);

            if (fullname.Length > MaxPathLength)
            {
                Main.Log($"Shortened path {fullname}, to {folderName}/{definition.GUID}.json");
                fullname = $"{folderName}/{definition.GUID}.json";
            }

            try
            {
                // Don't put this outside the loop or it caches objects already serialized and then outputs a reference instead 
                // of the whole object.
                var serializer = JsonSerializer.Create(JsonUtil.CreateSettings(PreserveReferencesHandling.None));

                using var sw = new StreamWriter(fullname);
                using JsonWriter writer = new JsonTextWriter(sw);

                var def = exportOriginalCopy ? GetDefinitionCopy(definition) : definition;
                serializer.Serialize(writer, def);
            }
            catch (Exception ex)
            {
                Main.Error(ex);
            }

            CurrentExports[exportId].PercentageComplete = (float)i / total;

            yield return null;
        }

        SetExport(exportId, null, 0f);

        Main.Log($"Export finished: {DateTime.UtcNow}, {DateTime.UtcNow - start}.");

        yield break;

        BaseDefinition GetDefinitionCopy(BaseDefinition definition)
        {
            if (baseDefinitionAndCopy == null)
            {
                return definition;
            }

            return baseDefinitionAndCopy.TryGetValue(definition, out var copy) ? copy : definition;

            // NOTE: some definitions won't be found when creating Assets.txt because they're explicitly excluded from export.
            // Assuming we won't have modified the excluded ones.
        }
    }

    internal struct ExportStatus
    {
        internal Coroutine Coroutine;
        internal float PercentageComplete;
    }
}
#endif
