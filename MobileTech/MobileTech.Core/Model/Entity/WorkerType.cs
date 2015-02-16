using System.Xml.Serialization;

namespace MobileTech.Models.Entity
{
    public class WorkerTypeInfo : SystemCodeInfo {
        [XmlElement(IsNullable = true)]
        public string UseRateTable { get; set; }

        [XmlElement(IsNullable = true)]
        public string WorkerType { get; set; }
    }
}
