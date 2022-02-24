using Newtonsoft.Json;
using System;

namespace SolastaCommunityExpansion.Json
{
    public class DefinitionConverter : JsonConverter
    {
        private bool cannotWrite;

        public override bool CanWrite => !cannotWrite;

        public override bool CanRead => true;

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            using (new PushValue<bool>(true, () => cannotWrite, (canWrite) => cannotWrite = canWrite))
            {
                serializer.Serialize(writer, value);
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(BaseDefinition).IsAssignableFrom(objectType);
        }
    }
}
