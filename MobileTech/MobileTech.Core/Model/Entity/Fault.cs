using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace MobileTech.Models.Entity
{
    public class FaultInfo : SystemCodeInfo
    {
        [XmlElement(IsNullable = true)]
        public string Notes { get; set; }
        [XmlElement(IsNullable = true)]
        public FaultCenter Center { get; set; }
    }

    public class FaultCenter
    {
        [XmlElement]
        public long FLCPrimaryId { get; set; }

        [XmlElement]
        public SystemCode ServiceCenter { get; set; }

        [XmlElement]
        public string Archived { get; set; }
    }
}