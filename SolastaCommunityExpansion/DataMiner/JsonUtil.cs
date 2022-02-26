using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace SolastaCommunityExpansion.DataMiner
{
    public static class JsonUtil
    {
        public static JsonSerializerSettings CreateSettings(PreserveReferencesHandling referencesHandling)
        {
            var refJsonSerializerSettings = new JsonSerializerSettings
            {
                CheckAdditionalContent = false,
                ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
                ContractResolver = new DataminerContractResolver(),
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                DateParseHandling = DateParseHandling.DateTime,
                DateTimeZoneHandling = DateTimeZoneHandling.RoundtripKind,
                DefaultValueHandling = DefaultValueHandling.Include,
                FloatFormatHandling = FloatFormatHandling.String,
                FloatParseHandling = FloatParseHandling.Double,
                Formatting = Formatting.Indented,
                MetadataPropertyHandling = MetadataPropertyHandling.ReadAhead,
                MissingMemberHandling = MissingMemberHandling.Ignore,
                NullValueHandling = NullValueHandling.Include,
                ObjectCreationHandling = ObjectCreationHandling.Replace,
                PreserveReferencesHandling = referencesHandling,
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                StringEscapeHandling = StringEscapeHandling.Default,
                TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
                TypeNameHandling = TypeNameHandling.Objects
            };

            refJsonSerializerSettings.Converters.Add(new StringEnumConverter());

            return refJsonSerializerSettings;
        }

        public static void CEBlueprintDump(IEnumerable<BaseDefinition> definitions, string path, Action<int, int> progress)
        {
            using StreamWriter sw = new StreamWriter(path);
            using JsonWriter writer = new JsonTextWriter(sw);

            // NOTE: currently needs to be assembled one definition at a time into a single file
            // Problem 1) serializing the whole array doesn't emit everything
            // Problem 2) serializing into individual files exceeds the folder path limit because some CE definitions have very long namessssssssss....
            sw.WriteLine("[");
            var lastDefinition = definitions.Last();
            int total = definitions.Count();

            foreach (var d in definitions.Select((d, i) => new { Definition = d, Index = i }))
            {
                JsonSerializer serializer = JsonSerializer.Create(CreateSettings(PreserveReferencesHandling.None));
                serializer.Serialize(writer, d.Definition);
                if (d.Definition != lastDefinition)
                {
                    sw.WriteLine(",");
                }

                progress(d.Index, total);
            }

            sw.WriteLine("]");

            progress(total, total);
        }

        public static void TABlueprintDump(BaseDefinition definition, string path)
        {
            // This crashes if PreserveReferencesHandling.None is set - TODO: find out why
            JsonSerializer serializer = JsonSerializer.Create(CreateSettings(PreserveReferencesHandling.Objects));
            using StreamWriter sw = new StreamWriter(path);
            using JsonWriter writer = new JsonTextWriter(sw);
            serializer.Serialize(writer, definition);
        }

        public static bool IsBlacklisted(MemberInfo _)
        {
            return false;
        }

        public static List<MemberInfo> GetUnitySerializableMembers(Type objectType)
        {
            if (objectType == null)
            {
                return new List<MemberInfo>();
            }

            IEnumerable<MemberInfo> publicFields = objectType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
                .Where(f => !f.IsInitOnly);

            IEnumerable<MemberInfo> privateFields = objectType.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);

            IEnumerable<MemberInfo> newtonsoftFields = objectType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly)
                .Where(f => ((f.IsPublic && f.IsInitOnly) || f.IsPrivate) && Attribute.IsDefined(f, typeof(JsonPropertyAttribute)));

            IEnumerable<MemberInfo> newtonsoftProperties = objectType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly)
                .Where(p => Attribute.IsDefined(p, typeof(JsonPropertyAttribute)));

            IEnumerable<MemberInfo> nameProperty = objectType == typeof(UnityEngine.Object) ?
                    new MemberInfo[] { objectType.GetProperty("name") } :
                    Array.Empty<MemberInfo>();

            return privateFields
                .Where((field) => Attribute.IsDefined(field, typeof(SerializeField)))
                .Concat(publicFields)
                .Concat(GetUnitySerializableMembers(objectType.BaseType))
                .Concat(nameProperty)
                .Concat(newtonsoftProperties)
                .Concat(newtonsoftFields)
                .Where(field => !IsBlacklisted(field))
                .ToList();
        }
    }
}
