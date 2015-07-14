using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;
using CoreGraphics;
using System.Globalization;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using Newtonsoft.Json;

namespace QMobile
{
	partial class TicketViewController : UIViewController
	{
		public TFTicket ticket;
		public TFGetTicketResponse olReservation;
		public TFScheduledAppointment appointment;
		public List<TFProcessOption> options;
		public TFCurrentServingResponse currentServing;

		public TicketViewController (IntPtr handle) : base (handle)
		{
		}

		#region View lifecycle

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			ticketContainerView.BackgroundColor = TFColor.FromHexString ("#00bcd4", 1.0f);
			branchDetailsView.BackgroundColor = TFColor.FromHexString ("#0097a9", 1.0f);

			//-------Details List-----------------------
			TFProcessOption option1 = new TFProcessOption ();
			TFProcessOption option2 = new TFProcessOption ();
			TFProcessOption option3 = new TFProcessOption ();
			TFProcessOption option4 = new TFProcessOption ();
			options = new List<TFProcessOption> ();
			//---------------------------------------------------------
			option1.title = "Status";
			option1.valueDisplay = ticket.status;
			option1.type = "Status";
			option1.value = ticket.status;
			options.Add (option1);

			if (ticket.type.Equals ("APPOINTMENT"))
				option2.title = "Appointment Date";
			else if (ticket.type.Equals ("RESERVATION"))
				option2.title = "Ticket Date";
			option2.valueDisplay = DateTime.ParseExact (ticket.date, "yyyy-MM-dd", CultureInfo.InvariantCulture).ToString ("ddd, MMM dd");
			option2.type = "Date";
			option2.value = ticket.date;
			options.Add (option2);

			if (ticket.type.Equals ("APPOINTMENT")) {
				option3.title = "Time Slot";
				option3.valueDisplay = DateTime.ParseExact (ticket.time, "HHmm", CultureInfo.InvariantCulture).ToString ("hh:mm tt") + " - "
				+ DateTime.ParseExact (ticket.time, "HHmm", CultureInfo.InvariantCulture).AddMinutes (ticket.merchant.schedReserve_interval).ToString ("hh:mm tt");
				option3.type = "Time";
				option3.value = ticket.time;
				options.Add (option3);
			}

			option4.title = "Service Type";
			option4.valueDisplay = ticket.tran_type_name;//should be 1st trantype from json
			option4.type = "Transaction";
			option4.value = option1.valueDisplay;//same as value display
			option4.valueId = ticket.tran_id_local; //tranype id
			options.Add (option4);

			//-------Details List-----------------------

			InvokeOnMainThread (() => {
				//this.NavigationItem.TitleView = new UIImageView (UIImage.FromBundle ("iconx30.png"));
				this.NavigationItem.TitleView = new UIImageView (FeaturedTableSource.MaxResizeImage(UIImage.FromBundle ("LogoWithOutBackground.png"), 50, 50));

				//ticketIconView.Image = UIImage.FromBundle(ticketColor);
				branchLabel.Text = ticket.merchant.BRANCH_NAME;
				companyLabel.Text = ticket.merchant.COMPANY_NAME;
				ticketNoLabel.Text = ticket.queue_no;
				currentlyServingValue.Text = "";
				currentlyServingLabel.Text = "";
				refreshButton.TintColor = UIColor.White;
				refreshButton.Hidden = true;
				ticketDetailsTable.TableFooterView = new UIView (CGRect.Empty);
				ticketDetailsTable.Source = new TicketDetailsSource (options.ToArray (), this);
				ticketDetailsTable.ReloadData ();
				if (ticket.type.Equals ("RESERVATION")) {
					AppDelegate.allowRunBGTasks = true;
					runGetCurrentServingByTranTask ();
				}
				Console.WriteLine ("Timer started, control is back here");
			});

			refreshButton.TouchUpInside += async (object sender, EventArgs e) => {
				Console.WriteLine ("refresh button was pressed");
				getCurrentServingByTran ();
				refreshButton.Hidden = true;
				await Task.Delay (20000);
				refreshButton.Hidden = false;
			};

		}

		async void runGetCurrentServingByTranTask ()
		{
			while (AppDelegate.allowRunBGTasks) {
				getCurrentServingByTran ();
				await Task.Delay (20000); //change to 20-30 secs
			}
		}

		async void getCurrentServingByTran ()
		{
			
			Console.WriteLine (">> GetCurrentServing Tran : " + ticket.tran_id_local + " - " + ticket.tran_type_name);
			string url = "";

			if (ticket.merchant.edition.Equals ("CLOUD-SAAS")) {
				url = String.Format (ticket.merchant.serviceURL	+ "/kioskJSON.svc/getCurrentServingByTranMobileJSON/{0}/{1}/{2}/", ticket.tran_id_local, ticket.merchant.COMPANY_NO, ticket.merchant.BRANCH_NO);
			} else {
				url = String.Format (ticket.merchant.serviceURL	+ "/kioskJSON.svc/getCurrentServingByTranMobileJSON/{0}", ticket.tran_type_name);
			}
			

			try {
				HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create (new Uri (url));
				request.ContentType = "application/json";
				request.Method = "GET";
				using (HttpWebResponse response = await request.GetResponseAsync () as HttpWebResponse) {
					if (response.StatusCode != HttpStatusCode.OK) {
						Console.Out.WriteLine ("Error fetching data. Server returned status code: {0}", response.StatusCode);
						new UIAlertView ("No Internet", "We can't seem to connect to the internet.", null, "OK", null).Show ();
					} else {
						using (StreamReader reader = new StreamReader (response.GetResponseStream ())) {
							var content = reader.ReadToEnd ();
							if (string.IsNullOrWhiteSpace (content)) {
								Console.Out.WriteLine ("Response contained empty body...");
							} else {
								Console.Out.WriteLine ("Response Body: \r\n {0}", content);
								currentServing = new TFCurrentServingResponse ();
								currentServing = JsonConvert.DeserializeObject<TFCurrentServingResponse> (content);
								Console.WriteLine (currentServing.getCurrentServingByTranMobileJSONResult.ResponseCode);
								Console.WriteLine (currentServing.getCurrentServingByTranMobileJSONResult.ResponseMessage);

								InvokeOnMainThread (async () => {
									Console.WriteLine ("Counter : " + currentServing.getCurrentServingByTranMobileJSONResult.CurrentServing.counterNo);
									Console.WriteLine ("Queue No : " + currentServing.getCurrentServingByTranMobileJSONResult.CurrentServing.queueNo);
									Console.WriteLine ("tranType : " + currentServing.getCurrentServingByTranMobileJSONResult.CurrentServing.tranType);
									//refreshButton.Hidden = false;
									currentlyServingLabel.Text = "Currently Serving";
									currentlyServingValue.Text = 
										String.IsNullOrEmpty (currentServing.getCurrentServingByTranMobileJSONResult.CurrentServing.queueNo) ? "None" : currentServing.getCurrentServingByTranMobileJSONResult.CurrentServing.queueNo;
								});

							}
						}
					}
				}
			} catch (Exception e) {
				new UIAlertView ("No Internet", "We can't seem to connect to the internet.", null, "OK", null).Show ();
				Console.WriteLine ("Problem loading Mobile Visitors Count...");
				Console.WriteLine (e.Message);
				Console.WriteLine (e.StackTrace);
			}
			//---------------------------------------------------------
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			if (ticket.type.Equals ("RESERVATION")) {
				AppDelegate.allowRunBGTasks = true;
			}
		}

		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);
		}

		public override void ViewWillDisappear (bool animated)
		{
			//this.mapMainView.RemoveObserver (this, new NSString ("myLocation"));
			AppDelegate.allowRunBGTasks = false;
			Console.WriteLine ("STOPPED >> GetCurrentServing Tran : " + ticket.tran_id_local + " - " + ticket.tran_type_name);
			base.ViewWillDisappear (animated);
		}

		public override void ViewDidDisappear (bool animated)
		{
			//myLoc.StopUpdatingLocation ();
			base.ViewDidDisappear (animated);
		}

		static UIImage FromURL (string uri)
		{
			using (var url = new NSUrl (uri))
			using (var data = NSData.FromUrl (url))
				return UIImage.LoadFromData (data);
		}

		#endregion
	}
}
