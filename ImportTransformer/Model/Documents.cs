using System.Collections.Generic;
using System.Xml.Serialization;

namespace ImportTransformer.Model
{
    //10319, 300, 321, 331, 915, 336
    [XmlRoot(ElementName = "documents")]
	public class Documents
	{
		[XmlElement(ElementName = "skzkm_foreign_emission")]
		public Skzkm_foreign_emission Skzkm_foreign_emission { get; set; }
		[XmlAttribute(AttributeName = "session_ui")]
		public string Session_ui { get; set; }
		[XmlAttribute(AttributeName = "version")]
		public string Version { get; set; }
		[XmlElement(ElementName = "foreign_emission")]
		public Foreign_emission Foreign_emission { get; set; }
		[XmlElement(ElementName = "foreign_shipment")]
		public Foreign_shipment Foreign_shipment { get; set; }
		[XmlElement(ElementName = "import_info")]
		public Import_info Import_info { get; set; }
		[XmlElement(ElementName = "multi_pack")]
		public Multi_pack Multi_pack { get; set; }
		[XmlElement(ElementName = "transfer_code_to_custom")]
		public Transfer_code_to_custom Transfer_code_to_custom { get; set; }
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
	public class Transfer_code_to_custom
	{
		[XmlElement(ElementName = "subject_id")]
		public string Subject_id { get; set; }
		[XmlElement(ElementName = "operation_date")]
		public string Operation_date { get; set; }
		[XmlElement(ElementName = "custom_receiver_id")]
		public string Custom_receiver_id { get; set; }
		[XmlElement(ElementName = "gtin")]
		public string Gtin { get; set; }
		[XmlElement(ElementName = "signs")]
		public Signs Signs { get; set; }
		[XmlAttribute(AttributeName = "action_id")]
		public string Action_id { get; set; }
	}

	[XmlRoot(ElementName = "device_info")]
	public class Device_info
	{
		[XmlElement(ElementName = "device_id")]
		public string Device_id { get; set; }
		[XmlElement(ElementName = "skzkm_origin_msg_id")]
		public string Skzkm_origin_msg_id { get; set; }
		[XmlElement(ElementName = "skzkm_report_id")]
		public string Skzkm_report_id { get; set; }
	}

	[XmlRoot(ElementName = "skzkm_foreign_emission")]
	public class Skzkm_foreign_emission
	{
		[XmlElement(ElementName = "subject_id")]
		public string Subject_id { get; set; }
		[XmlElement(ElementName = "operation_date")]
		public string Operation_date { get; set; }
		[XmlElement(ElementName = "packing_id")]
		public string Packing_id { get; set; }
		[XmlElement(ElementName = "control_id")]
		public string Control_id { get; set; }
		[XmlElement(ElementName = "series_number")]
		public string Series_number { get; set; }
		[XmlElement(ElementName = "expiration_date")]
		public string Expiration_date { get; set; }
		[XmlElement(ElementName = "gtin")]
		public string Gtin { get; set; }
		[XmlElement(ElementName = "signs")]
		public Signs Signs { get; set; }
		[XmlElement(ElementName = "device_info")]
		public Device_info Device_info { get; set; }
		[XmlAttribute(AttributeName = "action_id")]
		public string Action_id { get; set; }
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
	public class By_sscc
	{
		[XmlElement(ElementName = "detail")]
		public List<Detail> Detail { get; set; }
	}

	[XmlRoot(ElementName = "by_sgtin")]
	public class By_sgtin
	{
		[XmlElement(ElementName = "detail")]
		public List<Detail> Detail { get; set; }
	}

	[XmlRoot(ElementName = "multi_pack")]
	public class Multi_pack
	{
		[XmlElement(ElementName = "subject_id")]
		public string Subject_id { get; set; }
		[XmlElement(ElementName = "operation_date")]
		public string Operation_date { get; set; }
		[XmlElement(ElementName = "by_sscc")]
		public By_sscc By_sscc { get; set; }
		[XmlElement(ElementName = "by_sgtin")]
		public By_sgtin By_sgtin { get; set; }
		[XmlAttribute(AttributeName = "action_id")]
		public string Action_id { get; set; }
	}

	[XmlRoot(ElementName = "order_details")]
	public class Order_details
	{
		[XmlElement(ElementName = "sgtin")]
		public List<string> Sgtin { get; set; }
		[XmlElement(ElementName = "sscc")]
		public List<string> Sscc { get; set; }
	}

	[XmlRoot(ElementName = "import_info")]
	public class Import_info
	{
		[XmlElement(ElementName = "subject_id")]
		public string Subject_id { get; set; }
		[XmlElement(ElementName = "seller_id")]
		public string Seller_id { get; set; }
		[XmlElement(ElementName = "receiver_id")]
		public string Receiver_id { get; set; }
		[XmlElement(ElementName = "operation_date")]
		public string Operation_date { get; set; }
		[XmlElement(ElementName = "contract_type")]
		public string Contract_type { get; set; }
		[XmlElement(ElementName = "doc_num")]
		public string Doc_num { get; set; }
		[XmlElement(ElementName = "doc_date")]
		public string Doc_date { get; set; }
		[XmlElement(ElementName = "order_details")]
		public Order_details Order_details { get; set; }
		[XmlAttribute(AttributeName = "action_id")]
		public string Action_id { get; set; }
	}

	[XmlRoot(ElementName = "foreign_shipment")]
	public class Foreign_shipment
	{
		[XmlElement(ElementName = "subject_id")]
		public string Subject_id { get; set; }
		[XmlElement(ElementName = "seller_id")]
		public string Seller_id { get; set; }
		[XmlElement(ElementName = "receiver_id")]
		public string Receiver_id { get; set; }
		[XmlElement(ElementName = "custom_receiver_id")]
		public string Custom_receiver_id { get; set; }
		[XmlElement(ElementName = "operation_date")]
		public string Operation_date { get; set; }
		[XmlElement(ElementName = "contract_type")]
		public string Contract_type { get; set; }
		[XmlElement(ElementName = "doc_num")]
		public string Doc_num { get; set; }
		[XmlElement(ElementName = "doc_date")]
		public string Doc_date { get; set; }
		[XmlElement(ElementName = "order_details")]
		public Order_details Order_details { get; set; }
		[XmlAttribute(AttributeName = "action_id")]
		public string Action_id { get; set; }
	}

	[XmlRoot(ElementName = "foreign_emission")]
	public class Foreign_emission
	{
		[XmlElement(ElementName = "subject_id")]
		public string Subject_id { get; set; }
		[XmlElement(ElementName = "operation_date")]
		public string Operation_date { get; set; }
		[XmlElement(ElementName = "packing_id")]
		public string Packing_id { get; set; }
		[XmlElement(ElementName = "control_id")]
		public string Control_id { get; set; }
		[XmlElement(ElementName = "series_number")]
		public string Series_number { get; set; }
		[XmlElement(ElementName = "expiration_date")]
		public string Expiration_date { get; set; }
		[XmlElement(ElementName = "gtin")]
		public string Gtin { get; set; }
		[XmlElement(ElementName = "signs")]
		public Signs Signs { get; set; }
		[XmlAttribute(AttributeName = "action_id")]
		public string Action_id { get; set; }
	}
}
