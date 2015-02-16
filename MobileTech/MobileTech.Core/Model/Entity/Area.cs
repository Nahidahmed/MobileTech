using System.Xml.Serialization;

namespace MobileTech.Models.Entity
{
    public class Area {
        [XmlElement(IsNullable = true)]
        public string Name { get; set; }

        [XmlElement]
        public long PrimaryId { get; set; }

        [XmlElement(IsNullable = true)]
        public string Abbreviation { get; set; }
    }

    public class AreaInfo : Area {
        [XmlElement(IsNullable = true)]
        public string Archived { get; set; }

        [XmlElement(IsNullable = true)]
        public Account Account { get; set; }

        [XmlElement(IsNullable = true)]
        public AreaBuilding AreaBuilding { get; set; }
    }

    public class AreaDetails : AreaInfo {
        [XmlElement(IsNullable = true)]
        public string ContactName { get; set; }

        [XmlElement(IsNullable = true)]
        public string ContactPhone { get; set; }
    }

    public class AreaBuilding {
        [XmlElement]
        public long ABLPrimaryId { get; set; }

        [XmlElement(IsNullable = true)]
        public string Name { get; set; }

        [XmlElement(IsNullable = true)]
        public string Abbreviation { get; set; }
    }

    public class AreaBuildingInfo : AreaBuilding {
        [XmlElement(IsNullable = true)]
        public string Archived { get; set; }
    }
}
