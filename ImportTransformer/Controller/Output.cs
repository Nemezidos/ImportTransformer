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
        
        private static Logger _logger = LogManager.GetCurrentClassLogger();

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

           _logger.Info($"Создано {tasks.Select(s => s.IsCompleted).Count()} документов. Должно быть: {counter}");
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
                doc.TransferCodeToCustom.Signs.Sgtin.Clear();
                foreach (var e in temp)
                {
                    doc.TransferCodeToCustom.Signs.Sgtin.Add($"{e.Gtin}{e.Sn}");
                }
                var newPath = @$"{path}\forUpload\300-TransferCodeToCustom_{timestamp:yyyyMMddHHmmss}_{i + 1}.xml";

                tasks.Add(Task.Run(() => doc.SerializerXml(newPath)));
            }

            Task.WaitAll(tasks.ToArray());

            _logger.Info($"Создано {tasks.Select(s => s.IsCompleted).Count()} документов. Должно быть: {counter}");
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
            //до 25_000 в одном файле

            var counter = (data.Count() / 25000) + 1;

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
                var temp = data.Skip(i * 25000).Take(25000);
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
            //до 25_000 в одном файле

            var counter = (data.Count() / 25000) + 1;

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
                var temp = data.Skip(i * 25000).Take(25000);
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
