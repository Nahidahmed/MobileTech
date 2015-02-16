using System;
using System.Drawing;
using System.Collections.Generic;

using MonoTouch.UIKit;
using MonoTouch.Foundation;

using MobileTech.iOS.Screens;
using MobileTech.Models.Entity;

namespace MobileTech.iOS
{
	/// <summary>
	/// Combined DataSource and Delegate for our UITableView
	/// </summary>
	/// 


	public class TableItemGroup
	{
		public string Name { get; set; }

		public List<WorkOrderDetails> ListItems
		{
			get{return listItems;}
			set{listItems = value;}
		}

		protected List<WorkOrderDetails> listItems = new List<WorkOrderDetails>();
	}

	public class WOTableSource : UITableViewSource 
	{
		//---- declare vars
		//public List<TableItemGroup> tableItems;
		public List<TableItemGroup> tableItems;
		public string cellIdentifier = "TableCell";
		WkOrderListViewController woListController; 

		public WOTableSource (List<TableItemGroup> items)
		{
			tableItems = items;
		}

		public WOTableSource (List<TableItemGroup> items, WkOrderListViewController woListControllerObj)
		{
			tableItems = items;
			woListController = woListControllerObj;
		}


		#region data binding/display methods
		
		/// <summary>
		/// Called by the TableView to determine how many sections(groups) there are.
		/// </summary>
		public override int NumberOfSections (UITableView tableView)
		{
			return tableItems.Count;
		}

		/// <summary>
		/// Called by the TableView to determine how many cells to create for that particular section.
		/// </summary>
		public override int RowsInSection (UITableView tableview, int section)
		{
			return tableItems[section].ListItems.Count;
			//return tableItems.Count;
		}

		/// <summary>
		/// Called by the TableView to retrieve the header text for the particular section(group)
		/// </summary>
//		public override string TitleForHeader (UITableView tableView, int section)
//		{
//			return tableItems [section].Name.ToLower();
//		}


		public override UIView GetViewForHeader (UITableView tableView, int section)
		{
			UIView view = new UIView (new RectangleF (0,0, 300, 15));
							view.BackgroundColor = UIColor.Clear;


			UILabel label = new UILabel (new RectangleF (15,15,300,15));
			label.BackgroundColor = UIColor.Clear;
			label.ShadowOffset = new SizeF (0, 1);
			label.Font = UIFont.SystemFontOfSize (14);

			if (tableItems[section].Name != string.Empty) {
				if (tableItems[section].Name == "No network. Connect and sync manually.") {
					label.TextColor = UIColor.Red;
					label.Text = "No network. Connect and sync manually.";
				} else {
					label.Text = tableItems [section].Name;
				}
			}
			view.AddSubview (label);


			return view;
		}
		/// <summary>
		/// Called by the TableView to retrieve the footer text for the particular section(group)
		/// </summary>
//		public override string TitleForFooter (UITableView tableView, int section)
//		{
//			return tableItems[section].Footer;
//		}
		
		#endregion	
				
		#region user interaction methods


		
		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			WorkOrderDetails woDetails = new WorkOrderDetails ();
			woDetails = tableItems [indexPath.Section].ListItems [indexPath.Row];
			woListController.ShowWOEditor (woDetails, true);
			Consts.selectedWORow = woDetails.Number.Trim();
		}


		public override float GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
		{
			// In here you could customize how you want to get the height for row. Then   
			// just return it. 
			return 90;
		}

		public MobileTech.iOS.WOTableSource Filter(string text,List<TableItemGroup> tableItems,WkOrderListViewController woListControllerObj)
		{
			MobileTech.iOS.WOTableSource tableSource;

			List<TableItemGroup> filteredTableItems = new List<TableItemGroup> ();

			TableItemGroup tGroup;

			foreach (TableItemGroup item in tableItems) {
				foreach (WorkOrderDetails obj in item.ListItems) {
					if (obj.WODescription.ToLower().Contains(text.ToLower())
						|| obj.AssetDetails.Control.ToLower().Contains(text.ToLower())
						|| obj.Account.DepartmentName.ToLower().Contains(text.ToLower())
						|| obj.Request.Description.ToLower().Contains(text.ToLower())
						|| obj.AssetDetails.AssetCenter.Code.ToString().ToLower().Contains(text.ToLower())
						|| obj.AssetDetails.Model.DeviceCategory.DevCategory.ToLower ().Contains (text.ToLower())
						|| obj.OpenWorkOrderStatus.Description.ToLower().Contains (text.ToLower())
						|| obj.Urgency.Description.ToLower().Contains(text.ToLower())) {
						tGroup = new TableItemGroup () { };
						tGroup.ListItems.Add (obj);
						filteredTableItems.Add (tGroup);
					}
				}
			}


			tableSource = new MobileTech.iOS.WOTableSource(filteredTableItems,woListControllerObj);

			return tableSource;
		}


		public override void RowDeselected (UITableView tableView, NSIndexPath indexPath)
		{
			//Console.WriteLine ("Row " + indexPath.Row.ToString () + " deselected");	
		}

		// M.D.Prasad/19th Jan 2015/BA 27626/App is crashing in multiple scenerios.
//		public override void Scrolled (UIScrollView scrollView)
//		{
//
//			woListController.DissmisSearchKeyBoard ();
//		}
		#endregion	

		/// <summary>
		/// Called by the TableView to get the actual UITableViewCell to render for the particular section and row
		/// </summary>
		public override UITableViewCell GetCell (UITableView tableView, MonoTouch.Foundation.NSIndexPath indexPath)
		{
			var cell = tableView.DequeueReusableCell ("CustomWOTableView") as CustomWOTableView;

			if (cell == null) {
				cell = CustomWOTableView.Create ();

			}

			cell.UserInteractionEnabled = true;

			cell.Label1.Text = tableItems [indexPath.Section].ListItems [indexPath.Row].WODescription.Trim();
			cell.Label2.Text = tableItems [indexPath.Section].ListItems [indexPath.Row].Account.DepartmentName.Trim();


			cell.Label3.Text = tableItems [indexPath.Section].ListItems [indexPath.Row].Request.Description.Trim();

			if (tableItems [indexPath.Section].ListItems [indexPath.Row].AssetDetails.Control.Trim () != string.Empty) {
				cell.Label6.Hidden = false;
				cell.Label6.Text = tableItems [indexPath.Section].ListItems [indexPath.Row].AssetDetails.AssetCenter.Code.ToString().Trim() + "-"
					+ tableItems [indexPath.Section].ListItems [indexPath.Row].AssetDetails.Control.Trim();
			} else {
				cell.Label6.Hidden = true;
			}

			cell.Label7.Text = tableItems [indexPath.Section].ListItems [indexPath.Row].AssetDetails.Model.DeviceCategory.DevCategory.Trim();
		
			cell.Label4.Text = tableItems [indexPath.Section].ListItems [indexPath.Row].OpenWorkOrderStatus.Description.Trim();
			cell.Label5.Text = tableItems [indexPath.Section].ListItems [indexPath.Row].Urgency.Description.Trim();



			cell.Label1.Font =UIFont.FromName("Arial", 14f);
			cell.Label1.Font = UIFont.BoldSystemFontOfSize (14f);
			cell.Label2.Font =UIFont.FromName("Arial", 14f);
			cell.Label3.Font =UIFont.FromName("Arial", 14f);
			cell.Label4.Font =UIFont.FromName("Arial", 14f);
			cell.Label5.Font =UIFont.FromName("Arial", 14f);
			cell.Label6.Font =UIFont.FromName("Arial", 14f);
			cell.Label7.Font =UIFont.FromName("Arial", 14f);


			cell.Label2.TextColor = UIColor.LightGray;
			cell.Label3.TextColor = UIColor.LightGray;
			cell.Label4.TextColor = UIColor.LightGray;
			cell.Label5.TextColor = UIColor.LightGray;
			cell.Label6.TextColor = UIColor.LightGray;
			cell.Label7.TextColor = UIColor.LightGray;

			if (tableItems [indexPath.Section].ListItems[indexPath.Row].Number.Trim() == Consts.selectedWORow ) {
				cell.BackgroundColor = UIColor.FromRGB(230,230,231);
			} else {
				cell.BackgroundColor = UIColor.LightTextColor;
			}


			return cell;

		}

	}
}

