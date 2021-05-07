using ImportTransformer.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace ImportTransformer.Controller
{
    class Core
    {
        static void AllerganProject ()
        {
            try
            {
                Stopwatch SW = new Stopwatch();
                SW.Start();

                //test
                var header = new MsgHeaderData("0f4df088-6983-4b90-99d6-ca464c85e666", "88f76a80-db84-4278-98b8-d7750ced71c5", "839d3743-5203-418a-91e6-2e91980ac441",
                                               "839d3743-5203-418a-91e6-2e91980ac441", "37881987-64cd-4e94-86b7-7d3c42c79237", "fb3e30fb-41c4-4f89-5ef3-031065036f5a",
                                               "3b6d8a66-9098-4de9-b7ba-d164b134f336");

                //prod
                //var header = new MsgHeaderData("804f4ee2-2fda-4c0e-81e7-a4fd5837606b", "fff58179-c35c-4405-a657-afb2275216f6", "fff58179-c35c-4405-a657-afb2275216f6",
                //                               "fff58179-c35c-4405-a657-afb2275216f6", "d6b8c442-7999-4f79-8ec3-152e2adddf2d", "180bc612-069f-4122-a5f9-ada634449fac");

                string csvPath = @"D:\YandexDisk\santensTransformer\testrun\Restasis 05016007202922-16Mar2021-111113.csv";
                string santensPath = @"D:\YandexDisk\santensTransformer\Отчет по агрегации.xlsx";
                string utilPath = @"D:\YandexDisk\santensTransformer\testrun";
                string utilPathTestOrder = @"D:\YandexDisk\santensTransformer\testorder";
                string orderPath = @"D:\YandexDisk\santensTransformer\testorder\Входные данные\order_c2f646bf-d7e7-44b7-9aeb-a89e0b31f30c_gtin_05016007202922_quantity_30_15634070993818687573.csv";


                DateTime timestamp = DateTime.Now;

                List<CryptoCode> codes = Input.InputCsv(csvPath);
                List<CryptoCode> codesFromOrder = Input.InputCsvTestOrder(orderPath);

                List<SantensReport> ssccReport = Input.InputReport(santensPath, 0);
                List<SantensReport> sgtinReport = Input.InputReport(santensPath, 1);

                Output.Create915Message(sgtinReport, codes[0].Gtin, header, utilPath, timestamp.AddSeconds(10));
                Output.Create915Message(ssccReport, codes[0].Gtin, header, utilPath, timestamp.AddSeconds(20));

                List<CryptoCode> filteredCodes = Transformator.Filter(codes, sgtinReport);

                Output.CreateUtilisationReport(codes, utilPath, timestamp);

                //тест по файлу ордера
                //Output.CreateUtilisationReport(codesFromOrder, utilPathTestOrder, timestamp);
                //Output.Create300Message(codesFromOrder, header, utilPathTestOrder, timestamp);
                //Output.Create321Message(codesFromOrder, header, utilPathTestOrder, timestamp);
                //Output.Create915Message(sgtinReport, codesFromOrder[0].Gtin, header, utilPathTestOrder, timestamp.AddMinutes(10));
                //Output.Create915Message(ssccReport, codesFromOrder[0].Gtin, header, utilPathTestOrder, timestamp.AddMinutes(20));

                //Output.Create331Message(ssccReport, header, utilPathTestOrder, timestamp.AddHours(1));
                //Output.Create336Message(ssccReport, header, utilPathTestOrder, timestamp.AddHours(2));

                List<SantensReport> testlist = new List<SantensReport>
                {
                    new SantensReport()
                };
                testlist[0].ParentContainer = "1234567899877654321";
                foreach (var e in codesFromOrder)
                {
                    testlist[0].Content.Add(e.Sn);
                }

                Output.Create915Message(testlist, codesFromOrder[0].Gtin, header, utilPathTestOrder, timestamp.AddMinutes(1));

                // 300/321 -(10мин) - утиль || 10319 - 915 - 915 - 331/336  

                SW.Stop();

                string msg = "Скрипт исполнен за " + Convert.ToString(SW.Elapsed.TotalMilliseconds) + " миллисекунд(ы)";

                Logging.LogTimer(msg, utilPathTestOrder);

                Console.WriteLine(msg);
                Console.WriteLine("--------------------------------------------");
                Console.ReadKey();
            }
            catch (Exception e)
            {
                Console.WriteLine("\n\n" + e.ToString() + "\n\n");
                Console.ReadLine();
            }
        }
    }
}
