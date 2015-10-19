using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;
using SDWebImage;

namespace QMobile
{
	[Register ("TFCustomCell")]
	public class TFCustomCell : UITableViewCell
	{
		[Outlet]
		UILabel mainLabel { get; set; }
		[Outlet]
		UILabel subLabelA { get; set; }
		[Outlet]
		UILabel subLabelB { get; set; }
		[Outlet]
		UIImageView tableImageView { get; set; }

		public static readonly NSString Key = new NSString ("TFCustomCell");

		public TFCustomCell (IntPtr handle) : base (handle)
		{
		}

		public void setMainLabel (String title)
		{
			this.mainLabel.Text = title;
		}

		public void setSubLabelA (String title)
		{
			this.subLabelA.Text = title;
		}

		public void setSubLabelB (String title)
		{
			this.subLabelB.Text = title;
		}

		public void setTableImageView (String uri)
		{ 
			this.tableImageView.SetImage (
				url: new NSUrl (uri),
				placeholder: UIImage.FromBundle ("placeholder_store.jpg"),
				options: SDWebImageOptions.TransformAnimatedImage
			);
		}
	}
}
