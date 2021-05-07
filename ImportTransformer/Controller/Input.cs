using ImportTransformer.Model;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ImportTransformer.Controller
{
    class Input
    {
        /// <summary>
        /// Принимает файл трейслинк
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static List<CryptoCode> InputCsv(string path)
        {
            List<CryptoCode> codes = new List<CryptoCode>();
            List<string[]> temp = new List<string[]>();

            using (StreamReader sr = new StreamReader(path))
            {
                while (!sr.EndOfStream)
                {
                    string[] args = sr.ReadLine().Split(',');

                    temp.Add(args);
                }
            }

            temp.RemoveAt(0);

            for (int i = 0; i < temp.Count(); i++)
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

        public static List<CryptoCode> InputCsvTestOrder(string path)
        {
            List<CryptoCode> codes = new List<CryptoCode>();
            List<string> temp = new List<string>();

            using (StreamReader sr = new StreamReader(path))
            {
                while (!sr.EndOfStream)
                {
                    string args = sr.ReadLine();

                    temp.Add(args);
                }
            }

            for (int i = 0; i < temp.Count(); i++)
            {
                codes.Add(new CryptoCode
                {
                    //(01)[14](21)[13](91)[4](92)[44]
                    Gtin = temp[i].Substring(2, 14),
                    Sn = temp[i].Substring(18, 13),
                    Prefix = temp[i].Substring(34, 4),
                    CryptoKeyCode = temp[i].Substring(41)
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
        public static List<SantensReport> InputReport(string path, int page)
        {
            // If you use EPPlus in a noncommercial context
            // according to the Polyform Noncommercial license:
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            List<SantensReport> data = new List<SantensReport>();

            //using OfficeOpenXml;
            using (ExcelPackage xlPackage = new ExcelPackage(new FileInfo(path)))
            {
                var myWorksheet = xlPackage.Workbook.Worksheets[page]; //xlPackage.Workbook.Worksheets.First(); //select sheet here
                var totalRows = myWorksheet.Dimension.End.Row;
                var totalColumns = myWorksheet.Dimension.End.Column;

                List<int> blocks = new List<int>();

                for (int rowNum = 4; rowNum <= totalRows; rowNum++)
                {
                    if (myWorksheet.Cells[rowNum, 2].Value != null)
                    {
                        blocks.Add(rowNum);
                    }
                }
                blocks.Add(totalRows);

                for (int i = 0; i < blocks.Count(); i += 2)
                {
                    List<string> temp = new List<string>();

                    int startRow = blocks[i] + 3;
                    int lastRow = blocks[i + 1] - 2;

                    for (int j = startRow; j <= lastRow; j++)
                    {
                        if (myWorksheet.Cells[j, 4].Value != null)
                        {
                            temp.Add(myWorksheet.Cells[j, 4].Value.ToString());
                        }
                    }
                    data.Add(new SantensReport(myWorksheet.Cells[blocks[i], 4].Value.ToString(), temp));
                }
            }
            return data;
        }

        public static SupportDate InputSupport (string path)
        {
            // If you use EPPlus in a noncommercial context
            // according to the Polyform Noncommercial license:
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            SupportDate data = new SupportDate();

            //using OfficeOpenXml;
            using (ExcelPackage xlPackage = new ExcelPackage(new FileInfo(path)))
            {
                var myWorksheet = xlPackage.Workbook.Worksheets.First(); //xlPackage.Workbook.Worksheets.First(); //select sheet here

                data.TracelinkFilename = myWorksheet.Cells["B2"].Value.ToString();
                data.SantensFilename = myWorksheet.Cells["B3"].Value.ToString();
                data.Gtin = myWorksheet.Cells["B4"].Value.ToString();
                data.Batch = myWorksheet.Cells["B5"].Value.ToString();

                DateTime tempDate = new DateTime();
                if (DateTime.TryParse(myWorksheet.Cells["B6"].Value.ToString(), out tempDate))
                {
                    data.ExpirationDate = tempDate;
                }
                
                data.DocNum = myWorksheet.Cells["B7"].Value.ToString();

                tempDate = new DateTime();
                if (DateTime.TryParse(myWorksheet.Cells["B8"].Value.ToString(), out tempDate))
                {
                    data.DocDate = tempDate;
                }
                

            }
            return data;
        }
    }
}
