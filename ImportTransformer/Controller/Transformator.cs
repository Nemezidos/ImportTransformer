using ImportTransformer.Model;
using NLog;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ImportTransformer.Controller
{
    static class Transformator
    {
        private const int MaxSize = 900_000;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static IEnumerable<CryptoCode> Filter(this IEnumerable<CryptoCode> codes, IEnumerable<SantensReport> santensReports)
        {
            var cont = new List<string>();

            foreach (var e in santensReports)
            {
                cont.AddRange(e.Content);
            }

            return codes.Where(e => cont.Contains(e.Sn)).ToList();
        }

        public static void SplitAllMultiPackInDir(string dir)
        {
            var files = Directory.GetFiles(dir + @"\forUpload", "*915*.xml")
                .Where(s => new FileInfo(s).Length >= MaxSize)
                .Select(s => new { filepath = s, fileSize = new FileInfo(s).Length });

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

            var doc = filePath.DeSerializer();

            var fi = new FileInfo(filePath);
            var name = fi.Name;
            name = name.Remove(name.Length - 4);
            var dir = fi.DirectoryName;

            var arr = doc.Multi_pack.By_sgtin.Detail
                .OrderByDescending(s => s.Content.Sgtin.Count())
                .ToList()
                .SplitContentOfMultiPack(parts);

            var tasks = new List<Task>();

            for (var i = 0; i < parts; i++)
            {
                var tempName = $"{name}_{i + 1}.xml";

                var splittedDoc = new Documents
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
                tasks.Add(Task.Run(() => splittedDoc.SerializerXml(dir + @"\forUpload\" + tempName)));
            }

            Task.WaitAll(tasks.ToArray());

            Logger.Info($"Создано {tasks.Select(s => s.IsCompleted).Count()} документов. Должно быть: {parts}");
        }

        private static List<Detail>[] SplitContentOfMultiPack(this List<Detail> elements, int parts)
        {
            var arr = new List<Detail>[parts];

            for (var i = 0; i < parts; i++)
            {
                arr[i] = new List<Detail>();
            }

            var prefix = 0;
            var suffix = elements.Count() - 1;
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
