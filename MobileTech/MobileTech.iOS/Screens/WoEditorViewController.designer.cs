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
	[Register ("WoEditorViewController")]
	partial class WoEditorViewController
	{
		[Outlet]
		MonoTouch.UIKit.UILabel controlNoLabel { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel departmentLabel { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel descriptionLabel { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel devCatLabel { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITextField faultTextField { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITextView notesTextView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton OWSButtonText { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel owStatusLabel { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel reqCodeDescLabel { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITextField resultTextField { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITextField safetyTestTextField { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIView safetyTestView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel urgencyLabel { get; set; }

		[Action ("OWSButtonTouchUpInside:")]
		partial void OWSButtonTouchUpInside (MonoTouch.Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (controlNoLabel != null) {
				controlNoLabel.Dispose ();
				controlNoLabel = null;
			}

			if (departmentLabel != null) {
				departmentLabel.Dispose ();
				departmentLabel = null;
			}

			if (descriptionLabel != null) {
				descriptionLabel.Dispose ();
				descriptionLabel = null;
			}

			if (devCatLabel != null) {
				devCatLabel.Dispose ();
				devCatLabel = null;
			}

			if (faultTextField != null) {
				faultTextField.Dispose ();
				faultTextField = null;
			}

			if (notesTextView != null) {
				notesTextView.Dispose ();
				notesTextView = null;
			}

			if (OWSButtonText != null) {
				OWSButtonText.Dispose ();
				OWSButtonText = null;
			}

			if (owStatusLabel != null) {
				owStatusLabel.Dispose ();
				owStatusLabel = null;
			}

			if (reqCodeDescLabel != null) {
				reqCodeDescLabel.Dispose ();
				reqCodeDescLabel = null;
			}

			if (resultTextField != null) {
				resultTextField.Dispose ();
				resultTextField = null;
			}

			if (safetyTestTextField != null) {
				safetyTestTextField.Dispose ();
				safetyTestTextField = null;
			}

			if (urgencyLabel != null) {
				urgencyLabel.Dispose ();
				urgencyLabel = null;
			}

			if (safetyTestView != null) {
				safetyTestView.Dispose ();
				safetyTestView = null;
			}
		}
	}
}
