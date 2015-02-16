using System;
using System.Collections.Generic;
using System.Linq;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MobileTech;
using MobileTech.iOS.Screens;
//using SQLite;

namespace MobileTech.iOS
{
	// The UIApplicationDelegate for the application. This class is responsible for launching the
	// User Interface of the application, as well as listening (and optionally responding) to
	// application events from iOS.
	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{
		// class-level declarations
		UIWindow window;

		//
		// This method is invoked when the application has loaded and is ready to run. In this
		// method you should instantiate the window, load the UI into it and then make the window
		// visible.
		//
		// You have 17 seconds to return from this method, or iOS will terminate your application.
		//
		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			UINavigationBar.Appearance.SetTitleTextAttributes(new UITextAttributes()
			{
				TextColor = UIColor.White
			});

//			SQLite.SQLite3.Config (SQLite3.ConfigOption.Serialized);
			NSObject ver = NSBundle.MainBundle.InfoDictionary["CFBundleShortVersionString"];
			NSObject build = NSBundle.MainBundle.InfoDictionary["CFBundleVersion"];

			Consts.AppVersionWithBuild = string.Format ("{0}, Build {1}", ver, build);

			// create a new window instance based on the screen size
			window = new UIWindow (UIScreen.MainScreen.Bounds);

			// If you have defined a root view controller, set it here:
			// window.RootViewController = myViewController;

			window = new UIWindow (UIScreen.MainScreen.Bounds);

			try {
				//To Update the App build without uninstalling previous one.
				var sud = NSUserDefaults.StandardUserDefaults;
				int localBuild = sud.IntForKey("localBuildNo");
				int newBuild = Convert.ToInt32(build.ToString());
				if(localBuild != newBuild)
				{
					MobileTech.Consts.SetupDatabase(true);
					sud.SetInt(newBuild, "localBuildNo");
				}

				//MobileTech.Consts.SetupDatabase(false);
			}
			catch(Exception ex)
			{
				MobileTech.Consts.LogException (ex);

				UIAlertView al =  new UIAlertView();
				al.Message = ex.Message;
				al.Show();
			}

			var rootNavigationController=new UINavigationController();

			var viewController = new LoginViewController ();

			rootNavigationController.PushViewController(viewController,true);
			this.window.RootViewController = rootNavigationController;

			// make the window visible
			window.MakeKeyAndVisible ();

			return true;
		}
	}
}

