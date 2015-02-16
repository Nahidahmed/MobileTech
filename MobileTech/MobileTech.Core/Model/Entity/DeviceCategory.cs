using System;
using System.Xml.Serialization;

namespace MobileTech.Models.Entity
{
    public class DeviceCategory {
        [XmlElement]
        public string DevCategory { get; set; }

        [XmlElement]
        public long DevPrimaryId { get; set; }
    }

    /// <summary>
    /// Entity used to model DeviceCategory
    /// </summary>
    public class DeviceCategoryInfo : DeviceCategory {
        [XmlElement(IsNullable = true)]
        public string Description { get; set; }

        [XmlElement(IsNullable = true)]
        public string UniversalId { get; set; }

        [XmlElement(IsNullable = true)]
        public SystemCode RiskGroup { get; set; }

        [XmlElement(IsNullable = true)]
        public DevCenter Center { get; set; }

        [XmlElement]
        public int MobileEnabled { get; set; }
    }

    public class DeviceCategorySync : DeviceCategory {
        [XmlElement]
        public string UniversalId { get; set; }
        
        [XmlElement]
        public string Description { get; set; }

        [XmlElement]
        public long RSKPrimaryId { get; set; }
    }

    /// <summary>
    /// Entity used to model Detialed DeviceCategory Information
    /// </summary>
    public class DeviceCategoryDetails : DeviceCategoryInfo {
        [XmlElement]
        public int OriginalId { get; set; }

        [XmlElement(IsNullable = true)]
        public string PhotoURL { get; set; }

        [XmlElement(IsNullable = true)]
        public DeviceCategory AccompanyingDevCat { get; set; }
    }

    public class DevCenter {
        [XmlElement]
        public long PrimaryId { get; set; }
        
        [XmlElement(IsNullable = true)]
        public string Archived { get; set; }

        [XmlElement]
        public byte DisableScheduling { get; set; }
       
        [XmlElement(IsNullable = true)]
        public string ReviewFlag { get; set; }

        [XmlElement(IsNullable = true)]
        public SystemCode ServiceCenter { get; set; }

        [XmlElement]
        public byte MobileEnabled { get; set; }

        [XmlElement]
        public byte Durable { get; set; }

        [XmlElement]
        public byte ControlRequired { get; set; }
    }

    public class DevCenterDetails : DevCenter {
        [XmlElement]
        public decimal SafetyAve { get; set; }

        [XmlElement]
        public decimal Pm1Jobs { get; set; }

        [XmlElement]
        public decimal Pm1Ave { get; set; }

        [XmlElement]
        public decimal Pm2Jobs { get; set; }

        [XmlElement]
        public decimal Pm2Ave { get; set; }

        [XmlElement]
        public decimal Pm3Jobs { get; set; }

        [XmlElement]
        public decimal Pm3Ave { get; set; }

        [XmlElement]
        public decimal Pm4Jobs { get; set; }

        [XmlElement]
        public decimal Pm4Ave { get; set; }

        [XmlElement]
        public DateTime ReviewDate { get; set; }

        [XmlElement]
        public byte OnePerPatient { get; set; }

        [XmlElement]
        public byte ChargeType { get; set; }

        [XmlElement(IsNullable = true)]
        public string ServiceCode { get; set; }

        [XmlElement]
        public decimal DailyRevenue { get; set; }

        [XmlElement]
        public byte DisableScheduleMobile { get; set; }
    }

    public enum SortDeviceCategoryBy {
        DeviceCategory = 1,
        RiskGroup = 2,
        UniversalId = 3,
        Description = 4,
        Flagged = 5,
        Mobile = 6,
        PrimaryId = 7,
        Archived = 8
    }
}