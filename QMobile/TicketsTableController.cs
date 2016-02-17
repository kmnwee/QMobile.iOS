using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;
using CoreGraphics;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using MBProgressHUD;
using Newtonsoft.Json;
using ToastIOS;
using System.Net;
using System.IO;

namespace QMobile
{
	partial class TicketsTableController : UITableViewController
	{
		LoadingOverlay _loadPop;
		public List<TFTicket> TFTickets = new List<TFTicket> ();
		public List<TFScheduledReservation> TFSchedList = new List<TFScheduledReservation> ();
		public List<TFOLReservation> TFReserveList = new List<TFOLReservation> ();
		MTMBProgressHUD hud;
		MTMBProgressHUD hud2;

		public TicketsTableController (IntPtr handle) : base (handle)
		{
		}

		#region View lifecycle

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			DashTabController dashTab = this.ParentViewController as DashTabController;
			Console.WriteLine ("Tix View Loaded");
			Console.WriteLine (String.Format ("Tix Tab : {0}, {1}, {2}", AppDelegate.tfAccount.name, AppDelegate.tfAccount.email, AppDelegate.tfAccount.birthday));

			//------LOADING Screen--------------------------
			// Determine the correct size to start the overlay (depending on device orientation)
//			var bounds = UIScreen.MainScreen.Bounds; // portrait bounds
//			if (UIApplication.SharedApplication.StatusBarOrientation == UIInterfaceOrientation.LandscapeLeft || UIApplication.SharedApplication.StatusBarOrientation == UIInterfaceOrientation.LandscapeRight) {
//				bounds.Size = new CGSize (bounds.Size.Height, bounds.Size.Width);
//			}
//			// show the loading overlay on the UI thread using the correct orientation sizing
//			this._loadPop = new LoadingOverlay (bounds);
			//------LOADING Screen--------------------------

			hud = new MTMBProgressHUD (View) {
				LabelText = "Loading tickets...",
				RemoveFromSuperViewOnHide = true,
				AnimationType = MBProgressHUDAnimation.Fade,
				//DetailsLabelText = "loading profile details...",
				Mode = MBProgressHUDMode.Indeterminate,
				Color = UIColor.Gray,
				Opacity = 60,
				DimBackground = true
			};

			RefreshControl = new UIRefreshControl ();
			RefreshControl.ValueChanged += (object sender, EventArgs e) => {
				Console.WriteLine ("Refresh Initiated - pull down");	
				RefreshTicketsTable (AppDelegate.tfAccount.email);
				InvokeOnMainThread (() => {
					RefreshControl.EndRefreshing ();
				});
			};

		}

		public void RefreshTicketsTable (String email)
		{
			DateTime dateNow = DateTime.Now;
			String dateNowString = dateNow.ToString ("yyyy-MM-dd");
			dateNow = DateTime.ParseExact (dateNowString, "yyyy-MM-dd", CultureInfo.InvariantCulture);
			Console.WriteLine ("DateNOW: " + dateNowString + " or " + dateNow.ToString ());
			Console.WriteLine ("RefreshTicketsTable email: " + email);

			InvokeOnMainThread (async () => {
				TFTickets = new List<TFTicket> ();
				TFSchedList = new List<TFScheduledReservation> ();
				TFReserveList = new List<TFOLReservation> ();
				List<TFMerchants> tfmerchants = new List<TFMerchants> ();
				try {
					TFSchedList = await AppDelegate.MobileService.GetTable<TFScheduledReservation> ().Take (100)
					.Where (TFScheduledReservation => TFScheduledReservation.email == email)
					.OrderByDescending (TFScheduledReservation => TFScheduledReservation.reservation_date)
					.ThenBy (TFScheduledReservation => TFScheduledReservation.reservation_time)
					.ToListAsync ();

					TFReserveList = await AppDelegate.MobileService.GetTable<TFOLReservation> ().Take (100)
					.Where (TFOLReservation => TFOLReservation.remarks.Contains (email))
					.OrderByDescending (TFOLReservation => TFOLReservation.date_in)
					.ToListAsync ();

					//TFFeaturedMerchants = TFMerchantList.GroupBy(c => c.COMPANY_NO).Select(grp => grp.FirstOrDefault()).OrderBy(f => f.featured_flag).ToList();
					TFTicket tix = new TFTicket ();
					foreach (TFScheduledReservation sr in TFSchedList) {
						tix = new TFTicket ();
//						tfmerchants = new List<TFMerchants> ();
//						tfmerchants = await AppDelegate.MobileService.GetTable<TFMerchants> ()
//						.Where (TFMerchants => TFMerchants.COMPANY_NO == sr.company_id && TFMerchants.BRANCH_NO == sr.branch_id).Take (1).ToListAsync ();
//						if (tfmerchants.Any ())
//							tix.merchant = tfmerchants.ToArray () [0];
						tix.branch_id = sr.branch_id;
						tix.company_id = sr.company_id;
						tix.id = Convert.ToString (sr.id);
						tix.cust_name = sr.cust_name;
						tix.queue_no = sr.queue_no;
						tix.status = sr.status;
						tix.date = sr.reservation_date;
						tix.time = sr.reservation_time;
						tix.timeString = sr.reservation_time_string;
						tix.image_icon = sr.image_icon;
						tix.tran_type_name = sr.tran_type_name;
						tix.tran_id_local = sr.tran_id_local;
						tix.type = "APPOINTMENT";
						if (DateTime.ParseExact (sr.reservation_date, "yyyy-MM-dd", CultureInfo.InvariantCulture).CompareTo (dateNow) >= 0) {
							TFTickets.Add (tix);
							Console.WriteLine ("APPT : " + tix.id + ",  " + tix.company_id + ":" + tix.branch_id);
						}
					}

					foreach (TFOLReservation sr in TFReserveList) {
						tix = new TFTicket ();
//						tfmerchants = new List<TFMerchants> ();
//						tfmerchants = await AppDelegate.MobileService.GetTable<TFMerchants> ()
//						.Where (TFMerchants => TFMerchants.COMPANY_NO == sr.company_id && TFMerchants.BRANCH_NO == sr.branch_id).Take (1).ToListAsync ();
//						if (tfmerchants.Any ())
//							tix.merchant = tfmerchants.ToArray () [0];
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
						if (DateTime.ParseExact (sr.date_in, "MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture).CompareTo (dateNow) >= 0) {
							TFTickets.Add (tix);
							Console.WriteLine ("RSVN : " + sr.user_refno + ",  " + tix.company_id + ":" + tix.branch_id);
						}
					}

					TFTickets = TFTickets.OrderByDescending (t => t.date).ThenBy(q => q.queue_no).ToList ();
					ticketsTable.Source = new TicketsTableSource (TFTickets.ToArray (), this);
					ticketsTable.ReloadData ();

					if (!TFTickets.Any ()) {
						//new UIAlertView ("No Tickets!", "You currently have no active tickets.", null, "OK", null).Show ();
						Toast.MakeText ("You currently have no active tickets.").SetDuration (4000).SetGravity(ToastGravity.Center).Show ();
					}
				} catch (Exception exex) {
					new UIAlertView ("Problem Connecting", "We can't seem to connect to the internet right now.", null, "OK", null).Show ();
					Console.WriteLine (exex.Message);
					Console.WriteLine (" >>>> " + exex.Message + " : " + exex.StackTrace);
				}

				hud.Hide (true);
				//------LOADING Screen END----------------------
				//this._loadPop.Hide ();
				//------LOADING Screen END----------------------
			});
		}


		public async void scanQR ()
		{
			var scanner = new ZXing.Mobile.MobileBarcodeScanner ();
			var result = await scanner.Scan ();

			if (result != null) {
				Console.WriteLine ("Scanned Barcode: " + result.Text);
				hud2 = new MTMBProgressHUD (View) {
					LabelText = "Processing...",
					RemoveFromSuperViewOnHide = true,
					AnimationType = MBProgressHUDAnimation.Fade,
					//DetailsLabelText = "loading profile details...",
					Mode = MBProgressHUDMode.Indeterminate,
					Color = UIColor.Gray,
					Opacity = 60,
					DimBackground = true
				};

				View.AddSubview (hud2);
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
									TicketViewController ticketView = this.Storyboard.InstantiateViewController ("TicketViewController") as TicketViewController;
									ticketView.ticket = tix;

									InvokeOnMainThread (() => this.NavigationController.PushViewController (ticketView, true));
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

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			InvokeOnMainThread (() => {
				Console.WriteLine ("Tickets Table!");
				this.ParentViewController.NavigationItem.SetRightBarButtonItem 
				(new UIBarButtonItem (FeaturedTableSource.MaxResizeImage (UIImage.FromBundle ("icons_f/qrwhite.png"), 30, 30), 
					UIBarButtonItemStyle.Plain, (sender, args) => {
					Console.WriteLine ("Scan Button was pressed!");
					scanQR ();
				}), true);
			});
		}

		public override void ViewDidAppear (bool animated)
		{
			Console.WriteLine ("Tix view: " + AppDelegate.tfAccount.email + "/ " + AppDelegate.tfAccount.loggedIn);

			View.AddSubview (hud);
			hud.Show (animated: true);

//			if (AppDelegate.initialLoadTix) {
//				this.View.Add (this._loadPop);
//				//------LOADING Screen--------------------------
//			}

			if (AppDelegate.tfAccount.loggedIn) {
				Console.WriteLine ("Refresh Initiated - ViewDidAppear ");	
				RefreshTicketsTable (AppDelegate.tfAccount.email);
				AppDelegate.initialLoadTix = false;
			} else {

				hud.Hide (true);
				//------LOADING Screen END----------------------
				//this._loadPop.Hide ();
				//------LOADING Screen END----------------------
				RefreshTicketsTable ("");
				InvokeOnMainThread (() => {
					ticketsTable = new UITableView ();
				});
				new UIAlertView ("Profile Needed", "Please login in profile tab.", null, "OK", null).Show ();
			}
			base.ViewDidAppear (animated);
		}

		public override void ViewDidLayoutSubviews ()
		{
			base.ViewDidLayoutSubviews ();
			if (ticketsTable != null)
				ticketsTable.ContentInset = new UIEdgeInsets (64, 0, 0, 0);

		}

		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);
			InvokeOnMainThread (() => {
				Console.WriteLine ("Tickets Table remove!");
				this.ParentViewController.NavigationItem.SetRightBarButtonItem (null, true);
			});
		}

		public override void ViewDidDisappear (bool animated)
		{
			base.ViewDidDisappear (animated);
		}

		#endregion
	}
}
