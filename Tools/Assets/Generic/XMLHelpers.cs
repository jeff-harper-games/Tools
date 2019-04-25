using System.Xml.Serialization;
using System.IO;
using UnityEngine;

public static class XMLHelpers
{
    public static string SerializeObject<T>(this T toSerialize)
    {
        XmlSerializer xmlSerializer = null; 
        if (toSerialize != null)
            xmlSerializer = new XmlSerializer(toSerialize.GetType());

        using (StringWriter textWriter = new StringWriter())
        {
            xmlSerializer.Serialize(textWriter, toSerialize);
            return textWriter.ToString();
        }
    }

    public static T Deserialize<T>(this string toDeserialize)
    {
        XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
        using (StringReader textReader = new StringReader(toDeserialize))
        {
            return (T)xmlSerializer.Deserialize(textReader);
        }
    }
}
