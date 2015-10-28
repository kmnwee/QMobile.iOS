using System;
using UIKit;
using Foundation;
using CoreGraphics;

namespace QMobile
{
	public class BranchViewOptionsSource : UITableViewSource
	{
		TFBranchOption[] tableItems;
		string cellId = "TableCell";
		private UIViewController viewControllerLocal;
		LoadingOverlay _loadPop;

		public BranchViewOptionsSource (TFBranchOption[] items, UIViewController viewController)
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

			AppointmentViewController appointmentView = viewControllerLocal.Storyboard.InstantiateViewController ("AppointmentViewController") as AppointmentViewController;
			CurrentServingViewController servingView = viewControllerLocal.Storyboard.InstantiateViewController ("CurrentServingViewController") as CurrentServingViewController;

			InvokeOnMainThread (() => {
				if (!tableItems [indexPath.Row].action.Equals ("CURRENTSERVING") && !tableItems [indexPath.Row].action.Equals ("ABOUT")) {
					if (!tableItems [indexPath.Row].merchant.edition.Equals ("DEMO") && !tableItems [indexPath.Row].merchant.edition.Equals ("DESKTOP")) {
						appointmentView.merchant = tableItems [indexPath.Row].merchant;
						appointmentView.action = tableItems [indexPath.Row].action;
						appointmentView.userLocation = (viewControllerLocal as BranchViewController).coords;
						viewControllerLocal.NavigationController.PushViewController (appointmentView, true);	

						//new UIAlertView("Alert", tableItems[indexPath.Row].title + " | " + tableItems[indexPath.Row].action + " | " + tableItems[indexPath.Row].merchant.BRANCH_NAME, null, "Got It!", null).Show ();

					} else {
						new UIAlertView ("Alert", "This site is running QApps " + tableItems [indexPath.Row].merchant.edition + " Edition! Mobile Transactions are not yet supported.", null, "Got It!", null).Show ();

					}
				} else {
					if (!tableItems [indexPath.Row].merchant.edition.Equals ("DEMO") && !tableItems [indexPath.Row].merchant.edition.Equals ("DESKTOP")) {
						if (!tableItems [indexPath.Row].action.Equals ("ABOUT")) {
							servingView.merchant = tableItems [indexPath.Row].merchant;
							Console.WriteLine (tableItems [indexPath.Row].merchant.COMPANY_NAME + " - " + tableItems [indexPath.Row].merchant.BRANCH_NAME);
							viewControllerLocal.NavigationController.PushViewController (servingView, true);	
						}else{
							new UIAlertView ("Store Info", "Address: " + tableItems [indexPath.Row].merchant.address + "\n" + "Contact: " + tableItems [indexPath.Row].merchant.contact_no, null, "Got It!", null).Show ();
						}
					} else {
						new UIAlertView ("Alert", "This site is running QApps " + tableItems [indexPath.Row].merchant.edition + " Edition! Mobile Transactions are not yet supported.", null, "Got It!", null).Show ();
					}

					//new UIAlertView ("Launch Current Serving Window", "View Counter Activity!", null, "Got It!", null).Show ();
				}

			});

			//			TestViewController test = viewControllerLocal.Storyboard.InstantiateViewController ("TestViewController") as TestViewController;
			//			InvokeOnMainThread (() => viewControllerLocal.NavigationController.PushViewController (test, true));

		}

		public override UITableViewCell GetCell (UITableView tableView, Foundation.NSIndexPath indexPath)
		{
			UITableViewCell cell = tableView.DequeueReusableCell (cellId);
			if (cell == null) {
				cell = new UITableViewCell (UITableViewCellStyle.Subtitle, cellId);
			}
			//			cell.merchant = tableItems [indexPath.Row];
			cell.TextLabel.Text = tableItems [indexPath.Row].title;// + " | " + tableItems[indexPath.Row].BRANCH_NAME;
			cell.DetailTextLabel.Text = tableItems [indexPath.Row].info;
			cell.Accessory = UITableViewCellAccessory.DisclosureIndicator;

			//Console.Out.WriteLine (cell.TextLabel.Text);
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

