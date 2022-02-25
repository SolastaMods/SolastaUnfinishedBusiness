using Newtonsoft.Json;
using System;

namespace SolastaCommunityExpansion.DataMiner
{
    public class DefinitionReferenceConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            try
            {
                var def = (BaseDefinition)value;
                writer.WriteValue(string.Format($"Definition:{def.Name}:{def.GUID}"));
            }
            catch (InvalidCastException ex)
            {
                writer.WriteValue(string.Format($"Error:{value?.GetType().FullName ?? "NULL"}:{ex}"));
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        private static readonly Type _tBaseDefinition = typeof(BaseDefinition);

        public override bool CanConvert(Type objectType)
        {
            return _tBaseDefinition.IsAssignableFrom(objectType);
        }
    }
}
