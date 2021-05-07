using System;
using System.Collections.Generic;
using System.Text;

namespace ImportTransformer.Model
{
    public class SupportDate
    {
        public string TracelinkFilename { get; set; }
        public string SantensFilename { get; set; }
        public string Gtin { get; set; }
        public string Batch { get; set; }
        public DateTime ExpirationDate { get; set; }
        public string DocNum { get; set; }
        public DateTime DocDate { get; set; }
    }
}
