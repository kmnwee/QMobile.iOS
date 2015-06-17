using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;
using CoreGraphics;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace QMobile
{
	partial class TicketsTableController : UITableViewController
	{
		LoadingOverlay _loadPop;
		public List<TFTicket> TFTickets = new List<TFTicket> ();
		public List<TFScheduledReservation> TFSchedList = new List<TFScheduledReservation> ();
		public List<TFOLReservation> TFReserveList = new List<TFOLReservation> ();

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

			RefreshControl = new UIRefreshControl ();
			RefreshControl.ValueChanged += (object sender, EventArgs e) => {
				Console.WriteLine ("Refresh Initiated");	
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
					.OrderBy (TFScheduledReservation => TFScheduledReservation.reservation_date)
					.ThenBy (TFScheduledReservation => TFScheduledReservation.reservation_time)
					.ToListAsync ();

					TFReserveList = await AppDelegate.MobileService.GetTable<TFOLReservation> ().Take (500)
					.Where (TFOLReservation => TFOLReservation.remarks.Contains (email))
					.OrderBy (TFOLReservation => TFOLReservation.date_in)
					.ToListAsync ();

					//TFFeaturedMerchants = TFMerchantList.GroupBy(c => c.COMPANY_NO).Select(grp => grp.FirstOrDefault()).OrderBy(f => f.featured_flag).ToList();
					TFTicket tix = new TFTicket ();
					foreach (TFScheduledReservation sr in TFSchedList) {
						tix = new TFTicket ();
						tfmerchants = new List<TFMerchants> ();
						tfmerchants = await AppDelegate.MobileService.GetTable<TFMerchants> ()
						.Where (TFMerchants => TFMerchants.COMPANY_NO == sr.company_id && TFMerchants.BRANCH_NO == sr.branch_id).Take (1).ToListAsync ();
						if (tfmerchants.Any ())
							tix.merchant = tfmerchants.ToArray () [0];
						tix.branch_id = sr.branch_id;
						tix.company_id = sr.company_id;
						tix.id = Convert.ToString (sr.id);
						tix.cust_name = sr.cust_name;
						tix.queue_no = sr.queue_no;
						tix.status = sr.status;
						tix.date = sr.reservation_date;
						tix.time = sr.reservation_time;
						tix.image_icon = sr.image_icon;
						tix.tran_type_name = sr.tran_type_name;
						tix.tran_id_local = sr.tran_id_local;
						tix.type = "APPOINTMENT";
						if (DateTime.ParseExact (sr.reservation_date, "yyyy-MM-dd", CultureInfo.InvariantCulture).CompareTo (dateNow) >= 0) {
							TFTickets.Add (tix);
							Console.WriteLine("APPT : " + tix.id + ",  " + tix.company_id + ":"+tix.branch_id);
						}
					}

					foreach (TFOLReservation sr in TFReserveList) {
						tix = new TFTicket ();
						tfmerchants = new List<TFMerchants> ();
						tfmerchants = await AppDelegate.MobileService.GetTable<TFMerchants> ()
						.Where (TFMerchants => TFMerchants.COMPANY_NO == sr.company_id && TFMerchants.BRANCH_NO == sr.branch_id).Take (1).ToListAsync ();
						if (tfmerchants.Any ())
							tix.merchant = tfmerchants.ToArray () [0];
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
							Console.WriteLine("RSVN : " + sr.user_refno + ",  " + tix.company_id + ":"+tix.branch_id);
						}
					}

					TFTickets = TFTickets.OrderBy (t => t.date).ToList ();
					ticketsTable.Source = new TicketsTableSource (TFTickets.ToArray (), this);
					ticketsTable.ReloadData ();

					if(!TFTickets.Any())
					{
						new UIAlertView ("No Tickets!", "You currently have no active tickets.", null, "OK", null).Show ();
					}
				} catch (Exception exex) {
					new UIAlertView ("Problem Connecting", "We can't seem to connect to the internet right now.", null, "OK", null).Show ();
					Console.WriteLine(" >>>> " + exex.Message + " : " + exex.StackTrace);
				}
				//------LOADING Screen END----------------------
				this._loadPop.Hide ();
				//------LOADING Screen END----------------------
			});
		}



		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
		}

		public override void ViewDidAppear (bool animated)
		{
			Console.WriteLine ("Tix view: " + AppDelegate.tfAccount.email + "/ " + AppDelegate.tfAccount.loggedIn);
			//------LOADING Screen--------------------------
			// Determine the correct size to start the overlay (depending on device orientation)
			var bounds = UIScreen.MainScreen.Bounds; // portrait bounds
			if (UIApplication.SharedApplication.StatusBarOrientation == UIInterfaceOrientation.LandscapeLeft || UIApplication.SharedApplication.StatusBarOrientation == UIInterfaceOrientation.LandscapeRight) {
				bounds.Size = new CGSize (bounds.Size.Height, bounds.Size.Width);
			}
			// show the loading overlay on the UI thread using the correct orientation sizing
			this._loadPop = new LoadingOverlay (bounds);
			this.View.Add (this._loadPop);
			//------LOADING Screen--------------------------

			if (AppDelegate.tfAccount.loggedIn) {
				RefreshTicketsTable (AppDelegate.tfAccount.email);
			} else {

				//------LOADING Screen END----------------------
				this._loadPop.Hide ();
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
		}

		public override void ViewDidDisappear (bool animated)
		{
			base.ViewDidDisappear (animated);
		}

		#endregion
	}
}
