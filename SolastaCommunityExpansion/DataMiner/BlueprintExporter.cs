#if DEBUG
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace SolastaCommunityExpansion.DataMiner
{
    internal class BlueprintExporter : MonoBehaviour
    {
        private const int MAX_PATH_LENGTH = 250;

        private static BlueprintExporter exporter;

        private static BlueprintExporter Exporter
        {
            get
            {
                if (exporter == null)
                {
                    exporter = new GameObject().AddComponent<BlueprintExporter>();
                    DontDestroyOnLoad(exporter.gameObject);
                }

                return exporter;
            }
        }

        internal struct ExportStatus
        {
            internal Coroutine coroutine;
            internal float percentageComplete;
        }

        internal static readonly ExportStatus[] CurrentExports = new ExportStatus[3];

        private static void EnsureFolderExists(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        private static void SetExport(int exportId, Coroutine coroutine, float percentageComplete)
        {
            CurrentExports[exportId].coroutine = coroutine;
            CurrentExports[exportId].percentageComplete = percentageComplete;
        }

        internal static void Cancel(int exportId)
        {
            if (CurrentExports[exportId].coroutine == null)
            {
                return;
            }

            Exporter.StopCoroutine(CurrentExports[exportId].coroutine);
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
            if (baseDefinitionsMap == null || baseDefinitions == null || CurrentExports[exportId].coroutine != null)
            {
                return;
            }

            var coroutine = ExportMany(exportId, baseDefinitions, baseDefinitionsMap, baseDefinitionAndCopy, exportOriginalCopy, path);

            SetExport(exportId, Exporter.StartCoroutine(coroutine), 0f);
        }

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

            EnsureFolderExists(path);

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
                var foldername = $"{path}/{definitionType}";
                var fullname = $"{foldername}/{filename}";

                EnsureFolderExists(foldername);

                if (fullname.Length > MAX_PATH_LENGTH)
                {
                    Main.Log($"Shortened path {fullname}, to {foldername}/{definition.GUID}.json");
                    fullname = $"{foldername}/{definition.GUID}.json";
                }

                try
                {
                    // Don't put this outside the loop or it caches objects already serialized and then outputs a reference instead 
                    // of the whole object.
                    var serializer = JsonSerializer.Create(JsonUtil.CreateSettings(PreserveReferencesHandling.None));

                    using StreamWriter sw = new StreamWriter(fullname);
                    using JsonWriter writer = new JsonTextWriter(sw);

                    var def = exportOriginalCopy ? GetDefinitionCopy(definition) : definition;
                    serializer.Serialize(writer, def);
                }
                catch (Exception ex)
                {
                    Main.Error(ex);
                }

                CurrentExports[exportId].percentageComplete = (float)i / total;

                yield return null;
            }

            SetExport(exportId, null, 0f);

            Main.Log($"Export finished: {DateTime.UtcNow}, {DateTime.UtcNow - start}.");

            BaseDefinition GetDefinitionCopy(BaseDefinition definition)
            {
                if(baseDefinitionAndCopy == null)
                {
                    return definition;
                }

                if(baseDefinitionAndCopy.TryGetValue(definition, out var copy))
                {
                    return copy;
                }

                // NOTE: some definitions won't be found when creating Assets.txt because they're explicitly excluded from export.
                // Assuming we won't have modified the excluded ones.

                return definition;
            }
        }
    }
}
#endif
