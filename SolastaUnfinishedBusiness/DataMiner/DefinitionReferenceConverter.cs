#if DEBUG
using System;
using Newtonsoft.Json;

namespace SolastaUnfinishedBusiness.DataMiner;

internal class DefinitionReferenceConverter : JsonConverter
{
    private static readonly Type TypeOfBaseDefinition = typeof(BaseDefinition);

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
        return TypeOfBaseDefinition.IsAssignableFrom(objectType);
    }
}
#endif
