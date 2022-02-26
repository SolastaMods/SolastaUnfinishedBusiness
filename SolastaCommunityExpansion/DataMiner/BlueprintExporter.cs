using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace SolastaCommunityExpansion.DataMiner
{
    internal class OfficialBlueprintExporter : MonoBehaviour
    {
        private static HashSet<BaseDefinition> BaseDefinitions;
        private static Dictionary<Type, List<BaseDefinition>> BaseDefinitionsMap;
        private IEnumerator Coroutine;
        private const string BlueprintFolder = "SolastaBlueprints";
        private static OfficialBlueprintExporter Exporter;

        internal float PercentageComplete { get; set; }

        internal static OfficialBlueprintExporter Shared
        {
            get
            {
                if (Exporter == null)
                {
                    Exporter = new GameObject().AddComponent<OfficialBlueprintExporter>();
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

        internal void ExportBlueprints(HashSet<BaseDefinition> baseDefinitions, Dictionary<Type, List<BaseDefinition>> baseDefinitionsMap)
        {
            if (Coroutine != null)
            {
                return;
            }

            BaseDefinitions = baseDefinitions;
            BaseDefinitionsMap = baseDefinitionsMap;

            Coroutine = Export();
            StartCoroutine(Coroutine);
        }

        internal IEnumerator Export()
        {
            EnsureFolderExists(BlueprintFolder);

            // Types.txt
            using (var sw = new StreamWriter($"{BlueprintFolder}/Types.txt"))
            {
                foreach (var type in BaseDefinitions.Select(t => t.GetType()).Distinct().OrderBy(t => t.Name))
                {
                    sw.WriteLine($"{type.FullName}");
                }
            }

            // Assets.txt
            using (var sw = new StreamWriter($"{BlueprintFolder}/Assets.txt"))
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

            var serializer = JsonSerializer.Create(JsonUtil.CreateSettings(PreserveReferencesHandling.Objects));
            var total = BaseDefinitions.Count;

            // Blueprints/definitions
            foreach (var d in BaseDefinitions.Select((d, i) => new { Definition = d, Index = i }))
            {
                var dbType = d.Definition.GetType();
                var value = d.Definition;
                var subfolder = value.GetType().Name;
                if (value.GetType() != dbType)
                {
                    subfolder = $"{dbType.FullName}/{subfolder}";
                }

                EnsureFolderExists($"{BlueprintFolder}/{subfolder}");

                var path = $"{BlueprintFolder}/{subfolder}/{value.Name}.{value.GUID}.json";

                using StreamWriter sw = new StreamWriter(path);
                using JsonWriter writer = new JsonTextWriter(sw);
                serializer.Serialize(writer, d.Definition);

                PercentageComplete = ((float)d.Index / total);

                yield return null;
            }

            Coroutine = null;
            PercentageComplete = 0;
        }
    }

    internal class ModBlueprintExporter : MonoBehaviour
    {
        private static IEnumerable<BaseDefinition> Definitions;
        private static string Path;
        private IEnumerator Coroutine;
        private static ModBlueprintExporter Exporter;

        internal float PercentageComplete { get; set; }

        internal static ModBlueprintExporter Shared
        {
            get
            {
                if (Exporter == null)
                {
                    Exporter = new GameObject().AddComponent<ModBlueprintExporter>();
                    DontDestroyOnLoad(Exporter.gameObject);
                }
                return Exporter;
            }
        }

        internal void ExportBlueprints(IEnumerable<BaseDefinition> definitions, string path)
        {
            if (Coroutine != null)
            {
                return;
            }

            Definitions = definitions;
            Path = path;

            Coroutine = Export();
            StartCoroutine(Coroutine);
        }

        internal IEnumerator Export()
        {
            using StreamWriter sw = new StreamWriter(Path);
            using JsonWriter writer = new JsonTextWriter(sw);

            // NOTE: currently needs to be assembled one definition at a time into a single file
            // Problem 1) serializing the whole array doesn't emit everything
            // Problem 2) serializing into individual files exceeds the folder path limit because some CE definitions have very long namessssssssss....
            sw.WriteLine("[");

            var lastDefinition = Definitions.Last();
            var total = Definitions.Count();

            foreach (var d in Definitions.Select((d, i) => new { Definition = d, Index = i }))
            {
                JsonSerializer serializer = JsonSerializer.Create(JsonUtil.CreateSettings(PreserveReferencesHandling.None));
                serializer.Serialize(writer, d.Definition);
                if (d.Definition != lastDefinition)
                {
                    sw.WriteLine(",");
                }

                PercentageComplete = ((float)d.Index / total);

                yield return null;
            }

            sw.WriteLine("]");

            Coroutine = null;
            PercentageComplete = 0;
        }
    }
}
