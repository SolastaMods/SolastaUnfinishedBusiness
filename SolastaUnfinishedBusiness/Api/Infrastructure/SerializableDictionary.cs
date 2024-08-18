using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using SolastaUnfinishedBusiness.Api.ModKit.Utility;

namespace SolastaUnfinishedBusiness.Api.Infrastructure;

[XmlRoot("SerializableDictionary")]
public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, IXmlSerializable, IUpdatableSettings
{
    public SerializableDictionary()
    {
    }

    public SerializableDictionary(IDictionary<TKey, TValue> dictionary) : base(dictionary) { }

    public void AddMissingKeys(IUpdatableSettings from)
    {
        if (from is SerializableDictionary<TKey, TValue> fromDict)
        {
            // ReSharper disable once UsageOfDefaultStructEquality
            _ = this.Union(fromDict.Where(k => !ContainsKey(k.Key))).ToDictionary(k => k.Key, v => v.Value);
        }
    }

    public XmlSchema GetSchema()
    {
        return null;
    }

    public void ReadXml(XmlReader reader)
    {
        XmlSerializer keySerializer = new(typeof(TKey));
        XmlSerializer valueSerializer = new(typeof(TValue));

        var wasEmpty = reader.IsEmptyElement;

        reader.Read();

        if (wasEmpty)
        {
            return;
        }

        while (reader.NodeType != XmlNodeType.EndElement)
        {
            reader.ReadStartElement("item");
            reader.ReadStartElement("key");

            var key = (TKey)keySerializer.Deserialize(reader);

            reader.ReadEndElement();
            reader.ReadStartElement("value");

            var value = (TValue)valueSerializer.Deserialize(reader);

            reader.ReadEndElement();

            Add(key, value);

            reader.ReadEndElement();
            reader.MoveToContent();
        }

        reader.ReadEndElement();
    }

    public void WriteXml(XmlWriter writer)
    {
        XmlSerializer keySerializer = new(typeof(TKey));
        XmlSerializer valueSerializer = new(typeof(TValue));

        foreach (var key in Keys)
        {
            writer.WriteStartElement("item");
            writer.WriteStartElement("key");
            keySerializer.Serialize(writer, key);
            writer.WriteEndElement();
            writer.WriteStartElement("value");

            var value = this[key];

            valueSerializer.Serialize(writer, value);
            writer.WriteEndElement();
            writer.WriteEndElement();
        }
    }
}
