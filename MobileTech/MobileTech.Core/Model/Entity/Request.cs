using System.Xml.Serialization;

namespace MobileTech.Models.Entity
{
    public class RequestInfo : SystemCodeInfo {
        [XmlElement(IsNullable = true)]
        public string Notes { get; set; }
        [XmlElement(IsNullable = true)]
        public RequestCenter Center { get; set; }
    }

    public class RequestCenter {
        [XmlElement]
        public long RqcPrimaryId { get; set; }

        [XmlElement]
        public SystemCode ServiceCenter { get; set; }

        [XmlElement]
        public string ForceEdit { get; set; }

        [XmlElement]
        public string RequireControl { get; set; }

        [XmlElement]
        public string RequireFaultCode { get; set; }

        [XmlElement]
        public SystemCode SafetyFailedRequest { get; set; }

        [XmlElement(IsNullable = true)]
        public string OffTheBooks { get; set; }
    }

    public class RequestSubClass : SystemCodeInfo {
        [XmlElement]
        public string Notes { get; set; }
        [XmlElement]
        public decimal DefaultTime { get; set; }
    }

    public class ValidRequestsResult {
        [XmlElement]
        public long VRRPrimaryId { get; set; }

        [XmlElement]
        public SystemCode ServiceCenter { get; set; }

        [XmlElement]
        public SystemCode Request { get; set; }

        [XmlElement]
        public SystemCode Result { get; set; }
    }
}