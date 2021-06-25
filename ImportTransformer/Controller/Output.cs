using ImportTransformer.Model;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImportTransformer.Controller
{
    static class Output
    {
        private const int SsccLenght = 13;
        
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Создаёт утилизейшон репорты
        /// </summary>
        /// <param name="codes"></param>
        /// <param name="path"></param>
        /// <param name="timestamp"></param>
        public static void CreateUtilisationReport(this IEnumerable<CryptoCode> codes, string path, DateTime timestamp)
        {
            var tasks = new List<Task>();

            var counter = (codes.Count() / 30000) + 1;
            //до 30_000 в одном файле

            for (var i = 0; i < counter; i++)
            {
                var temp = codes.Skip(i * 30000).Take(30000);

                var newPath = @$"{path}\forUpload\UtilReport_{timestamp:yyyyMMddHHmmss}_{i + 1}.csv";

                tasks.Add(Task.Run(() => CreateSingleUtil(temp, newPath)));
            }

            Task.WaitAll(tasks.ToArray());

           logger.Info($"Создано {tasks.Select(s => s.IsCompleted).Count()} документов. Должно быть: {counter}");
        }

        private static void CreateSingleUtil(IEnumerable<CryptoCode> cryptoCodes, string path)
        {
            const char gs = (char) 29; //Encoding.ASCII.GetString(new byte[] { 29 });

            using (var sw = new StreamWriter(path, false, new UTF8Encoding(false)))
            {
                foreach (var e in cryptoCodes)
                {
                    sw.WriteLine($"01{e.Gtin}21{e.Sn}{gs}91{e.Prefix}{gs}92{e.CryptoKeyCode}");
                }
            }

            logger.Info($"Создан отчёт о нанесении: {path}");
        }

        public static void CreateMultiPackMessages(this List<SantensReport> data, string gtin, MsgHeaderData headerData, string path, DateTime timestamp)
        {
            var doc = new Documents
            {
                Version = "1.36",
                Multi_pack = new Multi_pack
                {
                    Action_id = "915",
                    Subject_id = headerData.HubSubjectId,
                    Operation_date = $"{timestamp:yyyy-MM-ddTHH:mm:ss}+03:00"
                }
            };
            string marker;

            if (data.First().Content.First().Length > SsccLenght)
            {
                doc.Multi_pack.By_sscc = new By_sscc
                {
                    Detail = data.Select(fragment => new Detail
                    {
                        Sscc = fragment.ParentContainer,
                        Content = new Content
                        {
                            Sscc = fragment.Content.ToList()
                        }
                    }).ToList()
                };

                marker = "pallet";
            }
            else
            {
                doc.Multi_pack.By_sgtin = new By_sgtin
                {
                    Detail = data.Select(fragment => new Detail
                        {
                            Sscc = fragment.ParentContainer,
                            Content = new Content
                            {
                                Sgtin = fragment.Content.Select(s => gtin + s).ToList()
                            }
                        }).ToList()
                };

                marker = "case";
            }

            var newPath = path + $@"\forUpload\915-st_format_{timestamp:yyyyMMddHHmmss}_{marker}.xml";

            doc.SerializerXml(newPath);
        }

        public static void CreateTransferCodeToCustomMessages(this IEnumerable<CryptoCode> data, MsgHeaderData headerData, string path, DateTime timestamp)
        {
            var doc = new Documents
            {
                Version = "1.36",
                Session_ui = "4Aa246a6-D7e2-2465-a056-0234554369a3",
                Transfer_code_to_custom = new Transfer_code_to_custom 
                {
                    Action_id = "300",
                    Operation_date = $"{timestamp:yyyy-MM-ddTHH:mm:ss}+03:00",
                    Subject_id = headerData.SubjectId,
                    Custom_receiver_id = headerData.CustomReceiverId,
                    Gtin = data.FirstOrDefault().Gtin,
                    Signs = new Signs
                    {
                        Sgtin = new List<string>()
                    }
                }
            };

            var d = new DirectoryInfo(path + @"\forUpload\");
            if (!d.Exists)
                d.Create();

            //до 10_000 в одном файле
            var counter = (data.Count() / 10000) + 1;

            var tasks = new List<Task>();

            for (var i = 0; i < counter; i++)
            {
                var temp = data.Skip(i * 10000).Take(10000);
                doc.Transfer_code_to_custom.Signs.Sgtin.Clear();
                foreach (var e in temp)
                {
                    doc.Transfer_code_to_custom.Signs.Sgtin.Add($"{e.Gtin}{e.Sn}");
                }
                var newPath = @$"{path}\forUpload\300-TransferCodeToCustom_{timestamp:yyyyMMddHHmmss}_{i + 1}.xml";

                tasks.Add(Task.Run(() => doc.SerializerXml(newPath)));
            }

            Task.WaitAll(tasks.ToArray());

            logger.Info($"Создано {tasks.Select(s => s.IsCompleted).Count()} документов. Должно быть: {counter}");
        }

        public static void CreateForeignEmissionMessages(this IEnumerable<CryptoCode> data, MsgHeaderData headerData, SupportDate support, string path, DateTime timestamp)
        {
            var doc = new Documents
            {
                Version = "1.35",
                Foreign_emission = new Foreign_emission 
                {
                    Action_id = "321",
                    Subject_id = headerData.HubSubjectId,
                    Packing_id = headerData.HubSubjectId,
                    Control_id = headerData.HubSubjectId,
                    Operation_date = timestamp.ToString("yyyy-MM-ddTHH:mm:ss") + "+03:00",
                    Series_number = support.Batch,
                    Expiration_date = support.ExpirationDate.ToString("dd.MM.yyyy"),
                    Gtin = support.Gtin,
                    Signs = new Signs 
                    { 
                        Sgtin = new List<string>() 
                    }
                }
            };

            foreach (var e in data)
            {
                doc.Foreign_emission.Signs.Sgtin.Add(support.Gtin + e.Sn);
            }

            var newPath = path + @$"\forUpload\321-st_format_{timestamp:yyyyMMddHHmmss}_{support.Batch}.xml";

            doc.SerializerXml(newPath);
        }

        public static void CreateForeignShipmentMessages(this IEnumerable<SantensReport> data, MsgHeaderData headerData, SupportDate support, string path, DateTime timestamp)
        {
            //до 25_000 в одном файле

            var counter = (data.Count() / 25000) + 1;

            var doc = new Documents
            {
                Version = "1.36",
                Foreign_shipment = new Foreign_shipment 
                {
                    Action_id = "331",
                    Subject_id = headerData.HubSubjectId,
                    Seller_id = headerData.HubSellerId,
                    Receiver_id = headerData.HubReceiverId,
                    Custom_receiver_id = headerData.HubCustomReceiverId,
                    Operation_date = timestamp.ToString("yyyy-MM-ddTHH:mm:ss") + "+03:00",
                    Contract_type = "1",
                    Doc_num = support.DocNum,
                    Doc_date = support.DocDate.ToString("dd.MM.yyyy"),
                    Order_details = new Order_details 
                    { 
                        Sscc = new List<string>() 
                    }
                }
            };

            var tasks = new List<Task>();

            for (var i = 0; i < counter; i++)
            {
                var temp = data.Skip(i * 25000).Take(25000);
                doc.Foreign_shipment.Order_details.Sscc.Clear();
                foreach (var e in temp)
                {
                    doc.Foreign_shipment.Order_details.Sscc.Add(e.ParentContainer);
                }

                var newPath = $@"{path}\forUpload\331-st_format_{timestamp:yyyyMMddHHmmss}_{support.DocNum}_{i + 1}.xml";

                tasks.Add(Task.Run(() => doc.SerializerXml(newPath)));
            }

            Task.WaitAll(tasks.ToArray());

            logger.Info($"Создано {tasks.Select(s => s.IsCompleted).Count()} документов. Должно быть: {counter}");
        }

        public static void CreateImportInfoMessages(this IEnumerable<SantensReport> data, MsgHeaderData headerData, SupportDate support, string path, DateTime timestamp)
        {
            //до 25_000 в одном файле

            var counter = (data.Count() / 25000) + 1;

            var doc = new Documents
            {
                Version = "1.36",
                Session_ui = "4Aa246a6-D7e2-2465-a056-0234554369a3",
                Import_info = new Import_info 
                {
                    Action_id = "336",
                    Subject_id = headerData.SubjectId,
                    Seller_id = headerData.SellerId,
                    Receiver_id = headerData.ReceiverId,
                    Operation_date = timestamp.ToString("yyyy-MM-ddTHH:mm:ss") + "+03:00",
                    Contract_type = "1",
                    Doc_num = support.DocNum,
                    Doc_date = support.DocDate.ToString("dd.MM.yyyy"),
                    Order_details = new Order_details
                    {
                        Sscc = new List<string>()
                    }
                }
            };

            var tasks = new List<Task>();

            for (var i = 0; i < counter; i++)
            {
                var temp = data.Skip(i * 25000).Take(25000);
                doc.Import_info.Order_details.Sscc.Clear();
                foreach (var e in temp)
                {
                    doc.Import_info.Order_details.Sscc.Add(e.ParentContainer);
                }

                var newPath = $@"{path}\forUpload\336-st_format_{timestamp:yyyyMMddHHmmss}_{support.DocNum}_{i + 1}.xml";

                tasks.Add(Task.Run(() => doc.SerializerXml(newPath)));
            }

            Task.WaitAll(tasks.ToArray());

            logger.Info($"Создано {tasks.Select(s => s.IsCompleted).Count()} документов. Должно быть: {counter}");
        }

        public static void LogTimer(string text)
        {
            logger.Info(text);
        }

        public static void ErrorLog(Exception st)
        {
            logger.Error(st, $"Возникла ошибка: \n--{st.Message} \n\n{st.StackTrace}");

        }
    }
}
