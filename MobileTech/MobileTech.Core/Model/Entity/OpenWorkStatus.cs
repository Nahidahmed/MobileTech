using System.Xml.Serialization;

namespace MobileTech.Models.Entity
{
    public class OpenWorkStatusInfo : SystemCodeInfo {
        [XmlElement(IsNullable = true)]
        public string Notes { get; set; }
		[XmlElement(IsNullable = true)]
		public short RequestsActionCode { get; set; }

    }
}