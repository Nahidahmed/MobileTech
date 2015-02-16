using System;
using System.Xml.Serialization;

namespace MobileTech.Models.Entity {
    public class User {
        [XmlElement]
        public long ID { get; set; }

        [XmlElement]
        public string Username { get; set; }

        [XmlElement]
        public string DisplayName { get; set; }

        [XmlElement(IsNullable = true)]
        public string Password { get; set; }

        [XmlElement(IsNullable = true)]
        public DateTime? LastUpdateDateTime { get; set; }

        [XmlElement(IsNullable = true)]
        public DateTime? FinalDateTime { get; set; }
    }
}
