using System;
using System.Diagnostics;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.IO;
using System.Reflection;
using System.Net;
//using System.Web.Services;
using System.Threading;
using MonoTouch.ObjCRuntime;
using System.Collections.Generic;

using MobileTech.Core;
using MobileTechService;
using MobileTech.Models.Entity;
using System.Linq;
using System.Threading.Tasks;
using MobileTech.Core.MobileConnectService;
using Xamarin;



namespace MobileTech.iOS.Screens
{
	public partial class LoginViewController : UIViewController
	{
		//for ActivityIndicator
		//LoadingOverlay loadingOverlay;
		protected LoadingOverlay _loadPop = null;

		public string ctrlUserNameVal = string.Empty;
		public string ctrlPassWordVal = string.Empty;
		public string ctrlServerAddressVal = string.Empty;
		public string ctrlServerCode = string.Empty;

		public MobileTech.Consts.enumUserExceptionType exceptionType = MobileTech.Consts.enumUserExceptionType.None;
		public string exceptionMessage = string.Empty;

		private readonly AuthClient authClient = AuthClient.GetInstance();
		private const int _maxConnectionTryCount = 5;
	
		private Configuration config = new Configuration();
		//public bool isVisibleBtnUploadLogToServer=false;
		bool logoutFromMenu = false;

		static bool UserInterfaceIdiomIsPhone {
			get { return UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone; }
		}

		public LoginViewController (bool logoutFromMenu)
			: base ("LoginViewController_iPhone", null)
		{
			this.logoutFromMenu = logoutFromMenu;
		}


		public LoginViewController ()
			: base (UserInterfaceIdiomIsPhone ? "LoginViewController_iPhone" : "LoginViewController_iPad", null)
		{
		}

		partial void btnPurgeDBClicked (NSObject sender)
		{
			//TODO:28th Upload
//			MobileTech.DispatchHandler dispatchHandler = new MobileTech.DispatchHandler();
			//if(!dispatchHandler.CheckWsMessageConnector()){
			if(!false){
				var alert = new UIAlertView(); 
				alert.Title = "Reset Data"; 
				alert.Message = "The entire data will be deleted from your Database? Do you want to continue?"; 

				alert.AddButton("No"); 
				alert.AddButton("Yes"); 
				alert.Show();
				alert.Clicked += (s, e) => { 
				if (e.ButtonIndex == 1) { 
					MobileTech.Consts.LoginUserName = string.Empty;
					MobileTech.Consts.LoginUserPwd = string.Empty;
					this.txtUsername.Text =string.Empty;
					this.txtPassword.Text = string.Empty;
					var user = NSUserDefaults.StandardUserDefaults;
					user.SetString(string.Empty,"txtUserName");
					user.SetString(string.Empty,"txtPassWord");

					if(!string.IsNullOrEmpty(txtServerAddress.Text)){
						if(MobileConnService.IsCode(txtServerAddress.Text.Trim()))
						{
							user.SetString(this.txtServerAddress.Text.Trim(),"txtServerCode");
						}
						else
						{
							user.SetString(this.txtServerAddress.Text,"txtServerURL");
						}
					}

					UIApplication.SharedApplication.ApplicationIconBadgeNumber = 0;
					MobileTech.Consts.DeletePreviousUsersData();
					this.btnPurgeDB.Enabled = false;
					
					MobileTech.Consts.HandleExceptionLog(MobileTech.Consts.enumLogHandle.Delete);
					btnUploadLogToServer.Enabled = false;
					Consts.isVisibleBtnUploadLogToServer = false;
				}else if (e.ButtonIndex == 0){ 
					alert.DismissWithClickedButtonIndex(alert.CancelButtonIndex,false);
				} 
			};
			}else{
				ShowAlertMessag("There are Message(s) in the Queue, which have to be processed. Reset Data after some time.");
			}


		}

		partial void flipToServerSetup (NSObject sender)
		{
			mAnimationContainer.AddSubview(mServerSetupView);

			//Author/Date/Issue - M.D.Prasad/14th May 2014/ISS - 5395 or CRR - 1338  Upload iGotIt device logs
			//Author/Date/Issue - M.D.Prasad/26th May 2014/26353. ISS-5395 [CRR-1338 v1.4] 
			//Upload Log button is not getting dsiabled when Wi-Fi is lost and when button is clicked, app is getting crashed.
			//TODO:28th
			//Uri _remoteServerUrl = new Uri(MobileTech.Consts.ServerURL + "/AuthenticationService.asmx");
			//if ((MobileTech.Consts.IsWebApiUrlReachable (_remoteServerUrl, "AuthenticationService.asmx")) && (Consts.isVisibleBtnUploadLogToServer))
			Uri _remoteServerUrl = new Uri(MobileTech.Consts.ServerURL + "/AuthenticationService");
			if ((MobileTech.Consts.IsWebApiUrlReachable (_remoteServerUrl, "AuthenticationService")) && (Consts.isVisibleBtnUploadLogToServer))
				this.btnUploadLogToServer.Enabled = true;
			else
				this.btnUploadLogToServer.Enabled = false;
		}

		//Author/Date/Issue - M.D.Prasad/14th May 2014/ISS - 5395 or CRR - 1338  Upload iGotIt device logs
		partial void btnUploadLogToServerClick (NSObject sender)
		{
			//Author/Date/Issue - M.D.Prasad/25th June 2014/26353. ISS-5395 [CRR-1338 v1.4] 
			//Upload Log button is not getting dsiabled when Wi-Fi is lost and when button is clicked, app is getting crashed.
			//TODO:28th
			//Uri _remoteServerUrl = new Uri(MobileTech.Consts.ServerURL + "/AuthenticationService.asmx");
			//if (MobileTech.Consts.IsUrlReachable (_remoteServerUrl, "AuthenticationService.asmx")) {
			Uri _remoteServerUrl = new Uri(MobileTech.Consts.ServerURL + "/AuthenticationService");
			if (MobileTech.Consts.IsWebApiUrlReachable (_remoteServerUrl, "AuthenticationService")){
				if (string.IsNullOrEmpty (MobileTech.Consts.ExceptionLogFilePath))
					MobileTech.Consts.HandleExceptionLog (MobileTech.Consts.enumLogHandle.None);

				byte[] buff = FileToByteArray(MobileTech.Consts.ExceptionLogFilePath);
				FileInfo flInfo = new FileInfo(MobileTech.Consts.ExceptionLogFilePath);

				authClient.UploadExceptionLogFromDevice(flInfo.Name, buff);
			}
			else
			{
				ShowAlertMessag("Unable to upload log. No network found.");
			}
		}

		public byte[] FileToByteArray(string fileName)
		{

			byte[] buff = null;
			FileStream fs = new FileStream(fileName, 
				FileMode.Open, 
				FileAccess.Read);
			BinaryReader br = new BinaryReader(fs);
			long numBytes = new FileInfo(fileName).Length;
			buff = br.ReadBytes((int) numBytes);
			return buff;
		}

		partial void btnLoginAction (MonoTouch.Foundation.NSObject sender)
		{
			if(this.txtUsername.Text == string.Empty || this.txtPassword.Text == string.Empty)
			{
				if(this.txtUsername.Text == string.Empty && this.txtPassword.Text != string.Empty)
				{
					txtUsername.BecomeFirstResponder();
					ShowAlertMessag("Please enter a valid Login Username");
				}
				else if(this.txtUsername.Text != string.Empty && this.txtPassword.Text == string.Empty){
					txtPassword.BecomeFirstResponder();
					ShowAlertMessag("Please enter a valid Password.");
				}

				if(this.txtUsername.Text == string.Empty && this.txtPassword.Text == string.Empty){
					txtUsername.BecomeFirstResponder();
					ShowAlertMessag("Please enter valid Login Username and Password.");
				}

				var user = NSUserDefaults.StandardUserDefaults;
				user.SetString(string.Empty,"txtUserName");
				user.SetString(string.Empty,"txtPassWord");

				//				activityMonitor.StopAnimating();
				//				UIApplication.SharedApplication.NetworkActivityIndicatorVisible = false;
			}
			else
			{
			// show the loading overlay on the UI thread
			this._loadPop = new LoadingOverlay (UIScreen.MainScreen.Bounds);
			this.View.Add ( this._loadPop );

			ctrlUserNameVal = this.txtUsername.Text.Trim();
			ctrlPassWordVal = this.txtPassword.Text.Trim();
//			ctrlServerAddressVal = this.txtServerAddress.Text.Trim();
				ctrlServerAddressVal = Consts.ServerURL;

			// spin up a new thread to do some long running work using StartNew
		    Task.Factory.StartNew (
				// tasks allow you to use the lambda syntax to pass work
				() => {
					Console.WriteLine ( "Calling LoginAction on Thread" );
					LoginActionOnThread();
				}
				// ContinueWith allows you to specify an action that runs after the previous thread
				// completes
				// 
				// By using TaskScheduler.FromCurrentSyncrhonizationContext, we can make sure that 
				// this task now runs on the original calling thread, in this case the UI thread
				// so that any UI updates are safe. in this example, we want to hide our overlay, 
				// but we don't want to update the UI from a background thread.
				).ContinueWith ( 
				t => {
					this._loadPop.Hide ();
					Console.WriteLine ( "Finished, hiding our loading overlay from the UI thread." );
					
					if(exceptionType.Equals(MobileTech.Consts.enumUserExceptionType.None))
						{
							var woListScreen  = new WkOrderListViewController();//new WoListViewController();
							this.NavigationController.PushViewController(woListScreen, true);

							Console.WriteLine("Login Successful");
						}
					else if(exceptionType.Equals(MobileTech.Consts.enumUserExceptionType.LocalLogin))
						{
							UIAlertView alrtMsg = new UIAlertView();
							alrtMsg.AddButton("OK"); 
							alrtMsg.Message = "Web Server is not reachable. Application will display in the last known state.";
							alrtMsg.Clicked += (s, e) => { 
								if (e.ButtonIndex == 0) 
								{ 
									Console.WriteLine("Local Login Success");

									var woListScreen  = new WkOrderListViewController();//new WoListViewController();
									this.NavigationController.PushViewController(woListScreen, true);
								}
							};
							alrtMsg.Show();
						}
					else
						handleUserException();

				}, TaskScheduler.FromCurrentSynchronizationContext()
			);
			}
		}

		public void LoginActionOnThread()
		{
			string errorMessage = string.Empty;

				try {
					//TODO:28th
					//string tokenId = MobileTech.Consts.MyDeviceToken;
					string tokenId = "";
					string bundleId = NSBundle.MainBundle.BundleIdentifier;

					//					MobileTech.Consts.signonStatus = authClient.SignOn(this.txtUsername.Text,this.txtPassword.Text,ref errorMessage, bundleId, tokenId, MobileTech.Consts.app_AppVersion, MobileTech.Consts.app_wsVersion);
				MobileTech.Consts.signonStatus = authClient.SignOn(ctrlUserNameVal, ctrlPassWordVal, ref errorMessage, MobileTech.Consts.app_AppVersion, MobileTech.Consts.app_wsVersion);

					//MobileTech.Consts.signonStatus = authClient.SignOn(this.txtUsername.Text,this.txtPassword.Text,ref errorMessage);
					//activityMonitor.StopAnimating();

					if(MobileTech.Consts.signonStatus == "NoConnectionButDBIsNotEmpty"){
					if(MobileTech.Consts.LoginUserName.Trim().ToLower() == ctrlUserNameVal.ToLower()){
						MobileTech.Consts.LoginUserName = ctrlUserNameVal.ToLower();
						MobileTech.Consts.LoginUserPwd = ctrlPassWordVal;
						MobileTech.Consts.ServerURL = ctrlServerAddressVal;

							var user = NSUserDefaults.StandardUserDefaults;
							user.SetString(ctrlUserNameVal,"txtUserName");
							//Modified by Srikanth Nuvvula on BA#26980, password cannot be reset from application
							//user.SetString(this.txtPassword.Text,"txtPassWord");
						    user.SetString(Consts.ServerCode,"txtServerCode");

							exceptionType=Consts.enumUserExceptionType.LocalLogin;

						}else{
							var user = NSUserDefaults.StandardUserDefaults;
							user.SetString(string.Empty,"txtUserName");
							user.SetString(string.Empty,"txtPassWord");

							exceptionType = Consts.enumUserExceptionType.FireAlert;
							exceptionMessage = "Web Server is not reachable. Login user cannot be changed in offline mode.";
							//ShowAlertMessag("Web Server is not reachable. Login user cannot be changed in offline mode.");
						}
					}
					else if (MobileTech.Consts.signonStatus == "true")
					{
					    exceptionType = MobileTech.Consts.enumUserExceptionType.None;
						try{
							bool isPreviousUserDataUploaded;
							//Set sisPreviousUserDataUploaded to true only when the two users are different.
						if(MobileTech.Consts.LoginUserName.Trim().ToLower() == ctrlUserNameVal.ToLower()){
							//Instead of saving username and serverurl in local db, we are saving in root.plist (nsuserdefaults).
							//Security.insertSystemSetup(ctrlUserNameVal.ToLower(), ctrlServerAddressVal);
								//Nahid@Jan16 Start Test
								var user = NSUserDefaults.StandardUserDefaults;
								user.SetString(ctrlUserNameVal,"txtUserName");
								user.SetString(ctrlServerAddressVal,"txtServerURL");
								user.SetString(Consts.ServerCode,"txtServerCode");
								//Nahid@Jan16 End Test
								isPreviousUserDataUploaded = false;
							}else{
								//TODO:28th
								//MobileTech.DispatchHandler dispatchHandler = new MobileTech.DispatchHandler();
								//TODO:28th
								//isPreviousUserDataUploaded = dispatchHandler.CheckWsMessageConnector();
								isPreviousUserDataUploaded = false;
							}

							//insert into SystemSetup table only when login is for the first time or login user is changed.
							if(MobileTech.Consts.LoginUserName.Trim() == string.Empty){
							//Instead of saving username and serverurl in local db, we are saving in root.plist (nsuserdefaults).
							//Security.insertSystemSetup(ctrlUserNameVal.ToLower(), ctrlServerAddressVal);
								//Nahid@Jan16 Start Test
								var user = NSUserDefaults.StandardUserDefaults;
								user.SetString(ctrlUserNameVal,"txtUserName");
								user.SetString(ctrlServerAddressVal,"txtServerURL");
							    user.SetString(Consts.ServerCode,"txtServerCode"); 
								//Nahid@Jan16 End Test

								//Below assignments are required considering that first time login
								//will assign the global values as Security.getSystemSetup() is called only on ViewLoad.
							MobileTech.Consts.LoginUserName = ctrlUserNameVal.ToLower();
							MobileTech.Consts.LoginUserPwd = ctrlPassWordVal;
							MobileTech.Consts.ServerURL = ctrlServerAddressVal;
							}
						else if((MobileTech.Consts.LoginUserName.Trim().ToLower() != ctrlUserNameVal.ToLower()) || (MobileTech.Consts.ServerURL.Trim().ToLower() != ctrlServerAddressVal.ToLower())){
								//BackGround Sync Condition#2: There should be no data for upload in local DB
								if(!isPreviousUserDataUploaded){
								//Instead of saving username and serverurl in local db, we are saving in root.plist (nsuserdefaults).
								//Security.insertSystemSetup(ctrlUserNameVal, ctrlServerAddressVal);
									//Nahid@Jan16 Start Test
									var user = NSUserDefaults.StandardUserDefaults;
									user.SetString(ctrlUserNameVal,"txtUserName");
									user.SetString(ctrlServerAddressVal,"txtServerURL");
									//Modified by Srikanth Nuvvula on 25thSep 2014 for BA#26980
									//When login User changed password should be reset to empty
									//Nahid Ahmed, 23Jan,2013 Reset password in settings when the login user is changed.
									//user.SetString(this.txtPassword.Text,"txtPassWord");
									//Nahid@Jan16 End Test

									user.SetString(string.Empty,"txtPassWord");
								    user.SetString(Consts.ServerCode,"txtServerCode");

									MobileTech.Consts.DeletePreviousUsersData();
									MobileTech.Consts.DeletePreviousUsersMsgQueue();
									MobileTech.Consts.LoginUserName = ctrlUserNameVal.ToLower();
									MobileTech.Consts.LoginUserPwd = ctrlPassWordVal;
									MobileTech.Consts.ServerURL = ctrlServerAddressVal;
								}else{
									exceptionType = Consts.enumUserExceptionType.FireAlert;
									exceptionMessage = @"Unable to Login into the application. There are some records pending to be uploaded to the Server.
				                                                          Please Login with the previous User who was logged into the application.";
									//ShowAlertMessag(@"Unable to Login into the application. There are some records pending to be uploaded to the Server.
				                                                          //Please Login with the previous User who was logged into the application.");
								}
							}

							MobileTech.Consts.HandleExceptionLog(MobileTech.Consts.enumLogHandle.Manipulate);

							if (authClient.SessionState == AuthClient.SessionStateType.SignedOnRemote){
								if(!isPreviousUserDataUploaded){
								/*
								if(MobileTech.Consts.LoginUserName.Trim().ToLower() == ctrlUserNameVal.ToLower()){
									WsMessageConnector messageQueue = new WsMessageConnector();
									if (messageQueue != null) {
										// messageQueue.Start ();
										Uri _remoteServerUrl = new Uri (Consts.ServerURL + "/AuthenticationService");
										if (MobileTech.Consts.IsWebApiUrlReachable (_remoteServerUrl, "/AuthenticationService")) {
											//Consts.signonStatus = "true";
											messageQueue.ForceSync ();
										}
									}
								}
								*/
								getSyncData();

								/* Nahid Ahmed Jan 8,2015: DateFormat can be taken directly from the device
									//Culture set with DateFormat settings from Database.
									string strSettingField="DateFormat";
									//TODO:28th
									//									string dateFormat = Security.getSystemSettingForKey(strSettingField); 
									string dateFormat ="DDMMYYYY";

									System.Globalization.CultureInfo mTechCulture = (System.Globalization.CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
									mTechCulture.DateTimeFormat.ShortTimePattern = "h:mm tt";
									mTechCulture.DateTimeFormat.DateSeparator = "/";

									if (!string.IsNullOrEmpty(dateFormat))
									{
										if (dateFormat == "DDMMYYYY")
										{
											mTechCulture.DateTimeFormat.ShortDatePattern = "dd/MM/yyyy";
										}
										else if (dateFormat == "MMDDYYYY")
										{
											mTechCulture.DateTimeFormat.ShortDatePattern = "MM/dd/yyyy"; 
										}
									}
									else
										mTechCulture.DateTimeFormat.ShortDatePattern = MobileTech.Consts.defaultDateFormat;

									Thread.CurrentThread.CurrentCulture = mTechCulture;
									Thread.CurrentThread.CurrentUICulture = mTechCulture;
								*/

									//TODO:28th
									/*
									 	if(dispatchListScreen == null){
											dispatchListScreen = new iGotIt.Screens.DispatchListScreen();
											dispatchListScreen.View.Frame = new System.Drawing.RectangleF(0, UIApplication.SharedApplication.StatusBarFrame.Height, UIScreen.MainScreen.ApplicationFrame.Width, UIScreen.MainScreen.ApplicationFrame.Height);
										}
										this.NavigationController.PushViewController(dispatchListScreen, false);
										if (Consts.OpenID != 0)
										{
											AutoOpenID();
										}
									*/

									//TODO: Once Profile Settings implemented
									/*Repository repository = new Repository();
									List<WorkOrder> wkOrders = repository.GetWorkOrders();
									List<KeyValuePair<long, string>> tableItems = wkOrders.Select(it => new KeyValuePair<long, string> (it.WorPrimaryId, it.Number)).ToList();

									//var selectListScreen  = new SelectListScreen("Facility", tableItems, facilityId);
									var selectListScreen  = new SelectListScreen("Work Orders", tableItems, null);
									selectListScreen.ItemSelected += (object s, KeyValuePair<long, string> e) => {
										facilityId = e.Key;
										FacilityLabel.Text = e.Value;
										UpdateAvailableRoutes();
									};

									this.NavigationController.PushViewController(selectListScreen, true);
									*/

//									var woListScreen  = new WoListViewController();
//									this.NavigationController.PushViewController(woListScreen, true);
//
//									Console.WriteLine("Login Successful");

								}
							}else{
							exceptionType = MobileTech.Consts.enumUserExceptionType.SignedOnRemote;
							}

							Consts.isVisibleBtnUploadLogToServer=true;
						    string strLogMessageForConnect = string.Format("{0} Successfully connected to Web Service URL: {1}", DateTime.Now, ctrlServerAddressVal);
							MobileTech.Consts.HandleExceptionLog(strLogMessageForConnect);
						    Insights.Track("Login View Controller","LoginActionOnThread",strLogMessageForConnect);
							//TODO:28th
							//							string strLogMessageForLogin = string.Format("{0} Login User Name/Token Id/Session Id: {1}/{2}/{3}", DateTime.Now, MobileTech.Consts.LoginUserName.ToUpper(), MobileTech.Consts.MyDeviceToken, AuthClient.GetInstance().CurrentSession);
							string strLogMessageForLogin = string.Format("{0} Login User Name/Session Id: {1} / {2}", DateTime.Now, MobileTech.Consts.LoginUserName.ToUpper(), AuthClient.GetInstance().CurrentSession);
							MobileTech.Consts.HandleExceptionLog(strLogMessageForLogin);
						    Insights.Track("Login View Controller","LoginActionOnThread",strLogMessageForLogin);
						}
						catch (Exception ex) {
							string strLogMessage = string.Format("{0} btnLoginAction Exception: {1}", DateTime.Now, ex.Message);
							MobileTech.Consts.HandleExceptionLog(strLogMessage);
						    Insights.Report(ex);
						}
					}else{
						exceptionType =	MobileTech.Consts.enumUserExceptionType.SignonStatus;
					}
				}  
				catch (Exception) {
					//						activityMonitor.StopAnimating();
					//Console.WriteLine("Btn Login click: "+ex.Message);

					//Author/Date/Issue - M.D.Prasad/14th May 2014/ISS - 5395 or CRR - 1338  Upload iGotIt device logs
					//					string strLogMessage = string.Format("{0} btnLoginAction Exception: {1}.", DateTime.Now, ex.Message);
					//					MobileTech.Consts.HandleExceptionLog(strLogMessage);
				}
				finally{
				}
			//}
		}

		public void handleUserException()
		{
			switch (exceptionType)
			{
			case Consts.enumUserExceptionType.SignedOnRemote: 
				{
					string alrtMsg = string.Empty;
					if (MobileTech.Consts.InvalidUserCredentials) {
						alrtMsg = "Unable to login to the application with given credentials.";
						initializeUISettingValues ();
					} else {
						alrtMsg = "Web Server is not reachable. Application will display in last known state.";
					}
					ShowAlertMessag (alrtMsg);
				}
				break;

			case Consts.enumUserExceptionType.SignonStatus: 
				{
					string alrtMsg = string.Empty;
					if (MobileTech.Consts.signonStatus == "NoConnection") {
						if (MobileTech.Consts.InvalidUserCredentials) {
							alrtMsg = "Please enter valid login credentials.";
							var user = NSUserDefaults.StandardUserDefaults;
							user.SetString (string.Empty, "txtUserName");
							user.SetString (string.Empty, "txtPassWord");
						} else {
							alrtMsg = "Web Server is not reachable. Please check connection to your Web Server before login.";
						}
					} else if (MobileTech.Consts.signonStatus == "UserIsArchived") {
						alrtMsg = "The User is archived. Please contact Administrator.";
						initializeUISettingValues ();
					} else if (MobileTech.Consts.signonStatus == "InvalidMTechUser") {
						alrtMsg = "The specified User does not have access to MobileTech. Please contact Administrator.";
						initializeUISettingValues ();
					} else if (MobileTech.Consts.signonStatus == "ServerDBNotAccessbile") {
						alrtMsg = "Server Database is not accessible.";
						initializeUISettingValues ();
					} else if (MobileTech.Consts.signonStatus == "A network-related or instance-specific error") {
						alrtMsg = @"A network-related or instance-specific error occurred while establishing a connection to SQL Server. 
				                                            The server was not found or was not accessible. Verify that the instance name is correct and that SQL Server
				                                            is configured to allow remote connections";
						initializeUISettingValues ();
					} else if (MobileTech.Consts.signonStatus == "InvalidLoginCredentials") {
						alrtMsg = "Unable to login to the application with given credentials.";
						initializeUISettingValues ();
					} else if (MobileTech.Consts.signonStatus == "InValidWindowsLoginUser") {
						alrtMsg = "This User is mapped to a Windows User. Please use Windows User account name/password.";
						initializeUISettingValues ();
					} else if (MobileTech.Consts.signonStatus == "Not supported Content") {
						alrtMsg = "The web service is modified and contains some not supported contents.";
						initializeUISettingValues ();
					} else if (MobileTech.Consts.signonStatus == "This application is not compatible with the specified Web Service") {
						alrtMsg = "This application is not compatible with the specified Web Service. Please install the correct application or connect to correct web service.";
						initializeUISettingValues ();
					} else {
						alrtMsg = "Please enter a valid Login User Name.";
						initializeUISettingValues ();
					}
					ShowAlertMessag (alrtMsg);
				}
				break;

			case Consts.enumUserExceptionType.FireAlert:
			case Consts.enumUserExceptionType.DownloadFailed:
				{
					ShowAlertMessag (exceptionMessage);
				}
				break;
			}
		}

		public void initializeUISettingValues()
		{
			this.txtUsername.Text = string.Empty;
			this.txtPassword.Text = string.Empty;
			this.txtUsername.BecomeFirstResponder ();
			var user = NSUserDefaults.StandardUserDefaults;
			user.SetString (string.Empty, "txtUserName");
			user.SetString (string.Empty, "txtPassWord");
		}


		public void getSyncData()
		{
			string errorMessage = string.Empty;
			try 
			{
				config.GetSecurity("Security",ref errorMessage);
				MobileTech.Consts.WKRPrimaryId = Repository.getWorkerIDForLoginUser(MobileTech.Consts.LoginUserName,ref errorMessage);
				config.GetUrgencies("Codes_Urgency",ref errorMessage);
				config.GetRequestCodes("Codes_Request",ref errorMessage);
				config.GetOpenWorkStatus("Codes_OpenWorkStatus",ref errorMessage);
				config.GetControlCenter("Codes_ControlCenter",ref errorMessage);
				config.GetEquipments("Equipment",ref errorMessage);
				config.GetAccounts("Account",ref errorMessage);
				config.GetWkOrders("WkOrders",ref errorMessage);
				config.GetResults("Codes_Result",ref errorMessage);
				config.GetResultCenters("Codes_ResultCenter",ref errorMessage);
				config.GetFaults("Codes_Fault",ref errorMessage);
				config.GetFaultCenters("Codes_FaultCenter",ref errorMessage);
				config.GetRequestCenters("Codes_RequestCenter",ref errorMessage);
				config.GetValidRequestsResults("Codes_ValidRequestsResults",ref errorMessage);

				config.GetWOTexts("WOTexts",ref errorMessage);
				config.GetTimeTypes("Codes_WorkOrderTimeType",ref errorMessage);

				config.GetWorkers("Workers", ref errorMessage);
				config.GetAccountFacilities("AccountFacility", ref errorMessage);

				if(!Repository.isPDADBIDExists())
					config.GetPDADBId("PDADbid",ref errorMessage);

				config.GetVersionDetails(ref errorMessage);
				config.GetFilter();
				//config.GetSystemSettingsFromService();
			}catch (Exception ex) {
				string strLogMessage = string.Format("{0} getSyncData Exception: {1}", DateTime.Now, ex.Message);
				MobileTech.Consts.HandleExceptionLog(strLogMessage);
				Insights.Report (ex, "getSyncData", errorMessage);
				exceptionType =	MobileTech.Consts.enumUserExceptionType.DownloadFailed;
				exceptionMessage = ex.Message;
//				UIAlertView downloadFaild = new UIAlertView();
//				downloadFaild.Message = ex.Message;
//				downloadFaild.AddButton("Ok"); 
//				downloadFaild.Show();
			}
		}


		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
			// Release any cached data, images, etc that aren't in use.
		}

		partial void flipToLogin(NSObject sender)
		{
			string webServerURL = txtServerAddress.Text.Trim() + @"/AuthenticationService";
			string currentServerUrl = string.Empty;
			string currentServerCode = string.Empty;
			bool codeURLReachable = false;

			if(txtServerAddress.Text.Length==0){
				UIAlertView alert=new UIAlertView("Oops","Please enter Code or URL.",null,"OK",null);
				alert.Show();
			}
			else
			{
				try
				{
					MobileTech.Consts.NetworkStatus internetStatus = MobileTech.Consts.Reachability.InternetConnectionStatus();
					if(internetStatus == MobileTech.Consts.NetworkStatus.NotReachable) {
						codeURLReachable = false;
						UIAlertView alert=new UIAlertView("Invalid Data","The Web Server is not reachable. Please verify the Code/URL or check the network configuration.", null, "OK",null);
						alert.Clicked += (alertSender, buttonArgs) =>  { 
							if(string.IsNullOrEmpty(Consts.ServerCode)){
								if(!string.IsNullOrEmpty(Consts.ServerURL)){
									txtServerAddress.Text = Consts.ServerURL; 
								}
							}else{
								txtServerAddress.Text = Consts.ServerCode;
							}
						};
						alert.Show();
					}else{
						if (MobileConnService.IsCode(this.txtServerAddress.Text.Trim())){
							using (MobileConnectService mobileConnectService = new MobileConnectService(Consts.mobileConnectURL)){
								string serverUrl = mobileConnectService.GetUrl(this.txtServerAddress.Text.Trim(), "MobileTech");
								currentServerCode = this.txtServerAddress.Text.Trim();
								currentServerUrl = serverUrl;
								webServerURL = serverUrl.Trim() + @"/AuthenticationService";
								codeURLReachable = true;
							}
						}else{
							currentServerUrl = this.txtServerAddress.Text;
							currentServerCode = string.Empty;
							Uri _remoteServerUrl = new Uri(this.txtServerAddress.Text.Trim() + @"/AuthenticationService");
							if (MobileTech.Consts.IsWebApiUrlReachable (_remoteServerUrl, "AuthenticationService")) {
								codeURLReachable = true;
							}else{
								codeURLReachable = false;
							}
						}

						if (codeURLReachable) {
							//When Web Server Url Changed the Data should be reset.
							//                        if(txtServerAddress.Text.Trim() != MobileTech.Consts.ServerURL.Trim())
							Insights.Identify (txtServerAddress.Text.Trim(), "Name", currentServerUrl);
							if(currentServerUrl != Consts.ServerURL.Trim())
							{
								if(!false)
								{
									MobileTech.Consts.LoginUserName = string.Empty;
									MobileTech.Consts.LoginUserPwd = string.Empty;
									this.txtUsername.Text =string.Empty;
									this.txtPassword.Text = string.Empty;
									// When data reset username and password should be also be reset in settings screen 
									var user = NSUserDefaults.StandardUserDefaults;
									user.SetString(string.Empty,"txtUserName");
									user.SetString(string.Empty,"txtPassWord");
									UIApplication.SharedApplication.ApplicationIconBadgeNumber = 0;
									MobileTech.Consts.DeletePreviousUsersData();
									this.btnPurgeDB.Enabled = false;
									//                                if(!string.IsNullOrEmpty(txtServerAddress.Text))
									//                                    MobileTech.Consts.ServerURL=txtServerAddress.Text;

									if(!string.IsNullOrEmpty(currentServerUrl)){
										Consts.ServerURL=currentServerUrl;
									}

									if(!string.IsNullOrEmpty(currentServerCode)){
										Consts.ServerCode=currentServerCode;
									}else{
										Consts.ServerCode=string.Empty;
									}

									txtUsername.BecomeFirstResponder();
									mAnimationContainer.AddSubview(loginView);

									MobileTech.Consts.HandleExceptionLog(MobileTech.Consts.enumLogHandle.Delete);
									this.btnUploadLogToServer.Enabled = false;
									Consts.isVisibleBtnUploadLogToServer = false;
								}
								else{
									UIAlertView alert=new UIAlertView("","There are Message(s) in the Queue, which have to be processed. Please try to change Code or URL after some time.", null, "OK",null);

									alert.Clicked += (alertSender, buttonArgs) =>  { 
										if(!string.IsNullOrEmpty(MobileTech.Consts.ServerURL))
											txtServerAddress.Text = MobileTech.Consts.ServerURL;
									};

									alert.Show();
									//ShowAlertMessag("There are Message(s) in the Queue, which have to be processed. Reset Data after some time.");
									//                                if(!string.IsNullOrEmpty(MobileTech.Consts.ServerURL))
									//                                    txtServerAddress.Text = MobileTech.Consts.ServerURL;
								}
							}
							else
							{
								txtUsername.BecomeFirstResponder();
								mAnimationContainer.AddSubview(loginView);
							}
						}
						else{
							UIAlertView alert=new UIAlertView("Invalid Data","The Web Server is not reachable. Please verify the Code/URL or check the network configuration.", null, "OK",null);
							alert.Show();
						}
					}

					//txtUsername.BecomeFirstResponder();
					//mAnimationContainer.AddSubview(loginView);

					//webservice have work with ServerURL. 
					//                    if(!string.IsNullOrEmpty(txtServerAddress.Text))
					//                        MobileTech.Consts.ServerURL=txtServerAddress.Text;

				}
				catch(Exception ex)
				{
					if(ex.Message.Contains("Invalid URI") || ex.Message.Contains("ReceiveFailure") || ex.Message.Contains("Network is unreachable"))
					{
						UIAlertView alert=new UIAlertView("Invalid Data","The Web Server is not reachable. Please verify the Code/URL or check the network configuration.", null, "OK",null);
						alert.Show();
					}
					else if(ex.Message.Contains("Invalid Code"))
					{
						UIAlertView alert=new UIAlertView("Invalid Data","The entered Code is invalid. Please enter a valid Code.", null, "OK",null);
						alert.Show();
					}
					mAnimationContainer.AddSubview (mServerSetupView);
				}
			}
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			this.Title = "Logout";
			//Console.WriteLine(UIScreen.MainScreen.Bounds);
			this.txtPassword.SecureTextEntry = true;
			activityMonitor.HidesWhenStopped = true;
			activityMonitor.Hidden = true;

			//TODO:28th
			//M.D.Prasad/28th Jan 2015/we are fetching username and serverURL from Root.plist (NSUserDefaults)
			//To Update the App build without uninstalling previous one.
			//Security.getSystemSetup();
			existedAppSettings ();


			this.txtServerAddress.Text = "";
			this.txtUsername.Text = "";
			this.txtPassword.Text = "";

			var user = NSUserDefaults.StandardUserDefaults;

			if(!string.IsNullOrEmpty(MobileTech.Consts.ServerURL)){
				this.txtServerAddress.Text=MobileTech.Consts.ServerURL;
				if(user.StringForKey ("txtServerCode") != null){
					Consts.ServerCode = user.StringForKey("txtServerCode");
					this.txtServerAddress.Text = user.StringForKey("txtServerCode");
				}
				mAnimationContainer.AddSubview(loginView);
			}else{
				mAnimationContainer.AddSubview(mServerSetupView);
			}

			if(txtServerAddress.Text.Length==0){
				txtServerAddress.BecomeFirstResponder();
			}else if(txtUsername.Text.Length==0){
				txtUsername.BecomeFirstResponder();
			}else if(txtPassword.Text.Length==0){
				txtPassword.BecomeFirstResponder();
			}


			// make 'return' shift focus to next text field
			this.txtUsername.ShouldReturn = delegate(UITextField textField) {
				this.txtUsername.ResignFirstResponder();
				return true;
			};
			
			// make 'return' on last text field save and close the form
			this.txtPassword.ShouldReturn = delegate(UITextField textField) {
				this.txtPassword.ResignFirstResponder();
				return true;
			};

			this.txtServerAddress.ShouldReturn = delegate(UITextField textField) {
				this.txtServerAddress.ResignFirstResponder();
				return true;
			};

			this.txtUsername.ShouldChangeCharacters += delegate (UITextField textField, NSRange range, string replacementString) {
				return textField.Text.Length + replacementString.Length - range.Length <= 32;
			} ;


		}


//		public override void ViewDidUnload ()
//		{
//			base.ViewDidUnload ();
//			
//			// Clear any references to subviews of the main view in order to
//			// allow the Garbage Collector to collect them sooner.
//			//
//			// e.g. myOutlet.Dispose (); myOutlet = null;
//			
//			ReleaseDesignerOutlets ();
//		}
//		
//		public override bool ShouldAutorotateToInterfaceOrientation (UIInterfaceOrientation toInterfaceOrientation)
//		{
		//			// Return true for supported orientationsviewdi
//			return (toInterfaceOrientation != UIInterfaceOrientation.PortraitUpsideDown);
//		}

		public bool textFieldShouldReturn (UITextField theTextField)
		{
			if (theTextField.Tag == 1) {
				theTextField.ResignFirstResponder();
			}
			if(theTextField.Tag==2){
				theTextField.ResignFirstResponder();
			}
			if (theTextField.Tag == 3) {
				theTextField.ResignFirstResponder();
			}
			return true;
		}


		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear(animated);
			this.NavigationController.SetNavigationBarHidden(true,animated);
			//When user comes to login Screen, password should clear.

			// if already logged in (clicking on Logout on Dispatch List), send a SignOff to AuthenticationService

			//Nahid@ Jan28,2014: Send a SignOff only when signoned remotely.
			//if (authClient.SessionState == AuthClient.SessionStateType.SignedOnLocal || authClient.SessionState == AuthClient.SessionStateType.SignedOnRemote ) {

			if (authClient.SessionState == AuthClient.SessionStateType.SignedOnRemote ) {
				//TODO:28th
				/*WsMessageConnector msgQ = new WsMessageConnector (null);
				msgQ.ForceSync ();
				msgQ = null;
				*/

				//TODO:28th
				/*string webServerURL = txtServerAddress.Text.Trim() + @"/AuthenticationService.asmx";
				Uri _remoteServerUrl = new Uri(webServerURL);
				if (MobileTech.Consts.IsUrlReachable (_remoteServerUrl, "AuthenticationService.asmx")) {

					//string tokenId = MobileTech.Consts.MyDeviceToken;*/
				string webServerURL = Consts.ServerURL.Trim() + @"/AuthenticationService";
				Uri _remoteServerUrl = new Uri(webServerURL);
				if (MobileTech.Consts.IsWebApiUrlReachable (_remoteServerUrl, "AuthenticationService")) {
					string tokenId = "";
					string bundleId = NSBundle.MainBundle.BundleIdentifier;
					authClient.SignOff (this.txtUsername.Text, bundleId, tokenId);
				}
			}

			//this.txtServerAddress.Text= MobileTech.Consts.ServerURL;
			if (string.IsNullOrEmpty (Consts.ServerCode)) {
				this.txtServerAddress.Text = Consts.ServerURL;
			} else {
				this.txtServerAddress.Text = Consts.ServerCode;
			}


			if (MobileTech.Consts.IsDBEmpty ()) {
				this.btnPurgeDB.Enabled = false;
				//this.txtUsername.Text = string.Empty;
			} else {
				this.btnPurgeDB.Enabled = true;
				//this.txtUsername.Text = MobileTech.Consts.LoginUserName;
			}
			//TODO:28th
			Security.getSystemSetup();
			if(MobileTech.Consts.db_Version != string.Empty)
				this.lblDBVersion.Text = "Database Version: 5.5.0, Build " + MobileTech.Consts.db_Version;
			if(MobileTech.Consts.ws_Version != string.Empty)
				this.lblWSVersion.Text = "Web Service Version: 5.5.0, Build " +MobileTech.Consts.ws_Version;

			//Author/Date/Issue - M.D.Prasad/14th May 2014/ISS - 5395 or CRR - 1338  Upload iGotIt device logs
			if (Consts.isVisibleBtnUploadLogToServer)
				this.btnUploadLogToServer.Enabled = true;
			else
				this.btnUploadLogToServer.Enabled = false;

			var user = NSUserDefaults.StandardUserDefaults;

			if (user.StringForKey ("txtUserName") != null) {
				if (user.StringForKey ("txtPassWord") == null) {
					user.SetString (string.Empty, "txtPassWord");
				}
				if (user.StringForKey ("txtUserName").Trim () != string.Empty && user.StringForKey ("txtPassWord").Trim () != string.Empty) {
					this.txtUsername.Text = user.StringForKey ("txtUserName");
					this.txtPassword.Text = user.StringForKey ("txtPassWord");
					//MobileTech.Consts.LoginUserName = this.txtUsername.Text;
					//MobileTech.Consts.LoginUserPwd = this.txtPassword.Text;

					if (this.logoutFromMenu == false) {
						if (user.StringForKey ("txtPassWord") != string.Empty && this.txtServerAddress.Text.Trim () != string.Empty) {
							MonoTouch.Foundation.NSObject sender = new NSObject ();
							btnLoginAction (sender);
						}
					}
					this.logoutFromMenu = false;
				}

				if (user.StringForKey ("txtUserName").Trim () != string.Empty && user.StringForKey ("txtPassWord").Trim () == string.Empty) {
					this.txtUsername.Text = user.StringForKey ("txtUserName");
				}
			}


		}
		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear(animated);
			this.NavigationController.SetNavigationBarHidden(false,animated);
		}

		public void ShowAlertMessag(string alrtMessag)
		{
			UIAlertView offlineMode = new UIAlertView();
			offlineMode.Message = alrtMessag;
			offlineMode.AddButton("Ok"); 
			offlineMode.Show();
		}

		public void AutoOpenID()
		{
			//TODO:28th
			/*if (Consts.OpenID != 0) {
				// load Dispatch Detail
				Dispatches dispatchDetails = MobileTech.Consts.GetDispatchByID(Consts.OpenID);
				Consts.OpenID = 0;
				if (dispatchDetails.RQSPrimaryid != 0) {
					DispatchDetails details = new DispatchDetails (dispatchDetails);
					this.NavigationController.PushViewController (details, false);
				}
			}*/
		}

		private void LogCountOfDispatches(string msg)
		{
			//TODO:28th
			/*
			List<Dispatches> dispatches = MobileTech.Consts.GetLocalDispatches (0);

			MobileTech.Consts.LogEntry(msg + " Total Dispaches: "+dispatches.Count, true);
			foreach (Dispatches dispatch in dispatches) {
				MobileTech.Consts.LogEntry(dispatch.RQSPrimaryid.ToString(), true);
			}*/
		}

		public override bool ShouldAutorotateToInterfaceOrientation (UIInterfaceOrientation toInterfaceOrientation)
		{
			// Return true for supported orientations
			return (toInterfaceOrientation != UIInterfaceOrientation.PortraitUpsideDown);
		}

		public void existedAppSettings()
		{	
			var sud = NSUserDefaults.StandardUserDefaults;
			if(sud.StringForKey("txtUserName")!=null)
				MobileTech.Consts.LoginUserName = sud.StringForKey("txtUserName");

			if(sud.StringForKey("txtServerURL")!=null)
				MobileTech.Consts.ServerURL = sud.StringForKey("txtServerURL");
		}
	}
}

