using System;
using System.Xml.Serialization;

namespace MobileTech.Models.Entity
{
    /// <summary>
    /// Types of Accounts available in AE. 
    /// 1:AccountType.Owner 
    /// 2:AccountType.Location 
    /// 3:AccountType.Both
    /// </summary>
    public enum AccountType {
        Owner = 1,
        Location = 2,
        Both = 3,
    }

    /// <summary>
    /// Accounts can be sorted by either of these values:
    /// Account, Facility, DepartmentName, PrimaryId, Archived
    /// </summary>
    public enum SortAccountsBy {
        Account = 1,
        Facility = 2,
        DepartmentName = 3,
        PrimaryId = 4,
        Archived = 5
    }

    /// <summary> This class is used to maintain the details of the account.</summary>
    /// <remarks>accountId,departmentName,accPrimaryId are the properties of this class
    /// <para> aedev is the database used
    /// </para>
    /// </remarks>
    /// <example> The following code example demonstrates the usage of account class
    /// <code>public class Account{..} </code>
    /// </example>	
    public class Account {
        /// <summary>
        /// Unique Value that identifies Account Entity
        /// </summary>
        [XmlElement]
        public string AccountId { get; set; }

        /// <summary>
        /// The user name/label assigned to this account
        /// </summary>
        [XmlElement]
        public string DepartmentName { get; set; }

        /// <summary>
        /// The accPrimaryId of the account
        /// </summary>
        [XmlElement]
        public long AccPrimaryId { get; set; }
    }

    /// <summary>
    /// Business entity used to model Department/Account (Lookup/Lister).
    /// </summary>
    
    public class AccountInfo : Account {
        [XmlElement]
        public FacilityInfo Facility { get; set; }
        
        [XmlElement(IsNullable = true)]
        public Room Room { get; set; }

        [XmlElement(IsNullable = true)]
        public Area Area { get; set; }

        [XmlElement(IsNullable = true)]
        public byte? AccountType { get; set; }

        [XmlElement(IsNullable = true)]
        public AccountCenterDetails Center { get; set; }
    }
    
      
    public class AccountSync {
        [XmlElement]
        public long AccPrimaryId { get; set; }

        [XmlElement]
        public string AccountId { get; set; }

        [XmlElement]
        public long FacPrimaryId { get; set; }

        [XmlElement]
        public string DeptName { get; set; }
    }

    /*
     * ToDo/M.D.Prasad: Uncomment this code.
    /// <summary>
    /// Entity used to model Detailed Account Information.
    /// </summary>
    public class AccountDetails : AccountInfo {
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
        public string Country { get; set; }

        [XmlElement(IsNullable = true)]
        public string DepartmentPhone { get; set; }

        [XmlElement(IsNullable = true)]
        public decimal? DepartmentExtn { get; set; }

        [XmlElement(IsNullable = true)]
        public string Pager { get; set; }

        [XmlElement(IsNullable = true)]
        public string FaxNumber { get; set; }

        [XmlElement(IsNullable = true)]
        public string ContactName { get; set; }

        [XmlElement(IsNullable = true)]
        public string ContactPhone { get; set; }

        [XmlElement(IsNullable = true)]
        public decimal? ContactExtension { get; set; }

        [XmlElement(IsNullable = true)]
        public string ContactPager { get; set; }
    }
    */

    public class AccountCenter {
        [XmlElement]
        public long ActPrimaryId { get; set; }

        [XmlElement]
        public string Archived { get; set; }

        [XmlElement(IsNullable = true)]
        public SystemCode ServiceCenter { get; set; }

        [XmlElement(IsNullable = true)]
        public string ReviewFlag { get; set; }

        [XmlElement(IsNullable = true)]
        public DateTime? ReviewDate { get; set; }

        [XmlElement]
        public byte DisableScheduling { get; set; }
    }

    public class AccountCenterDetails : AccountCenter {
        [XmlElement(IsNullable = true)]
        public string Affiliated { get; set; }

        [XmlElement(IsNullable = true)]
        public SystemCode RateGroup { get; set; }

        [XmlElement(IsNullable = true)]
        public SystemCode Location { get; set; }

        [XmlElement(IsNullable = true)]
        public string ChargeTo { get; set; }

        [XmlElement(IsNullable = true)]
        public string LaborBill { get; set; }

        [XmlElement(IsNullable = true)]
        public string PartsBill { get; set; }

        [XmlElement(IsNullable = true)]
        public string MiscBill { get; set; }

        [XmlElement(IsNullable = true)]
        public SystemCode StockCenter { get; set; }

        [XmlElement(IsNullable = true)]
        public string ChargePart { get; set; }

        [XmlElement(IsNullable = true)]
        public string ScheduleLock { get; set; }

        [XmlElement(IsNullable = true)]
        public byte? Min_Int { get; set; }

        [XmlElement(IsNullable = true)]
        public byte? FirstTest { get; set; }
    }
}
