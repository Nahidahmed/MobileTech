using System;
using System.Xml.Serialization;

namespace MobileTech.Models.Entity
{
    public class Source {
        [XmlElement]
        public int Code { get; set; }
        [XmlElement(IsNullable = true)]
        public string Name { get; set; }
        [XmlElement]
        public long SouPrimaryId { get; set; }
        [XmlElement(IsNullable = true)]
        public string UsedTypeAs { get; set; }
        [XmlElement(IsNullable = true)]
        public string Division { get; set; }
    }

    /// <summary>
    /// Business entity used to model Source/Manufacturer/Vendor.
    /// </summary>
    public class SourceInfo : Source {
        [XmlElement(IsNullable = true)]
        public string PartsContact { get; set; }
        [XmlElement(IsNullable = true)]
        public string PartsPhone { get; set; }
        [XmlElement]
        public decimal PartsExtention { get; set; }
        [XmlElement(IsNullable = true)]
        public string PartsMobile { get; set; }
        [XmlElement(IsNullable = true)]
        public string PartsEmail { get; set; }
        [XmlElement(IsNullable = true)]
        public string CustomerNumber { get; set; }
        [XmlElement(IsNullable = true)]
        public string ReviewFlag { get; set; }
        [XmlElement]
        public int RentalCo { get; set; }
        [XmlElement(IsNullable = true)]
        public SourceCenter Center { get; set; }
    }

    public class SourceDetails : SourceInfo {
        [XmlElement]
        public byte AssetManufacturer { get; set; }
        [XmlElement]
        public byte AssetVendor { get; set; }
        [XmlElement]
        public byte MaterialsSupplier { get; set; }
        [XmlElement]
        public byte PurchaseRequestVendor { get; set; }
        [XmlElement]
        public byte LaborServiceProvider { get; set; }
        [XmlElement(IsNullable = true)]
        public string AutoShareSources { get; set; }
        [XmlElement(IsNullable = true)]
        public string AlternateNumber { get; set; }
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
        public string Attention { get; set; }
        [XmlElement(IsNullable = true)]
        public string FaxNumber { get; set; }
        [XmlElement(IsNullable = true)]
        public string PhoneNumber { get; set; }
        [XmlElement(IsNullable = true)]
        public string EMailAddress { get; set; }
        [XmlElement(IsNullable = true)]
        public string WebAddress { get; set; }
        [XmlElement(IsNullable = true)]
        public string ShippingDivision { get; set; }
        [XmlElement(IsNullable = true)]
        public string ShippingAddress1 { get; set; }
        [XmlElement(IsNullable = true)]
        public string ShippingAddress2 { get; set; }
        [XmlElement(IsNullable = true)]
        public string ShippingCity { get; set; }
        [XmlElement(IsNullable = true)]
        public string ShippingState { get; set; }
        [XmlElement(IsNullable = true)]
        public string ShippingZipCode { get; set; }
        [XmlElement(IsNullable = true)]
        public string ShippingCountry { get; set; }
        [XmlElement(IsNullable = true)]
        public string ShippingAttention { get; set; }
        [XmlElement(IsNullable = true)]
        public string ShippingFaxNumber { get; set; }
        [XmlElement(IsNullable = true)]
        public string SalesContact { get; set; }
        [XmlElement(IsNullable = true)]
        public string SalesPhone { get; set; }
        [XmlElement]
        public decimal SalesExtention { get; set; }
        [XmlElement(IsNullable = true)]
        public string SalesMobile { get; set; }
        [XmlElement(IsNullable = true)]
        public string SalesEmail { get; set; }
        [XmlElement(IsNullable = true)]
        public string ServiceContact { get; set; }
        [XmlElement(IsNullable = true)]
        public string ServicePhone { get; set; }
        [XmlElement]
        public decimal ServiceExtention { get; set; }
        [XmlElement(IsNullable = true)]
        public string ServiceMobile { get; set; }
        [XmlElement(IsNullable = true)]
        public string ServiceEmail { get; set; }
        [XmlElement]
        public decimal MinimumOrder { get; set; }
        [XmlElement(IsNullable = true)]
        public string SourceText { get; set; }
        [XmlElement]
        public byte ContractProvider { get; set; }
        [XmlElement]
        public byte RentalVendor { get; set; }
        [XmlElement]
        public byte LockCodeValue { get; set; }
        [XmlElement(IsNullable = true)]
        public string VarChargeMode { get; set; }
        [XmlElement]
        public byte ContractVendor { get; set; }
        [XmlElement(IsNullable = true)]
        public string UsedType { get; set; }
    }

    public class SourceSync {
        [XmlElement]
        public long SouPrimaryId { get; set; }
        [XmlElement]
        public string Name { get; set; }
        [XmlElement]
        public int SourceId { get; set; }
        [XmlElement]
        public string Division { get; set; }
    }

    public class Address {
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
        public string Attention { get; set; }
        [XmlElement(IsNullable = true)]
        public string FaxNumber { get; set; }
    }

    public class ShippingAddress : Address {
        [XmlElement(IsNullable = true)]
        public string Division { get; set; }
    }

    public class GeneralAddress : Address {
        [XmlElement(IsNullable = true)]
        public string PhoneNumber { get; set; }
        [XmlElement(IsNullable = true)]
        public string EMailAddress { get; set; }
        [XmlElement(IsNullable = true)]
        public string WebAddress { get; set; }
    }

    public class Contact {
        [XmlElement(IsNullable = true)]
        public string Name { get; set; }
        [XmlElement(IsNullable = true)]
        public string Phone { get; set; }
        [XmlElement]
        public decimal Extention { get; set; }
        [XmlElement(IsNullable = true)]
        public string Mobile { get; set; }
        [XmlElement(IsNullable = true)]
        public string Email { get; set; }
    }

    public class SourceCenter {
        [XmlElement]
        public long SwcPrimaryId { get; set; }
        [XmlElement(IsNullable = true)]
        public string Archived { get; set; }
        [XmlElement(IsNullable = true)]
        public SystemCode StockCenter { get; set; }
        [XmlElement(IsNullable = true)]
        public SystemCode ServiceCenter { get; set; }
    }

    public class SourceCenterDetails : SourceCenter {
        [XmlElement(IsNullable = true)]
        public string PoNumber { get; set; }
        [XmlElement(IsNullable = true)]
        public string CustomerNumber { get; set; }
        [XmlElement(IsNullable = true)]
        public string Abbreviation { get; set; }
        [XmlElement(IsNullable = true)]
        public string ReviewFlag { get; set; }
        [XmlElement]
        public DateTime ReviewDate { get; set; }
    }
}