using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

using MobileTech.Core;
using MobileTech.iOS.Screens;
using MobileTech.Models.Entity;
using System.Threading.Tasks;
using Xamarin;

namespace MobileTech.iOS
{
	public partial class WoEditorViewController : UIViewController
	{
		WorkOrderDetails woDetails;
		WorkOrderDetails woDetailsCopy;

		bool isViewLoadedFirstTime = true;

		SystemCodeInfo result = new SystemCodeInfo ();
		SystemCodeInfo fault = new SystemCodeInfo();
		SystemCodeInfo safetyTest = new SystemCodeInfo();
		SystemCode owStatus = new SystemCode();

		private UIView activeView;            //Controller that activated the keyboard
		private float scroll_amount = 0.0f;   //amount to scroll
		private float bottom = 0.0f;          //bottom point
		private float offset = 200.0f;         //extra offset
		private bool moveViewUp = false;      //direction up or down
		private short initialRequestsActionCode = 0;

		static bool UserInterfaceIdiomIsPhone {
			get { return UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone; }
		}

		public WoEditorViewController ()
			: base (UserInterfaceIdiomIsPhone ? "WoEditorViewController_iPhone" : "WoEditorViewController_iPad", null)
		{
		}

		public WoEditorViewController (WorkOrderDetails woDetailsObj)
			: base (UserInterfaceIdiomIsPhone ? "WoEditorViewController_iPhone" : "WoEditorViewController_iPad", null)
		{
			this.woDetails = woDetailsObj;
		}

		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
			
			// Release any cached data, images, etc that aren't in use.
		}

		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);
			this.Title = string.Empty;
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			NSNotificationCenter.DefaultCenter.AddObserver (UIKeyboard.DidShowNotification, KeyBoardUpNotification);
			NSNotificationCenter.DefaultCenter.AddObserver (UIKeyboard.WillHideNotification, KeyBoardDownNotification);


			// Perform any additional setup after loading the view, typically from a nib.
			UIBarButtonItem hamburger = new UIBarButtonItem ("\u2630", UIBarButtonItemStyle.Plain, (sender, args) => {
				var menuViewController = new MenuViewController();
				this.NavigationController.PushViewController(menuViewController, true);
			});
			this.NavigationItem.SetRightBarButtonItem (hamburger, true);


			UIToolbar keyboardDoneButtonToolbar = new UIToolbar(RectangleF.Empty)
			{
				BarStyle = UIBarStyle.Black,
				Translucent = true,
				UserInteractionEnabled = true,
				TintColor = null
			};
			keyboardDoneButtonToolbar.SizeToFit();

			// NOTE Need 2 spacer elements here and not sure why...
			UIBarButtonItem btnKeyboardDone = new UIBarButtonItem(UIBarButtonSystemItem.Done, this.KBToolbarButtonDoneHandler);
			keyboardDoneButtonToolbar.SetItems(new []
				{
					new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace, null),
					new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace, null),
					btnKeyboardDone
				}, true);

			notesTextView.InputAccessoryView = keyboardDoneButtonToolbar;

			initializeSettings ();
			fillWorkOrderDetailsView ();
		}

		private void KeyBoardUpNotification(NSNotification notifcation)
		{
			try
			{
				RectangleF r = UIKeyboard.BoundsFromNotification (notifcation);

				foreach (UIView view in this.View.Subviews[3].Subviews) {
					if(view.GetType().Name == "UITextView"){
						//					if (view.IsFirstResponder)
						activeView = view;
					}
				}

				//bottom of the controller = initial pos + height + offset
				bottom = (activeView.Frame.Y + activeView.Frame.Height + offset);

				//calculate how far we want to scroll
				scroll_amount = (r.Height - (View.Frame.Size.Height - bottom));

				//Perform the scrolling
				if (scroll_amount > 0) {
					moveViewUp = true;
					ScrollTheView (moveViewUp);
				} else {
					moveViewUp = false;
				}
			}
			catch(Exception e) {
				Console.WriteLine ("KeyBoardUpNotification ex: " + e.Message);
			}
		}

		private void KeyBoardDownNotification(NSNotification notification)
		{
			if (moveViewUp) {
				ScrollTheView (false);
			}
		}

		private void ScrollTheView(bool move)
		{
			//scroll the view up or down
			try{
				UIView.BeginAnimations (string.Empty, System.IntPtr.Zero);
				UIView.SetAnimationDuration (0.3);

				RectangleF frame = View.Frame;

				if (move) {
					frame.Y -= scroll_amount;
				} else {
					frame.Y += scroll_amount;
					scroll_amount = 0;
				}

				View.Frame = frame;
				UIView.CommitAnimations ();
			}
			catch(Exception e) {
				Console.WriteLine ("ScrollTheView ex: "+e.Message);
			}
		}

		public void KBToolbarButtonDoneHandler(object sender, EventArgs e)
		{
			notesTextView.ResignFirstResponder();
			woDetails.WOTextNotes = notesTextView.Text;
		}

		public override void ViewWillAppear (bool animated)
		{
			this.Title = string.Format("Work Order {0}:{1}", woDetails.ServiceCenter.Code, woDetails.Number);
			base.ViewWillAppear (animated);

			if (this.NavigationController != null) {
				this.NavigationItem.SetLeftBarButtonItem (new UIBarButtonItem ("<", UIBarButtonItemStyle.Plain, (sender, args) => {
					Repository repo = new Repository();
					User usr;
					Uri _remoteServerUrl = new Uri (Consts.ServerURL + "/AuthenticationService");
					if (MobileTech.Consts.IsWebApiUrlReachable (_remoteServerUrl, "/AuthenticationService")) {
						usr = repo.ValidSessionCheck();
					}
					else
						usr = new User();

					if(usr != null){
//						SaveWorkOrder();
//						this.NavigationController.PopViewControllerAnimated(true);

						//while closing workorder should be validate whether we filled all required information or not.
						if(owStatus.RequestsActionCode == (short)Consts.enumRequestsAction.Finish)
						{
							if (!isWorkorderEqual()) {
								PromptToCloseWorkOrder("OnBackSave");
							}else{
								this.NavigationController.PopViewControllerAnimated(true);
							}
						}
						else
						{
							//on every change in workorder status we are saving locally, once we moved to wolist then we are uploading to server all changes.
							SaveWorkOrder();
							this.NavigationController.PopViewControllerAnimated(true);
						}
					}else{
						ShowAlertMessage("Your session is no longer valid. Please log back in");
						var loginViewController  = new LoginViewController(true);
						this.NavigationController.SetViewControllers(new [] { loginViewController }, true);
					}
				}), true);
			}

			//When we loaded firstime we are taking transaction copy of woDetails, for avoiding multilpe savings same workorder details.
			if (isViewLoadedFirstTime) {
				woDetailsCopy = (WorkOrderDetails) woDetails.ShallowCopy();

				isViewLoadedFirstTime = false;
 			}
		
		
			//When Navigating from other Views, we are updating Workorder editor.
			/*
			AppBuffer.enumNavPage navPage = AppBuffer.navPage;
			if(navPage!= AppBuffer.enumNavPage.None)
				{
				woDetails = AppBuffer.workOder;
				if (woDetails != null) {
					switch (navPage) {
	
					//Update TimeEntries Data.
					case AppBuffer.enumNavPage.TimeEntry:
						{
							int curPosTimeEntry = woDetails.TimeEntries.Length;
							notesTextView.Text = notesTextView.Text + Environment.NewLine + woDetails.TimeEntries [curPosTimeEntry - 1].strNotes;
							break;
						}
					}
				}
			}
			*/
		}
			
		public void initializeSettings()
		{
//			SetupButtonBorder (resultLookUpText);
//			SetupButtonBorder (faultLookUpText);

			resultTextField.ShouldBeginEditing = (textField) => {
				ResultLookUpCall();
				return false;
			};

			faultTextField.ShouldBeginEditing = (textField) => {
				FaultLookUpCall();
				return false;
			};

			safetyTestTextField.ShouldBeginEditing = (textField) => {
				SafetyTestLookUpCall();
				return false;
			};
		}

		public static void SetupButtonBorder(UIButton btn)
		{
			btn.Layer.BorderColor = UIColor.LightGray.CGColor;
			btn.Layer.BorderWidth = 0.5f;
			btn.Layer.CornerRadius = 5; 
		}

		public void fillWorkOrderDetailsView()
		{
			//Filling Section 1 Details
			#region Section 1 Details
				descriptionLabel.Text = woDetails.WODescription;
				departmentLabel.Text=woDetails.Account.DepartmentName;

				reqCodeDescLabel.Text = woDetails.Request.Description;
				if(!string.IsNullOrEmpty(woDetails.AssetDetails.Control))
					controlNoLabel.Text =string.Format("{0}-{1}",woDetails.AssetDetails.AssetCenter.Code, woDetails.AssetDetails.Control);
				else
					controlNoLabel.Text = string.Empty;
				devCatLabel.Text = woDetails.AssetDetails.Model.DeviceCategory.DevCategory;

				owStatusLabel.Text = woDetails.OpenWorkOrderStatus.Description;
				urgencyLabel.Text = woDetails.Urgency.Description;
			#endregion

			//Filling Section 2 Details
			#region Section 2 Details
				if (woDetails.WorPrimaryId != null && woDetails.WorPrimaryId != 0) {
					Repository repository = new Repository ();
				    
				    //Author: Nahid Ahmed Dec 30, 2014
				    woDetails = repository.GetWorkOrderDetails(woDetails);

					initialRequestsActionCode = woDetails.LatestDispatchActionCode;

					//Result
					result = repository.GetResultForID (woDetails.WorPrimaryId);
					woDetails.Result = result;
					if(!string.IsNullOrEmpty(result.Description))
						resultTextField.Text = result.Description;

					//Fault
					if(repository.isFaultCodeRequired(woDetails.Request.PrimaryId, woDetails.ServiceCenter.PrimaryId))
					{
						fault = repository.GetFaultForID (woDetails.WorPrimaryId);
						woDetails.Fault = fault;
						if(!string.IsNullOrEmpty(fault.Description))
							faultTextField.Text = fault.Description;
					}
					else
						faultTextField.UserInteractionEnabled=false;

					//SafetyTest
					if(!string.IsNullOrEmpty(woDetails.AssetDetails.Control))
					{
						if (!string.IsNullOrEmpty (woDetails.SafetyTest)) {
							List<SystemCodeInfo> safetyTestList = getSafetyTestList ();
							safetyTest = safetyTestList.SingleOrDefault (x => (x.Abbreviation == woDetails.SafetyTest));
							safetyTest = safetyTest ?? new SystemCodeInfo ();
							safetyTestTextField.Text = safetyTest.Description;
						}
					}
					else
					{
						safetyTestView.Hidden=true;
					}

					//Notes
					WorkOrderText woTextObj = repository.GetWOText (woDetails.WorPrimaryId);
					if(woTextObj.Text!=null)
					{
						System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding ();
						woDetails.WOTextNotes = encoding.GetString (woTextObj.Text);
						notesTextView.Text = woDetails.WOTextNotes;
					}

					//Open Work Status
					owStatus = repository.GetOpenWorkStatusForID(woDetails.WorPrimaryId);
					woDetails.OpenWorkOrderStatus = owStatus;
					//initialRequestsActionCode = owStatus.RequestsActionCode;
					if(!string.IsNullOrEmpty(owStatus.Description))
					OWSButtonText.SetTitle(string.Format("Status - {0}", owStatus.Description), UIControlState.Normal);

					if(woDetails.LatestDispatchActionCode==(short)Consts.enumRequestsAction.AEWOs)
					{
						//we are not downloading Closed workorders and we are allow to download finish action with open flag, but we dont need to show lookup.
						//when locally saved dispatch having declined action dont need to fire lookup.
						if((owStatus.RequestsActionCode == (short)Consts.enumRequestsAction.Decline) || 
							(owStatus.RequestsActionCode == (short)Consts.enumRequestsAction.LogCompletion) ||
							(owStatus.RequestsActionCode == (short)Consts.enumRequestsAction.Finish))
								OWSButtonText.Alpha = (float)0.7;
					}
					else
					{
						if((woDetails.LatestDispatchActionCode == (short)Consts.enumRequestsAction.Decline) || 
							(woDetails.LatestDispatchActionCode == (short)Consts.enumRequestsAction.LogCompletion) ||
							(woDetails.LatestDispatchActionCode == (short)Consts.enumRequestsAction.Finish))
								OWSButtonText.Alpha = (float)0.7;
					}	
				}
			#endregion
		}

		public void SaveWorkOrder()
		{
//			woDetails.Result = result;
//			woDetails.Fault = fault;
//			//if(string.IsNullOrEmpty(safetyTest.Abbreviation))
//
//			woDetails.SafetyTest = safetyTest.Abbreviation ?? string.Empty;
//			woDetails.WOTextNotes = notesTextView.Text;
//			woDetails.OpenWorkOrderStatus = owStatus;


			//When there is no changes in workorder editor then we are not saving.
			//if (!object.Equals (woDetails, woDetailsCopy)) {
			if (!isWorkorderEqual()) {
				Repository repository = new Repository ();
				repository.UpdateWorkOrder (woDetails);
				string msg = string.Format ("{0} Updated WorkOrder: {1}", DateTime.Now, woDetails.Number);
				Consts.HandleExceptionLog (msg);
				Insights.Track("Work Order Editor View Controller","SaveWorkOrder",msg);

				WsMsgConnectorDAL.InsertWsMessageConnector ((int)EntityAction.Update, (int)EntityType.WorkOrder, Convert.ToString (woDetails.WorPrimaryId), Convert.ToString (woDetails.OpenWorkOrderStatus.PrimaryId));

				if(woDetails.TimeEntries!=null)
					woDetails.TimeEntries = null;
				woDetailsCopy = woDetails.ShallowCopy ();
			}
			
		}

		public bool isWorkorderEqual()
		{
			bool flag = true;
			if (woDetails.Result.PrimaryId != woDetailsCopy.Result.PrimaryId)
				flag = false;
			else if (woDetails.Fault.PrimaryId != woDetailsCopy.Fault.PrimaryId)
				flag = false;
			else if (woDetails.SafetyTest != woDetailsCopy.SafetyTest)
				flag = false;
			else if (woDetails.WOTextNotes != woDetailsCopy.WOTextNotes)
				flag = false;
			else if (woDetails.OpenWorkOrderStatus.PrimaryId != woDetailsCopy.OpenWorkOrderStatus.PrimaryId)
				flag = false;

			return flag;
		}

		private void UpdateUI()
		{
			if (result.PrimaryId != 0) {
				resultTextField.Text = result.Description;
			} else 
				resultTextField.Text = string.Empty;

			if (fault.PrimaryId != 0) {
				//resultLookUpText.SetTitle(result.Description, UIControlState.Normal);
				faultTextField.Text = fault.Description;
			} else
				faultTextField.Text = string.Empty;
				
			if (safetyTest.PrimaryId != 0) {
				//resultLookUpText.SetTitle(result.Description, UIControlState.Normal);
				safetyTestTextField.Text = safetyTest.Description;
			} else
				safetyTestTextField.Text = string.Empty;
		}

		public void ResultLookUpCall()
		{
			/*
			Repository repository= new Repository();
			List<SystemCode>results = repository.GetResults(1000000001, 1000000001);
			List<KeyValuePair<long, string>> tableItems = results.Select(it => new KeyValuePair<long, string> (it.PrimaryId, it.Description)).ToList();

			var selectListScreen  = new SelectListScreen(resultTextField.Placeholder, tableItems, result.PrimaryId, true);
			selectListScreen.ItemSelected += (object s, KeyValuePair<long, string> e) => {
				result.PrimaryId = e.Key;
				UpdateUI();
			};

			this.NavigationController.PushViewController(selectListScreen, true);
			*/

			Repository repository= new Repository();
			List<SystemCodeInfo> results = repository.GetResults (woDetails.Request.PrimaryId, woDetails.ServiceCenter.PrimaryId);
			List<KeyValuePair<long, string>> tableItems = results.Select(it => new KeyValuePair<long, string> (it.PrimaryId, it.Description)).ToList();

			//var selectListScreen  = new SelectListScreen(resultLookUpText.TitleLabel.Text, tableItems, result.PrimaryId, true);
			var selectListScreen  = new SelectListScreen("Result Lookup", tableItems, result.PrimaryId, true);
			selectListScreen.ItemSelected += (object s, KeyValuePair<long, string> e) => {
				//result.PrimaryId = e.Key;
				result = results.FirstOrDefault(x=>(x.PrimaryId==e.Key));
				result = result ?? new SystemCodeInfo();
				woDetails.Result = result;
				UpdateUI();
			};

			this.NavigationController.PushViewController(selectListScreen, true);
		}

		public void FaultLookUpCall()
		{
			Repository repository= new Repository();
			List<SystemCodeInfo>faults = repository.GetFaults(woDetails.ServiceCenter.PrimaryId);
			List<KeyValuePair<long, string>> tableItems = faults.Select(it => new KeyValuePair<long, string> (it.PrimaryId, it.Description)).ToList();

			//var selectListScreen  = new SelectListScreen(faultLookUpText.TitleLabel.Text, tableItems, fault.PrimaryId, true);
			var selectListScreen  = new SelectListScreen("Fault Lookup", tableItems, fault.PrimaryId, true);
			selectListScreen.ItemSelected += (object s, KeyValuePair<long, string> e) => {
				fault = faults.FirstOrDefault(x=>(x.PrimaryId==e.Key));
				fault = fault ?? new SystemCodeInfo();
				woDetails.Fault=fault;
				UpdateUI();
			};

			this.NavigationController.PushViewController(selectListScreen, true);
		}

		public List<SystemCodeInfo> getSafetyTestList()
		{
			List<SystemCodeInfo> stList = new List<SystemCodeInfo> (){
				//new SystemCodeInfo { Description = "", Abbreviation = "" }, 
				new SystemCodeInfo {PrimaryId=1000000001, Description = "Passed", Abbreviation = "P" }, 
				new SystemCodeInfo {PrimaryId=1000000002, Description = "Failed", Abbreviation = "F" },
				new SystemCodeInfo {PrimaryId=1000000003, Description = "Not Found", Abbreviation = "N" },
				new SystemCodeInfo {PrimaryId=1000000004, Description = "In Use", Abbreviation = "I" },
				new SystemCodeInfo {PrimaryId=1000000005, Description = "Not Applicable", Abbreviation = "A" },
				new SystemCodeInfo {PrimaryId=1000000006, Description = "Omitted", Abbreviation = "O" }
			};
			return stList;
		}

		//partial void SafetyTestTouchDown (MonoTouch.Foundation.NSObject sender)
		public void SafetyTestLookUpCall()
		{
			List<SystemCodeInfo>safetyTestList = getSafetyTestList();
			List<KeyValuePair<long, string>> tableItems = safetyTestList.Select(it => new KeyValuePair<long, string> (it.PrimaryId, it.Description)).ToList();

			var selectListScreen  = new SelectListScreen("Safety Test Lookup", tableItems, safetyTest.PrimaryId, true);
			selectListScreen.ItemSelected += (object s, KeyValuePair<long, string> e) => {
				safetyTest = safetyTestList.FirstOrDefault(x=>(x.PrimaryId==e.Key));
				safetyTest = safetyTest ?? new SystemCodeInfo();
				woDetails.SafetyTest = safetyTest.Abbreviation;
				UpdateUI();
			};

			this.NavigationController.PushViewController(selectListScreen, true);
		}

		partial void OWSButtonTouchUpInside (NSObject sender)
		{
			long initialOWSPrimaryId = owStatus.PrimaryId;
			Repository repository= new Repository();
			List<SystemCode>owstatuses;
			if(woDetails.LatestDispatchActionCode==(short)Consts.enumRequestsAction.AEWOs)
			{
				//we are not downloading declined dispatches.
				//when locally saved dispatch having declined action dont need to fire lookup.
				if(owStatus.RequestsActionCode == (short)Consts.enumRequestsAction.Decline)
					return;

				//we are not downloading Closed workorders and we are allow to download finish action with open flag, but we dont need to show lookup.
				if((owStatus.RequestsActionCode == (short)Consts.enumRequestsAction.LogCompletion) ||
					(owStatus.RequestsActionCode == (short)Consts.enumRequestsAction.Finish))
					return;

				owstatuses = repository.GetOpenWorkStatuses(owStatus.RequestsActionCode);
			}
			else
			{
				//we are not downloading declined dispatches.
				//when locally saved dispatch having declined action dont need to fire lookup.
				if(woDetails.LatestDispatchActionCode == (short)Consts.enumRequestsAction.Decline)
					return;

				//we are not downloading Closed workorders and we are allow to download finish action with open flag, but we dont need to show lookup.
				if((woDetails.LatestDispatchActionCode == (short)Consts.enumRequestsAction.LogCompletion) ||
					(woDetails.LatestDispatchActionCode == (short)Consts.enumRequestsAction.Finish))
					return;

				owstatuses = repository.GetOpenWorkStatuses(woDetails.LatestDispatchActionCode);
			}

			List<KeyValuePair<long, string>> tableItems = owstatuses.Select(it => new KeyValuePair<long, string> (it.PrimaryId, it.Description)).ToList();

			//var selectListScreen  = new SelectListScreen(faultLookUpText.TitleLabel.Text, tableItems, fault.PrimaryId, true);
			var selectListScreen  = new SelectListScreen("Open Work Status", tableItems, owStatus.PrimaryId);
			selectListScreen.ItemSelected += async (object s, KeyValuePair<long, string> e) => {
				owStatus = owstatuses.FirstOrDefault(x=>(x.PrimaryId==e.Key));
				owStatus = owStatus ?? new SystemCode();
				woDetails.OpenWorkOrderStatus=owStatus;
				//UpdateUI();
				//OWSButtonText.TitleLabel.Text= string.Format("Status - {0}", owStatus.Description);

				OWSButtonText.SetTitle(string.Format("Status - {0}", owStatus.Description), UIControlState.Normal);

				/*
				if((initialRequestsActionCode != owStatus.RequestsActionCode)&&(owStatus.RequestsActionCode == (short)Consts.enumRequestsAction.Start))
				{
				UIAlertView alert = new UIAlertView(string.Empty, "Add Time Entry?", null, "No", "Yes");
				alert.Clicked += TimeEntriesAlertHandler;
				alert.Show();
				}
				*/

				if (initialRequestsActionCode != owStatus.RequestsActionCode) 
				{
					switch (owStatus.RequestsActionCode)
					{
						case (short)Consts.enumRequestsAction.Start :{
							//Nahid Ahmed Jan 15, 2015: Update time entry whenever start action applied
							//if(woDetails.StartDate==null || woDetails.StartDate == DateTime.MinValue)
								if(initialRequestsActionCode != owStatus.RequestsActionCode){
									woDetails.StartDate = DateTime.Now;
									woDetails.StartTime = DateTime.Now;
								}
								break;
							}

						case (short)Consts.enumRequestsAction.Resume :{
							if(initialRequestsActionCode != owStatus.RequestsActionCode){
								woDetails.StartDate = DateTime.Now;
								woDetails.StartTime = DateTime.Now;
							}
							break;
						}

						case (short)Consts.enumRequestsAction.Finish:{
							UIAlertView alert = new UIAlertView(string.Empty, "Add Time Entry?", null, "No", "Yes");
							alert.Clicked += TimeEntriesAlertHandlerOnFinishAction;
							alert.Show();
							//Nahid Ahmed Jan 15, Time Entry should be added after alerting the user
							//AddAutoTimeEntry();
							woDetails.CompleteDate = DateTime.Now;
							woDetails.CompleteTime = DateTime.Now;
							break;
						}

						case (short)Consts.enumRequestsAction.Delay:{
							bool isRequiredTimeEntries = await AlertForTimeEntries();
							if(isRequiredTimeEntries)
								AddAutoTimeEntry();

//							UIAlertView alert = new UIAlertView(string.Empty, "Add Time Entry?", null, "No", "Yes");
//							alert.Clicked += TimeEntriesAlertHandlerOnDelayAction;
//							alert.Show();
							//AddAutoTimeEntry();
							break;
						}

						default: 
							break;
					}
				}

				//keep in buffer last RequestsActionCode.
				initialRequestsActionCode = owStatus.RequestsActionCode;

				//woDetails.LatestDispatchActionCode = owStatus.RequestsActionCode;

				
				if(owStatus.RequestsActionCode!=(short)Consts.enumRequestsAction.AEWOs)
				{
					woDetails.LatestDispatchActionCode = owStatus.RequestsActionCode;
				}
				

//				if(owStatus.RequestsActionCode.Equals((short)Consts.enumRequestsAction.Accept || (short)Consts.enumRequestsAction.Start || 
//					(short)Consts.enumRequestsAction.Delay || (short)Consts.enumRequestsAction.Decline || 
//					(short)Consts.enumRequestsAction.Resume || (short)Consts.enumRequestsAction.Finish))
//					woDetails.LatestDispatchActionCode = owStatus.RequestsActionCode;

				//while closing workorder, should be validate whether we filled all required information or not.
				if(owStatus.RequestsActionCode == (short)Consts.enumRequestsAction.Finish)
				{
					//PromptToCloseWorkOrder("OnOWSChange");
				}
				else
				{
					//on every change in workorder status we are saving locally, once we moved to wolist then we are uploading to server all changes.
					SaveWorkOrder();
				}
			};

			this.NavigationController.PushViewController(selectListScreen, true);
		}

		/*
		void TimeEntriesAlertHandler (object sender, UIButtonEventArgs e)
		{	
			//TimeEntriesViewController timeEntriesController = new TimeEntriesViewController(woDetails);
			TimeEntriesViewController timeEntriesController = new TimeEntriesViewController();
			if (e.ButtonIndex == 1) {
				//PresentViewController(timeEntriesController, true, null);
				AppBuffer.workOder = woDetails;
				AppBuffer.navPage = AppBuffer.enumNavPage.TimeEntry;
				this.NavigationController.PushViewController(timeEntriesController, true);
			}
		}
		*/


		public Task<bool> AlertForTimeEntries() {
			var tcs = new TaskCompletionSource<bool>();

			UIApplication.SharedApplication.InvokeOnMainThread(new NSAction(() =>
				{
					UIAlertView alert = new UIAlertView(string.Empty, "Add Time Entry?", null, NSBundle.MainBundle.LocalizedString("No", "No"),
						NSBundle.MainBundle.LocalizedString("Yes", "Yes"));
					//alert.Clicked += (sender, buttonArgs) => tcs.SetResult(buttonArgs.ButtonIndex != alert.CancelButtonIndex);
					alert.Clicked += (sender, buttonArgs) => tcs.SetResult(returnSeletedValue(buttonArgs.ButtonIndex));

					alert.Show();
				}));

			return tcs.Task;
		}

		public bool returnSeletedValue(int buttonIndex)
		{
			if (buttonIndex == 0) {
				return false;
			} else {
				return true;
			}
		}

		void TimeEntriesAlertHandlerOnFinishAction (object sender, UIButtonEventArgs e)
		{	
			//TimeEntriesViewController timeEntriesController = new TimeEntriesViewController(woDetails);
			TimeEntriesViewController timeEntriesController = new TimeEntriesViewController();
			if (e.ButtonIndex == 1) {
				AddAutoTimeEntry ();
			}
			PromptToCloseWorkOrder("OnOWSChange");
		}

		void TimeEntriesAlertHandlerOnDelayAction (object sender, UIButtonEventArgs e)
		{	
			//TimeEntriesViewController timeEntriesController = new TimeEntriesViewController(woDetails);
			TimeEntriesViewController timeEntriesController = new TimeEntriesViewController();
			if (e.ButtonIndex == 1) {
				AddAutoTimeEntry ();
			}
		}

		public void ShowAlertMessage(string alrtMessag)
		{
			UIAlertView alrt = new UIAlertView();
			alrt.Message = alrtMessag;
			alrt.AddButton("Ok"); 
			alrt.Show();
		}

		public void AddAutoTimeEntry()
		{
			WorkOrderTime woTime = new WorkOrderTime ();

			Repository repository = new Repository ();
			long WOMPrimaryId = repository.GetPDADBIDForRequest ("WOMPrimaryId");

			woTime.PrimaryId = ++WOMPrimaryId;
			woTime.LogDate = DateTime.Now;

			TimeSpan timeDiff = new TimeSpan(0);
			if(Convert.ToDateTime(woDetails.StartDate)!= DateTime.MinValue)
				timeDiff = DateTime.Now.Subtract(woDetails.StartDate);
				
			woTime.ActualTime =  Convert.ToDecimal(timeDiff.TotalHours);

			woTime.TimeType = new SystemCode ();
			woTime.TimeType.PrimaryId = 1000000001; //This is the requirement, we are going to pass 1st one.


			WorkOrderTime[] woTimeEntry = woDetails.TimeEntries;
			int count = 0;
			if (woTimeEntry != null)
				count = woDetails.TimeEntries.Length;

			Array.Resize<WorkOrderTime> (ref woTimeEntry, count + 1);
			woTimeEntry [count] = woTime;
			woDetails.TimeEntries = woTimeEntry;

			/*
			woDetails.TimeEntries = new WorkOrderTime[1];
			woDetails.TimeEntries [0] = woTime;
			*/

			repository.UpdatePDADBIDForRequest ("WOMPrimaryId", woTime.PrimaryId);
		}

		/*
		async void PromptToCloseWorkOrder()
		{
			bool accepted = await ShowAlert("Info", "Do you really...?");

			Console.WriteLine("Selected button {0}", accepted ? "Accepted" : "Canceled");
		}


		public Task<bool> ShowAlert(string title, string message) {
			var tcs = new TaskCompletionSource<bool>();

			UIApplication.SharedApplication.InvokeOnMainThread(new NSAction(() =>
				{
					UIAlertView alert = new UIAlertView(title, message, null, NSBundle.MainBundle.LocalizedString("Cancel", "Cancel"),
						NSBundle.MainBundle.LocalizedString("OK", "OK"));
					alert.Clicked += (sender, buttonArgs) => tcs.SetResult(PromptToCloseWorkOrderAlertHandler1(buttonArgs.ButtonIndex));
					alert.Show();
				}));

			return tcs.Task;
		}

		public bool PromptToCloseWorkOrderAlertHandler1(int buttonIndex)
		{
			if (buttonIndex == 0) {
				return true;
			} else {
				bool canCloseWorkOrder = validateWorkOrder ();
				return canCloseWorkOrder;
			}
		}
		*/

		public void PromptToCloseWorkOrder (string action)
		{
			UIAlertView alert = new UIAlertView(string.Empty, "Would you like to Close this Work Order?", null, "No", "Yes");
			if(action == "OnOWSChange")
				alert.Clicked += PromptToCloseWorkOrderAlertHandlerOnOWSChange;
			else
				alert.Clicked += PromptToCloseWorkOrderAlertHandlerOnBackSave;
			alert.Show();
		}

		void PromptToCloseWorkOrderAlertHandlerOnOWSChange (object sender, UIButtonEventArgs e)
		{	
			switch (e.ButtonIndex) {
				case 0://if user selects "NO", the work order will still save, but the work order remains open.
						SaveWorkOrder ();
						//After Adding Finish action to the workorder we dont need to display in the list. 
						Consts.isSyncRequired=true;
						break;

			case 1: //if user selects "YES", the app should check required fields and close work order if possible.
						bool canCloseWorkOrder = validateWorkOrder ();

						if (canCloseWorkOrder) {
							SaveWorkOrder ();
							//Nahid Ahmed Jan 15 - Lets pop to WO list immediately after WO is closed
					        this.NavigationController.PopViewControllerAnimated(true);
						}
						break;
			}
		}

		void PromptToCloseWorkOrderAlertHandlerOnBackSave (object sender, UIButtonEventArgs e)
		{	
			switch (e.ButtonIndex) {
			case 0://if user selects "NO", the work order will still save, but the work order remains open.
				SaveWorkOrder ();
				//After Adding Finish action to the workorder we dont need to display in the list. 
				Consts.isSyncRequired=true;
				this.NavigationController.PopViewControllerAnimated(true);
				break;

			case 1: //if user selects "YES", the app should check required fields and close work order if possible.
				bool canCloseWorkOrder = validateWorkOrder ();

				if (canCloseWorkOrder) {
					SaveWorkOrder ();
					this.NavigationController.PopViewControllerAnimated(true);
				}
				break;
			}
		}

		public bool validateWorkOrder()
		{
			bool flag = true;
			Repository repository = new Repository ();

			if (woDetails.canWOBeClosed == 1) {
				flag = false;

				UIAlertView alert = new UIAlertView (string.Empty, "Saving your work. To Close the Work Order, please complete open items in Asset Enterprise.", null, "OK");
				alert.Show ();
			} else if (woDetails.Worker.WKRPrimaryId == 0) {
				flag = false;

				UIAlertView alert = new UIAlertView ("Primary Worker is a required field to close the Work Order.", "Please enter a valid Primary Worker.", null, "OK");
				alert.Show();
			} else if (woDetails.Request.PrimaryId == 0) {
				flag = false;

				UIAlertView alert = new UIAlertView (string.Empty, "Plese select a Request.", null, "OK");
				alert.Show();
			} else if (woDetails.Result.PrimaryId == 0) {
				flag = false;

				UIAlertView alert = new UIAlertView ("Result is a required field to close the Work Order.", "Please select a Result.", null, "OK");
				alert.Show();
			} else if ((woDetails.IssueDate == null) || (woDetails.IssueDate == DateTime.MinValue)) {
				flag = false;

				UIAlertView alert = new UIAlertView ("Issue Date is required to close the Work Order.", "Please enter a valid Issue Date.", null, "OK");
				alert.Show();
			} else if ((woDetails.StartDate == null) || (woDetails.StartDate == DateTime.MinValue)) {
				flag = false;

				UIAlertView alert = new UIAlertView ("Start Date is required to close the Work Order.", "Please enter a valid Start Date.", null, "OK");
				alert.Show();
			} else if ((woDetails.CompleteDate == null) || (woDetails.CompleteDate == DateTime.MinValue)) {
				flag = false;

				UIAlertView alert = new UIAlertView ("Complete Date is required to close the Work Order.", "Please enter a valid Complete Date.", null, "OK");
				alert.Show();
			} else if (woDetails.IssueDate > woDetails.StartDate) {
				flag = false;

				UIAlertView alert = new UIAlertView (string.Empty, "Issue Date should always be less than Start Date.", null, "OK");
				alert.Show();
			} else if (woDetails.StartDate > woDetails.CompleteDate) {
				flag = false;

				UIAlertView alert = new UIAlertView (string.Empty, "Start Date should always be less than Completed Date.", null, "OK");
				alert.Show();
			} else if ((string.IsNullOrEmpty(woDetails.AssetDetails.Control)) && (repository.isControlRequired(woDetails.Request.PrimaryId, woDetails.ServiceCenter.PrimaryId))) {
				flag = false;

				UIAlertView alert = new UIAlertView ("Control is a required field.", "Please select control.", null, "OK");
				alert.Show();
			} else if (woDetails.Account.AccPrimaryId==0) {
				flag = false;

				UIAlertView alert = new UIAlertView ("Account is a required field.", "Please select a Account.", null, "OK");
				alert.Show();
			} else if((faultTextField.UserInteractionEnabled && string.IsNullOrEmpty(faultTextField.Text)))
			{
				flag = false;

				UIAlertView alert = new UIAlertView ("Fault is a required field for the Request type.", "Please select a Fault.", null, "OK");
				alert.Show();
			}
			else if(string.IsNullOrEmpty(safetyTestTextField.Text) && !string.IsNullOrEmpty(woDetails.AssetDetails.Control))
			{
				flag = false;

				UIAlertView alert = new UIAlertView ("Safety Test field is a required field.", "Please select a Safety Test.", null, "OK");
				alert.Show();
			}

			if(flag)
				woDetails.OpenFlag = 0;

			return flag;
		}
	}
}
	