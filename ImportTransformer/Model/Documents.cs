using System.Collections.Generic;
using System.Xml.Serialization;

namespace ImportTransformer.Model
{
    //10319, 300, 321, 331, 915, 336
    [XmlRoot(ElementName = "documents")]
	public class Documents
	{
		[XmlElement(ElementName = "skzkm_foreign_emission")]
		public SkzkmForeignEmission SkzkmForeignEmission { get; set; }
		[XmlAttribute(AttributeName = "session_ui")]
		public string SessionUi { get; set; }
		[XmlAttribute(AttributeName = "version")]
		public string Version { get; set; }
		[XmlElement(ElementName = "foreign_emission")]
		public ForeignEmission ForeignEmission { get; set; }
		[XmlElement(ElementName = "foreign_shipment")]
		public ForeignShipment ForeignShipment { get; set; }
		[XmlElement(ElementName = "import_info")]
		public ImportInfo ImportInfo { get; set; }
		[XmlElement(ElementName = "multi_pack")]
		public MultiPack MultiPack { get; set; }
		[XmlElement(ElementName = "transfer_code_to_custom")]
		public TransferCodeToCustom TransferCodeToCustom { get; set; }
		//[XmlAttribute(AttributeName = "xsi", Namespace = "http://www.w3.org/2000/xmlns/")]
		//public string Xsi { get; set; }
	}

	[XmlRoot(ElementName = "signs")]
	public class Signs
	{
		[XmlElement(ElementName = "sgtin")]
		public List<string> Sgtin { get; set; }
	}

	[XmlRoot(ElementName = "transfer_code_to_custom")]
	public class TransferCodeToCustom
	{
		[XmlElement(ElementName = "subject_id")]
		public string SubjectId { get; set; }
		[XmlElement(ElementName = "operation_date")]
		public string OperationDate { get; set; }
		[XmlElement(ElementName = "custom_receiver_id")]
		public string CustomReceiverId { get; set; }
		[XmlElement(ElementName = "gtin")]
		public string Gtin { get; set; }
		[XmlElement(ElementName = "signs")]
		public Signs Signs { get; set; }
		[XmlAttribute(AttributeName = "action_id")]
		public string ActionId { get; set; }
	}

	[XmlRoot(ElementName = "device_info")]
	public class DeviceInfo
	{
		[XmlElement(ElementName = "device_id")]
		public string DeviceId { get; set; }
		[XmlElement(ElementName = "skzkm_origin_msg_id")]
		public string SkzkmOriginMsgId { get; set; }
		[XmlElement(ElementName = "skzkm_report_id")]
		public string SkzkmReportId { get; set; }
	}

	[XmlRoot(ElementName = "skzkm_foreign_emission")]
	public class SkzkmForeignEmission
	{
		[XmlElement(ElementName = "subject_id")]
		public string SubjectId { get; set; }
		[XmlElement(ElementName = "operation_date")]
		public string OperationDate { get; set; }
		[XmlElement(ElementName = "packing_id")]
		public string PackingId { get; set; }
		[XmlElement(ElementName = "control_id")]
		public string ControlId { get; set; }
		[XmlElement(ElementName = "series_number")]
		public string SeriesNumber { get; set; }
		[XmlElement(ElementName = "expiration_date")]
		public string ExpirationDate { get; set; }
		[XmlElement(ElementName = "gtin")]
		public string Gtin { get; set; }
		[XmlElement(ElementName = "signs")]
		public Signs Signs { get; set; }
		[XmlElement(ElementName = "device_info")]
		public DeviceInfo DeviceInfo { get; set; }
		[XmlAttribute(AttributeName = "action_id")]
		public string ActionId { get; set; }
	}

	[XmlRoot(ElementName = "content")]
	public class Content
	{
		[XmlElement(ElementName = "sgtin")]
		public List<string> Sgtin { get; set; }
		[XmlElement(ElementName = "sscc")]
		public List<string> Sscc { get; set; }
	}

	[XmlRoot(ElementName = "detail")]
	public class Detail
	{
		[XmlElement(ElementName = "sscc")]
		public string Sscc { get; set; }
		[XmlElement(ElementName = "content")]
		public Content Content { get; set; }
	}

	[XmlRoot(ElementName = "by_sscc")]
	public class BySscc
	{
		[XmlElement(ElementName = "detail")]
		public List<Detail> Detail { get; set; }
	}

	[XmlRoot(ElementName = "by_sgtin")]
	public class BySgtin
	{
		[XmlElement(ElementName = "detail")]
		public List<Detail> Detail { get; set; }
	}

	[XmlRoot(ElementName = "multi_pack")]
	public class MultiPack
	{
		[XmlElement(ElementName = "subject_id")]
		public string SubjectId { get; set; }
		[XmlElement(ElementName = "operation_date")]
		public string OperationDate { get; set; }
		[XmlElement(ElementName = "by_sscc")]
		public BySscc BySscc { get; set; }
		[XmlElement(ElementName = "by_sgtin")]
		public BySgtin BySgtin { get; set; }
		[XmlAttribute(AttributeName = "action_id")]
		public string ActionId { get; set; }
	}

	[XmlRoot(ElementName = "order_details")]
	public class OrderDetails
	{
		[XmlElement(ElementName = "sgtin")]
		public List<string> Sgtin { get; set; }
		[XmlElement(ElementName = "sscc")]
		public List<string> Sscc { get; set; }
	}

	[XmlRoot(ElementName = "import_info")]
	public class ImportInfo
	{
		[XmlElement(ElementName = "subject_id")]
		public string SubjectId { get; set; }
		[XmlElement(ElementName = "seller_id")]
		public string SellerId { get; set; }
		[XmlElement(ElementName = "receiver_id")]
		public string ReceiverId { get; set; }
		[XmlElement(ElementName = "operation_date")]
		public string OperationDate { get; set; }
		[XmlElement(ElementName = "contract_type")]
		public string ContractType { get; set; }
		[XmlElement(ElementName = "doc_num")]
		public string DocNum { get; set; }
		[XmlElement(ElementName = "doc_date")]
		public string DocDate { get; set; }
		[XmlElement(ElementName = "order_details")]
		public OrderDetails OrderDetails { get; set; }
		[XmlAttribute(AttributeName = "action_id")]
		public string ActionId { get; set; }
	}

	[XmlRoot(ElementName = "foreign_shipment")]
	public class ForeignShipment
	{
		[XmlElement(ElementName = "subject_id")]
		public string SubjectId { get; set; }
		[XmlElement(ElementName = "seller_id")]
		public string SellerId { get; set; }
		[XmlElement(ElementName = "receiver_id")]
		public string ReceiverId { get; set; }
		[XmlElement(ElementName = "custom_receiver_id")]
		public string CustomReceiverId { get; set; }
		[XmlElement(ElementName = "operation_date")]
		public string OperationDate { get; set; }
		[XmlElement(ElementName = "contract_type")]
		public string ContractType { get; set; }
		[XmlElement(ElementName = "doc_num")]
		public string DocNum { get; set; }
		[XmlElement(ElementName = "doc_date")]
		public string DocDate { get; set; }
		[XmlElement(ElementName = "order_details")]
		public OrderDetails OrderDetails { get; set; }
		[XmlAttribute(AttributeName = "action_id")]
		public string ActionId { get; set; }
	}

	[XmlRoot(ElementName = "foreign_emission")]
	public class ForeignEmission
	{
		[XmlElement(ElementName = "subject_id")]
		public string SubjectId { get; set; }
		[XmlElement(ElementName = "operation_date")]
		public string OperationDate { get; set; }
		[XmlElement(ElementName = "packing_id")]
		public string PackingId { get; set; }
		[XmlElement(ElementName = "control_id")]
		public string ControlId { get; set; }
		[XmlElement(ElementName = "series_number")]
		public string SeriesNumber { get; set; }
		[XmlElement(ElementName = "expiration_date")]
		public string ExpirationDate { get; set; }
		[XmlElement(ElementName = "gtin")]
		public string Gtin { get; set; }
		[XmlElement(ElementName = "signs")]
		public Signs Signs { get; set; }
		[XmlAttribute(AttributeName = "action_id")]
		public string ActionId { get; set; }
	}
}
