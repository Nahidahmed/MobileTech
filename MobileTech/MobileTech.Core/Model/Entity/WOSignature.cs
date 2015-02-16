using System.Xml.Serialization;
using System;

namespace MobileTech.Models.Entity
{
    public class WOSignature
    {
        [XmlElement]
        public long WORPrimaryId { get; set; }

        [XmlElement(IsNullable = true)]
        public string CustomerName { get; set;}

        [XmlElement]
        public DateTime SignatureDate { get; set; }

        [XmlElement(IsNullable = true)]
        public byte[] Signature { get; set; }

        [XmlElement(IsNullable = true)]
        public string StrSignature { get; set; }

    }
}
