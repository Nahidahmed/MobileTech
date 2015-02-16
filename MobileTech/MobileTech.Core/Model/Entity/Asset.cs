using System;
using System.Xml.Serialization;

namespace MobileTech.Models.Entity
{
    /// <summary> 
    /// Represents an instance of a control. Holds basic information about 
    /// the Control Tag and the EquipId for an instance. 
    /// </summary> 
    public class Asset { 
        [XmlElement]
        public string Control { get; set; }

        [XmlElement]
        public long EquipId { get; set; }
    }

    public class AssetInfo : Asset {
        [XmlElement(IsNullable = true)]
        public AssetStatusInfo AssetStatus { get; set; }

        [XmlElement(IsNullable = true)]
        public DateTime? StatusDate { get; set; }

        [XmlElement(IsNullable = true)]
        public string BadgeNumber { get; set; }

        [XmlElement(IsNullable = true)]
        public long? BDGPrimaryId { get; set; }


        [XmlElement(IsNullable = true)]
        public SystemCode AssetCenter { get; set; }

        [XmlElement(IsNullable = true)]
        public AccountInfo OwnerAccount { get; set; }

        [XmlElement(IsNullable = true)]
        public AccountInfo LocationAccount { get; set; }

        [XmlElement(IsNullable = true)]
        public string Control2 { get; set; }

        [XmlElement(IsNullable = true)]
        public string SystemNumber { get; set; }

        [XmlElement(IsNullable = true)]
        public string PurchaseOrderNumber { get; set; }

        [XmlElement(IsNullable = true)]
        public SystemCode RateCategory { get; set; }

        [XmlElement(IsNullable = true)]
        public ModelInfo Model { get; set; }

        [XmlElement(IsNullable = true)]
        public string SerialNumber { get; set; }

        [XmlElement]
        public DateTime IncomingDate { get; set; }

        [XmlElement]
        public DateTime PurchaseDate { get; set; }

        [XmlElement]
        public decimal? Warranty { get; set; }

        [XmlElement]
        public decimal? PurchaseCost { get; set; }

        [XmlElement(IsNullable = true)]
        public Source PurchaseSource { get; set; }

        [XmlElement(IsNullable = true)]
        public string PropertyId { get; set; }

        [XmlElement(IsNullable = true)]
        public string Archived { get; set; }

        [XmlElement(IsNullable = true)]
        public string Notes { get; set; }

        [XmlElement(IsNullable = true)]
        public RoomInfo Room { get; set; }

        [XmlElement]
        public int OriginalId { get; set; }

        [XmlElement]
        public bool IsMobileAsset { get; set; }

        [XmlElement]
        public bool IsRentalAsset { get; set; }

        [XmlElement]
        public bool IsEvaluationAsset { get; set; }

        [XmlElement(IsNullable = true)]
        public string AdditionalNotes { get; set; }

        [XmlElement(IsNullable = true)]
        public string Coverage { get; set; }

        [XmlElement]
        public DateTime WarrantyEnd { get; set; }

        [XmlElement]
        public bool HasAttachments { get; set; }

        [XmlElement(IsNullable = true)]
        public string RoomNo { get; set; }
    }

    public class AssetDetails : AssetInfo {
        [XmlElement(IsNullable = true)]
        public string AuditNotes { get; set; }

        [XmlElement(IsNullable = true)]
        public SystemCode EquipUseStatus { get; set; }

        [XmlElement(IsNullable = true)]
        public bool? MobileEnabled { get; set; }

        [XmlElement(IsNullable = true)]
        public EqpCenter EqpCenter { get; set; }

        [XmlElement(IsNullable = true)]
        public UserDefinedFieldInfo[] AssetUserDefinedFields { get; set; }

		[XmlElement]
		public DateTime LastSyncDateTime { get; set; }

    }
   
    public class LocationMappingDetails
    {
        [XmlElement]
        public Int64 LMPPrimaryId { get; set; }

        [XmlElement]
        public string LocationCode { get; set; }

        [XmlElement]
        public Int64 FacPrimaryId { get; set; }

        [XmlElement]
        public Int64 AREPrimaryId { get; set; }

        [XmlElement]
        public Int64 ROMPrimaryId { get; set; }

        [XmlElement]
        public int MappingType { get; set; }

        [XmlElement]
        public string MonitorId { get; set; }
    }


    public class BadgesDetails
    {
        [XmlElement]
        public Int64 BDGPrimaryId { get; set; }

        [XmlElement]
        public string BadgeNumber { get; set; }

        [XmlElement]
        public string AssetLocatorRoom { get; set; }

        [XmlElement]
        public DateTime ALREntryTime { get; set; }

        [XmlElement]
        public DateTime ALRLastSeenTime { get; set; }

        [XmlElement]
        public int BadgeCurrentStatus { get; set; }

        [XmlElement]
        public int AssignedBadge { get; set; }

        [XmlElement]
        public Int64 RoutingObjectId { get; set; }

        [XmlElement]
        public int AccountType { get; set; }

        [XmlElement]
        public string Archived { get; set; }

        [XmlElement]
        public int BadgeAssociationType { get; set; }

        [XmlElement]
        public Boolean TrackLocation { get; set; }

        [XmlElement]
        public Int64 LMPPrimaryId { get; set; }

        [XmlElement]
        public string Notes { get; set; }
    }

    public class AssetSyncDetail {
        [XmlElement]
        public long EquipId { get; set; }
        [XmlElement]
        public long ESCPrimaryId { get; set; }
        [XmlElement]
        public DateTime StatusDate { get; set; }
        [XmlElement]
        public long CCCPrimaryId { get; set; }
        [XmlElement(IsNullable = true)]
        public string BadgeNumber{ get; set; }              
        [XmlElement]
        public long Owner { get; set; }
        [XmlElement]
        public long Location { get; set; }
        [XmlElement]
        public string Control { get; set; }
        [XmlElement(IsNullable = true)]
        public string Control2 { get; set; }
        [XmlElement(IsNullable = true)]
        public string SystemNumber { get; set; }
        [XmlElement(IsNullable = true)]
        public string PONumber { get; set; }
        [XmlElement]
        public long RGCPrimaryId { get; set; }
        [XmlElement]
        public long MKMDID { get; set; }
        [XmlElement(IsNullable = true)]
        public string Serial { get; set; }
        [XmlElement]
        public DateTime InDate { get; set; }
        [XmlElement(IsNullable = true)]
        public DateTime? PURCHDATE { get; set; }
        [XmlElement(IsNullable = true)]
        public decimal? WARRANTY { get; set; }
        [XmlElement(IsNullable = true)]
        public decimal? PURCHCOST { get; set; }
        [XmlElement(IsNullable = true)]
        public long? PURCHSRCE { get; set; }
        [XmlElement]
        public string PROPERTYID { get; set; }
        [XmlElement]
        public string Archived { get; set; }
        [XmlElement(IsNullable = true)]
        public string Notes { get; set; }
        [XmlElement(IsNullable = true)]
        public string ROOM { get; set; }
        [XmlElement(IsNullable = true)]
        public bool? Mobile { get; set; }
        [XmlElement(IsNullable = true)]
        public long? ROMPrimaryId { get; set; }
        [XmlElement(IsNullable = true)]
        public long? BDGPrimaryId { get; set; }
        [XmlElement(IsNullable = true)]
        public long? BDGPrimaryId2 { get; set; }
        [XmlElement(IsNullable = true)]
        public string Coverage { get; set; }
        [XmlElement(IsNullable = true)]
        public string DC { get; set; }
        [XmlElement(IsNullable = true)]
        public string Mdl { get; set; }
        [XmlElement]
        public int woCount { get; set; }
    }

    public class EqpCenter {
        [XmlElement]
        public long EqcPrimaryId { get; set; }
        
        [XmlElement(IsNullable = true)]
        public WorkOrder LastSafetyWorkOrder { get; set; }

        [XmlElement]
        public DateTime LastSafetyDate { get; set; }

        [XmlElement(IsNullable = true)]
        public string LastSafetyTestResult { get; set; }

        [XmlElement(IsNullable = true)]
        public WorkOrder LastPM1WorkOrder { get; set; }

        [XmlElement]
        public DateTime LastPM1Date { get; set; }

        [XmlElement(IsNullable = true)]
        public WorkOrder LastPM2WorkOrder { get; set; }

        [XmlElement]
        public DateTime LastPM2Date { get; set; }

        [XmlElement(IsNullable = true)]
        public WorkOrder LastPM3WorkOrder { get; set; }

        [XmlElement]
        public DateTime LastPM3Date { get; set; }

        [XmlElement(IsNullable = true)]
        public WorkOrder LastPM4WorkOrder { get; set; }

        [XmlElement]
        public DateTime LastPM4Date { get; set; }

        [XmlElement(IsNullable = true)]
        public WorkOrder LastMaintenanceWorkOrder { get; set; }

        [XmlElement]
        public DateTime LastMaintenanceDate { get; set; }

        [XmlElement]
        public DateTime LastSchedulerRunDate { get; set; }

        [XmlElement(IsNullable = true)]
        public WorkOrder SchedulerWorkOrder { get; set; }

        [XmlElement]
        public bool DisableScheduling { get; set; }

        [XmlElement(IsNullable = true)]
        public DateTime? SchedulingStartDate { get; set; }

        [XmlElement(IsNullable = true)]
        public SystemCode Location { get; set; }

        [XmlElement(IsNullable = true)]
        public string ReviewFlag { get; set; }

        [XmlElement]
        public DateTime ReviewDate { get; set; }
    }

    /// <summary>
    /// Equipment Center links an equipment with a workcenter.
    /// </summary>
    public class EqpCenterDetails : EqpCenter {
        [XmlElement(IsNullable = true)]
        public Asset Asset { get; set; }

        [XmlElement(IsNullable = true)]
        public SystemCode ServiceCenter { get; set; }
    }

    public class EqpCenterSyncDetails {
        [XmlElement]
        public long EquipId { get; set; }

        [XmlElement]
        public long EQCPrimaryId { get; set; }

        [XmlElement]
        public long WCCPrimaryId { get; set; }

        [XmlElement(IsNullable = true)]
        public string LastSftWO { get; set; }

        [XmlElement(IsNullable = true)]
        public DateTime? LastSftDate { get; set; }

        [XmlElement(IsNullable = true)]
        public string LastSftPass { get; set; }

        [XmlElement(IsNullable = true)]
        public string LastPm1WO { get; set; }

        [XmlElement(IsNullable = true)]
        public DateTime? LastPm1Date { get; set; }

        [XmlElement(IsNullable = true)]
        public string LastPm2WO { get; set; }

        [XmlElement(IsNullable = true)]
        public DateTime? LastPm2Date { get; set; }

        [XmlElement(IsNullable = true)]
        public string LastPm3WO { get; set; }

        [XmlElement(IsNullable = true)]
        public DateTime? LastPm3Date { get; set; }

        [XmlElement(IsNullable = true)]
        public string LastPm4WO { get; set; }

        [XmlElement(IsNullable = true)]
        public DateTime? LastPm4Date { get; set; }

        [XmlElement(IsNullable = true)]
        public string LastMntWO { get; set; }

        [XmlElement(IsNullable = true)]
        public DateTime? LastMntDate { get; set; }

        [XmlElement(IsNullable = true)]
        public bool? DisableScheduling { get; set; }

        [XmlElement(IsNullable = true)]
        public DateTime? SchdStartDate { get; set; }

        [XmlElement(IsNullable = true)]
        public long? LOCPrimaryId { get; set; }

        [XmlElement(IsNullable = true)]
        public string ReviewFlag { get; set; }

        [XmlElement(IsNullable = true)]
        public DateTime? ReviewDate { get; set; }
    }
    
    public enum SortAssetsBy {
        Devcat = 0,
        Control = 1,
        Serial = 2,
        OwnerAccount = 3,
        Scheduling = 4,
        Manufacturer = 5,
        Model = 6,
        Status = 7,
        Vendor = 8,
        Control2 = 9,
        SystemNumber = 10,
        PurchaseOrderNumber = 11,
        IncomingDate = 12,
        UniversalID = 13,
        PropertyTag = 14,
        LocationAccount = 15,
        AssetLocatorRoom = 16,
        Room = 17,
        Building = 18
    }

    public enum SafetyResult {
        None = 0,
        Pass = 1,
    }

    public class AssetStatusInfo : SystemCodeInfo {
        [XmlElement(IsNullable = true)]
        public string DisableScheduling { get; set; }
    }

    public class AssetCenterLinkage {
        [XmlElement]
        public long CclPrimaryId { get; set; }

        [XmlElement]
        public SystemCode ServiceCenter { get; set; }

        [XmlElement(IsNullable = true)]
        public SystemCode AssetCenter { get; set; }
    }

    public class AssetCenterLinkagesSync {
        [XmlElement]
        public long CCLPrimaryId { get; set; }
        [XmlElement]
        public long WCCPrimaryId { get; set; }
        [XmlElement]
        public long CCCPrimaryId { get; set; }
    }

    public class AssetNotes : Asset {
        [XmlElement(IsNullable = true)]
        public byte[] AdditionalNotes { get; set; }
    }
}
