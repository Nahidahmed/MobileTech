using System.Xml.Serialization;

namespace MobileTech.Models.Entity
{
    public class Room {
        [XmlElement(IsNullable = true)]
        public string Number { get; set; }
        [XmlElement]
        public long PrimaryId { get; set; }
    }

    public class RoomInfo : Room {
        [XmlElement(IsNullable = true)]
        public string Archived { get; set; }
        [XmlElement(IsNullable = true)]
        public AreaInfo Area { get; set; }
    }

    public class RoomDetails : RoomInfo {
        [XmlElement(IsNullable = true)]
        public string Notes { get; set; }
        [XmlElement]
        public bool OutPatientRoom { get; set; }
    }
}
