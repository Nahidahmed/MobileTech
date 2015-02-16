using System;
using System.Xml.Serialization;

namespace MobileTech.Models.Entity
{
    public class WorkOrder {
        [XmlElement(IsNullable = true)]
        public string Number { get; set; }
        [XmlElement]
        public long WorPrimaryId { get; set; }
    }

    public class WorkOrderInfo : WorkOrder {
        [XmlElement(IsNullable = true)]
        public WorkerInfo Worker { get; set; }
        [XmlElement(IsNullable = true)]
        public RequestInfo Request { get; set; }
        [XmlElement]
        public DateTime IssueDate { get; set; }
        [XmlElement]
        public DateTime StartDate { get; set; }
        [XmlElement(IsNullable = true)]
        public WorkOrder OriginalWorkOrder { get; set; } 
        [XmlElement(IsNullable = true)]
        public AssetDetails AssetDetails { get; set; }
    }

    public class WorkOrderDetails : WorkOrderInfo {
        [XmlElement]
        public decimal BillableTime { get; set; }
        [XmlElement(IsNullable = true)]
        public UserDefinedFieldInfo[] UserDefinedfields { get; set; }
        [XmlElement(IsNullable = true)]
        public string PoNumber { get; set; }
        [XmlElement(IsNullable = true)]
        public string WOTextNotes { get; set; }
        [XmlElement]
        public decimal MiscCredits { get; set; }
        [XmlElement]
        public Decimal TotalCost { get; set; }
        [XmlElement(IsNullable = true)]
        public SystemCodeInfo ServiceCenter { get; set; }
        [XmlElement(IsNullable = true)]
        public SystemCode RateCategory { get; set; }
        [XmlElement(IsNullable = true)]
        public SystemCodeInfo Result { get; set; }
		[XmlElement(IsNullable = true)]
        public SystemCodeInfo Fault { get; set; }
		[XmlElement(IsNullable = true)]
        public UrgencyInfo Urgency { get; set; }
        [XmlElement]
        public DateTime IssueTime { get; set; }
        [XmlElement]
        public DateTime StartTime { get; set; }
        [XmlElement]
        public DateTime CompleteDate { get; set; }
        [XmlElement]
        public DateTime CompleteTime { get; set; }
        [XmlElement]
        public DateTime DateDue { get; set; }
        [XmlElement]
        public DateTime DueTime { get; set; }
        [XmlElement]
        public DateTime ReportDate { get; set; }
        [XmlElement]
        public byte PmLevel { get; set; }
        [XmlElement(IsNullable = true)]
        public Account Account { get; set; }
        [XmlElement]
        public decimal TotalHours { get; set; }
        [XmlElement]
        public decimal Rate { get; set; }
        [XmlElement]
        public decimal LaborCost { get; set; }
        [XmlElement]
        public decimal PartsCost { get; set; }
        [XmlElement]
        public decimal MiscCosts { get; set; }
        [XmlElement]
        public decimal Credits { get; set; }
        [XmlElement]
        public decimal Tax { get; set; }
        [XmlElement]
        public decimal TaxRate { get; set; }
        [XmlElement]
        public byte Copies { get; set; }
        [XmlElement(IsNullable = true)]
        public string SafetyTest { get; set; }
        [XmlElement]
        public byte OpenFlag { get; set; }
        [XmlElement]
        public DateTime PostingDate { get; set; }
        [XmlElement(IsNullable = true)]
        public string AlternateWorkOrderNumber { get; set; }
        [XmlElement(IsNullable = true)]
        public string Affiliation { get; set; }
        [XmlElement(IsNullable = true)]
        public WorkerTypeInfo WorkerType { get; set; }
        [XmlElement(IsNullable = true)]
        public SystemCode AssetStatus { get; set; }
        [XmlElement(IsNullable = true)]
        public SystemCode OpenWorkOrderStatus { get; set; }
        [XmlElement(IsNullable = true)]
        public string LaborBill { get; set; }
        [XmlElement(IsNullable = true)]
        public string LaborTax { get; set; }
        [XmlElement(IsNullable = true)]
        public string PartsBill { get; set; }
        [XmlElement(IsNullable = true)]
        public string PartsTax { get; set; }
        [XmlElement(IsNullable = true)]
        public string MiscBill { get; set; }
        [XmlElement(IsNullable = true)]
        public string MiscTax { get; set; }
        [XmlElement]
        public decimal DownTime { get; set; }
        [XmlElement(IsNullable = true)]
        public string Requester { get; set; }
        [XmlElement]
        public decimal ActualTime { get; set; }
        [XmlElement(IsNullable = true)]
        public WorkorderMiscCharge[] MiscellaneousCharges { get; set; }
        [XmlElement(IsNullable = true)]
        public WorkorderMiscCredit[] MiscellaneousCredits { get; set; }
        [XmlElement(IsNullable = true)]
        public WorkOrderPart[] Parts { get; set; }
        [XmlElement(IsNullable = true)]
        public WorkOrderTime[] TimeEntries { get; set; }
        [XmlElement(IsNullable = true)]
        public WorkOrderDeletedTimeEntries[] DeletedTimeEntries { get; set; }
        [XmlElement(IsNullable = true)]
        public WorkOrderDeletedMaterialEntries[] DeletedMaterialEntries { get; set; }
        [XmlElement(IsNullable = true)]
        public WOTaskInfo[] WOChecklistTasks { get; set; }
        [XmlElement(IsNullable = true)]
        public GroupWorkOrder GroupWorkOrder { get; set; }
        [XmlElement(IsNullable = true)]
        public WorkOrderPart[] PurchaseRequests { get; set; }
        [XmlElement(IsNullable = true)]
        public WOSignature WOSignature { get; set; }
        [XmlElement(IsNullable = true)]
        public string ApplicationName { get; set; }
        [XmlElement(IsNullable = true)]
        public string WODescription { get; set; }
		[XmlElement(IsNullable = true)]
		public bool insertUpdateFlag{ get; set;}
		[XmlElement]
		public DateTime LastSyncDateTime { get; set; }
		[XmlElement]
		public short LatestDispatchActionCode { get; set; }
		[XmlElement]
		public byte canWOBeClosed { get; set; }


		public WorkOrderDetails ShallowCopy()
		{
			return (WorkOrderDetails) this.MemberwiseClone();
		}
    }

    public class WorkOrderByTags
    {
        [XmlElement(IsNullable = true)]
        public string DevCat { get; set; }
        [XmlElement(IsNullable = true)]
        public string Model { get; set; }
        [XmlElement(IsNullable = true)]
        public string ControlNumber { get; set; }
        [XmlElement(IsNullable = true)]
        public string WorkOrderNumber { get; set; }
        [XmlElement(IsNullable = true)]
        public string WorkOrderPrimaryId { get; set; }
        [XmlElement(IsNullable = true)]
        public string WorkerPrimaryId { get; set; }
        [XmlElement(IsNullable = true)]
        public string UserName { get; set; }
        [XmlElement(IsNullable = true)]
        public string BadgeNumber { get; set; }        
    }

    #region "WORK ORDER TIME"

    public class WorkOrderTime {
        [XmlElement(IsNullable = true)]
        public WorkerTypeInfo WorkerTypeInfo { get; set; }
        [XmlElement(IsNullable = true)]
        public string AutoCredited { get; set; }
        [XmlElement]
        public Decimal MinimumTime { get; set; }
        [XmlElement]
        public Decimal TimeMultiplier { get; set; }
        [XmlElement]
        public long PrimaryId { get; set; }
        [XmlElement]
        public DateTime LogDate { get; set; }
        [XmlElement(IsNullable = true)]
        public Worker Worker { get; set; }
        [XmlElement(IsNullable = true)]
        public SystemCode TimeType { get; set; }
        [XmlElement]
        public decimal ActualTime { get; set; }
        [XmlElement]
        public byte NoCharge { get; set; }
        [XmlElement]
        public decimal HourlyRate { get; set; }
        [XmlElement]
        public decimal LaborCharge { get; set; }
        [XmlElement]
        public decimal BillableTime { get; set; }
        [XmlElement(IsNullable = true)]
        public SystemCode Task { get; set; }
        [XmlElement]
        public byte[] Notes { get; set; }
        [XmlElement(IsNullable = true)]
        public string strNotes { get; set; }

    }

    public class WorkOrderTimeDetails : WorkOrderTime {
        [XmlElement(IsNullable = true)]
        public WorkOrder WorkOrder { get; set; }
    }

    public class WorkOrderDeletedTimeEntries
    {
        [XmlElement]
        public long PrimaryId { get; set; }
    }


    #endregion

    #region "WORK ORDER PART"
    
    public class WorkOrderPart : Material {
        [XmlElement]
        public long PrimaryId { get; set; }
        [XmlElement]
        public long SCCPrimaryId { get; set; }
        [XmlElement]
        public int LineNumber { get; set; }
        [XmlElement]
        public DateTime LogDate { get; set; }
        [XmlElement(IsNullable = true)]
        public WorkerInfo Worker { get; set; }
        [XmlElement]
        public int Quantity { get; set; }
        [XmlElement(IsNullable = true)]
        public SystemCode StockCenter { get; set; }
        [XmlElement(IsNullable = true)]
        public string Designator { get; set; }
        [XmlElement]
        public decimal UnitAmount { get; set; }
        [XmlElement(IsNullable = true)]
        public string Credit { get; set; }
        [XmlElement]
        public byte StockPulled { get; set; }
        [XmlElement(IsNullable = true)]
        public string Purchase { get; set; }
        [XmlElement(IsNullable = true)]
        public string Ordered { get; set; }
        [XmlElement]
        public bool Other { get; set; }
        [XmlElement(IsNullable = true)]
        public string LotNumber { get; set; }
        [XmlElement]
        public long PPOPrimaryId { get; set; }
        [XmlElement(IsNullable = true)]
        public string PartPurchaseReqNumber { get; set; }
        [XmlElement]
        public long WMCPrimaryId { get; set; }
        [XmlElement]
        public long VENDORID { get; set; }  
        [XmlElement(IsNullable = true)]
        public string VENDORPN { get; set; }
        [XmlElement]
        public int REQQUANTITY { get; set; }
        [XmlElement]
        public long UNTPRIMARYID { get; set; }
        [XmlElement]
        public decimal UNITCOST { get; set; }
        [XmlElement]
        public long CHACCT { get; set; }
        [XmlElement(IsNullable = true)]
        public string CHARGETO { get; set; }
        [XmlElement]
        public long DELACCT { get; set; }
        [XmlElement(IsNullable = true)]
        public string DELIVERTO { get; set; }
        [XmlElement(IsNullable = true)]
        public string REQBY { get; set; }
        [XmlElement]
        public long PRIPRIMARYID { get; set; }
        [XmlElement]
        public DateTime REQDATE { get; set; }
        [XmlElement]
        public DateTime ORDDATE { get; set; }
        [XmlElement]
        public DateTime RECDATE { get; set; }
        [XmlElement]
        public int ORDQUANTITY { get; set; }
        [XmlElement]
        public int RECQUANTITY { get; set; }
        [XmlElement]
        public int RETURNED { get; set; }
        [XmlElement(IsNullable = true)]
        public string BUYER { get; set; }
        [XmlElement(IsNullable = true)]
        public string REMARKS { get; set; }
        [XmlElement]
        public short OPENPO { get; set; }
        [XmlElement]
        public short TOPROCESS { get; set; }
        [XmlElement]
        public short QTYRECORDED { get; set; }
        [XmlElement(IsNullable = true)]
        public string DETAILS { get; set; }
        [XmlElement(IsNullable = true)]
        public string NEEDEDBY { get; set; }
        [XmlElement(IsNullable = true)]
        public string InvoiceNumber { get; set; }
        [XmlElement(IsNullable = true)]
        public string LoginUserName { get; set; }
        [XmlElement]
        public DateTime InvoiceDate { get; set; }
        [XmlElement]
        public long PTTPrimaryId { get; set; }
        [XmlElement(IsNullable = true)]
        public string PONumber { get; set; }
        [XmlElement]
        public long WCCPrimaryId { get; set; }
        [XmlElement]
        public long WORPrimaryId { get; set; }
        [XmlElement]
        public long WOPPrimaryId { get; set; }
    }

    public class WorkOrderDeletedMaterialEntries
    {
        [XmlElement]
        public long PrimaryId { get; set; }
    }


    public class WorkOrderPartDetails : WorkOrderPart {
        [XmlElement(IsNullable = true)]
        public WorkOrder WorkOrder { get; set; }
    }
    
    #endregion

    public class WorkOrderText {
        [XmlElement(IsNullable = true)]
        public byte[] Text { get; set; }
        [XmlElement(IsNullable = true)]
        public string ToolTipText { get; set; }
        [XmlElement(IsNullable = true)]
        public string LogSheet { get; set; }
        [XmlElement]
        public short PmTemplateIn { get; set; }
        [XmlElement(IsNullable = true)]
        public WorkOrder WorkOrder { get; set; }
    }

    public enum SortWorkOrdersBy {
        WO = 1,
        Worker = 2,
        Control = 3,
        RequestCode = 4,
        DateIssue = 5,
        Urgency = 6,
        AccountId = 7,
        ResultCode = 8,
        FaultCode = 9,
        AlternateWoNumber = 10,
        OpenWorkStatus = 11,
        StartDate = 12,
        DeviceCategory = 13,
        GroupWorkOrder = 14
    }

    public enum WOStatus {
        Open = 1,
        Closed = 2,
        All = 3
    }

    #region "WorkOrderMiscCredits"
    
    public class WorkorderMiscCredit {
        [XmlElement]
        public long WOMCRPrimaryId { get; set; }
        [XmlElement]
        public DateTime LogDate { get; set; }
        [XmlElement(IsNullable = true)]
        public Worker Worker { get; set; }
        [XmlElement]
        public Decimal Credit { get; set; }
        [XmlElement(IsNullable = true)]
        public string Description { get; set; }
    }

    public class WorkorderMiscCreditDetails : WorkorderMiscCredit {
        [XmlElement(IsNullable = true)]
        public WorkOrder WorkOrder { get; set; }
    }
    
    #endregion

    #region "WorkOrderMiscCharges"
    
    public class WorkorderMiscCharge {
        [XmlElement]
        public long WOMCHPrimaryId { get; set; }
        [XmlElement]
        public DateTime LogDate { get; set; }
        [XmlElement(IsNullable = true)]
        public Worker Worker { get; set; }
        [XmlElement]
        public Decimal Charge { get; set; }
        [XmlElement(IsNullable = true)]
        public string Description { get; set; }
    }

    public class WorkorderMiscChargeDetails : WorkorderMiscCharge {
        [XmlElement(IsNullable = true)]
        public WorkOrder WorkOrder { get; set; }
    }

    #endregion

    #region Group Work Order

    public class GroupWorkOrder {
        [XmlElement(IsNullable = true)]
        public long? PrimaryId { get; set; }
        [XmlElement(IsNullable = true)]
        public string Description { get; set; }
    }

    public class GroupWorkOrderInfo : GroupWorkOrder {
        [XmlElement(IsNullable = true)]
        public SystemCode Request { get; set; }
        [XmlElement(IsNullable = true)]
        public Worker Worker { get; set; }
        [XmlElement(IsNullable = true)]
        public Procedure Procedure { get; set; }
        [XmlElement(IsNullable = true)]
        public DateTime? CreatedDate { get; set; }
        [XmlElement(IsNullable = true)]
        public string CreatedBy { get; set; }
        [XmlElement(IsNullable = true)]
        public DateTime? Closed { get; set; }
        [XmlElement(IsNullable = true)]
        public string GroupType { get; set; }
        [XmlElement(IsNullable = true)]
        public int? WOCount { get; set; }
        [XmlElement(IsNullable = true)]
        public int? VoidWOCount { get; set; }
        [XmlElement(IsNullable = true)]
        public int? SelectedAssetsCount { get; set; }
        [XmlElement(IsNullable = true)]
        public bool? UseAnsurTemplate { get; set; }
        [XmlElement(IsNullable = true)]
        public bool? UseWOTemplate { get; set; }
        [XmlElement(IsNullable = true)]
        public bool? UseProcedureText { get; set; }
        [XmlElement(IsNullable = true)]
        public bool? UseProcedureChecklist { get; set; }
    }
    
    #endregion

    #region WOTasks

    public class WOTaskInfo : ChecklistTaskInfo {
        [XmlElement]
        public int Order { get; set; }
        [XmlElement]
        public decimal TaskValue { get; set; }
        [XmlElement]
        public long WctPrimaryId { get; set; }
        [XmlElement]
        public decimal PMin { get; set; }
        [XmlElement]
        public decimal PMax { get; set; }
        [XmlElement(IsNullable = true)]
        public string Units { get; set; }
        [XmlElement]
        public bool ReadOnly { get; set; }
        //Author/Date/ISS-Srikanth Nuvvula/3rd May 2013/ISS-2973,3865
        [XmlElement]
        public bool AllowNA { get; set; }
    }
 
    public class WOTaskDetails : WOTaskInfo {
        [XmlElement]
        public WorkOrder Workorder { get; set; }
    }
    
    #endregion

    #region WOReassignment
    public class WorkOrderReassignment
    {
        [XmlElement]
        public long WORPrimaryId { get; set; }
        [XmlElement]
        public string WORKORDER { get; set; }
        [XmlElement]
        public long NewWKRPrimaryId { get; set; }
    }

    #endregion

    #region WorkOrderMaterialChargeType
    public class WorkOrderMaterialChargeType
    {
        [XmlElement]
        public long WMCPrimaryId { get; set; }
        [XmlElement(IsNullable = true)]
        public string Description { get; set; }
        [XmlElement(IsNullable = true)]
        public string AutoCredited { get; set; }
    }
    #endregion

}