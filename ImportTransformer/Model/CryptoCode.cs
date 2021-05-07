using System;
using System.Collections.Generic;
using System.Text;

namespace ImportTransformer.Model
{
    public class CryptoCode
    {
        public CryptoCode(string gtin, string sn, string prefix, string cryptoKeyCode)
        {
            this.Gtin = gtin;
            this.Sn = sn;
            this.Prefix = prefix;
            this.CryptoKeyCode = cryptoKeyCode;
        }

        public CryptoCode()
        {

        }

        public string Gtin { get; set; }
        public string Sn { get; set; }
        public string Prefix { get; set; }
        public string CryptoKeyCode { get; set; }
    }
}
