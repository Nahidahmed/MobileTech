using System;
using System.Xml.Serialization;

namespace MobileTech.Models.Entity
{
    public enum SortMaterialsBy {
        StockNumber = 1,
        ItemDescription = 2,
        Description = 3,
        BinNumber = 4,
        VendorPartNumber = 5,
        VendorName = 6,
        Status = 7
    }

    public class Material {
        [XmlElement(IsNullable = true)]
        public string StockNumber { get; set; }
        [XmlElement(IsNullable = true)]
        public string Description { get; set; }
        [XmlElement]
        public long InvPrimaryId { get; set; }
    }

    public class MaterialInfo : Material {
        [XmlElement]
        public decimal ItemCost { get; set; }
        [XmlElement(IsNullable = true)]
        public MaterialCategory ItemCategory { get; set; }
        [XmlElement(IsNullable = true)]
        public PurchaseUnitDetails InventoryUnit { get; set; }
        [XmlElement(IsNullable = true)]
        public SystemCode InventoryCenter { get; set; }
    }

    public class MaterialDetails : MaterialInfo {
        [XmlElement(IsNullable = true)]
        public MaterialSupplier[] MaterialSuppliers { get; set; }
        [XmlElement]
        public decimal InternalCost { get; set; }
        [XmlElement]
        public decimal AffiliatedCost { get; set; }
        [XmlElement]
        public decimal ExternalCost { get; set; }
        [XmlElement(IsNullable = true)]
        public string Notes { get; set; }
        [XmlElement(IsNullable = true)]
        public string ReviewFlag { get; set; }
        [XmlElement(IsNullable = true)]
        public string Archived { get; set; }
        [XmlElement]
        public int OriginalId { get; set; }
        [XmlElement(IsNullable = true)]
        public string VendorPartNumber { get; set; }
        [XmlElement]
        public DateTime ReviewDate { get; set; }
    }

    public class MaterialDetailSync {
        [XmlElement]
        public long InvPrimaryId { get; set; }
        [XmlElement]
        public string StockNumber { get; set; }
        [XmlElement]
        public string Description { get; set; }
        [XmlElement(IsNullable = true)]
        public decimal? ItemCost { get; set; }
        [XmlElement(IsNullable = true)]
        public decimal? InternalCost { get; set; }
        [XmlElement(IsNullable = true)]
        public decimal? AffiliatedCost { get; set; }
        [XmlElement(IsNullable = true)]
        public decimal? ExternalCost { get; set; }
        [XmlElement(IsNullable = true)]
        public long? UNTPrimaryId { get; set; }
        [XmlElement]
        public string ReviewFlag { get; set; }
        [XmlElement]
        public long ICCPrimaryId { get; set; }
        [XmlElement]
        public long ITMPrimaryId { get; set; }
        [XmlElement]
        public string Archived { get; set; }
        [XmlElement(IsNullable = true)]
        public short? OriginalId { get; set; }
        [XmlElement(IsNullable = true)]
        public string VendorPartNumber { get; set; }
    }

    public class MaterialSupplier {
        [XmlElement(IsNullable = true)]
        public string VendorPricing { get; set; }
        [XmlElement(IsNullable = true)]
        public MaterialInfo Material { get; set; }
        [XmlElement(IsNullable = true)]
        public SourceInfo Vendor { get; set; }
        [XmlElement(IsNullable = true)]
        public string VendorPartNumber { get; set; }
        [XmlElement]
        public long PrimaryId { get; set; }

    }

    public class MaterialSupplierDetails : MaterialSupplier {
        [XmlElement]
        public decimal VendorPrice { get; set; }
        [XmlElement(IsNullable = true)]
        public PurchaseUnit PurchaseUnit { get; set; }
        [XmlElement(IsNullable = true)]
        public DateTime LastUsedDate { get; set; }

    }

    public class MaterialModelKit : Material {
        [XmlElement(IsNullable = true)]
        public ModelInfo ModelInfo { get; set; }
        [XmlElement]
        public long MDPPrimaryId { get; set; }
        [XmlElement(IsNullable = true)]
        public string Component { get; set; }
        [XmlElement]
        public int Standard { get; set; }
        [XmlElement]
        public int Alternative { get; set; }
        [XmlElement]
        public int PmLevel1 { get; set; }
        [XmlElement]
        public int PmLevel2 { get; set; }
        [XmlElement]
        public int PmLevel3 { get; set; }
        [XmlElement]
        public int PmLevel4 { get; set; }

        [XmlElement(IsNullable = true)]
        public MaterialCategory MaterialCategory { get; set; }

        [XmlElement(IsNullable = true)]
        public string UsageUnits { get; set; }
        [XmlElement(IsNullable = true)]
        public SystemCode ServiceCenter { get; set; }
    }
}
