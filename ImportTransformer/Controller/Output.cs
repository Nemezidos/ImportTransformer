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
        private const int MaxCapacityOfOrderReport = 150_000;
        private const int MaxCapacityOfUtilisationReport = 30_000;
        private const int MaxCapacityOfTransferCodeToCustomMessages = 10_000;
        private const int MaxCapacityOfCreateForeignShipmentMessages = 25_000;
        private const int MaxCapacityOfImportInfoMessages = 25_000;
        
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        public static void CreateOrderReport(this IEnumerable<CryptoCode> codes, string path, DateTime timestamp)
        {
            var tasks = new List<Task>();

            var counter = codes.Count() / MaxCapacityOfOrderReport;
            if (codes.Count() % MaxCapacityOfOrderReport != 0)
                counter++;

            for (var i = 0; i < counter; i++)
            {
                var temp = codes.Skip(i * MaxCapacityOfOrderReport).Take(MaxCapacityOfOrderReport).Select(c => c.Sn);

                var newPath = @$"{path}\forUpload\OrderReport_{timestamp:yyyyMMddHHmmss}_{i + 1}.csv";

                tasks.Add(Task.Run(() => CreateSingleOrder(temp, newPath)));
            }

            Task.WaitAll(tasks.ToArray());

            _logger.Info($"Создано {tasks.Select(s => s.IsCompleted).Count()} документов. Должно быть: {counter}");
        }
        
        private static void CreateSingleOrder(IEnumerable<string> serialNumbers, string path)
        {
            using (var sw = new StreamWriter(path, false, new UTF8Encoding(false)))
            {
                foreach (var e in serialNumbers)
                {
                    sw.WriteLine(e);
                }
            }

            _logger.Info($"Создан файл заказа: {path}");
        }
        
        public static void CreateUtilisationReport(this IEnumerable<CryptoCode> codes, string path, DateTime timestamp)
        {
            var tasks = new List<Task>();

            var counter = codes.Count() / MaxCapacityOfUtilisationReport;
            if (codes.Count() % MaxCapacityOfUtilisationReport != 0)
                counter++;

            for (var i = 0; i < counter; i++)
            {
                var temp = codes.Skip(i * MaxCapacityOfUtilisationReport).Take(MaxCapacityOfUtilisationReport);

                var newPath = @$"{path}\forUpload\UtilReport_{timestamp:yyyyMMddHHmmss}_{i + 1}.csv";

                tasks.Add(Task.Run(() => CreateSingleUtil(temp, newPath)));
            }

            Task.WaitAll(tasks.ToArray());

           _logger.Info($"Создано {tasks.Select(s => s.IsCompleted).Count()} документов. Должно быть: {counter}");
        }

        private static void CreateSingleUtil(IEnumerable<CryptoCode> cryptoCodes, string path)
        {
            const char gs = (char) 29;

            using (var sw = new StreamWriter(path, false, new UTF8Encoding(false)))
            {
                foreach (var e in cryptoCodes)
                {
                    sw.WriteLine($"01{e.Gtin}21{e.Sn}{gs}91{e.Prefix}{gs}92{e.CryptoKeyCode}");
                }
            }

            _logger.Info($"Создан отчёт о нанесении: {path}");
        }

        public static void CreateMultiPackMessages(this List<SantensReport> data, string gtin, MsgHeaderData headerData, string path, DateTime timestamp)
        {
            var doc = new Documents
            {
                Version = "1.36",
                MultiPack = new MultiPack
                {
                    ActionId = "915",
                    SubjectId = headerData.HubSubjectId,
                    OperationDate = $"{timestamp:yyyy-MM-ddTHH:mm:ss}+03:00"
                }
            };
            string marker;

            if (data.First().Content.First().Length > SsccLenght)
            {
                doc.MultiPack.BySscc = new BySscc
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
                doc.MultiPack.BySgtin = new BySgtin
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
                SessionUi = "4Aa246a6-D7e2-2465-a056-0234554369a3",
                TransferCodeToCustom = new TransferCodeToCustom 
                {
                    ActionId = "300",
                    OperationDate = $"{timestamp:yyyy-MM-ddTHH:mm:ss}+03:00",
                    SubjectId = headerData.SubjectId,
                    CustomReceiverId = headerData.CustomReceiverId,
                    Gtin = data.FirstOrDefault().Gtin,
                    Signs = new Signs()
                }
            };

            var d = new DirectoryInfo(path + @"\forUpload\");
            if (!d.Exists)
                d.Create();
            
            var counter = data.Count() / MaxCapacityOfTransferCodeToCustomMessages;
            if (data.Count() % MaxCapacityOfTransferCodeToCustomMessages != 0)
                counter++;

            //var tasks = new List<Task>();

            for (var i = 0; i < counter; i++)
            {
                var temp = data.Skip(i * MaxCapacityOfTransferCodeToCustomMessages)
                    .Take(MaxCapacityOfTransferCodeToCustomMessages).Select(s => $"{s.Gtin}{s.Sn}").ToList();
                doc.TransferCodeToCustom.Signs.Sgtin = temp;
                //foreach (var e in temp)
                //{
                //    doc.TransferCodeToCustom.Signs.Sgtin.Add($"{e.Gtin}{e.Sn}");
                //}
                var newPath = @$"{path}\forUpload\300-TransferCodeToCustom_{timestamp:yyyyMMddHHmmss}_{i + 1}.xml";

                //tasks.Add(Task.Run(() => doc.SerializerXml(newPath)));
                doc.SerializerXml(newPath);
            }

            //Task.WaitAll(tasks.ToArray());

            //_logger.Info($"Создано {tasks.Select(s => s.IsCompleted).Count()} документов. Должно быть: {counter}");
        }

        public static void CreateForeignEmissionMessages(this IEnumerable<CryptoCode> data, MsgHeaderData headerData, SupportDate support, string path, DateTime timestamp)
        {
            var doc = new Documents
            {
                Version = "1.35",
                ForeignEmission = new ForeignEmission 
                {
                    ActionId = "321",
                    SubjectId = headerData.HubSubjectId,
                    PackingId = headerData.HubSubjectId,
                    ControlId = headerData.HubSubjectId,
                    OperationDate = timestamp.ToString("yyyy-MM-ddTHH:mm:ss") + "+03:00",
                    SeriesNumber = support.Batch,
                    ExpirationDate = support.ExpirationDate.ToString("dd.MM.yyyy"),
                    Gtin = support.Gtin,
                    Signs = new Signs 
                    { 
                        Sgtin = new List<string>() 
                    }
                }
            };

            foreach (var e in data)
            {
                doc.ForeignEmission.Signs.Sgtin.Add(support.Gtin + e.Sn);
            }

            var newPath = path + @$"\forUpload\321-st_format_{timestamp:yyyyMMddHHmmss}_{support.Batch}.xml";

            doc.SerializerXml(newPath);
        }

        public static void CreateForeignShipmentMessages(this IEnumerable<SantensReport> data, MsgHeaderData headerData, SupportDate support, string path, DateTime timestamp)
        {
            var counter = data.Count() / MaxCapacityOfCreateForeignShipmentMessages;
            if (data.Count() % MaxCapacityOfCreateForeignShipmentMessages != 0)
                counter++;

            var doc = new Documents
            {
                Version = "1.36",
                ForeignShipment = new ForeignShipment 
                {
                    ActionId = "331",
                    SubjectId = headerData.HubSubjectId,
                    SellerId = headerData.HubSellerId,
                    ReceiverId = headerData.HubReceiverId,
                    CustomReceiverId = headerData.HubCustomReceiverId,
                    OperationDate = timestamp.ToString("yyyy-MM-ddTHH:mm:ss") + "+03:00",
                    ContractType = "1",
                    DocNum = support.DocNum,
                    DocDate = support.DocDate.ToString("dd.MM.yyyy"),
                    OrderDetails = new OrderDetails 
                    { 
                        Sscc = new List<string>() 
                    }
                }
            };

            var tasks = new List<Task>();

            for (var i = 0; i < counter; i++)
            {
                var temp = data.Skip(i * MaxCapacityOfCreateForeignShipmentMessages)
                    .Take(MaxCapacityOfCreateForeignShipmentMessages);
                doc.ForeignShipment.OrderDetails.Sscc.Clear();
                foreach (var e in temp)
                {
                    doc.ForeignShipment.OrderDetails.Sscc.Add(e.ParentContainer);
                }

                var newPath = $@"{path}\forUpload\331-st_format_{timestamp:yyyyMMddHHmmss}_{support.DocNum}_{i + 1}.xml";

                tasks.Add(Task.Run(() => doc.SerializerXml(newPath)));
            }

            Task.WaitAll(tasks.ToArray());

            _logger.Info($"Создано {tasks.Select(s => s.IsCompleted).Count()} документов. Должно быть: {counter}");
        }

        public static void CreateImportInfoMessages(this IEnumerable<SantensReport> data, MsgHeaderData headerData, SupportDate support, string path, DateTime timestamp)
        {
            var counter = data.Count() / MaxCapacityOfImportInfoMessages;
            if (data.Count() % MaxCapacityOfImportInfoMessages != 0)
                counter++;

            var doc = new Documents
            {
                Version = "1.36",
                SessionUi = "4Aa246a6-D7e2-2465-a056-0234554369a3",
                ImportInfo = new ImportInfo 
                {
                    ActionId = "336",
                    SubjectId = headerData.SubjectId,
                    SellerId = headerData.SellerId,
                    ReceiverId = headerData.ReceiverId,
                    OperationDate = timestamp.ToString("yyyy-MM-ddTHH:mm:ss") + "+03:00",
                    ContractType = "1",
                    DocNum = support.DocNum,
                    DocDate = support.DocDate.ToString("dd.MM.yyyy"),
                    OrderDetails = new OrderDetails
                    {
                        Sscc = new List<string>()
                    }
                }
            };

            var tasks = new List<Task>();

            for (var i = 0; i < counter; i++)
            {
                var temp = data.Skip(i * MaxCapacityOfImportInfoMessages).Take(MaxCapacityOfImportInfoMessages);
                doc.ImportInfo.OrderDetails.Sscc.Clear();
                foreach (var e in temp)
                {
                    doc.ImportInfo.OrderDetails.Sscc.Add(e.ParentContainer);
                }

                var newPath = $@"{path}\forUpload\336-st_format_{timestamp:yyyyMMddHHmmss}_{support.DocNum}_{i + 1}.xml";

                tasks.Add(Task.Run(() => doc.SerializerXml(newPath)));
            }

            Task.WaitAll(tasks.ToArray());

            _logger.Info($"Создано {tasks.Select(s => s.IsCompleted).Count()} документов. Должно быть: {counter}");
        }

        public static void LogTimer(string text)
        {
            _logger.Info(text);
        }

        public static void ErrorLog(Exception st)
        {
            _logger.Error(st, $"Возникла ошибка: \n--{st.Message} \n\n{st.StackTrace}");

        }
    }
}
