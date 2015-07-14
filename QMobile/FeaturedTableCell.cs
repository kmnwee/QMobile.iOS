using System;
using UIKit;
using System.Drawing;
using SDWebImage;
using Foundation;

namespace QMobile
{
	partial class FeaturedTableCell : UITableViewCell
	{
		public FeaturedTableCell () : base ()
		{
		}

		public FeaturedTableCell (IntPtr handle) : base (handle)
		{
			SelectionStyle = UITableViewCellSelectionStyle.Gray;
		}

		public void setCompanyName(string companyName)
		{
			this.TextLabel.Text = companyName;
		}

		public void setBranchName(string branchName)
		{
			this.DetailTextLabel.Text = branchName;
		}

		public void setCompanyLogo(string url)
		{
			this.ImageView.SetImage (
				url: new NSUrl (url),
				placeholder: UIImage.FromBundle ("placeholder_store.jpg"),
				options: SDWebImageOptions.RefreshCached
				//						completionHandler: (image, error, cacheType, obj) => {
				//							if (image != null) {
				//								image.Scale(new SizeF(50, 50), 1.0f);
				//							}
				//						}
			);
		}

		public override void LayoutSubviews ()
		{
			base.LayoutSubviews ();
			this.ImageView.Bounds = new RectangleF (0, 0, 50, 50);
			this.ImageView.Frame = new RectangleF (0, 0, 50, 50);
			this.ImageView.ContentMode = UIViewContentMode.ScaleAspectFit;
		}
	}
}

