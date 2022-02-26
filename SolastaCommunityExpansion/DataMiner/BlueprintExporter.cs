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
            string path)
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

            Singleton.StartCoroutine(ExportMany());
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

                if (path.Length > MAX_PATH_LENGTH)
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
    }
}
