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
	[Register ("MyProfileViewController")]
	partial class MyProfileViewController
	{
		[Outlet]
		MonoTouch.UIKit.UIButton departmentBtn { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITextField departmentLookUpTextField { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITextField facilityLookUpTextField { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel requiredDepartments { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel requiredFacilities { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel requiredWorkers { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITextField selectedDepartmentsTextField { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITextField selectedFacilitiesTextField { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITextField selectedWorkersTextField { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITextField workerLookUpTextField { get; set; }

		[Action ("departmentBtnArrowTap:")]
		partial void departmentBtnArrowTap (MonoTouch.Foundation.NSObject sender);

		[Action ("facilityBtnArrowTap:")]
		partial void facilityBtnArrowTap (MonoTouch.Foundation.NSObject sender);

		[Action ("saveFilterTouchUpInside:")]
		partial void saveFilterTouchUpInside (MonoTouch.Foundation.NSObject sender);

		[Action ("workerBtnArrowTap:")]
		partial void workerBtnArrowTap (MonoTouch.Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (departmentBtn != null) {
				departmentBtn.Dispose ();
				departmentBtn = null;
			}

			if (departmentLookUpTextField != null) {
				departmentLookUpTextField.Dispose ();
				departmentLookUpTextField = null;
			}

			if (facilityLookUpTextField != null) {
				facilityLookUpTextField.Dispose ();
				facilityLookUpTextField = null;
			}

			if (selectedFacilitiesTextField != null) {
				selectedFacilitiesTextField.Dispose ();
				selectedFacilitiesTextField = null;
			}

			if (selectedWorkersTextField != null) {
				selectedWorkersTextField.Dispose ();
				selectedWorkersTextField = null;
			}

			if (workerLookUpTextField != null) {
				workerLookUpTextField.Dispose ();
				workerLookUpTextField = null;
			}

			if (selectedDepartmentsTextField != null) {
				selectedDepartmentsTextField.Dispose ();
				selectedDepartmentsTextField = null;
			}

			if (requiredFacilities != null) {
				requiredFacilities.Dispose ();
				requiredFacilities = null;
			}

			if (requiredDepartments != null) {
				requiredDepartments.Dispose ();
				requiredDepartments = null;
			}

			if (requiredWorkers != null) {
				requiredWorkers.Dispose ();
				requiredWorkers = null;
			}
		}
	}
}
