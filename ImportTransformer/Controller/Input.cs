using ImportTransformer.Model;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ImportTransformer.Controller
{
    static class Input
    {
        /// <summary>
        /// Принимает файл трейслинк
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static IEnumerable<CryptoCode> InputCsv(this string path)
        {
            var codes = new List<CryptoCode>();
            var temp = new List<string[]>();

            using (var sr = new StreamReader(path))
            {
                while (!sr.EndOfStream)
                {
                    temp.Add(sr.ReadLine()?.Split(','));
                }
            }

            temp.RemoveAt(0);

            for (var i = 0; i < temp.Count(); i++)
            {
                codes.Add(new CryptoCode
                {
                    //(01)[14](21)[13](91)[4](92)[44]
                    Gtin = temp[i][0].Substring(2, 14),
                    Sn = temp[i][0].Substring(18, 13),
                    Prefix = temp[i][1].Substring(4, 4),
                    CryptoKeyCode = temp[i][1].Substring(12, 44)
                });
            }

            return codes;
        }

        /// <summary>
        /// Использовалось при тестах. Возможно понадобиться на следующих итерациях
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static IEnumerable<CryptoCode> InputCsvTestOrder(this string path)
        {
            var codes = new List<CryptoCode>();
            var temp = new List<string>();

            using (var sr = new StreamReader(path))
            {
                while (!sr.EndOfStream)
                {
                    temp.Add(sr.ReadLine());
                }
            }

            for (var i = 0; i < temp.Count(); i++)
            {
                codes.Add(new CryptoCode
                {
                    //(01)[14](21)[13](91)[4](92)[44]
                    Gtin = temp[i].Substring(2, 14),
                    Sn = temp[i].Substring(18, 13),
                    Prefix = temp[i].Substring(34, 4),
                    CryptoKeyCode = temp[i][41..]
                });
            }

            return codes;
        }

        /// <summary>
        /// Парсер отчета, который направляет сантенс
        /// </summary>
        /// <param name="path">путь к файлу отчета</param>
        /// <param name="page">0 - паллета/короб, 1 - короб/штука</param>
        /// <returns></returns>
        public static IEnumerable<SantensReport> InputReport(this string path, int page)
        {
            // If you use EPPlus in a noncommercial context
            // according to the Polyform Noncommercial license:
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            var data = new List<SantensReport>();

            using var xlPackage = new ExcelPackage(new FileInfo(path));
            var myWorksheet = xlPackage.Workbook.Worksheets[page];
            var totalRows = myWorksheet.Dimension.End.Row;

            var blocks = new List<int>();

            for (var rowNum = 1; rowNum <= totalRows; rowNum++)
            {
                if (myWorksheet.Cells[rowNum, 2].Value != null)
                {
                    rowNum++;
                    blocks.Add(rowNum);
                }
            }

            for (var i = blocks.Last() + 2; i < totalRows; i++)
            {
                if (myWorksheet.Cells[i, 4].Value == null
                    || string.IsNullOrEmpty(myWorksheet.Cells[i, 4].Value.ToString())
                    || string.IsNullOrWhiteSpace(myWorksheet.Cells[i, 4].Value.ToString()))
                    totalRows = i;
            }

            blocks.Add(totalRows+2);

            for (var i = 0; i < blocks.Count-1; i++)
            {
                var temp = new List<string>();

                var startRow = blocks[i] + 3;
                var lastRow = blocks[i + 1] >= totalRows ? totalRows : blocks[i + 1] - 2;

                for (var j = startRow; j <= lastRow; j++)
                {
                    if (myWorksheet.Cells[j, 4].Value != null)
                        temp.Add(myWorksheet.Cells[j, 4].Value.ToString());
                }
                data.Add(new SantensReport(myWorksheet.Cells[blocks[i], 4].Value.ToString(), temp));
            }

            return data;
        }

        public static SupportDate InputSupport (this string path)
        {
            // If you use EPPlus in a noncommercial context
            // according to the Polyform Noncommercial license:
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            var data = new SupportDate();

            using (var xlPackage = new ExcelPackage(new FileInfo(path)))
            {
                var myWorksheet = xlPackage.Workbook.Worksheets.First();

                data.TracelinkFilename = myWorksheet.Cells["B2"].Value.ToString();
                data.SantensFilename = myWorksheet.Cells["B3"].Value.ToString();
                data.Gtin = myWorksheet.Cells["B4"].Value.ToString();
                data.Batch = myWorksheet.Cells["B5"].Value.ToString();

                if (DateTime.TryParse(myWorksheet.Cells["B6"].Value.ToString(), out var tempDate))
                    data.ExpirationDate = tempDate;
                
                data.DocNum = myWorksheet.Cells["B7"].Value.ToString();

                if (DateTime.TryParse(myWorksheet.Cells["B8"].Value.ToString(), out tempDate))
                    data.DocDate = tempDate;
            }
            return data;
        }

        
    }
}
