using System.Xml.Serialization;
using System;

namespace MobileTech.Models.Entity
{
    public class ResultInfo : SystemCodeInfo {
        [XmlElement(IsNullable = true)]
        public string Notes { get; set; }
        [XmlElement(IsNullable = true)]
        public ResultCenter Center { get; set; }
    }

    public class ResultCenter {
        [XmlElement]
        public long RSCPrimaryId { get; set; }

        [XmlElement]
        public SystemCode ServiceCenter { get; set; }

        [XmlElement]
        public string Archived { get; set; }

        [XmlElement]
        public string ForceEdit { get; set; }

        [XmlElement]
        public string FullCredit { get; set; }
    }
}

