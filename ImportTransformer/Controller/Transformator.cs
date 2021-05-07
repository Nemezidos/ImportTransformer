using ImportTransformer.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ImportTransformer.Controller
{
    class Transformator
    {
        public static List<CryptoCode> Filter(List<CryptoCode> codes, List<SantensReport> santensReports)
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

        public static void Sorter(List<CryptoCode> codes, List<SantensReport> santensReports)
        {

        }

        public static void Linker(List<CryptoCode> codes, List<SantensReport> santensReports)
        {

        }

        public static void Split(string dir)
        {
            int maxSize = 800_000;

            var files = Directory.GetFiles(dir + @"\forUpload", "*915*.xml")
                .Where(s => new FileInfo(s).Length >= maxSize)
                .Select(s => new { filepath = s, fileSize = new FileInfo(s).Length });

            foreach (var file in files)
            {
                var parts = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(file.fileSize / maxSize)));

                var doc = Serializer.DeSerializer(file.filepath);

                FileInfo fi = new FileInfo(file.filepath);
                string name = fi.Name;
                name = name.Remove(name.Length - 4);

                string operDate = doc.Multi_pack.Operation_date;
                string subjectId = doc.Multi_pack.Subject_id;

                List<Detail> elements = doc.Multi_pack.By_sgtin.Detail.OrderByDescending(s => s.Content.Sgtin.Count()).ToList();

                var arr = new List<Detail>[parts];

                for (int i = 0; i < parts; i++)
                {
                    arr[i] = new List<Detail>();
                }

                int prefix;
                int suffix;
                for (int i = 0; i < (elements.Count() / 2) + 1; i++)
                {
                    prefix = i;
                    suffix = elements.Count() - i - 1;
                    if (prefix < suffix)
                    {
                        arr[i % parts].Add(elements[prefix]);
                        arr[i % parts].Add(elements[suffix]);
                    }
                    else
                    {
                        if (prefix == suffix)
                            arr[i % parts].Add(elements[prefix]);
                    }
                }

                for (int i = 0; i < parts; i++)
                {
                    string tempName = $"{name}_{i + 1}.xml";

                    Documents splittedDoc = new Documents
                    {
                        Version = "1.35",
                        Multi_pack = new Multi_pack()
                    };
                    splittedDoc.Multi_pack.Action_id = "915";
                    splittedDoc.Multi_pack.Operation_date = operDate;
                    splittedDoc.Multi_pack.Subject_id = subjectId;
                    splittedDoc.Multi_pack.By_sgtin = new By_sgtin
                    {
                        Detail = new List<Detail>()
                    };
                    splittedDoc.Multi_pack.By_sgtin.Detail.AddRange(arr[i]);

                    Logging.LogFileName(tempName, dir);

                    Serializer.SerializerXml(dir + @"\forUpload\" + tempName, splittedDoc);
                }
            }
        }
    }
}
