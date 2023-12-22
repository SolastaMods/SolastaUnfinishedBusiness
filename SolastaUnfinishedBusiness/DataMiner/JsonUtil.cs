#if DEBUG
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SolastaUnfinishedBusiness.DataMiner;

internal static class JsonUtil
{
    internal static JsonSerializerSettings CreateSettings(PreserveReferencesHandling referencesHandling)
    {
        var refJsonSerializerSettings = new JsonSerializerSettings
        {
            CheckAdditionalContent = false,
            ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
            ContractResolver = new DataMinerContractResolver(),
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

    internal static IEnumerable<MemberInfo> GetUnitySerializableMembers(Type objectType)
    {
        if (objectType == null)
        {
            return new List<MemberInfo>();
        }

        IEnumerable<MemberInfo> publicFields =
            objectType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
                .Where(f => !f.IsInitOnly);

        IEnumerable<MemberInfo> privateFields =
            objectType.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);

        IEnumerable<MemberInfo> newtonsoftFields =
            objectType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic |
                                 BindingFlags.DeclaredOnly)
                .Where(f => ((f.IsPublic && f.IsInitOnly) || f.IsPrivate) &&
                            Attribute.IsDefined(f, typeof(JsonPropertyAttribute)));

        IEnumerable<MemberInfo> newtonsoftProperties =
            objectType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic |
                                     BindingFlags.DeclaredOnly)
                .Where(p => Attribute.IsDefined(p, typeof(JsonPropertyAttribute)));

        IEnumerable<MemberInfo> nameProperty = objectType == typeof(Object)
            ? new MemberInfo[] { objectType.GetProperty("name") }
            : Array.Empty<MemberInfo>();

        return privateFields
            .Where(field => Attribute.IsDefined(field, typeof(SerializeField)))
            .Concat(publicFields)
            .Concat(GetUnitySerializableMembers(objectType.BaseType))
            .Concat(nameProperty)
            .Concat(newtonsoftProperties)
            .Concat(newtonsoftFields)
            .ToList();
    }
}
#endif
