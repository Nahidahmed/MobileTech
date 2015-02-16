using System;
using System.Data;
using Mono.Data.Sqlite;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using System.Collections.Generic;
using MobileTech;
	
namespace MobileTech
{
	public class Security//v1.12 
	{
		public Security ()
		{
		}

		public static void insertSystemSetup (string UserName, string ServerURL)
		{
			string sqlConfigurationDelete = @"Delete from SystemSetup where [Key] = @SettingField";

			//Insert into SystemSetup table, two records to hold Login User and Server URL values
			string sqlConfigurationSelect = @"Insert Into SystemSetup(Key, Value) values (@Key, @Value)";
			
			SqliteConnection conn = null;
			
			try {
				conn = MobileTech.Consts.GetDBConnection();

				using (SqliteCommand sqlQry = conn.CreateCommand()) {
					sqlQry.CommandText = sqlConfigurationDelete;
					sqlQry.Parameters.Add ("@SettingField",DbType.String);


					sqlQry.Parameters ["@SettingField"].Value = "ServerURL";
					sqlQry.ExecuteNonQuery();
					sqlQry.Parameters ["@SettingField"].Value = "UserName";
					sqlQry.ExecuteNonQuery();

					sqlQry.CommandText = sqlConfigurationSelect;
					sqlQry.Parameters.Add(new SqliteParameter { ParameterName = "@Key", DbType = DbType.String, Value = "UserName" });
					sqlQry.Parameters.Add(new SqliteParameter { ParameterName = "@Value", DbType = DbType.String, Value = UserName });
					sqlQry.ExecuteNonQuery();


					sqlQry.Parameters ["@Key"].Value = "ServerURL";
					sqlQry.Parameters ["@Value"].Value = ServerURL;
					sqlQry.ExecuteNonQuery();
				}
				
			} catch (Exception ex) {
				string strLogMessage = string.Format("{0} insertSystemSetup Exception: {1}", DateTime.Now, ex.Message);
				MobileTech.Consts.HandleExceptionLog(strLogMessage);

//				UIAlertView loginFailAlert = new UIAlertView ();
//				loginFailAlert.Message = "InsertLoginDetails err " + ex.GetBaseException ();
//				loginFailAlert.AddButton ("Ok"); 
//				loginFailAlert.Show ();
			} finally {
				if(conn != null)
					MobileTech.Consts.ReleaseDBConnection(conn);
			}
		}

		public static void getSystemSetup()
		{
			string sqlConfigurationSelect = @"Select [Key], [Value] From SystemSetup";
			SqliteConnection conn = null;

			try {
				conn = MobileTech.Consts.GetDBConnection();
				using (SqliteCommand sqlQry = conn.CreateCommand()) {
					
					sqlQry.CommandText = sqlConfigurationSelect;

					using (SqliteDataReader rReader = sqlQry.ExecuteReader()) {
						while (rReader.Read()) {

							//M.D.Prasad/28th Jan 2015/we are fetching username and serverURL from Root.plist (NSUserDefaults)
							//To Update the App build without uninstalling previous one.
//							if(Convert.ToString(rReader.GetValue (rReader.GetOrdinal ("Key"))) == "UserName")
//							{
//								MobileTech.Consts.LoginUserName = Convert.ToString(rReader.GetValue (rReader.GetOrdinal ("Value"))).Trim();
//							}
//
//							if(Convert.ToString(rReader.GetValue (rReader.GetOrdinal ("Key"))) == "ServerURL")
//							{
//								MobileTech.Consts.ServerURL = Convert.ToString(rReader.GetValue (rReader.GetOrdinal ("Value")));
//							}
							if(Convert.ToString(rReader.GetValue (rReader.GetOrdinal ("Key"))) == "DB_Version")
							{
								MobileTech.Consts.db_Version = Convert.ToString(rReader.GetValue (rReader.GetOrdinal ("Value")));
							}
							if(Convert.ToString(rReader.GetValue (rReader.GetOrdinal ("Key"))) == "WS_Version")
							{
								MobileTech.Consts.ws_Version = Convert.ToString(rReader.GetValue (rReader.GetOrdinal ("Value")));
							}
						}
					}
				}
				
			} catch (Exception ex) {
				string strLogMessage = string.Format("{0} getSystemSetup Exception: {1}", DateTime.Now, ex.Message);
				MobileTech.Consts.HandleExceptionLog(strLogMessage);

				UIAlertView loginFailAlert = new UIAlertView ();
				loginFailAlert.Message = "getSystemSetup err " + ex.GetBaseException ();
				loginFailAlert.AddButton ("Ok"); 
				loginFailAlert.Show ();
			} finally {
				if(conn != null)
					MobileTech.Consts.ReleaseDBConnection(conn);
			}
		}


		public static string getSystemSettingForKey(string strSettingField)
		{
			string sqlSelectQuery = @"Select [Key], [Value] From SystemSetup where [Key]=@SettingField";
			SqliteConnection conn = null;
			string strDateFormat=string.Empty;

			try {
				conn = MobileTech.Consts.GetDBConnection();
				using (SqliteCommand sqlQry = conn.CreateCommand()) {
					
					sqlQry.CommandText = sqlSelectQuery;
					sqlQry.Parameters.Add ("@SettingField",DbType.String);

					sqlQry.Parameters ["@SettingField"].Value = strSettingField; 

					using (SqliteDataReader rReader = sqlQry.ExecuteReader()) {
						if (rReader.Read()) {
							//Convert.ToString(rReader.GetValue (rReader.GetOrdinal ("Key")));
							strDateFormat = Convert.ToString(rReader.GetValue (rReader.GetOrdinal ("Value")));
						}
					}
				}
				
			} catch (Exception ex) {
				string strLogMessage = string.Format("{0} getSystemSettingForKey Exception: {1}", DateTime.Now, ex.Message);
				MobileTech.Consts.HandleExceptionLog(strLogMessage);

				UIAlertView loginFailAlert = new UIAlertView ();
				loginFailAlert.Message = "getSystemSettingForKey err " + ex.GetBaseException ();
				loginFailAlert.AddButton ("Ok"); 
				loginFailAlert.Show ();
			} finally {
				if(conn != null)
					MobileTech.Consts.ReleaseDBConnection(conn);
			}
			return strDateFormat;
		}
	}
}

