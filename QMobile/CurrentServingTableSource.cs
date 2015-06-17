using System;
using UIKit;

namespace QMobile
{
	public class CurrentServingTableSource : UITableViewSource
	{
		CurrentServingList[] tableItems;
		string cellId = "TableCell";
		private UIViewController viewControllerLocal;

		public CurrentServingTableSource (CurrentServingList[] items, UIViewController viewController)
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

			tableView.DeselectRow (indexPath, true);

		}

		public override UITableViewCell GetCell (UITableView tableView, Foundation.NSIndexPath indexPath)
		{
			UITableViewCell cell = tableView.DequeueReusableCell (cellId);
			if (cell == null) {
				cell = new UITableViewCell (UITableViewCellStyle.Value1, cellId);
			}
			cell.TextLabel.Text = "Counter " + tableItems [indexPath.Row].counterNo;
			cell.DetailTextLabel.Text = tableItems [indexPath.Row].queueNo;
			cell.UserInteractionEnabled = false;

			return cell;
		}

	}
}

