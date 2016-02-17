using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;
using SDWebImage;
using CoreAnimation;
using CoreGraphics;

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
		UILabel featuredDetails2 { get; set; }

		[Outlet]
		UILabel featuredDetails3 { get; set; }

		[Outlet]
		UIImageView featuredImageView { get; set; }

		[Outlet]
		UIView containerView { get; set; }

		public static readonly NSString Key = new NSString ("FeaturedViewCell");

		public FeaturedViewCell (IntPtr handle) : base (handle)
		{
		}

		public override void SetHighlighted (bool highlighted, bool animated)
		{
			//base.SetHighlighted (highlighted, animated);
			if (highlighted) {
				this.containerView.BackgroundColor = TFColor.FromHexString ("#d9d9d9", 1.0f);
			} else {
				this.containerView.BackgroundColor = TFColor.FromHexString ("#F2F2F2", 1.0f);
			}
		}

		public void setFeaturedMainTitle (String title)
		{
			this.featuredMainTitle.Text = title;
		}

		public void setFeaturedDetails (String title)
		{
			this.featuredDetails.TextColor = UIColor.Gray;
			this.featuredDetails.Text = title;
		}

		public void setFeaturedDetails2 (String title)
		{
			this.featuredDetails2.TextColor = UIColor.Gray;
			this.featuredDetails2.Text = title;
		}

		public void setFeaturedDetails3 (String title)
		{
			this.featuredDetails3.TextColor = UIColor.Gray;
			this.featuredDetails3.Text = title;
		}

		public void setContainerView ()
		{
			//this.containerView.Frame.Width = UIScreen.MainScreen.Bounds.Width - nfloat.Parse ("16");
//			this.containerView.UserInteractionEnabled = true;
//			UITapGestureRecognizer tapGesture = new UITapGestureRecognizer (TapCell);
//			this.containerView.AddGestureRecognizer (tapGesture);

			this.SelectionStyle = UITableViewCellSelectionStyle.None;
			//this.SelectedBackgroundView = this.cont
			this.containerView.Layer.ShadowColor = UIColor.DarkGray.CGColor;
			this.containerView.Layer.CornerRadius = 1.0f;
			this.containerView.Layer.ShadowOpacity = 1.0f;
			this.containerView.Layer.ShadowRadius = 1.0f;
			this.containerView.Layer.ShadowOffset = new System.Drawing.SizeF(0f, 1f);
			this.containerView.Layer.ShouldRasterize = true;
			this.containerView.Layer.MasksToBounds = false;
		}

		void TapCell(UITapGestureRecognizer tap)
		{
			Console.Out.WriteLine ("Tap that!");
			this.containerView.BackgroundColor = UIColor.LightGray;
		}
			
		public string getFeaturedDetails ()
		{
			return this.featuredDetails.Text;
		}

		public void setFeaturedImageView (String uri)
		{ 
			var maskPath = UIBezierPath.FromRoundedRect(this.featuredImageView.Bounds, UIRectCorner.BottomLeft | UIRectCorner.TopLeft,
				new CGSize(1.0, 1.0));
			var maskLayer = new CAShapeLayer
			{
				Frame = this.featuredImageView.Bounds,
				Path = maskPath.CGPath
			};
			this.featuredImageView.Layer.Mask = maskLayer;

			//CALayer featuredImageViewLayer = featuredImageView.Layer;
			//featuredImageViewLayer.CornerRadius = 8.77f;
			//featuredImageViewLayer.MasksToBounds = true;

			this.featuredImageView.SetImage (
				url: new NSUrl (uri),
				placeholder: UIImage.FromBundle ("placeholder_store.jpg"),
				options: SDWebImageOptions.RefreshCached
			);
		}
	}
}
