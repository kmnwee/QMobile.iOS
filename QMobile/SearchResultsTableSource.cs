using System;
using UIKit;
using Foundation;

namespace QMobile
{
	public class SearchResultsTableSource : UITableViewSource
	{
		TFMerchants[] tableItems;
		string cellId = "TableCell";
		private UIViewController viewControllerLocal;


		public SearchResultsTableSource (TFMerchants[] items, UIViewController viewController)
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
		//	new UIAlertView("Alert", tableItems[indexPath.Row].COMPANY_NAME + " | " + tableItems[indexPath.Row].BRANCH_NAME, null, "Got It!", null).Show ();
			tableView.DeselectRow (indexPath, true);

			BranchViewController branchView = viewControllerLocal.Storyboard.InstantiateViewController ("BranchViewController") as BranchViewController;
//			//searchResultsController.searchResultsTab = searchResultsTable;
			branchView.merchant = tableItems [indexPath.Row];
			//branchView.Title = tableItems [indexPath.Row].COMPANY_NAME;

			InvokeOnMainThread (() => {
				//branchView.NavigationItem.BackBarButtonItem = new UIBarButtonItem("Back", UIBarButtonItemStyle.Plain, null);
//				branchView.NavigationItem.SetLeftBarButtonItem(new UIBarButtonItem("Back", UIBarButtonItemStyle.Plain, null), true);
				//new UIImageView (UIImage.FromBundle ("iconx30.png"));
				viewControllerLocal.NavigationController.PushViewController (branchView, true);
			});

//			TestViewController test = viewControllerLocal.Storyboard.InstantiateViewController ("TestViewController") as TestViewController;
//			InvokeOnMainThread (() => viewControllerLocal.NavigationController.PushViewController (test, true));

		}

		public override UITableViewCell GetCell (UITableView tableView, Foundation.NSIndexPath indexPath)
		{
			UITableViewCell cell = tableView.DequeueReusableCell (cellId);
//			var cell = (MerchantCustomCell) tableView.DequeueReusableCell (MerchantCustomCell.Key);
			if (cell == null) {
//				cell = MerchantCustomCell.Create ();
				cell = new UITableViewCell (UITableViewCellStyle.Subtitle, cellId);
			}
//			cell.merchant = tableItems [indexPath.Row];
			cell.TextLabel.Text = tableItems [indexPath.Row].BRANCH_NAME;// + " | " + tableItems[indexPath.Row].BRANCH_NAME;
			cell.DetailTextLabel.Text = tableItems [indexPath.Row].COMPANY_NAME;
			if (tableItems [indexPath.Row].featured_flag > 0)
				cell.DetailTextLabel.Text += " | Featured ";
			Console.Out.WriteLine ("merchant : " + tableItems [indexPath.Row].BRANCH_NAME + "Sched flag: " + tableItems [indexPath.Row].schedReserve_flag);
			//cell.ImageView.Image = FromURL(tableItems [indexPath.Row].icon_image);// fix size. use small thumbnail only
			return cell;
		}

		static UIImage FromURL (string uri)
		{
			using (var url = new NSUrl (uri))
			using (var data = NSData.FromUrl (url))
				return UIImage.LoadFromData (data);
		}


	}
}

