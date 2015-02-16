using System;
using System.Xml.Serialization;

namespace MobileTech.Models.Entity
{
    public enum SortWorkerBy {
        Archived = 1,
        Worker = 2,
        Name = 3,
        Phone = 4,
        PrimaryId = 5
    }

    public class Worker {
        [XmlElement]
        public int Id { get; set; }
        [XmlElement(IsNullable = true)]
        public string Name { get; set; }
        [XmlElement]
        public long WKRPrimaryId { get; set; }
		[XmlElement]
		public DateTime LastSyncDate { get; set; }
    }

    public class WorkerInfo : Worker {
        [XmlElement]
        public DateTime StartDate { get; set; }
        [XmlElement]
        public DateTime EndDate { get; set; }
        [XmlElement(IsNullable = true)]
        public string WorkerPhone { get; set; }
        [XmlElement(IsNullable = true)]
        public string Status { get; set; }
        [XmlElement(IsNullable = true)]
        public WorkerTypeInfo WorkerType { get; set; }
        [XmlElement(IsNullable = true)]
        public WorkerCenter Center { get; set; }
        [XmlElement(IsNullable = true)]
        public Source Source { get; set; }
    }

    public class WorkerDetails : WorkerInfo {
        [XmlElement(IsNullable = true)]
        public string Address1 { get; set; }
        [XmlElement(IsNullable = true)]
        public string Address2 { get; set; }
        [XmlElement(IsNullable = true)]
        public string City { get; set; }
        [XmlElement(IsNullable = true)]
        public string State { get; set; }
        [XmlElement(IsNullable = true)]
        public string ZipCode { get; set; }
        [XmlElement(IsNullable = true)]
        public string HomePhone { get; set; }
        [XmlElement(IsNullable = true)]
        public string AlternatePhone { get; set; }
        [XmlElement(IsNullable = true)]
        public string PagerAvailable { get; set; }
        [XmlElement(IsNullable = true)]
        public Pager Pager { get; set; }
        [XmlElement(IsNullable = true)]
        public string EmailAddress { get; set; }
        [XmlElement]
        public long SIRolePrimaryId { get; set; }
        [XmlElement]
        public byte RoleWosyst { get; set; }
        [XmlElement]
        public byte SecurityLevel { get; set; }
        [XmlElement]
        public long LMRolePrimaryId { get; set; }
    }

    public enum WorkersType {
        Worker = 1,
        Vendor = 2,
        Both = 3,
    }
    /*
     *ToDo/M.D.Prasad: Uncomment this code.
    public class WorkerAssignment {
        [XmlElement]
        public byte ScheduleQueue { get; set; }
        [XmlElement(IsNullable = true)]
        public AssetInfo Asset { get; set; }
        [XmlElement(IsNullable = true)]
        public long EQWPrimaryId { get; set; }
        [XmlElement(IsNullable = true)]
        public SystemCode ServiceCenter { get; set; }
        [XmlElement(IsNullable = true)]
        public Worker Worker { get; set; }
    }
    */
    public class PagerProvider {
        [XmlElement]
        public long PrimaryId { get; set; }
        [XmlElement(IsNullable = true)]
        public string Name { get; set; }
    }

    public class WorkerPagerProvider : PagerProvider {
        [XmlElement]
        public byte BeeperEnabled { get; set; }
    }

    public class PagerProviderDetails : WorkerPagerProvider {
        [XmlElement(IsNullable = true)]
        public string AccessNumber { get; set; }
        [XmlElement(IsNullable = true)]
        public string Password { get; set; }
        [XmlElement(IsNullable = true)]
        public string BaudRate { get; set; }
        [XmlElement(IsNullable = true)]
        public string NoParity { get; set; }
        [XmlElement]
        public decimal MaxBlockSize { get; set; }
        [XmlElement]
        public decimal MaxMessageLen { get; set; }
        [XmlElement]
        public decimal MaxMessagesPerCall { get; set; }
        [XmlElement]
        public decimal BeeperDelay { get; set; }
        [XmlElement(IsNullable = true)]
        public string BeeperTermination { get; set; }
    }

    public class Pager {
        [XmlElement(IsNullable = true)]
        public string Id { get; set; }
        [XmlElement(IsNullable = true)]
        public PagerType Type { get; set; }
        [XmlElement(IsNullable = true)]
        public WorkerPagerProvider workerPagerProvider { get; set; }
    }

    public class PageRecipient {
        [XmlElement(IsNullable = true)]
        public Pager Pager { get; set; }
        [XmlElement(IsNullable = true)]
        public Worker Worker { get; set; }
        [XmlElement(IsNullable = true)]
        public Source Source { get; set; }
        [XmlElement(IsNullable = true)]
        public Account Account { get; set; }
    }

    public class WorkerCenter {
        [XmlElement]
        public long PrimaryId { get; set; }
        [XmlElement(IsNullable = true)]
        public SystemCode ServiceCenter { get; set; }
        [XmlElement(IsNullable = true)]
        public string Status { get; set; }
        [XmlElement]
        public DateTime StartDate { get; set; }
        [XmlElement]
        public DateTime EndDate { get; set; }
    }

    public class SafetyRole {
        [XmlElement]
        public int RoleSystem { get; set; }
        [XmlElement(IsNullable = true)]
        public string RoleNotes { get; set; }
        [XmlElement]
        public int RoleArchived { get; set; }
        [XmlElement]
        public int RoleIx { get; set; }
        [XmlElement]
        public long SIRolePrimaryId { get; set; }
        [XmlElement(IsNullable = true)]
        public string Role { get; set; }
    }

    public enum PagerType {
        AlphaNumeric = 0,
        Beeper = 1,
    }
}