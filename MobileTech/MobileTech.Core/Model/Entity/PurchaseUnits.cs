using System.Xml.Serialization;

namespace MobileTech.Models.Entity
{
    public class PurchaseUnit {
        [XmlElement(IsNullable = true)]
        public string Unit { get; set; }
        [XmlElement]
        public long UntPrimaryId { get; set; }
    }

    public class PurchaseUnitInfo : PurchaseUnit {
        [XmlElement(IsNullable = true)]
        public string Archived { get; set; }
        [XmlElement(IsNullable = true)]
        public SystemCode InventoryCenter { get; set; }
    }

    public class PurchaseUnitDetails : PurchaseUnitInfo {
        [XmlElement]
        public int EquivalentUsageUnits { get; set; }
    }

    public enum SortPurchaseUnitsBy {
        Archived = 1,
        Units = 2,
        PrimaryId = 3
    }
}
