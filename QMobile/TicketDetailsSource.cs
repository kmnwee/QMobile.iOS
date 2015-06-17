using System;
using UIKit;
using Foundation;

namespace QMobile
{
	public class TicketDetailsSource : UITableViewSource
	{
		TFProcessOption[] tableItems;
		TFMerchants merchant;
		string cellId = "TableCell";
		private UIViewController viewControllerLocal;

		public TicketDetailsSource (TFProcessOption[] items, UIViewController viewController)
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
			cell.TextLabel.Text = tableItems [indexPath.Row].title;// + " | " + tableItems[indexPath.Row].BRANCH_NAME;
			if (tableItems [indexPath.Row].type.Equals ("Status")) {
				switch (tableItems [indexPath.Row].valueDisplay) {
				case "PENDING":
					cell.DetailTextLabel.TextColor = UIColor.Green;
					break;
				case "CALLED":
					cell.DetailTextLabel.TextColor = UIColor.Yellow;
					break;
				case "NO-SHOW":
					cell.DetailTextLabel.TextColor = UIColor.Red;
					break;
				case "DONE":
					cell.DetailTextLabel.TextColor = UIColor.Blue;
					break;
				case "SERVING":
					cell.DetailTextLabel.TextColor = UIColor.Yellow;
					break;
				default:
					cell.DetailTextLabel.TextColor = UIColor.Green;
					break;
				}
			}
			cell.DetailTextLabel.Text = tableItems [indexPath.Row].valueDisplay;
			cell.UserInteractionEnabled = false;
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

