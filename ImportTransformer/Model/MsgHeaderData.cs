namespace ImportTransformer.Model
{
    public class MsgHeaderData
    {
        public MsgHeaderData(string sellerId, string subjectId, string packingId, string controlId, string receiverId, string customReceiverId,
            string hubSubjectId, string hubSellerId, string hubReceiverId, string hubCustomReceiverId)
        {
            this.SellerId = sellerId;
            this.SubjectId = subjectId;
            this.PackingId = packingId;
            this.ControlId = controlId;
            this.ReceiverId = receiverId;
            this.CustomReceiverId = customReceiverId;

            this.HubSellerId = hubSellerId;
            this.HubSubjectId = hubSubjectId;
            this.HubReceiverId = hubReceiverId;
            this.HubCustomReceiverId = hubCustomReceiverId;
        }

        public MsgHeaderData()
        {

        }

        public string SellerId { get; set; }
        public string SubjectId { get; set; }
        public string PackingId { get; set; }
        public string ControlId { get; set; }
        public string ReceiverId { get; set; }
        public string CustomReceiverId { get; set; }
        public string HubSellerId { get; set; }
        public string HubSubjectId { get; set; }
        public string HubReceiverId { get; set; }
        public string HubCustomReceiverId { get; set; }
    }
}
