using System;
using System.IO;
using System.Data;
using System.Collections;
using System.Linq;
using Mono.Data.Sqlite;
using System.Reflection;
using System.Net;
using System.Collections.Generic;
using MonoTouch.Foundation;
using MobileTech.Models.Entity;
using Mono.CSharp;
using MonoTouch.SystemConfiguration;
using MonoTouch.CoreFoundation;

namespace MobileTech
{
	public class Consts
	{
		//static System.Threading.Mutex mut =new System.Threading.Mutex(false);
		private static SqliteConnection glb_conn=null;

		static string documents = Environment.GetFolderPath (Environment.SpecialFolder.MyDocuments);
		static string connectionString = "URI=file:"+Path.Combine(documents,"mTech.sqlite");
		public static string signonStatus = string.Empty;
		public static string LoginUserName = string.Empty;
		public static long WKRPrimaryId=0;
		public static string LoginUserPwd = string.Empty;
		public static string ServerCode = string.Empty;
		public static string ServerURL = string.Empty;
		public static readonly string defaultDateFormat = "MM/dd/yyyy";
		public static bool InvalidUserCredentials;
		public static string db_Version = string.Empty;
		public static string ws_Version = string.Empty;
		public enum enumLogHandle { None = 0, Create = 1, Manipulate = 2, Delete = 3 };
		public static string ExceptionLogFilePath = string.Empty;
		public enum enumUserExceptionType { None = 0, SignonStatus = 1, SignedOnRemote=2, DownloadFailed =3, FireAlert=4, LocalLogin=5};
		public enum enumRequestsAction {AEWOs = 0, Accept = 12, Decline=11, Delay=33, Finish=32, Start=31, Resume=34, LogCompletion=27};
		//public static WorkOrderTime woTime;
		public static bool isSyncRequired = false;
		public static string mobileConnectURL = @"http://maul.stxlive.com/MC/MobileConnectService.asmx";

		public static string app_AppVersion="5.5.115.0";
		public static string app_wsVersion = "5.5.112.0";

		public static string selectedWORow = string.Empty;

		public static bool isVisibleBtnUploadLogToServer=false;

		private static string dbPath = Path.Combine(documents,"mTech.sqlite");

		public static string DBPath { 
			get {
				return dbPath;
			}
		}

		public static string AppVersionWithBuild {
			get;
			set;
		}

		public static string DeviceId {
			get {
				var deviceid = MonoTouch.UIKit.UIDevice.CurrentDevice.IdentifierForVendor.AsString();
				return deviceid;
			}
		}

		public Consts ()
		{
		}

		public static void SetupDatabase(bool force)
		{
			string personalFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			string dbName = "mTech.sqlite";
			string dbPath = Path.Combine (personalFolder, dbName);

			if (force || !File.Exists (dbPath)) {
				File.Copy(dbName, dbPath, true);

//				var user = NSUserDefaults.StandardUserDefaults;
//				user.SetString(string.Empty,"txtUserName");
//				user.SetString(string.Empty,"txtPassWord");
//				user.SetString(string.Empty,"txtServerURL");
//				user.SetString(string.Empty, "ServerUrl");
			}

			dbName = "WsMsgConnectorQueue.sqlite";
			dbPath = Path.Combine (personalFolder, dbName);

			if (force || !File.Exists (dbPath)) {
				File.Copy(dbName, dbPath, true);
			}
		}

		public static SqliteConnection GetDBConnection()
		{
			//mut.WaitOne();
			if(glb_conn ==null){
				glb_conn=new SqliteConnection(connectionString);
				glb_conn.Open();
			}

			if(glb_conn.State!=System.Data.ConnectionState.Open)
				glb_conn.Open();

			return glb_conn;
		}

		public static void ReleaseDBConnection(SqliteConnection sqlConn)
		{
			//glb_conn.Close ();
			//mut.ReleaseMutex();
		}

		public static bool IsUrlReachable(Uri url,string expectedText)
		{

			bool isUrlReachable = false;
			WebRequest httpRequest;
			WebResponse httpResponse = null;
			Stream responseStream = null;
			StreamReader responseReader = null;
			for (int connectTrycount = 0; !isUrlReachable && connectTrycount < 5; connectTrycount++)
			{
				try
				{
					httpRequest = WebRequest.Create(url);
					httpRequest.Method = "GET";
					httpRequest.Timeout = 10000;
					httpResponse = httpRequest.GetResponse();
					responseStream = httpResponse.GetResponseStream();
					responseReader = new StreamReader(responseStream);
					string responseText = responseReader.ReadToEnd();

					isUrlReachable = responseText.IndexOf(expectedText) >= 0;
				}
				catch (Exception ex) {
					MobileTech.Consts.LogException (ex);

					//Console.WriteLine("Url:" + url);
					Console.WriteLine(ex.Message);
					break;
				}finally{
					if (responseReader != null){
						responseReader.Close();
					}if (responseStream != null){
						responseStream.Close();
					}if (httpResponse != null){
						httpResponse.Close();
					}
				}
			}
			return isUrlReachable;
		}

		public static bool IsWebApiUrlReachable(Uri url, string expectedText)
		{
			bool isUrlReachable = false;
			WebRequest httpRequest;
			WebResponse httpResponse = null;
			Stream responseStream = null;
			StreamReader responseReader = null;

			for (int connectTrycount = 0; !isUrlReachable && connectTrycount < 5; connectTrycount++)
			{
				try
				{
					var request = HttpWebRequest.Create(url.ToString());

					// HttpWebRequest.Create(string.Format(@"http://192.168.1.19/mTechServices/api/Security/Users"));
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
								isUrlReachable = true;
								Console.Out.WriteLine("Response contained empty body...");
							}
							else {
								Console.Out.WriteLine("Response Body: \r\n {0}", content);
								isUrlReachable = false;
							}

						}
					}
				}
				catch (Exception ex) {
					//MobileTech.Consts.LogException (ex);
					string strLogMessage = string.Format("{0} Connection failure or Web Service not reachable.: {1}", DateTime.Now, ex.Message);
					MobileTech.Consts.HandleExceptionLog(strLogMessage);

					//Console.WriteLine("Url:" + url);
					Console.WriteLine(ex.Message);
					break;
				}finally{
					if (responseReader != null){
						responseReader.Close();
					}if (responseStream != null){
						responseStream.Close();
					}if (httpResponse != null){
						httpResponse.Close();
					}
				}
			}
			return isUrlReachable;
		}


		/*public static void LogEntry(string message, bool logToFile)
		{
			string documents = Environment.GetFolderPath (Environment.SpecialFolder.MyDocuments);

			//Nahid: If more than 14 debug files then delete all.
			int fCount = Directory.GetFiles(documents, "*.txt", SearchOption.TopDirectoryOnly).Length;
			if (fCount > 14) {
				var files = Directory.GetFiles(documents, "*.txt", SearchOption.TopDirectoryOnly).Where(s => s.EndsWith(".txt") );

				foreach (string file in files)
				{
					File.Delete(file);
				}
			}

			string filename = Path.Combine (documents, DateTime.Now.ToString("yyyy-MM-dd")+"_"+AuthClient.GetInstance().CurrentSession+".txt");

			string output = System.DateTime.Now.ToString ("[yyyy-MM-dd HH:mm:ss.fff] ") + message;
			System.Diagnostics.Debug.WriteLine (output);
			if (logToFile) {
				System.IO.File.AppendAllText (filename, output + "\r\n");
			}
		}*/

		public static bool IsDBEmpty()
		{
			//			bool isDBEmpty = false;
			int count = 0;
			string sqlConfigurationSelect = @"Select count(secprimaryid) as count from Security";

			SqliteConnection conn = null;

			try {
				conn = MobileTech.Consts.GetDBConnection();
				using (SqliteCommand sqlQry = conn.CreateCommand()) {
					sqlQry.CommandText = sqlConfigurationSelect;
					using (SqliteDataReader rReader = sqlQry.ExecuteReader()) {
						while (rReader.Read()) {
							count = Convert.ToInt16(rReader.GetValue (rReader.GetOrdinal ("count")));
						}
					}
				}

			} catch (Exception ex) {
				MobileTech.Consts.LogException (ex);

				//Author/Date/Issue - M.D.Prasad/14th May 2014/ISS - 5395 or CRR - 1338  Upload iGotIt device logs
				//string strLogMessage = string.Format("{0} IsDBEmpty Exception: {1}", DateTime.Now, ex.Message);
				//iGotItService.Consts.HandleExceptionLog(strLogMessage);
				//
				//				UIAlertView loginFailAlert = new UIAlertView ();
				//				loginFailAlert.Message = "IsDBEmpty err " + ex.GetBaseException ();
				//				loginFailAlert.AddButton ("Ok"); 
				//				loginFailAlert.Show ();
			} finally {
				if(conn != null)
					MobileTech.Consts.ReleaseDBConnection(conn);
			}
			return count > 0 ? false : true;
		}

		public static void DeletePreviousUsersData()
		{
			string sqlDelStat = @"Delete from ";
			string[] tables = {"Account","Security","ConfigurationSync","Codes_Urgency","Codes_Request","Codes_OpenWorkStatus","Codes_ControlCenter",
								"Equipment","WkOrders", "Codes_Result", "Codes_ResultCenter", "Codes_Fault", "Codes_FaultCenter", 
								"Codes_RequestCenter", "Codes_ValidRequestsResults", "WOText", "Codes_WorkOrderTimeType", "PDADBID", "Workers", 
								"AccountFacility"};
			SqliteConnection conn = null;

			try {
				conn = MobileTech.Consts.GetDBConnection();

				using (SqliteCommand sqlQry = conn.CreateCommand()) {
					foreach (string  table in tables) {
						sqlQry.CommandText = sqlDelStat + table;
						sqlQry.ExecuteNonQuery();	
					}
				}

			} catch (Exception ex) {
				MobileTech.Consts.LogException (ex);


			} finally {
				if(conn != null)
					MobileTech.Consts.ReleaseDBConnection(conn);
			}
		}

		public static void DeletePreviousUsersDataForSpecificTables(string[] tables)
		{
			string sqlDelStat = @"Delete from ";

			SqliteConnection conn = null;
			string[] configTable = { "ConfigurationSync" };

			try {
				conn = MobileTech.Consts.GetDBConnection();

				using (SqliteCommand sqlQry = conn.CreateCommand()) {
					foreach (string  table in tables) {
						if(table == "ConfigurationSync")
							sqlQry.CommandText = sqlDelStat + table + " Where Block in ('"+string.Join("','", tables.Except(configTable))+"')";
						else
							sqlQry.CommandText = sqlDelStat + table;
						sqlQry.ExecuteNonQuery();	
					}
				}

			} catch (Exception ex) {
				MobileTech.Consts.LogException (ex);


			} finally {
				if(conn != null)
					MobileTech.Consts.ReleaseDBConnection(conn);
			}
		}

		public static void DeletePreviousUsersMsgQueue()
		{
			string sqlDelStat = @"Delete from WsMsgConnectorQueue";
			SqliteConnection conn = null;
			string documents = Environment.GetFolderPath (Environment.SpecialFolder.MyDocuments);
			string CONNECTION_STRING = "URI=file:"+Path.Combine(documents,"WsMsgConnectorQueue.sqlite");

			try {
				conn = new SqliteConnection(CONNECTION_STRING);
				conn.Open();
				using (SqliteCommand sqlQry = conn.CreateCommand()) {
					sqlQry.CommandText = sqlDelStat;
					sqlQry.ExecuteNonQuery();	
				}

			} catch (Exception ex) {
				MobileTech.Consts.LogException (ex);

			} finally {
				if(conn != null)
					conn.Close();
				conn.Dispose();
			}
		}

		public static void HandleExceptionLog(enumLogHandle objEnumLogHandle)
		{
			var documents = Environment.GetFolderPath (Environment.SpecialFolder.MyDocuments);
			var directoryname = Path.Combine (documents, "Logs");
			string filename = string.Format ("MobileTechApp_device_{0}.log", LoginUserName);

			var filepath = Path.Combine (directoryname, filename);
			//if(string.IsNullOrEmpty(ExceptionLogFilePath))
			ExceptionLogFilePath = filepath;
			try {
				switch (objEnumLogHandle) {
				case enumLogHandle.Create:
					//earlier we used this. 
					/*
					if (!Directory.Exists (directoryname))
						Directory.CreateDirectory (directoryname);

					if (!File.Exists (ExceptionLogFilePath))
						File.Create (ExceptionLogFilePath);
						*/
					break;

				case enumLogHandle.Manipulate:
					if (File.Exists (ExceptionLogFilePath)) {
						long flSizeInBytes = new System.IO.FileInfo (ExceptionLogFilePath).Length;
						double flSizeInMegaBytes = (flSizeInBytes / 1024f) / 1024f;

						if (flSizeInMegaBytes > 3.1) { //when file size is more than 3 mb, then delete the content and create new file.
							File.Delete (ExceptionLogFilePath);
							File.Create (ExceptionLogFilePath);
						}
					}
					break;

				case enumLogHandle.Delete:
					if (Directory.Exists (directoryname))
					{
						Array.ForEach(Directory.GetFiles(directoryname),
							delegate(string path) { File.Delete(path); });
					}
					break;

				case enumLogHandle.None:
				default:
					if (!Directory.Exists (directoryname))
						Directory.CreateDirectory (directoryname);

					//					if (!File.Exists (ExceptionLogFilePath))
					//						ExceptionLogFilePath = string.Empty;
					break;
				}
			} catch (Exception ex) {
				MobileTech.Consts.LogException (ex);

				string strLogMessage = string.Format("{0} HandleExceptionLog Exception: {1}", DateTime.Now, ex.Message);
				HandleExceptionLog(strLogMessage);
			}
		}

		public static void HandleExceptionLog(string message)
		{
			try {
				HandleExceptionLog (enumLogHandle.None);
				if(!string.IsNullOrEmpty(ExceptionLogFilePath)){
					using (FileStream fs = new FileStream (ExceptionLogFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite)) {
						using (StreamWriter sw = new StreamWriter (fs)) {
							sw.BaseStream.Seek (0, SeekOrigin.End);
							sw.Write (message + "\r\n");
						}
					}
				}
			}
			catch (Exception e)
			{
			}
		}

		public static void LogException(Exception e)
		{
			try
			{
				HandleExceptionLog (enumLogHandle.None);
				if(!string.IsNullOrEmpty(ExceptionLogFilePath)){
					using (FileStream fs = new FileStream (ExceptionLogFilePath, FileMode.OpenOrCreate, FileAccess.Write)) {
						using (StreamWriter sw = new StreamWriter (fs)) {
							sw.BaseStream.Seek (0, SeekOrigin.End);
							sw.Write (DateTime.Now + " " + e.ToString () + "\r\n");
						}
					}
				}
			} catch (Exception ex) {
			}
		}

		public static string LastActionDateTime
		{
			get { 
				string value = NSUserDefaults.StandardUserDefaults.StringForKey("Key"); 
				if (value == null)
					return DateTime.MinValue.ToString();
				else
					return value;
			}
			set {
				NSUserDefaults.StandardUserDefaults.SetString(value.ToString (), "Key"); 
				NSUserDefaults.StandardUserDefaults.Synchronize ();
			}
		}

		public enum NetworkStatus
		{
			NotReachable,
			ReachableViaCarrierDataNetwork,
			ReachableViaWiFiNetwork
		}

		public static class Reachability
		{
			public static string HostName = "www.google.com";

			public static bool IsReachableWithoutRequiringConnection(NetworkReachabilityFlags flags)
			{
				// Is it reachable with the current network configuration?
				bool isReachable = (flags & NetworkReachabilityFlags.Reachable) != 0;

				// Do we need a connection to reach it?
				bool noConnectionRequired = (flags & NetworkReachabilityFlags.ConnectionRequired) == 0;

				// Since the network stack will automatically try to get the WAN up,
				// probe that
				if ((flags & NetworkReachabilityFlags.IsWWAN) != 0)
					noConnectionRequired = true;

				return isReachable && noConnectionRequired;
			}

			// Is the host reachable with the current network configuration
			public static bool IsHostReachable(string host)
			{
				if (string.IsNullOrEmpty(host))
					return false;

				using (var r = new NetworkReachability(host))
				{
					NetworkReachabilityFlags flags;

					if (r.TryGetFlags(out flags))
					{
						return IsReachableWithoutRequiringConnection(flags);
					}
				}
				return false;
			}

			//
			// Raised every time there is an interesting reachable event,
			// we do not even pass the info as to what changed, and
			// we lump all three status we probe into one
			//
			public static event EventHandler ReachabilityChanged;

			static void OnChange(NetworkReachabilityFlags flags)
			{
				var h = ReachabilityChanged;
				if (h != null)
					h(null, EventArgs.Empty);
			}

			//
			// Returns true if it is possible to reach the AdHoc WiFi network
			// and optionally provides extra network reachability flags as the
			// out parameter
			//
			static NetworkReachability adHocWiFiNetworkReachability;

			public static bool IsAdHocWiFiNetworkAvailable(out NetworkReachabilityFlags flags)
			{
				if (adHocWiFiNetworkReachability == null)
				{
					adHocWiFiNetworkReachability = new NetworkReachability(new IPAddress(new byte [] { 169, 254, 0, 0 }));
					adHocWiFiNetworkReachability.SetNotification(OnChange);
					adHocWiFiNetworkReachability.Schedule(CFRunLoop.Current, CFRunLoop.ModeDefault);
				}

				return adHocWiFiNetworkReachability.TryGetFlags(out flags) && IsReachableWithoutRequiringConnection(flags);
			}

			static NetworkReachability defaultRouteReachability;

			static bool IsNetworkAvailable(out NetworkReachabilityFlags flags)
			{
				if (defaultRouteReachability == null)
				{
					defaultRouteReachability = new NetworkReachability(new IPAddress(0));
					defaultRouteReachability.SetNotification(OnChange);
					defaultRouteReachability.Schedule(CFRunLoop.Current, CFRunLoop.ModeDefault);
				}
				return defaultRouteReachability.TryGetFlags(out flags) && IsReachableWithoutRequiringConnection(flags);
			}

			static NetworkReachability remoteHostReachability;

			public static NetworkStatus RemoteHostStatus()
			{
				NetworkReachabilityFlags flags;
				bool reachable;

				if (remoteHostReachability == null)
				{
					remoteHostReachability = new NetworkReachability(HostName);

					// Need to probe before we queue, or we wont get any meaningful values
					// this only happens when you create NetworkReachability from a hostname
					reachable = remoteHostReachability.TryGetFlags(out flags);

					remoteHostReachability.SetNotification(OnChange);
					remoteHostReachability.Schedule(CFRunLoop.Current, CFRunLoop.ModeDefault);
				}
				else
					reachable = remoteHostReachability.TryGetFlags(out flags);

				if (!reachable)
					return NetworkStatus.NotReachable;

				if (!IsReachableWithoutRequiringConnection(flags))
					return NetworkStatus.NotReachable;

				if ((flags & NetworkReachabilityFlags.IsWWAN) != 0)
					return NetworkStatus.ReachableViaCarrierDataNetwork;

				return NetworkStatus.ReachableViaWiFiNetwork;

			}

			public static NetworkStatus InternetConnectionStatus()
			{

				NetworkReachabilityFlags flags;
				bool defaultNetworkAvailable = IsNetworkAvailable (out flags);
				if (defaultNetworkAvailable) {
					if (flags.HasFlag (NetworkReachabilityFlags.IsWWAN)) {
						return NetworkStatus.ReachableViaCarrierDataNetwork;
					} else {
						return NetworkStatus.ReachableViaWiFiNetwork;
					}
				} else {
					return NetworkStatus.NotReachable;
				}
			}

			public static NetworkStatus LocalWifiConnectionStatus()
			{
				NetworkReachabilityFlags flags;
				if (IsAdHocWiFiNetworkAvailable(out flags))
				{
					if ((flags & NetworkReachabilityFlags.IsDirect) != 0)
						return NetworkStatus.ReachableViaWiFiNetwork;
				}
				return NetworkStatus.NotReachable;
			}
		}
	}
}

