
using System;
using System.Drawing;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Collections.Generic;
using MobileTech.Models.Entity;
using MobileTech.Core;
using MobileTechService;
using Xamarin;

namespace MobileTech.iOS
{
	public partial class WkOrderListViewController : UIViewController
	{
		UIRefreshControl refCtrl;
		private Configuration config = new Configuration();
		protected MobileTech.iOS.WOTableSource tableSource;
		List<TableItemGroup> tableItems;
		public WsMessageConnector messageQueue = null;

		static bool UserInterfaceIdiomIsPhone {
			get { return UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone; }
		}

		public WkOrderListViewController ()
			: base (UserInterfaceIdiomIsPhone ? "WkOrderListViewController_iPhone" : "WkOrderListViewController_iPad", null)
		{
		}

		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
			
			// Release any cached data, images, etc that aren't in use.
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			this.NavigationItem.SetHidesBackButton (true, false);

			this.NavigationController.NavigationBar.TintColor = UIColor.White;
			this.NavigationController.NavigationBar.BarTintColor = UIColor.Orange;


			refCtrl = new UIRefreshControl();
			this.TableView.AddSubview (refCtrl);

//			this.RefreshControl = refCtrl;

			NSAttributedString str = new NSAttributedString("Release to update...");
			refCtrl.ValueChanged += (object sender, EventArgs e) => {
				refCtrl.AttributedTitle = str;
				if (messageQueue != null)
				{
					messageQueue.ForceSync();
				}
				getBackgroundSyncData();

				refCtrl.EndRefreshing();
			};


			//searchBar.ShowsCancelButton = false;
			messageQueue = new WsMessageConnector();

			this.searchBar.TextChanged += (object sender, UISearchBarTextChangedEventArgs e) => 
			{
				searchBar.ShowsCancelButton = true;
				searchTable();
			};

			this.searchBar.SearchButtonClicked+= (object sender, EventArgs e) => {
				this.searchBar.EndEditing(true);
				searchBar.ShowsCancelButton = false;
				this.searchBar.ResignFirstResponder();
			};

			this.searchBar.CancelButtonClicked += (object sender, EventArgs e) => {
				searchBar.Text=string.Empty;
				searchTable();
				searchBar.ResignFirstResponder();
				searchBar.ShowsCancelButton = false;
			};

			UIBarButtonItem hamburger = new UIBarButtonItem ("\u2630", UIBarButtonItemStyle.Plain, (sender, args) => {
				var menuViewController = new MenuViewController();
				this.NavigationController.PushViewController(menuViewController, true);
			});
			this.NavigationItem.SetRightBarButtonItem (hamburger, true);

		}

		private void searchTable()
		{
			this.TableView.Source  = this.tableSource.Filter(searchBar.Text,tableItems,this);
			this.TableView.ReloadData();
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			this.Title = "Work Orders";

			this.searchBar.SetShowsCancelButton (false, false);

			if (MobileTech.Consts.signonStatus == "true") {
				this.CreateTableItems (string.Empty);
			} else {
				this.CreateTableItems ("offline");
			}
			this.TableView.Source = tableSource;

			//Nahid Ahmed Jan 1, 2015 BA#27499
			if (searchBar.Text.Trim () != string.Empty) {
				searchTable ();
			}

			if (messageQueue != null) {
				// messageQueue.Start ();
				Uri _remoteServerUrl = new Uri (Consts.ServerURL + "/AuthenticationService");
				if (MobileTech.Consts.IsWebApiUrlReachable (_remoteServerUrl, "/AuthenticationService")) {
					Consts.signonStatus = "true";
					messageQueue.ForceSync ();

					InsightTrack();

					if (Consts.isSyncRequired) {
						getBackgroundSyncData ();
						Consts.isSyncRequired = false;
					}
				}
			}

		}

		private void InsightTrack()
		{
			var user = NSUserDefaults.StandardUserDefaults;

			if(user.StringForKey("ProcessMessage_Exception") != null){
				string msg = user.StringForKey ("ProcessMessage_Exception").Trim ();
				Insights.Track ("Work Order List", "WO Upd Exception", msg);
				user.RemoveObject ("ProcessMessage_Exception");
			}

			if(user.StringForKey("WO_Uploaded_Successfully") != null){
				string msg = user.StringForKey ("WO_Uploaded_Successfully").Trim ();
				Insights.Track ("Work Order List", "WO Upd Status", msg);
				user.RemoveObject ("WO_Uploaded_Successfully");
			}

			if(user.StringForKey("UploadPDADBID_Exception") != null){
				string msg = user.StringForKey ("UploadPDADBID_Exception").Trim ();
				Insights.Track ("Work Order List", "UploadPDADBID Exception", msg);
				user.RemoveObject ("UploadPDADBID_Exception");
			}

			if(user.StringForKey("UploadWorkOrder_Exception") != null){
				string msg = user.StringForKey ("UploadWorkOrder_Exception").Trim ();
				Insights.Track ("Work Order List", "UploadWorkOrder_Exception", msg);
				user.RemoveObject ("UploadWorkOrder_Exception");
			}

			if(user.StringForKey("PDADBID_Updated_on_Server") != null){
				string msg = user.StringForKey ("PDADBID_Updated_on_Server").Trim ();
				Insights.Track ("Work Order List", "PDADBID Updated on Server", msg);
				user.RemoveObject ("PDADBID_Updated_on_Server");
			}
		}

		protected void CreateTableItems (string offlineConnectionText)
		{
			tableItems = new List<TableItemGroup> ();

			// declare vars
			TableItemGroup tGroup;
			List<WorkOrderDetails> lstWorkOrder; 

			// Section 1 -- New Dispatches
			if (offlineConnectionText != string.Empty) {
				tGroup = new TableItemGroup (){ Name = @"No network. Connect and sync manually." };
				tGroup.ListItems.Clear ();
				tableItems.Add (tGroup);
			}

			Repository repository = new Repository();
			lstWorkOrder = repository.GetWorkOrders();

			if (lstWorkOrder.Count > 0) {
				tGroup = new TableItemGroup () { };

				foreach (WorkOrderDetails myRequest in lstWorkOrder) {
					tGroup.ListItems.Add (myRequest);
				}	
				tableItems.Add (tGroup);
			} else {
				tGroup = new TableItemGroup (){  };
				tGroup.ListItems.Clear ();
			}

			tableSource = new MobileTech.iOS.WOTableSource(tableItems, this);
		}

		public void ShowWOEditor ( WorkOrderDetails woDetails, bool animated)
		{
			WoEditorViewController editor = new WoEditorViewController(woDetails);
			NavigationController.PushViewController(editor, animated);
		}

		public override void ViewWillDisappear (bool animated)
		{
			this.Title = string.Empty;
		}

		public void DissmisSearchKeyBoard()
		{
			this.searchBar.EndEditing(true);
			searchBar.ShowsCancelButton = true;
			this.searchBar.ResignFirstResponder();
		}

		public void getBackgroundSyncData()
		{
			string errorMessage = string.Empty;
			this.TableView.AllowsSelection = false;
			string offline = string.Empty;

			Uri _remoteServerUrl = new Uri(MobileTech.Consts.ServerURL + "/AuthenticationService");

			//BackGround Sync Condition#1: remote server should be reachable
			if (MobileTech.Consts.IsWebApiUrlReachable (_remoteServerUrl, "AuthenticationService")){

//				iGotItService.DispatchHandler dispatchHandler = new iGotItService.DispatchHandler ();

				//BackGround Sync Condition#2: There should be no data for upload in local DB
//				if (!dispatchHandler.CheckWsMessageConnector ()) {
					int intCount = 0;
					try {
						if (AuthClient.GetInstance ().IsValidSession ()) {
							config.GetControlCenter("Codes_ControlCenter",ref errorMessage);
							config.GetEquipments("Equipment",ref errorMessage);
							config.GetAccounts("Account",ref errorMessage);
							config.GetWOTexts("WOTexts",ref errorMessage);
							config.GetTimeTypes("Codes_WorkOrderTimeType",ref errorMessage);
							config.GetWkOrders("WkOrders",ref errorMessage);
						} else {
//							DisposeTimer ();

							AuthClient.GetInstance ().SessionState = AuthClient.SessionStateType.SignedOnRemote;
							ShowAlertMessage("Your session is no longer valid. Please log back in");

							this.NavigationController.PopViewControllerAnimated (true);
						}
					} catch (Exception ex) {
						//Author/Date/Issue - M.D.Prasad/14th May 2014/ISS - 5395 or CRR - 1338  Upload iGotIt device logs
						string strLogMessage = string.Format("{0} getBackgroundSyncData Exception: {1}", DateTime.Now, ex.Message);
						Consts.HandleExceptionLog(strLogMessage);
					    Insights.Report (ex, "getBackgroundSyncData", errorMessage);
//						DisposeTimer ();
						ShowAlertMessage("Your session is no longer valid. Please log back in");
					}
//				} else {
//					if (messageQueue != null){
//						iGotItService.Consts.signonStatus = "true";
//						messageQueue.ForceSync();
//					}
//				}
			} else {
				offline = "offline";
//				DisposeTimer ();
				Consts.signonStatus = "NoConnectionButDBIsNotEmpty";
				ShowAlertMessage("Web Server is not reachable. Please check network connection.");
			} 

			this.CreateTableItems (offline);
			this.TableView.Source = tableSource;
			this.TableView.ReloadData ();

			this.TableView.AllowsSelection = true;

			//refCtrl.EndRefreshing();

		}

		public void ShowAlertMessage(string alrtMessag)
		{
			UIAlertView alrt = new UIAlertView();
			alrt.Message = alrtMessag;
			alrt.AddButton("Ok"); 
			alrt.Show();
		}


	}
}

