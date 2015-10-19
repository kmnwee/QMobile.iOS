using System;
using UIKit;
using Foundation;
using System.Net;
using System.IO;
using ToastIOS;
using Newtonsoft.Json;
using MBProgressHUD;

namespace QMobile
{
	public class CompanyTableSource : UITableViewSource
	{
		TFMerchants[] tableItems;
		string cellId = "TableCell";
		private UIViewController viewControllerLocal;
		MTMBProgressHUD hud;

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
			hud = new MTMBProgressHUD (this.viewControllerLocal.View) {
				LabelText = "checking branch status...",
				RemoveFromSuperViewOnHide = true,
				AnimationType = MBProgressHUDAnimation.Fade,
				//DetailsLabelText = "loading profile details...",
				Mode = MBProgressHUDMode.Indeterminate,
				Color = UIColor.Gray,
				Opacity = 60,
				DimBackground = true
			};
			this.viewControllerLocal.View.AddSubview (hud);

			//new UIAlertView("Alert", tableItems[indexPath.Row].company_id + " | " + tableItems[indexPath.Row].branch_id, null, "Got It!", null).Show ();
			tableView.DeselectRow (indexPath, true);
			if (tableItems [indexPath.Row].COMPANY_NO != 7) {
				if (!tableItems [indexPath.Row].edition.Equals ("DEMO") && !tableItems [indexPath.Row].edition.Equals ("DESKTOP")) {
					BranchViewController branchView = viewControllerLocal.Storyboard.InstantiateViewController ("BranchViewController") as BranchViewController;
					//			//searchResultsController.searchResultsTab = searchResultsTable;
					branchView.merchant = tableItems [indexPath.Row];
					//branchView.Title = tableItems [indexPath.Row].COMPANY_NAME;

					InvokeOnMainThread (() => {
						viewControllerLocal.NavigationController.PushViewController (branchView, true);
					});
				} else {
					new UIAlertView ("Alert", "This branch does not yet support mobile transactions", null, "Got It!", null).Show ();
				}
			} else {
				//Smart Store, check status first before enter
				getBranchStatus (indexPath, tableItems [indexPath.Row].serviceURL, tableItems [indexPath.Row].COMPANY_NO, tableItems [indexPath.Row].BRANCH_NO);


			}
		}

		public override UITableViewCell GetCell (UITableView tableView, Foundation.NSIndexPath indexPath)
		{
			UITableViewCell cell = tableView.DequeueReusableCell (cellId);
			if (cell == null)
				cell = new UITableViewCell (UITableViewCellStyle.Subtitle, cellId);


			cell.TextLabel.Text = Convert.ToString (tableItems [indexPath.Row].BRANCH_NAME);// + " | " + tableItems[indexPath.Row].BRANCH_NAME;
			cell.DetailTextLabel.Text = (Math.Ceiling ((tableItems [indexPath.Row].distance / 1000) * 100) / 100.0) + " kilometers";


			//cell.ImageView.Image = RounderCorners(FromURL(tableItems[indexPath.Row].icon_image), 50, 25);
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

		public async void getBranchStatus (Foundation.NSIndexPath indexPath, String serviceURL, int company_id, int branch_id)
		{
			hud.Show (true);
			try {
				string url = String.Format (serviceURL + "/kioskJSON.svc/getBranchStatus/{0}/{1}/", company_id, branch_id);
				HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create (new Uri (url));
				request.ContentType = "application/json";
				request.Method = "GET";
				using (HttpWebResponse response = await request.GetResponseAsync () as HttpWebResponse) {
					if (response.StatusCode != HttpStatusCode.OK) {
						Console.Out.WriteLine ("Error fetching data. Server returned status code: {0}", response.StatusCode);
						new UIAlertView ("Problem Connecting", "We can't seem to connect to the internet.", null, "OK", null).Show ();
					} else {
						using (StreamReader reader = new StreamReader (response.GetResponseStream ())) {
							var content = reader.ReadToEnd ();
							if (string.IsNullOrWhiteSpace (content)) {
								Console.Out.WriteLine ("Response contained empty body...");
							} else {
								Console.Out.WriteLine ("Response Body: \r\n {0}", content);
								GetBranchStatusResponse branchStatus = new GetBranchStatusResponse ();
								branchStatus = JsonConvert.DeserializeObject<GetBranchStatusResponse> (content);
								Console.WriteLine (branchStatus.getBranchStatusResult.ResponseCode);
								Console.WriteLine (branchStatus.getBranchStatusResult.ResponseMessage);
								if (branchStatus.getBranchStatusResult.ResponseCode.Equals ("00")) {
									//after checking branch status:
									BranchViewController branchView = viewControllerLocal.Storyboard.InstantiateViewController ("BranchViewController") as BranchViewController;
									//			//searchResultsController.searchResultsTab = searchResultsTable;
									branchView.merchant = tableItems [indexPath.Row];
									//branchView.Title = tableItems [indexPath.Row].COMPANY_NAME;

									InvokeOnMainThread (() => {
										viewControllerLocal.NavigationController.PushViewController (branchView, true);
									});

								} else {
									Toast.MakeText (branchStatus.getBranchStatusResult.ResponseMessage)
									.SetDuration (5000)
									.Show ();
								}
							}
						}
					}
				}
			} catch (Exception e) {
				new UIAlertView ("Problem Connecting", "We can't seem to connect to the internet.", null, "OK", null).Show ();
				Console.WriteLine (e.Message);
				Console.WriteLine (e.StackTrace);
			}
			//---------------------------------------------------------
			hud.Hide (animated: true);
		}
	}
}

