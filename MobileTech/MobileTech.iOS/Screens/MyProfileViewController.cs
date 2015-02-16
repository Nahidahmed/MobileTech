using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using Xamarin;

using MobileTech.Core;
using MobileTech.iOS.Screens;
using MobileTech.Models.Entity;
using MobileTechService;

namespace MobileTech.iOS
{
	public partial class MyProfileViewController : UIViewController
	{
		#region Declaration
		FilterInfo filter;
		/*
		Dictionary<string, object> dicProfileFilter = new Dictionary<string, object> {
			{"FacilitiesKey", string.Empty},
			{"DepartmentsKey", new List<string>()},
			{"WorkersKey", new List<string>()}
		};
		*/

		public WsMessageConnector messageQueue = null;
		protected LoadingOverlay _loadPop = null;
		public string exceptionMessage = string.Empty;

		public MobileTech.Consts.enumUserExceptionType exceptionType = MobileTech.Consts.enumUserExceptionType.None;

		Dictionary<string, List<string>> dicProfileFilter = new Dictionary<string, List<string>> {
			{"FacilitiesKey", new List<string>()},
			{"DepartmentsKey", new List<string>()},
			{"WorkersKey", new List<string>()}
		};
		#endregion

		static bool UserInterfaceIdiomIsPhone {
			get { return UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone; }
		}

		public MyProfileViewController ()
			: base (UserInterfaceIdiomIsPhone ? "MyProfileViewController_iPhone" : "MyProfileViewController_iPad", null)
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
			
			// Perform any additional setup after loading the view, typically from a nib.
			facilityLookUpTextField.ShouldBeginEditing = (textField) => {
				facilityBtnArrowTap(null);
				return false;
			};

			selectedFacilitiesTextField.ShouldBeginEditing = (textField) => {
				facilityBtnArrowTap(null);
				return false;
			};


			departmentLookUpTextField.ShouldBeginEditing = (textField) => {
				departmentBtnArrowTap(null);
				return false;
			};
			selectedDepartmentsTextField.ShouldBeginEditing = (textField) => {
				departmentBtnArrowTap(null);
				return false;
			};

			workerLookUpTextField.ShouldBeginEditing = (textField) => {
				workerBtnArrowTap(null);
				return false;
			};
			selectedWorkersTextField.ShouldBeginEditing = (textField) => {
				workerBtnArrowTap(null);
				return false;
			};

			UpdateFacilityUI ();
			UpdateDepartmentUI ();
			UpdateWorkerUI ();

			LoadFilters ();
		}

		public void LoadFilters()
		{
			Repository repo = new Repository ();
			filter = repo.GetFilterDetails();
			if (filter.MTFPrimaryId > 0) {
				selectedFacilitiesTextField.Text = repo.GetFilterAccountFacilities (filter.FACPrimaryIds);
				dicProfileFilter ["FacilitiesKey"].Clear ();
				dicProfileFilter ["FacilitiesKey"] = filter.FACPrimaryIds.Split (',').ToList<string> ();

				selectedDepartmentsTextField.Text = repo.GetFilterDepartments (filter.ACCPrimaryIds);
				dicProfileFilter ["DepartmentsKey"].Clear ();
				dicProfileFilter ["DepartmentsKey"] = filter.ACCPrimaryIds.Split (',').ToList<string> ();

				selectedWorkersTextField.Text = repo.GetFilterWorker (filter.WKRPrimaryIds);
				dicProfileFilter ["WorkersKey"].Clear ();
				dicProfileFilter ["WorkersKey"] = filter.WKRPrimaryIds.Split (',').ToList<string> ();

			} else {
				selectedFacilitiesTextField.Text = string.Empty;
				selectedDepartmentsTextField.Text = string.Empty;
			}
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			this.Title = "Filters";

			DisplayMandatoryMark ();
		}

		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);
			this.Title = string.Empty;
		}

		public void UpdateFacilityUI()
		{
			selectedFacilitiesTextField.Text = string.Empty;
		}

		public void UpdateDepartmentUI()
		{
			//if (string.IsNullOrEmpty (dicProfileFilter ["FacilitiesKey"].ToString ())) 
			dicProfileFilter ["DepartmentsKey"].Clear ();
			selectedDepartmentsTextField.Text = string.Empty;
			if(dicProfileFilter ["FacilitiesKey"].Count<=0)
			{
				departmentLookUpTextField.Enabled = false;
				selectedDepartmentsTextField.Enabled = false;
				departmentBtn.Enabled = false;
			} else {
				departmentLookUpTextField.Enabled = true;
				selectedDepartmentsTextField.Enabled = true;
				departmentBtn.Enabled = true;
			}
		}

		public void UpdateWorkerUI()
		{
			/*
			Repository repository = new Repository ();
			worker = repository.GetWorkerOnID(MobileTech.Consts.WKRPrimaryId);
			if (!string.IsNullOrEmpty(worker.Name))
				selectedWorkersTextField.Text = worker.Name;
				*/
			selectedWorkersTextField.Text = "<My>";
		}

		public void DisplayMandatoryMark()
		{
			if (selectedFacilitiesTextField.Text.Trim ().Length > 0)
				requiredFacilities.Hidden = true;
			else
				requiredFacilities.Hidden = false;

			if (selectedDepartmentsTextField.Text.Trim ().Length > 0)
				requiredDepartments.Hidden = true;
			else
				requiredDepartments.Hidden = false;

			if (selectedWorkersTextField.Text.Trim ().Length > 0)
				requiredWorkers.Hidden = true;
			else
				requiredWorkers.Hidden = false;
		}

		#region Button Actions

		partial void facilityBtnArrowTap (NSObject sender)
		{
			Repository repository= new Repository();
			List<FacilityDetails> facilities = repository.GetAccountFacilities();

			List<KeyValuePair<long, string>> facilityItems = facilities.Select(it => new KeyValuePair<long, string> (it.FacPrimaryId, it.Name)).ToList();
			TableItems facilitiesGroup = new TableItems();
			facilitiesGroup.Group = new KeyValuePair<long, string>(1, string.Empty);
			facilitiesGroup.ListItems = facilityItems;

			List<TableItems> tableItems = new List<TableItems>();
			tableItems.Add(facilitiesGroup);

			List<KeyValuePair<long, string>> selectedItems = facilities.Where(it => dicProfileFilter ["FacilitiesKey"].Contains(it.FacPrimaryId.ToString())).Select(it => new KeyValuePair<long, string> (it.FacPrimaryId, it.Name)).ToList();

			var selectListScreen  = new MultiSelectListScreen("Facilities", tableItems, selectedItems, searchFirstSectionAlso:true);
			selectListScreen.selectedItemsHandler += (object s, List<KeyValuePair<long, string>> e) => {
					selectedFacilitiesTextField.Text = e.Count>0 ? string.Join(",",e.Select(x => x.Value).ToArray()) : string.Empty;
					dicProfileFilter ["FacilitiesKey"].Clear();
					dicProfileFilter ["FacilitiesKey"].AddRange(e.Select(x=>x.Key.ToString()).ToList<string>());
				UpdateDepartmentUI();
				DisplayMandatoryMark();
			};

			this.NavigationController.PushViewController(selectListScreen, true);
		}

		partial void departmentBtnArrowTap (NSObject sender)
		{
			Repository repository= new Repository();
			List<AccountInfo> accounts = repository.GetAccountDepartments(string.Join(",", dicProfileFilter["FacilitiesKey"]));

			List<KeyValuePair<long, string>> groups = accounts.Select(x => new KeyValuePair<long, string>(x.Facility.FacPrimaryId, x.Facility.Name)).Distinct().ToList();

			List<TableItems> tableItems = new List<TableItems>();
			foreach (KeyValuePair<long, string> group in groups)
			{
				var items = accounts.Where(x => x.Facility.FacPrimaryId == group.Key).Select(x => new KeyValuePair<long, string>(x.AccPrimaryId, x.DepartmentName)).ToList();
				if(items.Count>0)
				{
					TableItems accountItem = new TableItems();
					accountItem.Group = group;
					accountItem.ListItems= items;

					tableItems.Add(accountItem);
				}
			}

			List<KeyValuePair<long, string>> selectedItems = accounts.Where(it => dicProfileFilter ["DepartmentsKey"].Contains(it.AccPrimaryId.ToString())).Select(x => new KeyValuePair<long, string>(x.AccPrimaryId, x.DepartmentName)).ToList();

			var selectListScreen  = new MultiSelectListScreen("Departments", tableItems, selectedItems, true, true);
				selectListScreen.selectedItemsHandler += (object s, List<KeyValuePair<long, string>> e) => {
				selectedDepartmentsTextField.Text = e.Count>0 ? string.Join(",",e.Select(x => x.Value).ToArray()) : string.Empty;
					dicProfileFilter ["DepartmentsKey"].Clear();
					dicProfileFilter ["DepartmentsKey"].AddRange(e.Select(x=>x.Key.ToString()).ToList<string>());
				DisplayMandatoryMark();
			};

			this.NavigationController.PushViewController(selectListScreen, true);
		}

		partial void workerBtnArrowTap (NSObject sender)
		{
			Repository repository= new Repository();
			List<WorkerInfo>workers = repository.GetWorkers(Consts.WKRPrimaryId);

			List<KeyValuePair<long, string>> additionalItems = new List<KeyValuePair<long, string>>{
				{new KeyValuePair<long, string>(Consts.WKRPrimaryId,"<My>")},
				{new KeyValuePair<long, string>(2, "<Unassigned>")}
			};

			TableItems additiveGroup = new TableItems();
			additiveGroup.Group = new KeyValuePair<long, string>(0, "Additives");
			additiveGroup.ListItems = additionalItems;

			List<KeyValuePair<long, string>> workerItems = workers.Select(it => new KeyValuePair<long, string> (it.WKRPrimaryId, it.Name)).ToList();
			TableItems workersGroup = new TableItems();
			workersGroup.Group = new KeyValuePair<long, string>(1, "Workers");
			workersGroup.ListItems = workerItems;

			List<TableItems> tableItems = new List<TableItems>();
			tableItems.Add(additiveGroup);
			tableItems.Add(workersGroup);

			var selItemsAdditives = additionalItems.Where(it => dicProfileFilter ["WorkersKey"].Contains(it.Key.ToString())).Select(it => new KeyValuePair<long, string> (it.Key, it.Value)).ToList();
			var selItemsWorkers = workers.Where(it => dicProfileFilter ["WorkersKey"].Contains(it.WKRPrimaryId.ToString())).Select(it => new KeyValuePair<long, string> (it.WKRPrimaryId, it.Name)).ToList();
			List<KeyValuePair<long, string>> selectedItems = new List<KeyValuePair<long, string>>();

			if(selItemsAdditives.Count>0)
				selectedItems.AddRange(selItemsAdditives);
			if(selItemsWorkers.Count>0)
				selectedItems.AddRange(selItemsWorkers);

			if(selectedItems.Count==0)
				selectedItems.Add(new KeyValuePair<long, string>(Consts.WKRPrimaryId,"<My>"));
			/*
			else
			{
				var myObject = selectedItems.Single(x => x.Key == Consts.WKRPrimaryId);
				int index = selectedItems.IndexOf(myObject);

				selectedItems.RemoveAt(index);
				selectedItems.Insert(index, new KeyValuePair<long, string>(Consts.WKRPrimaryId,"<My>"));
			}
			*/

			var selectListScreen  = new MultiSelectListScreen("Workers", tableItems, selectedItems);
			//var selectListScreen  = new MultiSelectListScreen("Workers", tableItems, selectedItems, additionalItems, false);
			selectListScreen.selectedItemsHandler += (object s, List<KeyValuePair<long, string>> e) => {
				selectedWorkersTextField.Text = e.Count>0 ? string.Join(",",e.Select(x => x.Value).ToArray()) : string.Empty;
					//var keys = e.Select(x=>x.Key).ToList<string>();
					dicProfileFilter ["WorkersKey"].Clear();
					dicProfileFilter ["WorkersKey"].AddRange(e.Select(x=>x.Key.ToString()).ToList<string>());
				DisplayMandatoryMark();
			};
		
			this.NavigationController.PushViewController(selectListScreen, true);
		}

		partial void saveFilterTouchUpInside (NSObject sender)
		{
			if(isFilterValid())
			{
				Uri _remoteServerUrl = new Uri(MobileTech.Consts.ServerURL + "/AuthenticationService");
				if (MobileTech.Consts.IsWebApiUrlReachable (_remoteServerUrl, "AuthenticationService")){

					filter.FilterName = "Default";
					filter.FACPrimaryIds = string.Join(",", dicProfileFilter["FacilitiesKey"]);
					filter.ACCPrimaryIds = string.Join(",", dicProfileFilter["DepartmentsKey"]);
					filter.WKRPrimaryIds = string.Join(",", dicProfileFilter["WorkersKey"]);
					filter.Active = Convert.ToByte(1);

					Repository repository = new Repository();
					repository.InsertORUpdateFilter(filter); 

					repository.UploadFilter();

					//After uploaded to filter successfully then we do following activities.
					//1. If exists, should upload pending records from message queue.
					//2. Delete local data.
					//3. Download User's requested Data.  While downloading by any chance net disconnected "logout" and control moves to login view.

					messageQueue = new WsMessageConnector();

					if (messageQueue != null) {
						// messageQueue.Start ();
						//Uri _remoteServerUrl = new Uri (Consts.ServerURL + "/AuthenticationService");
						if (MobileTech.Consts.IsWebApiUrlReachable (_remoteServerUrl, "/AuthenticationService")) {
							Consts.signonStatus = "true";
							string[] tables = {"Account","ConfigurationSync","Equipment","WkOrders", "WOText", "AccountFacility", "Workers"};

							messageQueue.ForceSync ();

							MobileTech.Consts.DeletePreviousUsersDataForSpecificTables(tables);
							MobileTech.Consts.DeletePreviousUsersMsgQueue();
							filterSync();
						}
					}

				}else{
					ShowAlertMessage("Web Server is not reachable. The Profile cannot be changed.");
				}
			}
		}
		#endregion

		#region Miscellaneous Methods
		public bool isFilterValid()
		{
			bool isPageValid = true;
			if (!requiredFacilities.Hidden) {
				ShowAlertMessage ("Facilities are required field, please select Facilities");
				isPageValid = false;
			}else if (!requiredDepartments.Hidden){
				ShowAlertMessage ("Departments are required field, please select Departments");
				isPageValid = false;
			}else if(!requiredWorkers.Hidden){
				ShowAlertMessage ("Workers are required field, please select Workers");
				isPageValid = false;
			}

			return isPageValid;
		}

		public void ShowAlertMessage(string alrtMessag)
		{
			UIAlertView msg = new UIAlertView();
			msg.Message = alrtMessag;
			msg.AddButton("Ok"); 
			msg.Show();
		}
		#endregion

		#region Re-Sync Process
		public void filterSync()
		{
			// show the loading overlay on the UI thread
			this._loadPop = new LoadingOverlay (UIScreen.MainScreen.Bounds);
			this.View.Add ( this._loadPop );

			//ctrlUserNameVal = MobileTech.Consts.LoginUserName;
			//ctrlPassWordVal = MobileTech.Consts.LoginUserPwd;
			//			ctrlServerAddressVal = this.txtServerAddress.Text.Trim();
			//ctrlServerAddressVal = Consts.ServerURL;

			// spin up a new thread to do some long running work using StartNew
			Task.Factory.StartNew (
				// tasks allow you to use the lambda syntax to pass work
				() => {
					Console.WriteLine ( "Calling filtersync on Thread" );
					getSyncDataOnThread();
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

						Console.WriteLine("Filter Data Downloaded Successfully");
					}
					else if(exceptionType.Equals(MobileTech.Consts.enumUserExceptionType.DownloadFailed))
					{
						ShowAlertMessage(exceptionMessage);
						UINavigationController navController = this.NavigationController;
						var loginViewController  = new LoginViewController(true);
						navController.SetViewControllers(new [] { loginViewController }, true);

						/*
						UINavigationController navController = this.NavigationController;

						Task.Factory.StartNew(async () => {
							bool accepted = await AlertHelper.ShowYesNoAlert("Download Failed", exceptionMessage);

							if (accepted)
							{
								InvokeOnMainThread ( () => {
									var loginViewController  = new LoginViewController(true);
									navController.SetViewControllers(new [] { loginViewController }, true);
								});

								string msg = string.Format("{0} User logged out or his session got terminated: {1}/{2}/{3}.", DateTime.Now, Consts.LoginUserName, Consts.DeviceId, MobileTechService.AuthClient.GetInstance().CurrentSession);
								Consts.HandleExceptionLog(msg);
								//Insights.Track("Menu View Controller","TableRowSelected",msg);
							}
						});
						*/
					}
				}, TaskScheduler.FromCurrentSynchronizationContext()
			);

		}

		public void getSyncDataOnThread()
		{
			string errorMessage = string.Empty;
			try 
			{
				Configuration config = new Configuration();
				config.GetEquipments("Equipment",ref errorMessage);
				config.GetAccounts("Account",ref errorMessage);
				config.GetWkOrders("WkOrders",ref errorMessage);
				config.GetWOTexts("WOTexts",ref errorMessage);
				config.GetWorkers("Workers", ref errorMessage);
				config.GetAccountFacilities("AccountFacility", ref errorMessage);
				config.GetFilter();
			}catch (Exception ex) {
				string strLogMessage = string.Format("{0} getSyncDataOnThread Exception: {1}", DateTime.Now, ex.Message);
				MobileTech.Consts.HandleExceptionLog(strLogMessage);
				Insights.Report (ex, "getSyncDataOnThread", errorMessage);
				exceptionType =	MobileTech.Consts.enumUserExceptionType.DownloadFailed;
				exceptionMessage = ex.Message;
			}
			if (!string.IsNullOrEmpty (errorMessage)) {
				exceptionType =	MobileTech.Consts.enumUserExceptionType.DownloadFailed;
				exceptionMessage = errorMessage;
			}

		}

		#endregion
	}
}

