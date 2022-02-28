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

        internal static readonly ExportStatus[] CurrentExports = new ExportStatus[2];

        private static void EnsureFolderExists(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        internal static void Cancel(int exportId)
        {
            if (CurrentExports[exportId].percentageComplete == 0)
            {
                return;
            }

            Exporter.StopCoroutine(CurrentExports[exportId].coroutine);
            CurrentExports[exportId].percentageComplete = 0f;
        }

        internal static void ExportBlueprints(
            int exportId,
            BaseDefinition[] baseDefinitions,
            Dictionary<Type, BaseDefinition[]> baseDefinitionsMap,
            string path)
        {
            if (baseDefinitionsMap == null || baseDefinitions == null || CurrentExports[exportId].percentageComplete > 0)
            {
                return;
            }

            CurrentExports[exportId].percentageComplete = 0.001f;
            CurrentExports[exportId].coroutine = Exporter.StartCoroutine(
                ExportMany(
                    exportId,
                    baseDefinitions,
                    baseDefinitionsMap,
                    path));
        }

        private static IEnumerator ExportMany(
            int exportId,
            BaseDefinition[] baseDefinitions,
            Dictionary<Type, BaseDefinition[]> baseDefinitionsMap,
            string path)
        {
            var start = DateTime.UtcNow;
            Main.Log($"Export started: {DateTime.UtcNow}");

            yield return null;

            EnsureFolderExists(path);

            // Types.txt
            using (var sw = new StreamWriter($"{path}/Types.txt"))
            {
                foreach (var type in baseDefinitions.Select(t => t.GetType()).Distinct().OrderBy(t => t.Name))
                {
                    sw.WriteLine($"{type.FullName}");
                }
            }

            yield return null;

            // Assets.txt
            using (var sw = new StreamWriter($"{path}/Assets.txt"))
            {
                sw.WriteLine("{0}\t{1}\t{2}\t{3}", "Name", "Type", "DatabaseType", "GUID");

                foreach (var db in baseDefinitionsMap.OrderBy(db => db.Key.FullName))
                {
                    foreach (var definition in db.Value)
                    {
                        sw.WriteLine("{0}\t{1}\t{2}\t{3}", definition.Name, definition.GetType().FullName, db.Key.FullName, definition.GUID);
                    }
                }
            }

            yield return null;

            var total = baseDefinitions.Length;

            // Blueprints/definitions
            for (var i = 0; i < total; i++)
            {
                var definition = baseDefinitions[i];
                var dbType = definition.GetType();

                if (Array.IndexOf(Main.Settings.ExcludeFromExport, dbType) > 0)
                {
                    continue;
                }

                // Don't put this outside the loop or it caches objects already serialized and then outputs a reference instead 
                // of the whole object.
                var serializer = JsonSerializer.Create(JsonUtil.CreateSettings(PreserveReferencesHandling.Objects));



                var subfolder = definition.GetType().Name;

                if (definition.GetType() != dbType)
                {
                    subfolder = $"{dbType.FullName}/{subfolder}";
                }

                EnsureFolderExists($"{path}/{subfolder}");

                var filename = $"{definition.Name}.json";
                var folder = $"{path}/{subfolder}";
                var fullFilename = $"{folder}/{filename}";

                if (fullFilename.Length > MAX_PATH_LENGTH)
                {
                    Main.Log($"Shortened path {fullFilename}, to {folder}/{definition.GUID}.json");
                    fullFilename = $"{folder}/{definition.GUID}.json";
                }

                try
                {
                    using StreamWriter sw = new StreamWriter(fullFilename);
                    using JsonWriter writer = new JsonTextWriter(sw);
                    serializer.Serialize(writer, definition);
                }
                catch (Exception ex)
                {
                    Main.Error(ex);
                }

                CurrentExports[exportId].percentageComplete = (float)i / total;

                yield return null;
            }

            CurrentExports[exportId].percentageComplete = 0f;

            Main.Log($"Export finished: {DateTime.UtcNow}, {DateTime.UtcNow - start}.");
        }
    }
}
