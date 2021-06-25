using ImportTransformer.Model;
using NLog;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace ImportTransformer.Controller
{
    public class Core
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        public static void DoMessages(FilePaths pathes, PartsNeeded needs)
        {
            var sw = new Stopwatch();
            sw.Start();

            pathes.Results = Directory.GetCurrentDirectory();

            var timestamp = DateTime.Now;

            var ssccReport = pathes.Santens.InputReport(0);
            var sgtinReport = pathes.Santens.InputReport(1);

            var codes = Input.InputCsv(pathes.Tracelink);
            var filteredCodes = codes.Filter(sgtinReport);

            var serializer = new XmlSerializer(typeof(MsgHeaderData));

            using Stream reader = new FileStream(pathes.Headers, FileMode.Open);
            var header = (MsgHeaderData)serializer.Deserialize(reader);

            var supportData = Input.InputSupport(pathes.Support);

            if (needs.FirstPart)
            {
                filteredCodes.CreateUtilisationReport(pathes.Results, timestamp);
                filteredCodes.CreateTransferCodeToCustomMessages(header, pathes.Results, timestamp);
                filteredCodes.CreateForeignEmissionMessages(header, supportData, pathes.Results, timestamp);
            }

            if (needs.SecondPart)
            {
                sgtinReport.ToList().CreateMultiPackMessages(supportData.Gtin, header, pathes.Results, timestamp.AddMinutes(10));
                ssccReport.ToList().CreateMultiPackMessages(supportData.Gtin, header, pathes.Results, timestamp.AddMinutes(20));

                ssccReport.CreateForeignShipmentMessages(header, supportData, pathes.Results, timestamp.AddHours(1));
                ssccReport.CreateImportInfoMessages(header, supportData, pathes.Results, timestamp.AddHours(2));

                if (new DirectoryInfo(@$"{pathes.Results}\forUpload").Exists)
                    Transformator.SplitAllMultiPackInDir(@$"{pathes.Results}\forUpload");
            }

            sw.Stop();

            logger.Info(
                $"Скрипт исполнен за {Convert.ToString(sw.Elapsed.TotalMilliseconds, CultureInfo.InvariantCulture)} миллисекунд(ы)");
        }

        public static string ExistHeaders()
        {
            CreateHeader(false, out var path);

            return File.Exists(path) ? path : string.Empty;
        }

        public static void CreateHeader(bool isNew, out string path)
        {
            var fileName = "Headers.xml";
            if (isNew)
                 fileName = "NewHeaders.xml";

            path = Path.Combine(Directory.GetCurrentDirectory(), fileName);

            if (!File.Exists(path))
            {
                var tempHeaders = new MsgHeaderData("seller", "subject", "packing", "control", "receiver", "customreceiver", "hubsubject", "hubseller", "hubreceiver", "hubcustomreceiver");

                var serializer = new XmlSerializer(typeof(MsgHeaderData));

                var settings = new XmlWriterSettings() { 
                    OmitXmlDeclaration = false, 
                    Indent = true, 
                    Encoding = new UTF8Encoding(false) 
                };

                using Stream writer = new FileStream(path, FileMode.OpenOrCreate);
                using var wr = XmlWriter.Create(writer, settings);
                var a = XmlWriter.Create(writer, settings);
                serializer.Serialize(a, tempHeaders);

                logger.Info("Создан новый файл заголовков");
            }
        }
    }
}
