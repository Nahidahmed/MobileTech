using System;
using System.Collections.Generic;

using Mono.Data.Sqlite;

using MobileTech.Models.Entity;
using System.Data;
using System.Net;
using MobileTechService;
using System.IO;
using Newtonsoft.Json;
using System.Text;


namespace MobileTech.Core
{
	public class Repository
	{
		public Repository ()
		{
		}

		public static long getWorkerIDForLoginUser (string loginUser,ref string errorMsg)
		{
			errorMsg = string.Empty;
			long WKRPrimaryId = 0;
			SqliteConnection conn = null;
			try {
				conn = MobileTech.Consts.GetDBConnection ();

				string sqlUsrSelect = @"Select WKRPrimaryId from Security Where lower(UserNm) = @loginUser or lower(WindowsUser) = @loginUser";

				using (SqliteCommand sqlUsrSelectQry = conn.CreateCommand()) {
					sqlUsrSelectQry.CommandText = sqlUsrSelect;

					sqlUsrSelectQry.Parameters.Add ("@loginUser", DbType.String);
					sqlUsrSelectQry.Parameters ["@loginUser"].Value = MobileTech.Consts.LoginUserName.ToLower();

					using (SqliteDataReader rReader = sqlUsrSelectQry.ExecuteReader()) {
						while (rReader.Read()) {
							WKRPrimaryId = Convert.ToInt64(rReader.GetValue (rReader.GetOrdinal ("WKRPrimaryId")));
						}
					}
				}
			}catch (Exception ex) {
				//Console.WriteLine ("Exception in getWorkerIDForLoginUser : " + ex.Message);
				string strLogMessage = string.Format("{0} getWorkerIDForLoginUser Exception: {1}", DateTime.Now, ex.Message);
				errorMsg = strLogMessage;
				Consts.HandleExceptionLog(strLogMessage);
			} finally {
				if (conn != null)
					MobileTech.Consts.ReleaseDBConnection (conn);
			}

			return WKRPrimaryId;
		}

		public List<WorkOrderDetails> GetWorkOrders() {

			SqliteConnection conn = null;
			List<WorkOrderDetails> wkOrders = new List<WorkOrderDetails> ();
			try {
				conn = MobileTech.Consts.GetDBConnection ();

				string sqlUsrSelect = @"Select W.WORPrimaryId, W.WorkOrder, W.WODescription, R.Description ReqDesc, c.ControlCenterCode, E.Control, E.DC, E.MDL
                            , O.Description OpenWrkStatusDesc, U.Description UrgDesc, Account.DepartmentName, W.SFTest, W.WCCPrimaryId, W.WorkCenterCode, W.REQPrimaryId
							from WkOrders W 
							Left Outer Join Codes_Request R on W.REQPrimaryId = R.REQPrimaryId
							Left Outer Join Equipment E on W.EquipId = E.EquipId
                            Left Outer Join Codes_ControlCenter  C on C.CCCPrimaryID = E.CCCPrimaryID 
							Left Outer Join Codes_OpenWorkStatus O on W.OWSPrimaryId = O.OWSPrimaryId
							Left Outer Join Codes_Urgency U on W.URGPrimaryId = U.URGPrimaryId
                            Left Outer Join Account on Account.ACCPrimaryid = W.ACCPrimaryid
                            where W.openflag = 1 
							Order by  DateDue isnull, DateDue, DateIssue isnull, DateIssue";

				using (SqliteCommand sqlUsrSelectQry = conn.CreateCommand()) {
					sqlUsrSelectQry.CommandText = sqlUsrSelect;
					//					sqlUsrSelectQry.Parameters.Add ("@loginUser", DbType.String);
					//sqlUsrSelectQry.Parameters ["@loginUser"].Value = iNeedIt.Consts.LoginUserName.ToLower();
					using (SqliteDataReader rReader = sqlUsrSelectQry.ExecuteReader()) {
						while (rReader.Read()) {
							WorkOrderDetails wkOrder = new WorkOrderDetails();

							wkOrder.WorPrimaryId = Convert.ToInt64(rReader.GetValue (rReader.GetOrdinal ("WORPrimaryId")));
							wkOrder.Number = Convert.ToString(rReader.GetValue(rReader.GetOrdinal("WorkOrder")));
							wkOrder.WODescription = Convert.ToString(rReader.GetValue(rReader.GetOrdinal("WODescription")));

							wkOrder.Request = new RequestInfo(); 
							wkOrder.AssetDetails = new AssetDetails();
							wkOrder.AssetDetails.Model = new ModelInfo();
							wkOrder.AssetDetails.Model.DeviceCategory = new DeviceCategoryInfo();
							wkOrder.OpenWorkOrderStatus = new SystemCode();
							wkOrder.Urgency = new UrgencyInfo();
							wkOrder.ServiceCenter = new SystemCodeInfo();

							wkOrder.Request.Description = Convert.ToString(rReader.GetValue(rReader.GetOrdinal("ReqDesc")));
							wkOrder.Request.PrimaryId = Convert.ToInt64(rReader.GetValue (rReader.GetOrdinal ("REQPrimaryId")));
							wkOrder.AssetDetails.Control = Convert.ToString(rReader.GetValue(rReader.GetOrdinal("Control")) == DBNull.Value ? string.Empty : rReader.GetValue(rReader.GetOrdinal("Control")) );
							wkOrder.AssetDetails.AssetCenter = new SystemCode();

							if(wkOrder.AssetDetails.Control != string.Empty){
								wkOrder.AssetDetails.AssetCenter.Code = Convert.ToInt16(rReader.GetValue(rReader.GetOrdinal("ControlCenterCode")));
								wkOrder.AssetDetails.Model.DeviceCategory.DevCategory = Convert.ToString(rReader.GetValue(rReader.GetOrdinal("DC")));
								wkOrder.AssetDetails.Model.ModelName = Convert.ToString(rReader.GetValue(rReader.GetOrdinal("MDL")));
							}else{
								wkOrder.AssetDetails.AssetCenter.Code = Convert.ToInt16(0);
								wkOrder.AssetDetails.Model.DeviceCategory.DevCategory = string.Empty;
								wkOrder.AssetDetails.Model.ModelName =string.Empty;
							}

							wkOrder.OpenWorkOrderStatus.Description = Convert.ToString(rReader.GetValue(rReader.GetOrdinal("OpenWrkStatusDesc")));
							wkOrder.Urgency.Description = Convert.ToString(rReader.GetValue(rReader.GetOrdinal("UrgDesc")));

							wkOrder.Account = new Account();
							wkOrder.Account.DepartmentName = Convert.ToString(rReader.GetValue(rReader.GetOrdinal("DepartmentName")));

							wkOrder.SafetyTest = Convert.ToString(rReader.GetValue(rReader.GetOrdinal("SFTest")));

							wkOrder.ServiceCenter.PrimaryId = Convert.ToInt64(rReader.GetValue (rReader.GetOrdinal ("WCCPrimaryId")));
							wkOrder.ServiceCenter.Code = Convert.ToInt16(rReader.GetValue(rReader.GetOrdinal("WorkCenterCode")));

							wkOrders.Add(wkOrder);
						}
					}
				}
			}catch (Exception ex) {
				Console.WriteLine ("Exception in GetWorkOrders : " + ex.Message);
			} finally {
				if (conn != null)
					MobileTech.Consts.ReleaseDBConnection (conn);
			}

			return wkOrders;

		}

		public List<SystemCodeInfo> GetResults(long REQPrimaryId, long WCCPrimaryId) {

			SqliteConnection conn = null;
			List<SystemCodeInfo> results = new List<SystemCodeInfo> ();
			try {
				conn = MobileTech.Consts.GetDBConnection ();

				string sqlUsrSelect = @"SELECT C.RESPrimaryId, ResultCode, [Description] FROM Codes_ValidRequestsResults C
											INNER JOIN Codes_Result ON C.RESPrimaryId = Codes_Result.RESPrimaryId
											INNER JOIN Codes_ResultCenter ON C.RESPrimaryId = Codes_ResultCenter.RESPrimaryId
											WHERE C.REQPrimaryId = @REQPrimaryId AND C.WCCPrimaryId = @WCCPrimaryId AND 
												Codes_ResultCenter.WCCPrimaryId = @WCCPrimaryId	AND Codes_ResultCenter.Archived = ''
											ORDER BY Description";

				using (SqliteCommand sqlUsrSelectQry = conn.CreateCommand()) {
					sqlUsrSelectQry.CommandText = sqlUsrSelect;

					sqlUsrSelectQry.Parameters.AddWithValue("@REQPrimaryId", REQPrimaryId);
					sqlUsrSelectQry.Parameters.AddWithValue("@WCCPrimaryId", WCCPrimaryId);

					using (SqliteDataReader rReader = sqlUsrSelectQry.ExecuteReader()) {
						while (rReader.Read()) {
							SystemCodeInfo resultObj = new SystemCodeInfo();

							resultObj.PrimaryId = Convert.ToInt64(rReader.GetValue (rReader.GetOrdinal ("RESPrimaryId")));
							resultObj.Code = Convert.ToInt16(rReader.GetValue(rReader.GetOrdinal("ResultCode")));
							resultObj.Description = Convert.ToString(rReader.GetValue(rReader.GetOrdinal("Description")));

							results.Add(resultObj);
						}
					}
				}
			}catch (Exception ex) {
				//Console.WriteLine ("Exception in GetResults : " + ex.Message);
				string strLogMessage = string.Format("{0} GetResults Exception: {1}", DateTime.Now, ex.Message);
				Consts.HandleExceptionLog(strLogMessage);
			} finally {
				if (conn != null)
					MobileTech.Consts.ReleaseDBConnection (conn);
			}

			return results;
		}

		public List<SystemCodeInfo> GetFaults(long WCCPrimaryId) {

			SqliteConnection conn = null;
			List<SystemCodeInfo> faults = new List<SystemCodeInfo> ();
			try {
				conn = MobileTech.Consts.GetDBConnection ();

				string sqlUsrSelect = @"SELECT Codes_Fault.FLTPrimaryId, FaultCode, [Description] FROM Codes_Fault
											INNER JOIN Codes_FaultCenter ON Codes_Fault.FLTPrimaryId = Codes_FaultCenter.FLTPrimaryId
											WHERE Codes_FaultCenter.WCCPrimaryId = @WCCPrimaryId AND
											Codes_FaultCenter.Archived = '' ORDER BY Description";

				using (SqliteCommand sqlUsrSelectQry = conn.CreateCommand()) {
					sqlUsrSelectQry.CommandText = sqlUsrSelect;

					sqlUsrSelectQry.Parameters.AddWithValue("@WCCPrimaryId", WCCPrimaryId);

					using (SqliteDataReader rReader = sqlUsrSelectQry.ExecuteReader()) {
						while (rReader.Read()) {
							SystemCodeInfo faultObj = new SystemCodeInfo();

							faultObj.PrimaryId = Convert.ToInt64(rReader.GetValue (rReader.GetOrdinal ("FLTPrimaryId")));
							faultObj.Code = Convert.ToInt16(rReader.GetValue(rReader.GetOrdinal("FaultCode")));
							faultObj.Description = Convert.ToString(rReader.GetValue(rReader.GetOrdinal("Description")));

							faults.Add(faultObj);
						}
					}
				}
			}catch (Exception ex) {
				//Console.WriteLine ("Exception in GetFaults : " + ex.Message);
				string strLogMessage = string.Format("{0} GetFaults Exception: {1}", DateTime.Now, ex.Message);
				Consts.HandleExceptionLog(strLogMessage);
			} finally {
				if (conn != null)
					MobileTech.Consts.ReleaseDBConnection (conn);
			}

			return faults;
		}

		public List<SystemCode> GetOpenWorkStatuses(int latestRequestActionCode) {

			string sqlSelectQuery = string.Empty;
			switch (latestRequestActionCode) {
//			case (short)Consts.enumRequestsAction.None://AE Work Orders will have not having any codes, as we are saving 0 instead null.
//				sqlSelectQuery = @"SELECT OWSPrimaryId, OpenStatusCode, Description, RequestsActionCode FROM Codes_OpenWorkStatus ORDER BY Description";
//				break;

			case (short)Consts.enumRequestsAction.Accept:
				sqlSelectQuery = String.Format(@"SELECT OWSPrimaryId, OpenStatusCode, Description,RequestsActionCode FROM Codes_OpenWorkStatus 
									WHERE RequestsActionCode In ({0}) ORDER BY Description", (short)Consts.enumRequestsAction.Start);
				break;
			
			case (short)Consts.enumRequestsAction.Resume:
			case (short)Consts.enumRequestsAction.Start:
				sqlSelectQuery = String.Format(@"SELECT OWSPrimaryId, OpenStatusCode, Description,RequestsActionCode FROM Codes_OpenWorkStatus 
									WHERE RequestsActionCode In ({0}, {1}) ORDER BY Description", (short)Consts.enumRequestsAction.Delay, (short)Consts.enumRequestsAction.Finish);
				break;

			case (short)Consts.enumRequestsAction.Delay:
				sqlSelectQuery = String.Format(@"SELECT OWSPrimaryId, OpenStatusCode, Description,RequestsActionCode FROM Codes_OpenWorkStatus 
									WHERE RequestsActionCode In ({0}, {1}) ORDER BY Description", (short)Consts.enumRequestsAction.Start, (short)Consts.enumRequestsAction.Resume);
				break;

			default://if an action code is not in (Accept, Start, Delay, Resume) then this is a new dispatch...
				//Before Start Action applied different action codes will be there, so we can identify this is new dispatch and not yet added start action.
				sqlSelectQuery = String.Format(@"SELECT OWSPrimaryId, OpenStatusCode, Description,RequestsActionCode FROM Codes_OpenWorkStatus 
									WHERE RequestsActionCode In ({0}, {1}) ORDER BY Description", (short)Consts.enumRequestsAction.Accept, (short)Consts.enumRequestsAction.Decline);
				break;

			}

			SqliteConnection conn = null;
			List<SystemCode> owStatuses = new List<SystemCode> ();
			try {
				conn = MobileTech.Consts.GetDBConnection ();

				//string sqlUsrSelect = @"SELECT OWSPrimaryId, OpenStatusCode, Description, RequestsActionCode FROM Codes_OpenWorkStatus ORDER BY Description";

				using (SqliteCommand sqlUsrSelectQry = conn.CreateCommand()) {
					sqlUsrSelectQry.CommandText = sqlSelectQuery;

					using (SqliteDataReader rReader = sqlUsrSelectQry.ExecuteReader()) {
						while (rReader.Read()) {
							SystemCode owStatus = new SystemCode();

							owStatus.PrimaryId = Convert.ToInt64(rReader.GetValue (rReader.GetOrdinal ("OWSPrimaryId")));
							owStatus.Code = Convert.ToInt16(rReader.GetValue(rReader.GetOrdinal("OpenStatusCode")));
							owStatus.Description = Convert.ToString(rReader.GetValue(rReader.GetOrdinal("Description")));
							owStatus.RequestsActionCode = Convert.ToInt16(rReader.GetValue(rReader.GetOrdinal("RequestsActionCode")));
							owStatuses.Add(owStatus);
						}
					}
				}
			}catch (Exception ex) {
				string strLogMessage = string.Format("{0} GetOpenWorkStatuses Exception: {1}", DateTime.Now, ex.Message);
				Consts.HandleExceptionLog(strLogMessage);
			} finally {
				if (conn != null)
					MobileTech.Consts.ReleaseDBConnection (conn);
			}

			return owStatuses;
		}


		public void UpdateWorkOrder(WorkOrderDetails woDetails)
		{
			const string sqlStmt_UpdateWorkorder = @"Update WkOrders Set
										   RESPrimaryId = @RESPrimaryId, FLTPrimaryId = @FLTPrimaryId, OWSPrimaryId = @OWSPrimaryId, SFTest=@SFTest 
                                           ,DateStarted = @DateStarted, DateCompleted = @DateCompleted, OpenFlag = @OpenFlag, 
											LatestDispatchActionCode = @LatestDispatchActionCode
										    Where WORPrimaryId=@WORPrimaryId";

			const string sqlStmt_UpdateWOText = @"Update WoText Set TextField = @TextField 
 											 		Where WORPrimaryId=@WORPrimaryId";

			const string sqlStmt_InsertWOTime = @"Insert Into WoTime(WOMPrimaryid, WORPrimaryid, LogDate, WKRPrimaryId, WTBPrimaryId, ActualTime, Notes) 
													Values(@WOMPrimaryid, @WORPrimaryid, @LogDate, @WKRPrimaryId, @WTBPrimaryId, @ActualTime, @Notes)";

			//const string sqlStmt_UpdatePDADBID = @"Update PDADBID Set WOMPrimaryid = @WOMPrimaryid";

			SqliteConnection conn = null;
			SqliteTransaction tr = null;
			try {
				conn = Consts.GetDBConnection();

				using(tr = conn.BeginTransaction())
				{
					//Updating WorkOrder table
					using (SqliteCommand sqlQry = conn.CreateCommand()) {

						sqlQry.CommandText = sqlStmt_UpdateWorkorder;
						sqlQry.Parameters.Add ("@WORPrimaryId",DbType.UInt64);
						sqlQry.Parameters.Add ("@RESPrimaryId",DbType.UInt64);
						sqlQry.Parameters.Add ("@FLTPrimaryId",DbType.UInt64);
						sqlQry.Parameters.Add ("@OWSPrimaryId",DbType.UInt64);
						sqlQry.Parameters.Add ("@SFTest", DbType.String);
						sqlQry.Parameters.Add ("@DateStarted",DbType.DateTime);
						sqlQry.Parameters.Add ("@DateCompleted",DbType.DateTime);
						sqlQry.Parameters.Add ("@OpenFlag",DbType.UInt16);
						sqlQry.Parameters.Add ("@LatestDispatchActionCode",DbType.UInt16);

						#region Parameters Assigning
						sqlQry.Parameters ["@WORPrimaryId"].Value = woDetails.WorPrimaryId; 
						sqlQry.Parameters ["@RESPrimaryId"].Value = woDetails.Result.PrimaryId;
						sqlQry.Parameters ["@FLTPrimaryId"].Value = woDetails.Fault.PrimaryId;
						sqlQry.Parameters ["@OWSPrimaryId"].Value = woDetails.OpenWorkOrderStatus.PrimaryId;
						sqlQry.Parameters ["@SFTest"].Value = woDetails.SafetyTest;
						sqlQry.Parameters ["@DateStarted"].Value = woDetails.StartDate == DateTime.MinValue ? Convert.DBNull : woDetails.StartDate;
						sqlQry.Parameters ["@DateCompleted"].Value = woDetails.CompleteDate == DateTime.MinValue ? Convert.DBNull : woDetails.CompleteDate;
						sqlQry.Parameters ["@OpenFlag"].Value = woDetails.OpenFlag;
						sqlQry.Parameters ["@LatestDispatchActionCode"].Value = woDetails.LatestDispatchActionCode;
						#endregion

						sqlQry.ExecuteNonQuery ();

					}

					//Updating WOText table
					if(woDetails.WOTextNotes!=null)
					{
						using (SqliteCommand sqlQry = conn.CreateCommand()) {
							sqlQry.CommandText = sqlStmt_UpdateWOText;
							sqlQry.Parameters.Add ("@WORPrimaryId",DbType.UInt64);
							sqlQry.Parameters.Add ("@TextField", DbType.Binary);

							#region Parameters Assigning
							sqlQry.Parameters ["@WORPrimaryId"].Value = woDetails.WorPrimaryId; 
							System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
							sqlQry.Parameters ["@TextField"].Value = encoding.GetBytes(woDetails.WOTextNotes);
							#endregion

							sqlQry.ExecuteNonQuery ();
						}
					}

					//Insert WOTime table
					if(woDetails.TimeEntries!=null)
					{
						using (SqliteCommand sqlQry = conn.CreateCommand()) {
							sqlQry.CommandText = sqlStmt_InsertWOTime;
							sqlQry.Parameters.Add ("@WOMPrimaryId",DbType.UInt64);
							sqlQry.Parameters.Add ("@WORPrimaryid",DbType.UInt64);
							sqlQry.Parameters.Add ("@LogDate", DbType.DateTime);
							sqlQry.Parameters.Add ("@WKRPrimaryId", DbType.UInt64);
							sqlQry.Parameters.Add ("@WTBPrimaryId", DbType.UInt64);
							sqlQry.Parameters.Add ("@ActualTime", DbType.Decimal);
							sqlQry.Parameters.Add ("@Notes", DbType.Binary);

							for(int i=0; i< woDetails.TimeEntries.Length; i++)
							{
								WorkOrderTime woTimeObj = woDetails.TimeEntries[i];
								if(woTimeObj == null)
									continue;
								sqlQry.Parameters ["@WOMPrimaryId"].Value = woTimeObj.PrimaryId; 
								sqlQry.Parameters ["@WORPrimaryid"].Value = woDetails.WorPrimaryId; 
								sqlQry.Parameters ["@LogDate"].Value = woTimeObj.LogDate;
								sqlQry.Parameters ["@WKRPrimaryId"].Value = MobileTech.Consts.WKRPrimaryId;
								sqlQry.Parameters ["@WTBPrimaryId"].Value = woTimeObj.TimeType.PrimaryId;
								sqlQry.Parameters ["@ActualTime"].Value = woTimeObj.ActualTime;
								sqlQry.Parameters ["@Notes"].Value = woTimeObj.Notes;

								sqlQry.ExecuteNonQuery ();
							}

						}
					}

					//Update PDADBID Table
//					using (SqliteCommand sqlQry = conn.CreateCommand()) {
//						sqlQry.CommandText = sqlStmt_UpdatePDADBID;
//						sqlQry.Parameters.Add ("@WOMPrimaryid",DbType.UInt64);
//
//						#region Parameters Assigning
//						sqlQry.Parameters ["@WOMPrimaryid"].Value = PrimaryId; 
//						#endregion
//
//						sqlQry.ExecuteNonQuery ();
//					}

					tr.Commit();
				}


			} catch (Exception ex) {
				string strLogMessage = string.Format("{0} UpdateWorkOrder Exception: {1}", DateTime.Now, ex.Message);
				Consts.HandleExceptionLog(strLogMessage);

				if(tr!=null)
					tr.Dispose();

//				if (tr != null)
//				{
//					try 
//					{
//						tr.Rollback();
//
//					} catch (SqliteException ex2)
//					{
//
//						Console.WriteLine("Transaction rollback failed.");
//						Console.WriteLine("Error: {0}",  ex2.ToString());
//
//					} 
//					finally
//					{
//						tr.Dispose();
//					}
//				}


			} finally {
				if(conn != null)
					Consts.ReleaseDBConnection(conn);
			}

		}
		public WorkOrderDetails GetWorkOrderDetails(WorkOrderDetails woDetails)
		{
			SqliteConnection conn = null;

			try {
				conn = MobileTech.Consts.GetDBConnection ();

				string sqlUsrSelect = @"Select WorkOrder, REQPrimaryid, RESPrimaryid, WKRPrimaryId, ACCPrimaryId, SFTest, FLTPrimaryId, O.OWSPrimaryId, O.Description, 
											WoText.TextField, DateIssue, DateStarted, DateCompleted, OpenFlag, LatestDispatchActionCode, canWOBeClosed
                                            From WkOrders 
											Left Outer Join Codes_OpenWorkStatus O on WkOrders.OWSPrimaryId = O.OWSPrimaryId 
											Left Outer Join Wotext on WkOrders.WORPrimaryId = WoText.WORPrimaryId
											Where WkOrders.WORPrimaryId = @WORPrimaryid";

				using (SqliteCommand sqlUsrSelectQry = conn.CreateCommand()) {
					sqlUsrSelectQry.CommandText = sqlUsrSelect;
					sqlUsrSelectQry.Parameters.AddWithValue("@WORPrimaryId", woDetails.WorPrimaryId);

					using (SqliteDataReader rReader = sqlUsrSelectQry.ExecuteReader()) {
						if (rReader.Read()) {
							woDetails.WorPrimaryId = Convert.ToInt64(woDetails.WorPrimaryId);
							woDetails.Request = new RequestInfo();
							woDetails.Result = new SystemCodeInfo();
							woDetails.Fault = new SystemCodeInfo();
							woDetails.Worker = new WorkerInfo();
							woDetails.Account = new Account();
							woDetails.OpenWorkOrderStatus = new SystemCode();

							woDetails.Number=Convert.ToString(rReader.GetValue (rReader.GetOrdinal ("WorkOrder")));
							woDetails.Request.PrimaryId = Convert.ToInt64(rReader.GetValue (rReader.GetOrdinal ("REQPrimaryid")));
							woDetails.Result.PrimaryId = Convert.ToInt64(rReader.GetValue (rReader.GetOrdinal ("RESPrimaryid")));
							woDetails.Fault.PrimaryId = Convert.ToInt64(rReader.GetValue (rReader.GetOrdinal ("FLTPrimaryId")));
							woDetails.Worker.WKRPrimaryId = Convert.ToInt64(rReader.GetValue (rReader.GetOrdinal ("WKRPrimaryId")));
							woDetails.Account.AccPrimaryId = Convert.ToInt64(rReader.GetValue (rReader.GetOrdinal ("ACCPrimaryId")));

							woDetails.SafetyTest = Convert.ToString(rReader.GetValue (rReader.GetOrdinal ("SFTest")));
							woDetails.OpenWorkOrderStatus.PrimaryId = Convert.ToInt64(rReader.IsDBNull(rReader.GetOrdinal("OWSPrimaryId")) ? 0 : rReader.GetValue(rReader.GetOrdinal("OWSPrimaryId")));

							woDetails.OpenWorkOrderStatus.Description = Convert.ToString(rReader.GetValue (rReader.GetOrdinal ("Description")));

							woDetails.IssueDate = Convert.ToDateTime(rReader.IsDBNull(rReader.GetOrdinal("DateIssue")) ? DateTime.MinValue : rReader.GetValue(rReader.GetOrdinal("DateIssue")) );

							woDetails.StartDate = Convert.ToDateTime(rReader.IsDBNull(rReader.GetOrdinal("DateStarted")) ? DateTime.MinValue : rReader.GetValue(rReader.GetOrdinal("DateStarted")) );

							woDetails.CompleteDate = Convert.ToDateTime(rReader.IsDBNull(rReader.GetOrdinal("DateCompleted")) ? DateTime.MinValue : rReader.GetValue(rReader.GetOrdinal("DateCompleted")) );

							woDetails.ApplicationName = "mTech.iOS";
							woDetails.OpenFlag = Convert.ToByte(1);

							System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();

							if(rReader.GetValue(rReader.GetOrdinal("TextField")) != DBNull.Value){
								woDetails.WOTextNotes = encoding.GetString((byte []) rReader.GetValue(rReader.GetOrdinal("TextField")));
							}


							//woDetails.OpenFlag = Convert.ToByte(rReader.GetValue (rReader.GetOrdinal ("OpenFlag")));

							woDetails.LatestDispatchActionCode = Convert.ToInt16(rReader.GetValue (rReader.GetOrdinal ("LatestDispatchActionCode")));

							woDetails.canWOBeClosed = Convert.ToByte(rReader.GetValue (rReader.GetOrdinal ("canWOBeClosed"))); 
						}
					}
				}
			}catch (Exception ex) {
				string strLogMessage = string.Format("{0} GetWorkOrderDetails Exception: {1}", DateTime.Now, ex.Message);
				Consts.HandleExceptionLog(strLogMessage);
			} finally {
				if (conn != null)
					MobileTech.Consts.ReleaseDBConnection (conn);
			}

			return woDetails;
		}

		public WorkOrderDetails GetWorkOrderDetails(long WORPrimaryId)
		{
			SqliteConnection conn = null;

			WorkOrderDetails woDetails = new WorkOrderDetails();
			try {
				conn = MobileTech.Consts.GetDBConnection ();

//				string sqlUsrSelect = @"Select WorkOrder, REQPrimaryid, RESPrimaryid, SFTest, FLTPrimaryId, O.Description from WkOrders
//											left outer Join Codes_OpenWorkStatus O on WkOrders.OWSPrimaryId = O.OWSPrimaryId
//											Where WORPrimaryid = @WORPrimaryid";
				 
				string sqlUsrSelect = @"Select WkOrders.WORPrimaryId, WorkOrder, REQPrimaryid, RESPrimaryid, SFTest, FLTPrimaryId, O.OWSPrimaryId, O.Description, WoText.TextField 
                                            ,DateStarted, DateCompleted, OpenFlag,WKRPrimaryID
                                            From WkOrders 
											Left Outer Join Codes_OpenWorkStatus O on WkOrders.OWSPrimaryId = O.OWSPrimaryId 
											Left Outer Join Wotext on WkOrders.WORPrimaryId = WoText.WORPrimaryId
											Where WkOrders.WORPrimaryId = @WORPrimaryid";

				string sqlSelectTimeEntries = "Select WOMPrimaryid, LogDate, WKRPrimaryId, WTBPrimaryId, ActualTime, Notes " +
					"From WoTime Where WORPrimaryId = @WORPrimaryId";

				using (SqliteCommand sqlUsrSelectQry = conn.CreateCommand()) {
					sqlUsrSelectQry.CommandText = sqlUsrSelect;
					sqlUsrSelectQry.Parameters.AddWithValue("@WORPrimaryId", WORPrimaryId);

					using (SqliteDataReader rReader = sqlUsrSelectQry.ExecuteReader()) {
						if (rReader.Read()) {
							woDetails.WorPrimaryId = Convert.ToInt64(WORPrimaryId);
							woDetails.Request = new RequestInfo();
							woDetails.Result = new SystemCodeInfo();
							woDetails.Fault = new SystemCodeInfo();
							woDetails.OpenWorkOrderStatus = new SystemCode();
							woDetails.Worker = new WorkerInfo();

							woDetails.WorPrimaryId = Convert.ToInt64(rReader.GetValue (rReader.GetOrdinal ("WORPrimaryId")));
							woDetails.Number=Convert.ToString(rReader.GetValue (rReader.GetOrdinal ("WorkOrder")));
							woDetails.Request.PrimaryId = Convert.ToInt64(rReader.GetValue (rReader.GetOrdinal ("REQPrimaryid")));
							woDetails.Result.PrimaryId = Convert.ToInt64(rReader.GetValue (rReader.GetOrdinal ("RESPrimaryid")));
							woDetails.Fault.PrimaryId = Convert.ToInt64(rReader.GetValue (rReader.GetOrdinal ("FLTPrimaryId")));
							woDetails.Worker.WKRPrimaryId = Convert.ToInt64(rReader.GetValue (rReader.GetOrdinal ("WKRPrimaryID")));
							woDetails.SafetyTest = Convert.ToString(rReader.GetValue (rReader.GetOrdinal ("SFTest")));
							woDetails.OpenWorkOrderStatus.PrimaryId =   rReader.GetValue (rReader.GetOrdinal ("OWSPrimaryId")) == DBNull.Value ? Convert.ToInt64(0) : Convert.ToInt64(rReader.GetValue (rReader.GetOrdinal ("OWSPrimaryId")));
							woDetails.OpenWorkOrderStatus.Description = Convert.ToString(rReader.GetValue (rReader.GetOrdinal ("Description")));

							woDetails.StartDate = Convert.ToDateTime(rReader.IsDBNull(rReader.GetOrdinal("DateStarted")) ? DateTime.MinValue : rReader.GetValue(rReader.GetOrdinal("DateStarted")) );

							woDetails.CompleteDate = Convert.ToDateTime(rReader.IsDBNull(rReader.GetOrdinal("DateCompleted")) ? DateTime.MinValue : rReader.GetValue(rReader.GetOrdinal("DateCompleted")) );

							woDetails.ApplicationName = "mTech.iOS";

							System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();

							if(rReader.GetValue(rReader.GetOrdinal("TextField")) != DBNull.Value){
								woDetails.WOTextNotes = encoding.GetString((byte []) rReader.GetValue(rReader.GetOrdinal("TextField")));
							}
							woDetails.OpenFlag = Convert.ToByte(rReader.GetValue (rReader.GetOrdinal ("OpenFlag"))); 
						}
					}
				}

				using (SqliteCommand sqlUsrSelectQry = conn.CreateCommand()) {
					sqlUsrSelectQry.CommandText = sqlSelectTimeEntries;
					sqlUsrSelectQry.Parameters.AddWithValue("@WORPrimaryId", WORPrimaryId);

					using (SqliteDataReader rReader = sqlUsrSelectQry.ExecuteReader()) {
						while (rReader.Read()) {
							WorkOrderTime woTime = new WorkOrderTime();
//							woDetails.TimeEntries = new WorkOrderTime[count+1];
//							woDetails.TimeEntries[count] = new WorkOrderTime();
							woTime.TimeType = new SystemCode();
							woTime.Worker = new Worker();
							//woDetails.WorPrimaryId = Convert.ToInt64(WORPrimaryId);
							woTime.PrimaryId=Convert.ToInt64(rReader.GetValue (rReader.GetOrdinal ("WOMPrimaryid")));
							woTime.LogDate = Convert.ToDateTime(rReader.GetValue (rReader.GetOrdinal ("LogDate")));
							woTime.Worker.WKRPrimaryId = Convert.ToInt64(rReader.GetValue (rReader.GetOrdinal ("WKRPrimaryId")));
							woTime.TimeType.PrimaryId = Convert.ToInt64(rReader.GetValue (rReader.GetOrdinal ("WTBPrimaryId")));
							woTime.ActualTime = Convert.ToDecimal(rReader.GetValue (rReader.GetOrdinal ("ActualTime")));
							//woDetails.TimeEntries[count].Notes = (byte [])(rReader.GetValue (rReader.GetOrdinal ("Notes")));
							//System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
							//woDetails.TimeEntries[count].strNotes = encoding.GetString((byte [])(rReader.GetValue (rReader.GetOrdinal ("Notes"))));
							int count =0;
							WorkOrderTime[] woTimeEntry = woDetails.TimeEntries;
							if (woTimeEntry != null)
								count = woDetails.TimeEntries.Length;

							Array.Resize<WorkOrderTime> (ref woTimeEntry, count + 1);
							woTimeEntry [count] = woTime;
							woDetails.TimeEntries = woTimeEntry;

						
//							woDetails.TimeEntries = new WorkOrderTime[count+1];
//							woDetails.TimeEntries[count] = new WorkOrderTime();
//							woDetails.TimeEntries[count].TimeType = new SystemCode();
//							woDetails.TimeEntries[count].Worker = new Worker();
//							//woDetails.WorPrimaryId = Convert.ToInt64(WORPrimaryId);
//							woDetails.TimeEntries[count].PrimaryId=Convert.ToInt64(rReader.GetValue (rReader.GetOrdinal ("WOMPrimaryid")));
//							woDetails.TimeEntries[count].LogDate = Convert.ToDateTime(rReader.GetValue (rReader.GetOrdinal ("LogDate")));
//							woDetails.TimeEntries[count].Worker.WKRPrimaryId = Convert.ToInt64(rReader.GetValue (rReader.GetOrdinal ("WKRPrimaryId")));
//							woDetails.TimeEntries[count].TimeType.PrimaryId = Convert.ToInt64(rReader.GetValue (rReader.GetOrdinal ("WTBPrimaryId")));
//							woDetails.TimeEntries[count].ActualTime = Convert.ToDecimal(rReader.GetValue (rReader.GetOrdinal ("ActualTime")));
//							//woDetails.TimeEntries[count].Notes = (byte [])(rReader.GetValue (rReader.GetOrdinal ("Notes")));
//							//System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
//							//woDetails.TimeEntries[count].strNotes = encoding.GetString((byte [])(rReader.GetValue (rReader.GetOrdinal ("Notes"))));

						
						
						}
					}
				}
			}catch (Exception ex) {
				string strLogMessage = string.Format("{0} GetWorkOrderDetails Exception: {1}", DateTime.Now, ex.Message);
				Consts.HandleExceptionLog(strLogMessage);
			} finally {
				if (conn != null)
					MobileTech.Consts.ReleaseDBConnection (conn);
			}

			return woDetails;
		}


		public PDADBID GetPDADBIDDetails()
		{
			SqliteConnection conn = null;
			PDADBID pdadbid = new PDADBID ();

			try {
				conn = MobileTech.Consts.GetDBConnection ();

				string sqlSelectPDADBID = @"Select username, WORPrimaryid, WOPPrimaryid, WOMPrimaryid, PPOPrimaryid, 
												UDDPrimaryID, WOTPrimaryID, WOMCHPrimaryid, WOMCRPrimaryid 
												From PDADbid ";

				using (SqliteCommand sqlUsrSelectQry = conn.CreateCommand()) {
					sqlUsrSelectQry.CommandText = sqlSelectPDADBID;

					using (SqliteDataReader rReader = sqlUsrSelectQry.ExecuteReader()) {
						if (rReader.Read()) {
							pdadbid.username = Convert.ToString(rReader.GetValue(rReader.GetOrdinal("username")));
							pdadbid.WORPrimaryId = Convert.ToInt64(rReader.GetValue (rReader.GetOrdinal ("WORPrimaryid")));
							pdadbid.WOPPrimaryid = Convert.ToInt64(rReader.GetValue (rReader.GetOrdinal ("WOPPrimaryid")));
							pdadbid.WOMPrimaryid = Convert.ToInt64(rReader.GetValue (rReader.GetOrdinal ("WOMPrimaryid")));
							pdadbid.PPOPrimaryid = Convert.ToInt64(rReader.GetValue (rReader.GetOrdinal ("PPOPrimaryid")));
							pdadbid.UDDPrimaryid = Convert.ToInt64(rReader.GetValue (rReader.GetOrdinal ("UDDPrimaryID")));
							pdadbid.WOTPrimaryid = Convert.ToInt64(rReader.GetValue (rReader.GetOrdinal ("WOTPrimaryID")));
							pdadbid.WOMCHPrimaryid = Convert.ToInt64(rReader.GetValue (rReader.GetOrdinal ("WOMCHPrimaryid")));
							pdadbid.WOMCRPrimaryid = Convert.ToInt64(rReader.GetValue (rReader.GetOrdinal ("WOMCRPrimaryid")));
						}
					}
				}

			}catch (Exception ex) {
				string strLogMessage = string.Format("{0} GetPDADBIDDetails Exception: {1}", DateTime.Now, ex.Message);
				Consts.HandleExceptionLog(strLogMessage);
			} finally {
				if (conn != null)
					MobileTech.Consts.ReleaseDBConnection (conn);
			}

			return pdadbid;
		}

		public WorkOrderText GetWOText(long WORPrimaryId) {

			SqliteConnection conn = null;

			WorkOrderText woText = new WorkOrderText ();
			try {
				conn = MobileTech.Consts.GetDBConnection ();

				string sqlUsrSelect = @"SELECT WORPrimaryId, TextField From WOText Where WORPrimaryId = @WORPrimaryId";

				using (SqliteCommand sqlUsrSelectQry = conn.CreateCommand()) {
					sqlUsrSelectQry.CommandText = sqlUsrSelect;
					sqlUsrSelectQry.Parameters.AddWithValue("@WORPrimaryId", WORPrimaryId);

					using (SqliteDataReader rReader = sqlUsrSelectQry.ExecuteReader()) {
						if (rReader.Read()) {
							woText.WorkOrder = new WorkOrder();

							woText.WorkOrder.WorPrimaryId = Convert.ToInt64(rReader.GetValue (rReader.GetOrdinal ("WORPrimaryId")));
							woText.Text = (byte []) rReader.GetValue(rReader.GetOrdinal("TextField"));
						}
					}
				}
			}catch (Exception ex) {
				string strLogMessage = string.Format("{0} GetWOText Exception: {1}", DateTime.Now, ex.Message);
				Consts.HandleExceptionLog(strLogMessage);
			} finally {
				if (conn != null)
					MobileTech.Consts.ReleaseDBConnection (conn);
			}

			return woText;
		}

		public SystemCodeInfo GetResultForID(long WORPrimaryId) {
			SqliteConnection conn = null;
			SystemCodeInfo resultObj = new SystemCodeInfo();
			try {
				conn = MobileTech.Consts.GetDBConnection ();

				string sqlUsrSelect = @"SELECT Codes_Result.RESPrimaryId, ResultCode, [Description] 
											FROM Codes_Result 
											Inner Join WkOrders on codes_Result.RESPrimaryId = WkOrders.RESPrimaryId
											WHERE WkOrders.WORPrimaryId = @WORPrimaryId";

				using (SqliteCommand sqlUsrSelectQry = conn.CreateCommand()) {
					sqlUsrSelectQry.CommandText = sqlUsrSelect;

					sqlUsrSelectQry.Parameters.AddWithValue("@WORPrimaryId", WORPrimaryId);

					using (SqliteDataReader rReader = sqlUsrSelectQry.ExecuteReader()) {
						if (rReader.Read()) {
							resultObj.PrimaryId = Convert.ToInt64(rReader.GetValue (rReader.GetOrdinal ("RESPrimaryId")));
							resultObj.Code = Convert.ToInt16(rReader.GetValue(rReader.GetOrdinal("ResultCode")));
							resultObj.Description = Convert.ToString(rReader.GetValue(rReader.GetOrdinal("Description")));
						}
					}
				}
			}catch (Exception ex) {
				string strLogMessage = string.Format("{0} GetResultForID Exception: {1}", DateTime.Now, ex.Message);
				Consts.HandleExceptionLog(strLogMessage);
			} finally {
				if (conn != null)
					MobileTech.Consts.ReleaseDBConnection (conn);
			}

			return resultObj;
		}

		public SystemCodeInfo GetFaultForID(long WORPrimaryId) {
			SqliteConnection conn = null;
			SystemCodeInfo faultObj = new SystemCodeInfo();
			try {
				conn = MobileTech.Consts.GetDBConnection ();

				string sqlUsrSelect = @"SELECT Codes_Fault.FLTPrimaryId, FaultCode, [Description] 
											FROM Codes_Fault 
											Inner Join WkOrders on Codes_Fault.FLTPrimaryId = WkOrders.FLTPrimaryId
											WHERE WkOrders.WORPrimaryId = @WORPrimaryId";


				using (SqliteCommand sqlUsrSelectQry = conn.CreateCommand()) {
					sqlUsrSelectQry.CommandText = sqlUsrSelect;

					sqlUsrSelectQry.Parameters.AddWithValue("@WORPrimaryId", WORPrimaryId);

					using (SqliteDataReader rReader = sqlUsrSelectQry.ExecuteReader()) {
						if (rReader.Read()) {
							faultObj.PrimaryId = Convert.ToInt64(rReader.GetValue (rReader.GetOrdinal ("FLTPrimaryId")));
							faultObj.Code = Convert.ToInt16(rReader.GetValue(rReader.GetOrdinal("FaultCode")));
							faultObj.Description = Convert.ToString(rReader.GetValue(rReader.GetOrdinal("Description")));
						}
					}
				}
			}catch (Exception ex) {
				string strLogMessage = string.Format("{0} GetFaultForID Exception: {1}", DateTime.Now, ex.Message);
				Consts.HandleExceptionLog(strLogMessage);
			} finally {
				if (conn != null)
					MobileTech.Consts.ReleaseDBConnection (conn);
			}

			return faultObj;
		}

		public SystemCode GetOpenWorkStatusForID(long WORPrimaryId) {

			SqliteConnection conn = null;
			SystemCode owStatusObj = new SystemCode();
			try {
				conn = MobileTech.Consts.GetDBConnection ();

				string sqlUsrSelect = @"SELECT Codes_OpenWorkStatus.OWSPrimaryId, OpenStatusCode, Description, RequestsActionCode 
											FROM Codes_OpenWorkStatus 
											Inner Join WkOrders on Codes_OpenWorkStatus.OWSPrimaryId = WkOrders.OWSPrimaryId
											WHERE WkOrders.WORPrimaryId = @WORPrimaryId";

				using (SqliteCommand sqlUsrSelectQry = conn.CreateCommand()) {
					sqlUsrSelectQry.CommandText = sqlUsrSelect;

					sqlUsrSelectQry.Parameters.AddWithValue("@WORPrimaryId", WORPrimaryId);

					using (SqliteDataReader rReader = sqlUsrSelectQry.ExecuteReader()) {
						if (rReader.Read()) {
							owStatusObj.PrimaryId = Convert.ToInt64(rReader.GetValue (rReader.GetOrdinal ("OWSPrimaryId")));
							owStatusObj.Code = Convert.ToInt16(rReader.GetValue(rReader.GetOrdinal("OpenStatusCode")));
							owStatusObj.Description = Convert.ToString(rReader.GetValue(rReader.GetOrdinal("Description")));
							owStatusObj.RequestsActionCode = Convert.ToInt16(rReader.GetValue(rReader.GetOrdinal("RequestsActionCode")));
						}
					}
				}
			}catch (Exception ex) {
				string strLogMessage = string.Format("{0} GetOpenWorkStatusForID Exception: {1}", DateTime.Now, ex.Message);
				Consts.HandleExceptionLog(strLogMessage);
			} finally {
				if (conn != null)
					MobileTech.Consts.ReleaseDBConnection (conn);
			}

			return owStatusObj;
		}

		public bool isFaultCodeRequired(long REQPrimaryId, long WCCPrimaryId) {

			SqliteConnection conn = null;
			bool isFaultCodeStatusRequired = false;
			try {
				conn = MobileTech.Consts.GetDBConnection ();

				string sqlUsrSelect = @"Select RequireFaultCode From Codes_RequestCenter Where REQPrimaryId = @REQPrimaryId AND WCCPrimaryId = @WCCPrimaryId";

				using (SqliteCommand sqlUsrSelectQry = conn.CreateCommand()) {
					sqlUsrSelectQry.CommandText = sqlUsrSelect;

					sqlUsrSelectQry.Parameters.AddWithValue("@REQPrimaryId", REQPrimaryId);
					sqlUsrSelectQry.Parameters.AddWithValue("@WCCPrimaryId", WCCPrimaryId);

					using (SqliteDataReader rReader = sqlUsrSelectQry.ExecuteReader()) {
						if (rReader.Read()) {
							if(Convert.ToString(rReader.GetValue(rReader.GetOrdinal("RequireFaultCode"))).ToLower()=="yes")
								isFaultCodeStatusRequired = true;
						}
					}
				}
			}catch (Exception ex) {
				//Console.WriteLine ("Exception in GetResults : " + ex.Message);
				string strLogMessage = string.Format("{0} isFaultCodeRequired Exception: {1}", DateTime.Now, ex.Message);
				Consts.HandleExceptionLog(strLogMessage);
			} finally {
				if (conn != null)
					MobileTech.Consts.ReleaseDBConnection (conn);
			}

			return isFaultCodeStatusRequired;
		}

		public bool isControlRequired(long REQPrimaryId, long WCCPrimaryId) {

			SqliteConnection conn = null;
			bool isWOControlRequired = false;
			try {
				conn = MobileTech.Consts.GetDBConnection ();

				string sqlUsrSelect = @"Select RequireControl From Codes_RequestCenter Where REQPrimaryId = @REQPrimaryId AND WCCPrimaryId = @WCCPrimaryId";

				using (SqliteCommand sqlUsrSelectQry = conn.CreateCommand()) {
					sqlUsrSelectQry.CommandText = sqlUsrSelect;

					sqlUsrSelectQry.Parameters.AddWithValue("@REQPrimaryId", REQPrimaryId);
					sqlUsrSelectQry.Parameters.AddWithValue("@WCCPrimaryId", WCCPrimaryId);

					using (SqliteDataReader rReader = sqlUsrSelectQry.ExecuteReader()) {
						if (rReader.Read()) {
							if(Convert.ToString(rReader.GetValue(rReader.GetOrdinal("RequireControl"))).ToLower()=="yes")
								isWOControlRequired = true;
						}
					}
				}
			}catch (Exception ex) {
				//Console.WriteLine ("Exception in GetResults : " + ex.Message);
				string strLogMessage = string.Format("{0} isControlRequired Exception: {1}", DateTime.Now, ex.Message);
				Consts.HandleExceptionLog(strLogMessage);
			} finally {
				if (conn != null)
					MobileTech.Consts.ReleaseDBConnection (conn);
			}

			return isWOControlRequired;
		}

		public User ValidSessionCheck ()
		{
			User result = null;

			try
			{
				var request = HttpWebRequest.Create(string.Format(@"{0}/AuthenticationService/GetCurrentUser?appName=mTech.iOS",MobileTech.Consts.ServerURL));
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
							//Console.Out.WriteLine("Response contained empty body...");
						}
						else {
							result = (User)JsonConvert.DeserializeObject(content , typeof(User));
							Console.Out.WriteLine("Response Body: \r\n {0}", content);
						}
					}
				}
			}
			catch(Exception ex)
			{
				MobileTech.Consts.LogException (ex);
			}
			return result;
		}

		public List<SystemCode> GetTimeTypes() {

			SqliteConnection conn = null;
			List<SystemCode> timeTypes = new List<SystemCode> ();
			try {
				conn = MobileTech.Consts.GetDBConnection ();

				string sqlUsrSelect = @"SELECT WTBPrimaryid, Description From Codes_WorkOrderTimeType ORDER BY Description";

				using (SqliteCommand sqlUsrSelectQry = conn.CreateCommand()) {
					sqlUsrSelectQry.CommandText = sqlUsrSelect;

					using (SqliteDataReader rReader = sqlUsrSelectQry.ExecuteReader()) {
						while (rReader.Read()) {
							SystemCode timeTypeObj = new SystemCode();

							timeTypeObj.PrimaryId = Convert.ToInt64(rReader.GetValue (rReader.GetOrdinal ("WTBPrimaryid")));
							timeTypeObj.Description = Convert.ToString(rReader.GetValue(rReader.GetOrdinal("Description")));

							timeTypes.Add(timeTypeObj);
						}
					}
				}
			}catch (Exception ex) {
				//Console.WriteLine ("Exception in GetResults : " + ex.Message);
				string strLogMessage = string.Format("{0} GetTimeTypes Exception: {1}", DateTime.Now, ex.Message);
				Consts.HandleExceptionLog(strLogMessage);
			} finally {
				if (conn != null)
					MobileTech.Consts.ReleaseDBConnection (conn);
			}

			return timeTypes;
		}

		/*
		public PDADBID GetPDADBID() {

			SqliteConnection conn = null;
			PDADBID pdadbid = new PDADBID ();
			try {
				conn = MobileTech.Consts.GetDBConnection ();

				string sqlUsrSelect = @"Select username, Dbid, Devprimaryid, MKMDid, Souprimaryid, DVCPrimaryid, 
											EQCPrimaryid, MMCprimaryid, Equipid, WORPrimaryid, WOPPrimaryid, WOMPrimaryid, MESPrimaryid, 
											EORPrimaryID, EOHPrimaryID, PATPrimaryID, PACPrimaryID, PPOPrimaryid, SWCPrimaryID, 
											UDDPrimaryID, WOTPrimaryID, WOMCHPrimaryid, WOMCRPrimaryid 
										From PDADbid ";

				using (SqliteCommand sqlUsrSelectQry = conn.CreateCommand()) {
					sqlUsrSelectQry.CommandText = sqlUsrSelect;

					using (SqliteDataReader rReader = sqlUsrSelectQry.ExecuteReader()) {
						if (rReader.Read()) {
							pdadbid.username = Convert.ToString(rReader.GetValue(rReader.GetOrdinal("username")));
							pdadbid.Dbid = Convert.ToInt16(rReader.GetValue (rReader.GetOrdinal ("Dbid")));
							pdadbid.DevPrimaryid = Convert.ToInt64(rReader.GetValue (rReader.GetOrdinal ("Devprimaryid")));
							pdadbid.MKMDid = Convert.ToInt64(rReader.GetValue (rReader.GetOrdinal ("MKMDid")));
							pdadbid.SouPrimaryid = Convert.ToInt64(rReader.GetValue (rReader.GetOrdinal ("Souprimaryid")));
							//pdadbid.dvc = Convert.ToInt64(rReader.GetValue (rReader.GetOrdinal ("DVCPrimaryid")));
							//pdadbid.eq = Convert.ToInt64(rReader.GetValue (rReader.GetOrdinal ("EQCPrimaryid")));
							//pdadbid.mm = Convert.ToInt64(rReader.GetValue (rReader.GetOrdinal ("MMCprimaryid")));
							pdadbid.Equipid = Convert.ToInt64(rReader.GetValue (rReader.GetOrdinal ("Equipid")));
							pdadbid.WORPrimaryId = Convert.ToInt64(rReader.GetValue (rReader.GetOrdinal ("WORPrimaryid")));
							pdadbid.WOPPrimaryid = Convert.ToInt64(rReader.GetValue (rReader.GetOrdinal ("WOPPrimaryid")));
							pdadbid.WOMPrimaryid = Convert.ToInt64(rReader.GetValue (rReader.GetOrdinal ("WOMPrimaryid")));
							//pdadbid.me = Convert.ToInt64(rReader.GetValue (rReader.GetOrdinal ("MESPrimaryid")));
							//pdadbid.e = Convert.ToInt64(rReader.GetValue (rReader.GetOrdinal ("EORPrimaryID")));
							//pdadbid.PrimaryId = Convert.ToInt64(rReader.GetValue (rReader.GetOrdinal ("EOHPrimaryID")));
							//pdadbid.p = Convert.ToInt64(rReader.GetValue (rReader.GetOrdinal ("PATPrimaryID")));
							//pdadbid.pa = Convert.ToInt64(rReader.GetValue (rReader.GetOrdinal ("PACPrimaryID")));
							pdadbid.PPOPrimaryid = Convert.ToInt64(rReader.GetValue (rReader.GetOrdinal ("PPOPrimaryid")));
							//pdadbid.sw = Convert.ToInt64(rReader.GetValue (rReader.GetOrdinal ("SWCPrimaryID")));
							pdadbid.UDDPrimaryid = Convert.ToInt64(rReader.GetValue (rReader.GetOrdinal ("UDDPrimaryID")));
							pdadbid.WOTPrimaryid = Convert.ToInt64(rReader.GetValue (rReader.GetOrdinal ("WOTPrimaryID")));
							pdadbid.WOMCHPrimaryid = Convert.ToInt64(rReader.GetValue (rReader.GetOrdinal ("WOMCHPrimaryid")));
							pdadbid.WOMCRPrimaryid = Convert.ToInt64(rReader.GetValue (rReader.GetOrdinal ("WOMCRPrimaryid")));

						}
					}
				}
			}catch (Exception ex) {
				//Console.WriteLine ("Exception in GetResults : " + ex.Message);
				string strLogMessage = string.Format("{0} GetPDADBID Exception: {1}", DateTime.Now, ex.Message);
				Consts.HandleExceptionLog(strLogMessage);
			} finally {
				if (conn != null)
					MobileTech.Consts.ReleaseDBConnection (conn);
			}

			return pdadbid;
		}
		*/

		public long GetPDADBIDForRequest(string RequestId) {

			SqliteConnection conn = null;
			long pdadbid=0;
			try {
				conn = MobileTech.Consts.GetDBConnection ();

				string sqlUsrSelect = @"Select "+ RequestId +" From PDADbid ";

				using (SqliteCommand sqlUsrSelectQry = conn.CreateCommand()) {
					sqlUsrSelectQry.CommandText = sqlUsrSelect;

					using (SqliteDataReader rReader = sqlUsrSelectQry.ExecuteReader()) {
						if (rReader.Read()) {
							pdadbid = Convert.ToInt64(rReader.GetValue(0));
						}
					}
				}
			}catch (Exception ex) {
				//Console.WriteLine ("Exception in GetResults : " + ex.Message);
				string strLogMessage = string.Format("{0} GetPDADBIDForRequest {1} Exception: {2}", DateTime.Now, RequestId, ex.Message);
				Consts.HandleExceptionLog(strLogMessage);
			} finally {
				if (conn != null)
					MobileTech.Consts.ReleaseDBConnection (conn);
			}

			return pdadbid;
		}

		public void UpdatePDADBIDForRequest(string RequestId, long PrimaryId) {

			SqliteConnection conn = null;
			try {
				conn = MobileTech.Consts.GetDBConnection ();

				string sqlUpdatePDADBID = @"Update PDADBID Set "+ RequestId +" = @"+ RequestId ;

				//Updating PDADBID table
				using (SqliteCommand sqlQry = conn.CreateCommand()) {
					sqlQry.CommandText = sqlUpdatePDADBID;
					sqlQry.Parameters.Add ("@"+RequestId+"",DbType.UInt64);

					#region Parameters Assigning
					sqlQry.Parameters ["@"+RequestId+""].Value = PrimaryId; 
					#endregion

					sqlQry.ExecuteNonQuery ();
				}
			}catch (Exception ex) {
				//Console.WriteLine ("Exception in GetResults : " + ex.Message);
				string strLogMessage = string.Format("{0} UpdatePDADBIDForRequest {1} Exception: {2}", DateTime.Now, RequestId, ex.Message);
				Consts.HandleExceptionLog(strLogMessage);
			} finally {
				if (conn != null)
					MobileTech.Consts.ReleaseDBConnection (conn);
			}

		}

		public static bool isPDADBIDExists() {

			SqliteConnection conn = null;
			bool flag = false;
			try {
				conn = MobileTech.Consts.GetDBConnection ();

				string sqlUsrSelect = @"Select Count(*) From PDADbid ";

				using (SqliteCommand sqlUsrSelectQry = conn.CreateCommand()) {
					sqlUsrSelectQry.CommandText = sqlUsrSelect;

					object result = sqlUsrSelectQry.ExecuteScalar();

					if (result != null){
						if (System.Convert.ToInt32(result) > 0){
							flag = true;
						}
					}
				}
			}catch (Exception ex) {
				//Console.WriteLine ("Exception in GetResults : " + ex.Message);
				string strLogMessage = string.Format("{0} isPDADBIDExists Exception: {2}", DateTime.Now, ex.Message);
				Consts.HandleExceptionLog(strLogMessage);
			} finally {
				if (conn != null)
					MobileTech.Consts.ReleaseDBConnection (conn);
			}

			return flag;
		}


		public List<Account> GetAccounts(long FacPrimaryId) 
		{
			SqliteConnection conn = null;
			List<Account> accounts = new List<Account> ();
			try {
				conn = MobileTech.Consts.GetDBConnection ();

				string sqlUsrSelect = @"SELECT ACCPrimaryId, AccountId, DepartmentName FROM Account Where FACPrimaryId = @FACPrimaryId";

				using (SqliteCommand sqlUsrSelectQry = conn.CreateCommand()) {
					sqlUsrSelectQry.CommandText = sqlUsrSelect;
					sqlUsrSelectQry.Parameters.Add ("@FACPrimaryId",DbType.UInt64);

					#region Parameters Assigning
					sqlUsrSelectQry.Parameters ["@FACPrimaryId"].Value = FacPrimaryId; 
					#endregion

					using (SqliteDataReader rReader = sqlUsrSelectQry.ExecuteReader()) {
						while (rReader.Read()) {
							Account account = new Account();

							account.AccPrimaryId = Convert.ToInt64(rReader.GetValue (rReader.GetOrdinal ("ACCPrimaryId")));
							account.AccountId = Convert.ToString(rReader.GetValue(rReader.GetOrdinal("AccountId")));
							account.DepartmentName = Convert.ToString(rReader.GetValue(rReader.GetOrdinal("DepartmentName")));

							accounts.Add(account);
						}
					}
				}
			}catch (Exception ex) {
				string strLogMessage = string.Format("{0} GetAccounts Exception: {1}", DateTime.Now, ex.Message);
				Consts.HandleExceptionLog(strLogMessage);
			} finally {
				if (conn != null)
					MobileTech.Consts.ReleaseDBConnection (conn);
			}

			return accounts;
		}

		public List<FacilityDetails> GetAccountFacilities() 
		{
			SqliteConnection conn = null;
			List<FacilityDetails> facilities = new List<FacilityDetails> ();
			try {
				conn = MobileTech.Consts.GetDBConnection ();

				string sqlUsrSelect = @"SELECT FACPrimaryId, Facility FROM AccountFacility Order By Facility";

				using (SqliteCommand sqlUsrSelectQry = conn.CreateCommand()) {
					sqlUsrSelectQry.CommandText = sqlUsrSelect;

					using (SqliteDataReader rReader = sqlUsrSelectQry.ExecuteReader()) {
						while (rReader.Read()) {
							FacilityDetails facilityInfo = new FacilityDetails();

							facilityInfo.FacPrimaryId = Convert.ToInt64(rReader.GetValue (rReader.GetOrdinal ("FACPrimaryId")));
							facilityInfo.Name = Convert.ToString(rReader.GetValue(rReader.GetOrdinal("Facility")));

							facilities.Add(facilityInfo);
						}
					}
				}
			}catch (Exception ex) {
				string strLogMessage = string.Format("{0} GetAccountFacilities Exception: {1}", DateTime.Now, ex.Message);
				Consts.HandleExceptionLog(strLogMessage);
			} finally {
				if (conn != null)
					MobileTech.Consts.ReleaseDBConnection (conn);
			}

			return facilities;
		}

		public List<WorkerInfo> GetWorkers(long WkrPrimaryId) 
		{
			SqliteConnection conn = null;
			List<WorkerInfo> workers = new List<WorkerInfo> ();
			try {
				conn = MobileTech.Consts.GetDBConnection ();

				string sqlUsrSelect = @"SELECT WKRPrimaryId, Name FROM Workers Where WKRPrimaryId<>@WKRPrimaryId Order By Name";

				using (SqliteCommand sqlUsrSelectQry = conn.CreateCommand()) {
					sqlUsrSelectQry.CommandText = sqlUsrSelect;
					sqlUsrSelectQry.Parameters.Add ("@WKRPrimaryId",DbType.UInt64);

					#region Parameters Assigning
					sqlUsrSelectQry.Parameters ["@WKRPrimaryId"].Value = WkrPrimaryId; 
					#endregion

					using (SqliteDataReader rReader = sqlUsrSelectQry.ExecuteReader()) {
						while (rReader.Read()) {
							WorkerInfo workerInfo = new WorkerInfo();

							workerInfo.WKRPrimaryId = Convert.ToInt64(rReader.GetValue (rReader.GetOrdinal ("WKRPrimaryid")));
							//workerInfo.Id = Convert.ToInt16(rReader.GetValue (rReader.GetOrdinal ("WorkerId")));
							workerInfo.Name = Convert.ToString(rReader.GetValue(rReader.GetOrdinal("Name")));

							workers.Add(workerInfo);
						}
					}
				}
			}catch (Exception ex) {
				string strLogMessage = string.Format("{0} GetWorkers Exception: {1}", DateTime.Now, ex.Message);
				Consts.HandleExceptionLog(strLogMessage);
			} finally {
				if (conn != null)
					MobileTech.Consts.ReleaseDBConnection (conn);
			}

			return workers;
		}

		public WorkerInfo GetWorkerOnID(long WkrPrimaryId) 
		{
			SqliteConnection conn = null;
			WorkerInfo workerInfo = new WorkerInfo();
			try {
				conn = MobileTech.Consts.GetDBConnection ();

				string sqlUsrSelect = @"SELECT WKRPrimaryId, Name FROM Workers Where WKRPrimaryId = @WKRPrimaryId";

				using (SqliteCommand sqlUsrSelectQry = conn.CreateCommand()) {
					sqlUsrSelectQry.CommandText = sqlUsrSelect;

					sqlUsrSelectQry.Parameters.Add ("@WKRPrimaryId",DbType.UInt64);
					sqlUsrSelectQry.Parameters ["@WKRPrimaryId"].Value = WkrPrimaryId; 

					using (SqliteDataReader rReader = sqlUsrSelectQry.ExecuteReader()) {
						if (rReader.Read()) {
							//workerInfo  = new WorkerInfo();

							workerInfo.WKRPrimaryId = Convert.ToInt64(rReader.GetValue (rReader.GetOrdinal ("WKRPrimaryid")));
							workerInfo.Name = Convert.ToString(rReader.GetValue(rReader.GetOrdinal("Name")));
						}
					}
				}
			}catch (Exception ex) {
				string strLogMessage = string.Format("{0} GetWorkerOnID Exception: {1}", DateTime.Now, ex.Message);
				Consts.HandleExceptionLog(strLogMessage);
			} finally {
				if (conn != null)
					MobileTech.Consts.ReleaseDBConnection (conn);
			}
			return workerInfo;
		}

		public List<AccountInfo> GetAccountDepartments(string facPrimaryIDs) 
		{
			SqliteConnection conn = null;
			List<AccountInfo> accountInfo = new List<AccountInfo> ();
			//string strFacPrimaryIds = string.Format ("'{0}'", facPrimaryIDs.Replace(",", "','"));

			try {
				conn = MobileTech.Consts.GetDBConnection ();

//				string sqlUsrSelect = @"SELECT ACCPrimaryId, AccountId, DepartmentName, Facility, AccountFacility.FACPrimaryid from Account
//										left outer join AccountFacility on Account.FACPrimaryid = AccountFacility.FACPrimaryid
//										where AccountFacility.FACPrimaryid in ('@FACPrimaryid')";

				string sqlUsrSelect = @"SELECT ACCPrimaryId, AccountId, DepartmentName, Facility, AccountFacility.FACPrimaryid From Account
										Left Outer Join AccountFacility on Account.FACPrimaryid = AccountFacility.FACPrimaryid
										Where AccountFacility.FACPrimaryid In ("+facPrimaryIDs+") Order By Facility, DepartmentName";

				using (SqliteCommand sqlUsrSelectQry = conn.CreateCommand()) {
					sqlUsrSelectQry.CommandText = sqlUsrSelect;

//					sqlUsrSelectQry.Parameters.Add ("@FACPrimaryid",DbType.String);
//
//					#region Parameters Assigning
//					sqlUsrSelectQry.Parameters ["@FACPrimaryid"].Value = facPrimaryIDs.Replace(",", "','"); 
//					#endregion
					//sqlUsrSelectQry.Parameters.Add(new SqliteParameter{ParameterName="facprimaryid", DbType = DbType.String, Value = strFacPrimaryIds});

					using (SqliteDataReader rReader = sqlUsrSelectQry.ExecuteReader()) {
						while (rReader.Read()) {
							AccountInfo accDetails = new AccountInfo();
							accDetails.AccPrimaryId = Convert.ToInt64(rReader.GetValue (rReader.GetOrdinal ("ACCPrimaryId")));
							//accDetails.AccountId = Convert.ToString(rReader.GetValue (rReader.GetOrdinal ("AccountId")));
							accDetails.DepartmentName = Convert.ToString(rReader.GetValue (rReader.GetOrdinal ("DepartmentName")));
							accDetails.Facility = new FacilityInfo{Name =Convert.ToString(rReader.GetValue (rReader.GetOrdinal ("Facility"))), 
								FacPrimaryId = Convert.ToInt64(rReader.GetValue (rReader.GetOrdinal ("FACPrimaryid")))};
							accountInfo.Add(accDetails);
						}
					}
				}
			}catch (Exception ex) {
				string strLogMessage = string.Format("{0} GetAccountDepartments Exception: {1}", DateTime.Now, ex.Message);
				Consts.HandleExceptionLog(strLogMessage);
			} finally {
				if (conn != null)
					MobileTech.Consts.ReleaseDBConnection (conn);
			}

			return accountInfo;
		}

		public void InsertORUpdateFilter(FilterInfo filterInfo) 
		{
			SqliteConnection conn = null;
			try {
				conn = MobileTech.Consts.GetDBConnection ();

				string sqlStmt_InsertUpdateFilter = string.Empty;

				const string sqlStmt_CheckFilter = @"Select Exists (Select MTFPrimaryId from Filter where MTFPrimaryId = @MTFPrimaryId)";


				const string sqlStmt_InsertFilter = "Insert into Filter (MTFPrimaryId, Name, FACPrimaryIds, ACCPrimaryIds, WKRPrimaryIds,Active) " +
					"Values (@MTFPrimaryId, @Name, @FACPrimaryIds, @ACCPrimaryIds, @WKRPrimaryIds,@Active)";

				const string sqlStmt_UpdateFilter = "Update Filter Set FACPrimaryIds = @FACPrimaryIds, ACCPrimaryIds = @ACCPrimaryIds, " +
					"WKRPrimaryIds = @WKRPrimaryIds, Active = @Active Where MTFPrimaryId = @MTFPrimaryId";

				using (SqliteCommand sqlQryCheck = conn.CreateCommand()) {
					sqlQryCheck.CommandText = sqlStmt_CheckFilter;

					sqlQryCheck.Parameters.Add ("@MTFPrimaryId",DbType.UInt64);
					sqlQryCheck.Parameters ["@MTFPrimaryId"].Value = filterInfo.MTFPrimaryId; 

					object result = sqlQryCheck.ExecuteScalar();

					if (result != null){
						if (System.Convert.ToInt32(result) == 1)
							sqlStmt_InsertUpdateFilter = sqlStmt_UpdateFilter;
						else
							sqlStmt_InsertUpdateFilter = sqlStmt_InsertFilter;

						#region Insert Or Update Filter
						using (SqliteCommand sqlQry = conn.CreateCommand()) {
							sqlQry.CommandText = sqlStmt_InsertUpdateFilter;
							sqlQry.Parameters.Add ("@MTFPrimaryId",DbType.UInt64);
							sqlQry.Parameters.Add ("@Name",DbType.String);
							sqlQry.Parameters.Add ("@FACPrimaryIds",DbType.String);
							sqlQry.Parameters.Add ("@ACCPrimaryIds",DbType.String);
							sqlQry.Parameters.Add ("@WKRPrimaryIds",DbType.String);
							sqlQry.Parameters.Add ("@Active",DbType.Boolean);

							sqlQry.Parameters ["@MTFPrimaryId"].Value = filterInfo.MTFPrimaryId; 
							sqlQry.Parameters ["@Name"].Value = filterInfo.FilterName; 
							sqlQry.Parameters ["@FACPrimaryIds"].Value = filterInfo.FACPrimaryIds; 
							sqlQry.Parameters ["@ACCPrimaryIds"].Value = filterInfo.ACCPrimaryIds; 
							sqlQry.Parameters ["@WKRPrimaryIds"].Value = filterInfo.WKRPrimaryIds; 
							sqlQry.Parameters ["@Active"].Value = filterInfo.Active;

							sqlQry.ExecuteNonQuery ();
						}
						#endregion

					}
				}
			}catch (Exception ex) {
				//Console.WriteLine ("Exception in GetResults : " + ex.Message);
				string strLogMessage = string.Format("{0} InsertORUpdateFilter Exception: {1}", DateTime.Now, ex.Message);
				Consts.HandleExceptionLog(strLogMessage);
			} finally {
				if (conn != null)
					MobileTech.Consts.ReleaseDBConnection (conn);
			}

		}

		public string UploadFilter() {
			FilterInfo filter = GetFilterDetails();
			string serverResponse = string.Empty;
			try{

				var request = HttpWebRequest.Create(string.Format(@"{0}/Filter/UpdateMobileTechFilters",MobileTech.Consts.ServerURL));
				//request.Headers.Add("Accept", "application/json");

				request.ContentType = "application/json; charset=utf-8";
				request.Method = "POST";

				request.Headers.Set("SessionID", AuthClient.GetInstance().CurrentSession);

				string json = JsonConvert.SerializeObject(filter, Formatting.Indented);

				using (var streamWriter = new StreamWriter(request.GetRequestStream())){
					json = json.Replace("\r\n","");
					//json = json.Replace("\",", "\","   + "\"" +"\u002B");
					streamWriter.Write(json);
					streamWriter.Flush();
					streamWriter.Close();
				}

				//request.GetResponse();

				HttpWebResponse myHttpWebResponse = (HttpWebResponse)request.GetResponse();

				Stream responseStream = myHttpWebResponse.GetResponseStream();

				StreamReader myStreamReader = new StreamReader(responseStream, Encoding.Default);

				serverResponse = myStreamReader.ReadToEnd();

				myStreamReader.Close();
				responseStream.Close();

				if(serverResponse.ToLower()=="true"){
					string strLogMessage = string.Format("{0} Successfully Filter Updated on Server : {1}", DateTime.Now, filter.FilterName);
					MobileTech.Consts.HandleExceptionLog(strLogMessage);
				}


			}catch (Exception ex){
				MobileTech.Consts.LogException (ex);
				string strLogMessage = string.Format("{0} UploadExceptionLogFromDevice UploadFilter Exception: {1}", DateTime.Now, ex.Message);

				MobileTech.Consts.HandleExceptionLog(strLogMessage);
			}

			return serverResponse;
		}

		public FilterInfo GetFilterDetails()
		{
			SqliteConnection conn = null;
			FilterInfo filter = new FilterInfo ();

			try {
				conn = MobileTech.Consts.GetDBConnection ();

				string sqlSelectPDADBID = @"select MTFPrimaryID,Name,FACPrimaryIds,ACCPrimaryIds,WKRPrimaryIds, Active from Filter ";

				using (SqliteCommand sqlUsrSelectQry = conn.CreateCommand()) {
					sqlUsrSelectQry.CommandText = sqlSelectPDADBID;

					using (SqliteDataReader rReader = sqlUsrSelectQry.ExecuteReader()) {
						if (rReader.Read()) {
							filter.MTFPrimaryId = Convert.ToInt64(rReader.GetValue(rReader.GetOrdinal("MTFPrimaryID")));
							filter.FilterName = Convert.ToString(rReader.GetValue (rReader.GetOrdinal ("Name")));
							filter.FACPrimaryIds = Convert.ToString(rReader.GetValue (rReader.GetOrdinal ("FACPrimaryIds")));
							filter.ACCPrimaryIds = Convert.ToString(rReader.GetValue (rReader.GetOrdinal ("ACCPrimaryIds")));
							filter.WKRPrimaryIds = Convert.ToString(rReader.GetValue (rReader.GetOrdinal ("WKRPrimaryIds")));
							filter.Active = Convert.ToByte(rReader.GetValue (rReader.GetOrdinal ("Active")));
						}
					}
				}
			}catch (Exception ex) {
				string strLogMessage = string.Format("{0} GetFilterDetails Exception: {1}", DateTime.Now, ex.Message);
				Consts.HandleExceptionLog(strLogMessage);
			} finally {
				if (conn != null)
					MobileTech.Consts.ReleaseDBConnection (conn);
			}

			return filter;
		}

		public string GetFilterAccountFacilities(string facPrimaryIDs) 
		{
			string retFacilities = string.Empty;
			SqliteConnection conn = null;
			List<FacilityDetails> facilities = new List<FacilityDetails> ();
			try {
				conn = MobileTech.Consts.GetDBConnection ();

				string sqlUsrSelect = @"SELECT Facility FROM AccountFacility where FACPrimaryId in ( " + facPrimaryIDs + " )";

				using (SqliteCommand sqlUsrSelectQry = conn.CreateCommand()) {
					sqlUsrSelectQry.CommandText = sqlUsrSelect;

					using (SqliteDataReader rReader = sqlUsrSelectQry.ExecuteReader()) {
						while (rReader.Read()) {
							retFacilities  +=  Convert.ToString(rReader.GetValue(rReader.GetOrdinal("Facility"))) + ",";
						}
					}
				}
			}catch (Exception ex) {
				string strLogMessage = string.Format("{0} GetAccountFacilities View Appear Exception: {1}", DateTime.Now, ex.Message);
				Consts.HandleExceptionLog(strLogMessage);
			} finally {
				if (conn != null)
					MobileTech.Consts.ReleaseDBConnection (conn);
			}

			return retFacilities.Trim().TrimEnd(',');
		}

		public string GetFilterDepartments(string accPrimaryIDs) 
		{
			string retAccDepts = string.Empty;
			SqliteConnection conn = null;
			List<FacilityDetails> facilities = new List<FacilityDetails> ();
			try {
				conn = MobileTech.Consts.GetDBConnection ();

				string sqlUsrSelect = @"SELECT DepartmentName FROM Account where ACCPrimaryId in ( " + accPrimaryIDs + " )";

				using (SqliteCommand sqlUsrSelectQry = conn.CreateCommand()) {
					sqlUsrSelectQry.CommandText = sqlUsrSelect;

					using (SqliteDataReader rReader = sqlUsrSelectQry.ExecuteReader()) {
						while (rReader.Read()) {
							retAccDepts +=  Convert.ToString(rReader.GetValue(rReader.GetOrdinal("DepartmentName"))) + ",";
						}
					}
				}
			}catch (Exception ex) {
				string strLogMessage = string.Format("{0} GetAccountDepartments View Appear Exception: {1}", DateTime.Now, ex.Message);
				Consts.HandleExceptionLog(strLogMessage);
			} finally {
				if (conn != null)
					MobileTech.Consts.ReleaseDBConnection (conn);
			}

			return retAccDepts.Trim().TrimEnd(',');
		}

		public string GetFilterWorker(string wkrPrimaryIDs) 
		{
			string retWorkers = string.Empty;
			List<string> lstWorkers = new List<string> ();
			string myWorker = string.Empty;
			SqliteConnection conn = null;
			List<FacilityDetails> facilities = new List<FacilityDetails> ();
			try {
				conn = MobileTech.Consts.GetDBConnection ();

				string sqlUsrSelect = @"SELECT WKRPrimaryId, Name FROM workers where WKRPrimaryId in ( " + wkrPrimaryIDs + " )";

				using (SqliteCommand sqlUsrSelectQry = conn.CreateCommand()) {
					sqlUsrSelectQry.CommandText = sqlUsrSelect;

					using (SqliteDataReader rReader = sqlUsrSelectQry.ExecuteReader()) {
						while (rReader.Read()) {
							if(Convert.ToInt64(rReader.GetValue(rReader.GetOrdinal("WKRPrimaryId"))) == Consts.WKRPrimaryId)
								myWorker= "<My>";
							else
								lstWorkers.Add(Convert.ToString(rReader.GetValue(rReader.GetOrdinal("Name"))));
						}
					}
				}
				if(!string.IsNullOrEmpty(myWorker))
					lstWorkers.Insert(0,myWorker);
			}catch (Exception ex) {
				string strLogMessage = string.Format("{0} GetFilterWorker View Appear Exception: {1}", DateTime.Now, ex.Message);
				Consts.HandleExceptionLog(strLogMessage);
			} finally {
				if (conn != null)
					MobileTech.Consts.ReleaseDBConnection (conn);
			}

			//return retWorkers.Trim().TrimEnd(',');
			retWorkers = string.Join (",", lstWorkers);
			return retWorkers;
		}


	}
}