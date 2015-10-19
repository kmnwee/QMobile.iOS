using System;
using UIKit;
using CoreAnimation;
using SDWebImage;
using Foundation;
using System.Drawing;
using System.Net.Http;
using System.Threading.Tasks;

namespace QMobile
{
	public class QTableSource: UITableViewSource
	{
		TFMerchants[] tableItems;
		string cellId = "TableCell";
		private UIViewController viewControllerLocal;


		public QTableSource (TFMerchants[] items, UIViewController viewController)
		{
			tableItems = items;
			viewControllerLocal = viewController;
		}

		public override nint RowsInSection (UITableView tableview, nint section)
		{
			return tableItems.Length;
		}

		//		public override string TitleForHeader(UITableView tableView, nint section)
		//		{
		//			return "Featured Companies";
		//		}

		public override void RowSelected (UITableView tableView, Foundation.NSIndexPath indexPath)
		{
			//new UIAlertView("Alert", tableItems[indexPath.Row].COMPANY_NAME + " | " + tableItems[indexPath.Row].COUNTRY, null, "Got It!", null).Show ();
			tableView.DeselectRow (indexPath, true);
			//			CompanyViewController companyView = viewControllerLocal.Storyboard.InstantiateViewController ("CompanyViewController") as CompanyViewController;
			//			companyView.companyId = tableItems [indexPath.Row].COMPANY_NO;
			//			companyView.companyName = tableItems [indexPath.Row].COMPANY_NAME;
			//			InvokeOnMainThread (() => viewControllerLocal.NavigationController.PushViewController (companyView, true));
			Console.WriteLine ("Row Selected");
			//test Table
//			QMobileViewController qview = viewControllerLocal.Storyboard.InstantiateViewController ("QMobileViewController") as QMobileViewController;
//			qview.tableItems = tableItems;
//			InvokeOnMainThread (() => viewControllerLocal.NavigationController.PushViewController (qview, true));
		}

		public override UITableViewCell GetCell (UITableView tableView, Foundation.NSIndexPath indexPath)
		{
			//UITableViewCell cell = tableView.DequeueReusableCell (cellId);
			//FeaturedTableCell cell = tableView.DequeueReusableCell (cellId) as FeaturedTableCell;
//			TFCustomCell cell = tableView.DequeueReusableCell (cellId) as TFCustomCell;
//			if (cell == null) {
//				cell = new TFCustomCell (cellId);// as FeaturedTableCell;
//			}
			Console.WriteLine ("key : " + TFCustomCell.Key);
			TFCustomCell cell = (TFCustomCell)tableView.DequeueReusableCell (TFCustomCell.Key);
//			if (cell == null) {
//				cell = new TFCustomCell (cellId);// as FeaturedTableCell;
//			}
			//cell.
			//cell.TextLabel.Text = tableItems [indexPath.Row].COMPANY_NAME;
			//cell.setCompanyName (tableItems [indexPath.Row].COMPANY_NAME);
			//cell.setCompanyLogo (tableItems [indexPath.Row].icon_image);

			cell.setMainLabel (tableItems [indexPath.Row].BRANCH_NAME);
			cell.setSubLabelA (tableItems [indexPath.Row].COMPANY_NAME);
			cell.setSubLabelB (tableItems [indexPath.Row].contact_no);
			cell.setTableImageView (tableItems [indexPath.Row].icon_image);

			Console.Out.WriteLine (tableItems [indexPath.Row].COMPANY_NAME + ": " + tableItems [indexPath.Row].icon_image);

			return cell;
		}

		public static UIImage MaxResizeImage (UIImage sourceImage, float maxWidth, float maxHeight)
		{
			Console.WriteLine ("MaxResizeImage was called");

			var sourceSize = sourceImage.Size;
			var maxResizeFactor = Math.Min (maxWidth / sourceSize.Width, maxHeight / sourceSize.Height);
			if (maxResizeFactor > 1)
				return sourceImage;
			var width = maxResizeFactor * sourceSize.Width;
			var height = maxResizeFactor * sourceSize.Height;

			float widthF = (float)width;
			if (float.IsPositiveInfinity (widthF)) {
				widthF = float.MaxValue;
			} else if (float.IsNegativeInfinity (widthF)) {
				widthF = float.MinValue;
			}

			float heightF = (float)height;
			if (float.IsPositiveInfinity (heightF)) {
				heightF = float.MaxValue;
			} else if (float.IsNegativeInfinity (heightF)) {
				heightF = float.MinValue;
			}

			UIGraphics.BeginImageContextWithOptions (new SizeF (widthF, heightF), false, UIScreen.MainScreen.Scale);
			//UIGraphics.GetCurrentContext ().InterpolationQuality = CGInterpolationQuality.None;
			//			UIGraphics.GetCurrentContext ().TranslateCTM (0, maxHeight);
			//			UIGraphics.GetCurrentContext ().ScaleCTM (1f, -1f);
			sourceImage.Draw (new RectangleF (0, 0, widthF, heightF));
			var resultImage = UIGraphics.GetImageFromCurrentImageContext ();
			UIGraphics.EndImageContext ();
			return resultImage;
		}

		static UIImage FromURL (string uri)
		{
			using (var url = new NSUrl (uri))
			using (var data = NSData.FromUrl (url))
				return UIImage.LoadFromData (data);
		}

		public async Task<UIImage> LoadImage (string imageUrl)
		{
			var httpClient = new HttpClient ();

			Task<byte[]> contentsTask = httpClient.GetByteArrayAsync (imageUrl);

			// await! control returns to the caller and the task continues to run on another thread
			var contents = await contentsTask;

			// load from bytes
			return MaxResizeImage (UIImage.LoadFromData (NSData.FromArray (contents)), 50, 50);
		}


		//		public static UIImage resizeImage(UIImage source, CGSize newSize)
		//		{
		//			CGContext context = UIGraphics.GetCurrentContext ();
		//			context.InterpolationQuality = CGInterpolationQuality.None;
		//
		//
		//			context.TranslateCTM (0, newSize.Height);
		//			context.ScaleCTM (1f, -1f);
		//
		//			context.DrawImage (new RectangleF (0, 0, newSize.Width, newSize.Height), source.CGImage);
		//
		//			var scaledImage = UIGraphics.GetImageFromCurrentImageContext();
		//			UIGraphics.EndImageContext();
		//
		//			return scaledImage;
		//		}

		//		- (UIImage*)imageWithImage:(UIImage*)image
		//		scaledToSize:(CGSize)newSize;
		//		{
		//			UIGraphicsBeginImageContext( newSize );
		//			[image drawInRect:CGRectMake(0,0,newSize.width,newSize.height)];
		//			UIImage* newImage = UIGraphicsGetImageFromCurrentImageContext();
		//			UIGraphicsEndImageContext();
		//
		//			return newImage;
		//		}

	}
}