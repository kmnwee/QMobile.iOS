using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;
using System.Collections.Generic;
using TimesSquare.iOS;
using CoreGraphics;
using System.Net;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Linq;
using System.Globalization;
using CoreLocation;

namespace QMobile
{
	partial class AppointmentViewController : UIViewController
	{
		public TFMerchants merchant;

		public List<TFProcessOption> options;
		LoadingOverlay _loadPop;
		public TFAvailableSchedule availableSchedResultJSON;
		public TFTransactionTypes transactionTypesJSON;
		public string action;

		//For Process Values
		public string schedDate;
		public string schedTimeKey;
		public string schedTimeString;
		public string schedTranType;
		public string schedTranTypeId;
		public string reservMobileNo;
		public CLLocationCoordinate2D userLocation;

		public AppointmentViewController (IntPtr handle) : base (handle)
		{
		}

		#region View lifecycle

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
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


			Console.WriteLine ("Merchant ID " + merchant.COMPANY_NO + " " + merchant.BRANCH_NO + ">" + merchant.serviceURL);
			branchDetailsView.BackgroundColor = TFColor.FromHexString ("#0097a9", 1.0f);
			companyLabel.Text = merchant.COMPANY_NAME + " | " + merchant.BRANCH_NAME;
			if (action.Equals ("APPOINTMENT"))
				branchLabel.Text = "Schedule an Appointment";
			else if (action.Equals ("RESERVATION"))
				branchLabel.Text = "Reserve a Ticket";
			
			DateTime initDate = new DateTime ();

			if(merchant.schedReserve_sameDay)
				initDate = DateTime.Now;
			else
				initDate = DateTime.Now.AddDays(1);
				

//			TFProcessOption option1 = new TFProcessOption ();
//			TFProcessOption option2 = new TFProcessOption ();
//			TFProcessOption option3 = new TFProcessOption ();
//			TFProcessOption option4 = new TFProcessOption ();
//			options = new List<TFProcessOption> ();

			//initialize--------------------------------------------- 
			//Date
			if (action.Equals ("APPOINTMENT")) {
				switch (merchant.schedReserveWeekend_flag) {
				case 0:
					if (initDate.DayOfWeek == DayOfWeek.Saturday)
						initDate = initDate.AddDays (2);
					else if (initDate.DayOfWeek == DayOfWeek.Sunday)
						initDate = initDate.AddDays (1);
					break;
				case 1:
					if (initDate.DayOfWeek == DayOfWeek.Sunday)
						initDate = initDate.AddDays (1);
					break;
				case 2:
					if (initDate.DayOfWeek == DayOfWeek.Saturday)
						initDate = initDate.AddDays (1);
					break;
				default:
					break;
				}
			}
			schedDate = initDate.ToString ("yyyy-MM-dd");

			getTransactionTypes (); //add checking of trantypes here.

		}

		public override void ViewDidLayoutSubviews ()
		{
			base.ViewDidLayoutSubviews ();

			if (appointmentOptionsTable != null) {
				appointmentOptionsTable.ContentInset = new UIEdgeInsets (0, 0, 0, 0);
			}

		}

		public void initializeView ()
		{
			TFProcessOption option1 = new TFProcessOption ();
			TFProcessOption option2 = new TFProcessOption ();
			TFProcessOption option3 = new TFProcessOption ();
			TFProcessOption option4 = new TFProcessOption ();
			options = new List<TFProcessOption> ();

			//---------------------------------------------------------
			if (action.Equals ("APPOINTMENT"))
				option1.title = "Appointment Date";
			else if (action.Equals ("RESERVATION"))
				option1.title = "Date";
			option1.valueDisplay = DateTime.ParseExact (schedDate, "yyyy-MM-dd", CultureInfo.InvariantCulture).ToString ("ddd, MMM dd");
			option1.type = "Date";
			option1.value = schedDate;
			options.Add (option1);

			if (action.Equals ("APPOINTMENT")) {
				option2.title = "Time Slot";
				option2.valueDisplay = schedTimeString;//should be 1st available time slot
				option2.type = "Time";
				option2.value = schedTimeKey;
				options.Add (option2);
			} else if (action.Equals ("RESERVATION")) {
				option2.title = "Mobile No";
				option2.valueDisplay = "Optional";//should be 1st available time slot
				option2.type = "Mobile";
				option2.value = option2.valueDisplay;
				options.Add (option2);
			}

			option3.title = "Service Type";
			option3.valueDisplay = schedTranType;//should be 1st trantype from json
			option3.type = "Transaction";
			option3.value = option1.valueDisplay;//same as value display
			option3.valueId = schedTranTypeId; //tranype id
			options.Add (option3);

			option4.title = "Proceed";
			option4.valueDisplay = "";
			option4.type = "Proceed";
			option4.value = "";
			options.Add (option4);

			InvokeOnMainThread (() => {
				//this.NavigationItem.TitleView = new UIImageView (UIImage.FromBundle ("iconx30.png"));
				this.NavigationItem.TitleView = new UIImageView (FeaturedTableSource.MaxResizeImage(UIImage.FromBundle ("LogoWithOutBackground.png"), 50, 50));
				appointmentOptionsTable.TableFooterView = new UIView (CGRect.Empty);
				appointmentOptionsTable.Source = new AppointmentOptionsSource (options.ToArray (), merchant, this);
				appointmentOptionsTable.ReloadData ();
			});
		}

		public async void getTransactionTypes ()
		{
			//Tran Type
			//------------------------------------------
			try {
				string urlTranTypes = String.Format (merchant.serviceURL + "/kioskJSON.svc/getAllTranTypesMobileJSON");
				if (merchant.edition.Equals ("CLOUD-SAAS"))
					urlTranTypes += String.Format ("/{0}/{1}/", merchant.COMPANY_NO, merchant.BRANCH_NO);

				Console.WriteLine (urlTranTypes);
				HttpWebRequest requestTranTypes = (HttpWebRequest)HttpWebRequest.Create (new Uri (urlTranTypes));
				requestTranTypes.ContentType = "application/json";
				requestTranTypes.Method = "GET";
				using (HttpWebResponse responseTranTypes = await requestTranTypes.GetResponseAsync () as HttpWebResponse) {
					if (responseTranTypes.StatusCode != HttpStatusCode.OK) {
						Console.Out.WriteLine ("Error fetching data. Server returned status code: {0}", responseTranTypes.StatusCode);
						//Alert here "Problem connecting to the internet...
						new UIAlertView ("Problem Connecting", "We can't seem to connect to the internet.", null, "OK", null).Show ();
					} else {
						using (StreamReader reader = new StreamReader (responseTranTypes.GetResponseStream ())) {
							var content = reader.ReadToEnd ();
							if (string.IsNullOrWhiteSpace (content)) {
								Console.Out.WriteLine ("Response contained empty body...");
							} else {
								Console.Out.WriteLine ("Response Body: \r\n {0}", content);
								transactionTypesJSON = new TFTransactionTypes ();
								transactionTypesJSON = JsonConvert.DeserializeObject<TFTransactionTypes> (content);
								Console.WriteLine (transactionTypesJSON.getAllTranTypesMobileJSONResult.ResponseCode);
								Console.WriteLine (transactionTypesJSON.getAllTranTypesMobileJSONResult.ResponseCode);
								if (transactionTypesJSON.getAllTranTypesMobileJSONResult.TransactionTypes.Any ()) {
									schedTranType = transactionTypesJSON.getAllTranTypesMobileJSONResult.TransactionTypes.First ().tranType;
									schedTranTypeId = transactionTypesJSON.getAllTranTypesMobileJSONResult.TransactionTypes.First ().tran_id_local;
								}
							}
						}
					}
				}
			} catch (Exception e) {
				new UIAlertView ("Problem Connecting", "We can't seem to connect to the internet.", null, "OK", null).Show ();
			}



			if (!action.Equals ("APPOINTMENT")) {
				initializeView ();
				this._loadPop.Hide ();
			} else {
				availableSchedResultJSON = new TFAvailableSchedule ();
				if (!String.IsNullOrEmpty (schedTranType)) {
					if (action.Equals ("APPOINTMENT")) {
						updateAvailableSched ();
					}
				}	
			}

		}

		public async void updateAvailableSched ()
		{
			try {
				string url = String.Format ("http://tfsmsgatesit.azurewebsites.net/TFGatewayJSON.svc/getTFMerchantAvailableSchedule/{0}/{1}/{2}/{3}/{4}/", merchant.COMPANY_NO, merchant.BRANCH_NO, schedDate, "ID", String.IsNullOrEmpty (schedTranTypeId) ? "0" : schedTranTypeId);
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
								availableSchedResultJSON = new TFAvailableSchedule ();
								availableSchedResultJSON = JsonConvert.DeserializeObject<TFAvailableSchedule> (content);
								Console.WriteLine (availableSchedResultJSON.getMerchantAvailableScheduleResult.ResponseCode);
								Console.WriteLine (availableSchedResultJSON.getMerchantAvailableScheduleResult.ResponseMessage);
								if (availableSchedResultJSON.getMerchantAvailableScheduleResult.availableSched.Any ()) {
									schedTimeKey = availableSchedResultJSON.getMerchantAvailableScheduleResult.availableSched.First ().schedKey;
									schedTimeString = availableSchedResultJSON.getMerchantAvailableScheduleResult.availableSched.First ().schedString;
								}
							}
						}
					}
				}
			} catch (Exception e) {
				new UIAlertView ("Problem Connecting", "We can't seem to connect to the internet.", null, "OK", null).Show ();
			}

			initializeView ();
			this._loadPop.Hide ();

		}


		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
//			//------LOADING Screen--------------------------
//			// Determine the correct size to start the overlay (depending on device orientation)
//			var bounds = UIScreen.MainScreen.Bounds; // portrait bounds
//			if (UIApplication.SharedApplication.StatusBarOrientation == UIInterfaceOrientation.LandscapeLeft || UIApplication.SharedApplication.StatusBarOrientation == UIInterfaceOrientation.LandscapeRight) {
//				bounds.Size = new CGSize (bounds.Size.Height, bounds.Size.Width);
//			}
//			// show the loading overlay on the UI thread using the correct orientation sizing
//			this._loadPop = new LoadingOverlay (bounds);
//			this.View.Add (this._loadPop);
//			//------LOADING Screen--------------------------

		}

		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);
			//------LOADING Screen END----------------------
			this._loadPop.Hide ();
			//------LOADING Screen END----------------------
		}

		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);
		}

		public override void ViewDidDisappear (bool animated)
		{
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
