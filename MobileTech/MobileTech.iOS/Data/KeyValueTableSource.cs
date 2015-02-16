using System;
using System.Linq;
using MonoTouch.UIKit;
using System.Collections.Generic;

namespace MobileTech.iOS
{
	public class KeyValueTableSource : UITableViewSource {

		List<KeyValuePair<long, string>> originalTableItems;
		List<KeyValuePair<long, string>> tableItems;
		long? selectedItemKey;
		string cellIdentifier = "TableCell";

		public string FilterText { get; private set; }

		public event EventHandler<KeyValuePair<long, string>> RowSelectedEvent;

		public KeyValueTableSource (List<KeyValuePair<long, string>> items, long? selectedItemKey)
		{
			this.originalTableItems = items;
			this.tableItems = originalTableItems.ToList ();
			this.selectedItemKey = selectedItemKey;
		}

		public override int RowsInSection (UITableView tableview, int section)
		{
			return tableItems.Count;
		}

		public override UITableViewCell GetCell (UITableView tableView, MonoTouch.Foundation.NSIndexPath indexPath)
		{
			UITableViewCell cell = tableView.DequeueReusableCell (cellIdentifier);
			// if there are no cells to reuse, create a new one
			if (cell == null)
				cell = new UITableViewCell (UITableViewCellStyle.Default, cellIdentifier);
			cell.TextLabel.Text = tableItems [indexPath.Row].Value;

			if (selectedItemKey != null) {
				if (tableItems [indexPath.Row].Key == selectedItemKey) {
					cell.TextLabel.TextColor = UIColor.LightGray;
				} else {
					cell.TextLabel.TextColor = UIColor.Black;
				}
			}
			return cell;
		}

		public override void RowSelected (UITableView tableView, MonoTouch.Foundation.NSIndexPath indexPath)
		{
			if (RowSelectedEvent != null)
				RowSelectedEvent (this, tableItems[indexPath.Row]);

			tableView.DeselectRow (indexPath, true);

			// NOTE: Don't call the base implementation on a Model class
			// see http://docs.xamarin.com/guides/ios/application_fundamentals/delegates,_protocols,_and_events
			//throw new NotImplementedException ();
		}

		public void Filter(string text)
		{
			FilterText = text;
			tableItems = originalTableItems.Where (it => it.Value.ToLower ().Contains (text.ToLower())).ToList ();
		}
	}
}

