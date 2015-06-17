using System;
using UIKit;
using Foundation;

namespace QMobile
{
	public class CompanyTableSource : UITableViewSource
	{
		TFMerchants[] tableItems;
		string cellId = "TableCell";
		private UIViewController viewControllerLocal;

		public CompanyTableSource (TFMerchants[] items, UIViewController viewController)
		{
			tableItems = items;
			viewControllerLocal = viewController;
		}

		public override nint RowsInSection (UITableView tableview, nint section)
		{
			return tableItems.Length;
		}

		public override void RowSelected (UITableView tableView, Foundation.NSIndexPath indexPath)
		{
			//new UIAlertView("Alert", tableItems[indexPath.Row].company_id + " | " + tableItems[indexPath.Row].branch_id, null, "Got It!", null).Show ();
			tableView.DeselectRow (indexPath, true);
			if (!tableItems [indexPath.Row].edition.Equals ("DEMO") && !tableItems [indexPath.Row].edition.Equals ("DESKTOP")) {
				BranchViewController branchView = viewControllerLocal.Storyboard.InstantiateViewController ("BranchViewController") as BranchViewController;
				//			//searchResultsController.searchResultsTab = searchResultsTable;
				branchView.merchant = tableItems [indexPath.Row];
				//branchView.Title = tableItems [indexPath.Row].COMPANY_NAME;

				InvokeOnMainThread (() => {
					viewControllerLocal.NavigationController.PushViewController (branchView, true);
				});
			} else {
				new UIAlertView("Alert", "This branch does not yet support mobile transactions", null, "Got It!", null).Show ();
			}
		}

		public override UITableViewCell GetCell (UITableView tableView, Foundation.NSIndexPath indexPath)
		{
			UITableViewCell cell = tableView.DequeueReusableCell (cellId);
			if (cell == null)
				cell = new UITableViewCell (UITableViewCellStyle.Subtitle, cellId);


			cell.TextLabel.Text = Convert.ToString(tableItems[indexPath.Row].BRANCH_NAME);// + " | " + tableItems[indexPath.Row].BRANCH_NAME;
			cell.DetailTextLabel.Text = (Math.Ceiling((tableItems[indexPath.Row].distance/1000)*100) / 100.0) + " kilometers";


			//cell.ImageView.Image = RounderCorners(FromURL(tableItems[indexPath.Row].icon_image), 50, 25);
			//cell.ImageView.Image = FromURL(tableItems[indexPath.Row].icon_image);
			return cell;
		}

		public override nfloat GetHeightForRow(UITableView tableview, Foundation.NSIndexPath indexPath)
		{
			return 60;
		}

		static UIImage FromURL (string uri)
		{
			using (var url = new NSUrl (uri))
			using (var data = NSData.FromUrl (url))
				return UIImage.LoadFromData (data);
		}
	}
}

