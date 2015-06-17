using System;
using UIKit;
using Foundation;
using System.Globalization;

namespace QMobile
{
	public class TicketsTableSource : UITableViewSource
	{
		TFTicket[] tableItems;
		string cellId = "TableCell";
		private UIViewController viewControllerLocal;

		public TicketsTableSource (TFTicket[] items, UIViewController viewController)
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
//			return "My Tickets";
//		}

		public override void RowSelected (UITableView tableView, Foundation.NSIndexPath indexPath)
		{
			//new UIAlertView("Alert", tableItems[indexPath.Row].company_id + " | " + tableItems[indexPath.Row].branch_id, null, "Got It!", null).Show ();
			tableView.DeselectRow (indexPath, true);

			TicketViewController ticketView = viewControllerLocal.Storyboard.InstantiateViewController ("TicketViewController") as TicketViewController;
			ticketView.ticket = tableItems [indexPath.Row];

			Console.WriteLine (" >>> " + tableItems [indexPath.Row].date);

			InvokeOnMainThread (() => {
				viewControllerLocal.NavigationController.PushViewController (ticketView, true);
			});
		}

		public override UITableViewCell GetCell (UITableView tableView, Foundation.NSIndexPath indexPath)
		{
			UITableViewCell cell = tableView.DequeueReusableCell (cellId);
			if (cell == null)
				cell = new UITableViewCell (UITableViewCellStyle.Subtitle, cellId);

			cell.TextLabel.Text = tableItems[indexPath.Row].queue_no + " | " + tableItems[indexPath.Row].merchant.BRANCH_NAME + ", "+ tableItems[indexPath.Row].merchant.COMPANY_NAME;
			cell.DetailTextLabel.Text = tableItems[indexPath.Row].type;
			if (tableItems [indexPath.Row].type.Equals ("APPOINTMENT")) 
			{
				DateTime date = DateTime.ParseExact(tableItems[indexPath.Row].date, "yyyy-MM-dd", CultureInfo.InvariantCulture);
				DateTime time = DateTime.ParseExact (tableItems [indexPath.Row].time, "HHmm", CultureInfo.InvariantCulture);
				cell.DetailTextLabel.Text += " | " + date.ToString("MMM dd") + " (" + time.ToString ("t") + ")";
			}
			else if (tableItems [indexPath.Row].type.Equals ("RESERVATION")) 
			{
				DateTime date = DateTime.ParseExact(tableItems[indexPath.Row].date, "yyyy-MM-dd", CultureInfo.InvariantCulture);
				cell.DetailTextLabel.Text += " | " + date.ToString("MMM dd");
			}

			cell.DetailTextLabel.Text += " | " + tableItems[indexPath.Row].status;

			string ticketColor = "";
			switch (tableItems [indexPath.Row].status) {
			case "PENDING":
				ticketColor = "ticketcolors/ticketGreen.png";
				break;
			case "CALLED":
				ticketColor = "ticketcolors/ticketYellow.png";
				break;
			case "NO-SHOW":
				ticketColor = "ticketcolors/ticketRed.png";
				break;
			case "DONE":
				ticketColor = "ticketcolors/ticketBlue.png";
				break;
			case "SERVING":
				ticketColor = "ticketcolors/ticketYellow.png";
				break;
			default:
				ticketColor = "ticketcolors/ticketGreen.png";
				break;
			}
			cell.ImageView.Image = UIImage.FromBundle(ticketColor);
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

