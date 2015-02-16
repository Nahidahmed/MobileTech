using System;
using System.Xml.Serialization;

namespace MobileTech.Models.Entity
{
    public class SyncParams {
        public enum SyncParamType {
            MSPrimaryId,
            STNPrimaryId,
            WCCPrimaryId,
            CCCPrimaryId,
            FACPrimaryId,
            WKRPrimaryId,
            IncludeUnassignedWorkOrders,
            ExcludeAssetsNotAssociatedWithOpenWorkOrders,
            IncludeOtherWorkersWorkOrders,
            Archived,
            LastUpdateTime,
            User
        }

        [XmlElement]
        public long MSPrimaryId { get; set; }

        [XmlElement]
        public long STNPrimaryId { get; set; }

        [XmlElement]
        public long WCCPrimaryId { get; set; }

        [XmlElement]
        public long CCCPrimaryId { get; set; }

        [XmlElement]
        public long FACPrimaryId { get; set; }

        [XmlElement]
        public long WKRPrimaryId { get; set; }

        [XmlElement]
        public bool IncludeUnassignedWorkOrders { get; set; }

        [XmlElement]
        public bool ExcludeAssetsNotAssociatedWithOpenWorkOrders { get; set; }
        //Author/Date/CRR - Srikanth Nuvuula/20120824/CRR 976
        //Added to Include Other Workers Work Orders

        [XmlElement]
        public bool IncludeOtherWorkersWorkOrders { get; set; }
        
        [XmlElement]
        public DateTime LastUpdateTime { get; set; }

        [XmlElement]
        public string User { get; set; }
    }
}
