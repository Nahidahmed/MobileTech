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
	[Register ("MultiSelectListScreen")]
	partial class MultiSelectListScreen
	{
		[Outlet]
		MonoTouch.UIKit.UISearchBar searchBar { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITableView Table { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (Table != null) {
				Table.Dispose ();
				Table = null;
			}

			if (searchBar != null) {
				searchBar.Dispose ();
				searchBar = null;
			}
		}
	}
}
