using ImportTransformer.Model;
using NLog;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace ImportTransformer.Controller
{
    public static class Serializer
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static Documents DeSerializer(this string file)
        {
            var serializer = new XmlSerializer(typeof(Documents));

            using Stream reader = new FileStream(file, FileMode.Open);
            var doc = (Documents)serializer.Deserialize(reader);

            Logger.Info($"Принят файл {file}");

            return doc;
        }

        public static void SerializerXml(this Documents doc, string path)
        {
            var serializer = new XmlSerializer(typeof(Documents));

            var settings = new XmlWriterSettings() { OmitXmlDeclaration = false, Indent = true, Encoding = new UTF8Encoding(false) };

            using Stream writer = new FileStream(path, FileMode.OpenOrCreate);
            using var wr = XmlWriter.Create(writer, settings);
            serializer.Serialize(wr, doc);

            Logger.Info($"Создан файл {path}");
        }
    }
}
