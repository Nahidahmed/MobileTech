using System.Xml.Serialization;

namespace MobileTech.Models.Entity
{
    public class MaterialCategory {
        [XmlElement(IsNullable = true)]
        public string MaterialCategoryName { get; set; }
        [XmlElement]
        public long ItmPrimaryId { get; set; }
    }

    public class MaterialCategoryInfo : MaterialCategory {
        [XmlElement(IsNullable = true)]
        public SystemCode InventoryCenter { get; set; }
    }
}
