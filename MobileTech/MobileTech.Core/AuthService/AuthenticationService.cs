using System;
using System.Web.Services;
using System.Diagnostics;
using System.Web.Services.Protocols;
using System.Xml.Serialization;
using System.ComponentModel;

namespace MobileTech.Core.AuthService
{
	public partial class AuthenticationService : System.Web.Services.Protocols.SoapHttpClientProtocol
	{
		public string SessionID { get; set; }

		protected override System.Net.WebRequest GetWebRequest(System.Uri uri)
		{
			System.Net.HttpWebRequest request = (System.Net.HttpWebRequest)base.GetWebRequest(uri);
			if (!string.IsNullOrEmpty(SessionID))
			{
				request.Headers["sessionid"] = SessionID;
			}
			return request;
		}
	}
}

