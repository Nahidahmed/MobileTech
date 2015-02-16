using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

using MobileTech.Core;
using MobileTech.iOS.Screens;
using MobileTech.Models.Entity;
using MobileTech.ModalPicker;

namespace MobileTech.iOS
{
	public partial class TimeEntriesViewController : UIViewController
	{
		WorkOrderDetails woDetails;
		//SystemCode timeType = new SystemCode();
		WorkOrderTime woTime = new WorkOrderTime();

		private UIView activeView;            //Controller that activated the keyboard
		private float scroll_amount = 0.0f;   //amount to scroll
		private float bottom = 0.0f;          //bottom point
		private float offset = 50.0f;         //extra offset
		private bool moveViewUp = false;      //direction up or down

		/*
		private DateTime[] _customDates;
		private void Initialize()
		{
			_customDates = new DateTime[] 
			{ 
				DateTime.Now, DateTime.Now.AddDays(7), DateTime.Now.AddDays(7*2), 
				DateTime.Now.AddDays(7*3), DateTime.Now.AddDays(7*4), DateTime.Now.AddDays(7*5),
				DateTime.Now.AddDays(7*6), DateTime.Now.AddDays(7*7), DateTime.Now.AddDays(7*8),
				DateTime.Now.AddDays(7*9), DateTime.Now.AddDays(7*10), DateTime.Now.AddDays(7*11), 
				DateTime.Now.AddDays(7*12), DateTime.Now.AddDays(7*13), DateTime.Now.AddDays(7*14)
			};
		}
		*/

		static bool UserInterfaceIdiomIsPhone {
			get { return UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone; }
		}

		public TimeEntriesViewController ()
			: base (UserInterfaceIdiomIsPhone ? "TimeEntriesViewController_iPhone" : "TimeEntriesViewController_iPad", null)
		{
		}

		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
			
			// Release any cached data, images, etc that aren't in use.
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			this.Title="Work Order - Time Entry";
		}

		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);
			this.Title = string.Empty;
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			//Initialize ();

			woDetails = AppBuffer.workOder;

			// Perform any additional setup after loading the view, typically from a nib.
			NSNotificationCenter.DefaultCenter.AddObserver (UIKeyboard.DidShowNotification, KeyBoardUpNotification);
			NSNotificationCenter.DefaultCenter.AddObserver (UIKeyboard.WillHideNotification, KeyBoardDownNotification);

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

			timeNotesTextView.InputAccessoryView = keyboardDoneButtonToolbar;

			this.NavigationItem.SetHidesBackButton (true, false);
			initializeSettings ();
			fillTimeEntries ();
			Repository repository = new Repository ();
			long WOMPrimaryId = repository.GetPDADBIDForRequest ("WOMPrimaryId");
			woTime.PrimaryId = ++WOMPrimaryId;

			/*
			timeTextField.ShouldBeginEditing = delegate {
				//Create custom data source
				var customDatesList = new List<string>();
				foreach(var date in _customDates)
				{
					customDatesList.Add(date.ToString("ddd, MMM dd, yyyy"));
				}

				//Create the modal picker and style it as you see fit
				var modalPicker = new ModalPickerViewController(ModalPickerType.Custom, "Select A Date", this)
				{
					HeaderBackgroundColor = UIColor.Blue,
					HeaderTextColor = UIColor.White,
					TransitioningDelegate = new ModalPickerTransitionDelegate(),
					ModalPresentationStyle = UIModalPresentationStyle.Custom
				};

				//Create the model for the Picker View
				modalPicker.PickerView.Model = new CustomPickerModel(customDatesList);

				//On an item is selected, update our label with the selected item.
				modalPicker.OnModalPickerDismissed += (s, ea) => 
				{
					var index = modalPicker.PickerView.SelectedRowInComponent(0);
					timeTextField.Text = customDatesList[index];
				};

				PresentViewControllerAsync(modalPicker, true);

				return false;
			};
		*/

		}

		private void KeyBoardUpNotification(NSNotification notifcation)
		{
			try
			{
				RectangleF r = UIKeyboard.BoundsFromNotification (notifcation);

				UIView view = this.View.Subviews.Single(x => (x.GetType().Name == "UITextView"));
				if(view!=null)
					activeView = view;

				/*
				foreach (UIView view in this.View.Subviews) {
					if(view.GetType().Name == "UITextView"){
						//					if (view.IsFirstResponder)
						activeView = view;
					}
				}
				*/

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
			timeNotesTextView.ResignFirstResponder();
		}

		public void initializeSettings()
		{
			//We are not allowing to edit past or future dates
			dateTextField.UserInteractionEnabled = false;
			/*
			dateTextField.ShouldBeginEditing = delegate {
				var modalPicker = new ModalPickerViewController (ModalPickerType.Date, "Select Date", this) {
					HeaderBackgroundColor = UIColor.Orange,
					HeaderTextColor = UIColor.White,
					TransitioningDelegate = new ModalPickerTransitionDelegate (),
					ModalPresentationStyle = UIModalPresentationStyle.Custom
				};

				modalPicker.DatePicker.Mode = UIDatePickerMode.Date;

				modalPicker.OnModalPickerDismissed += (s, ea) => {
					var dateFormatter = new NSDateFormatter () {
						DateFormat = "MM/dd/yyyy"
					};

					dateTextField.Text = dateFormatter.ToString (modalPicker.DatePicker.Date);
				};

				PresentViewControllerAsync (modalPicker, true);

				return false;
			};
			*/

			//We are not allowing to edit past or future Time
			timeTextField.UserInteractionEnabled = false;
			/*
			timeTextField.ShouldBeginEditing = delegate {
				var modalPicker = new ModalPickerViewController (ModalPickerType.Date, "Select Time", this) {
					HeaderBackgroundColor = UIColor.Orange,
					HeaderTextColor = UIColor.White,
					TransitioningDelegate = new ModalPickerTransitionDelegate (),
					ModalPresentationStyle = UIModalPresentationStyle.Custom
				};

				modalPicker.DatePicker.Mode = UIDatePickerMode.Time;

				modalPicker.OnModalPickerDismissed += (s, ea) => {
					var dateFormatter = new NSDateFormatter () {
						//DateFormat = "MM/dd/yyyy"
						DateFormat = "hh:mm a"
					};

					timeTextField.Text = dateFormatter.ToString (modalPicker.DatePicker.Date);
				};

				PresentViewControllerAsync (modalPicker, true);

				return false;
			};
			*/

			actualTimeTextField.ShouldBeginEditing = delegate {
				var modalPicker = new ModalPickerViewController (ModalPickerType.Date, "Select Time", this) {
					HeaderBackgroundColor = UIColor.Orange,
					HeaderTextColor = UIColor.White,
					TransitioningDelegate = new ModalPickerTransitionDelegate (),
					ModalPresentationStyle = UIModalPresentationStyle.Custom
				};

				modalPicker.DatePicker.Mode = UIDatePickerMode.CountDownTimer;

				modalPicker.OnModalPickerDismissed += (s, ea) => {
					var dateFormatter = new NSDateFormatter () {
						//DateFormat = "MM/dd/yyyy"
						DateFormat = "HH:mm"
					};

					actualTimeTextField.Text = dateFormatter.ToString (modalPicker.DatePicker.Date);
				};

				PresentViewControllerAsync (modalPicker, true);

				return false;
			};


			timeTypeTextField.ShouldBeginEditing = (textField) => {
				TimeTypeLookUpCall();
				return false;
			};
		}

		public void TimeTypeLookUpCall()
		{
			//SystemCode timeType = new SystemCode ();
			if (woTime.TimeType == null)
				woTime.TimeType = new SystemCode ();
			Repository repository = new Repository();

			List<SystemCode> timeTypes = repository.GetTimeTypes();
			List<KeyValuePair<long, string>> tableItems = timeTypes.Select(it => new KeyValuePair<long, string> (it.PrimaryId, it.Description)).ToList();

			var selectListScreen  = new SelectListScreen("Time Type Lookup", tableItems, woTime.TimeType.PrimaryId, true);
			selectListScreen.ItemSelected += (object s, KeyValuePair<long, string> e) => {
				//result.PrimaryId = e.Key;
				woTime.TimeType = timeTypes.FirstOrDefault(x=>(x.PrimaryId==e.Key));
				woTime.TimeType = woTime.TimeType ?? new SystemCode();
				UpdateUI();
				//woDetails.Result = result;
			};

			this.NavigationController.PushViewController(selectListScreen, true);
			//PresentViewController(selectListScreen, true, null);
		}

		private void UpdateUI()
		{
			if (woTime.TimeType.PrimaryId != 0) {
				timeTypeTextField.Text = woTime.TimeType.Description;
			} else 
				timeTypeTextField.Text = string.Empty;
		}

		partial void cancelButtonTouchUpInside (NSObject sender)
		{
			woTime = new WorkOrderTime();
			this.NavigationController.PopViewControllerAnimated(true);
		}

		partial void saveButtonTouchUpInside (NSObject sender)
		{
			saveTimeEntries();

			//this.NavigationController.PopViewControllerAnimated(true);
			//WoEditorViewController woEditorViewController = new WoEditorViewController(woTime);
			//woEditorViewController.updateWoTime(woTime);
			//this.NavigationController.PopToViewController(woEditorViewController, true);

			Repository repository = new Repository ();
			repository.UpdatePDADBIDForRequest ("WOMPrimaryId", woTime.PrimaryId);

			AppBuffer.workOder = woDetails;
			this.NavigationController.PopViewControllerAnimated(true);
		}

		public void saveTimeEntries()
		{
			//string iString = "2009-05-08 14:40:52,531";
			string strDateTextField = dateTextField.Text.Replace('-', '/');
			DateTime d = DateTime.ParseExact(strDateTextField, "MM/dd/yyyy",System.Globalization.CultureInfo.InvariantCulture);
			//DateTime d = Convert.ToDateTime (dateTextField.Text);
			DateTime t = Convert.ToDateTime (timeTextField.Text);
			woTime.LogDate = new DateTime(d.Year, d.Month, d.Day, t.Hour, t.Minute, t.Second);
			Consts.LastActionDateTime = woTime.LogDate.ToString();

			woTime.ActualTime = Convert.ToDecimal(TimeSpan.Parse(actualTimeTextField.Text).TotalHours);
			System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding ();

			woTime.Notes = encoding.GetBytes(timeNotesTextView.Text);
			woTime.strNotes = timeNotesTextView.Text;

			//Consts.woTime = woTime;

			int count = 0;
			if (woDetails.TimeEntries != null) 
				count = woDetails.TimeEntries.Length;

			woDetails.TimeEntries = new WorkOrderTime[count + 1];
			woDetails.TimeEntries [count] = woTime;

			//NSUserDefaults.StandardUserDefaults.SetValueForKey (woTime, new NSString("woTime"));

			/*
			int count = 0;
			if (woDetails.TimeEntries == null) {
				woDetails.TimeEntries = new WorkOrderTime[count + 1];
				woDetails.TimeEntries [count] = woTime;
			} else {
				count = woDetails.TimeEntries.Length;
				woDetails.TimeEntries = new WorkOrderTime[count + 1];
				woDetails.TimeEntries [count] = woTime;
			}
			*/

			//Appending Time Entries notes to Work Order Notes
			//woDetails.WOTextNotes = woDetails.WOTextNotes + Environment.NewLine + timeNotesTextView.Text;

		}

		public void fillTimeEntries()
		{
			dateTextField.Text = DateTime.Now.ToString("MM/dd/yyyy");
			timeTextField.Text = DateTime.Now.ToString("hh:mm tt");
//			if(woTime.TimeType!=null)
//				timeTypeTextField.Text = woTime.TimeType.Description;
			TimeSpan timeDiff = new TimeSpan(0);
			if(Convert.ToDateTime(Consts.LastActionDateTime)!= DateTime.MinValue)
				timeDiff = DateTime.Now.Subtract(Convert.ToDateTime(Consts.LastActionDateTime));
//			if(timeDiff.Ticks==0)
//				actualTimeTextField.Text="00:00";
//			else
				actualTimeTextField.Text = string.Format("{0:00}:{1:00}", timeDiff.Hours, timeDiff.Minutes);
			timeNotesTextView.Text = string.Empty;
		}
	}
}

