using System;
using UIKit;
using Foundation;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;

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

			cell.TextLabel.Text = tableItems [indexPath.Row].queue_no;// + " | " + tableItems[indexPath.Row].merchant.BRANCH_NAME + ", "+ tableItems[indexPath.Row].merchant.COMPANY_NAME;

			InvokeOnMainThread (async () => {
				List<TFMerchants> tfmerchants = new List<TFMerchants> ();
				try {
					tfmerchants = await AppDelegate.MobileService.GetTable<TFMerchants> ()
						.Where (TFMerchants => TFMerchants.COMPANY_NO == tableItems [indexPath.Row].company_id && TFMerchants.BRANCH_NO == tableItems [indexPath.Row].branch_id).Take (1).ToListAsync ();
					if (tfmerchants.Any ()) {
						cell.TextLabel.Text += " | " + tfmerchants.ToArray () [0].BRANCH_NAME + ", " + tfmerchants.ToArray () [0].COMPANY_NAME;
						tableItems [indexPath.Row].merchant = tfmerchants.ToArray () [0];
					}
				} catch (Exception e) {
					Console.WriteLine ("Problem loading Merchant...");
					Console.WriteLine (e.Message);
					Console.WriteLine (e.StackTrace);
				}
			});


			cell.DetailTextLabel.Text = tableItems [indexPath.Row].type;
			if (tableItems [indexPath.Row].type.Equals ("APPOINTMENT")) {
				DateTime date = DateTime.ParseExact (tableItems [indexPath.Row].date, "yyyy-MM-dd", CultureInfo.InvariantCulture);
				DateTime time = DateTime.ParseExact (tableItems [indexPath.Row].time, "HHmm", CultureInfo.InvariantCulture);
				if(tableItems [indexPath.Row].company_id != 7)
					cell.DetailTextLabel.Text += " | " + date.ToString ("MMM dd") + " (" + time.ToString ("t") + ")";
				else
					cell.DetailTextLabel.Text += " | " + date.ToString ("MMM dd") + " (" + tableItems [indexPath.Row].timeString.Replace(";",":") + ")";
			} else if (tableItems [indexPath.Row].type.Equals ("RESERVATION")) {
				DateTime date = DateTime.ParseExact (tableItems [indexPath.Row].date, "yyyy-MM-dd", CultureInfo.InvariantCulture);
				cell.DetailTextLabel.Text += " | " + date.ToString ("MMM dd");
			}

			cell.DetailTextLabel.Text += " | " + tableItems [indexPath.Row].status;

			string ticketColor = "";
			switch (tableItems [indexPath.Row].status) {
			case "PENDING":
				ticketColor = "ticketcolors/tag_green.png";
				break;
			case "CALLED":
				ticketColor = "ticketcolors/tag_yellow.png";
				break;
			case "NO-SHOW":
				ticketColor = "ticketcolors/tag_red.png";
				break;
			case "DONE":
				ticketColor = "ticketcolors/tag_blue.png";
				break;
			case "SERVING":
				ticketColor = "ticketcolors/tag_yellow.png";
				break;
			case "TRANSFERRED":
				ticketColor = "ticketcolors/tag_blue.png";
				break;
			default:
				ticketColor = "ticketcolors/tag_green.png";
				break;
			}
			cell.ImageView.Image = FeaturedTableSource.MaxResizeImage(UIImage.FromBundle (ticketColor), 50, 50);
			//cell.ImageView.Image = FromURL(tableItems[indexPath.Row].icon_image);
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

	}
}

