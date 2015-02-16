
using System;
using System.Drawing;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace MobileTech.iOS
{
	public partial class CustomWOTableView : UITableViewCell
	{
		public static readonly NSString Key = new NSString ("CustomWOTableView");
		public static readonly UINib Nib;

		public UILabel Label1
		{
			get{return this.lbl1;}
			set{this.lbl1 = value;}
		}

		public UILabel Label2
		{
			get{return this.lbl2;}
			set{this.lbl2 = value;}
		}

		public UILabel Label3
		{
			get{return this.lbl3;}
			set{this.lbl3 = value;}
		}

		public UILabel Label4
		{
			get{return this.lbl4;}
			set{this.lbl4 = value;}
		}

		public UILabel Label5
		{
			get{return this.lbl5;}
			set{this.lbl5 = value;}
		}

		public UILabel Label6
		{
			get{return this.lbl6;}
			set{this.lbl6 = value;}
		}

		public UILabel Label7
		{
			get{return this.lbl7;}
			set{this.lbl7 = value;}
		}

		static CustomWOTableView ()
		{
			if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone)
				Nib = UINib.FromName ("CustomWOTableView_iPhone", NSBundle.MainBundle);
			else
				Nib = UINib.FromName ("CustomWOTableView_iPad", NSBundle.MainBundle);
		}

		public CustomWOTableView (IntPtr handle) : base (handle)
		{
		}

		public static CustomWOTableView Create ()
		{
			return (CustomWOTableView)Nib.Instantiate (null, null) [0];
		}

		public override bool Selected {

			get {
				return base.Selected;
			}
			set {
				base.Selected = value;
			}
		}
	}
}

