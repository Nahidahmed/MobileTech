using System.Xml.Serialization;

namespace MobileTech.Models.Entity
{
    public class SystemCode {
        [XmlElement]
        public short Code { get; set; }
        [XmlElement]
        public string Description { get; set; }
        [XmlElement]
        public long PrimaryId { get; set; }
        [XmlElement(IsNullable = true)]
        public string PrimaryIds { get; set; }
        [XmlElement(IsNullable = true)]
        public string Codes { get; set; }
		[XmlElement(IsNullable = true)]
		public short RequestsActionCode { get; set; }

    }

    /// <summary>
    /// Business entity used to model System Code.
    /// </summary>
    public class SystemCodeInfo : SystemCode {
        [XmlElement(IsNullable = true)]
        public string Archived { get; set; }

        [XmlElement(IsNullable = true)]
        public string Abbreviation { get; set; }
    }

    public class Affiliation {
        [XmlElement]
        public bool Internal { get; set; }
        [XmlElement]
        public bool Affiliated { get; set; }
        [XmlElement]
        public bool NonAffiliated { get; set; }
    }

    public class LabourBilling {
        [XmlElement]
        public bool LaborNone { get; set; }
        [XmlElement]
        public bool LaborAccounting { get; set; }
        [XmlElement]
        public bool LaborBilled { get; set; }
    }

    public class PartsBilling {
        [XmlElement]
        public bool PartsNone { get; set; }
        [XmlElement]
        public bool PartsAccounting { get; set; }
        [XmlElement]
        public bool PartsBilled { get; set; }
        [XmlElement]
        public bool PartsPurchased { get; set; }
    }

    public class MiscBilling {
        [XmlElement]
        public bool MiscNone { get; set; }
        [XmlElement]
        public bool MiscAccounting { get; set; }
        [XmlElement]
        public bool MiscBilled { get; set; }
    }
}