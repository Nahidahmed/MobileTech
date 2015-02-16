using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Data.SqlClient;
using MobileTechService;
using System.Diagnostics;
using System.Net;
using Mono.Data.Sqlite;
using MonoTouch.Foundation;


 
namespace MobileTech {
    public class WsMessageConnector {

		static string documents = Environment.GetFolderPath (Environment.SpecialFolder.MyDocuments);
		static string ConnectionString = "URI=file:"+Path.Combine(documents,"WsMsgConnectorQueue.sqlite");

		private const int _maxConnectionTryCount = 5;
        private bool bRunning = true;


	    Uri _remoteServerUrl = new Uri(Consts.ServerURL + "/AuthenticationService");
        private Thread process;
        //private readonly NetworkMonitor networkMonitor;
        private SqliteConnection connection;
        //private readonly AutoResetEvent connectionEvent = new AutoResetEvent(false);
        private SqliteCommand cmd;
        private DataTable dataTable = new DataTable(); 
		private MsgProcessingState CurrentState { get; set; }

//		MobileTech.iOS.WkOrderListViewController dispListObj;
        private static WsMessageConnector wsMessageConnector;
		private readonly AuthClient authClient = AuthClient.GetInstance();
        
		public  WsMessageConnector GetInstance()
        {
//			return wsMessageConnector ?? (wsMessageConnector = new WsMessageConnector(dispListObj));
			return wsMessageConnector ?? (wsMessageConnector = new WsMessageConnector());
        }

//		public WsMessageConnector(iGotIt.Screens.DispatchListScreen dispListSourceObj) {
		public WsMessageConnector() {
//			dispListObj = dispListSourceObj;
            CurrentState = MsgProcessingState.UnInitialized;
            connection = connection ?? (connection = new SqliteConnection(ConnectionString));
            connection.Open();
        }

		public void Start() {
			bRunning = true;
			process = new Thread(CheckQueue) {IsBackground = true};
			process.Start();
//			MobileTech.Consts.LogEntry ("WsMessageConnector started", false);
		}

        public void Stop() {
            if (connection != null) {
                connection.Close();
            }
			connection.Dispose();
            bRunning = false;
//			MobileTech.Consts.LogEntry ("WsMessageConnector stopped", false);
        }

        /*private void NetworkMonitor_OnNetworkStatusChange(NetworkMonitor.ConnectionStatus connStatus) {
            if (connStatus == NetworkMonitor.ConnectionStatus.Connected) {
                connectionEvent.Set();
            }
        }*/

        private void GetMessagesFromDB() {
			try
			{
            	dataTable.Clear();
				cmd = new SqliteCommand("select * from WsMsgConnectorQueue Where EntityStatus <> 2 order by id", connection);

            	using (SqliteDataAdapter dataAdapter = new SqliteDataAdapter(cmd)){
                	dataAdapter.Fill(dataTable);
            	}
			}catch(Exception ex){
				//Console.WriteLine("GetMessagesFromDB: "+ex.Message);

				//Author/Date/Issue - M.D.Prasad/14th May 2014/ISS - 5395 or CRR - 1338  Upload iGotIt device logs
				string strLogMessage = string.Format("{0} GetMessagesFromDB Exception: {1}", DateTime.Now, ex.Message);
				MobileTech.Consts.HandleExceptionLog(strLogMessage);
			}
        }

		private void CheckQueue()
		{
			while (bRunning) {
//				iGotItService.Consts.LogEntry ("WsMessageConnector heartbeat. Current State = " + CurrentState.ToString(), true);
				ProcessMessages ();
				Thread.Sleep (5000);
			}
		}

		public void ForceSync()
		{
			if (CurrentState != MsgProcessingState.ProcessingMessages) {
				CurrentState = MsgProcessingState.ProcessingMessages;
//				iGotItService.Consts.LogEntry ("Forcing Synch", false);
				GetMessagesFromDB();
				if (dataTable != null) {
					ProcessMessages ();
				}
			} else {
//				iGotItService.Consts.LogEntry ("Already forcing synch. Not doing anything", false);
			}
		}

        public void ProcessMessages() {
            switch (CurrentState) {
                case MsgProcessingState.UnInitialized:
                    CurrentState = MsgProcessingState.WaitForMessage;
                    break;
                case MsgProcessingState.ProcessingMessages:
                    if (dataTable != null)
                    {
					    bool newtorkConnected = true;
					    
						if (!Consts.IsWebApiUrlReachable(_remoteServerUrl, "/AuthenticationService"))
					    {
						    newtorkConnected = false;
					    }

						foreach (DataRow dr in dataTable.Rows.Cast<DataRow>().TakeWhile(dr =>  newtorkConnected && bRunning && (Consts.signonStatus != "false" && Consts.signonStatus != "NoConnection" && Consts.signonStatus != "NoConnectionButDBIsNotEmpty")))
                        {
							if (authClient.IsValidSession ()) {
								BuildMsgType (
									Convert.ToInt32 (dr ["Id"]),
									(EntityAction)Convert.ToInt32(dr["EntityAction"]),
									(EntityType)Convert.ToInt32(dr["EntityType"]),
									dr["EntityKey"].ToString(),
								    dr["EntitySubKey"].ToString()
									//dr["AdditionalNotes"].ToString(),
									//Convert.ToInt32 (dr ["ApplyReasonID"] == DBNull.Value ? 0 : dr ["ApplyReasonID"])
								);
								
							}
                        }
                    }
                    CurrentState = MsgProcessingState.WaitForMessage;
                    break;
                case MsgProcessingState.WaitingForConnection:
                    //connectionEvent.WaitOne();
                    CurrentState = MsgProcessingState.WaitForMessage;
                    break;
                case MsgProcessingState.WaitForMessage:
                    //The sleep is done at the begin of this state so we don't get into a spin of processing messages 
                    //and wait for messages if we have one in the queue that gets reprocessed.
                    GetMessagesFromDB();
                    if (dataTable != null && dataTable.Rows.Count > 0) {
						if (!Consts.IsWebApiUrlReachable(_remoteServerUrl, "/AuthenticationService")) {
							CurrentState = MsgProcessingState.WaitForMessage;
						}
						else {
							CurrentState = MsgProcessingState.ProcessingMessages;
						}
                    }
					else {
                        CurrentState = MsgProcessingState.WaitForMessage;                           
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

		private  void BuildMsgType(int id, EntityAction ea, EntityType et, string ek,string eSK) {
            // based on the entity type you construct the appropriate object
            switch (et) {
                case EntityType.WorkOrder:
				new DispatchHandler().ProcessMessage(id, ea, ek,eSK);
//				NSObject nsObject = new NSObject();
//				nsObject.InvokeOnMainThread(delegate {
//					this.dispListObj.getBackgroundSyncData();
//				});

                    break;
                case EntityType.Asset:
                    break;
                case EntityType.TestMessage:
                    break;
                default:
                    throw new ArgumentOutOfRangeException("et");
            }
        }
    }
}