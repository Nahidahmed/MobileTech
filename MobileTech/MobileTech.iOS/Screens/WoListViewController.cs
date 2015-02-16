using System;
using System.Collections;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using System.Collections.Generic;
using MobileTechService;
using MobileTech.Models.Entity;
using MobileTech.Core;

namespace MobileTech.iOS.Screens
{
	public class WoListViewController : UITableViewController
	{
		protected MobileTech.iOS.WOTableSource tableSource;
		private Configuration config = new Configuration();

		UIRefreshControl refCtrl;


		public WoListViewController ()
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			this.Title = "Work Orders";


			//TODO: BackGroundSync

			/*refCtrl = new UIRefreshControl();
			this.RefreshControl = refCtrl;
			NSAttributedString str = new NSAttributedString("Release to update...");
			refCtrl.ValueChanged += (object sender, EventArgs e) => {
				refCtrl.AttributedTitle = str;

				//Nahid Nov 20,2014: No Timer is required.
//				if(timer == null){
//					timer = NSTimer.CreateRepeatingScheduledTimer (new TimeSpan (0, 0, (int)0, 30, 0), delegate {
//						getBackgroundSyncData ();
//					});
//				}
				getBackgroundSyncData();
			};

			getBackgroundSyncData();*/

		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			if (MobileTech.Consts.signonStatus == "true") {
				this.CreateTableItems (string.Empty);
			} else {
				this.CreateTableItems ("offline");
			}
			this.TableView.Source = tableSource;

		}

		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear(animated);

		}


		protected void CreateTableItems (string offlineConnectionText)
		{
			List<TableItemGroup> tableItems = new List<TableItemGroup> ();

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
				/*MyRequests emptyObj = new MyRequests ();
				emptyObj.ActionDescription = string.Empty;
				emptyObj.ParentObjectName = string.Empty;
				emptyObj.ObjectName = string.Empty;
				emptyObj.iNeedItAlias = string.Empty;
				emptyObj.ReqNumber = "No Open Requests.";
				tGroup.ListItems.Add (emptyObj);
				tableItems.Add (tGroup);*/
			}


//			tableSource = new MobileTech.iOS.WOTableSource(tableItems, this);
		}

		/*public void getBackgroundSyncData()
		{

			string offline = string.Empty;
			Uri _remoteServerUrl = new Uri(iNeedIt.Consts.ServerURL + "/AuthenticationService.asmx");
			//BackGround Sync Condition#1: remote server should be reachable
			if (iNeedIt.Consts.IsUrlReachable (_remoteServerUrl, "AuthenticationService.asmx")) {
				try {
					if (AuthClient.GetInstance ().IsValidSession ()) {
						if (AuthClient.GetInstance ().IsValidSession ()) {
							config.GetMyRequests("MyRequests");
						} 
					}else {
						AuthClient.GetInstance ().SessionState = AuthClient.SessionStateType.SignedOnRemote;
						ShowAlertMessage ("Your session is no longer valid. Please log back in");

						InvokeOnMainThread ( () => {
							var loginViewController  = new LoginViewController(true);
							this.NavigationController.SetViewControllers(new [] { loginViewController }, true);
						});
					}

				} catch (Exception ex) {
					string strLogMessage = string.Format("{0} getBackgroundSyncData Exception: {1}", DateTime.Now, ex.Message);
					iNeedIt.Consts.HandleExceptionLog(strLogMessage);
				}
				offline = string.Empty;
			} 
			else {
				offline = "offline";
				//Nahid Nov 20,2014: No Timer is required.
//				DisposeTimer ();
				iNeedIt.Consts.signonStatus = "NoConnectionButDBIsNotEmpty";
				ShowAlertMessage("Web Server is not reachable. Please check network connection.");
			} 

			this.CreateTableItems (offline);
			this.TableView.Source = tableSource;
			this.TableView.ReloadData ();

			this.TableView.AllowsSelection = true;

			refCtrl.EndRefreshing();


		}*/

		public void ShowAlertMessage(string alrtMessag)
		{
			UIAlertView alrt = new UIAlertView();
			alrt.Message = alrtMessag;
			alrt.AddButton("Ok"); 
			alrt.Show();
		}

		//Nahid Nov 20,2014: No Timer is required.
//		private void DisposeTimer()
//		{
//			if (timer != null) {
//				timer.Invalidate ();
//				timer.Dispose ();
//				timer = null;
//			}
//		}



		public void ShowWOEditor ( WorkOrderDetails woDetails, bool animated)
		{
			WoEditorViewController editor = new WoEditorViewController(woDetails);
			NavigationController.PushViewController(editor, animated);
		}

	}
}

