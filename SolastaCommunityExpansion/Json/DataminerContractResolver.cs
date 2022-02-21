using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SolastaCommunityExpansion.Json
{
    internal class DataminerContractResolver : DefaultContractResolver
    {
        private readonly DefinitionReferenceConverter DefinitionReferenceConverter = new DefinitionReferenceConverter();

        private readonly DefinitionConverter DefinitionConverter = new DefinitionConverter();

        protected override List<MemberInfo> GetSerializableMembers(Type objectType)
        {
            return JsonUtil.GetUnitySerializableMembers(objectType).Distinct().ToList();
        }

        protected override JsonConverter ResolveContractConverter(Type objectType)
        {
            if (objectType == null)
            {
                return null;
            }

            if (typeof(BaseDefinition).IsAssignableFrom(objectType))
            {
                if (DefinitionConverter.CanRead && DefinitionConverter.CanWrite)
                {
                    return DefinitionConverter;
                }
                else
                {
                    return DefinitionReferenceConverter;
                }
            }

            return null;
        }

        protected override JsonContract CreateContract(Type objectType)
        {
            var contract = base.CreateContract(objectType);
            if (typeof(BaseDefinition).IsAssignableFrom(objectType))
            {
                contract.IsReference = false;
                contract.OnSerializedCallbacks.Add((_, __) => contract.Converter = DefinitionConverter);
                contract.OnSerializingCallbacks.Add((_, __) => contract.Converter = DefinitionReferenceConverter);
                contract.OnDeserializedCallbacks.Add((_, __) => contract.Converter = DefinitionConverter);
                contract.OnDeserializingCallbacks.Add((_, __) => contract.Converter = DefinitionReferenceConverter);
            }
            return contract;
        }

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var jsonProp = base.CreateProperty(member, memberSerialization);
            if (member is FieldInfo)
            {
                jsonProp.Readable = true;
                jsonProp.Writable = true;
            }
            return jsonProp;
        }
    }
}
