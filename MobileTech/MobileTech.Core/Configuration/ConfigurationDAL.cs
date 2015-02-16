using System;
using System.Data;
using System.Data.SqlTypes;
using System.Data.SqlClient;
using Mono.Data.Sqlite;
using MonoTouch.UIKit;
using System.Collections.Generic;
using System.Linq;

using MobileTech.Core;
using MobileTech.Models.Entity;
//using MobileTech.Core.ConfigService;

namespace MobileTechService
{
	public class ConfigurationDAL
	{
		public ConfigurationDAL ()
		{
		}
		private const string sqlConfigurationDelete = "Delete from ConfigurationSync where block = '{0}'";
		//private const string sqlConfigurationInsert = "Insert into ConfigurationSync (Block,LastSyncDate) values ('{0}','{1}')";
		private const string sqlConfigurationInsert = "Insert into ConfigurationSync (Block,LastSyncDate) values (@Block,@LastSyncDate)";
		
		public static DateTime GetLastSyncDateByblock (string block,ref string errorMsg)
		{
			errorMsg = string.Empty;
			DateTime dt = (DateTime)SqlDateTime.MinValue;

			//string sqlConfigurationSelect = @"Select DateTime(LastSyncDate) as LastSyncDate From ConfigurationSync where Block = '{0}' Order by LastSyncDate desc";
			string sqlConfigurationSelect = @"Select LastSyncDate From ConfigurationSync where Block = '{0}' Order by LastSyncDate desc";
			
			SqliteConnection conn = null;
			
			try {
				conn = MobileTech.Consts.GetDBConnection ();
				using (SqliteCommand sqlQry = conn.CreateCommand()) {
					sqlQry.CommandText = string.Format (sqlConfigurationSelect, block);	//sqlQry.CommandText = sqlConfigurationSelect;
					using (SqliteDataReader rReader = sqlQry.ExecuteReader()) {
						while (rReader.Read()) {
							//string strLastSyndDate =  Convert.ToString(rReader.GetValue(rReader.GetOrdinal("LastSyncDate")));
							string strLastSyndDate =  Convert.ToDateTime(rReader.GetValue(rReader.GetOrdinal("LastSyncDate"))).ToString("yyyy-MM-dd HH:mm:ss.fff");
							dt = string.IsNullOrEmpty(strLastSyndDate)? (DateTime)SqlDateTime.MinValue : Convert.ToDateTime(strLastSyndDate);
							//dt = string.IsNullOrEmpty(strLastSyndDate)? (DateTime)SqlDateTime.MinValue : Convert.ToDateTime(rReader.GetValue(rReader.GetOrdinal("LastSyncDate")));
						}
					}
				}
				
			} catch (Exception ex) {
				string strLogMessage = string.Format("{0} GetLastSyncDateByblock Exception: {1}", DateTime.Now, ex.Message);
				MobileTech.Consts.HandleExceptionLog(strLogMessage);
				errorMsg = strLogMessage;
			} finally {
				if(conn != null)
					MobileTech.Consts.ReleaseDBConnection(conn);
			}
			return dt;
		}
		
		public static void DeleteItems (string[] delKeys, string sqlStatement)
		{
			if (delKeys == null || delKeys.Count () == 0) {
				return;
			}
			
			SqliteConnection conn = null;
			try {
				conn = MobileTech.Consts.GetDBConnection ();
				using (SqliteCommand sqlQry = conn.CreateCommand()) {
					List<string> keyList = new List<string> ();
					int total = 0;
					foreach (string key in delKeys) {
						keyList.Add (key);
						
						if (keyList.Count < 100 && total + keyList.Count < delKeys.Length) {
							continue;
						}
						
						sqlQry.CommandText = string.Format (sqlStatement, "'" + string.Join ("','", keyList.ToArray ()) + "','");
						sqlQry.CommandText = sqlQry.CommandText.Replace(",')",")");
						sqlQry.ExecuteNonQuery();
						
						total += keyList.Count;
						keyList.Clear ();
					}
				}
			} catch (Exception ex) {
				string strLogMessage = string.Format("{0} DeleteItems Exception: {1}", DateTime.Now, ex.Message);
				MobileTech.Consts.HandleExceptionLog(strLogMessage);
				//Console.WriteLine("DeleteItems: "+ex.Message);
			} finally {
				if(conn != null)
					MobileTech.Consts.ReleaseDBConnection(conn);
			}
		}

		public static void deleteSystemSettings (SystemSetting[] systemSettings)
		{
			string strKeys=string.Empty;

			SqliteConnection conn = null;
			try {
				conn = MobileTech.Consts.GetDBConnection();

				using (SqliteCommand sqlQry = conn.CreateCommand()) 
				{

					string sqlStatement = @"Delete from SystemSetup where [Key] = @SettingField";
					sqlQry.CommandText = sqlStatement;
					sqlQry.Parameters.Add ("@SettingField",DbType.String);

					foreach(SystemSetting setting in systemSettings)
					{
						sqlQry.Parameters ["@SettingField"].Value = setting.SettingField; 
						sqlQry.ExecuteNonQuery ();
					}
				}
			} catch (Exception ex) {
				string strLogMessage = string.Format("{0} deleteSystemSettings Exception: {1}", DateTime.Now, ex.Message);
				MobileTech.Consts.HandleExceptionLog(strLogMessage);
			} finally {
				if(conn != null)
					MobileTech.Consts.ReleaseDBConnection(conn);
			}
		}


		public static void DeleteSecurityItems(string[] DeleteKeys)
		{
			const string sqlStatement = @"Delete from Security where SecPrimaryid in ({0})";
			DeleteItems(DeleteKeys,sqlStatement);
		}


		public static void InsertSecurityItems (SecuritiesSync[] securities,ref string errorMsg)
		{
			errorMsg = string.Empty;
			const string sqlStatement = @"Insert Into Security (UserNm,Password,SecPrimaryid,CodeKey,UserID,WKRPrimaryId,RoleWosyst,WindowsUser) 
									values (@UserName,@password,@SecPrimaryid,@CodeKey,@UserID,@WKRPrimaryId,@RoleWosyst,@WindowsUser)";
			
			SqliteConnection conn = null;
			try {
				conn = MobileTech.Consts.GetDBConnection();
//				string connectionString  = "URI=file:iGotIt.sqlite";
//				conn = new SqliteConnection(connectionString);
//				conn.Open();
				using (SqliteCommand sqlQry = conn.CreateCommand()) {
					
					sqlQry.CommandText = sqlStatement;
					sqlQry.Parameters.Add ("@UserName",DbType.String);
					sqlQry.Parameters.Add ("@SecPrimaryid", DbType.UInt64);
					sqlQry.Parameters.Add ("@password", DbType.Binary);
					sqlQry.Parameters.Add ("@CodeKey",DbType.Int16);
					sqlQry.Parameters.Add ("@UserID", DbType.UInt32);
					sqlQry.Parameters.Add ("@WKRPrimaryId", DbType.UInt64);
					sqlQry.Parameters.Add ("@RoleWosyst",DbType.Int16);
					sqlQry.Parameters.Add ("@WindowsUser", DbType.String);

					foreach (SecuritiesSync security in securities) {
						sqlQry.Parameters ["@UserName"].Value = security.UserName; 
						sqlQry.Parameters ["@SecPrimaryid"].Value = security.SecPrimaryId;
						sqlQry.Parameters ["@password"].Value = security.Password;
						sqlQry.Parameters ["@CodeKey"].Value = security.CodeKey; 
						sqlQry.Parameters ["@UserID"].Value = security.UserId;
						sqlQry.Parameters ["@WKRPrimaryId"].Value = security.AssociatedWorker;
						sqlQry.Parameters ["@RoleWosyst"].Value = security.AccessToWosyst; 
						sqlQry.Parameters ["@WindowsUser"].Value = security.WindowsUser;
					
						sqlQry.ExecuteNonQuery ();
					}
					
				}
			} catch (Exception ex) {
				string strLogMessage = string.Format("{0} InsertSecurityItems Exception: {1}", DateTime.Now, ex.Message);
				MobileTech.Consts.HandleExceptionLog(strLogMessage);
				errorMsg = strLogMessage;
//				UIAlertView loginFailAlert = new UIAlertView();
//				loginFailAlert.Message = "err "+ ex.GetBaseException();
//				loginFailAlert.AddButton("Ok"); 
//				loginFailAlert.Show();
				
			} finally {
				if(conn != null)
					MobileTech.Consts.ReleaseDBConnection(conn);
			}
		}

		public static void DeleteUrgencyItems (string[] DeleteKeys)
		{
			const string sqlStatement = @"Delete from Codes_Urgency where UrgPrimaryId in ({0})";
			DeleteItems (DeleteKeys, sqlStatement);
		}

		public static void InsertUrgencyItems (UrgencyInfo[] urgencies,ref string errorMsg)
		{
			errorMsg = string.Empty;
			const string sqlStatement = @"Insert Into Codes_Urgency (URGPrimaryId, Code, Description) 
											values (@UrgPrimaryId, @Code, @Description)";

			SqliteConnection conn = null;
			try {
				conn = MobileTech.Consts.GetDBConnection ();
				using (SqliteCommand sqlQry = conn.CreateCommand ()) {

					sqlQry.CommandText = sqlStatement;
					sqlQry.Parameters.Add ("@UrgPrimaryId", DbType.UInt64);
					sqlQry.Parameters.Add ("@Code", DbType.UInt16);
					sqlQry.Parameters.Add ("@Description", DbType.String);

					foreach (UrgencyInfo urgency in urgencies) {
						sqlQry.Parameters ["@UrgPrimaryId"].Value = urgency.PrimaryId;
						sqlQry.Parameters ["@Code"].Value = urgency.Code;
						sqlQry.Parameters ["@Description"].Value = urgency.Description;

						sqlQry.ExecuteNonQuery ();
					}

				}
			} catch (Exception ex) {
				string strLogMessage = string.Format("{0} InsertUrgencyItems Exception: {1}", DateTime.Now, ex.Message);
				MobileTech.Consts.HandleExceptionLog (strLogMessage);
				errorMsg = strLogMessage;
//				UIAlertView loginFailAlert = new UIAlertView();
//				loginFailAlert.Message = "err "+ ex.GetBaseException();
//				loginFailAlert.AddButton("Ok"); 
//				loginFailAlert.Show();

			} finally {
				if (conn != null)
					MobileTech.Consts.ReleaseDBConnection (conn);
			}

		}

		public static void DeleteRequestItems (string[] DeleteKeys)
		{
			const string sqlStatement = @"Delete from Codes_Request where REQPrimaryId in ({0})";
			DeleteItems (DeleteKeys, sqlStatement);
		}

		public static void InsertRequestItems (RequestInfo[] requests,ref string errorMsg)
		{
			const string sqlStatement = @"Insert Into Codes_Request (REQPrimaryId, RequestCode, Description) 
											values (@REQPrimaryId, @RequestCode, @Description)";

			SqliteConnection conn = null;
			try {
				conn = MobileTech.Consts.GetDBConnection ();
				using (SqliteCommand sqlQry = conn.CreateCommand ()) {

					sqlQry.CommandText = sqlStatement;
					sqlQry.Parameters.Add ("@REQPrimaryId", DbType.UInt64);
					sqlQry.Parameters.Add ("@RequestCode", DbType.UInt16);
					sqlQry.Parameters.Add ("@Description", DbType.String);

					foreach (RequestInfo request in requests) {
						sqlQry.Parameters ["@REQPrimaryId"].Value = request.PrimaryId;
						sqlQry.Parameters ["@RequestCode"].Value = request.Code;
						sqlQry.Parameters ["@Description"].Value = request.Description;

						sqlQry.ExecuteNonQuery ();
					}

				}
			} catch (Exception ex) {
				string strLogMessage = string.Format("{0} InsertRequestItems Exception: {1}", DateTime.Now, ex.Message);
				MobileTech.Consts.HandleExceptionLog (strLogMessage);
				errorMsg = strLogMessage;
			} finally {
				if (conn != null)
					MobileTech.Consts.ReleaseDBConnection (conn);
			}
		}


		public static void DeleteOpenWorkStatusItems (string[] DeleteKeys)
		{
			const string sqlStatement = @"Delete from Codes_OpenWorkStatus where OWSPrimaryId in ({0})";
			DeleteItems (DeleteKeys, sqlStatement);
		}

		public static void InsertOpenWorkStatusItems (OpenWorkStatusInfo[] openWorkStatuses,ref string errorMsg)
		{
			const string sqlStatement = @"Insert Into Codes_OpenWorkStatus (OWSPrimaryId, OpenStatusCode, Description,RequestsActionCode) 
											values (@OWSPrimaryId, @OpenStatusCode, @Description,@RequestsActionCode)";

			SqliteConnection conn = null;
			try {
				conn = MobileTech.Consts.GetDBConnection ();
				using (SqliteCommand sqlQry = conn.CreateCommand ()) {

					sqlQry.CommandText = sqlStatement;
					sqlQry.Parameters.Add ("@OWSPrimaryId", DbType.UInt64);
					sqlQry.Parameters.Add ("@OpenStatusCode", DbType.UInt16);
					sqlQry.Parameters.Add ("@Description", DbType.String);
					sqlQry.Parameters.Add ("@RequestsActionCode", DbType.UInt16);

					foreach (OpenWorkStatusInfo openWorkStatus in openWorkStatuses) {
						sqlQry.Parameters ["@OWSPrimaryId"].Value = openWorkStatus.PrimaryId;
						sqlQry.Parameters ["@OpenStatusCode"].Value = openWorkStatus.Code;
						sqlQry.Parameters ["@Description"].Value = openWorkStatus.Description;
						sqlQry.Parameters ["@RequestsActionCode"].Value = openWorkStatus.RequestsActionCode;

						sqlQry.ExecuteNonQuery ();
					}

				}
			} catch (Exception ex) {
				string strLogMessage = string.Format("{0} InsertOpenWorkStatusItems Exception: {1}", DateTime.Now, ex.Message);
				MobileTech.Consts.HandleExceptionLog (strLogMessage);
				errorMsg = strLogMessage;
//				UIAlertView loginFailAlert = new UIAlertView();
//				loginFailAlert.Message = "err "+ ex.GetBaseException();
//				loginFailAlert.AddButton("Ok"); 
//				loginFailAlert.Show();

			} finally {
				if (conn != null)
					MobileTech.Consts.ReleaseDBConnection (conn);
			}

		}

		public static void DeleteAssetCenterItems (string[] DeleteKeys)
		{
			const string sqlStatement = @"Delete from Codes_ControlCenter where CCCPrimaryId in ({0})";
			DeleteItems (DeleteKeys, sqlStatement);
		}

		public static void InsertAssetCenterItems (SystemCode[] assetCenters,ref string errorMsg)
		{
			const string sqlStatement = @"Insert Into Codes_ControlCenter (CCCPrimaryId, ControlCenterCode, Description) 
											values (@CCCPrimaryId, @ControlCenterCode, @Description)";

			SqliteConnection conn = null;
			try {
				conn = MobileTech.Consts.GetDBConnection ();
				using (SqliteCommand sqlQry = conn.CreateCommand ()) {

					sqlQry.CommandText = sqlStatement;
					sqlQry.Parameters.Add ("@CCCPrimaryId", DbType.UInt64);
					sqlQry.Parameters.Add ("@ControlCenterCode", DbType.UInt16);
					sqlQry.Parameters.Add ("@Description", DbType.String);

					foreach (SystemCode assetCenter in assetCenters) {
						sqlQry.Parameters ["@CCCPrimaryId"].Value = assetCenter.PrimaryId;
						sqlQry.Parameters ["@ControlCenterCode"].Value = assetCenter.Code;
						sqlQry.Parameters ["@Description"].Value = assetCenter.Description;

						sqlQry.ExecuteNonQuery ();
					}

				}
			} catch (Exception ex) {
				string strLogMessage = string.Format("{0} InsertAssetCenterItems Exception: {1}", DateTime.Now, ex.Message);
				MobileTech.Consts.HandleExceptionLog (strLogMessage);
				errorMsg = strLogMessage;
			} finally {
				if (conn != null)
					MobileTech.Consts.ReleaseDBConnection (conn);
			}

		}

		public static void DeleteEquipmentItems (string[] DeleteKeys)
		{
			const string sqlStatement = @"Delete from Equipment where EquipId in ({0})";
			DeleteItems (DeleteKeys, sqlStatement);
		}

		public static void InsertEquipmentItems (AssetDetails[] equipments,ref string errorMsg)
		{
			const string sqlStatement = @"Insert Into Equipment (EquipId, Control, CCCPrimaryId, DC, MDL, Owner, Location) 
											values (@EquipId, @Control, @CCCPrimaryId, @DC, @MDL, @Owner, @Location)";

			SqliteConnection conn = null;
			try {
				conn = MobileTech.Consts.GetDBConnection ();
				using (SqliteCommand sqlQry = conn.CreateCommand ()) {

					sqlQry.CommandText = sqlStatement;
					sqlQry.Parameters.Add ("@EquipId", DbType.UInt64);
					sqlQry.Parameters.Add ("@Control", DbType.String);
					sqlQry.Parameters.Add ("@CCCPrimaryId", DbType.UInt64);
					sqlQry.Parameters.Add ("@DC", DbType.String);
					sqlQry.Parameters.Add ("@MDL", DbType.String);
					sqlQry.Parameters.Add ("@Owner", DbType.UInt64);
					sqlQry.Parameters.Add ("@Location", DbType.UInt64);

					foreach (AssetDetails equipment in equipments) {
						sqlQry.Parameters ["@EquipId"].Value = equipment.EquipId;
						sqlQry.Parameters ["@Control"].Value = equipment.Control;
						sqlQry.Parameters ["@CCCPrimaryId"].Value = equipment.AssetCenter.PrimaryId;
						sqlQry.Parameters ["@DC"].Value = equipment.Model.DeviceCategory.DevCategory;
						sqlQry.Parameters ["@MDL"].Value = equipment.Model.ModelName;
						sqlQry.Parameters ["@Owner"].Value = equipment.OwnerAccount.AccPrimaryId;
						sqlQry.Parameters ["@Location"].Value = equipment.LocationAccount.AccPrimaryId;

						sqlQry.ExecuteNonQuery ();
					}

				}
			} catch (Exception ex) {
				string strLogMessage = string.Format("{0} InsertEquipmentItems Exception: {1}", DateTime.Now, ex.Message);
				MobileTech.Consts.HandleExceptionLog (strLogMessage);
				errorMsg = strLogMessage;
//				UIAlertView loginFailAlert = new UIAlertView();
//				loginFailAlert.Message = "err "+ ex.GetBaseException();
//				loginFailAlert.AddButton("Ok"); 
//				loginFailAlert.Show();

			} finally {
				if (conn != null)
					MobileTech.Consts.ReleaseDBConnection (conn);
			}

		}

		public static void DeleteAccountItems (string[] DeleteKeys)
		{
			const string sqlStatement = @"Delete from Account where ACCPrimaryId in ({0})";
			DeleteItems (DeleteKeys, sqlStatement);
		}

		public static void InsertAccountItems (AccountSync[] accounts,ref string errorMsg)
		{
			const string sqlStatement = @"Insert Into Account (ACCPrimaryId, AccountId, FACPrimaryId, DepartmentName) 
											values (@ACCPrimaryId, @AccountId, @FACPrimaryId, @DepartmentName)";

			SqliteConnection conn = null;
			try {
				conn = MobileTech.Consts.GetDBConnection ();
				using (SqliteCommand sqlQry = conn.CreateCommand ()) {

					sqlQry.CommandText = sqlStatement;
					sqlQry.Parameters.Add ("@ACCPrimaryId", DbType.UInt64);
					sqlQry.Parameters.Add ("@AccountId", DbType.String);
					sqlQry.Parameters.Add ("@FACPrimaryId", DbType.UInt64);
					sqlQry.Parameters.Add ("@DepartmentName", DbType.String);

					foreach (AccountSync account in accounts) {
						sqlQry.Parameters ["@ACCPrimaryId"].Value = account.AccPrimaryId;
						sqlQry.Parameters ["@AccountId"].Value = account.AccountId;
						sqlQry.Parameters ["@FACPrimaryId"].Value = account.FacPrimaryId;
						sqlQry.Parameters ["@DepartmentName"].Value = account.DeptName;

						sqlQry.ExecuteNonQuery ();
					}

				}
			} catch (Exception ex) {
				string strLogMessage = string.Format("{0} InsertAccountItems Exception: {1}", DateTime.Now, ex.Message);
				MobileTech.Consts.HandleExceptionLog (strLogMessage);
				errorMsg = strLogMessage;
			} finally {
				if (conn != null)
					MobileTech.Consts.ReleaseDBConnection (conn);
			}
		}

		public static void DeleteWkOrderItems (string[] DeleteKeys)
		{
			const string sqlStatement = @"Delete from WkOrders where WORPrimaryid in ({0})";
			DeleteItems (DeleteKeys, sqlStatement);
		}

		public static void InsertWkOrderItems (WorkOrderDetails[] wkOrders,ref string errorMsg)
		{
			errorMsg = string.Empty;
			const string sqlStatement = @"Insert Into WkOrders (WORPrimaryid, WorkOrder, REQPrimaryId, RESPrimaryId, FLTPrimaryId, EquipId, ACCPrimaryId, DateIssue, DateStarted,
				IssueTime, StartTime, CompleteTime, DateDue, DateCompleted, OWSPrimaryId, SFTest, URGPrimaryId, WCCPrimaryId, WKRPrimaryId, OpenFlag, WODescription, WorkCenterCode, 
				LatestDispatchActionCode, canWOBeClosed) 
											values (@WORPrimaryid, @WorkOrder, @REQPrimaryId, @RESPrimaryId, @FLTPrimaryId, @EquipId, @ACCPrimaryId, @DateIssue, @DateStarted,
				@IssueTime, @StartTime, @CompleteTime, @DateDue, @DateCompleted, @OWSPrimaryId, @SFTest, @URGPrimaryId, @WCCPrimaryId, @WKRPrimaryId, @OpenFlag, @WODescription, 
				@WorkCenterCode, @LatestDispatchActionCode, @canWOBeClosed)";

			SqliteConnection conn = null;
			try {
				conn = MobileTech.Consts.GetDBConnection ();
				using (SqliteCommand sqlQuery = conn.CreateCommand ()) {

					sqlQuery.CommandText = sqlStatement;
					sqlQuery.Parameters.Add("@WORPrimaryId", DbType.UInt64);
					sqlQuery.Parameters.Add("@WORKORDER", DbType.String);
					sqlQuery.Parameters.Add("@WCCPrimaryId", DbType.UInt64);
					sqlQuery.Parameters.Add("@WKRPrimaryId", DbType.UInt64);
					sqlQuery.Parameters.Add("@REQPrimaryId", DbType.UInt64);
					sqlQuery.Parameters.Add("@RESPrimaryId", DbType.UInt64);
					sqlQuery.Parameters.Add("@FLTPrimaryId", DbType.UInt64);
					sqlQuery.Parameters.Add("@DateIssue", DbType.DateTime);
					sqlQuery.Parameters.Add("@DateStarted", DbType.DateTime);
					sqlQuery.Parameters.Add("@DateCompleted", DbType.DateTime);
					sqlQuery.Parameters.Add("@EquipId", DbType.UInt64);
					sqlQuery.Parameters.Add("@ACCPrimaryId", DbType.UInt64);
					sqlQuery.Parameters.Add("@SFTest", DbType.String);
					sqlQuery.Parameters.Add("@OpenFlag", DbType.UInt32);
					sqlQuery.Parameters.Add("@IssueTime", DbType.DateTime);
					sqlQuery.Parameters.Add("@StartTime", DbType.DateTime);
					sqlQuery.Parameters.Add("@CompleteTime", DbType.DateTime);
					sqlQuery.Parameters.Add("@DateDue", DbType.DateTime);
					sqlQuery.Parameters.Add("@OWSPrimaryId", DbType.UInt64);
					sqlQuery.Parameters.Add("@URGPrimaryId", DbType.UInt64);
					sqlQuery.Parameters.Add("@WODescription", DbType.String);
					sqlQuery.Parameters.Add("@WorkCenterCode", DbType.Int16);
					sqlQuery.Parameters.Add("@LatestDispatchActionCode", DbType.Int16);
					sqlQuery.Parameters.Add("@canWOBeClosed", DbType.UInt32);



					foreach (WorkOrderDetails wkOrder in wkOrders) {
						sqlQuery.Parameters["@WORPrimaryId"].Value = wkOrder.WorPrimaryId;
						sqlQuery.Parameters["@WORKORDER"].Value = wkOrder.Number;
						sqlQuery.Parameters["@WCCPrimaryId"].Value = wkOrder.ServiceCenter.PrimaryId;
						sqlQuery.Parameters["@WKRPrimaryId"].Value = wkOrder.Worker.WKRPrimaryId;
						sqlQuery.Parameters["@REQPrimaryId"].Value = wkOrder.Request.PrimaryId;
						sqlQuery.Parameters["@RESPrimaryId"].Value = wkOrder.Result.PrimaryId;
						sqlQuery.Parameters["@FLTPrimaryId"].Value = wkOrder.Fault.PrimaryId;

						if (wkOrder.IssueDate == DateTime.MinValue){
							sqlQuery.Parameters["@DateIssue"].Value = DBNull.Value;
							sqlQuery.Parameters["@DateIssue"].DbType = DbType.DateTime;
						}
						else
						{
							sqlQuery.Parameters["@DateIssue"].Value = wkOrder.IssueDate;
						}

						if (wkOrder.StartDate == DateTime.MinValue){
							sqlQuery.Parameters["@DateStarted"].Value = DBNull.Value;
							sqlQuery.Parameters["@DateStarted"].DbType = DbType.DateTime;
						}
						else
						{
							sqlQuery.Parameters["@DateStarted"].Value = wkOrder.StartDate;
						}

						if (wkOrder.CompleteDate == DateTime.MinValue){
							sqlQuery.Parameters["@DateCompleted"].Value = DBNull.Value;
							sqlQuery.Parameters["@DateCompleted"].DbType = DbType.DateTime;
						}
						else
						{
							sqlQuery.Parameters["@DateCompleted"].Value = wkOrder.CompleteDate;
						}

						sqlQuery.Parameters["@EquipId"].Value = wkOrder.AssetDetails.EquipId;
						sqlQuery.Parameters["@ACCPrimaryId"].Value = wkOrder.Account.AccPrimaryId;

						sqlQuery.Parameters["@SFTest"].Value = wkOrder.SafetyTest;
						sqlQuery.Parameters["@OpenFlag"].Value = wkOrder.OpenFlag;


						if (wkOrder.IssueTime == DateTime.MinValue){
							sqlQuery.Parameters["@IssueTime"].Value = DBNull.Value;
							sqlQuery.Parameters["@IssueTime"].DbType = DbType.DateTime;
						}
						else
						{
							sqlQuery.Parameters["@IssueTime"].Value = wkOrder.IssueTime;
						}

						if (wkOrder.StartTime == DateTime.MinValue){
							sqlQuery.Parameters["@StartTime"].Value = DBNull.Value;
							sqlQuery.Parameters["@StartTime"].DbType = DbType.DateTime;
						}
						else
						{
							sqlQuery.Parameters["@StartTime"].Value = wkOrder.StartTime;
						}

						if (wkOrder.CompleteTime == DateTime.MinValue){
							sqlQuery.Parameters["@CompleteTime"].Value = DBNull.Value;
							sqlQuery.Parameters["@CompleteTime"].DbType = DbType.DateTime;
						}
						else
						{
							sqlQuery.Parameters["@CompleteTime"].Value = wkOrder.CompleteTime;
						}
							
						if (wkOrder.DateDue == DateTime.MinValue){
							sqlQuery.Parameters["@DateDue"].Value = DBNull.Value;
							sqlQuery.Parameters["@DateDue"].DbType = DbType.DateTime;
						}
						else
						{
							sqlQuery.Parameters["@DateDue"].Value = wkOrder.DateDue;
						}
							
						sqlQuery.Parameters["@OWSPrimaryId"].Value = wkOrder.OpenWorkOrderStatus.PrimaryId;
						sqlQuery.Parameters["@URGPrimaryId"].Value = wkOrder.Urgency.PrimaryId == 0 ? (object)DBNull.Value : wkOrder.Urgency.PrimaryId;
						sqlQuery.Parameters["@WODescription"].Value = wkOrder.WODescription;
						sqlQuery.Parameters["@WorkCenterCode"].Value = wkOrder.ServiceCenter.Code;
						sqlQuery.Parameters["@LatestDispatchActionCode"].Value = wkOrder.LatestDispatchActionCode;
						sqlQuery.Parameters["@canWOBeClosed"].Value = wkOrder.canWOBeClosed;
						sqlQuery.ExecuteNonQuery ();
					}

				}
			} catch (Exception ex) {
				string strLogMessage = string.Format("{0} InsertWkOrderItems Exception: {1}", DateTime.Now, ex.Message);
				MobileTech.Consts.HandleExceptionLog (strLogMessage);
				errorMsg = strLogMessage;
			} finally {
				if (conn != null)
					MobileTech.Consts.ReleaseDBConnection (conn);
			}
		}

		public static void UpdateBlockLastSyncDate (DateTime syncDate, string block)
		{
			SqliteConnection conn = null;
			try {
				conn = MobileTech.Consts.GetDBConnection ();
				using (SqliteCommand sqlQry = conn.CreateCommand()) {
					sqlQry.CommandText = string.Format (sqlConfigurationDelete, block);
					sqlQry.ExecuteNonQuery ();
				
					//sqlQry.CommandText = string.Format(sqlConfigurationInsert, block,syncDate);
					sqlQry.CommandText = sqlConfigurationInsert;
					sqlQry.Parameters.Add(new SqliteParameter { ParameterName = "@Block", DbType = DbType.String, Value = block });
					sqlQry.Parameters.Add(new SqliteParameter { ParameterName = "@LastSyncDate", DbType = DbType.DateTime, Value = syncDate });

					sqlQry.ExecuteNonQuery ();
				}
			} catch (Exception ex) {
				string strLogMessage = string.Format("{0} UpdateBlockLastSyncDate Exception: {1}", DateTime.Now, ex.Message);
				MobileTech.Consts.HandleExceptionLog(strLogMessage);

//				UIAlertView loginFailAlert = new UIAlertView();
//				loginFailAlert.Message = "err "+ ex.GetBaseException();
//				loginFailAlert.AddButton("Ok"); 
//				loginFailAlert.Show();
			} finally {
				if (conn != null)
					MobileTech.Consts.ReleaseDBConnection (conn);
			}
			
		}


		public static void deleteVersionDetailsSystemSettings (string settingField)
		{
			SqliteConnection conn = null;
			try {
				conn = MobileTech.Consts.GetDBConnection();

				using (SqliteCommand sqlQry = conn.CreateCommand()){
					string sqlStatement = @"Delete from SystemSetup where [Key] = @SettingField";
					sqlQry.CommandText = sqlStatement;
					sqlQry.Parameters.Add ("@SettingField",DbType.String);
					sqlQry.Parameters ["@SettingField"].Value = settingField; 
					sqlQry.ExecuteNonQuery ();
				}
			} catch (Exception ex) {
				string strLogMessage = string.Format("{0} deleteVersionDetailsSystemSettings Exception: {1}", DateTime.Now, ex.Message);
				MobileTech.Consts.HandleExceptionLog(strLogMessage);
			} finally {
				if(conn != null)
					MobileTech.Consts.ReleaseDBConnection(conn);
			}
		}

		public static void insertSystemSettings (SystemSetting[] systemSettings,ref string errorMsg)
		{
			errorMsg = string.Empty;
			const string sqlStatement = @"Insert Into SystemSetup (KEY, VALUE) values (@Key,@Value)";

			SqliteConnection conn = null;
			try {
				conn = MobileTech.Consts.GetDBConnection();
				using (SqliteCommand sqlQry = conn.CreateCommand()) {

					sqlQry.CommandText = sqlStatement;
					sqlQry.Parameters.Add ("@Key",DbType.String);
					sqlQry.Parameters.Add ("@Value", DbType.String);

					foreach (SystemSetting setting in systemSettings) {
						sqlQry.Parameters ["@Key"].Value = setting.SettingField; 
						sqlQry.Parameters ["@Value"].Value = setting.SettingValue;

						sqlQry.ExecuteNonQuery ();
					}
				}
			} catch (Exception ex) {
				//Author/Date/Issue - M.D.Prasad/14th May 2014/ISS - 5395 or CRR - 1338  Upload iGotIt device logs
				string strLogMessage = string.Format("{0} insertSystemSettings Exception: {1}", DateTime.Now, ex.Message);
				MobileTech.Consts.HandleExceptionLog(strLogMessage);
				errorMsg = string.Empty;
			} finally {
				if(conn != null)
					MobileTech.Consts.ReleaseDBConnection(conn);
			}
		}

		public static void DeleteResultItems(string[] DeleteKeys)
		{
			const string sqlStatement = @"Delete from Codes_Result where ResPrimaryid in ({0})";
			DeleteItems(DeleteKeys,sqlStatement);
		}

		public static void InsertResultItems (SystemCode[] results,ref string errorMsg)
		{
			errorMsg = string.Empty;
			const string sqlStatement = @"Insert Into Codes_Result (RESPrimaryId, ResultCode, Description) 
									values (@RESPrimaryId, @ResultCode, @Description)";

			SqliteConnection conn = null;
			try {
				conn = MobileTech.Consts.GetDBConnection();
				using (SqliteCommand sqlQry = conn.CreateCommand ()) {

					sqlQry.CommandText = sqlStatement;
					sqlQry.Parameters.Add ("@RESPrimaryId", DbType.UInt64);
					sqlQry.Parameters.Add ("@ResultCode", DbType.UInt16);
					sqlQry.Parameters.Add ("@Description", DbType.String);

					foreach (SystemCode result in results) {
						sqlQry.Parameters ["@RESPrimaryId"].Value = result.PrimaryId;
						sqlQry.Parameters ["@ResultCode"].Value = result.Code;
						sqlQry.Parameters ["@Description"].Value = result.Description;

						sqlQry.ExecuteNonQuery ();
					}

				}
			} catch (Exception ex) {
				string strLogMessage = string.Format("{0} InsertResultItems Exception: {1}", DateTime.Now, ex.Message);
				MobileTech.Consts.HandleExceptionLog(strLogMessage);
				errorMsg = strLogMessage;
			} finally {
				if(conn != null)
					MobileTech.Consts.ReleaseDBConnection(conn);
			}
		}

		public static void DeleteFaultItems(string[] DeleteKeys)
		{
			const string sqlStatement = @"Delete from Codes_Fault where FltPrimaryid in ({0})";
			DeleteItems(DeleteKeys,sqlStatement);
		}

		public static void InsertFaultItems (SystemCode[] faults,ref string errorMsg)
		{
			errorMsg = string.Empty;
			const string sqlStatement = @"Insert Into Codes_Fault (FLTPrimaryId, FaultCode, Description) 
									values (@FLTPrimaryId, @FaultCode, @Description)";

			SqliteConnection conn = null;
			try {
				conn = MobileTech.Consts.GetDBConnection();
				using (SqliteCommand sqlQry = conn.CreateCommand ()) {

					sqlQry.CommandText = sqlStatement;
					sqlQry.Parameters.Add ("@FLTPrimaryId", DbType.UInt64);
					sqlQry.Parameters.Add ("@FaultCode", DbType.UInt16);
					sqlQry.Parameters.Add ("@Description", DbType.String);

					foreach (SystemCode fault in faults) {
						sqlQry.Parameters ["@FLTPrimaryId"].Value = fault.PrimaryId;
						sqlQry.Parameters ["@FaultCode"].Value = fault.Code;
						sqlQry.Parameters ["@Description"].Value = fault.Description;

						sqlQry.ExecuteNonQuery ();
					}

				}
			} catch (Exception ex) {
				string strLogMessage = string.Format("{0} InsertFaultItems Exception: {1}", DateTime.Now, ex.Message);
				MobileTech.Consts.HandleExceptionLog(strLogMessage);
				errorMsg = strLogMessage;
			} finally {
				if(conn != null)
					MobileTech.Consts.ReleaseDBConnection(conn);
			}

		}

		public static void DeleteResultCenterItems(string[] DeleteKeys)
		{
			const string sqlStatement = @"Delete from Codes_ResultCenter where RSCPrimaryId in ({0})";
			DeleteItems(DeleteKeys,sqlStatement);
		}

		public static void InsertResultCenterItems (ResultInfo[] resultCenters,ref string errorMsg)
		{
			errorMsg = string.Empty;
			const string sqlStatement = @"Insert Into Codes_ResultCenter (RSCPrimaryId, RESPrimaryId, WCCPrimaryId, Archived) 
									values (@RSCPrimaryId, @RESPrimaryId, @WCCPrimaryId, @Archived)";

			SqliteConnection conn = null;
			try {
				conn = MobileTech.Consts.GetDBConnection();
				using (SqliteCommand sqlQry = conn.CreateCommand ()) {

					sqlQry.CommandText = sqlStatement;
					sqlQry.Parameters.Add ("@RSCPrimaryId", DbType.UInt64);
					sqlQry.Parameters.Add ("@RESPrimaryId", DbType.UInt64);
					sqlQry.Parameters.Add ("@WCCPrimaryId", DbType.UInt64);
					sqlQry.Parameters.Add ("@Archived", DbType.String);

					foreach (ResultInfo resultInfo in resultCenters) {
						sqlQry.Parameters ["@RSCPrimaryId"].Value = resultInfo.Center.RSCPrimaryId;
						sqlQry.Parameters ["@RESPrimaryId"].Value = resultInfo.PrimaryId;
						sqlQry.Parameters ["@WCCPrimaryId"].Value = resultInfo.Center.ServiceCenter.PrimaryId;
						sqlQry.Parameters ["@Archived"].Value = resultInfo.Center.Archived;
						sqlQry.ExecuteNonQuery ();
					}
				}
			} catch (Exception ex) {
				string strLogMessage = string.Format("{0} InsertResultCenterItems Exception: {1}", DateTime.Now, ex.Message);
				MobileTech.Consts.HandleExceptionLog(strLogMessage);
				errorMsg = strLogMessage;
			} finally {
				if(conn != null)
					MobileTech.Consts.ReleaseDBConnection(conn);
			}
		}

		public static void DeleteFaultCenterItems(string[] DeleteKeys)
		{
			const string sqlStatement = @"Delete from Codes_FaultCenter where FLCPrimaryId in ({0})";
			DeleteItems(DeleteKeys,sqlStatement);
		}

		public static void InsertFaultCenterItems (FaultInfo[] faultCenters,ref string errorMsg)
		{
			errorMsg = string.Empty;
			const string sqlStatement = @"Insert Into Codes_FaultCenter (FLCPrimaryId, FLTPrimaryId, WCCPrimaryId, Archived) 
									values (@FLCPrimaryId, @FLTPrimaryId, @WCCPrimaryId, @Archived)";

			SqliteConnection conn = null;
			try {
				conn = MobileTech.Consts.GetDBConnection();
				using (SqliteCommand sqlQry = conn.CreateCommand ()) {

					sqlQry.CommandText = sqlStatement;
					sqlQry.Parameters.Add ("@FLCPrimaryId", DbType.UInt64);
					sqlQry.Parameters.Add ("@FLTPrimaryId", DbType.UInt64);
					sqlQry.Parameters.Add ("@WCCPrimaryId", DbType.UInt64);
					sqlQry.Parameters.Add ("@Archived", DbType.String);

					foreach (FaultInfo faultInfo in faultCenters) {
						sqlQry.Parameters ["@FLCPrimaryId"].Value = faultInfo.Center.FLCPrimaryId;
						sqlQry.Parameters ["@FLTPrimaryId"].Value = faultInfo.PrimaryId;
						sqlQry.Parameters ["@WCCPrimaryId"].Value = faultInfo.Center.ServiceCenter.PrimaryId;
						sqlQry.Parameters ["@Archived"].Value = faultInfo.Center.Archived;

						sqlQry.ExecuteNonQuery ();
					}

				}
			} catch (Exception ex) {
				string strLogMessage = string.Format("{0} InsertFaultCenterItems Exception: {1}", DateTime.Now, ex.Message);
				MobileTech.Consts.HandleExceptionLog(strLogMessage);
				errorMsg = strLogMessage;
			} finally {
				if(conn != null)
					MobileTech.Consts.ReleaseDBConnection(conn);
			}
		}

		public static void DeleteRequestCenterItems(string[] DeleteKeys)
		{
			const string sqlStatement = @"Delete from Codes_RequestCenter where RQCPrimaryId in ({0})";
			DeleteItems(DeleteKeys,sqlStatement);
		}

		public static void InsertRequestCenterItems (RequestInfo[] requestCenters,ref string errorMsg)
		{
			errorMsg = string.Empty;
			const string sqlStatement = @"Insert Into Codes_RequestCenter (RQCPrimaryId, REQPrimaryId, WCCPrimaryId, RequireFaultCode, RequireControl) 
									values (@RQCPrimaryId, @REQPrimaryId, @WCCPrimaryId, @RequireFaultCode, @RequireControl)";

			SqliteConnection conn = null;
			try {
				conn = MobileTech.Consts.GetDBConnection();
				using (SqliteCommand sqlQry = conn.CreateCommand ()) {

					sqlQry.CommandText = sqlStatement;
					sqlQry.Parameters.Add ("@RQCPrimaryId", DbType.UInt64);
					sqlQry.Parameters.Add ("@REQPrimaryId", DbType.UInt64);
					sqlQry.Parameters.Add ("@WCCPrimaryId", DbType.UInt64);
					sqlQry.Parameters.Add ("@RequireFaultCode", DbType.String);
					sqlQry.Parameters.Add ("@RequireControl", DbType.String);

					foreach (RequestInfo requestInfo in requestCenters) {
						sqlQry.Parameters ["@RQCPrimaryId"].Value = requestInfo.Center.RqcPrimaryId;
						sqlQry.Parameters ["@REQPrimaryId"].Value = requestInfo.PrimaryId;
						sqlQry.Parameters ["@WCCPrimaryId"].Value = requestInfo.Center.ServiceCenter.PrimaryId;
						sqlQry.Parameters ["@RequireFaultCode"].Value = requestInfo.Center.RequireFaultCode;
						sqlQry.Parameters ["@RequireControl"].Value = requestInfo.Center.RequireControl;
						sqlQry.ExecuteNonQuery ();
					}
				}
			} catch (Exception ex) {
				string strLogMessage = string.Format("{0} InsertRequestCenterItems Exception: {1}", DateTime.Now, ex.Message);
				MobileTech.Consts.HandleExceptionLog(strLogMessage);
				errorMsg = strLogMessage;
			} finally {
				if(conn != null)
					MobileTech.Consts.ReleaseDBConnection(conn);
			}
		}

		public static void DeleteValidRequestsResultsItems(string[] DeleteKeys)
		{
			const string sqlStatement = @"Delete from Codes_ValidRequestsResults where VRRPrimaryId in ({0})";
			DeleteItems(DeleteKeys,sqlStatement);
		}

		public static void InsertValidRequestsResultsItems (ValidRequestsResult[] validRequestsResults,ref string errorMsg)
		{
			errorMsg = string.Empty;
			const string sqlStatement = @"Insert Into Codes_ValidRequestsResults (VRRPrimaryId, REQPrimaryId, RESPrimaryId, WCCPrimaryId) 
									values (@VRRPrimaryId, @REQPrimaryId, @RESPrimaryId, @WCCPrimaryId)";

			SqliteConnection conn = null;
			try {
				conn = MobileTech.Consts.GetDBConnection();
				using (SqliteCommand sqlQry = conn.CreateCommand ()) {
					sqlQry.CommandText = sqlStatement;
					sqlQry.Parameters.Add ("@VRRPrimaryId", DbType.UInt64);
					sqlQry.Parameters.Add ("@REQPrimaryId", DbType.UInt64);
					sqlQry.Parameters.Add ("@RESPrimaryId", DbType.UInt64);
					sqlQry.Parameters.Add ("@WCCPrimaryId", DbType.UInt64);

					foreach (ValidRequestsResult validReqResult in validRequestsResults) {
						sqlQry.Parameters ["@VRRPrimaryId"].Value = validReqResult.VRRPrimaryId;
						sqlQry.Parameters ["@REQPrimaryId"].Value = validReqResult.Request.PrimaryId;
						sqlQry.Parameters ["@RESPrimaryId"].Value = validReqResult.Result.PrimaryId;
						sqlQry.Parameters ["@WCCPrimaryId"].Value = validReqResult.ServiceCenter.PrimaryId;
						sqlQry.ExecuteNonQuery ();
					}
				}
			} catch (Exception ex) {
				string strLogMessage = string.Format("{0} InsertValidRequestsResultsItems Exception: {1}", DateTime.Now, ex.Message);
				MobileTech.Consts.HandleExceptionLog(strLogMessage);
				errorMsg = strLogMessage;
			} finally {
				if(conn != null)
					MobileTech.Consts.ReleaseDBConnection(conn);
			}
		}

		public static void DeleteWOTextItems(string[] DeleteKeys)
		{
			const string sqlStatement = @"Delete from Wotext where WORPrimaryid in ({0})";
			DeleteItems(DeleteKeys,sqlStatement);
		}

		public static void DeleteTable (string tableName)
		{
			SqliteConnection conn = null;
			try {
				conn = MobileTech.Consts.GetDBConnection();

				using (SqliteCommand sqlQry = conn.CreateCommand()){
					string sqlStatement = @"Delete from "+ tableName;
					sqlQry.CommandText = sqlStatement;
					sqlQry.ExecuteNonQuery ();
				}
			} catch (Exception ex) {
				string strLogMessage = string.Format("{0} deleteTable Exception: {1}", DateTime.Now, ex.Message);
				MobileTech.Consts.HandleExceptionLog(strLogMessage);
			} finally {
				if(conn != null)
					MobileTech.Consts.ReleaseDBConnection(conn);
			}
		}


		public static void InsertWOTextItems (WorkOrderText[] woTexts,ref string errorMsg)
		{
			errorMsg = string.Empty;
			const string sqlStatement = @"Insert Into Wotext (WORPrimaryid, TextField) 
									values (@WORPrimaryid, @TextField)";

			SqliteConnection conn = null;
			try {
				conn = MobileTech.Consts.GetDBConnection();
				using (SqliteCommand sqlQry = conn.CreateCommand ()) {

					sqlQry.CommandText = sqlStatement;
					sqlQry.Parameters.Add ("@WORPrimaryid", DbType.UInt64);
					sqlQry.Parameters.Add ("@TextField", DbType.Binary);

					foreach (WorkOrderText woText in woTexts) {
						sqlQry.Parameters ["@WORPrimaryid"].Value = woText.WorkOrder.WorPrimaryId;
						sqlQry.Parameters ["@TextField"].Value = woText.Text;

						sqlQry.ExecuteNonQuery ();
					}
				}
			} catch (Exception ex) {
				string strLogMessage = string.Format("{0} InsertWOTextItems Exception: {1}", DateTime.Now, ex.Message);
				MobileTech.Consts.HandleExceptionLog(strLogMessage);
				errorMsg = strLogMessage;
			} finally {
				if(conn != null)
					MobileTech.Consts.ReleaseDBConnection(conn);
			}
		}

		public static void DeleteTimeTypeItems(string[] DeleteKeys)
		{
			const string sqlStatement = @"Delete from Codes_WorkOrderTimeType where WtbPrimaryid in ({0})";
			DeleteItems(DeleteKeys,sqlStatement);
		}

		public static void InsertTimeTypeItems (SystemCode[] timeTypes,ref string errorMsg)
		{
			errorMsg = string.Empty;
			const string sqlStatement = @"Insert Into Codes_WorkOrderTimeType (WTBPrimaryid, Description) 
									values (@WTBPrimaryid, @Description)";

			SqliteConnection conn = null;
			try {
				conn = MobileTech.Consts.GetDBConnection();
				using (SqliteCommand sqlQry = conn.CreateCommand ()) {

					sqlQry.CommandText = sqlStatement;
					sqlQry.Parameters.Add ("@WTBPrimaryid", DbType.UInt64);
					sqlQry.Parameters.Add ("@Description", DbType.String);

					foreach (SystemCode timetype in timeTypes) {
						sqlQry.Parameters ["@WTBPrimaryid"].Value = timetype.PrimaryId;
						sqlQry.Parameters ["@Description"].Value = timetype.Description;

						sqlQry.ExecuteNonQuery ();
					}

				}
			} catch (Exception ex) {
				string strLogMessage = string.Format("{0} InsertTimeTypeItems Exception: {1}", DateTime.Now, ex.Message);
				MobileTech.Consts.HandleExceptionLog(strLogMessage);
				errorMsg = strLogMessage;
			} finally {
				if(conn != null)
					MobileTech.Consts.ReleaseDBConnection(conn);
			}
		}

		public static void InsertPDADBID (PDADBID[] pdadbids,ref string errorMsg)
		{
			errorMsg = string.Empty;
			const string sqlStatement = @"Insert Into PDADbid (username, Dbid, Devprimaryid, MKMDid, Souprimaryid, DVCPrimaryid, 
											EQCPrimaryid, MMCprimaryid, Equipid, WORPrimaryid, WOPPrimaryid, WOMPrimaryid, MESPrimaryid, 
											EORPrimaryID, EOHPrimaryID, PATPrimaryID, PACPrimaryID, PPOPrimaryid, SWCPrimaryID, 
											UDDPrimaryID, WOTPrimaryID, WOMCHPrimaryid, WOMCRPrimaryid) 
										Values (@username, @Dbid, @Devprimaryid, @MKMDid, @Souprimaryid, @DVCPrimaryid, 
											@EQCPrimaryid, @MMCprimaryid, @Equipid, @WORPrimaryid, @WOPPrimaryid, @WOMPrimaryid, 
											@MESPrimaryid, @EORPrimaryID, @EOHPrimaryID, @PATPrimaryID, @PACPrimaryID, @PPOPrimaryid, 
											@SWCPrimaryID, @UDDPrimaryID, @WOTPrimaryID, @WOMCHPrimaryid, @WOMCRPrimaryid)";

			SqliteConnection conn = null;
			try {
				conn = MobileTech.Consts.GetDBConnection();
				using (SqliteCommand sqlQry = conn.CreateCommand ()) {

					sqlQry.CommandText = sqlStatement;
					sqlQry.Parameters.Add ("@username", DbType.String);
					sqlQry.Parameters.Add ("@Dbid", DbType.UInt64);
					sqlQry.Parameters.Add ("@Devprimaryid", DbType.UInt64);
					sqlQry.Parameters.Add ("@MKMDid", DbType.UInt64);
					sqlQry.Parameters.Add ("@Souprimaryid", DbType.UInt64);
					sqlQry.Parameters.Add ("@DVCPrimaryid", DbType.UInt64);
					sqlQry.Parameters.Add ("@EQCPrimaryid", DbType.UInt64);
					sqlQry.Parameters.Add ("@MMCprimaryid", DbType.UInt64);
					sqlQry.Parameters.Add ("@Equipid", DbType.UInt64);
					sqlQry.Parameters.Add ("@WORPrimaryid", DbType.UInt64);
					sqlQry.Parameters.Add ("@WOPPrimaryid", DbType.UInt64);
					sqlQry.Parameters.Add ("@WOMPrimaryid", DbType.UInt64);
					sqlQry.Parameters.Add ("@MESPrimaryid", DbType.UInt64);
					sqlQry.Parameters.Add ("@EORPrimaryID", DbType.UInt64);
					sqlQry.Parameters.Add ("@EOHPrimaryID", DbType.UInt64);
					sqlQry.Parameters.Add ("@PATPrimaryID", DbType.UInt64);
					sqlQry.Parameters.Add ("@PACPrimaryID", DbType.UInt64);
					sqlQry.Parameters.Add ("@PPOPrimaryid", DbType.UInt64);
					sqlQry.Parameters.Add ("@SWCPrimaryID", DbType.UInt64);
					sqlQry.Parameters.Add ("@UDDPrimaryID", DbType.UInt64);
					sqlQry.Parameters.Add ("@WOTPrimaryID", DbType.UInt64);
					sqlQry.Parameters.Add ("@WOMCHPrimaryid", DbType.UInt64);
					sqlQry.Parameters.Add ("@WOMCRPrimaryid", DbType.UInt64);

					foreach (PDADBID pdadbid in pdadbids) {
						#region Parameters Assigning
						sqlQry.Parameters ["@username"].Value = pdadbid.username;
						sqlQry.Parameters ["@Dbid"].Value = pdadbid.Dbid;
						sqlQry.Parameters ["@Devprimaryid"].Value = pdadbid.DevPrimaryid;
						sqlQry.Parameters ["@MKMDid"].Value = pdadbid.MKMDid;
						sqlQry.Parameters ["@Souprimaryid"].Value = pdadbid.SouPrimaryid;
//						sqlQry.Parameters ["@DVCPrimaryid"].Value = 0;
//						sqlQry.Parameters ["@EQCPrimaryid"].Value = 0;
//						sqlQry.Parameters ["@MMCprimaryid"].Value = 0;
						sqlQry.Parameters ["@Equipid"].Value = pdadbid.Equipid;
						sqlQry.Parameters ["@WORPrimaryid"].Value = pdadbid.WORPrimaryId;
						sqlQry.Parameters ["@WOPPrimaryid"].Value = pdadbid.WOPPrimaryid;
						sqlQry.Parameters ["@WOMPrimaryid"].Value = pdadbid.WOMPrimaryid;
//						sqlQry.Parameters ["@MESPrimaryid"].Value = 0;
//						sqlQry.Parameters ["@EORPrimaryID"].Value = 0;
//						sqlQry.Parameters ["@EOHPrimaryID"].Value = 0;
//						sqlQry.Parameters ["@PATPrimaryID"].Value = 0;
//						sqlQry.Parameters ["@PACPrimaryID"].Value = 0;
						sqlQry.Parameters ["@PPOPrimaryid"].Value = pdadbid.PPOPrimaryid;
//						sqlQry.Parameters ["@SWCPrimaryID"].Value = 0;
						sqlQry.Parameters ["@UDDPrimaryID"].Value = pdadbid.UDDPrimaryid;
						sqlQry.Parameters ["@WOTPrimaryID"].Value = pdadbid.WOTPrimaryid;
						sqlQry.Parameters ["@WOMCHPrimaryid"].Value = pdadbid.WOMCHPrimaryid;
						sqlQry.Parameters ["@WOMCRPrimaryid"].Value = pdadbid.WOMCRPrimaryid;
						#endregion

						sqlQry.ExecuteNonQuery ();
					}
				}
			} catch (Exception ex) {
				string strLogMessage = string.Format("{0} InsertPDADBID Exception: {1}", DateTime.Now, ex.Message);
				MobileTech.Consts.HandleExceptionLog(strLogMessage);
				errorMsg = strLogMessage;
			} finally {
				if(conn != null)
					MobileTech.Consts.ReleaseDBConnection(conn);
			}
		}

		public static void DeleteAccountFacilityItems(string[] DeleteKeys)
		{
			const string sqlStatement = @"Delete from AccountFacility where FACPrimaryId in ({0})";
			DeleteItems(DeleteKeys,sqlStatement);
		}

		public static void InsertAccountFacilityItems (FacilityDetails[] facilities, ref string errorMsg)
		{
//			const string sqlStatement = @"Insert Into AccountFacility (FACPrimaryId, Facility, TaxSTAT, TaxRate, LaborTax, PartsTax, MiscTax) 
//									values (@FACPrimaryId, @Facility, @TaxSTAT, @TaxRate, @LaborTax, @PartsTax, @MiscTax)";

			const string sqlStatement = @"Insert Into AccountFacility (FACPrimaryId, Facility) 
									values (@FACPrimaryId, @Facility)";

			SqliteConnection conn = null;
			try {
				conn = MobileTech.Consts.GetDBConnection();
				using (SqliteCommand sqlQry = conn.CreateCommand ()) {

					sqlQry.CommandText = sqlStatement;
					sqlQry.Parameters.Add ("@FACPrimaryId", DbType.UInt64);
					sqlQry.Parameters.Add ("@Facility", DbType.String);
//					sqlQry.Parameters.Add ("@TaxSTAT", DbType.String);
//					sqlQry.Parameters.Add ("@TaxRate", DbType.Decimal);
//					sqlQry.Parameters.Add ("@LaborTax", DbType.String);
//					sqlQry.Parameters.Add ("@PartsTax", DbType.String);
//					sqlQry.Parameters.Add ("@MiscTax", DbType.String);

					foreach (FacilityDetails facility in facilities) {
						sqlQry.Parameters ["@FACPrimaryId"].Value = facility.FacPrimaryId;
						sqlQry.Parameters ["@Facility"].Value = facility.Name;
//						sqlQry.Parameters ["@TaxSTAT"].Value = facility.TaxStatus;
//						sqlQry.Parameters ["@TaxRate"].Value = facility.TaxRate;
//						sqlQry.Parameters ["@LaborTax"].Value = facility.LaborTax;
//						sqlQry.Parameters ["@PartsTax"].Value = facility.PartsTax;
//						sqlQry.Parameters ["@MiscTax"].Value = facility.MiscTax;

						sqlQry.ExecuteNonQuery ();
					}
				}
			} catch (Exception ex) {
				string strLogMessage = string.Format("{0} InsertAccountFacilityItems Exception: {1}", DateTime.Now, ex.Message);
				MobileTech.Consts.HandleExceptionLog(strLogMessage);
				errorMsg = strLogMessage;
			} finally {
				if(conn != null)
					MobileTech.Consts.ReleaseDBConnection(conn);
			}
		}

		public static void DeleteWorkerItems(string[] DeleteKeys)
		{
			const string sqlStatement = @"Delete from Workers where WKRPrimaryid in ({0})";
			DeleteItems(DeleteKeys,sqlStatement);
		}

		public static void InsertWorkerItems (WorkerInfo[] workers, ref string errorMsg)
		{
			//const string sqlStatement = @"Insert Into Workers (WKRPrimaryid, WorkerId, Name, WTCPrimaryid, Status, StartDate, EndDate, SOUPrimaryId)  
									//values (@WKRPrimaryid, @WorkerId, @Name, @WTCPrimaryid, @Status, @StartDate, @EndDate, @SOUPrimaryId)";

			const string sqlStatement = @"Insert Into Workers (WKRPrimaryid, Name)  
									values (@WKRPrimaryid, @Name)";

			SqliteConnection conn = null;
			try {
				conn = MobileTech.Consts.GetDBConnection();
				using (SqliteCommand sqlQry = conn.CreateCommand ()) {

					sqlQry.CommandText = sqlStatement;
					sqlQry.Parameters.Add ("@WKRPrimaryid", DbType.UInt64);
					//sqlQry.Parameters.Add ("@WorkerId", DbType.Int16);
					sqlQry.Parameters.Add ("@Name", DbType.String);
//					sqlQry.Parameters.Add ("@WTCPrimaryid", DbType.UInt64);
//					sqlQry.Parameters.Add ("@Status", DbType.String);
//					sqlQry.Parameters.Add ("@StartDate", DbType.String);
//					sqlQry.Parameters.Add ("@EndDate", DbType.String);
//					sqlQry.Parameters.Add ("@SOUPrimaryId", DbType.UInt64);

					foreach (WorkerInfo workerInfo in workers) {
						sqlQry.Parameters ["@WKRPrimaryid"].Value = workerInfo.WKRPrimaryId;
						//sqlQry.Parameters ["@WorkerId"].Value = workerInfo.Id;
						sqlQry.Parameters ["@Name"].Value = workerInfo.Name;
//						sqlQry.Parameters ["@WTCPrimaryid"].Value = workerInfo.WorkerType.PrimaryId;
//						sqlQry.Parameters ["@Status"].Value = workerInfo.Status;
//						sqlQry.Parameters ["@StartDate"].Value = workerInfo.StartDate;
//						sqlQry.Parameters ["@EndDate"].Value = workerInfo.EndDate;
//						sqlQry.Parameters ["@SOUPrimaryId"].Value = workerInfo.Source.SouPrimaryId;

						sqlQry.ExecuteNonQuery ();
					}

				}
			} catch (Exception ex) {
				string strLogMessage = string.Format("{0} InsertWorkerItems Exception: {1}", DateTime.Now, ex.Message);
				MobileTech.Consts.HandleExceptionLog(strLogMessage);
				errorMsg = strLogMessage;
			} finally {
				if(conn != null)
					MobileTech.Consts.ReleaseDBConnection(conn);
			}
		}
	}
}

