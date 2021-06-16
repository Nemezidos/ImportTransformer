using ImportTransformer.Model;
using NLog;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ImportTransformer.Controller
{
    class Transformator
    {
        private const int MaxSize = 900_000;
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public static IEnumerable<CryptoCode> Filter(IEnumerable<CryptoCode> codes, IEnumerable<SantensReport> santensReports)
        {
            List<string> cont = new List<string>();
            List<CryptoCode> result = new List<CryptoCode>();

            foreach (var e in santensReports)
            {
                cont.AddRange(e.Content);
            }

            foreach (var e in codes)
            {
                if (cont.Contains(e.Sn))
                {
                    result.Add(e);
                }
            }

            return result;
        }

        public static void SplitAllMultiPackInDir(string dir)
        {
            

            var files = Directory.GetFiles(dir + @"\forUpload", "*915*.xml")
                .Where(s => new FileInfo(s).Length >= MaxSize)
                .Select(s => new { filepath = s, fileSize = new FileInfo(s).Length });

            var tasks = new List<Task>();

            foreach (var file in files)
            {
                SplitSingleMultiPack(file.filepath, file.fileSize);
            }
        }

        private static void SplitSingleMultiPack(string filePath, long fileSize)
        {
            var parts = (int)(fileSize / MaxSize);
            if (fileSize % MaxSize > 0)
                parts++;

            var doc = Serializer.DeSerializer(filePath);

            FileInfo fi = new FileInfo(filePath);
            string name = fi.Name;
            name = name.Remove(name.Length - 4);
            string dir = fi.DirectoryName;

            var arr = SplitContentOfMultiPack(doc.Multi_pack.By_sgtin.Detail.OrderByDescending(s => s.Content.Sgtin.Count()).ToList(), parts);

            var tasks = new List<Task>();

            for (int i = 0; i < parts; i++)
            {
                string tempName = $"{name}_{i + 1}.xml";

                Documents splittedDoc = new Documents
                {
                    Version = "1.36",
                    Multi_pack = new Multi_pack
                    {
                        Action_id = "915",
                        Operation_date = doc.Multi_pack.Operation_date,
                        Subject_id = doc.Multi_pack.Subject_id,
                        By_sgtin = new By_sgtin 
                        { 
                            Detail = arr[i]
                        }
                    }
                };
                tasks.Add(Task.Run(() => Serializer.SerializerXml(dir + @"\forUpload\" + tempName, splittedDoc)));
            }

            Task.WaitAll(tasks.ToArray());

            logger.Info($"Создано {tasks.Select(s => s.IsCompleted).Count()} документов. Должно быть: {parts}");
        }

        private static List<Detail>[] SplitContentOfMultiPack(List<Detail> elements, int parts)
        {
            var arr = new List<Detail>[parts];

            for (int i = 0; i < parts; i++)
            {
                arr[i] = new List<Detail>();
            }

            int prefix = 0;
            int suffix = elements.Count() - 1;
            while (prefix < suffix)
            {
                arr[prefix % parts].Add(elements[prefix]);
                arr[prefix % parts].Add(elements[suffix]);

                prefix++;
                suffix--;
            }
            if (prefix == suffix)
                arr[prefix % parts].Add(elements[prefix]);

            return arr;
        }
    }
}
