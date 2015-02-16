using System;
using System.Drawing;
using System.Linq;
using System.Collections.Generic;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace MobileTech.iOS
{
	public partial class MultiSelectListScreen : UIViewController
	{
		private string _title;
		//private long? selectedItemKey;
		public bool AutoPopController { get; set; }
		public event EventHandler<List<KeyValuePair<long, string>>> selectedItemsHandler;

		List<TableItems> items; 
		List<KeyValuePair<long, string>> selectedItems = new List<KeyValuePair<long, string>>();
		bool showSectionHeading = false;
		bool searchFirstSectionAlso = false;

		static bool UserInterfaceIdiomIsPhone {
			get { return UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone; }
		}


		public MultiSelectListScreen (string title, List<TableItems> items, List<KeyValuePair<long, string>> selectedItems, bool showSectionHeading = false, bool searchFirstSectionAlso = false)
			: base (UserInterfaceIdiomIsPhone ? "MultiSelectListScreen_iPhone" : "MultiSelectListScreen_iPad", null)
		{
			this._title = title;
			this.items = items.ToList ();

//			if (showNone) {
//				this.items.Insert (0, new KeyValuePair<long, string> (0, "( None )"));
//			}

			if (showSectionHeading)
				this.showSectionHeading = showSectionHeading;

			if (searchFirstSectionAlso)
				this.searchFirstSectionAlso = searchFirstSectionAlso;

			//this.selectedItemKey = selectedItemKey;
			this.selectedItems = selectedItems;
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

			MultiSelectTableSource dataSource = new MultiSelectTableSource(items, selectedItems, showSectionHeading, searchFirstSectionAlso);
			dataSource.RowSelectedEvent += TableRowSelected;
			dataSource.RowDeSelectedEvent += TableRowDeSelected;

			this.Table.Source = dataSource;
			this.Table.AllowsMultipleSelection = true;

			//this.Table.TableHeaderView.BackgroundColor = UIColor.Red;

			this.searchBar.TextChanged += (object sender, UISearchBarTextChangedEventArgs e) => 
			{
				dataSource.Filter(searchBar.Text);
				this.Table.ReloadData();
			};

			this.searchBar.SearchButtonClicked+= (object sender, EventArgs e) => {
				this.searchBar.EndEditing(true);
			};
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

			if (selectedItems.Count > 0) {
				if (selectedItemsHandler != null)
					selectedItemsHandler (this, selectedItems);
			} else {
				//List<KeyValuePair<long, string>> e = new List<KeyValuePair<long, string>>{new KeyValuePair<long, string>(0, null)};
				List<KeyValuePair<long, string>> e = new List<KeyValuePair<long, string>>();
				if (selectedItemsHandler != null)
					selectedItemsHandler (this, e);
			}

		}

		public override void ViewDidDisappear (bool animated)
		{
			base.ViewDidDisappear (animated);
		}

		protected void TableRowSelected(object sender, KeyValuePair<long, string> e)
		{
			if(e.Key!=0)
				selectedItems.Add (e);
		}

		protected void TableRowDeSelected(object sender, KeyValuePair<long, string> e)
		{
			var item = selectedItems.Single (x => x.Key == e.Key);
			selectedItems.Remove(item);
		}
	}
}

