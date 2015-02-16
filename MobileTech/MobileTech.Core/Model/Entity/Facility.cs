using System;
using System.Xml.Serialization;

namespace MobileTech.Models.Entity
{
    public class Facility {
        [XmlElement]
        public string Name { get; set; }

        [XmlElement]
        public long FacPrimaryId { get; set; }

        [XmlElement(IsNullable = true)]
        public string Abbreviation { get; set; }

		[XmlElement]
		public DateTime LastSyncDate { get; set; }
    }

    /// <summary>
    /// Bussiness entity used to model AccountFacility (Lookup/Lister)
    /// </summary>
    public class FacilityInfo : Facility {
        [XmlElement(IsNullable = true)]
        public string TaxStatus { get; set; }

        [XmlElement(IsNullable = true)]
        public string HomeState { get; set; }

        [XmlElement]
        public byte FiscalYear { get; set; }

        [XmlElement(IsNullable = true)]
        public FacilityCenterDetails Center { get; set; }
    }

    public class FacilityCenterSync {
        [XmlElement]
        public long FacPrimaryId { get; set; }
        [XmlElement]
        public long FCTPrimaryId { get; set; }
        [XmlElement]
        public string Archived { get; set; }
        [XmlElement(IsNullable = true)]
        public string ScheduleLock { get; set; }
        [XmlElement]
        public long SCSPrimaryId { get; set; }
        [XmlElement]
        public long WCCPrimaryId { get; set; }
    }

    public class FacilityDetails : FacilityInfo {
        [XmlElement(IsNullable = true)]
        public string TaxId { get; set; }

        [XmlElement(IsNullable = true)]
        public string HomeCountry { get; set; }
        
        [XmlElement]
        public decimal TaxRate { get; set; }

        [XmlElement(IsNullable = true)]
        public string LaborTax { get; set; }

        [XmlElement(IsNullable = true)]
        public string PartsTax { get; set; }
        
        [XmlElement(IsNullable = true)]
        public string MiscTax { get; set; }

        [XmlElement]
        public int OriginalId { get; set; }
    }

    public class FacilitiesSync {
        [XmlElement]
        public long FacPrimaryId { get; set; }

        [XmlElement]
        public string Facility { get; set; }

        [XmlElement(IsNullable = true)]
        public string TaxStatus { get; set; }

        [XmlElement(IsNullable = true)]
        public decimal? TaxRate { get; set; }

        [XmlElement(IsNullable = true)]
        public string LaborTax { get; set; }

        [XmlElement(IsNullable = true)]
        public string PartsTax { get; set; }

        [XmlElement(IsNullable = true)]
        public string MiscTax { get; set; }
    }

    public class FacilityCenter {
        [XmlElement]
        public long FctPrimaryId { get; set; }

        [XmlElement(IsNullable = true)]
        public string Archived { get; set; }

        [XmlElement(IsNullable = true)]
        public string ReviewFlag { get; set; }

        [XmlElement]
        public DateTime ReviewDate { get; set; }
    }

    public class FacilityCenterDetails : FacilityCenter {
        [XmlElement(IsNullable = true)]
        public string ScheduleLock { get; set; }

        [XmlElement]
        public byte MinInt { get; set; }

        [XmlElement]
        public byte FirstTest { get; set; }

        [XmlElement(IsNullable = true)]
        public string ReportsDesired { get; set; }

        [XmlElement]
        public byte CoverSheets { get; set; }

        [XmlElement]
        public byte PmControl { get; set; }

        [XmlElement]
        public byte PmDescription { get; set; }

        [XmlElement]
        public byte PmPriority { get; set; }

        [XmlElement]
        public byte SafetyControl { get; set; }

        [XmlElement]
        public byte SafetyDescription { get; set; }

        [XmlElement]
        public byte SafetyPriority { get; set; }

        [XmlElement]
        public byte CombinedControl { get; set; }

        [XmlElement]
        public byte CombinedDescription { get; set; }

        [XmlElement]
        public byte CombinedPriority { get; set; }

        [XmlElement(IsNullable = true)]
        public string FormType { get; set; }

        [XmlElement(IsNullable = true)]
        public string FormSorting { get; set; }

        [XmlElement(IsNullable = true)]
        public string FormRequests { get; set; }

        [XmlElement]
        public byte AllowFormReprints { get; set; }

        [XmlElement]
        public byte ReprintSamePeriod { get; set; }

        [XmlElement]
        public byte DisableScheduling { get; set; }

        [XmlElement]
        public byte RequesterEnabled { get; set; }

        [XmlElement(IsNullable = true)]
        public ScheduleSourceInfo ScheduleSource { get; set; }

        [XmlElement(IsNullable = true)]
        public SystemCode ServiceCenter { get; set; }
    }

    public class ScheduleSourceInfo {
        [XmlElement(IsNullable = true)]
        public string ScheduleSource { get; set; }

        [XmlElement]
        public long ScsPrimaryId { get; set; }
    }
}
