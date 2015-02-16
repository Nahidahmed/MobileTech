using System;
using System.Xml.Serialization;

namespace MobileTech.Models.Entity
{
    public class Model {
        [XmlElement]
        public string ModelName { get; set; }
        [XmlElement(IsNullable = true)]
        public string Description { get; set; }
        [XmlElement]
        public long MkmdId { get; set; }
    }

    public class ModelInfo : Model {
        [XmlElement(IsNullable = true)]
        public DeviceCategoryInfo DeviceCategory { get; set; }
        [XmlElement(IsNullable = true)]
        public Source Manufacturer { get; set; }
        [XmlElement(IsNullable = true)]
        public string Archived { get; set; }
        [XmlElement(IsNullable = true)]
        public string Docs { get; set; }
        [XmlElement(IsNullable = true)]
        public ModelCenter Center { get; set; }
    }

    public class ModelDetails : ModelInfo {
        [XmlElement]
        public decimal LifeCode { get; set; }
        [XmlElement]
        public decimal PurchaseCost { get; set; }
        [XmlElement]
        public decimal FairValue { get; set; }
        [XmlElement(IsNullable = true)]
        public string Notes { get; set; }
        [XmlElement(IsNullable = true)]
        public string PhotoAddress { get; set; }
        [XmlElement]
        public int OriginalId { get; set; }
    }

    public class ModelSync {
        [XmlElement]
        public long MkmdId { get; set; }
        [XmlElement]
        public long DevPrimaryId { get; set; }
        [XmlElement]
        public long SouPrimaryId { get; set; }
        [XmlElement]
        public string Model { get; set; }
        [XmlElement(IsNullable = true)]
        public string Description { get; set; }
        [XmlElement]
        public string Archived { get; set; }
        [XmlElement(IsNullable = true)]
        public decimal? LifeCode { get; set; }
    }

    public class ModelCenter {
        [XmlElement]
        public long MMCPrimaryId { get; set; }
        [XmlElement]
        public byte DisableScheduling { get; set; }
        [XmlElement(IsNullable = true)]
        public string ReviewFlag { get; set; }
        [XmlElement]
        public DateTime ReviewDate { get; set; }
        [XmlElement]
        public byte OperatorManualExists { get; set; }
        [XmlElement(IsNullable = true)]
        public string OperatorManualLocation { get; set; }
        [XmlElement]
        public byte ServiceManualExists { get; set; }
        [XmlElement(IsNullable = true)]
        public string ServiceManualLocation { get; set; }
    }

    public class ModelCenterDetails : ModelCenter {
        [XmlElement(IsNullable = true)]
        public Model Model { get; set; }
        [XmlElement(IsNullable = true)]
        public SystemCode ServiceCenter { get; set; }
    }

    /// <summary>
    /// Model can be sorted by either of these values:
    /// Model, Manufacturer,Device category, Model Description, Documents, review date, archived
    /// </summary>
    public enum SortModelsBy {
        Model = 1,
        ModelDescription = 2,
        Manufacturer = 3,
        DeviceCategroy = 4,
        Documents = 5,
        ReviewDate = 6,
        PrimaryId = 7,
        Archived = 8
    }
}
