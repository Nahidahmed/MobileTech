// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;
using System.CodeDom.Compiler;

namespace MobileTech.iOS
{
	[Register ("TimeEntriesViewController")]
	partial class TimeEntriesViewController
	{
		[Outlet]
		MonoTouch.UIKit.UITextField actualTimeTextField { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITextField dateTextField { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITextView timeNotesTextView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITextField timeTextField { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITextField timeTypeTextField { get; set; }

		[Action ("cancelButtonTouchUpInside:")]
		partial void cancelButtonTouchUpInside (MonoTouch.Foundation.NSObject sender);

		[Action ("saveButton:")]
		partial void saveButton (MonoTouch.Foundation.NSObject sender);

		[Action ("saveButtonTouchUpInside:")]
		partial void saveButtonTouchUpInside (MonoTouch.Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (actualTimeTextField != null) {
				actualTimeTextField.Dispose ();
				actualTimeTextField = null;
			}

			if (dateTextField != null) {
				dateTextField.Dispose ();
				dateTextField = null;
			}

			if (timeNotesTextView != null) {
				timeNotesTextView.Dispose ();
				timeNotesTextView = null;
			}

			if (timeTextField != null) {
				timeTextField.Dispose ();
				timeTextField = null;
			}

			if (timeTypeTextField != null) {
				timeTypeTextField.Dispose ();
				timeTypeTextField = null;
			}
		}
	}
}
