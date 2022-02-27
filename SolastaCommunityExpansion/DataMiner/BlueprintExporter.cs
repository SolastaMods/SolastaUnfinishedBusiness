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
        private const int MAX_PATH_LENGTH = 260;

        private class ExportStatus
        {
            internal Coroutine coroutine;
            internal float percentageComplete;
        }

        private static readonly Dictionary<int, ExportStatus> CurrentExports = new();

        private static BlueprintExporter Exporter;

        private static BlueprintExporter Singleton
        {
            get
            {
                if (Exporter == null)
                {
                    Exporter = new GameObject().AddComponent<BlueprintExporter>();
                    DontDestroyOnLoad(Exporter.gameObject);
                }

                return Exporter;
            }
        }

        private static void EnsureFolderExists(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        internal static float PercentageComplete(int exportId)
        {
            if (CurrentExports.ContainsKey(exportId))
            {
                return CurrentExports[exportId].percentageComplete;
            }

            return 0f;
        }

        internal static void Cancel(int exportId)
        {
            if (!CurrentExports.ContainsKey(exportId))
            {
                return;
            }

            Singleton.StopCoroutine(CurrentExports[exportId].coroutine);
            CurrentExports.Remove(exportId);
        }

        internal static void ExportBlueprints(
            int exportId,
            HashSet<BaseDefinition> baseDefinitions,
            Dictionary<Type, List<BaseDefinition>> baseDefinitionsMap,
            string path)
        {
            if (baseDefinitionsMap == null || baseDefinitions == null || CurrentExports.ContainsKey(exportId))
            {
                return;
            }

            CurrentExports.Add(
                exportId,
                new ExportStatus
                {
                    percentageComplete = 0.0001f,
                    coroutine = Singleton.StartCoroutine(ExportMany(
                        exportId,
                        baseDefinitions,
                        baseDefinitionsMap,
                        path))
                }
            );
        }

        private static IEnumerator ExportMany(
            int exportId,
            HashSet<BaseDefinition> baseDefinitions,
            Dictionary<Type, List<BaseDefinition>> baseDefinitionsMap,
            string path)
        {
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

            var total = baseDefinitions.Count;

            // Blueprints/definitions
            foreach (var d in baseDefinitions.Select((d, i) => new { Definition = d, Index = i }))
            {
                // Don't put this outside the loop or it caches objects already serialized and then outputs a reference instead 
                // of the whole object.
                var serializer = JsonSerializer.Create(JsonUtil.CreateSettings(PreserveReferencesHandling.Objects));

                var dbType = d.Definition.GetType();
                var value = d.Definition;
                var subfolder = value.GetType().Name;

                if (value.GetType() != dbType)
                {
                    subfolder = $"{dbType.FullName}/{subfolder}";
                }

                EnsureFolderExists($"{path}/{subfolder}");

                var filename = $"{value.Name}.{value.GUID}.json";
                var folder = $"{path}/{subfolder}";
                var fullFilename = $"{folder}/{filename}";

                if (fullFilename.Length > MAX_PATH_LENGTH)
                {
                    Main.Log($"Shortened path {fullFilename}, to {folder}/{value.GUID}.json");
                    fullFilename = $"{folder}/{value.GUID}.json";
                }

                try
                {
                    using StreamWriter sw = new StreamWriter(fullFilename);
                    using JsonWriter writer = new JsonTextWriter(sw);
                    serializer.Serialize(writer, d.Definition);
                }
                catch (Exception ex)
                {
                    Main.Error(ex);
                }

                CurrentExports[exportId].percentageComplete = (float)d.Index / total;

                yield return null;
            }

            CurrentExports.Remove(exportId);
        }
    }
}
