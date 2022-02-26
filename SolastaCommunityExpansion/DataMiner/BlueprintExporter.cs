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
        // private const string BlueprintFolder = "SolastaBlueprints";
        private static BlueprintExporter Exporter;

        internal static string ExportName { get; private set; }

        internal static HashSet<BaseDefinition> BaseDefinitions { get; private set; }

        internal static Dictionary<Type, List<BaseDefinition>> BaseDefinitionsMap { get; private set; }

        internal static string Path { get; private set; }

        internal static float PercentageComplete { get; private set; }

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

        internal static void ExportBlueprints(
            string exportName,
            HashSet<BaseDefinition> baseDefinitions,
            Dictionary<Type, List<BaseDefinition>> baseDefinitionsMap,
            string path,
            bool useSingleFile = false)
        {
            if (baseDefinitionsMap == null || baseDefinitions == null || PercentageComplete > 0)
            {
                return;
            }

            PercentageComplete = 1 / 1000f;
            ExportName = exportName;
            BaseDefinitions = baseDefinitions;
            BaseDefinitionsMap = baseDefinitionsMap;
            Path = path;

            if (useSingleFile)
            {
                Singleton.StartCoroutine(ExportSingle());
            }
            else
            {
                Singleton.StartCoroutine(ExportMany());
            }
        }

        private static IEnumerator ExportMany()
        {
            EnsureFolderExists(Path);

            // Types.txt
            using (var sw = new StreamWriter($"{Path}/Types.txt"))
            {
                foreach (var type in BaseDefinitions.Select(t => t.GetType()).Distinct().OrderBy(t => t.Name))
                {
                    sw.WriteLine($"{type.FullName}");
                }
            }

            // Assets.txt
            using (var sw = new StreamWriter($"{Path}/Assets.txt"))
            {
                sw.WriteLine("{0}\t{1}\t{2}\t{3}", "Name", "Type", "DatabaseType", "GUID");

                foreach (var db in BaseDefinitionsMap.OrderBy(db => db.Key.FullName))
                {
                    foreach (var definition in db.Value)
                    {
                        sw.WriteLine("{0}\t{1}\t{2}\t{3}", definition.Name, definition.GetType().FullName, db.Key.FullName, definition.GUID);
                    }
                }
            }

            var total = BaseDefinitions.Count;

            // Blueprints/definitions
            foreach (var d in BaseDefinitions.Select((d, i) => new { Definition = d, Index = i }))
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

                EnsureFolderExists($"{Path}/{subfolder}");

                var filename = $"{value.Name}.{value.GUID}.json";
                var folder = $"{Path}/{subfolder}";

                var path = $"{folder}/{filename}";
                if (folder.Length + filename.Length > 250)
                {
                    Main.Log($"Shortened path {path}, to {folder}/{value.GUID}.json");
                    path = $"{folder}/{value.GUID}.json";
                }

                try
                {
                    using StreamWriter sw = new StreamWriter(path);
                    using JsonWriter writer = new JsonTextWriter(sw);
                    serializer.Serialize(writer, d.Definition);
                }
                catch (Exception ex)
                {
                    Main.Error(ex);
                }

                PercentageComplete = ((float)d.Index / total);

                yield return null;
            }

            PercentageComplete = 0;
            ExportName = "";
        }

        private static IEnumerator ExportSingle()
        {
            using StreamWriter sw = new StreamWriter(Path);
            using JsonWriter writer = new JsonTextWriter(sw);

            // NOTE: currently needs to be assembled one definition at a time into a single file
            // Problem 1) serializing the whole array doesn't emit everything
            // Problem 2) serializing into individual files exceeds the folder path limit because some CE definitions have very long namessssssssss....
            sw.WriteLine("[");

            var definitions = BaseDefinitions.OrderBy(d => d.Name).ThenBy(d => d.GetType().Name);
            var lastDefinition = definitions.Last();
            var total = definitions.Count();

            foreach (var d in definitions.Select((d, i) => new { Definition = d, Index = i }))
            {
                // Don't put this outside the loop or it caches objects already serialized and then outputs a reference instead 
                // of the whole object.
                var serializer = JsonSerializer.Create(JsonUtil.CreateSettings(PreserveReferencesHandling.None));

                serializer.Serialize(writer, d.Definition);

                if (d.Definition != lastDefinition)
                {
                    sw.WriteLine(",");
                }

                PercentageComplete = ((float)d.Index / total);

                yield return null;
            }

            sw.WriteLine("]");

            PercentageComplete = 0;
            ExportName = "";
        }
    }
}
