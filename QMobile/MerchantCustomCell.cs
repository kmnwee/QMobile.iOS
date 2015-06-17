
using System;
using System.Drawing;

using Foundation;
using UIKit;

namespace QMobile
{
	public partial class MerchantCustomCell : UITableViewCell
	{
		public static readonly NSString Key = new NSString ("MerchantCustomCell");
		public static readonly UINib Nib;
		public TFMerchants merchant { get; set;}

		static MerchantCustomCell ()
		{
			if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone)
				Nib = UINib.FromName ("MerchantCustomCell_iPhone", NSBundle.MainBundle);
			else
				Nib = UINib.FromName ("MerchantCustomCell_iPad", NSBundle.MainBundle);
		}

		public MerchantCustomCell (IntPtr handle) : base (handle)
		{
		}

		public static MerchantCustomCell Create ()
		{
			return (MerchantCustomCell)Nib.Instantiate (null, null) [0];
		}

		public override void LayoutSubviews ()
		{
			base.LayoutSubviews ();

			this.companyName.Text = merchant.COMPANY_NAME;
			this.branchName.Text = merchant.BRANCH_NAME;
			this.info.Text = merchant.address;
		}
	}
}

