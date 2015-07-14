using System;
using UIKit;
using Foundation;
using Xamarin.Forms;
using ImageCircle.Forms.Plugin.iOS;
using System.Drawing;
using Microsoft.WindowsAzure.MobileServices;

namespace QMobile
{
	public class FavoritesTableSource : UITableViewSource
	{
		TFMemberFavoritesEx[] tableItems;
		string cellId = "TableCell";
		private UIViewController viewControllerLocal;


		public FavoritesTableSource (TFMemberFavoritesEx[] items, UIViewController viewController)
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
		//			return "Bookmarked Stores";
		//		}

		public override void RowSelected (UITableView tableView, Foundation.NSIndexPath indexPath)
		{
			//new UIAlertView("Alert", tableItems[indexPath.Row].company_id + " | " + tableItems[indexPath.Row].branch_id, null, "Got It!", null).Show ();
			tableView.DeselectRow (indexPath, true);

			BranchViewController branchView = viewControllerLocal.Storyboard.InstantiateViewController ("BranchViewController") as BranchViewController;
			branchView.merchant = tableItems [indexPath.Row].merchant;

			InvokeOnMainThread (() => {
				viewControllerLocal.NavigationController.PushViewController (branchView, true);
			});
		}

		public override UITableViewCell GetCell (UITableView tableView, Foundation.NSIndexPath indexPath)
		{
			UITableViewCell cell = tableView.DequeueReusableCell (cellId);
			if (cell == null)
				cell = new UITableViewCell (UITableViewCellStyle.Subtitle, cellId);
			
			cell.TextLabel.Text = Convert.ToString (tableItems [indexPath.Row].branch_name);
			cell.DetailTextLabel.Text = Convert.ToString (tableItems [indexPath.Row].company_name);
			InvokeOnMainThread (async () => {
				long ticketsRes = 0;
				long ticketsAppt = 0;
				try {
					ticketsRes = (await AppDelegate.MobileService.GetTable<TFOLReservation> ().Take (0).IncludeTotalCount ()
					.Where (TFOLReservation => TFOLReservation.mobile_userid == tableItems [indexPath.Row].email
					&& TFOLReservation.company_id == tableItems [indexPath.Row].company_id
					&& TFOLReservation.branch_id == tableItems [indexPath.Row].branch_id)
					.ToListAsync () as ITotalCountProvider).TotalCount;
					ticketsAppt = (await AppDelegate.MobileService.GetTable<TFScheduledReservation> ().Take (0).IncludeTotalCount ()
					.Where (TFScheduledReservation => TFScheduledReservation.email == tableItems [indexPath.Row].email
					&& TFScheduledReservation.company_id == tableItems [indexPath.Row].company_id
					&& TFScheduledReservation.branch_id == tableItems [indexPath.Row].branch_id)
					.ToListAsync () as ITotalCountProvider).TotalCount;
					cell.DetailTextLabel.Text += String.Format (" | You had {0} ticket/s here", ticketsRes + ticketsAppt);
				} catch (Exception e) {
					Console.WriteLine ("Problem loading Tickets Count...");
					Console.WriteLine (e.Message);
					Console.WriteLine (e.StackTrace);
				}
			});
			return cell;
		}

		public override nfloat GetHeightForRow (UITableView tableview, Foundation.NSIndexPath indexPath)
		{
			return 60;
		}

		static UIImage FromURL (string uri)
		{
			using (var url = new NSUrl (uri))
			using (var data = NSData.FromUrl (url))
				return UIImage.LoadFromData (data);
		}

		static UIImage RounderCorners (UIImage image, float width, float radius)
		{
			UIGraphics.BeginImageContext (new SizeF (width, width));
			var c = UIGraphics.GetCurrentContext ();

			c.BeginPath ();
			c.MoveTo (width, width / 2);
			c.AddArcToPoint (width, width, width / 2, width, radius);
			c.AddArcToPoint (0, width, 0, width / 2, radius);
			c.AddArcToPoint (0, 0, width / 2, 0, radius);
			c.AddArcToPoint (width, 0, width, width / 2, radius);
			c.ClosePath ();
			c.Clip ();

			image.Draw (new PointF (0, 0));
			var converted = UIGraphics.GetImageFromCurrentImageContext ();
			UIGraphics.EndImageContext ();
			return converted;
		}

	}
}

