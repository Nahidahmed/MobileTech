using System;
using System.Xml.Serialization;

namespace MobileTech.Models.Entity
{
    public class PDADBID {
        [XmlElement]
        public int Dbid { get; set; }

        [XmlElement]
        public string username { get; set; }

        [XmlElement]
        public long WORPrimaryId { get; set; }

        [XmlElement]
        public long WOPPrimaryid { get; set; }

        [XmlElement]
        public long WOMPrimaryid { get; set; }

        [XmlElement]
        public long DevPrimaryid { get; set; }

        [XmlElement]
        public long MKMDid { get; set; }

        [XmlElement]
        public long SouPrimaryid { get; set; }

        [XmlElement]
        public long Equipid { get; set; }

        [XmlElement]
        public long UDDPrimaryid { get; set; }

        [XmlElement]
        public long WOTPrimaryid { get; set; }
        //Added by Srikanth Nuvvula on 10th June 2013 for BA#24623 for IAE PDADBID
        [XmlElement]
        public long WOMCHPrimaryid { get; set; }

        [XmlElement]
        public long WOMCRPrimaryid { get; set; }

        [XmlElement]
        public long PPOPrimaryid { get; set; }

    }
}