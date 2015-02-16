using System;
using System.Web.Services.Protocols;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using MobileTechService;
using MobileTech.Models.Entity;
using MobileTech.Core;
using System.Net;
using Newtonsoft.Json;
using System.IO;
using System.Text;

 
namespace MobileTech {
    public class DispatchHandler : WsMsgConnector {


		public  bool  CheckWsMessageConnector() {
			return WsMsgConnectorDAL.CheckWsMessageConnector();
		}


		public override void ProcessMessage(int id, EntityAction ea, string ek, string eSK) {
			string retVal = string.Empty; // set to -1 if service returns error
			Repository repository = new Repository();
			WorkOrderDetails woDetails = repository.GetWorkOrderDetails(Convert.ToInt64(ek));

			if(woDetails.WorPrimaryId==0)
			{
				WsMsgConnectorDAL.UpdateWsMessageConnector (id, (int)EntityStatus.Fail);
				return;
			}

			if (eSK != string.Empty) {
				woDetails.OpenWorkOrderStatus.PrimaryId = Convert.ToInt64 (eSK);
			}

			try {
				switch (ea) {
				case EntityAction.Create:
					break;
				case EntityAction.Update:
					if (woDetails != null) {
						retVal = UploadWorkOrder (woDetails);
						if (retVal.ToLower () == "1") {
							var info = UploadPDADBID (repository.GetPDADBIDDetails ());
						}
					}
					break;
				case EntityAction.Delete:
                    // use the key to delete the item
					break;
				}
			} catch (SoapException) {
				//Load the Detail element of the SoaopException object
            
			} catch (Exception ex) {
				//Author/Date/Issue - M.D.Prasad/14th May 2014/ISS - 5395 or CRR - 1338  Upload iGotIt device logs
				string strLogMessage = string.Format ("{0} ProcessMessage Exception: {1}", DateTime.Now, ex.Message);
				Consts.HandleExceptionLog (strLogMessage);

				var user = NSUserDefaults.StandardUserDefaults;
				user.SetString(strLogMessage,"ProcessMessage_Exception");

				if (ex.Message == "WebException") {
				}
				// TODO: This message needs to be put into a Error State so it doesn't keep hitting the backend
			} finally {
				// delete message if successfully sent
				switch (retVal) {

					case "0"://if retval=0; then something fails on server.
						WsMsgConnectorDAL.UpdateWsMessageConnector (id, (int)EntityStatus.Fail);
						break;
					case "1"://if retval=1; then successfully updated on server.
						WsMsgConnectorDAL.DeleteWsMessageConnector (id);
						break;
					case "2"://if retval=2; then workorder assigned to another worker.
						WsMsgConnectorDAL.DeleteWsMessageConnector (id);
						try {
							NSObject nsObject = new NSObject ();
							nsObject.InvokeOnMainThread (delegate {
								ShowAlertMessag ("Unable to update WO # " + woDetails.Number + " as it is assigned to another Worker.");
								Consts.isSyncRequired=true;
							} );
						}  catch (Exception) {
						}
						break;
					case "3"://if retval=3; then workorder voided.
						WsMsgConnectorDAL.DeleteWsMessageConnector (id);
						try {
							NSObject nsObject = new NSObject ();
							nsObject.InvokeOnMainThread (delegate {
								ShowAlertMessag ("Unable to update WO # " + woDetails.Number + " as it is Voided.");
								Consts.isSyncRequired=true;
							} );
						}  catch (Exception) {
						}
						break;

					case "4"://if retval=4; then workorder Closed.
						WsMsgConnectorDAL.DeleteWsMessageConnector (id);
						try {
							NSObject nsObject = new NSObject ();
							nsObject.InvokeOnMainThread (delegate {
								ShowAlertMessag ("Unable to update WO # " + woDetails.Number + " as it is Closed.");
								Consts.isSyncRequired=true;
							} );
						}  catch (Exception) {
						}
						break;
					default:
						break;
				}
			}

        }

		public void ShowAlertMessag(string alrtMessag)
		{
			UIAlertView alrtMsg = new UIAlertView();
			alrtMsg.Message = alrtMessag;
			alrtMsg.AddButton("Ok"); 
		    alrtMsg.Show();
		}


		public string UploadWorkOrder(WorkOrderDetails woDetails) {
			string serverResponse = string.Empty;
			try{
				//TODO:28th
				//AuthenticationService.UploadExceptionLogFromDevice(filename, data);

				var request = HttpWebRequest.Create(string.Format(@"{0}/WorkOrder/InsertUpdateWorkOrderDetails",MobileTech.Consts.ServerURL));
				//request.Headers.Add("Accept", "application/json");

				request.ContentType = "application/json; charset=utf-8";
				request.Method = "POST";

				request.Headers.Set("SessionID", AuthClient.GetInstance().CurrentSession);

				string json = JsonConvert.SerializeObject(woDetails, Formatting.Indented);

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

				if(serverResponse.ToLower()=="1"){
					var user = NSUserDefaults.StandardUserDefaults;
					string strLogMessage = string.Format("{0} Successfully Work Order Updated on Server : {1}", DateTime.Now, woDetails.Number);
					user.SetString(strLogMessage,"WO_Uploaded_Successfully");
				    MobileTech.Consts.HandleExceptionLog(strLogMessage);
				}



			}catch (Exception ex){
				MobileTech.Consts.LogException (ex);

				string strLogMessage = string.Format("{0} UploadExceptionLogFromDevice UploadWorkOrder Exception: {1}", DateTime.Now, ex.Message);
				MobileTech.Consts.HandleExceptionLog(strLogMessage);
				var user = NSUserDefaults.StandardUserDefaults;
				user.SetString(strLogMessage,"UploadWorkOrder_Exception");
			}

			return serverResponse;
		}

		public string UploadPDADBID(PDADBID pdadbid) {
			string serverResponse = string.Empty;
			try{

				var request = HttpWebRequest.Create(string.Format(@"{0}/PDADBID/updatepdadbid",MobileTech.Consts.ServerURL));
				//request.Headers.Add("Accept", "application/json");

				request.ContentType = "application/json; charset=utf-8";
				request.Method = "POST";

				request.Headers.Set("SessionID", AuthClient.GetInstance().CurrentSession);

				string json = JsonConvert.SerializeObject(pdadbid, Formatting.Indented);

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
					string strLogMessage = string.Format("{0} Successfully PDADBID Updated on Server : {1}", DateTime.Now, pdadbid.username);
					MobileTech.Consts.HandleExceptionLog(strLogMessage);
					var user = NSUserDefaults.StandardUserDefaults;
					user.SetString(strLogMessage,"PDADBID_Updated_on_Server");
				}


			}catch (Exception ex){
				MobileTech.Consts.LogException (ex);

				string strLogMessage = string.Format("{0} UploadExceptionLogFromDevice UploadPDADBID Exception: {1}", DateTime.Now, ex.Message);

				var user = NSUserDefaults.StandardUserDefaults;
				user.SetString(strLogMessage,"UploadPDADBID_Exception");

				MobileTech.Consts.HandleExceptionLog(strLogMessage);
			}

			return serverResponse;
		}
    }
}