using System;
using System.Collections.Generic;
using System.Text;

namespace ImportTransformer.Model
{
    public class MsgHeaderData
    {
        public MsgHeaderData(string sellerId, string subjectId, string packingId, string controlId, string receiverId, string customReceiverId, string hubSubjectId)
        {
            this.SellerId = sellerId;
            this.SubjectId = subjectId;
            this.PackingId = packingId;
            this.ControlId = controlId;
            this.ReceiverId = receiverId;
            this.CustomReceiverId = customReceiverId;
            this.HubSubjectId = hubSubjectId;
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
        public string HubSubjectId { get; set; }
    }
}
