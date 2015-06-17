using System;
using UIKit;
using Foundation;
using System.Collections.Generic;
using CoreAnimation;
using System.Drawing;
using Microsoft.WindowsAzure.MobileServices;

namespace QMobile
{
	public class FeaturedTableSource : UITableViewSource
	{
		TFMerchants[] tableItems;
		string cellId = "TableCell";
		private UIViewController viewControllerLocal;

	
		public FeaturedTableSource (TFMerchants[] items, UIViewController viewController)
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
			CompanyViewController companyView = viewControllerLocal.Storyboard.InstantiateViewController ("CompanyViewController") as CompanyViewController;
			companyView.companyId = tableItems [indexPath.Row].COMPANY_NO;
			companyView.companyName = tableItems [indexPath.Row].COMPANY_NAME;
			InvokeOnMainThread (() => viewControllerLocal.NavigationController.PushViewController (companyView, true));
		}

		public override UITableViewCell GetCell (UITableView tableView, Foundation.NSIndexPath indexPath)
		{
			UITableViewCell cell = tableView.DequeueReusableCell (cellId);
			if (cell == null) {
				cell = new UITableViewCell (UITableViewCellStyle.Subtitle, cellId);
			}
			cell.TextLabel.Text = tableItems [indexPath.Row].COMPANY_NAME;

			InvokeOnMainThread (async () => {
				TFBusinessTypes bt = new TFBusinessTypes ();
				long visitCountRes = 0;
				long visitCountAppt = 0;
				try {
					bt = await AppDelegate.MobileService.GetTable<TFBusinessTypes> ().LookupAsync (tableItems [indexPath.Row].businesstype_id);
					visitCountRes = (await AppDelegate.MobileService.GetTable<TFOLReservation> ().Take (0).IncludeTotalCount ()
					.Where (TFOLReservation => TFOLReservation.company_id == tableItems [indexPath.Row].COMPANY_NO)
					.ToListAsync () as ITotalCountProvider).TotalCount;
					visitCountAppt = (await AppDelegate.MobileService.GetTable<TFScheduledReservation> ().Take (0).IncludeTotalCount ()
					.Where (TFScheduledReservation => TFScheduledReservation.company_id == tableItems [indexPath.Row].COMPANY_NO)
					.ToListAsync () as ITotalCountProvider).TotalCount;
					if (bt != null) {
						cell.DetailTextLabel.Text = bt.businesstype_name;
					} else {
						cell.DetailTextLabel.Text = " ";
					}
					cell.DetailTextLabel.Text += String.Format (" | {0} Mobile Visitors", visitCountRes + visitCountAppt);

				} catch (Exception e) {
					Console.WriteLine ("Problem loading Mobile Visitors Count...");
					Console.WriteLine (e.Message);
					Console.WriteLine (e.StackTrace);
				}
			});



			InvokeOnMainThread (async () => {
				try {
					//Console.WriteLine("Loading Image " + tableItems [indexPath.Row].icon_image.Replace("merchantlogos", "merchantlogos-ios"));
					//cell.ImageView.Image = FromURL(tableItems [indexPath.Row].icon_image.Replace("merchantlogos", "merchantlogos-ios"));
					cell.ImageView.Image = MaxResizeImage (FromURL (tableItems [indexPath.Row].icon_image), 50, 50);
				} catch (Exception ex) {
					Console.Out.WriteLine (ex.Message);
					Console.Out.WriteLine ("Cannot Load Image");
				}
			});
			
			Console.Out.WriteLine (tableItems [indexPath.Row].COMPANY_NAME + ": " + tableItems [indexPath.Row].icon_image);



			return cell;
		}

		public static UIImage MaxResizeImage (UIImage sourceImage, float maxWidth, float maxHeight)
		{
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

			UIGraphics.BeginImageContext (new SizeF (widthF, heightF));
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

	}
}

