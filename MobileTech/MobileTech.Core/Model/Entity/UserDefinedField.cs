using System.Xml.Serialization;

namespace MobileTech.Models.Entity
{
    public class UserDefinedField {
        [XmlElement]
        public long PrimaryId { get; set; }
        [XmlElement]
        public string LabelName { get; set; }
        [XmlElement]
        public string Data { get; set; }
        [XmlElement]
        public long UddPrimaryId { get; set; }
    }

    public class UserDefinedFieldInfo : UserDefinedField {
        [XmlElement(IsNullable = true)]
        public string Type { get; set; }
        [XmlElement(IsNullable = true)]
        public int? DecimalPlaces { get; set; }
        [XmlElement(IsNullable = true)]
        public byte? Required { get; set; }
        [XmlElement(IsNullable = true)]
        public byte? ReadOnly { get; set; }
        [XmlElement(IsNullable = true)]
        public byte? Hidden { get; set; }
        [XmlElement(IsNullable = true)]
        public int? SortOrder { get; set; }
        [XmlElement(IsNullable = true)]
        public string DefaultValue { get; set; }
        [XmlElement(IsNullable = true)]
        public SystemCode ServiceCenter { get; set; }
        [XmlElement(IsNullable = true)]
        public string Archived { get; set; }
        [XmlElement(IsNullable = true)]
        public UserDefinedListItem[] ListItems { get; set; }
    }

    public class ModelAssignmentUserDefinedField : UserDefinedField {
        [XmlElement(IsNullable = true)]
        public long? EUMPrimaryId { get; set; }

        [XmlElement(IsNullable = true)]
        public Model Model { get; set; }
    }

    public class DevcatAssignmentUserDefinedField : UserDefinedField {
        [XmlElement(IsNullable = true)]
        public long? EUDPrimaryId { get; set; }

        [XmlElement(IsNullable = true)]
        public DeviceCategory DeviceCategory { get; set; }
    }

    public class RiskAssignmentUserDefinedField : UserDefinedField {
        [XmlElement(IsNullable = true)]
        public long? EURPrimaryId { get; set; }
        [XmlElement(IsNullable = true)]
        public SystemCode Risk { get; set; }
    }

    public class UserDefinedListItem {
        [XmlElement(IsNullable = true)]
        public long? UDLPrimaryId { get; set; }
        [XmlElement(IsNullable = true)]
        public string Data { get; set; }
        [XmlElement(IsNullable = true)]
        public short? Order { get; set; }
    }

    public class UserDefinedListItemInfo : UserDefinedListItem {
        [XmlElement(IsNullable = true)]
        public long? UDFPrimaryId { get; set; }
        [XmlElement(IsNullable = true)]
        public long? LBLPrimaryId { get; set; }
    }

    public class AssetUserDefinedFieldData : UserDefinedField {
        [XmlElement(IsNullable = true)]
        public long? CUDPrimaryId { get; set; }
        [XmlElement(IsNullable = true)]
        public Asset Control { get; set; }
    }

    public class WorkOrderUserDefinedFieldData : UserDefinedField {
        [XmlElement(IsNullable = true)]
        public long? UDDPrimaryId { get; set; }
        [XmlElement(IsNullable = true)]
        public WorkOrder WO { get; set; }
    }
}
