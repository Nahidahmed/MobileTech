using System;
using System.Linq;
using MonoTouch.UIKit;
using System.Collections.Generic;
using System.IO;
//using MonoTouch.UIKit;
using System.Runtime.Serialization.Formatters.Binary;
using System.Drawing;

namespace MobileTech.iOS
{
	[Serializable]
	public class TableItems
	{
		public KeyValuePair<long, string> Group { get; set; }

		public List<KeyValuePair<long, string>> ListItems
		{
			get{return listItems;}
			set{listItems = value;}
		}

		protected List<KeyValuePair<long, string>> listItems = new List<KeyValuePair<long, string>>();

		/*
		public TableItems ShallowCopy()
		{
			return (TableItems) this.MemberwiseClone();
		}
		*/

		public static List<TableItems> DeepCopy(List<TableItems> source)
		{
			using (MemoryStream ms = new MemoryStream())
			{
				BinaryFormatter formatter = new BinaryFormatter();
				formatter.Serialize(ms, source);
				ms.Position = 0;
				return (List<TableItems>)formatter.Deserialize(ms);
			}
		}
	}

	public class MultiSelectTableSource: UITableViewSource
	{
		List<TableItems> originalTableItems;
		List<TableItems> tableItems;
		//long? selectedItemKey;
		string cellIdentifier = "TableCell";
		public string FilterText { get; private set; }

		bool showSectionHeading = false;
		bool searchFirstSectionAlso = false;

		List<KeyValuePair<long, string>> selectedItems;// = new List<KeyValuePair<long, string>>();

		public event EventHandler<KeyValuePair<long, string>> RowSelectedEvent;
		public event EventHandler<KeyValuePair<long, string>> RowDeSelectedEvent;

		public MultiSelectTableSource (List<TableItems> items, List<KeyValuePair<long, string>> selectedItems, bool showSectionHeading, bool searchFirstSectionAlso)
		{
			this.originalTableItems = items;
			this.tableItems = TableItems.DeepCopy (originalTableItems);

			//this.selectedItemKey = selectedItemKey;
			this.selectedItems = selectedItems;
			this.showSectionHeading = showSectionHeading;
			this.searchFirstSectionAlso = searchFirstSectionAlso;
		}

		/// <summary>
		/// Called by the TableView to determine how many sections(groups) there are.
		/// </summary>
		public override int NumberOfSections (UITableView tableView)
		{
			return tableItems.Count;
		}

		/*
		/// <summary>
		/// Called by the TableView to retrieve the header text for the particular section(group)
		/// </summary>
		public override string TitleForHeader (UITableView tableView, int section)
		{
			if (showSectionHeading) {
				tableView.SectionHeaderHeight = 40f;
				tableView.SectionIndexColor = UIColor.Red;
				tableView.SectionIndexBackgroundColor = UIColor.Blue;


				//tableView.EstimatedSectionFooterHeight = 40f;
				return tableItems [section].Group.Value;

			}
			else
				return string.Empty;
		}
		*/

		public override float GetHeightForHeader (UITableView tableView, int section)
		{
			if (showSectionHeading)
				return 40f;
			else
				return 0f;
		}


	
		public override UIView GetViewForHeader (UITableView tableView, int section)
		{
			UIView view;
			if (showSectionHeading) {
				view = new UIView (new RectangleF (0, 0, UIScreen.MainScreen.Bounds.Width, tableView.SectionHeaderHeight));
				view.BackgroundColor = UIColor.DarkGray;
				//UIScreen.MainScreen.Bounds.Width, UIScreen.MainScreen.Bounds.Height

				UILabel label = new UILabel (new RectangleF (5, 10, UIScreen.MainScreen.Bounds.Width, tableView.SectionHeaderHeight));
				label.BackgroundColor = UIColor.Clear;
				label.ShadowOffset = new SizeF (0, 1);
				//label.Font = UIFont.SystemFontOfSize (14);
				label.TextColor = UIColor.White;


				label.Text = tableItems [section].Group.Value;
				
				view.AddSubview (label);
			} 
			else {
				view = new UIView (new RectangleF (0, 0, 0, 0));
			}

			return view;
		}


		public override int RowsInSection (UITableView tableview, int section)
		{
			return tableItems[section].ListItems.Count;
		}

		public override UITableViewCell GetCell (UITableView tableView, MonoTouch.Foundation.NSIndexPath indexPath)
		{
			UITableViewCell cell = tableView.DequeueReusableCell (cellIdentifier);
			// if there are no cells to reuse, create a new one
			if (cell == null)
				cell = new UITableViewCell (UITableViewCellStyle.Default, cellIdentifier);

			cell.TextLabel.Text = tableItems [indexPath.Section].ListItems [indexPath.Row].Value;

			//cell.TextLabel.Text = tableItems [indexPath.Row].Value;
			cell.SelectionStyle = UITableViewCellSelectionStyle.None;
			cell.Accessory = UITableViewCellAccessory.None;
			cell.TintColor = UIColor.DarkGray;

			if (selectedItems!=null) {
				if (selectedItems.Exists (x => x.Key == tableItems [indexPath.Section].ListItems [indexPath.Row].Key)) {
					cell.TextLabel.TextColor = UIColor.DarkGray;
					cell.Accessory = UITableViewCellAccessory.Checkmark;
					tableView.SelectRow (indexPath, false, UITableViewScrollPosition.None);
				} else {
					cell.TextLabel.TextColor = UIColor.LightGray;
				}
			}

			return cell;
		}


		public override void RowSelected (UITableView tableView, MonoTouch.Foundation.NSIndexPath indexPath)
		{
			if (RowSelectedEvent != null)
				RowSelectedEvent (this, tableItems [indexPath.Section].ListItems [indexPath.Row]);

			tableView.CellAt (indexPath).Accessory = UITableViewCellAccessory.Checkmark;
			tableView.CellAt (indexPath).TextLabel.TextColor = UIColor.DarkGray;
		}

		public override void RowDeselected (UITableView tableView, MonoTouch.Foundation.NSIndexPath indexPath)
		{
			if (RowDeSelectedEvent != null)
				RowDeSelectedEvent (this, tableItems [indexPath.Section].ListItems [indexPath.Row]);

			tableView.CellAt (indexPath).Accessory = UITableViewCellAccessory.None;
			tableView.CellAt (indexPath).TextLabel.TextColor = UIColor.LightGray;
		}

		public void Filter(string text)
		{
			//FilterText = text;
			int initialVal = 0;

			if (!searchFirstSectionAlso)
				initialVal = 1;

			for (int cur = initialVal; cur < originalTableItems.Count; cur++) {

				TableItems groupItem = originalTableItems [cur];

				var items = groupItem.ListItems.Where (it => it.Value.ToLower ().Contains (text.ToLower ()) || selectedItems.Any (x => x.Value == it.Value)).ToList ();

				if (items.Count > 0) {
					for (int i = initialVal; i < tableItems.Count; i++) {
						if (tableItems [i].Group.Key == groupItem.Group.Key) {
							tableItems [i].ListItems.Clear ();
							tableItems [i].ListItems = items;
						}
					}
				}
			}
		}
	}
}

