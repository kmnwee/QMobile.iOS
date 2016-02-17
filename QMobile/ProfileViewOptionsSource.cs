using System;
using UIKit;
using MBProgressHUD;
using ToastIOS;
using Newtonsoft.Json;
using System.Net;
using System.IO;
using System.Collections.Generic;
using System.Globalization;
using Xamarin.Auth;
using Newtonsoft.Json.Linq;
using System.Web;
using Microsoft.WindowsAzure.MobileServices;

namespace QMobile
{
	public class ProfileViewOptionsSource: UITableViewSource
	{
		TFProfileOption[] tableItems;
		string cellId = "TableCell";
		private UIViewController viewControllerLocal;
		//LoadingOverlay _loadPop;
		MTMBProgressHUD hud2;
		MTMBProgressHUD hud;

		bool loggedIn;
		TFMemberRegistry memberRegistry;
		List<TFProfileOption> options;
		DashTabController dashTab;

		public ProfileViewOptionsSource (TFProfileOption[] items, UIViewController viewController)
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
			dashTab = viewControllerLocal.ParentViewController as DashTabController;
			//AppointmentViewController appointmentView = viewControllerLocal.Storyboard.InstantiateViewController ("AppointmentViewController") as AppointmentViewController;
			//CurrentServingViewController servingView = viewControllerLocal.Storyboard.InstantiateViewController ("CurrentServingViewController") as CurrentServingViewController;

			InvokeOnMainThread (() => {
				//if (tableItems [indexPath.Row].action.Equals ("QR")) 
				switch(tableItems [indexPath.Row].action){
				case "SIFB":
					(viewControllerLocal as ProfileViewController).signIn();
					break;
				case "SIG":
					(viewControllerLocal as ProfileViewController).signInGuest();
					break;
				case "Search":
					(viewControllerLocal as ProfileViewController).setTabIndex(1);
					break;
				case "Favorites":
					(viewControllerLocal as ProfileViewController).setTabIndex(2);
					break;
				case "Tickets":
					(viewControllerLocal as ProfileViewController).setTabIndex(3);
					break;
				case "SignOut":
					(viewControllerLocal as ProfileViewController).signOut();
					break;
				case "QR":
					Console.WriteLine(tableItems [indexPath.Row].action);
					scanQR();
					break;
				default:
					Console.WriteLine(tableItems [indexPath.Row].action);
					break;
				}

			});
		}

		public override UITableViewCell GetCell (UITableView tableView, Foundation.NSIndexPath indexPath)
		{
			UITableViewCell cell = tableView.DequeueReusableCell (cellId);
			if (cell == null) {
				cell = new UITableViewCell (UITableViewCellStyle.Subtitle, cellId);
			}

			Console.WriteLine (" >> " + tableItems [indexPath.Row].image);
			cell.TextLabel.Text = tableItems [indexPath.Row].title;// + " | " + tableItems[indexPath.Row].BRANCH_NAME;
			cell.DetailTextLabel.Text = tableItems [indexPath.Row].info;
			if(!String.IsNullOrEmpty(tableItems [indexPath.Row].image))
			cell.ImageView.Image = FeaturedTableSource.MaxResizeImage (UIImage.FromBundle (tableItems [indexPath.Row].image), 30, 30);
			cell.Accessory = UITableViewCellAccessory.DisclosureIndicator;


			//Console.Out.WriteLine (cell.TextLabel.Text);
			//cell.ImageView.Image = FromURL(tableItems [indexPath.Row].icon_image);// fix size. use small thumbnail only
			return cell;
		}


		public async void scanQR ()
		{
			var scanner = new ZXing.Mobile.MobileBarcodeScanner ();
			var result = await scanner.Scan ();

			if (result != null) {
				Console.WriteLine ("Scanned Barcode: " + result.Text);
				hud2 = new MTMBProgressHUD (viewControllerLocal.View) {
					LabelText = "Processing...",
					RemoveFromSuperViewOnHide = true,
					AnimationType = MBProgressHUDAnimation.Fade,
					//DetailsLabelText = "loading profile details...",
					Mode = MBProgressHUDMode.Indeterminate,
					Color = UIColor.Gray,
					Opacity = 60,
					DimBackground = true
				};

				viewControllerLocal.View.AddSubview (hud2);
				//hud2.Show (animated: true);

				try {
					QRCodeUser qr = new QRCodeUser ();
					qr = JsonConvert.DeserializeObject<QRCodeUser> (result.Text);

					Console.WriteLine (qr.u);
					Console.WriteLine (qr.b);
					Console.WriteLine (qr.c);

					if (!String.IsNullOrEmpty (qr.u) && !String.IsNullOrEmpty (qr.b) && !String.IsNullOrEmpty (qr.c)) {
						//map to webservice
						addUserFromQR (qr.u, qr.c, qr.b);
					} else {
						Toast.MakeText ("The QR code you scanned is invalid. Please try again.")
							.SetDuration (5000)
							.Show ();
						//hud2.Hide (animated: true);
					}

				} catch (Exception e) {
					//new UIAlertView ("Code Unrecognized", "The QR code you scanned is invalid. Please try again.", null, "OK", null).Show ();
					Toast.MakeText ("The QR code you scanned is invalid. Please try again.")
						.SetDuration (5000)
						.Show ();
				}
				//				hud2.Hide (animated: true);
			}
		}

		public async void addUserFromQR (String refNo, String company_id, String branch_id)
		{
			//Time Slot -- add error checking for this
			try {
				string url = String.Format ("https://tfsmsgatesit.azurewebsites.net/TFGatewayJSON.svc/addUserFromQR/{0}/{1}/{2}/{3}/{4}/", 
					refNo, AppDelegate.tfAccount.email, AppDelegate.tfAccount.name, company_id, branch_id);
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
								AddUserFromQRResponse addUserFromQRResp = new AddUserFromQRResponse ();
								addUserFromQRResp = JsonConvert.DeserializeObject<AddUserFromQRResponse> (content);
								Console.WriteLine (addUserFromQRResp.addUserFromQRResult.ResponseCode);
								Console.WriteLine (addUserFromQRResp.addUserFromQRResult.ResponseMessage);
								Console.WriteLine (addUserFromQRResp.addUserFromQRResult.refNo);
								Console.WriteLine (addUserFromQRResp.addUserFromQRResult.company_id);
								Console.WriteLine (addUserFromQRResp.addUserFromQRResult.branch_id);
								Toast.MakeText (addUserFromQRResp.addUserFromQRResult.ResponseMessage)
									.SetDuration (5000)
									.Show ();

								if (addUserFromQRResp.addUserFromQRResult.ResponseCode.Equals ("00")) {
									List<TFOLReservation> ticketRaw = new List<TFOLReservation> ();
									List<TFMerchants> merchants = new List<TFMerchants>();
									TFMerchants merchant = new TFMerchants();
									TFTicket tix = new TFTicket ();
									int company_id2 = Convert.ToInt32 (addUserFromQRResp.addUserFromQRResult.company_id);
									int branch_id2 = Convert.ToInt32 (addUserFromQRResp.addUserFromQRResult.branch_id);
									string refNo2 = addUserFromQRResp.addUserFromQRResult.refNo;
									merchants = await AppDelegate.MobileService.GetTable<TFMerchants> ().Take (1)
										.Where (m => m.COMPANY_NO == company_id2 && m.BRANCH_NO == branch_id2).ToListAsync ();
									ticketRaw = await AppDelegate.MobileService.GetTable<TFOLReservation> ().Take (1)
										.Where (tf => tf.company_id == company_id2 && tf.branch_id == branch_id2 && tf.user_refno == refNo2).ToListAsync ();

									merchant = merchants.ToArray()[0];
									foreach (TFOLReservation sr in ticketRaw) {
										tix = new TFTicket();
										tix.branch_id = sr.branch_id;
										tix.company_id = sr.company_id;
										tix.id = Convert.ToString (sr.id);
										tix.cust_name = sr.cust_name;
										tix.queue_no = sr.queue_no;
										tix.status = sr.status;
										tix.date = DateTime.ParseExact (sr.date_in, "MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString ("yyyy-MM-dd");
										tix.image_icon = sr.image_icon;
										tix.tran_type_name = sr.tran_type;
										tix.tran_id_local = sr.tran_id_local;
										tix.type = "RESERVATION";
									}
									tix.merchant = merchant;
									TicketViewController ticketView = viewControllerLocal.Storyboard.InstantiateViewController ("TicketViewController") as TicketViewController;
									ticketView.ticket = tix;

									InvokeOnMainThread (() => viewControllerLocal.NavigationController.PushViewController (ticketView, true));
								}
								//Console.WriteLine ("Refresh Initiated - QR blank ");	
								//RefreshTicketsTable ("");//clear first
								//Console.WriteLine ("Refresh Initiated - QR ");	
								//RefreshTicketsTable (AppDelegate.tfAccount.email);
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
			//hud2.Hide (animated: true);
		}

	}

}

