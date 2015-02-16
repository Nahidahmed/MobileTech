
using System;
using System.Drawing;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MobileTech.Core;
using System.Collections.Generic;

namespace MobileTech.iOS.Screens
{
	public partial class SelectListScreen : UIViewController
	{
		private string _title;
		private long? selectedItemKey;
		public bool AutoPopController { get; set; }
		public event EventHandler<KeyValuePair<long, string>> ItemSelected;

		List<KeyValuePair<long, string>> items;
		public SelectListScreen (string title, List<KeyValuePair<long, string>> items, long? selectedItemKey, bool showNone = false) : base ("SelectListScreen", null)
		{
			this._title = title;
			this.AutoPopController = true;
			this.items = items.ToList ();
			if (showNone) {
				this.items.Insert (0, new KeyValuePair<long, string> (0, "( None )"));
			}
			this.selectedItemKey = selectedItemKey;
		}

		public SelectListScreen (string title, List<KeyValuePair<long, string>> items, long? selectedItemKey, List<KeyValuePair<long, string>> additionalItems, bool showNone = false) : base ("SelectListScreen", null)
		{
			this._title = title;
			this.AutoPopController = true;
			if (additionalItems.Count > 0) {
				items.InsertRange (0, additionalItems);
			}

			this.items = items.ToList ();

			if (showNone) {
					this.items.Insert (0, new KeyValuePair<long, string> (0, "( None )"));
			}
			this.selectedItemKey = selectedItemKey;
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

			KeyValueTableSource dataSource = new KeyValueTableSource (items, selectedItemKey);
			dataSource.RowSelectedEvent += TableRowSelected;

			this.Table.Source = dataSource;

			this.searchBar.TextChanged += (object sender, UISearchBarTextChangedEventArgs e) => 
			{
				dataSource.Filter(searchBar.Text);
				this.Table.ReloadData();
			};

			this.searchBar.SearchButtonClicked+= (object sender, EventArgs e) => {
				this.searchBar.EndEditing(true);
			};
			//this.Table.Source.RowSelected

			// Perform any additional setup after loading the view, typically from a nib.
		}

		public override void ViewWillAppear (bool animated)
		{
			this.View.EndEditing (true);
			this.Title = _title;
			base.ViewWillAppear (animated);
		}

		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);
			this.Title = "";
		}

		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);

			//this.Table.Frame = new RectangleF (0, 0, 320, 1000);

			//RectangleF contentRect = RectangleF.Empty;
			//foreach (UIView view in this.ScrollView.Subviews) {
			//	contentRect = RectangleF.Union (contentRect, view.Frame);
			//}

			//var width = this.View.Frame.Width;
			//var height = this.items.Count * this.Table.RowHeight;
			//this.Table.ContentSize = new SizeF (width, height);
		}

		protected void TableRowSelected(object sender, KeyValuePair<long, string> e)
		{
			KeyValueTableSource source = (KeyValueTableSource)sender;

			if (e.Key == 0) {
				KeyValuePair<long, string> e2 = new KeyValuePair<long, string> (0, null);
				if (ItemSelected != null)
					ItemSelected (this, e2);
			} else {
				if (ItemSelected != null)
					ItemSelected (this, e);
			}

			if (AutoPopController)
				this.NavigationController.PopViewControllerAnimated (true);
		}
	}
}

