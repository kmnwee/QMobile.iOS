using System;
using UIKit;
using Foundation;
using System.Collections.Generic;
using CoreAnimation;
using System.Drawing;
using Microsoft.WindowsAzure.MobileServices;
using System.Threading.Tasks;
using System.Net.Http;
using CoreGraphics;
using SDWebImage;


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
			//FeaturedTableCell cell = tableView.DequeueReusableCell (cellId) as FeaturedTableCell;
			if (cell == null) {
				cell = new UITableViewCell (UITableViewCellStyle.Subtitle, cellId);// as FeaturedTableCell;
			}

			cell.TextLabel.Text = tableItems [indexPath.Row].COMPANY_NAME;
			//cell.setCompanyName (tableItems [indexPath.Row].COMPANY_NAME);
			//cell.setCompanyLogo (tableItems [indexPath.Row].icon_image);

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
						//cell.setBranchName (bt.businesstype_name);
					} else {
						cell.DetailTextLabel.Text = " ";
						//cell.setBranchName (" ");
					}
					cell.DetailTextLabel.Text += String.Format (" | {0} Mobile Visitors", visitCountRes + visitCountAppt);
					//cell.setBranchName (String.Format ("{0} | {1} Mobile Visitors", bt.businesstype_name, visitCountRes + visitCountAppt));

				} catch (Exception e) {
					Console.WriteLine ("Problem loading Mobile Visitors Count...");
					Console.WriteLine (e.Message);
					Console.WriteLine (e.StackTrace);
				}
			});



			InvokeOnMainThread (async () => {
				try {
					Console.WriteLine ("Loading Image " + tableItems [indexPath.Row].icon_image.Replace ("merchantlogos", "merchantlogos-ios"));

//					cell.ImageView.ContentMode = UIViewContentMode.ScaleAspectFit;

					cell.ImageView.SetImage (
						url: new NSUrl (tableItems [indexPath.Row].icon_image),
						placeholder: UIImage.FromBundle ("placeholder_store.jpg"),
						//options: SDWebImageOptions.RefreshCached,
						completionHandler: (image, error, cacheType, url) => {
							Console.WriteLine("Handler was called");
							if (image != null) {
								//image.Scale(new SizeF(50, 50), 0.0f);
								Console.WriteLine("Image not null");
								image = MaxResizeImage(image, 50, 50);
								cell.ImageView.ContentMode = UIViewContentMode.ScaleAspectFit;
								cell.ImageView.Image = image;
							}
						}
					);

					//cell.ImageView.Image = FromURL(tableItems [indexPath.Row].icon_image.Replace("merchantlogos", "merchantlogos-ios"));
					//cell.ImageView.Image = await LoadImage (tableItems [indexPath.Row].icon_image.Replace ("merchantlogos", "merchantlogos-ios"));
					//cell.ImageView.SetImage(await LoadImage (tableItems [indexPath.Row].icon_image));
					//cell.ImageView.Image = await LoadImage (tableItems [indexPath.Row].icon_image);
					//cell.ImageView.Image = MaxResizeImage (FromURL (tableItems [indexPath.Row].icon_image), 50, 50);
				} catch (Exception ex) {
					//cell.ImageView.Image = MaxResizeImage (UIImage.FromBundle ("placeholder_store.jpg"), 50, 50);
					Console.Out.WriteLine (ex.Message);
					Console.Out.WriteLine ("Cannot Load Image");
				}
			});


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
			return MaxResizeImage(UIImage.LoadFromData (NSData.FromArray (contents)), 50, 50);
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

