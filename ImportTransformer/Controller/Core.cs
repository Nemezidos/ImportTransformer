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
    public static class Core
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        public static void DoMessages(FilePaths paths, PartsNeeded needs)
        {
            var sw = new Stopwatch();
            sw.Start();

            paths.Results = Directory.GetCurrentDirectory();

            var pathForUpload = Path.Combine(paths.Results, "forUpload");
            if (!Directory.Exists(pathForUpload))
            {
                Directory.CreateDirectory(pathForUpload);
            }

            var timestamp = DateTime.Now;

            var ssccReport = paths.Santens.InputReport(0);
            var sgtinReport = paths.Santens.InputReport(1);

            var codes = paths.Tracelink.InputCsv();
            var filteredCodes = codes.Filter(sgtinReport);

            var serializer = new XmlSerializer(typeof(MsgHeaderData));

            using Stream reader = new FileStream(paths.Headers, FileMode.Open);
            var header = (MsgHeaderData)serializer.Deserialize(reader);

            var supportData = paths.Support.InputSupport();

            if (needs.FirstPart)
            {
                filteredCodes.CreateUtilisationReport(paths.Results, timestamp);
                filteredCodes.CreateTransferCodeToCustomMessages(header, paths.Results, timestamp);
                filteredCodes.CreateForeignEmissionMessages(header, supportData, paths.Results, timestamp);
            }

            if (needs.SecondPart)
            {
                sgtinReport.ToList().CreateMultiPackMessages(supportData.Gtin, header, paths.Results, timestamp.AddMinutes(10));
                ssccReport.ToList().CreateMultiPackMessages(supportData.Gtin, header, paths.Results, timestamp.AddMinutes(20));

                ssccReport.CreateForeignShipmentMessages(header, supportData, paths.Results, timestamp.AddHours(1));
                ssccReport.CreateImportInfoMessages(header, supportData, paths.Results, timestamp.AddHours(2));

                if (new DirectoryInfo(@$"{paths.Results}\forUpload").Exists)
                    Transformator.SplitAllMultiPackInDir(@$"{paths.Results}\forUpload");
            }

            sw.Stop();

            Logger.Info(
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

                Logger.Info("Создан новый файл заголовков");
            }
        }
    }
}
