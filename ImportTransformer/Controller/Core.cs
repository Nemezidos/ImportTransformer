using ImportTransformer.Model;
using NLog;
using System;
using System.Diagnostics;
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
            Stopwatch SW = new Stopwatch();
            SW.Start();

            pathes.Results = Directory.GetCurrentDirectory();

            DateTime timestamp = DateTime.Now;

            var ssccReport = Input.InputReport(pathes.Santens, 0);
            var sgtinReport = Input.InputReport(pathes.Santens, 1);

            var codes = Input.InputCsv(pathes.Tracelink);
            var filteredCodes = Transformator.Filter(codes, sgtinReport);

            XmlSerializer serializer = new XmlSerializer(typeof(MsgHeaderData));

            using Stream reader = new FileStream(pathes.Headers, FileMode.Open);
            var header = (MsgHeaderData)serializer.Deserialize(reader);

            var supportData = Input.InputSupport(pathes.Support);

            if (needs.FirstPart)
            {
                Output.CreateUtilisationReport(filteredCodes, pathes.Results, timestamp);
                Output.CreateTransferCodeToCustomMessages(filteredCodes, header, pathes.Results, timestamp);
                Output.CreateForeignEmissionMessages(filteredCodes, header, supportData, pathes.Results, timestamp);
            }

            if (needs.SecondPart)
            {
                Output.CreateMultiPackMessages(sgtinReport.ToList(), supportData.Gtin, header, pathes.Results, timestamp.AddMinutes(10));
                Output.CreateMultiPackMessages(ssccReport.ToList(), supportData.Gtin, header, pathes.Results, timestamp.AddMinutes(20));

                Output.CreateForeignShipmentMessages(ssccReport, header, supportData, pathes.Results, timestamp.AddHours(1));
                Output.CreateImportInfoMessages(ssccReport, header, supportData, pathes.Results, timestamp.AddHours(2));

                if (new DirectoryInfo(@$"{pathes.Results}\forUpload").Exists)
                    Transformator.SplitAllMultiPackInDir(@$"{pathes.Results}\forUpload");
            }

            SW.Stop();

            Output.LogTimer($"Скрипт исполнен за {Convert.ToString(SW.Elapsed.TotalMilliseconds)} миллисекунд(ы)");
        }

        public static string ExistHeaders()
        {
            CreateHeader(false, out string path);

            if (File.Exists(path))
                return path;
            else
                return string.Empty;
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

                XmlSerializer serializer = new XmlSerializer(typeof(MsgHeaderData));

                XmlWriterSettings settings = new XmlWriterSettings() { 
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
