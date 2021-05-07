using ImportTransformer.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Input.Manipulations;

namespace ImportTransformer.Controller
{
    class Output
    {
        /// <summary>
        /// Создаёт утилизейшон репорты
        /// </summary>
        /// <param name="codes"></param>
        /// <param name="path"></param>
        public static void CreateUtilisationReport(List<CryptoCode> codes, string path, DateTime timestamp)
        {
            string GS = Encoding.ASCII.GetString(new byte[1] { 29 });

            int counter = (codes.Count / 30000) + 1;
            //до 30_000 в одном файле

            for (int i = 0; i < counter; i++)
            {
                var temp = codes.Skip(i * 30000).Take(30000).ToList();

                string newPath = path + @$"\forUpload\UtilReport_{timestamp:yyyyMMddHHmmss}_{i + 1}.csv";

                using (StreamWriter sw = new StreamWriter(newPath, false, new UTF8Encoding(false)))
                {
                    foreach (var e in temp)
                    {
                        sw.WriteLine($"01{e.Gtin}21{e.Sn}{GS}91{e.Prefix}{GS}92{e.CryptoKeyCode}");
                    }
                }
                Logging.LogFileName(newPath, path);
            }
        }

        public static void Create915Message(List<SantensReport> data, string gtin, MsgHeaderData headerData, string path, DateTime timestamp)
        {
            Documents doc = new Documents
            {
                Version = "1.36",
                Multi_pack = new Multi_pack()
            };
            string marker;
            doc.Multi_pack.Action_id = "915";
            doc.Multi_pack.Subject_id = headerData.HubSubjectId;

            doc.Multi_pack.Operation_date = timestamp.ToString("yyyy-MM-ddTHH:mm:ss") + "+03:00";

            if (data[0].Content[0].Length > 13)
            {
                doc.Multi_pack.By_sscc = new By_sscc
                {
                    Detail = new List<Detail>()
                };

                for (int i = 0; i < data.Count(); i++)
                {
                    doc.Multi_pack.By_sscc.Detail.Add(new Detail());
                    doc.Multi_pack.By_sscc.Detail[i].Sscc = data[i].ParentContainer;
                    doc.Multi_pack.By_sscc.Detail[i].Content = new Content
                    {
                        Sscc = data[i].Content
                    };
                }

                marker = "pallet";
            }
            else
            {
                doc.Multi_pack.By_sgtin = new By_sgtin
                {
                    Detail = new List<Detail>()
                };

                for (int i = 0; i < data.Count(); i++)
                {
                    doc.Multi_pack.By_sgtin.Detail.Add(new Detail());
                    doc.Multi_pack.By_sgtin.Detail[i].Sscc = data[i].ParentContainer;
                    doc.Multi_pack.By_sgtin.Detail[i].Content = new Content
                    {
                        Sgtin = new List<string>()
                    };
                    for (int j = 0; j < data[i].Content.Count(); j++)
                    {
                        doc.Multi_pack.By_sgtin.Detail[i].Content.Sgtin.Add(gtin + data[i].Content[j]);
                    }
                }

                marker = "case";
            }

            string newPath = path + $@"\forUpload\915-st_format_{timestamp:yyyyMMddHHmmss}_{marker}.xml";

            Serializer.SerializerXml(newPath, doc);
            Logging.LogFileName(newPath, path);
        }

        public static void Create300Message(List<CryptoCode> data, MsgHeaderData headerData, string path, DateTime timestamp)
        {
            Documents doc = new Documents
            {
                Version = "1.36",
                Session_ui = "4Aa246a6-D7e2-2465-a056-0234554369a3",
                Transfer_code_to_custom = new Transfer_code_to_custom()
            };

            doc.Transfer_code_to_custom.Action_id = "300";
            doc.Transfer_code_to_custom.Operation_date = timestamp.ToString("yyyy-MM-ddTHH:mm:ss") + "+03:00";
            doc.Transfer_code_to_custom.Subject_id = headerData.SubjectId;
            doc.Transfer_code_to_custom.Custom_receiver_id = headerData.CustomReceiverId;

            doc.Transfer_code_to_custom.Gtin = data[0].Gtin;

            doc.Transfer_code_to_custom.Signs = new Signs
            {
                Sgtin = new List<string>()
            };

            DirectoryInfo d = new DirectoryInfo(path + @"\forUpload\");
            if (!d.Exists)
                d.Create();

            //до 10_000 в одном файле
            int counter = (data.Count() / 10000) + 1;

            for (int i = 0; i < counter; i++)
            {
                var temp = data.Skip(i * 10000).Take(10000).ToList();
                doc.Transfer_code_to_custom.Signs.Sgtin.Clear();
                foreach (var e in temp)
                {
                    doc.Transfer_code_to_custom.Signs.Sgtin.Add($"{e.Gtin}{e.Sn}");
                }
                string newPath = path + @$"\forUpload\300-TransferCodeToCustom_{timestamp:yyyyMMddHHmmss}_{(i + 1)}.xml";

                Serializer.SerializerXml(newPath, doc);
                Logging.LogFileName(newPath, path);
            }

        }

        public static void Create321Message(List<CryptoCode> data, MsgHeaderData headerData, SupportDate support, string path, DateTime timestamp)
        {
            Documents doc = new Documents
            {
                Version = "1.35",
                Foreign_emission = new Foreign_emission()
            };
            doc.Foreign_emission.Action_id = "321";
            doc.Foreign_emission.Subject_id = headerData.HubSubjectId;
            doc.Foreign_emission.Packing_id = headerData.HubSubjectId;
            doc.Foreign_emission.Control_id = headerData.HubSubjectId;

            doc.Foreign_emission.Operation_date = timestamp.ToString("yyyy-MM-ddTHH:mm:ss") + "+03:00";

            doc.Foreign_emission.Series_number = support.Batch;
            doc.Foreign_emission.Expiration_date = support.ExpirationDate.ToString("dd.MM.yyyy");

            doc.Foreign_emission.Gtin = support.Gtin;
            doc.Foreign_emission.Signs = new Signs { Sgtin = new List<string>() };

            foreach (var e in data)
            {
                doc.Foreign_emission.Signs.Sgtin.Add(support.Gtin + e.Sn);
            }

            string newPath = path + @$"\forUpload\321-st_format_{timestamp:yyyyMMddHHmmss}_{support.Batch}.xml";

            Serializer.SerializerXml(newPath, doc);
            Logging.LogFileName(newPath, path);
        }

        public static void Create331Message(List<SantensReport> data, MsgHeaderData headerData, SupportDate support, string path, DateTime timestamp)
        {
            //до 25_000 в одном файле

            int counter = (data.Count() / 25000) + 1;

            Documents doc = new Documents
            {
                Version = "1.36",
                Foreign_shipment = new Foreign_shipment()
            };
            doc.Foreign_shipment.Action_id = "331";
            doc.Foreign_shipment.Subject_id = headerData.HubSubjectId;
            doc.Foreign_shipment.Seller_id = headerData.SellerId;
            doc.Foreign_shipment.Receiver_id = headerData.ReceiverId;
            doc.Foreign_shipment.Custom_receiver_id = headerData.CustomReceiverId;

            doc.Foreign_shipment.Operation_date = timestamp.ToString("yyyy-MM-ddTHH:mm:ss") + "+03:00";

            doc.Foreign_shipment.Contract_type = "1";
            doc.Foreign_shipment.Doc_num = support.DocNum;
            doc.Foreign_shipment.Doc_date = support.DocDate.ToString("dd.MM.yyyy");
            doc.Foreign_shipment.Order_details = new Order_details { Sscc = new List<string>() };

            for (int i = 0; i < counter; i++)
            {
                var temp = data.Skip(i * 25000).Take(25000).ToList();
                doc.Foreign_shipment.Order_details.Sscc.Clear();
                foreach (var e in temp)
                {
                    doc.Foreign_shipment.Order_details.Sscc.Add(e.ParentContainer);
                }

                string newPath = path + $@"\forUpload\331-st_format_{timestamp:yyyyMMddHHmmss}_{support.DocNum}_{i + 1}.xml";

                Serializer.SerializerXml(newPath, doc);
                Logging.LogFileName(newPath, path);
            }
        }

        public static void Create336Message(List<SantensReport> data, MsgHeaderData headerData, SupportDate support, string path, DateTime timestamp)
        {
            //до 25_000 в одном файле

            int counter = (data.Count() / 25000) + 1;

            Documents doc = new Documents
            {
                Version = "1.36",
                Session_ui = "4Aa246a6-D7e2-2465-a056-0234554369a3",
                Import_info = new Import_info()
            };
            doc.Import_info.Action_id = "336";
            doc.Import_info.Subject_id = headerData.SubjectId;
            doc.Import_info.Seller_id = headerData.SellerId;
            doc.Import_info.Receiver_id = headerData.ReceiverId;

            doc.Import_info.Operation_date = timestamp.ToString("yyyy-MM-ddTHH:mm:ss") + "+03:00";

            doc.Import_info.Contract_type = "1";

            doc.Import_info.Doc_num = support.DocNum;
            doc.Import_info.Doc_date = support.DocDate.ToString("dd.MM.yyyy");

            doc.Import_info.Order_details = new Order_details
            {
                Sscc = new List<string>()
            };

            for (int i = 0; i < counter; i++)
            {
                var temp = data.Skip(i * 25000).Take(25000).ToList();
                doc.Import_info.Order_details.Sscc.Clear();
                foreach (var e in temp)
                {
                    doc.Import_info.Order_details.Sscc.Add(e.ParentContainer);
                }

                string newPath = path + $@"\forUpload\336-st_format_{timestamp:yyyyMMddHHmmss}_{support.DocNum}_{i + 1}.xml";

                Serializer.SerializerXml(newPath, doc);
                Logging.LogFileName(newPath, path);
            }
        }
    }
}
