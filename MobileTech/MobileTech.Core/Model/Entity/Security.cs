using System;
using System.Xml.Serialization;

namespace MobileTech.Models.Entity{
    public class Security {
        [XmlElement(IsNullable = true)]
        public string UserName { get; set; }
        [XmlElement]
        public long SecPrimaryId { get; set; }
        [XmlElement]
        public int UserId { get; set; }
    }

    public class SecurityInfo : Security {
        [XmlElement(IsNullable = true)]
        public Worker AssociatedWorker { get; set; }
        [XmlElement]
        public byte AccessLevel { get; set; }
        [XmlElement(IsNullable = true)]
        public string WindowsUser { get; set; }
        [XmlElement(IsNullable = true)]
        public WorkerTypeInfo WorkerTypeInfo { get; set; }
    }

    public class SecurityDetails : SecurityInfo {
        [XmlElement(IsNullable = true)]
        public byte[] Password { get; set; }
        [XmlElement]
        public byte AccessToWosyst { get; set; }
        [XmlElement]
        public byte AccessToReports { get; set; }
        [XmlElement]
        public byte AccessToServiceRequester { get; set; }
        [XmlElement]
        public byte AccessToServiceCenterManager { get; set; }
        [XmlElement]
        public DateTime LastPasswordChangeDate { get; set; }
        [XmlElement]
        public DateTime LastPasswordChangeTime { get; set; }
        [XmlElement(IsNullable = true)]
        public string EmailAddress { get; set; }
        [XmlElement]
        public short UnSuccessfullLoginAttempts { get; set; }
        [XmlElement]
        public short DefaultLogin { get; set; }
        [XmlElement]
        public byte AuthenticateType { get; set; }
        [XmlElement]
        public long SIRolePrimaryID { get; set; }
        [XmlElement]
        public long LMRolePrimaryID { get; set; }
        [XmlElement]
        public short LoginAttemps { get; set; }
        [XmlElement]
        public int UserStatus { get; set; }
        [XmlElement]
        public byte CodeKey { get; set; }
        [XmlElement]
        public byte RoleMAE { get; set; }
        [XmlElement]
        public byte RoleiTrackIt { get; set; }

    }

    public class SecuritiesSync {
        [XmlElement]
        public long SecPrimaryId { get; set; }
        [XmlElement]
        public string UserName { get; set; }
        [XmlElement]
        public int UserId { get; set; }
        [XmlElement]
        public byte[] Password { get; set; }
        [XmlElement]
        public byte CodeKey { get; set; }
        [XmlElement(IsNullable = true)]
        public byte? AccessToWosyst { get; set; }
        [XmlElement(IsNullable = true)]
        public long? AssociatedWorker { get; set; }
        [XmlElement(IsNullable = true)]
        public byte? RTLSEnabled { get; set; }
        //Modified by Srikanth Nuvvula on 14th Nov'2014 for ISS - 4736(If Windows user tries to Re-Open WO, its raising an alert & it need to be fixed.)
        //Added WindowsUser, unable to fetch security details when logged with Windows User
        [XmlElement]
        public string WindowsUser { get; set; }
    }
}
