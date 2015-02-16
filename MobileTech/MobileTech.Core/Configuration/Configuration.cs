using MonoTouch.UIKit;
using System;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using MobileTech.Core;
using MobileTech.Core.ConfigService;
using MobileTech.Core;
using MobileTech.Models.Entity;


namespace MobileTechService
{
	public class Configuration
	{
		public Configuration ()
		{
		}

		private ConfigService configurationService;

		private ConfigService ConfigService
		{
			get{
				if(configurationService == null){
					configurationService =new ConfigService{
						Url = MobileTech.Consts.ServerURL + "/ConfigService.asmx"
					};
					configurationService.Timeout=40*60*1000;
				}
				else if (!(configurationService.Url.Trim() == (MobileTech.Consts.ServerURL.Trim()+"/ConfigService.asmx"))) {

					configurationService.Url = MobileTech.Consts.ServerURL + "/ConfigService.asmx";
				}
				configurationService.SessionID = AuthClient.GetInstance().CurrentSession;
				return configurationService;				
			}
		}

		/*public bool GetSecurity (string blockName)
		{
			bool retValue = true;
			DateTime syncDate = DateTime.Now;
			
			try {
				DateTime lastSyncDate = ConfigurationDAL.GetLastSyncDateByblock(blockName);
				
				string[] deleteResult = ConfigService.GetSecuritiesDeleted(lastSyncDate);
				ConfigurationDAL.DeleteSecurityItems(deleteResult);
				int pageSize = 1000;
				int page =1;
				int count;
				
				do{

					Security[] result = ConfigService.GetSecuritiesFromSync(lastSyncDate,page,pageSize);
					ConfigurationDAL.InsertSecurityItems(result);
					
					page++;
					count = result.Length;
				}while(count >= pageSize);
				
				ConfigurationDAL.UpdateBlockLastSyncDate(syncDate,blockName);
			} 
			catch (Exception ex) {
				string strLogMessage = string.Format("{0} GetSecurity Exception: {1}", DateTime.Now, ex.Message);
				MobileTech.Consts.HandleExceptionLog(strLogMessage);

				//Console.WriteLine("GetSecurity: "+ex.Message);
				retValue = false;
			}
			return retValue;
		}*/

		private string JsonWebAPICallForVersionDetails(string webMethod)
		{
			string responseContent = string.Empty;

			var	 request = HttpWebRequest.Create (string.Format (@"{0}{1}", MobileTech.Consts.ServerURL, webMethod));

			request.ContentType = "application/json";
			request.Method = "GET";

			request.Headers.Set("SessionID", AuthClient.GetInstance().CurrentSession);

			using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
			{
				if (response.StatusCode != HttpStatusCode.OK)
					Console.Out.WriteLine("Error while fetching {1} data. Server returned status code: {0}", response.StatusCode,webMethod);
				using (StreamReader reader = new StreamReader(response.GetResponseStream()))
				{
					var content = reader.ReadToEnd();
					if(string.IsNullOrWhiteSpace(content)) {
						//Console.Out.WriteLine("Response contained empty body...");
					}
					else {
						//Console.Out.WriteLine("Response Body: \r\n {0}", content);
					}

					//config.DeserializeJson (content);
					responseContent = content;
				}
			}
			return responseContent;
		}

		private string JsonWebAPICallForDownloadSync(string webMethod, DateTime lastSyncDate)
		{
			string responseContent = string.Empty;

			var	request = HttpWebRequest.Create (string.Format (@"{0}{1}lastSyncTime={2}", MobileTech.Consts.ServerURL, webMethod, lastSyncDate.ToString ("MM/dd/yyyy hh:mm:ss tt")));

			request.ContentType = "application/json";
			request.Method = "GET";

			request.Headers.Set("SessionID", AuthClient.GetInstance().CurrentSession);

			using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
			{
				if (response.StatusCode != HttpStatusCode.OK)
					Console.Out.WriteLine("Error while fetching {1} data. Server returned status code: {0}", response.StatusCode,webMethod);
				using (StreamReader reader = new StreamReader(response.GetResponseStream()))
				{
					var content = reader.ReadToEnd();
					if(string.IsNullOrWhiteSpace(content)) {
						//Console.Out.WriteLine("Response contained empty body...");
					}
					else {
						//Console.Out.WriteLine("Response Body: \r\n {0}", content);
					}

					//config.DeserializeJson (content);
					responseContent = content;
				}
			}
			return responseContent;
		}


		public bool GetSecurity (string blockName,ref string errorMessage)
		{
			//Console.WriteLine ("Security Started");
			errorMessage = string.Empty;
			bool retValue = true;
			DateTime syncDate = DateTime.Now;

			try {
				DateTime lastSyncDate = ConfigurationDAL.GetLastSyncDateByblock(blockName,ref errorMessage);


				string responseDeleteResult = JsonWebAPICallForDownloadSync("/Security/GetSecuritiesDeleted?",lastSyncDate);
				string[] deleteResult = ((List<string>)JsonConvert.DeserializeObject(responseDeleteResult , typeof(List<string>))).ToArray();
				ConfigurationDAL.DeleteSecurityItems(deleteResult);

				string responseResult = JsonWebAPICallForDownloadSync("/Security/GetSecurities?",lastSyncDate);
				List<SecuritiesSync> myObjectList = (List<SecuritiesSync>)JsonConvert.DeserializeObject(responseResult , typeof(List<SecuritiesSync>));
				ConfigurationDAL.InsertSecurityItems(myObjectList.ToArray(),ref errorMessage);

				ConfigurationDAL.UpdateBlockLastSyncDate(syncDate,blockName);

			} 
			catch (Exception ex) {
				string strLogMessage = string.Format("{0} GetSecurity Exception: {1}", DateTime.Now, ex.Message);
				MobileTech.Consts.HandleExceptionLog(strLogMessage);
				errorMessage = strLogMessage;
//				Console.WriteLine (strLogMessage);
				//Console.WriteLine("GetSecurity: "+ex.Message);
				retValue = false;
			}
			//Console.WriteLine ("Security Finished");
			return retValue;
		}


		public bool GetUrgencies (string blockName,ref string errorMsg)
		{
			//Console.WriteLine ("Urgencies Started");
			errorMsg = string.Empty;
			bool retValue = true;
			DateTime syncDate = DateTime.Now;

			try {
				DateTime lastSyncDate = ConfigurationDAL.GetLastSyncDateByblock(blockName,ref errorMsg);

				string responseDeleteResult = JsonWebAPICallForDownloadSync("/UrgencyCodes/GetUrgencyCodesDeleted?",lastSyncDate);
				string[] deleteResult = ((List<string>)JsonConvert.DeserializeObject(responseDeleteResult , typeof(List<string>))).ToArray();
				ConfigurationDAL.DeleteUrgencyItems(deleteResult);

				string responseResult = JsonWebAPICallForDownloadSync("/UrgencyCodes/GetUrgencyCodes?",lastSyncDate);
				List<UrgencyInfo> myObjectList = (List<UrgencyInfo>)JsonConvert.DeserializeObject(responseResult , typeof(List<UrgencyInfo>));
				ConfigurationDAL.InsertUrgencyItems(myObjectList.ToArray(),ref errorMsg);

				ConfigurationDAL.UpdateBlockLastSyncDate(syncDate,blockName);

			} 
			catch (Exception ex) {
				string strLogMessage = string.Format("{0} GetUrgencies Exception: {1}", DateTime.Now, ex.Message);
				MobileTech.Consts.HandleExceptionLog(strLogMessage);
				errorMsg = strLogMessage;
				Console.WriteLine (strLogMessage);
				retValue = false;
			}

			//Console.WriteLine ("Urgencies Finished");
			return retValue;
		}

		public bool GetRequestCodes (string blockName,ref string errorMsg)
		{
			//Console.WriteLine ("RequestCodes Started");
			errorMsg = string.Empty;
			bool retValue = true;
			DateTime syncDate = DateTime.Now;

			try {
				DateTime lastSyncDate = ConfigurationDAL.GetLastSyncDateByblock(blockName,ref errorMsg);

				string responseDeleteResult = JsonWebAPICallForDownloadSync("/RequestCodes/GetRequestCodesDeleted?",lastSyncDate);
				string[] deleteResult = ((List<string>)JsonConvert.DeserializeObject(responseDeleteResult , typeof(List<string>))).ToArray();
				ConfigurationDAL.DeleteRequestItems(deleteResult);

				string responseResult = JsonWebAPICallForDownloadSync("/RequestCodes/GetRequestCodes?",lastSyncDate);
				List<RequestInfo> myObjectList = (List<RequestInfo>)JsonConvert.DeserializeObject(responseResult , typeof(List<RequestInfo>));
				ConfigurationDAL.InsertRequestItems(myObjectList.ToArray(),ref errorMsg);

				ConfigurationDAL.UpdateBlockLastSyncDate(syncDate,blockName);

			} 
			catch (Exception ex) {
				string strLogMessage = string.Format("{0} GetRequestCodes Exception: {1}", DateTime.Now, ex.Message);
				MobileTech.Consts.HandleExceptionLog(strLogMessage);
				errorMsg = strLogMessage;
				Console.WriteLine (strLogMessage);
				retValue = false;
			}
			//Console.WriteLine ("RequestCodes Finished");
			return retValue;
		}

		public bool GetOpenWorkStatus (string blockName,ref string errorMsg)
		{
			errorMsg = string.Empty;
			bool retValue = true;
			DateTime syncDate = DateTime.Now;

			try {
				DateTime lastSyncDate = ConfigurationDAL.GetLastSyncDateByblock(blockName,ref errorMsg);

				string responseDeleteResult = JsonWebAPICallForDownloadSync("/OpenWorkStatusCodes/GetOpenWorkStatusesDeleted?",lastSyncDate);
				string[] deleteResult = ((List<string>)JsonConvert.DeserializeObject(responseDeleteResult , typeof(List<string>))).ToArray();
				ConfigurationDAL.DeleteOpenWorkStatusItems(deleteResult);

				string responseResult = JsonWebAPICallForDownloadSync("/OpenWorkStatusCodes/GetOpenWorkStatuses?",lastSyncDate);
				List<OpenWorkStatusInfo> myObjectList = (List<OpenWorkStatusInfo>)JsonConvert.DeserializeObject(responseResult , typeof(List<OpenWorkStatusInfo>));
				ConfigurationDAL.InsertOpenWorkStatusItems(myObjectList.ToArray(),ref errorMsg);

				ConfigurationDAL.UpdateBlockLastSyncDate(syncDate,blockName);

			} 
			catch (Exception ex) {
				string strLogMessage = string.Format("{0} GetOpenWorkStatus Exception: {1}", DateTime.Now, ex.Message);
				MobileTech.Consts.HandleExceptionLog(strLogMessage);
				errorMsg = strLogMessage;
				Console.WriteLine (strLogMessage);
				retValue = false;
			}
			return retValue;
		}


		public bool GetControlCenter (string blockName,ref string errorMsg)
		{
			errorMsg = string.Empty;
			bool retValue = true;
			DateTime syncDate = DateTime.Now;

			try {
				DateTime lastSyncDate = ConfigurationDAL.GetLastSyncDateByblock(blockName,ref errorMsg);

				string responseDeleteResult = JsonWebAPICallForDownloadSync("/ControlCenterCodes/GetControlCenterCodesDeleted?",lastSyncDate);
				string[] deleteResult = ((List<string>)JsonConvert.DeserializeObject(responseDeleteResult , typeof(List<string>))).ToArray();
				ConfigurationDAL.DeleteAssetCenterItems(deleteResult);

				string responseResult = JsonWebAPICallForDownloadSync("/ControlCenterCodes/GetControlCenterCodes?",lastSyncDate);
				List<SystemCode> myObjectList = (List<SystemCode>)JsonConvert.DeserializeObject(responseResult , typeof(List<SystemCode>));
				ConfigurationDAL.InsertAssetCenterItems(myObjectList.ToArray(),ref errorMsg);

				ConfigurationDAL.UpdateBlockLastSyncDate(syncDate,blockName);

			} 
			catch (Exception ex) {
				string strLogMessage = string.Format("{0} GetControlCenter Exception: {1}", DateTime.Now, ex.Message);
				MobileTech.Consts.HandleExceptionLog(strLogMessage);
				errorMsg = strLogMessage;
//				Console.WriteLine (strLogMessage);
				retValue = false;
			}
			return retValue;
		}

		public bool GetEquipments(string blockName,ref string errorMsg)
		{
			errorMsg = string.Empty;
			bool retValue = true;
			DateTime syncDate = DateTime.Now;

			try {
				DateTime lastSyncDate = ConfigurationDAL.GetLastSyncDateByblock(blockName,ref errorMsg);

				//string responseDeleteResult = JsonWebAPICallForDownloadSync("/Assets/GetAssetsDeleted?",lastSyncDate);
				string responseDeleteResult = JsonWebAPICallForDownloadSync(string.Format(@"/Assets/GetAssetsDeleted?workerID={0}&",MobileTech.Consts.WKRPrimaryId.ToString()),lastSyncDate);
				string[] deleteResult = ((List<string>)JsonConvert.DeserializeObject(responseDeleteResult , typeof(List<string>))).ToArray();
				ConfigurationDAL.DeleteEquipmentItems(deleteResult);

				string responseResult = JsonWebAPICallForDownloadSync(string.Format(@"/Assets/GetAssets?workerID={0}&",MobileTech.Consts.WKRPrimaryId.ToString()),lastSyncDate);
				List<AssetDetails> myObjectList = (List<AssetDetails>)JsonConvert.DeserializeObject(responseResult , typeof(List<AssetDetails>));
				ConfigurationDAL.InsertEquipmentItems(myObjectList.ToArray(),ref errorMsg);

				if(myObjectList.Count > 0){
					ConfigurationDAL.UpdateBlockLastSyncDate(myObjectList[0].LastSyncDateTime,blockName);
				}

			} 
			catch (Exception ex) {
				string strLogMessage = string.Format("{0} GetEquipments Exception: {1}", DateTime.Now, ex.Message);
				MobileTech.Consts.HandleExceptionLog(strLogMessage);
				errorMsg = strLogMessage;
				retValue = false;
			}
			return retValue;
		}

		public bool GetAccounts(string blockName,ref string errorMsg)
		{
			errorMsg = string.Empty;
			bool retValue = true;
			DateTime syncDate = DateTime.Now;

			try {
				DateTime lastSyncDate = ConfigurationDAL.GetLastSyncDateByblock(blockName,ref errorMsg);

				//string responseDeleteResult = JsonWebAPICallForDownloadSync("/Accounts/GetAccountsDeleted?",lastSyncDate);
				string responseDeleteResult = JsonWebAPICallForDownloadSync(string.Format(@"/Accounts/GetAccountsDeleted?workerID={0}&",MobileTech.Consts.WKRPrimaryId.ToString()),lastSyncDate);
				string[] deleteResult = ((List<string>)JsonConvert.DeserializeObject(responseDeleteResult , typeof(List<string>))).ToArray();
				ConfigurationDAL.DeleteAccountItems(deleteResult);

				string responseResult = JsonWebAPICallForDownloadSync(string.Format(@"/Accounts/GetAccounts?workerID={0}&",MobileTech.Consts.WKRPrimaryId.ToString()),lastSyncDate);
				List<AccountSync> myObjectList = (List<AccountSync>)JsonConvert.DeserializeObject(responseResult , typeof(List<AccountSync>));
				ConfigurationDAL.InsertAccountItems(myObjectList.ToArray(),ref errorMsg);

				ConfigurationDAL.UpdateBlockLastSyncDate(syncDate,blockName);

			} 
			catch (Exception ex) {
				string strLogMessage = string.Format("{0} GetAccounts Exception: {1}", DateTime.Now, ex.Message);
				MobileTech.Consts.HandleExceptionLog(strLogMessage);
				errorMsg = strLogMessage;
				retValue = false;
			}
			return retValue;
		}

		public bool GetWkOrders(string blockName,ref string errorMsg)
		{
			errorMsg = string.Empty;
			bool retValue = true;
			DateTime syncDate = DateTime.Now;

			try {
				DateTime lastSyncDate = ConfigurationDAL.GetLastSyncDateByblock(blockName,ref errorMsg);

				string responseDeleteResult = JsonWebAPICallForDownloadSync("/WorkOrder/GetWorkOrdersDeleted?",lastSyncDate);
				string[] deleteResult = ((List<string>)JsonConvert.DeserializeObject(responseDeleteResult , typeof(List<string>))).ToArray();
				ConfigurationDAL.DeleteWkOrderItems(deleteResult);

				string responseResult = JsonWebAPICallForDownloadSync(string.Format(@"/WorkOrder/GetWorkOrders?workerID={0}&",MobileTech.Consts.WKRPrimaryId.ToString()),lastSyncDate);
				List<WorkOrderDetails> myObjectList = (List<WorkOrderDetails>)JsonConvert.DeserializeObject(responseResult , typeof(List<WorkOrderDetails>));
				ConfigurationDAL.InsertWkOrderItems(myObjectList.ToArray(),ref errorMsg);

				if(myObjectList.Count > 0){
					ConfigurationDAL.UpdateBlockLastSyncDate(myObjectList[0].LastSyncDateTime,blockName);
				}
			} 
			catch (Exception ex) {
				string strLogMessage = string.Format("{0} GetWkOrders Exception: {1}", DateTime.Now, ex.Message);
				MobileTech.Consts.HandleExceptionLog(strLogMessage);
				errorMsg = strLogMessage;
				retValue = false;
			}
			return retValue;
		}

		public bool GetVersionDetails (ref string errorMsg)
		{
			//Console.WriteLine ("RequestCodes Started");
			errorMsg = string.Empty;
			bool retValue = true;
			DateTime syncDate = DateTime.Now;

			try {
				string responseResult = JsonWebAPICallForVersionDetails("/VersionDetails");
				List<SystemSetting> myObjectList = (List<SystemSetting>)JsonConvert.DeserializeObject(responseResult , typeof(List<SystemSetting>));

				ConfigurationDAL.deleteSystemSettings(myObjectList.ToArray());
				ConfigurationDAL.insertSystemSettings(myObjectList.ToArray(),ref errorMsg);
			} 
			catch (Exception ex) {
				string strLogMessage = string.Format("{0} GetVersionDetails Exception: {1}", DateTime.Now, ex.Message);
				MobileTech.Consts.HandleExceptionLog(strLogMessage);
				errorMsg = strLogMessage;
				retValue = false;
			}
			return retValue;
		}

		public bool GetResults (string blockName,ref string errorMsg)
		{
			//Console.WriteLine ("Security Started");
			errorMsg = string.Empty;
			bool retValue = true;
			DateTime syncDate = DateTime.Now;

			try {
				DateTime lastSyncDate = ConfigurationDAL.GetLastSyncDateByblock(blockName,ref errorMsg);


				string responseDeleteResult = JsonWebAPICallForDownloadSync("/Results/GetResultsDeleted?",lastSyncDate);
				string[] deleteResult = ((List<string>)JsonConvert.DeserializeObject(responseDeleteResult , typeof(List<string>))).ToArray();
				ConfigurationDAL.DeleteResultItems(deleteResult);

				string responseResult = JsonWebAPICallForDownloadSync("/Results/GetResults?",lastSyncDate);
				List<SystemCode> myObjectList = (List<SystemCode>)JsonConvert.DeserializeObject(responseResult , typeof(List<SystemCode>));
				ConfigurationDAL.InsertResultItems(myObjectList.ToArray(),ref errorMsg);

				ConfigurationDAL.UpdateBlockLastSyncDate(syncDate,blockName);

			} 
			catch (Exception ex) {
				string strLogMessage = string.Format("{0} GetResults Exception: {1}", DateTime.Now, ex.Message);
				MobileTech.Consts.HandleExceptionLog(strLogMessage);
				errorMsg = strLogMessage;
				retValue = false;
			}
			//Console.WriteLine ("Security Finished");
			return retValue;
		}

		public bool GetResultCenters (string blockName,ref string errorMsg)
		{
			errorMsg = string.Empty;
			bool retValue = true;
			DateTime syncDate = DateTime.Now;

			try {
				DateTime lastSyncDate = ConfigurationDAL.GetLastSyncDateByblock(blockName,ref errorMsg);


				string responseDeleteResult = JsonWebAPICallForDownloadSync("/ResultCenters/GetDeletedResultCenters?",lastSyncDate);
				string[] deleteResult = ((List<string>)JsonConvert.DeserializeObject(responseDeleteResult , typeof(List<string>))).ToArray();
				ConfigurationDAL.DeleteResultCenterItems(deleteResult);

				string responseResult = JsonWebAPICallForDownloadSync("/ResultCenters/GetResultCenters?",lastSyncDate);
				List<ResultInfo> myObjectList = (List<ResultInfo>)JsonConvert.DeserializeObject(responseResult , typeof(List<ResultInfo>));
				ConfigurationDAL.InsertResultCenterItems(myObjectList.ToArray(),ref errorMsg);

				ConfigurationDAL.UpdateBlockLastSyncDate(syncDate,blockName);
			} 
			catch (Exception ex) {
				string strLogMessage = string.Format("{0} GetResultCenters Exception: {1}", DateTime.Now, ex.Message);
				MobileTech.Consts.HandleExceptionLog(strLogMessage);
				errorMsg = strLogMessage;
				retValue = false;
			}
			//Console.WriteLine ("Security Finished");
			return retValue;
		}

		public bool GetFaults (string blockName,ref string errorMsg)
		{
			//Console.WriteLine ("Security Started");
			errorMsg = string.Empty;
			bool retValue = true;
			DateTime syncDate = DateTime.Now;

			try {
				DateTime lastSyncDate = ConfigurationDAL.GetLastSyncDateByblock(blockName,ref errorMsg);


				string responseDeleteResult = JsonWebAPICallForDownloadSync("/Faults/GetDeletedFaults?",lastSyncDate);
				string[] deleteResult = ((List<string>)JsonConvert.DeserializeObject(responseDeleteResult , typeof(List<string>))).ToArray();
				ConfigurationDAL.DeleteFaultItems(deleteResult);

				string responseResult = JsonWebAPICallForDownloadSync("/Faults/GetFaults?",lastSyncDate);
				List<SystemCode> myObjectList = (List<SystemCode>)JsonConvert.DeserializeObject(responseResult , typeof(List<SystemCode>));
				ConfigurationDAL.InsertFaultItems(myObjectList.ToArray(),ref errorMsg);

				ConfigurationDAL.UpdateBlockLastSyncDate(syncDate,blockName);

			} 
			catch (Exception ex) {
				string strLogMessage = string.Format("{0} GetFaults Exception: {1}", DateTime.Now, ex.Message);
				MobileTech.Consts.HandleExceptionLog(strLogMessage);
				errorMsg = strLogMessage;
				retValue = false;
			}
			//Console.WriteLine ("Security Finished");
			return retValue;
		}

		public bool GetFaultCenters (string blockName,ref string errorMsg)
		{
			errorMsg = string.Empty;
			bool retValue = true;
			DateTime syncDate = DateTime.Now;

			try {
				DateTime lastSyncDate = ConfigurationDAL.GetLastSyncDateByblock(blockName,ref errorMsg);


				string responseDeleteResult = JsonWebAPICallForDownloadSync("/FaultCenters/GetDeletedFaultCenters?",lastSyncDate);
				string[] deleteResult = ((List<string>)JsonConvert.DeserializeObject(responseDeleteResult , typeof(List<string>))).ToArray();
				ConfigurationDAL.DeleteFaultCenterItems(deleteResult);

				string responseResult = JsonWebAPICallForDownloadSync("/FaultCenters/GetFaultCenters?",lastSyncDate);
				List<FaultInfo> myObjectList = (List<FaultInfo>)JsonConvert.DeserializeObject(responseResult , typeof(List<FaultInfo>));
				ConfigurationDAL.InsertFaultCenterItems(myObjectList.ToArray(),ref errorMsg);

				ConfigurationDAL.UpdateBlockLastSyncDate(syncDate,blockName);

			} 
			catch (Exception ex) {
				string strLogMessage = string.Format("{0} GetFaultCenters Exception: {1}", DateTime.Now, ex.Message);
				MobileTech.Consts.HandleExceptionLog(strLogMessage);
				errorMsg = strLogMessage;
				retValue = false;
			}
			//Console.WriteLine ("Security Finished");
			return retValue;
		}

		public bool GetRequestCenters (string blockName,ref string errorMsg)
		{
			errorMsg = string.Empty;
			bool retValue = true;
			DateTime syncDate = DateTime.Now;

			try {
				DateTime lastSyncDate = ConfigurationDAL.GetLastSyncDateByblock(blockName,ref errorMsg);


				string responseDeleteResult = JsonWebAPICallForDownloadSync("/RequestCenters/GetDeletedRequestCenters?",lastSyncDate);
				string[] deleteResult = ((List<string>)JsonConvert.DeserializeObject(responseDeleteResult , typeof(List<string>))).ToArray();
				ConfigurationDAL.DeleteRequestCenterItems(deleteResult);

				string responseResult = JsonWebAPICallForDownloadSync("/RequestCenters/GetRequestCenters?",lastSyncDate);
				List<RequestInfo> myObjectList = (List<RequestInfo>)JsonConvert.DeserializeObject(responseResult , typeof(List<RequestInfo>));
				ConfigurationDAL.InsertRequestCenterItems(myObjectList.ToArray(),ref errorMsg);

				ConfigurationDAL.UpdateBlockLastSyncDate(syncDate,blockName);

			} 
			catch (Exception ex) {
				string strLogMessage = string.Format("{0} GetRequestCenters Exception: {1}", DateTime.Now, ex.Message);
				MobileTech.Consts.HandleExceptionLog(strLogMessage);
				errorMsg = strLogMessage;
				retValue = false;
			}
			//Console.WriteLine ("Security Finished");
			return retValue;
		}

		public bool GetValidRequestsResults (string blockName,ref string errorMsg)
		{
			errorMsg = string.Empty;
			bool retValue = true;
			DateTime syncDate = DateTime.Now;

			try {
				DateTime lastSyncDate = ConfigurationDAL.GetLastSyncDateByblock(blockName,ref errorMsg);


				string responseDeleteResult = JsonWebAPICallForDownloadSync("/ValidRequestsResults/GetDeletedValidRequestsResults?",lastSyncDate);
				string[] deleteResult = ((List<string>)JsonConvert.DeserializeObject(responseDeleteResult , typeof(List<string>))).ToArray();
				ConfigurationDAL.DeleteValidRequestsResultsItems(deleteResult);

				string responseResult = JsonWebAPICallForDownloadSync("/ValidRequestsResults/GetValidRequestsResults?",lastSyncDate);
				List<ValidRequestsResult> myObjectList = (List<ValidRequestsResult>)JsonConvert.DeserializeObject(responseResult , typeof(List<ValidRequestsResult>));
				ConfigurationDAL.InsertValidRequestsResultsItems(myObjectList.ToArray(),ref errorMsg);

				ConfigurationDAL.UpdateBlockLastSyncDate(syncDate,blockName);

			} 
			catch (Exception ex) {
				string strLogMessage = string.Format("{0} GetValidRequestsResults Exception: {1}", DateTime.Now, ex.Message);
				MobileTech.Consts.HandleExceptionLog(strLogMessage);
				errorMsg = strLogMessage;
				retValue = false;
			}
			//Console.WriteLine ("Security Finished");
			return retValue;
		}

		public bool GetWOTexts(string blockName,ref string errorMsg)
		{
			errorMsg = string.Empty;
			bool retValue = true;
			DateTime syncDate = DateTime.Now;

			try {
				DateTime lastSyncDate = ConfigurationDAL.GetLastSyncDateByblock(blockName,ref errorMsg);

				string responseDeleteResult = JsonWebAPICallForDownloadSync("/WOText/GetWOTextsDeleted?",lastSyncDate);
				string[] deleteResult = ((List<string>)JsonConvert.DeserializeObject(responseDeleteResult , typeof(List<string>))).ToArray();
				ConfigurationDAL.DeleteWOTextItems(deleteResult);

				string responseResult = JsonWebAPICallForDownloadSync(string.Format(@"/WOText/GetWOTexts?workerID={0}&",MobileTech.Consts.WKRPrimaryId.ToString()),lastSyncDate);
				List<WorkOrderText> myObjectList = (List<WorkOrderText>)JsonConvert.DeserializeObject(responseResult , typeof(List<WorkOrderText>));
				ConfigurationDAL.InsertWOTextItems(myObjectList.ToArray(),ref errorMsg);

				ConfigurationDAL.UpdateBlockLastSyncDate(syncDate,blockName);

			} 
			catch (Exception ex) {
				string strLogMessage = string.Format("{0} GetWOTexts Exception: {1}", DateTime.Now, ex.Message);
				MobileTech.Consts.HandleExceptionLog(strLogMessage);
				errorMsg = strLogMessage;
				retValue = false;
			}
			return retValue;
		}

		public bool GetFilter()
		{
			bool retValue = true;
			try {

				ConfigurationDAL.DeleteTable("Filter");

				string responseResult = JsonWebAPICallForVersionDetails("/Filter/GetFilters");
				List<FilterInfo> myfilter = (List<FilterInfo>)JsonConvert.DeserializeObject(responseResult , typeof(List<FilterInfo>));
				Repository repository = new Repository();
				repository.InsertORUpdateFilter(myfilter[0]);
			} 
			catch (Exception ex) {
				string strLogMessage = string.Format("{0} GetFilter Exception: {1}", DateTime.Now, ex.Message);
				MobileTech.Consts.HandleExceptionLog(strLogMessage);
				retValue = false;
			}
			return retValue;
		}

		public bool GetTimeTypes (string blockName,ref string errorMsg)
		{
			//Console.WriteLine ("Security Started");
			errorMsg = string.Empty;
			bool retValue = true;
			DateTime syncDate = DateTime.Now;

			try {
				DateTime lastSyncDate = ConfigurationDAL.GetLastSyncDateByblock(blockName,ref errorMsg);


				string responseDeleteResult = JsonWebAPICallForDownloadSync("/TimeType/GetDeletedTimeTypes?",lastSyncDate);
				string[] deleteResult = ((List<string>)JsonConvert.DeserializeObject(responseDeleteResult , typeof(List<string>))).ToArray();
				ConfigurationDAL.DeleteTimeTypeItems(deleteResult);

				string responseResult = JsonWebAPICallForDownloadSync("/TimeType/GetTimeTypes?",lastSyncDate);
				List<SystemCode> myObjectList = (List<SystemCode>)JsonConvert.DeserializeObject(responseResult , typeof(List<SystemCode>));
				ConfigurationDAL.InsertTimeTypeItems(myObjectList.ToArray(),ref errorMsg);

				ConfigurationDAL.UpdateBlockLastSyncDate(syncDate,blockName);

			} 
			catch (Exception ex) {
				string strLogMessage = string.Format("{0} GetTimeTypes Exception: {1}", DateTime.Now, ex.Message);
				MobileTech.Consts.HandleExceptionLog(strLogMessage);
				errorMsg = strLogMessage;
				retValue = false;
			}
			//Console.WriteLine ("Security Finished");
			return retValue;
		}

		public bool GetPDADBId (string blockName,ref string errorMsg)
		{
			//Console.WriteLine ("Security Started");
			errorMsg = string.Empty;
			bool retValue = true;
			DateTime syncDate = DateTime.Now;

			try {
				var deviceid = UIDevice.CurrentDevice.IdentifierForVendor.AsString();

				var	request = HttpWebRequest.Create (string.Format (@"{0}{1}username={2}", MobileTech.Consts.ServerURL, "/PDADBID/GetPDADBID?", deviceid));

				request.ContentType = "application/json";
				request.Method = "GET";

				request.Headers.Set("SessionID", AuthClient.GetInstance().CurrentSession);

				using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
				{
					if (response.StatusCode != HttpStatusCode.OK)
						Console.Out.WriteLine("Error while fetching {1} data. Server returned status code: {0}", response.StatusCode, "/PDADBID/GetPDADBID?");
					using (StreamReader reader = new StreamReader(response.GetResponseStream()))
					{
						var responseResult = reader.ReadToEnd();
						if(string.IsNullOrWhiteSpace(responseResult)) {
							Console.Out.WriteLine("Response contained empty body...");
						}
						else {
							List<PDADBID> myObjectList = (List<PDADBID>)JsonConvert.DeserializeObject(responseResult , typeof(List<PDADBID>));
							//PDADBID myObjectList = (PDADBID)JsonConvert.DeserializeObject(responseResult , typeof(PDADBID));
							ConfigurationDAL.InsertPDADBID(myObjectList.ToArray(),ref errorMsg);

							ConfigurationDAL.UpdateBlockLastSyncDate(syncDate,blockName);
						}
					}
				}
			} 
			catch (Exception ex) {
				string strLogMessage = string.Format("{0} GetPDADBId Exception: {1}", DateTime.Now, ex.Message);
				MobileTech.Consts.HandleExceptionLog(strLogMessage);
				errorMsg = strLogMessage;
				retValue = false;
			}
			return retValue;
		}


		public bool GetAccountFacilities (string blockName, ref string errorMsg)
		{
			errorMsg = string.Empty;
			bool retValue = true;
			DateTime syncDate = DateTime.Now;

			try {
				DateTime lastSyncDate = ConfigurationDAL.GetLastSyncDateByblock(blockName, ref errorMsg);

				string responseDeleteResult = JsonWebAPICallForDownloadSync("/Facilities/GetFacilitiesDeleted?",lastSyncDate);
				string[] deleteResult = ((List<string>)JsonConvert.DeserializeObject(responseDeleteResult , typeof(List<string>))).ToArray();
				ConfigurationDAL.DeleteAccountFacilityItems(deleteResult);

				string responseResult = JsonWebAPICallForDownloadSync("/Facilities/GetFacilities?",lastSyncDate);
				List<FacilityDetails> myObjectList = (List<FacilityDetails>)JsonConvert.DeserializeObject(responseResult , typeof(List<FacilityDetails>));
				ConfigurationDAL.InsertAccountFacilityItems(myObjectList.ToArray(), ref errorMsg);

				if(myObjectList.Count > 0){
					ConfigurationDAL.UpdateBlockLastSyncDate(myObjectList[0].LastSyncDate, blockName);
				}
			} 
			catch (Exception ex) {
				string strLogMessage = string.Format("{0} GetAccountFacilities Exception: {1}", DateTime.Now, ex.Message);
				MobileTech.Consts.HandleExceptionLog(strLogMessage);
				errorMsg = strLogMessage;
				retValue = false;
			}
			return retValue;
		}

		public bool GetWorkers (string blockName, ref string errorMsg)
		{
			errorMsg = string.Empty;
			bool retValue = true;
			DateTime syncDate = DateTime.Now;

			try {
				DateTime lastSyncDate = ConfigurationDAL.GetLastSyncDateByblock(blockName, ref errorMsg);

				string responseDeleteResult = JsonWebAPICallForDownloadSync("/Workers/GetWorkersDeleted?",lastSyncDate);
				string[] deleteResult = ((List<string>)JsonConvert.DeserializeObject(responseDeleteResult , typeof(List<string>))).ToArray();
				ConfigurationDAL.DeleteWorkerItems(deleteResult);

				string responseResult = JsonWebAPICallForDownloadSync("/Workers/GetWorkers?",lastSyncDate);
				List<WorkerInfo> myObjectList = (List<WorkerInfo>)JsonConvert.DeserializeObject(responseResult , typeof(List<WorkerInfo>));
				ConfigurationDAL.InsertWorkerItems(myObjectList.ToArray(),ref errorMsg);

				if(myObjectList.Count > 0){
					ConfigurationDAL.UpdateBlockLastSyncDate(myObjectList[0].LastSyncDate, blockName);
				}

			} 
			catch (Exception ex) {
				string strLogMessage = string.Format("{0} GetWorkers Exception: {1}", DateTime.Now, ex.Message);
				MobileTech.Consts.HandleExceptionLog(strLogMessage);
				errorMsg = strLogMessage;
				retValue = false;
			}
			return retValue;
		}
	}
}

