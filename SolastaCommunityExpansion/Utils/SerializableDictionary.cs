using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace SolastaCommunityExpansion.Utils
{
    // Generic types should not contain static fields, or they get copied for every instance of the type
    static class SerializableDictionary
    {
        public const string ItemTag = "item";
        public const string KeyTag = "key";
        public const string ValueTag = "value";
    }

    /// <summary>
    /// Base on https://weblogs.asp.net/pwelter34/444961
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    [XmlRoot("dictionary")]
#pragma warning disable S3925 // "ISerializable" should be implemented correctly
    public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, IXmlSerializable
#pragma warning restore S3925 // "ISerializable" should be implemented correctly
    {
        // XmlSerializer.Deserialize() will create a new Object, and then call ReadXml()
        // So cannot use instance field, use class field

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            if (reader.IsEmptyElement)
            {
                return;
            }

            var keySerializer = new XmlSerializer(typeof(TKey));
            var valueSerializer = new XmlSerializer(typeof(TValue));

            reader.ReadStartElement();

            // IsStartElement() will call MoveToContent()
            while (reader.IsStartElement(SerializableDictionary.ItemTag))
            {
                reader.ReadStartElement(SerializableDictionary.ItemTag);

                reader.ReadStartElement(SerializableDictionary.KeyTag);
                TKey key = (TKey)keySerializer.Deserialize(reader);
                reader.ReadEndElement();

                reader.ReadStartElement(SerializableDictionary.ValueTag);
                TValue value = (TValue)valueSerializer.Deserialize(reader);
                reader.ReadEndElement();

                reader.ReadEndElement();
                this.Add(key, value);
            }

            reader.ReadEndElement();
        }

        public void WriteXml(XmlWriter writer)
        {
            var keySerializer = new XmlSerializer(typeof(TKey));
            var valueSerializer = new XmlSerializer(typeof(TValue));

            foreach (var kvp in this)
            {
                writer.WriteStartElement(SerializableDictionary.ItemTag);

                writer.WriteStartElement(SerializableDictionary.KeyTag);
                keySerializer.Serialize(writer, kvp.Key);
                writer.WriteEndElement();

                writer.WriteStartElement(SerializableDictionary.ValueTag);
                valueSerializer.Serialize(writer, kvp.Value);
                writer.WriteEndElement();

                writer.WriteEndElement();
            }
        }
    }
}
