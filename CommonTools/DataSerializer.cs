using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace CommonTools
{
    public static class DataSerializer
    {
        public static string Serialize(object obj)
        {
            var result = string.Empty;
            _DCT.Execute(data =>
            {
                var serializer = new DataContractSerializer(obj.GetType());
                using (var memoryStream = new MemoryStream())
                {
                    var xmlWriterSettings = new XmlWriterSettings()
                    {
                        CloseOutput = false,
                        Encoding = Encoding.UTF8,
                        OmitXmlDeclaration = false,
                        Indent = true,
                    };
                    using (var xw = XmlWriter.Create(memoryStream, xmlWriterSettings))
                        serializer.WriteObject(xw, obj);
                    result = Encoding.UTF8.GetString(memoryStream.ToArray());
                }

            });
            return result;
        }

        public static T Deserialize<T>(string xml)
        {
            T result = default(T);
            using (var stream = new MemoryStream())
            {
                var data = Encoding.UTF8.GetBytes(xml);
                var deserializer = new DataContractSerializer(typeof(T));
                var reader = XmlDictionaryReader.CreateTextReader(data, new XmlDictionaryReaderQuotas());
                result = (T)deserializer.ReadObject(reader);
            }
            return result;
        }
    }
}
