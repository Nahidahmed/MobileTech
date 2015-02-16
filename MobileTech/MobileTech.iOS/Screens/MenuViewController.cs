using System;
using System.Drawing;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Collections.Generic;
using MobileTech.iOS.Screens;
using System.Threading.Tasks;
using MobileTech.Core;
using Xamarin;

namespace MobileTech.iOS
{
	public partial class MenuViewController : UIViewController
	{
		public MenuViewController () : base ("MenuViewController", null)
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
			this.Title = "Menu";
			base.ViewWillAppear (animated);
		}

		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);
			this.Title = "";
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			List<KeyValuePair<long, string>> items = new [] {
//				new KeyValuePair<long, string> ((long)MenuOption.MyRequests, "My Requests"),
//				new KeyValuePair<long, string> ((long)MenuOption.Home, "Home"),
				new KeyValuePair<long, string> ((long)MenuOption.MyProfile, "My Profile"),
//				new KeyValuePair<long, string> ((long)MenuOption.About, "About"),
				new KeyValuePair<long, string> ((long)MenuOption.Logout, "Logout")
			}.ToList ();

			var tableSource = new KeyValueTableSource (items, null);

			tableSource.RowSelectedEvent +=	TableRowSelected;

			this.Table.Source = tableSource;

			// Perform any additional setup after loading the view, typically from a nib.
		}

		protected void TableRowSelected(object sender, KeyValuePair<long, string> e)
		{
			KeyValueTableSource source = (KeyValueTableSource)sender;

			MenuOption menuOption = (MenuOption)e.Key;

			//var viewControllers = this.NavigationController.ViewControllers.ToArray ();
			//this.NavigationController.SetViewControllers (viewControllers, false);
			//var homeViewController = new HomeViewController();
			UINavigationController navController = this.NavigationController;

			switch (menuOption) {

			case MenuOption.MyProfile:
				var myProfileViewController = new MyProfileViewController ();
				navController.PopViewControllerAnimated (false);
				navController.PushViewController (myProfileViewController, true);
				break;

//			case MenuOption.About:
//				var aboutViewController = new AboutViewController ();
//				navController.PopViewControllerAnimated (false);
//				navController.PushViewController (aboutViewController, true);
//				break;

			case MenuOption.Logout:
				Task.Factory.StartNew(async () => {
					bool accepted = await AlertHelper.ShowYesNoAlert("Logout", "Logout of the application?");

					if (accepted)
					{
						InvokeOnMainThread ( () => {
							var loginViewController  = new LoginViewController(true);
							navController.SetViewControllers(new [] { loginViewController }, true);
						});

						string msg = string.Format("{0} User logged out or his session got terminated: {1}/{2}/{3}.", DateTime.Now, Consts.LoginUserName, Consts.DeviceId, MobileTechService.AuthClient.GetInstance().CurrentSession);
						Consts.HandleExceptionLog(msg);
						Insights.Track("Menu View Controller","TableRowSelected",msg);
					}
				});
			break;
			}
		}
	}

	public enum MenuOption {
		MyRequests = 1,
		Home,
		MyProfile,
		About,
		Logout
	}
}

