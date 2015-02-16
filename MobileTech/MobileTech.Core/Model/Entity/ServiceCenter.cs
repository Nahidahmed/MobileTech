using System.Xml.Serialization;

namespace MobileTech.Models.Entity
{
    public class ServiceCenterInfo : SystemCodeInfo {
        [XmlElement]
        public short FiscalMonth { get; set; }
        [XmlElement]
        public SystemCode InventoryCenter { get; set; }
        [XmlElement]
        public Account Account { get; set; }
    }
}
