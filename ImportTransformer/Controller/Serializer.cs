using ImportTransformer.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace ImportTransformer.Controller
{
    class Serializer
    {
        public static Documents DeSerializer(string file)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Documents));

            Documents doc;

            using (Stream reader = new FileStream(file, FileMode.Open))
            {
                doc = (Documents)serializer.Deserialize(reader);
            }

            return doc;
        }

        public static void SerializerXml(string path, Documents doc)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Documents));

            //Для корректировки неймспейса можно здесь записать это в строку, привести к билдеру и скорректировать принудительно.

            XmlWriterSettings settings = new XmlWriterSettings() { OmitXmlDeclaration = false, Indent = true, Encoding = new UTF8Encoding(false) };

            using (Stream writer = new FileStream(path, FileMode.OpenOrCreate))
            using (var wr = XmlWriter.Create(writer, settings))
            {
                var a = XmlWriter.Create(writer, settings);
                serializer.Serialize(a, doc);
            }

        }
    }
}
