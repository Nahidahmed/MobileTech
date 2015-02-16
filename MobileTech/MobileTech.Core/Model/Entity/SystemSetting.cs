using System;
using System.Xml.Serialization;

namespace MobileTech.Models.Entity
{
	public class SystemSetting
	{
		[XmlElement]
		public string SettingField { get; set; }

		[XmlElement]
		public string SettingValue { get; set; }
	}
}

