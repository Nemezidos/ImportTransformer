using ImportTransformer.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ImportTransformer.Controller
{
    class Logging
    {
        public static void LogFileName(string path, string dir)
        {
            using (StreamWriter sw = new StreamWriter(dir + @"\Check.txt", true, Encoding.UTF8))
            {
                sw.WriteLine(path);
            }
        }

        public static void LogCountFB(int countOutside, int countInside, bool pallet, string actionId, string dir)
        {
            using (StreamWriter sw = new StreamWriter(dir + @"\Check.txt", true, Encoding.UTF8))
            {
                if (pallet)
                    sw.WriteLine($" {actionId}: SSCC in file = {countInside} in {countOutside}");
                else
                    sw.WriteLine($" {actionId}: SGTIN in file = {countInside} in {countOutside}");
            }
        }

        public static void LogTimer(string text, string dir)
        {
            using (StreamWriter sw = new StreamWriter(dir + @"\Check.txt", true, Encoding.UTF8))
            {
                sw.WriteLine(text);
            }
        }

        public static void CounterFB(Documents doc, string dir)
        {
            int count = 0;
            // Проверка 915, паллеты.
            if (doc.Multi_pack.By_sscc != null)
            {
                foreach (var d in doc.Multi_pack.By_sscc.Detail)
                {
                    count += d.Content.Sscc.Count();
                }
                LogCountFB(doc.Multi_pack.By_sscc.Detail.Count(), count, true, doc.Multi_pack.Action_id, dir);
            }
            // Проверка 915, короба.
            else if (doc.Multi_pack.By_sgtin != null)
            {
                foreach (var d in doc.Multi_pack.By_sgtin.Detail)
                {
                    count += d.Content.Sgtin.Count();
                }
                LogCountFB(doc.Multi_pack.By_sgtin.Detail.Count(), count, false, doc.Multi_pack.Action_id, dir);
            }
            // Исключение на случай пустого файла.
            else
            {
                throw new Exception("Обнаружен битый файл. Требуется проверка вручную!");
            }
        }

        public static void ErrorLog(Exception st)
        {
            DirectoryInfo dir = new DirectoryInfo(Environment.CurrentDirectory);
            using (StreamWriter sw = new StreamWriter(dir + @"\ErrorMessage.txt", true, Encoding.UTF8))
            {
                sw.WriteLine();
                sw.WriteLine(st.ToString());
            }

        }
    }
}
