// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;
using System.CodeDom.Compiler;

namespace MobileTech.iOS.Screens
{
	[Register ("LoginViewController")]
	partial class LoginViewController
	{
		[Outlet]
		MonoTouch.UIKit.UIActivityIndicatorView activityMonitor { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton btnPurgeDB { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton btnUploadLogToServer { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel lblDBVersion { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel lblWSVersion { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIView loginView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIView mAnimationContainer { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIView mServerSetupView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITextField txtPassword { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITextField txtServerAddress { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITextField txtUsername { get; set; }

		[Action ("btnLoginAction:")]
		partial void btnLoginAction (MonoTouch.Foundation.NSObject sender);

		[Action ("btnPurgeDBClicked:")]
		partial void btnPurgeDBClicked (MonoTouch.Foundation.NSObject sender);

		[Action ("btnUploadLogToServerClick:")]
		partial void btnUploadLogToServerClick (MonoTouch.Foundation.NSObject sender);

		[Action ("flipToLogin:")]
		partial void flipToLogin (MonoTouch.Foundation.NSObject sender);

		[Action ("flipToServerSetup:")]
		partial void flipToServerSetup (MonoTouch.Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (activityMonitor != null) {
				activityMonitor.Dispose ();
				activityMonitor = null;
			}

			if (btnPurgeDB != null) {
				btnPurgeDB.Dispose ();
				btnPurgeDB = null;
			}

			if (lblDBVersion != null) {
				lblDBVersion.Dispose ();
				lblDBVersion = null;
			}

			if (lblWSVersion != null) {
				lblWSVersion.Dispose ();
				lblWSVersion = null;
			}

			if (loginView != null) {
				loginView.Dispose ();
				loginView = null;
			}

			if (mAnimationContainer != null) {
				mAnimationContainer.Dispose ();
				mAnimationContainer = null;
			}

			if (mServerSetupView != null) {
				mServerSetupView.Dispose ();
				mServerSetupView = null;
			}

			if (txtPassword != null) {
				txtPassword.Dispose ();
				txtPassword = null;
			}

			if (txtServerAddress != null) {
				txtServerAddress.Dispose ();
				txtServerAddress = null;
			}

			if (txtUsername != null) {
				txtUsername.Dispose ();
				txtUsername = null;
			}

			if (btnUploadLogToServer != null) {
				btnUploadLogToServer.Dispose ();
				btnUploadLogToServer = null;
			}
		}
	}
}
