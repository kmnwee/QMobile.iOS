using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;
using SDWebImage;
using CoreAnimation;

namespace QMobile
{
	[Register ("FeaturedViewCell")]
	public class FeaturedViewCell : UITableViewCell
	{
		[Outlet]
		UILabel featuredMainTitle { get; set; }
		[Outlet]
		UILabel featuredDetails { get; set; }
		[Outlet]
		UIImageView featuredImageView { get; set; }

		public static readonly NSString Key = new NSString ("FeaturedViewCell");

		public FeaturedViewCell (IntPtr handle) : base (handle)
		{
		}

		public void setFeaturedMainTitle (String title)
		{
			this.featuredMainTitle.Text = title;
		}

		public void setFeaturedDetails (String title)
		{
			this.featuredDetails.Text = title;
		}

		public string getFeaturedDetails ()
		{
			return this.featuredDetails.Text;
		}
			
		public void setFeaturedImageView (String uri)
		{ 
			CALayer featuredImageViewLayer = featuredImageView.Layer;
			featuredImageViewLayer.CornerRadius = 8.77f;
			featuredImageViewLayer.MasksToBounds = true;

			this.featuredImageView.SetImage (
				url: new NSUrl (uri),
				placeholder: UIImage.FromBundle ("placeholder_store.jpg"),
				options: SDWebImageOptions.RefreshCached
			);
		}
	}
}
