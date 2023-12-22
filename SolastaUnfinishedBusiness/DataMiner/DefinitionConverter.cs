#if DEBUG
using System;
using Newtonsoft.Json;

namespace SolastaUnfinishedBusiness.DataMiner;

internal class DefinitionConverter : JsonConverter
{
    private bool _cannotWrite;

    public override bool CanWrite => !_cannotWrite;

    public override bool CanRead => true;

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        using (new PushValue<bool>(true, () => _cannotWrite, canWrite => _cannotWrite = canWrite))
        {
            serializer.Serialize(writer, value);
        }
    }

    public override object ReadJson(
        JsonReader reader,
        Type objectType,
        object existingValue,
        JsonSerializer serializer)
    {
        return null;
    }

    public override bool CanConvert(Type objectType)
    {
        return typeof(BaseDefinition).IsAssignableFrom(objectType);
    }
}
#endif
