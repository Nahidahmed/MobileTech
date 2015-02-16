using System;
using MobileTech.Core;
using MobileTech.Core.AuthService;
using Newtonsoft.Json;
using System.Net;
using System.IO;

using MobileTech.Models.Entity;


namespace MobileTechService
{
	public class DeviceLog
	{
		public string filename { get; set; }
		public byte[] data { get; set; }
	}

	public class AuthClient
	{
		private static AuthClient authClient;
		private bool noConnection;
		private bool emptyDB;
		public enum SessionStateType {
			SignedOff,
			SignedOnRemote,
			SignedOnLocal,
		}
		public SessionStateType SessionState { get;  set; }
		public User CurrentUser { get; private set; }

		public string CurrentSession{get;private set;}
		protected AuthenticationService authenticationService;

		public AuthClient ()
		{
		}

		public static AuthClient GetInstance ()
		{
			return authClient ?? (authClient = new AuthClient());
		}

		protected AuthenticationService AuthenticationService {
			get {
				if (authenticationService == null) {
					authenticationService = new AuthenticationService{
						Url =  MobileTech.Consts.ServerURL + "/AuthenticationService.asmx"
					};
				}

				authenticationService.SessionID = CurrentSession;
				return authenticationService;
			}
		}


		private async System.Threading.Tasks.Task<bool> UrlExists(string url)
		{
			var client = new System.Net.Http.HttpClient();
			var httpRequestMsg = new System.Net.Http.HttpRequestMessage(System.Net.Http.HttpMethod.Head, url);
			var response = await client.SendAsync(httpRequestMsg);
			return response.IsSuccessStatusCode;
		}


		public bool SignOnRemote (string username, string password, ref string error, string app_AppVersion, string app_wsVersion)
		{
			bool result = false;

			try
			{
				//var request = HttpWebRequest.Create(string.Format(@"http://192.168.1.19/mTechServices/AuthenticationService/SignOn?username=iaeuserdm&password=123&appname=iae"));
				var request = HttpWebRequest.Create(string.Format(@"{0}/AuthenticationService/SignOnWithVersion?username={1}&password={2}&appname=mTech.iOS&app_AppVersion={3}&app_WSVersion={4}",MobileTech.Consts.ServerURL,username,password,app_AppVersion,app_wsVersion));
//				var request = HttpWebRequest.Create(string.Format(@"{0}/AuthenticationService/SignOn?username={1}&password={2}&appname=mTech.iOS",MobileTech.Consts.ServerURL,username,password));

//				var request = HttpWebRequest.Create(string.Format(@"http://192.168.1.19/mTechServices/api/Security/Users"));
					request.ContentType = "application/json";
					request.Method = "GET";

					using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
					{
						if (response.StatusCode != HttpStatusCode.OK)
							Console.Out.WriteLine("Error fetching data. Server returned status code: {0}", response.StatusCode);
						using (StreamReader reader = new StreamReader(response.GetResponseStream()))
						{
							var content = reader.ReadToEnd();
							if(string.IsNullOrWhiteSpace(content)) {
								//Console.Out.WriteLine("Response contained empty body...");
							}
							else {
								string responseData = (string)JsonConvert.DeserializeObject(content , typeof(string));
								error = responseData;
								Console.Out.WriteLine("Response Body: \r\n {0}", content);
							}

//						string sessionID = (string)JsonConvert.DeserializeObject(content , typeof(string));
//						CurrentSession = sessionID;
						}
					}

				//string sessionID = AuthenticationService.SignOn(username.Trim(),password,"iNeedIt.iOS", app_AppVersion, app_wsVersion);
				//CurrentSession = sessionID;
				result = true;
			}
			catch(Exception ex)
			{
				error = ex.Message;
				MobileTech.Consts.LogException (ex);
			}
			return result;
		}

		public string SignOn(string username, string password, ref string errorMessage, string app_AppVersion, string app_wsVersion) {
			bool result;
			errorMessage = string.Empty;

			//TODO:28th Always returns true except network not reachable
//			if (!(result = SignOnRemote(username, password, ref errorMessage, app_AppVersion, app_wsVersion))) {
			if ((result = SignOnRemote(username, password, ref errorMessage, app_AppVersion, app_wsVersion))) {
				Console.WriteLine ("Authentication Error: " + errorMessage);
				noConnection = true;

				if (errorMessage.Contains ("User doesn't have MTech Access")) {
					return "InvalidMTechUser";
				} else if (errorMessage.Contains ("Invalid username/password")) {
					return "InvalidLoginCredentials";
				} else if (errorMessage.Contains ("User does not have MTech access")) {
					return "InvalidMTechUser";
				} else if (errorMessage.Contains ("User is archived")) {
					return "UserIsArchived";
				} else if (errorMessage.Contains ("Sync Params Not Set")) {
					return "SyncParamsNotSet";
				} else if (errorMessage.Contains ("Cannot open database")) {
					return "ServerDBNotAccessbile";
				} else if (errorMessage.Contains ("Login failed for user")) {
					return "ServerDBNotAccessbile";
				} else if (errorMessage.Contains ("A network-related or instance-specific error")) {
					return "A network-related or instance-specific error";
				} else if (errorMessage.Contains ("Not supported Content")) {
					return "Not supported Content";
				} else if (errorMessage.Contains ("This application is not compatible with the specified Web Service")) {
					return "This application is not compatible with the specified Web Service";
				}

				//This Scenerio will come at, when connecting new App to Old Web Service
				else if (errorMessage.Contains ("Server did not recognize the value of HTTP Header SOAPAction")) {
					return "This application is not compatible with the specified Web Service";
				} else if (errorMessage.Contains ("This User is mapped to a Windows User.")) {
					return "InValidWindowsLoginUser";
				} 
				else {
					CurrentSession = errorMessage;
					noConnection = false;
					SessionState = SessionStateType.SignedOnRemote;
					MobileTech.Consts.InvalidUserCredentials = false;
				}
				//If the remote signon fails then test against the local logon.
				//TODO:28th Remarks:Implemeneted below
					/*if (!(result = LocalSignOn (username, password, ref errorMessage))) {
						SessionState = SessionStateType.SignedOff;
					}*/

			} else {
				noConnection = true;

				if (!(result = LocalSignOn(username, password, ref errorMessage))) {
					SessionState = SessionStateType.SignedOff;
				}
				//return "A network-related or instance-specific error";
//				Error: ConnectFailure (Network is unreachable)
//				CurrentSession = errorMessage;
//				noConnection = false;
//				SessionState = SessionStateType.SignedOnRemote;
			}

			if (noConnection && emptyDB) {
				return "NoConnection";
			}

			if(!MobileTech.Consts.InvalidUserCredentials)
			{
				if (noConnection && emptyDB == false) {
					return "NoConnectionButDBIsNotEmpty";
				}
			}
			else
				return "InvalidLoginCredentials";

			if (result) {
				return "true";
			}

			return "false";
		}

		private bool LocalSignOn(string username, string password, ref string errorMessage) {
			bool localSignonStatus = false;
			errorMessage = "Invalid local User Name/Password";

			string authUserStatus = AuthenticationDAL.AuthenticateUser(username, password);

			if (authUserStatus == "EmtypDB")
			{
				localSignonStatus = true;
				emptyDB = true;
			}
			else
			{
				emptyDB = false;
			}

			if (authUserStatus == "true") {
				localSignonStatus = true;
			}

			MobileTech.Consts.InvalidUserCredentials = authUserStatus == "false";

			//else
			//{
			//    throw new Exception("Invalid username/password");
			//}
			// TODO: Authenticate against local database       
			//localSignonStatus == result of local signon status from the existing business logic. 
			if (localSignonStatus) {
				SessionState = SessionStateType.SignedOnLocal;
				CurrentUser = new User {Username = username, Password = password};
			}

			// TODO: Authenticate against local database

			return localSignonStatus;
		}

		public void SignOff(string username, string bundleId, string tokenId)
		{
			try{
				//AuthenticationService.SignOff();

				var request = HttpWebRequest.Create(string.Format(@"{0}/AuthenticationService/SignOff",MobileTech.Consts.ServerURL));

				request.ContentType = "application/json";
				request.Method = "GET";

				request.Headers.Set("SessionID", AuthClient.GetInstance().CurrentSession);
				using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
				{
					if (response.StatusCode != HttpStatusCode.OK)
						Console.Out.WriteLine("Error fetching data. Server returned status code: {0}", response.StatusCode);
					using (StreamReader reader = new StreamReader(response.GetResponseStream()))
					{
						var content = reader.ReadToEnd();
						if(string.IsNullOrWhiteSpace(content)) {
							Console.Out.WriteLine("Response contained empty body...");
						}
						else {
							//string responseData = (string)JsonConvert.DeserializeObject(content , typeof(string));
							Console.Out.WriteLine("Response Body: \r\n {0}", content);
						}
					}
				}
			}catch (Exception e){
				MobileTech.Consts.LogException (e);
			}
			MobileTech.Consts.signonStatus = "false";
			CurrentSession = "";
			SessionState = SessionStateType.SignedOff;
		}

		public bool IsValidSession()
		{
			bool blnIsValid = false;
			try{
				//TODO:28th
				//User u = AuthenticationService.GetCurrentUser("iNeedIt.iOS");
				User u = new User();
				if(u !=  null)
					blnIsValid = true;
			}catch (Exception e) {
				MobileTech.Consts.LogException (e);
			}
			return blnIsValid;
		}

		public void UploadExceptionLogFromDevice(string filename, byte[] data) {

			try{
				//TODO:28th
				//AuthenticationService.UploadExceptionLogFromDevice(filename, data);

				var request = HttpWebRequest.Create(string.Format(@"{0}/AuthenticationService/UploadExceptionLogFromDevice",MobileTech.Consts.ServerURL));

				request.ContentType = "application/json; charset=utf-8";
				request.Method = "POST";

				DeviceLog devLog = new DeviceLog();
				devLog.filename=filename;
				devLog.data = data;

				string json = JsonConvert.SerializeObject(devLog, Formatting.Indented);

				using (var streamWriter = new StreamWriter(request.GetRequestStream()))
				{
					json = json.Replace("\r\n","");
					//json = json.Replace("\",", "\","   + "\"" +"\u002B");
					streamWriter.Write(json);
					streamWriter.Flush();
					streamWriter.Close();
				}

				request.GetResponse();

			}catch (Exception ex){
				MobileTech.Consts.LogException (ex);

				string strLogMessage = string.Format("{0} UploadExceptionLogFromDevice Exception: {1}", DateTime.Now, ex.Message);
				MobileTech.Consts.HandleExceptionLog(strLogMessage);
			}
		}

	}
}

