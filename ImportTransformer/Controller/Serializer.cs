using ImportTransformer.Model;
using NLog;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace ImportTransformer.Controller
{
    class Serializer
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public static Documents DeSerializer(string file)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Documents));

            Documents doc;

            using (Stream reader = new FileStream(file, FileMode.Open))
            {
                doc = (Documents)serializer.Deserialize(reader);
            }

            logger.Info($"Принят файл {file}");

            return doc;
        }

        public static void SerializerXml(string path, Documents doc)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Documents));

            XmlWriterSettings settings = new XmlWriterSettings() { OmitXmlDeclaration = false, Indent = true, Encoding = new UTF8Encoding(false) };

            using (Stream writer = new FileStream(path, FileMode.OpenOrCreate))
            using (var wr = XmlWriter.Create(writer, settings))
            {
                var a = XmlWriter.Create(writer, settings);
                serializer.Serialize(a, doc);
            }
            logger.Info($"Создан файл {path}");
        }
    }
}
